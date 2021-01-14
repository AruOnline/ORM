using System;

namespace ORM_DEV.Framework.Entities
{
    public abstract class Entity<T> : EntityBase where T : Entity<T>
    {
        protected Entity()
        {
            Id = Cache.Cache.GenerateId(); 
            Cache.Cache.Add<T>(this);
        }
        
        public static T Get(Func<T, bool> filter = null)
        {
            return Cache.Cache.Get<T>(e => e.Id == 1);
        }

        public bool Delete()
        {
            return Cache.Cache.Delete<T>(this);
        }
    }
}