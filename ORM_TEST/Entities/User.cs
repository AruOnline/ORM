using DBCache;

namespace ORM_TEST.Entities
{
    public class User : Entity
    {
        public virtual string Name { get; set; }
        
        public virtual Player ActivePlayer { get; set; }
    }
}