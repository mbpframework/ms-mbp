using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Modular;
using Mbp.Ddd.Application.ObjectMapper;
using Mbp.Ddd.Application.ObjectMapper.AutoMapper;
using Mbp.Ddd.Application.Uow;

using System;

namespace Mbp.Ddd
{
    /// <summary>
    /// DDD设计支撑模块
    /// </summary>
    [DependsOn(typeof(MbpModule))]
    public class MbpDddModule : MbpModule
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 因为UnitOfWorkManager共享了UnitOfWork，为了避免并发出现资源争夺问题，取消了线程内共享。
            // 两种方式处理顺序错乱提交问题
            // 1.使用AddScoped<IUnitOfWorkManager, UnitOfWorkManager>()注册
            // 2.使用AsynctLocal来定义IUnitOfWork，我们采用第二种方式，共享工作单元管理器
            services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 使用其他对象映射组件，需在这里进行注册
            // 注册AutoMapper IMapper访问对象
            var mapperAccessor = new MapperAccessor();
            services.AddSingleton<IMapperAccessor>(_ => mapperAccessor);
            services.AddSingleton<MapperAccessor>(_ => mapperAccessor);

            // 注册AutoMapper提供程序
            services.AddTransient<IAutoObjectMappingProvider, AutoMapperAutoObjectMappingProvider>();
            services.AddTransient(typeof(IAutoObjectMappingProvider<>), typeof(AutoMapperAutoObjectMappingProvider<>));

            // 注册对象映射服务
            services.AddTransient(typeof(INgObjectMapper<>), typeof(DefaultObjectMapper<>));

            return services;
        }

        /// <summary>
        /// 启动Ddd模块
        /// </summary>
        /// <param name="provider"></param>
        public override void OnModuleInitialization(IServiceProvider provider)
        {
            // 注册对象映射,使用automapper，如果使用其他对象映射组件，在此处扩展。
            CreateMappings(provider);
        }

        private void CreateMappings(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<MbpAutoMapperOptions>>().Value;

                void ConfigureAll(IMbpAutoMapperConfigurationContext ctx)
                {
                    foreach (var configurator in options.Configurators)
                    {
                        configurator(ctx);
                    }
                }

                var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
                {
                    ConfigureAll(new MbpAutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider));
                });

                scope.ServiceProvider.GetRequiredService<MapperAccessor>().Mapper = mapperConfiguration.CreateMapper();
            }
        }
    }
}
