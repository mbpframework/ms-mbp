using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.ObjectMapper.AutoMapper
{
    public class MbpAutoMapperOptions
    {
        public List<Action<IMbpAutoMapperConfigurationContext>> Configurators { get; }

        public MbpAutoMapperOptions()
        {
            Configurators = new List<Action<IMbpAutoMapperConfigurationContext>>();
        }

        public void AddMaps<TModule>()
        {
            var assembly = typeof(TModule).Assembly;

            Configurators.Add(context =>
            {
                context.MapperConfiguration.AddMaps(assembly);

                // 因为有不可逆的转换规则，所以不设置全局的名称映射规则 
                // 变更设计到在Profile文件中指定
            });
        }
    }
}
