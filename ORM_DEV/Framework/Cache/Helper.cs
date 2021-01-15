using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ORM_DEV.Framework.Attributes;
using ORM_DEV.Framework.Entities;

namespace ORM_DEV.Framework.Cache
{
    internal static class Helper
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

        internal static Dictionary<string, object> GetDbFields(this Type type, EntityBase entity, bool isForUpdate = true)
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();
            
            foreach (PropertyInfo field in type.GetProperties())
            {
                if (isForUpdate && field.Name == "Id") continue; 
                if (field.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                
                if (field.GetValue(entity) == null) continue;

                if (!field.GetType().IsPrimitive)
                {
                    if (field.GetValue(entity) is EntityBase innerEntity)
                    {
                        Console.WriteLine($"NonPrimitive:: {field.GetValue(entity).GetType()}");
                        fields.Add(field.Name, innerEntity.Id);
                        continue;
                    }
                }
                
                fields.Add(field.Name, field.GetValue(entity));
            }
            
            return fields;
        }
        
        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            IEnumerable<PropertyDescriptor> convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            IEnumerable<PropertyDescriptor> entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            TConvert convert = new TConvert();

            foreach (PropertyDescriptor entityProperty in entityProperties)
            {
                PropertyDescriptor property = entityProperty;
                PropertyDescriptor convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                convertProperty?.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
            }

            return convert;
        }
    }
}