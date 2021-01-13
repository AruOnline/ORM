using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper;
using ORM_DEV.Entities;

namespace ORM_DEV
{
    public static class Cache
    {
        /// <summary>
        /// A time value in Milliseconds which needs to pass until the next database update happens
        /// </summary>
        public static int SaveInterval { get; set; } = 30000;
        
        private static List<EntityBase> _dbcache = new List<EntityBase>();
        
        public static void Run()
        {
            while (true)
            {
                Thread.Sleep(SaveInterval);
                lock (_dbcache)
                {
                    SaveEntities();
                }
            }
        }

        private static void SaveEntities()
        {
            foreach (EntityBase entity in _dbcache)
            {
                // Check for changes on that element if it already exists in cache or db
                using (EntityManager.Connection)
                {
                    
                }
            }
        }

        public static bool Delete<T>(EntityBase entity)
        {
            lock (_dbcache)
            {
                if (_dbcache.Contains(entity)) _dbcache.Remove(entity);
            }

            using (EntityManager.Connection)
            {
                return EntityManager.Connection.Execute($"DELETE FROM {typeof(T).GetTableName()} WHERE Id={entity.Id}") > 0;
            }
        }

        public static bool Save<T>(EntityBase entity)
        {
            lock (_dbcache)
            {
                if (_dbcache.Find(e => e.Id == entity.Id) == null)
                {
                    _dbcache.Add(entity);
                    // TODO: add flag for that is being a add operation
                }
            }

            using (EntityManager.Connection)
            {
                // TODO: needs to map all properties to a key=value like string.
                return EntityManager.Connection.Execute($"UPDATE {typeof(T).GetTableName()} SET x=1") > 0;
            }
        }

        public static T Get<T>(Func<T, bool> filter)
        {
            lock (_dbcache)
            {
                // Search cache for object 
                // return if found
            }
            
            // if we are still here we 
        }
    }
}