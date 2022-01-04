using Shouldly;
using Xunit;
using Mbp.Internal.Extensions;

namespace Mbp.Core.Test.Extensions
{
    public class ExtensionMethods_Test
    {
        [Fact]
        public void IsNullOrEmpty_Test()
        {
            string str = null;

            "abc".IsNullOrEmpty().ShouldBeFalse();
            "".IsNullOrEmpty().ShouldBeTrue();
            " ".IsNullOrEmpty().ShouldBeFalse();
            string.Empty.IsNullOrEmpty().ShouldBeTrue();
            str.IsNullOrEmpty().ShouldBeTrue();
        }

        [Fact]
        public void IsIn_Test()
        {
            string[] strs = { "a", "b", "c" };

            "a".IsIn().ShouldBeFalse();
            "a".IsIn(strs).ShouldBeTrue();
            "b".IsIn(strs).ShouldBeTrue();

            "".IsIn(strs).ShouldBeFalse();
            "d".IsIn(strs).ShouldBeFalse();
        }

        [Fact]
        public void RemovePostFix_Test()
        {
            var str = "GetAsync";
            var str1 = "GetList";
            var str2 = "GetUserInfo";

            var fixs = new string[] { "Async", "List" };
            str.RemovePostFix(fixs).ShouldBe("Get");
            str1.RemovePostFix(fixs).ShouldBe("Get");
            str2.RemovePostFix(fixs).ShouldBe("GetUserInfo");
        }

        [Fact]
        public void RemovePreFix_Test()
        {
            var str = "EditUser";
            var str1 = "AddUser";
            var str2 = "GetUser";

            var fixs = new string[] { "Edit", "Add" };
            str.RemovePreFix(fixs).ShouldBe("User");
            str1.RemovePreFix(fixs).ShouldBe("User");
            str2.RemovePreFix(fixs).ShouldBe("GetUser");
        }

        [Fact]
        public void Left_Test()
        {
            var str = "Mbp";
            str.Left(1).ShouldBe("n");
            str.Left(2).ShouldBe("ni");
        }

        [Fact]
        public void Right_Test()
        {
            var str = "Mbp";
            str.Right(1).ShouldBe("n");
            str.Right(2).ShouldBe("en");
        }

        [Fact]
        public void GetCamelCaseFirstWord_Test()
        {
            var str = "getUser";
            str.GetCamelCaseFirstWord().ShouldBe("get");
            var str1 = "GetUser";
            str1.GetCamelCaseFirstWord().ShouldBe("");
        }

        [Fact]
        public void GetPascalCaseFirstWord_Test()
        {
            var str = "GetUser";
            str.GetPascalCaseFirstWord().ShouldBe("Get");
            var str1 = "getUser";
            str1.GetPascalCaseFirstWord().ShouldBe("User");
        }

        [Fact]
        public void GetPascalOrCamelCaseFirstWord_Test()
        {
            var str = "GetUser";
            var str1 = "getUser";
            str.GetPascalOrCamelCaseFirstWord().ShouldBe("Get");
            str1.GetPascalOrCamelCaseFirstWord().ShouldBe("get");
        }
    }
}
