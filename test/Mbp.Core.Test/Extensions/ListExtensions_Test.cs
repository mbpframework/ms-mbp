using Mbp.Extensions;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using Mbp.Internal.Extensions;

namespace Mbp.Core.Test.Extensions
{
    public class ListExtensions_Test
    {
        [Fact]
        public void SortByDependencies_Test()
        {
            var A = new Module_Test() { Name = "A" };
            var B = new Module_Test() { Name = "B" };
            var C = new Module_Test() { Name = "C" };
            var D = new Module_Test() { Name = "D" };

            A.Dependencies.Add(B);
            B.Dependencies.Add(C);
            C.Dependencies.Add(D);

            var list = new List<Module_Test>();
            list.Add(A);
            list.Add(C);
            list.Add(D);
            list.Add(B);

            list = list.SortByDependencies(l => l.Dependencies, new ModuleEqualityComparer_Test());

            list[0].ShouldBe(D);


            var A1 = new Module_Test() { Name = "A1" };
            var B1 = new Module_Test() { Name = "B1" };
            var C1 = new Module_Test() { Name = "C1" };
            var D1 = new Module_Test() { Name = "D1" };

            A1.Dependencies.Add(B1);
            A1.Dependencies.Add(C1);
            B1.Dependencies.Add(D1);
            C1.Dependencies.Add(D1);

            var list1 = new List<Module_Test>();
            list1.Add(A1);
            list1.Add(C1);
            list1.Add(D1);
            list1.Add(B1);

            list1 = list1.SortByDependencies(l => l.Dependencies, new ModuleEqualityComparer_Test());

            list1[0].ShouldBe(D1);
        }

        [Fact]
        public void ModuleEqualityComparer_Test()
        {
            var A = new Module_Test() { Name = "A" };
            var B = new Module_Test() { Name = "B" };
            var C = new Module_Test() { Name = "C" };
            var D = new Module_Test() { Name = "C" };

            Dictionary<Module_Test, string> keyValuePairs = new Dictionary<Module_Test, string>(new ModuleEqualityComparer_Test());
            keyValuePairs.Add(A,"A");
            keyValuePairs.Add(B, "B");
            keyValuePairs.Add(C, "C");

            keyValuePairs.TryAdd(D, "D").ShouldBeFalse();
        }
    }

    public class Module_Test
    {
        public string Name { get; set; }

        public List<Module_Test> Dependencies { get; set; } = new List<Module_Test>();
    }

    public class ModuleEqualityComparer_Test : EqualityComparer<Module_Test>
    {
        public override bool Equals(Module_Test x, Module_Test y)
        {
            return (x == null && y == null) || (x != null && y != null && x.Name == y.Name);
        }

        public override int GetHashCode(Module_Test obj)
        {
            return obj == null ? 0 : obj.Name.GetHashCode();
        }
    }
}
