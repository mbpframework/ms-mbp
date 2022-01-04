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
    /// �����࣬��������˳�� ConfigureServices(����ע��) ---> ConfigureContainer(����ע�ᣬĿ��autofac) ---> Configure(���ֹܵ��м������)
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

        // ע���ܷ���
        public void ConfigureServices(IServiceCollection services)
        {
            // �õ�Mbp������
            var MbpBuilder = services.AddMbp();

            // Ĭ�ϼ��ؿ�ܱ����
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

            // ��̬����ģ�� MbpCachingModule MbpEventBusModule MbpDiscoveryModule MbpApolloModule
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

            // ������ܲ�ģ�飬��ʱֻ������ע��
            ModuleBuilder.Instance.Init(_services).BuildMbpFalsework();

            // ����ҵ��ϵͳģ��
            ModuleBuilder.Instance.Init(_services).BuildApplicationFalsework();

            // ����״�����
            services.AddHealthChecks();

            // Swagger
            AddSwagger(services);
        }

        // Autofac ע�ᣬ��ConfigureServices֮�󣬻������ע��֮����� 
        //��ע�⡿��Autofac�滻DI֮ǰ����Ҫ������ģ��ע����ɡ����򽫳����޷�ע��EF CORE�������
        public void ConfigureContainer(ContainerBuilder builder)
        {
            ILoggerFactory factory = _services.BuildServiceProvider().GetService<ILoggerFactory>();
            var _logger = factory.CreateLogger("Startup");

            // ע�����ִ�в��
            builder.RegisterType<CustomizePlugin>();

            _builder = builder;

            // ���죺ɨ��ҵ���Ʒ
            // ����ʱĿ¼
            var runtimePath = AppContext.BaseDirectory;

            // ��ȡɨ��Ŀ¼
            var directories = Directory.GetDirectories(runtimePath, "Mbp.*");

            // ɨ��Mbp.*���ļ�Ŀ¼������Mbp��Ʒ
            foreach (var directoryPath in directories)
            {
                var systemName = Path.GetFileName(directoryPath);
                _logger.LogInformation($"===>��ʼ����ҵ��ϵͳ ��{systemName}�� ");
                Stopwatch watch = Stopwatch.StartNew();
                var appServiceFiles = Directory.GetFiles(directoryPath, $"{systemName}.Application.dll").Union(Directory.GetFiles(directoryPath, $"{systemName}.Service.dll")).ToArray();
                foreach (var appService in appServiceFiles)
                {
                    var applicatonAssembly = Assembly.LoadFrom(Path.Combine(directoryPath, appService));

                    // ʵ��IAppService�����Ͷ��ᱻɨ��autofac
                    builder.RegisterAssemblyTypes(applicatonAssembly)
                              .Where(t => t.Name.EndsWith("AppService")
                              && typeof(IAppService).IsAssignableFrom(t)
                              )
                              .EnableClassInterceptors()
                              .InterceptedBy(typeof(CustomizePlugin));
                }

                watch.Stop();
                _logger.LogInformation($"<===ҵ��ϵͳ ��{systemName}�� ������ɣ���ʱ��{watch.ElapsedMilliseconds} ����");
            }

            // ����:ɨ�������Ŀ ע�뵽autofac��Ҳ����ѡ�����Ʒ���������ķ�ʽע�ᵽ��������
        }

        // ���ÿ��
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

            // ����Mbp���
            app.UseMbp();

            // UseSwagger
            UseSwagger(app);

            // ·���м��
            app.UseRouting();

            // �����֤�м��
            if (!string.IsNullOrEmpty(webOptions.Value.IdentityServer))
            {
                app.UseAuthentication();
            }

            // ��Ȩ
            app.UseAuthorization();

            // ·���ս������ �����ս��֮��,Mbp��Ȩ�޹����������м������ʽ��������,��������ӵ�ActionDescriptor 
            // Ҳ����˵,���ǲ�Ҫѡ�������ַ�ʽ�����غ��Զ����Ȩ�㷨
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

            // ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            services.AddSwaggerGen(options =>
            {
                options.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? ($"{methodInfo.DeclaringType.Name.Replace("AppService", string.Empty)}_{methodInfo.Name}") : null;
                });

                options.SwaggerDoc(configuration.Version, new OpenApiInfo() { Title = configuration.Title, Version = configuration.Version });

                // ����:һ��Ҫ����true��
                options.DocInclusionPredicate((docName, description) =>
                {
                    return true;
                });
                // ���� swagger UI����ע��
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

                // Jwt����
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken,��ʽ:Bearer {Token}",
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

            // �����м����������Swagger��ΪJSON�ս��
            app.UseSwagger();

            // �����м�������swagger-ui��ָ��Swagger JSON�ս��
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
