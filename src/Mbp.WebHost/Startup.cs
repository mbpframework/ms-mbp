using Autofac;
using Autofac.Extras.DynamicProxy;
using Mbp.AspNetCore;
using Mbp.AspNetCore.Convention;
using Mbp.Config.Apollo;
using Mbp.Core;
using Mbp.DataAccess;
using Mbp.Ddd;
using Mbp.Discovery;
using Mbp.Internal.Extensions;
using Mbp.Logging;
using Mbp.Modular;
using Mbp.Net;
using Mbp.WebHost;
using Mbp.WebHost.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mbp.WebHost
{
    /// <summary>
    /// 启动类，此类启动顺序 ConfigureServices(各种注册) ---> ConfigureContainer(各种注册，目标autofac) ---> Configure(各种管道中间件启用)
    /// </summary>
    public class Startup
    {
        private ContainerBuilder _builder;
        private IServiceCollection _services;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 注册框架服务
        public void ConfigureServices(IServiceCollection services)
        {
            // 得到Mbp构造器
            var MbpBuilder = services.AddMbp();

            // 默认加载框架必须的
            MbpBuilder
                .AddModule<MbpWebModule>()
                .AddModule<MbpLoggerModule>()
                .AddModule<MbpDiscoveryModule>()
                .AddModule<MbpDataAccessModule>()
                .AddModule<MbpNetModule>()
                .AddModule<MbpDddModule>()
                .AddModule<MbpApolloModule>()
                .AddModule<MbpWebHostModule>()
                ;

            // 动态挂载模块 MbpCachingModule MbpEventBusModule MbpDiscoveryModule MbpApolloModule
            var modulesSection = Configuration.GetSection("Mbp:Modules");
            if (modulesSection.Exists())
            {
                foreach (var moduleSection in modulesSection.GetChildren())
                {
                    MbpBuilder.AddModule(new InstanceFactory().Create<MbpModule>(moduleSection.Value));
                }
            }

            MbpBuilder.StartBuild();
            _services = services;

            // 构建框架层模块，暂时只做服务注册
            ModuleBuilder.Instance.Init(_services).BuildMbpFalsework();

            // 构造业务系统模块
            ModuleBuilder.Instance.Init(_services).BuildApplicationFalsework();

            // 运行状况检查
            services.AddHealthChecks();

            // Swagger
            AddSwagger(services);
        }

        // Autofac 注册，在ConfigureServices之后，基础框架注册之后进行 
        //【注意】在Autofac替换DI之前，需要把所有模块注册完成。否则将出现无法注册EF CORE等情况。
        public void ConfigureContainer(ContainerBuilder builder)
        {
            ILoggerFactory factory = _services.BuildServiceProvider().GetService<ILoggerFactory>();
            var _logger = factory.CreateLogger("Startup");

            // 注册二开执行插件
            builder.RegisterType<CustomizePlugin>();

            _builder = builder;

            // 代办：扫描业务产品
            // 运行时目录
            var runtimePath = AppContext.BaseDirectory;

            // 获取扫描目录
            var directories = Directory.GetDirectories(runtimePath, "Mbp.*");

            // 扫描Mbp.*的文件目录，挂载Mbp产品
            foreach (var directoryPath in directories)
            {
                var systemName = Path.GetFileName(directoryPath);
                _logger.LogInformation($"===>开始代理业务系统 “{systemName}” ");
                Stopwatch watch = Stopwatch.StartNew();
                var appServiceFiles = Directory.GetFiles(directoryPath, $"{systemName}.Application.dll").Union(Directory.GetFiles(directoryPath, $"{systemName}.Service.dll")).ToArray();
                foreach (var appService in appServiceFiles)
                {
                    var applicatonAssembly = Assembly.LoadFrom(Path.Combine(directoryPath, appService));

                    // 实现IAppService的类型都会被扫入autofac
                    builder.RegisterAssemblyTypes(applicatonAssembly)
                              .Where(t => t.Name.EndsWith("AppService")
                              && typeof(IAppService).IsAssignableFrom(t)
                              )
                              .EnableClassInterceptors()
                              .InterceptedBy(typeof(CustomizePlugin));
                }

                watch.Stop();
                _logger.LogInformation($"<===业务系统 “{systemName}” 代理完成，耗时：{watch.ElapsedMilliseconds} 毫秒");
            }

            // 代办:扫描二开项目 注入到autofac，也可以选择跟产品方法那样的方式注册到内置容器
        }

        // 启用框架
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appHostLifetime, IOptions<WebModuleOptions> webOptions)
        {
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("Mbp.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            appHostLifetime.ApplicationStarted.Register(() =>
            {
            });

            appHostLifetime.ApplicationStopping.Register(() =>
            {
            });

            appHostLifetime.ApplicationStopped.Register(() =>
            {
            });

            // 启用Mbp框架
            app.UseMbp();

            // UseSwagger
            UseSwagger(app);

            // 路由中间件
            app.UseRouting();

            // 身份认证中间件
            if (!string.IsNullOrEmpty(webOptions.Value.IdentityServer))
            {
                app.UseAuthentication();
            }

            // 授权
            app.UseAuthorization();

            // 路由终结点配置 开启终结点之后,Mbp的权限过滤器将以中间件的形式独立运行,不会再添加到ActionDescriptor 
            // 也就是说,我们不要选择以这种方式来拦截和自定义鉴权算法
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                {
                    AllowCachingResponses = true
                });
                endpoints.MapControllers();
            });

            app.UseGrpcService();
        }

        public void AddSwagger(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IOptions<WebModuleOptions>>().Value;

            services.Configure<OpenApiInfo>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("SwaggerOption"));

            // 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? ($"{methodInfo.DeclaringType.Name.Replace("AppService", string.Empty)}_{methodInfo.Name}") : null;
                });

                options.SwaggerDoc(configuration.Version, new OpenApiInfo() { Title = configuration.Title, Version = configuration.Version });

                // 代办:一定要返回true！
                options.DocInclusionPredicate((docName, description) =>
                {
                    return true;
                });
                // 代办 swagger UI加入注释
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml").ToList().ForEach(file =>
                {
                    options.IncludeXmlComments(file);
                });

                Directory.GetDirectories(AppContext.BaseDirectory, "Mbp.*").ToList().ForEach(folder =>
                {
                    Directory.GetFiles(folder, "*.xml").ToList().ForEach(file =>
                    {
                        options.IncludeXmlComments(file);
                    });
                });

                // Jwt集成
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token,格式:Bearer {Token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { "readAccess", "writeAccess" }
                    }
                });
            });

        }

        public void UseSwagger(IApplicationBuilder app)
        {
            var webModuleOptions = app.ApplicationServices.GetService<IOptions<WebModuleOptions>>().Value;

            // 启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();

            // 启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.EnableFilter();
                c.ConfigObject.ShowExtensions = true;
                c.SwaggerEndpoint(webModuleOptions.Url, webModuleOptions.Title);
                c.IndexStream = () => GetType().Assembly.GetManifestResourceStream("Mbp.WebHost.index.html");
            });
        }
    }
}
