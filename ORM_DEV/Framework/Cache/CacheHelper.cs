using System;
using System.Reflection;
using ORM_DEV.Framework.Attributes;

namespace ORM_DEV.Framework.Cache
{
    internal static class CacheHelper
    {
        internal static string GetTableName(this Type type)
        {
            if (type.GetCustomAttribute<TableAttribute>() != null)
                return type.GetCustomAttribute<TableAttribute>().Name;
            
            if (type.Name.EndsWith("y"))
                return $"{type.Name.Substring(0, type.Name.Length - 1)}ies".ToLower();

            if (type.Name.EndsWith("s") || type.Name.EndsWith("sh") || type.Name.EndsWith("ch") ||
                type.Name.EndsWith("x") || type.Name.EndsWith("z"))
                return $"{type.Name}es".ToLower();
            
            return $"{type.Name}s".ToLower();
        }
    }
}