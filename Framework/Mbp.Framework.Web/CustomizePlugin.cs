using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Mbp.Framework.Web
{
    public class CustomizePlugin : IInterceptor
    {
        /// <summary>
        /// 二开插件核心方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            //var willBeIntercepted = AutofacService.Resolve<CustomizeClass>();

            //var customizeType = willBeIntercepted.GetType();
            //var customizeMethod = customizeType.GetMethod(invocation.Method.Name);

            //customizeMethod.Invoke(willBeIntercepted, invocation.Arguments);

            Console.WriteLine("fsfsdf");

            invocation.Proceed();
        }
    }
}
