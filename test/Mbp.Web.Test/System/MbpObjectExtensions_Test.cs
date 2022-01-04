using Shouldly;
using System;
using Xunit;

namespace Mbp.AspNetCore.Test.System
{
    public class MbpObjectExtensions_Test
    {
        [Fact]
        public void As_Test()
        {
            var b = new B();
            b.As<A>().IsWho().ShouldBe("A");
        }

        [Fact]
        public void IsIn_Test()
        {
            string[] ls = new string[] { "a", "b", "c" };
            "a".IsIn(ls).ShouldBeTrue();
            "d".IsIn(ls).ShouldBeFalse();
        }

        [Fact]
        public void If_Test()
        {
            string str = "abc";
            str.If(true, (m) => m = m + "d").ShouldBe("abcd");
            str.If(false, (m) => m = m + "d").ShouldBe("abc");

            string str1 = "abc";
            str1.If(true, (s) => s = s + "d").ShouldBe("abcd");
            str1.If(false, (s) => s = s + "d").ShouldBe("abc");
        }
    }

    public class A
    {
        public string IsWho()
        {
            return "A";
        }
    }

    public class B : A
    {
        public string IsWho()
        {
            return "B";
        }
    }
}
