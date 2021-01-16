using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IdGen;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace DBCache
{
    public static class Cache
    {
        private static ISessionFactory _sessionFactory;
        private static bool _isRunning = true;
        
        private static readonly IdGenerator GENERATOR = new IdGenerator(0);
        private static readonly Dictionary<Type, List<Entity>> DB_CACHE = new Dictionary<Type, List<Entity>>();
        
        public static void Initialize(IPersistenceConfigurer dbConfig)
        {
            _sessionFactory = CreateSessionFactory(dbConfig);
            
            Thread cache = new Thread(Execute) {Name = "Cache"};
            cache.Start();
        }

        internal static long GenerateId()
        {
            return GENERATOR.CreateId();
        }

        internal static T Get<T>(Func<T, bool> filter) where T : Entity
        {
            lock (DB_CACHE)
            {
                if (DB_CACHE.ContainsKey(typeof(T)))
                {
                    return DB_CACHE[typeof(T)].Cast<T>().FirstOrDefault(filter);
                }
            }

            using (ISession session = _sessionFactory.OpenSession())
            {
                return session.Query<T>().FirstOrDefault(filter);
            }
        }

        internal static void Delete(Entity entity)
        {
            lock (DB_CACHE)
            {
                if (DB_CACHE.ContainsKey(entity.GetType()) && DB_CACHE[entity.GetType()].Exists(e => e.Id == entity.Id))
                {
                    DB_CACHE[entity.GetType()].Remove(entity);
                }
            }
            
            using (ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(entity);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        internal static void Add(Entity entity)
        {
            lock (DB_CACHE)
            {
                if (!DB_CACHE.ContainsKey(entity.GetType()))
                {
                    DB_CACHE.Add(entity.GetType(), new List<Entity>());
                }

                if (!DB_CACHE[entity.GetType()].Exists(e => e.Id == entity.Id))
                {
                    DB_CACHE[entity.GetType()].Add(entity);
                }
            }
        }

        private static void Execute()
        {
            while (_isRunning)
            {
                Thread.Sleep(5000);
                DispatchChanges();
            }
        }

        private static void DispatchChanges()
        {
            using (ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    lock (DB_CACHE)
                    {
                        try
                        {
                            foreach (Entity entity in DB_CACHE.SelectMany(entityType => DB_CACHE[entityType.Key]))
                            {
                                session.SaveOrUpdate(entity);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Some action triggered rollback, canceling thread...");
                            Console.WriteLine(ex.Message);
                            _isRunning = false;
                        }    
                    }
                }
            }
            Console.WriteLine("Triggered!!");
        }
        
        private static ISessionFactory CreateSessionFactory(IPersistenceConfigurer dbConfig)
        {
            AutoPersistenceModel autoMapping = AutoMap
                .Assembly(Assembly.GetEntryAssembly(), new MapConfig());
                //.Override<User>(u => u.Id(i => i.Id).GeneratedBy.Assigned())
                //.Override<Player>(u => u.Id(i => i.Id).GeneratedBy.Assigned());
            
            return Fluently
                .Configure()
                .Database(dbConfig)
                .Mappings(m => m.AutoMappings.Add(autoMapping))
                .ExposeConfiguration(config => new SchemaUpdate(config).Execute(true, true))
                .BuildSessionFactory();
        }
    }
}