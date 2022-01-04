using System.Collections.Generic;
using System.Reflection;

namespace Mbp.Modular.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAssemblyFinder
    {
        IReadOnlyList<Assembly> Assemblies { get; }
    }
}
