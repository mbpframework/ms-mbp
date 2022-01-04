using Mbp.Ddd.Application.ObjectMapper.AutoMapper;
using Mbp.Framework.Application.Contracts.Dto;
using Mbp.Framework.Domain.Do;

namespace Mbp.Framework.Web
{

    /// <summary>
    /// 类型映射概述文件，用来做类型映射配置 从头Entity--->DTO
    /// </summary>
    public class SampleProfile : MbpEntity2DtoProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public SampleProfile()
        {
            CreateMap<SampleEntity, SampleOutputDto>();
        }
    }

    /// <summary>
    /// 类型映射概述文件，用来做类型映射配置 从DTO--->Entity
    /// </summary>
    public class SampleProfile1 : MbpDto2EntityProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public SampleProfile1()
        {
            CreateMap<SampleInputDto, SampleEntity>();
        }
    }
}
