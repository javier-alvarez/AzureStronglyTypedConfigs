using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureConfiguration
{
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
