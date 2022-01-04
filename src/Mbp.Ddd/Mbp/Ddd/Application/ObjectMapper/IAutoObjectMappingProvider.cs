using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper
{
    public interface IAutoObjectMappingProvider
    {
        TDestination Map<TSource, TDestination>(object source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }

    public interface IAutoObjectMappingProvider<TContext> : IAutoObjectMappingProvider
    {

    }
}
