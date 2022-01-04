using Shouldly;
using System.Collections.Generic;
using Xunit;
using Mbp.Internal.Extensions;

namespace Mbp.Core.Test.Extensions
{
    public class CollectionExtensions_Test
    {
        [Fact]
        public void AddIfNotExist_With_Func()
        {
            List<int> collection = new List<int> { 4, 5, 6 };

            collection.AddIfNotExist(5);
            collection.Count.ShouldBe(3);

            collection.AddIfNotExist(42);
            collection.Count.ShouldBe(4);

            collection.AddIfNotExist(8);
            collection.Count.ShouldBe(5);

            collection.AddIfNotExist(999);
            collection.Count.ShouldBe(6);

            collection.AddIfNotExist(33, (m) => collection.Exists(m => m == 33));
            collection.Count.ShouldBe(7);

            collection.AddIfNotExist(4, (m) => collection.Exists(m => m == 4));
            collection.Count.ShouldBe(7);
        }

        [Fact]
        public void AddIf_Test()
        {
            // 代表值类型
            List<int> values = new List<int> { 4, 5, 6 };

            // 代表引用类型和记录类型
            List<string> refs = new List<string>() { "a", "b", "c" };

            values.AddIf(3, true);
            values.Count.ShouldBe(4);
            values.AddIf(5, false);
            values.Count.ShouldBe(4);

            refs.AddIf("d", true);
            refs.Count.ShouldBe(4);
            refs.AddIf("e", false);
            refs.Count.ShouldBe(4);
        }

        [Fact]
        public void AddIf_With_Func()
        {
            // 代表值类型
            List<int> values = new List<int> { 4, 5, 6 };

            // 代表引用类型和记录类型
            List<string> refs = new List<string>() { "a", "b", "c" };

            values.AddIf(3, () => true);
            values.Count.ShouldBe(4);
            values.AddIf(5, () => false);
            values.Count.ShouldBe(4);

            refs.AddIf("d", () => true);
            refs.Count.ShouldBe(4);
            refs.AddIf("e", () => false);
            refs.Count.ShouldBe(4);
        }

        [Fact]
        public void AddIfNotNull_Test()
        {
            // 代表引用类型和记录类型
            List<string> refs = new List<string>() { "a", "b", "c" };

            refs.AddIfNotNull("d");
            refs.Count.ShouldBe(4);
            refs.AddIfNotNull(null);
            refs.Count.ShouldBe(4);
            refs.AddIfNotNull("e");
            refs.Count.ShouldBe(5);
        }

        [Fact]
        public void GetOrAdd_Test()
        {
            // 代表值类型
            List<int> values = new List<int> { 4, 5, 6 };

            values.GetOrAdd(m => m == 8, () => 8);
            values.Count.ShouldBe(4);
            values.Exists(m => m == 8).ShouldBeTrue();

            values.GetOrAdd(m => m == 4, () => 4);
            values.Count.ShouldBe(4);
            values.Exists(m => m == 9).ShouldBeFalse();
        }

        [Fact]
        public void IsNullOrEmpty_Test()
        {
            // 代表值类型
            List<int> values = new List<int> { 4, 5, 6 };
            List<int> values1 = new List<int>();
            List<int> values2 = null;
            values.IsNullOrEmpty().ShouldBeFalse();
            values1.IsNullOrEmpty().ShouldBeTrue();
            values2.IsNullOrEmpty().ShouldBeTrue();
        }
    }
}
