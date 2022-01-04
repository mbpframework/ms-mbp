using AutoMapper;
using System.Drawing;

namespace Mbp.Ddd.Application.ObjectMapper.AutoMapper
{
    /// <summary>
    /// 默认的映射配置文件，适用于可逆的名称转换规则
    /// </summary>
    public abstract class MbpProfile : Profile
    {
        public MbpProfile()
        {
            // 默认规则
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        }
    }

    /// <summary>
    /// 实体映射为传输模型对象
    /// </summary>
    public class MbpEntity2DtoProfile : MbpProfile
    {
        public MbpEntity2DtoProfile()
        {
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        }
    }

    /// <summary>
    /// 传输模型对象映射成实体
    /// </summary>
    public class MbpDto2EntityProfile : MbpProfile
    {
        public MbpDto2EntityProfile()
        {
            SourceMemberNamingConvention = new PascalCaseNamingConvention();
            DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
        }
    }
}
