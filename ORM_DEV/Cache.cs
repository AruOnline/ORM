using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper;

namespace ORM_DEV
{
    public static class Cache
    {
        private static bool _isRunning = true;
        private static int _updateInterval;
        private static Thread _main;

        private static readonly Dictionary<Type, List<EntityBase>> DBCACHE = new Dictionary<Type, List<EntityBase>>();
        
        /// <summary>
        /// Initializes the Cache and starts a task running that cache
        /// </summary>
        /// <param name="updateInterval">Milliseconds between two database updates</param>
        public static void Initialize(int updateInterval = 30000)
        {
            _updateInterval = updateInterval;
            
            _main = new Thread(Run);
            _main.Start();
        }

        /// <summary>
        /// Stops the Cache task
        /// </summary>
        public static void Stop()
        {
            _isRunning = true;
            _main.Abort();
        }
        
        /// <summary>
        /// Enqueue an entity for deletion
        /// </summary>
        /// <param name="entity">The entity which should be deleted</param>
        /// <typeparam name="T">The type of that entity</typeparam>
        /// <returns>Whether the deletion has succeeded</returns>
        public static bool Delete<T>(EntityBase entity) where T : EntityBase
        {
            lock (DBCACHE)
            {
                if (DBCACHE.ContainsKey(typeof(T)) && DBCACHE[typeof(T)].Exists(e => e.Id == entity.Id))
                {
                    DBCACHE[typeof(T)].Remove(entity);
                }
            }
            
            using (EntityManager.Connection)
            {
                return EntityManager.Connection.Execute($"DELETE FROM {typeof(T).GetTableName()} WHERE Id={entity.Id}") > 0;
            }
        }

        /// <summary>
        /// Enqueue an entity for saving to the database (marks as changed)
        /// </summary>
        /// <param name="entity">The entity which should be saved to the database</param>
        /// <typeparam name="T">The type of that entity</typeparam>
        /// <returns>Whether the save has succeeded</returns>
        public static bool Save<T>(EntityBase entity) where T : EntityBase
        {
            
            lock (DBCACHE)
            {
                if (DBCACHE.ContainsKey(typeof(T)) && !DBCACHE[typeof(T)].Exists(e => e.Id == entity.Id))
                {
                    DBCACHE[typeof(T)].Add(entity);
                }
            }

            // TODO: move that to the save cycle
            using (EntityManager.Connection)
            {
                // TODO: needs to map all properties to a key=value like string.
                return EntityManager.Connection.Execute($"UPDATE {typeof(T).GetTableName()} SET x=1") > 0;
            }
        }

        /// <summary>
        /// Retrieve an entity from cache, if available, or from database
        /// </summary>
        /// <param name="filter">A simple filter to decide which entity to select</param>
        /// <typeparam name="T">The type of that entity</typeparam>
        /// <returns>The requested entity or null otherwise</returns>
        public static T Get<T>(Func<EntityBase, bool> filter) where T : EntityBase
        {
            lock (DBCACHE)
            {
                if (DBCACHE.ContainsKey(typeof(T)) && DBCACHE[typeof(T)].Exists(new Predicate<EntityBase>(filter)))
                {
                    return DBCACHE[typeof(T)].FirstOrDefault(filter) as T;
                }
            }

            using (EntityManager.Connection)
            {
                if (typeof(T).TableExists())
                    return EntityManager.Connection.Query<T>($"SELECT * FROM {typeof(T).GetTableName()}").First(filter) as T;
                return null;
            }
        }

        private static void Run()
        {
            while (_isRunning)
            {
                Thread.Sleep(_updateInterval);
                DispatchActions();
                Console.WriteLine("Updated DB...");
            }
        }

        private static void DispatchActions()
        {
            
        }
    }
}
