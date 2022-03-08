using Ins.Sample.DataAccess.Do;
using Ins.Sample.Service.Dto;
using WuhanIns.Nitrogen.Ddd.Application.ObjectMapper.AutoMapper;

namespace Ins.Sample.Service
{

    /// <summary>
    /// 类型映射概述文件，用来做类型映射配置 从头Entity--->DTO
    /// </summary>
    public class DemoProfile : NitrogenEntity2DtoProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public DemoProfile()
        {
            CreateMap<DemoEntity, DemoOutputDto>();
        }
    }

    /// <summary>
    /// 类型映射概述文件，用来做类型映射配置 从DTO--->Entity
    /// </summary>
    public class DemoProfile1 : NitrogenDto2EntityProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public DemoProfile1()
        {
            CreateMap<DemoInputDto, DemoEntity>();
        }
    }
}
