using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace ORM_DEV
{
    public abstract class Entity<T> : EntityBase where T : Entity<T>
    {
        /*public static T Find(Func<T, bool> filter = null)
        {
            using (EntityManager.Connection)
            {
                if (filter != null)
                    return EntityManager.Connection.Query<T>($"SELECT * FROM {typeof(T).GetTableName()}").Where(filter).FirstOrDefault();

                return EntityManager.Connection.Query<T>($"SELECT * FROM {typeof(T).GetTableName()}").FirstOrDefault();
            }
        }
        
        public static IEnumerable<T> FindAll(Func<T, bool> filter = null)
        {
            using (EntityManager.Connection)
            {
                if (filter != null)
                    return EntityManager.Connection.Query<T>($"SELECT * FROM {typeof(T).GetTableName()}").Where(filter);

                return EntityManager.Connection.Query<T>($"SELECT * FROM {typeof(T).GetTableName()}");
            }
        }*/

        public T Get(Func<T, bool> filter = null)
        {
            return Cache.Get<T>(filter);
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