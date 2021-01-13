using System;

namespace ORM_DEV
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name;
        
        public TableAttribute(string name = "")
        {
            Name = name;
        }
    }
}