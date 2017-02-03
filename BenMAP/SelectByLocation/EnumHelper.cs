using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BenMAP.SelectByLocation
{
    public static class EnumHelper
    {
        public static IEnumerable<NamedEntity> ToEnumEntities(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentOutOfRangeException("enumType", "Only enums are supported");
            }
            
            foreach (var enumValue in enumType.GetEnumValues())
            {
                var memberInfo = enumType.GetMember(enumValue.ToString());
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                var enumEntity = new NamedEntity
                {
                    Value = enumValue,
                    Name = attributes.Length > 0 ? ((DescriptionAttribute) attributes[0]).Description : enumValue.ToString()
                };
                yield return enumEntity;
            }
        }
    }

    public class NamedEntity
    {
        public object Value { get; set; }
        public string Name { get; set; }
    }
}