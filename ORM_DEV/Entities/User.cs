using System;
using ORM_DEV.Framework;
using ORM_DEV.Framework.Attributes;
using ORM_DEV.Framework.Entities;

namespace ORM_DEV.Entities
{
    [Table("user")]
    public class User : Entity<User>
    {
        public string Name { get; set; }
    }
}