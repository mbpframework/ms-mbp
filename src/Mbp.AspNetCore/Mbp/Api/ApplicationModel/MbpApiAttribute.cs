using Mbp.Modular.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mbp.AspNetCore.Api
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class MbpApiAttribute : Attribute
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsMetadataEnabled { get; set; }

        public string Module { get; set; }

        public MbpApiAttribute(bool isEnabled = true)
        {
            IsEnabled = isEnabled;
            IsMetadataEnabled = true;
        }

        public virtual bool IsEnabledFor(Type type)
        {
            return IsEnabled;
        }

        public virtual bool IsEnabledFor(MethodInfo method)
        {
            return IsEnabled;
        }

        public virtual bool IsMetadataEnabledFor(Type type)
        {
            return IsMetadataEnabled;
        }

        public virtual bool IsMetadataEnabledFor(MethodInfo method)
        {
            return IsMetadataEnabled;
        }

        internal static bool IsExplicitlyEnabledFor(Type type)
        {
            var remoteServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsEnabledFor(type);
        }

        internal static bool IsExplicitlyDisabledFor(Type type)
        {
            var remoteServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type);
        }

        internal static bool IsMetadataExplicitlyEnabledFor(Type type)
        {
            var remoteServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsMetadataEnabledFor(type);
        }

        internal static bool IsMetadataExplicitlyDisabledFor(Type type)
        {
            var remoteServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsMetadataEnabledFor(type);
        }

        internal static bool IsMetadataExplicitlyDisabledFor(MethodInfo method)
        {
            var remoteServiceAttr = method.GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsMetadataEnabledFor(method); 
        }

        internal static bool IsMetadataExplicitlyEnabledFor(MethodInfo method)
        {
            var remoteServiceAttr = method.GetSingleAttributeOrNull<MbpApiAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsMetadataEnabledFor(method);
        }
    }
}
