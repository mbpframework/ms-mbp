using Mbp.Core.User;
using Mbp.Core.User.MultiTenant;
using Mbp.Internal.Extensions;
using Mbp.Modular;
using Mbp.AspNetCore.Api;
using Mbp.AspNetCore.Api.Filters;
using Mbp.AspNetCore.Api.Middleware;
using Mbp.AspNetCore.Permission;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Mbp.AspNetCore
{
    /// <summary>
    /// AspNetCore模块
    /// </summary>
    [DependsOn(typeof(MbpModule), typeof(MbpAspNetModule))]
    public class MbpWebModule : MbpAspNetModule
    {
        /// <summary>
        /// 服务注册之前
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<WebModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Web"));
            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var webOptions = services.BuildServiceProvider().GetService<IOptions<WebModuleOptions>>().Value;
            if (webOptions == null)
            {
                throw new Exception("Web模块配置获取失败！");
            }

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // 不使用驼峰
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                // 设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).AddMvcOptions(options =>
            {
                // 禁用Version的绑定
                options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(System.Version)));

                // 请求响应统一格式处理中间件
                options.Filters.Add(typeof(ResponseMiddlewareAttribute));

                // 设置内容协议
                options.ReturnHttpNotAcceptable = true;

            })
            // 注册到容器中，并在staup时候替换到autofac中
            .AddControllersAsServices();

            // 注入全局HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 配置应用程序模型约定
            ConfigApplicaitonModel(services, new MbpApiOptions());

            // 开启响应压缩
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();

            });

            // 最快压缩
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            // 最快压缩
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            // 创建Cors策略
            services.AddCors(options =>
            {
                options.AddPolicy("MbpCors",
                builder =>
                {
                    builder.WithOrigins(webOptions.AllowedHosts)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // 添加身份认证
            if (!string.IsNullOrEmpty(webOptions.IdentityServer))
            {
                // 使用identityserver认证
                AddAuthenticationJwt(services);
            }

            // 注册当前用户信息
            services.AddScoped<ICurrentUser, CurrentUser>();

            // 注册当前租户信息
            services.AddScoped<ICurrentTenant, CurrentTenant>();

            // 注册授权策略处理程序
            if (!string.IsNullOrEmpty(webOptions.IdentityServer))
            {
                services.AddScoped<IAuthorizationHandler, MbpPermissionHandler>();
            }

            // 注册自定义身份认证服务逻辑
            if (string.IsNullOrEmpty(webOptions.IdentityServer))
            {
                services.Remove(services.FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationService)));
                var authenticationServiceDescriptor = new ServiceDescriptor(typeof(IAuthenticationService), typeof(CustomizeAuthenticationService), ServiceLifetime.Scoped);
                services.Add(authenticationServiceDescriptor);
            }

            // 注册自定义授权服务逻辑
            if (string.IsNullOrEmpty(webOptions.IdentityServer))
            {
                services.Remove(services.FirstOrDefault(d => d.ServiceType == typeof(IAuthorizationService)));
                var authorizationServiceDescriptor = new ServiceDescriptor(typeof(IAuthorizationService), typeof(CustomizeAuthorizationService), ServiceLifetime.Transient);
                services.Add(authorizationServiceDescriptor);
            }

            return base.ConfigureServices(services);
        }

        public override void OnModuleInitialization(IApplicationBuilder app)
        {
            // 启用跨域请求中间件
            app.UseCors("MbpCors");

            // 启用Mbp上下文
            app.UseMiddleware(typeof(MbpProccessMiddleware));

            // 开启响应压缩 Mark:Swagger UI 日志面板 Miniprofiler不启用压缩
            app.UseResponseCompression();

            // Mime管理和文件请求
            UseMimeManage(app);

            // 启用应用服务层全局错误处理中间件
            app.UseMiddleware(typeof(MbpGlobaExceptionMiddleware));

            // 启用多租户中间件
            app.UseMiddleware(typeof(MbpMultiTenantMiddleware));

            base.OnModuleInitialization(app);
        }

        // 定制应用程序模型
        private void ConfigApplicaitonModel(IServiceCollection services, MbpApiOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException(nameof(options));
            }

            options.Valid();

            ApplicationServiceConsts.DefaultAreaName = options.DefaultAreaName;
            ApplicationServiceConsts.DefaultHttpVerb = options.DefaultHttpVerb;
            ApplicationServiceConsts.DefaultApiPreFix = options.DefaultApiPrefix;
            ApplicationServiceConsts.ControllerPostfixes = options.RemoveControllerPostfixes;
            ApplicationServiceConsts.ActionPostfixes = options.RemoveActionPostfixes;
            ApplicationServiceConsts.FormBodyBindingIgnoredTypes = options.FormBodyBindingIgnoredTypes;

            var partManager = services.GetSingletonInstanceOrNull<ApplicationPartManager>();

            if (partManager == null)
            {
                throw new InvalidOperationException("\"ConfigApplicaitonModel\" 必须在调用 \"AddControllers/AddMvc\"之后.");
            }

            // 自定义控制器检查提供程序
            partManager.FeatureProviders.Add(new MbpConventionalControllerFeatureProvider());

            services.Configure<MvcOptions>(o =>
            {
                // 添加Mbp应用程序模型约定
                o.Conventions.Add(new MbpApplicationModelConvention(services, services.BuildServiceProvider().GetService<IOptions<WebModuleOptions>>()));
            });
        }

        // 身份验证
        private void AddAuthenticationJwt(IServiceCollection services)
        {
            var gConfig = services.BuildServiceProvider().GetService<IOptions<WebModuleOptions>>().Value;
            //var secret = gConfig.PublicKey;
            //var data = Encoding.UTF8.GetBytes(secret);
            //var credential = new SigningCredentials(new X509SecurityKey(new X509Certificate2(data)), "RS256");

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = gConfig.IdentityServer;
                options.RequireHttpsMetadata = gConfig.RequireHttpsMetadata;

                // rs256 jwt token 验证逻辑设置
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,// 验证token颁发者
                    ValidateLifetime = true,// 验证token 过期时间
                    ClockSkew = TimeSpan.FromSeconds(0),// 过期偏移时间默认是5分钟，设置成没有偏移时间
                    //ValidateIssuerSigningKey = true,// 验证token签名公钥
                    //IssuerSigningKey = credential.Key,// 指示token签名公钥
                };

                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("action", "timeOut");

                            context.Response.OnStarting(async () =>
                            {
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { state = 4001, message = "Token过期", version = 1, content = new List<object>() }));
                            });
                        }
                        else if (context.Exception.GetType() == typeof(CryptographicException))
                        {
                            context.Response.OnStarting(async () =>
                            {
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { state = 4001, message = "无效的公钥", version = 1, content = new List<object>() }));
                            });
                        }
                        else if (context.Exception.GetType() == typeof(SecurityTokenInvalidSignatureException))
                        {
                            context.Response.OnStarting(async () =>
                            {
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { state = 4001, message = "非法Token", version = 1, content = new List<object>() }));
                            });
                        }
                        else if (context.Exception.GetType() == typeof(Exception))
                        {
                            context.Response.OnStarting(async () =>
                            {
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { state = 4001, message = "未知错误", version = 1, content = new List<object>() }));
                            });
                        }
                        return Task.CompletedTask;
                    },
                    // token验证成功
                    OnTokenValidated = context =>
                    {
                        // 判断是否在网关处进行身份认证，认证通过之后可以直接获取相应的参数
                        if (context.HttpContext.Request.Headers.TryGetValue("Authorized", out StringValues isAuthorized) && isAuthorized == "1")
                        {
                            var userId = context.HttpContext.Request.Headers["UserId"];
                            var userName = context.HttpContext.Request.Headers["UserName"];
                            var loginName = context.HttpContext.Request.Headers["LoginName"];
                            var token = context.HttpContext.Request.Headers["Authorization"].ToString().RemovePreFix("Bearer ");

                            // 解析用户信息
                            context.HttpContext.RequestServices.GetService<ICurrentUser>().ReloveUser(userId, userName, loginName, token);
                        }
                        else
                        {
                            // 解析用户信息
                            context.HttpContext.RequestServices.GetService<ICurrentUser>().ReloveUser(context);
                        }

                        return Task.CompletedTask;
                    },
                    // 鉴权过程失败
                    OnChallenge = context =>
                      {
                          var c = context;
                          return Task.CompletedTask;
                      },
                    // 权限不足
                    OnForbidden = context =>
                    {
                        return Task.CompletedTask;
                    },
                    // 接收token 可以自定义token取值来源
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // Mime类型管理
        private static void UseMimeManage(IApplicationBuilder app)
        {
            var webModuleOptions = app.ApplicationServices.GetService<IOptions<WebModuleOptions>>()?.Value;

            var provider = new FileExtensionContentTypeProvider();

            if (webModuleOptions != null)
            {
                foreach (var mime in webModuleOptions.Mimes)
                {
                    // 删除Mime
                    if (mime.Value == "removed")
                    {
                        provider.Mappings.Remove(mime.Key);
                        continue;
                    }

                    // 添加MiMe
                    provider.Mappings[mime.Key] = mime.Value;
                }
            }

            // 配置静态文件目录
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "FileRoot")),
                RequestPath = "/files",
                ContentTypeProvider = provider
            });
        }
    }
}
