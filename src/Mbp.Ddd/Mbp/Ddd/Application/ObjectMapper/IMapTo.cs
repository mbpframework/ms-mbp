using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper
{
    public interface IMapTo<TDestination>
    {
        TDestination MapTo();

        void MapTo(TDestination destination);
    }
}
