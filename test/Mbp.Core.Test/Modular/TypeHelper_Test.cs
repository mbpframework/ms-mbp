using Mbp.Utils;
using Shouldly;
using System;
using Xunit;

namespace Mbp.Core.Test.Modular
{
    public class Class1
    {
        public Func<int> GetName => () => 1;

        public Action<int> GetName1 => (m) => m = 1;
    }

    public class TypeHelper_Test
    {
        [Fact]
        public void IsFunc()
        {
            InternalTypeUtil.IsFunc(new Class1().GetName).ShouldBeTrue();

            InternalTypeUtil.IsFunc(new Class1().GetName1).ShouldBeFalse();

            InternalTypeUtil.IsFunc<int>(new Class1().GetName).ShouldBeTrue();
        }

        [Fact]
        public void IsNullable()
        {
            InternalTypeUtil.IsNullable(typeof(Nullable)).ShouldBeFalse();
            InternalTypeUtil.IsNullable(typeof(Nullable<>)).ShouldBeTrue();
        }
    }
}
