using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mbp.AspNetCore.Convention;
using Mbp.Modular.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Mbp.AspNetCore.Api
{
    /// <summary>
    /// 发现Controller
    /// </summary>
    internal class MbpConventionalControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            // 继承接口IAppService 公开的非抽象且非泛型方法
            if (!typeof(IAppService).IsAssignableFrom(type) 
                || !typeInfo.IsPublic 
                || typeInfo.IsAbstract 
                || typeInfo.IsGenericType)
            {
                return false;
            }

            return true;
        }
    }
}
