using System;

namespace Mbp.Modular
{
    public interface IDependedTypesProvider
    {
        Type[] GetDependedTypes();
    }
}
