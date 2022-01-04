using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper.AutoMapper
{
    public class MbpAutoMapperConfigurationContext : IMbpAutoMapperConfigurationContext
    {
        public IMapperConfigurationExpression MapperConfiguration { get; }
        public IServiceProvider ServiceProvider { get; }

        public MbpAutoMapperConfigurationContext(
            IMapperConfigurationExpression mapperConfigurationExpression,
            IServiceProvider serviceProvider)
        {
            MapperConfiguration = mapperConfigurationExpression;
            ServiceProvider = serviceProvider;
        }
    }
}
