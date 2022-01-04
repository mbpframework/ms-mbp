using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Ddd.Application.ObjectMapper;
using Mbp.Ddd.Application.ObjectMapper.AutoMapper;
using Mbp.Test.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Options;
using Mbp.Ddd.Application.Dto;
using Mbp.Ddd.Domain;
using JetBrains.Annotations;

namespace Mbp.Ddd.Test
{
    public class AutoMapper_Test : MbpTestBase
    {
        private readonly INgObjectMapper<AutoMapper_Test> _objectMapper = null;

        public AutoMapper_Test()
        {
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

            services.Configure<MbpAutoMapperOptions>(options =>
            {
                options.AddMaps<DemoProfile>();
            });

            var provider = services.BuildServiceProvider();

            _objectMapper = provider.GetService<INgObjectMapper<AutoMapper_Test>>();

            CreateMappings(provider);
        }

        [Fact]
        public void Map_Test()
        {
            var entity = new Entity_Test() { Name = "张三", Age = 11 };
            var dto = new Dto_Test() { Name = "李四", Age = 22, };

            var dto_except = _objectMapper.Map<Entity_Test, Dto_Test>(entity);
            dto_except.Name.ShouldBe("张三");

            var entity_except = _objectMapper.Map<Dto_Test, Entity_Test>(dto);
            entity_except.Name.ShouldBe("李四");

            var destinationEntity = new Entity_Test() { Name = "王五", Age = 11 };
            var destinationDto = new Dto_Test() { Name = "赵六", Age = 22 };

            destinationDto.Name.ShouldBe("赵六");
            destinationEntity.Name.ShouldBe("王五");

            _objectMapper.Map<Entity_Test, Dto_Test>(entity, destinationDto).Name.ShouldBe("张三");

            _objectMapper.Map<Dto_Test, Entity_Test>(dto, destinationEntity).Name.ShouldBe("李四");
        }

        [Fact]
        public void Map1_Test()
        {
            var entity = new Entity1_Test() { USER_NAME = "张三", USER_REAL_AGE = 11, ID = 1 };
            var dto = new Dto1_Test() { UserName = "李四", UserRealAge = 22, Id = 2 };

            var dto_except = _objectMapper.Map<Entity1_Test, Dto1_Test>(entity);
            dto_except.UserName.ShouldBe("张三");

            var entity_except = _objectMapper.Map<Dto1_Test, Entity1_Test>(dto);
            entity_except.USER_NAME.ShouldBe("李四");

            var destinationEntity = new Entity1_Test() { USER_NAME = "王五", USER_REAL_AGE = 11 };
            var destinationDto = new Dto1_Test() { UserName = "赵六", UserRealAge = 22 };

            destinationDto.UserName.ShouldBe("赵六");
            destinationEntity.USER_NAME.ShouldBe("王五");

            _objectMapper.Map<Entity1_Test, Dto1_Test>(entity, destinationDto).UserName.ShouldBe("张三");

            _objectMapper.Map<Dto1_Test, Entity1_Test>(dto, destinationEntity).USER_NAME.ShouldBe("李四");
        }

        [Fact]
        public void Map2_Test()
        {
            var entity = new Entity2_Test() { USER_NAME = "张三", USER_REAL_AGE = "11", ID = 1 };
            var dto = new Dto2_Test() { Name = "李四", RealAge = 22, Id = 2 };

            var dto_except = _objectMapper.Map<Entity2_Test, Dto2_Test>(entity);
            dto_except.Name.ShouldBe("张三");

            var entity_except = _objectMapper.Map<Dto2_Test, Entity2_Test>(dto);
            entity_except.USER_NAME.ShouldBe("李四");

            //var destinationEntity = new Entity1_Test() { USER_NAME = "王五", USER_REAL_AGE = 11 };
            //var destinationDto = new Dto1_Test() { UserName = "赵六", UserRealAge = 22 };

            //destinationDto.UserName.ShouldBe("赵六");
            //destinationEntity.USER_NAME.ShouldBe("王五");

            //_objectMapper.Map<Entity1_Test, Dto1_Test>(entity, destinationDto).UserName.ShouldBe("张三");

            //_objectMapper.Map<Dto1_Test, Entity1_Test>(dto, destinationEntity).USER_NAME.ShouldBe("李四");
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

    public class Entity_Test : EntityBase<int>
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Dto_Test : DtoBase
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Entity1_Test : EntityBase<int>
    {
        public string USER_NAME { get; set; }

        public int USER_REAL_AGE { get; set; }
    }

    public class Dto1_Test : DtoBase
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int UserRealAge { get; set; }
    }

    public class Entity2_Test : EntityBase<int>
    {
        public string USER_NAME { get; set; }

        public string USER_REAL_AGE { get; set; }
    }

    public class Dto2_Test : DtoBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RealAge { get; set; }
    }

    public class DemoProfile : MbpEntity2DtoProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public DemoProfile()
        {
            CreateMap<Entity_Test, Dto_Test>();
            CreateMap<Entity1_Test, Dto1_Test>();
            CreateMap<Entity2_Test, Dto2_Test>()
                .ForMember("Name", o =>
                {
                    o.MapFrom(src => src.USER_NAME);
                }).ForMember("RealAge", o =>
                {
                    o.MapFrom(src => src.USER_REAL_AGE);
                });
        }
    }

    public class DemoProfile1 : MbpDto2EntityProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public DemoProfile1()
        {
            CreateMap<Dto_Test, Entity_Test>();
            CreateMap<Dto1_Test, Entity1_Test>();
            CreateMap<Dto2_Test, Entity2_Test>()
                .ForMember(src=>src.USER_NAME, o =>
            {
                o.MapFrom(src => src.Name);
            }).ForMember("USER_REAL_AGE", o =>
            {
                o.MapFrom(src => src.RealAge);
            });
        }
    }
}
