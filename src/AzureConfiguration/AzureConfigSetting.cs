namespace AzureStronglyTypedConfigs
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class AzureConfigSetting : Attribute
    {
        private const int DefaultExtraCapacity=5;
        public AzureConfigSetting()
        {
            IncludeInCSDEF = true;
            ExtraCapacity = DefaultExtraCapacity;
        }

        public string Name { get; set; }

        public bool IncludeInCSDEF { get; set; }

        public int ExtraCapacity { get; set; }

        public static string GetNameFromPropertyInfo(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(x => x is AzureConfigSetting) as AzureConfigSetting;
            if (nameAttribute != null && !string.IsNullOrEmpty(nameAttribute.Name))
            {
                return nameAttribute.Name;
            }

            return propertyInfo.Name;
        }

        public static bool GetIncludeInCSDEFFromPropertyInfo(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(x => x is AzureConfigSetting) as AzureConfigSetting;
            if (nameAttribute != null)
            {
                return nameAttribute.IncludeInCSDEF;
            }

            return true;
        }

        public static int GetExtraCapacityPropertyInfo(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(x => x is AzureConfigSetting) as AzureConfigSetting;
            if (nameAttribute != null)
            {
                return nameAttribute.ExtraCapacity;
            }

            return DefaultExtraCapacity;
        }

    }
}
