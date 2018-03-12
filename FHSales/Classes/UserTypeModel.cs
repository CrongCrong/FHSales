using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FHSales.Classes
{
    public class UserTypeModel
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }
    }
}
