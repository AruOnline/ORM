using DBCache;

namespace ORM_TEST.Entities
{
    public class Player : Entity
    {
        public virtual string Name { get; set; }
        
        public virtual User Owner { get; set; }
    }
}