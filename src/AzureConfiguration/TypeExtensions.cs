namespace AzureStronglyTypedConfigs
{
    using System;
    using System.Collections;

    public static class TypeExtensions
    {
        public static bool IsIEnumerable(this Type type)
        {
            foreach (Type iType in type.GetInterfaces())
            {
                if (iType == typeof(IEnumerable))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
