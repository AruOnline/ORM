using System;

namespace ORM_DEV.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public readonly string Name;
        
        public TableAttribute(string name = "")
        {
            Name = name;
        }
    }
}