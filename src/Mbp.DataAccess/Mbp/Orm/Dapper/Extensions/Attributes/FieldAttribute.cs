using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public FieldAttribute(string name)
        {
            this.Name = name;
        }
    }
}
