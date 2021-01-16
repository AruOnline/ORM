using System;

namespace DBCache
{
    public abstract class Entity
    {
        public virtual long Id { get; } = 0;

        protected Entity()
        {
            // Need to create a id
            if (Id == 0) Id = Cache.GenerateId();
            Cache.Add(this);
        }

        public static T Get<T>(Func<T, bool> filter) where T : Entity
        {
            return Cache.Get(filter);
        }

        public virtual void Delete()
        {
            Cache.Delete(this);
        }
    }
}