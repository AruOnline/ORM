using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ORM_TEST.Entities;
using ORM_TEST.Framework;

namespace ORM_TEST
{
    internal static class Program
    {
        private const string CONN_STRING = "Server=127.0.0.1;Database=game;Uid=game;Pwd=Ferrarie1!;";
        
        public static void Main(string[] args)
        {
            ISessionFactory sessionFactory = CreateSessionFactory();
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User user = new User {Name = "Bob"};
                    Player p1 = new Player {Name = "P1", Owner = user};
                    Player p2 = new Player {Name = "P2", Owner = user};
                    user.ActivePlayer = p1;
                    
                    session.SaveOrUpdate(user);
                    session.SaveOrUpdate(p1);
                    session.SaveOrUpdate(p2);
                    
                    transaction.Commit();
                }
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently
                .Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(CONN_STRING))
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<User>(new MapConfig())))
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).Create(true, true);
        }
    }
}