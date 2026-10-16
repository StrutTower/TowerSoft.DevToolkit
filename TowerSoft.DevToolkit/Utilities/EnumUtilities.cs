using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TowerSoft.DevToolkit.Utilities {
    internal class EnumUtilities {
        /// <summary>
        /// Returns the display name for the enum value if the DisplayAttribute is defined otherwise it returns the default ToString value.
        /// </summary>
        /// <param name="enumValue">Enum value to get the display name</param>
        public static string GetEnumDisplayName(Enum enumValue) {
            Type type = enumValue.GetType();
            MemberInfo member = type.GetMember(enumValue.ToString())[0];
            DisplayAttribute attr = member.GetCustomAttribute<DisplayAttribute>();
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Name)) {
                return attr.Name;
            }
            return enumValue.ToString();
        }
    }
}
