using System;

namespace ORM_DEV.Entities
{
    [Table("user")]
    public class User : Entity<User>
    {
        public string Name { get; set; }
    }
}