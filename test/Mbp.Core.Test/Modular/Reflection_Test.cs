using Mbp.Modular.Reflection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using Xunit;

namespace Mbp.Core.Test.Modular
{
    [DisplayName("Class_Reflection")]
    [Table("Ng_User")]
    public class Class_Reflection_Test
    {
        [EmailAddress(ErrorMessage = "错误")]
        public string Address { get; set; }
    }

    public class Reflection_Test
    {
        [Fact]
        public void FindAll_Test()
        {
            var attr = ReflectionHelper.GetSingleAttributeOrDefaultByFullSearch<DisplayNameAttribute>(typeof(Class_Reflection_Test).GetTypeInfo());

            attr.DisplayName.ShouldBe("Class_Reflection");
        }

        [Fact]
        public void GetSingleAttributeOrDefaultByFullSearch_Test()
        {
            var attr = ReflectionHelper.GetSingleAttributeOrDefaultByFullSearch<TableAttribute>(typeof(Class_Reflection_Test).GetTypeInfo());

            attr.Name.ShouldBe("Ng_User");
        }

        [Fact]
        public void GetSingleAttributeOrNull_Test()
        {
            typeof(Class_Reflection_Test).GetProperty("Address").GetSingleAttributeOrNull<EmailAddressAttribute>(true).ErrorMessage.ShouldBe("错误");
        }

        [Fact]
        public void GetSingleAttributeOfTypeOrBaseTypesOrNull_Test()
        {
            ReflectionHelper.GetSingleAttributeOrDefault<EmailAddressAttribute>(typeof(Class_Reflection_Test).GetProperty("Address")).ShouldNotBeNull();
        }
    }
}
