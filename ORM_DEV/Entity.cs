using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace ORM_DEV
{
    public abstract class Entity<T> : EntityBase where T : Entity<T>
    {
        public static T Get(Func<T, bool> filter = null)
        {
            return Cache.Get<T>(e => e.Id == 1);
        }

        public bool Delete()
        {
            return Cache.Delete<T>(this);
        }

        public bool Save()
        {
            return Cache.Save<T>(this);
        }
    }
}