using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper.AutoMapper
{
    public interface IMbpAutoMapperConfigurationContext
    {
        IMapperConfigurationExpression MapperConfiguration { get; }

        IServiceProvider ServiceProvider { get; }
    }
}
