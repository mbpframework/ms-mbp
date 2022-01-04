using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper
{
    public interface IMapFrom<in TSource>
    {
        void MapFrom(TSource source);
    }
}
