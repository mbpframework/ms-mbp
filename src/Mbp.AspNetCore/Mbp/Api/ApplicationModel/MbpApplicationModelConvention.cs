using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Modular.Reflection;
using Mbp.Utils;
using Mbp.AspNetCore.Convention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mbp.Internal.Extensions;

namespace Mbp.AspNetCore.Api
{
    /// <summary>
    /// 应用模型自定义，Mbp的应用服务和Asp.net Core本身自带的控制器也会得到增强（不增强功能性）的改进。
    /// </summary>
    internal class MbpApplicationModelConvention : IApplicationModelConvention
    {
        private readonly IServiceCollection _services;
        private readonly IOptions<WebModuleOptions> _options = null;
        private List<ActionModel> _removeList = new List<ActionModel>();
        private bool _isNeedHttpMethodAttribute = false;

        public MbpApplicationModelConvention(IServiceCollection services, IOptions<WebModuleOptions> options)
        {
            this._services = services;
            _options = options;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var type = controller.ControllerType.AsType();

                var dynamicWebApiAttr = ReflectionHelper.GetSingleAttributeOrDefaultByFullSearch<MbpApiAttribute>(type.GetTypeInfo());

                // 自定义的应用服务
                if (typeof(IAppService).GetTypeInfo().IsAssignableFrom(type))
                {
                    controller.ControllerName = controller.ControllerName.RemovePostFix(ApplicationServiceConsts.ControllerPostfixes.ToArray());
                    ConfigureApplicationService(controller, dynamicWebApiAttr);
                }
                else
                {
                    // Web Api控制器
                    if (dynamicWebApiAttr != null)
                    {
                        ConfigureApplicationService(controller, dynamicWebApiAttr);
                    }
                }

                // 移除不满足规范的Action
                _removeList.ForEach(r => controller.Actions.Remove(r));

            }
        }

        protected void ConfigureApplicationService(ControllerModel controller, MbpApiAttribute controllerAttr)
        {
            ConfigureApiExplorer(controller);
            ConfigureSelector(controller, controllerAttr);
            ConfigureParameters(controller);
        }

        protected void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                if (!IsStandardApi(action))
                {
                    _removeList.Add(action);
                    continue;
                }

                foreach (var para in action.Parameters)
                {
                    if (para.BindingInfo != null)
                    {
                        continue;
                    }

                    if (!InternalTypeUtil.IsPrimitiveExtendedIncludingNullable(para.ParameterInfo.ParameterType))
                    {
                        if (CanUseFormBodyBinding(action, para))
                        {
                            para.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                        }
                    }
                }
            }
        }

        protected bool CanUseFormBodyBinding(ActionModel action, ParameterModel parameter)
        {
            if (ApplicationServiceConsts.FormBodyBindingIgnoredTypes.Any(t => t.IsAssignableFrom(parameter.ParameterInfo.ParameterType)))
            {
                return false;
            }

            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                {
                    continue;
                }

                foreach (var actionConstraint in selector.ActionConstraints)
                {
                    var httpMethodActionConstraint = actionConstraint as HttpMethodActionConstraint;
                    if (httpMethodActionConstraint == null)
                    {
                        continue;
                    }

                    if (httpMethodActionConstraint.HttpMethods.All(hm => hm.IsIn("GET", "DELETE", "TRACE", "HEAD")))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected void ConfigureApiExplorer(ControllerModel controller)
        {
            if (controller.ApiExplorer.GroupName.IsNullOrEmpty())
            {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }

            if (controller.ApiExplorer.IsVisible == null)
            {
                controller.ApiExplorer.IsVisible = true;
            }

            foreach (var action in controller.Actions)
            {
                if (!IsStandardApi(action))
                {
                    _removeList.Add(action);
                    continue;
                }

                ConfigureApiExplorer(action);
            }
        }

        protected void ConfigureApiExplorer(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible == null)
            {
                action.ApiExplorer.IsVisible = true;
            }
        }

        protected void ConfigureSelector(ControllerModel controller, MbpApiAttribute controllerAttr)
        {
            RemoveEmptySelectors(controller.Selectors);

            if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            {
                return;
            }

            var rootPath = string.Empty;

            if (controllerAttr != null)
            {
                rootPath = controllerAttr.Module;
            }

            foreach (var action in controller.Actions)
            {
                if (!IsStandardApi(action))
                {
                    _removeList.Add(action);
                    continue;
                }

                ConfigureSelector(rootPath, controller.ControllerName, action);
            }
        }

        protected void ConfigureSelector(string rootPath, string controllerName, ActionModel action)
        {
            RemoveEmptySelectors(action.Selectors);

            var nonAttr = ReflectionHelper.GetSingleAttributeOrDefault<NoneMbpApiAttribute>(action.ActionMethod);

            if (nonAttr != null)
            {
                return;
            }

            if (!action.Selectors.Any())
            {
                AddAppServiceSelector(rootPath, controllerName, action);
            }
            else
            {
                NormalizeSelectorRoutes(rootPath, controllerName, action);
            }
        }

        protected void AddAppServiceSelector(string rootPath, string controllerName, ActionModel action)
        {
            string verb = GetConventionalVerbForMethodName(action);

            var appServiceSelectorModel = new SelectorModel
            {
                AttributeRouteModel = CreateActionRouteModel(rootPath, controllerName, action)
            };

            appServiceSelectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { verb }));

            action.Selectors.Add(appServiceSelectorModel);
        }

        protected string GetRestFulActionName(string actionName)
        {
            // 如果不启用Restful风格
            if (!_options.Value.IsRestful)
                return actionName;


            // Remove Postfix
            actionName = actionName.RemovePostFix(ApplicationServiceConsts.ActionPostfixes.ToArray());

            return actionName;

            // Remove Prefix
            //var verbKey = actionName.GetPascalOrCamelCaseFirstWord().ToLower();
            //if (ApplicationServiceConsts.HttpVerbs.ContainsKey(verbKey))
            //{
            //    if (actionName.Length == verbKey.Length)
            //    {
            //        return "";
            //    }
            //    else
            //    {
            //        return actionName.Substring(verbKey.Length);
            //    }
            //}
            //else
            //{
            //    return actionName;
            //}
        }

        protected void NormalizeSelectorRoutes(string rootPath, string controllerName, ActionModel action)
        {
            foreach (var selector in action.Selectors)
            {
                // 防止正常的控制器类没有指定 HttpMethod，根据约定加上
                string verb = GetConventionalVerbForMethodName(action);

                if (!selector.ActionConstraints.OfType<HttpMethodActionConstraint>().Any())
                {
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { verb }));
                }

                // 没有指定路由会同Mbp的应用服务一起被定制 否则会使用指定的路由
                selector.AttributeRouteModel = selector.AttributeRouteModel == null ?
                    CreateActionRouteModel(rootPath, controllerName, action) :
                    AttributeRouteModel.CombineAttributeRouteModel(CreateActionRouteModel(rootPath, controllerName), selector.AttributeRouteModel);

            }
        }

        private static string GetConventionalVerbForMethodName(ActionModel action)
        {
            string verb;
            var verbKey = action.ActionName.GetPascalOrCamelCaseFirstWord().ToLower();
            verb = ApplicationServiceConsts.HttpVerbs.ContainsKey(verbKey) ? ApplicationServiceConsts.HttpVerbs[verbKey] : ApplicationServiceConsts.DefaultHttpVerb;
            return verb;
        }

        protected AttributeRouteModel CreateActionRouteModel(string rootPath, string controllerName)
        {
            var routeStr =
               $"{ApplicationServiceConsts.DefaultApiPreFix}/{rootPath}/{controllerName}".Replace("//", "/");

            return new AttributeRouteModel(new RouteAttribute(routeStr));
        }

        protected AttributeRouteModel CreateActionRouteModel(string rootPath, string controllerName, ActionModel action)
        {
            action.ActionName = GetRestFulActionName(action.ActionName);

            var routeStr =
                $"{ApplicationServiceConsts.DefaultApiPreFix}/{rootPath}/{controllerName}/{action.ActionName}".Replace("//", "/");

            var idParameterModel = action.Parameters.FirstOrDefault(p => p.ParameterName == "id");
            if (idParameterModel != null)
            {
                if (InternalTypeUtil.IsPrimitiveExtended(idParameterModel.ParameterType, includeEnums: true))
                {
                    routeStr += "/{id}";
                }
                else
                {
                    var properties = idParameterModel
                        .ParameterType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    foreach (var property in properties)
                    {
                        routeStr += "/{" + property.Name + "}";
                    }
                }
            }

            return new AttributeRouteModel(new RouteAttribute(routeStr));
        }

        protected void RemoveEmptySelectors(IList<SelectorModel> selectors)
        {
            selectors
                .Where(IsEmptySelector)
                .ToList()
                .ForEach(s => selectors.Remove(s));
        }

        protected bool IsEmptySelector(SelectorModel selector)
        {
            return selector.AttributeRouteModel == null
                   && selector.ActionConstraints.IsNullOrEmpty()
                   // 防止Authorize被移除
                   && selector.EndpointMetadata.IsNullOrEmpty();
        }

        // 检查是否满足公司API规范 ，约束层层检查，一个不符合就返回
        private bool IsStandardApi(ActionModel action)
        {
            // 框架强制要求，控制器方法一定要虚方法。
            if (!action.ActionMethod.IsVirtual) return false;

            // 约束一：一定要注明请求方法和路由
            if (_isNeedHttpMethodAttribute)
                return action.Attributes.Any(attr => attr is HttpMethodAttribute);

            // 约束二：

            // 没有进行约束执行 返回true
            return true;
        }
    }
}
