using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using IdGen;
using ORM_DEV.Framework.Entities;

namespace ORM_DEV.Framework.Cache
{
    internal static class Cache
    {
        private static bool _isRunning = true;
        private static int _updateInterval;
        private static Thread _main;

        private static readonly Dictionary<Type, List<EntityBase>> DB_CACHE = new Dictionary<Type, List<EntityBase>>();
        private static readonly IdGenerator GENERATOR = new IdGenerator(0);
        
        /**
         * Internal methods
         */
        
        internal static void Initialize(IDbConnection connection, int updateInterval = 5000)
        {
            _updateInterval = updateInterval;
            Sql.Initialize(connection);
            
            _main = new Thread(Run) {Name = "DB_Cache"};
            _main.Start();
        }

        internal static long GenerateId()
        {
            return GENERATOR.CreateId();
        }
        
        internal static void Add<T>(EntityBase entity)
        {
            lock (DB_CACHE)
            {
                // Add list for that type if it doesnt exist already
                if (!DB_CACHE.ContainsKey(typeof(T)))
                {
                    DB_CACHE.Add(typeof(T), new List<EntityBase>());
                }
                
                // Add instance to type list
                if (!DB_CACHE[typeof(T)].Exists(e => e.Id == entity.Id))
                {
                    DB_CACHE[typeof(T)].Add(entity);
                }
                else
                {
                    Console.WriteLine("Cache: duplicate id");
                }
            }
        }

        internal static T Get<T>(Func<EntityBase, bool> filter) where T : EntityBase
        {
            lock (DB_CACHE)
            {
                if (DB_CACHE.ContainsKey(typeof(T)) && DB_CACHE[typeof(T)].Exists(new Predicate<EntityBase>(filter)))
                {
                    return DB_CACHE[typeof(T)].FirstOrDefault(filter) as T;
                }
            }
            
            return Sql.Get<T>();
        }

        internal static bool Delete<T>(EntityBase entity) where T : EntityBase
        {
            lock (DB_CACHE)
            {
                if (DB_CACHE.ContainsKey(typeof(T)) && DB_CACHE[typeof(T)].Exists(e => e.Id == entity.Id))
                {
                    DB_CACHE[typeof(T)].Remove(entity);
                }
            }

            return Sql.Delete(entity);
        }

        /**
         * Private methods
         */
        private static void Run()
        {
            CreateTables();
            while (_isRunning)
            {
                Thread.Sleep(_updateInterval);
                DispatchUpdates();
                Console.WriteLine("Updated DB...");
            }
        }
        
        private static void CreateTables()
        {
            IEnumerable<Type> entities = Assembly.GetEntryAssembly()?.GetTypes();
            if (entities == null) return;

            foreach (Type entity in entities.Where(e => e.IsSubclassOf(typeof(EntityBase))))
            {
                if (entity.GetTableName().Contains("entity")) continue;
                
                Console.WriteLine($"Table {entity.GetTableName()} missing...");
                entity.CreateTable();
            }
        }

        private static void DispatchUpdates()
        {
            lock (DB_CACHE)
            {
                foreach (KeyValuePair<Type, List<EntityBase>> cachedType in DB_CACHE)
                {
                    foreach (EntityBase entity in DB_CACHE[cachedType.Key])
                    {
                        if (!Sql.Exists(entity))
                        {
                            Sql.Insert(entity);
                            continue;
                        }
                            
                        Sql.Update(entity);
                    }
                }
            }
        }
    }
}
