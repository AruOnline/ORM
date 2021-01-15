using System.Collections.Generic;
using ORM_TEST.Framework;
using ORM_TEST.Framework.Attributes;

namespace ORM_TEST.Entities
{
    public class User : Entity
    {
        [NotMapped]
        public virtual string Name { get; set; }
        
        public virtual Player ActivePlayer { get; set; }
    }
}