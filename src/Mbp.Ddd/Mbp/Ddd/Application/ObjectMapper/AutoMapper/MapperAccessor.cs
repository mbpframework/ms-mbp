using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper.AutoMapper
{
    public class MapperAccessor : IMapperAccessor
    {
        public IMapper Mapper { get; set; }
    }
}
