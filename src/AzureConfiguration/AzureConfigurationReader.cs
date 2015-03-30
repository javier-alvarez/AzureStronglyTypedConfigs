namespace AzureStronglyTypedConfigs
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Azure;

    /// <summary>
    /// The azure configuration base.
    /// </summary>
    public static class AzureConfigurationReader
    {
        public static void ReadConfig<T>(T config)
        {
            var type = config.GetType();

            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType.IsIEnumerable())
                {
                    int i = 0;
                    var values = new List<string>();
                    string value = CloudConfigurationManager.GetSetting(string.Format("{0}_item{1}", AzureConfigSetting.GetNameFromPropertyInfo(propertyInfo), i++));
                    while (!string.IsNullOrEmpty(value))
                    {
                        values.Add(value);
                        value = CloudConfigurationManager.GetSetting(string.Format("{0}_item{1}", AzureConfigSetting.GetNameFromPropertyInfo(propertyInfo), i++));
                    }
                    Type genericType = GetIEnumerableInternalType(propertyInfo.PropertyType);
                    var listType = typeof(List<>);
                    var concreteType = listType.MakeGenericType(genericType);
                    var newList = Activator.CreateInstance(concreteType);
                    foreach (var v in values)
                    {
                        newList.GetType().GetMethod("Add").Invoke(newList, new object[] { Convert.ChangeType(v, genericType) });
                    }

                    propertyInfo.SetValue(config, newList);
                }
                else
                {
                    string value = CloudConfigurationManager.GetSetting(AzureConfigSetting.GetNameFromPropertyInfo(propertyInfo));
                    propertyInfo.SetValue(config, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                }
            }
        }

        private static Type GetIEnumerableInternalType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition()
                    == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            foreach (Type iType in type.GetInterfaces())
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition()
                    == typeof(IEnumerable<>))
                {
                    return iType.GetGenericArguments()[0];
                }
            }
            return null;
        }
    }
}