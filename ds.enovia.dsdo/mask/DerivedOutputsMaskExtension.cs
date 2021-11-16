using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.mask
{
    public static class DerivedOutputsMaskExtension
    {
        public static string GetString(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DefaultValueAttribute[] attributes = fi.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            if (attributes != null && attributes.Any())
            {
                return ((string)attributes.First().Value);
            }

            return value.ToString();
        }
    }
}
