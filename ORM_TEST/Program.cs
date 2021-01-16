using System;
using System.Threading;
using DBCache;
using FluentNHibernate.Cfg.Db;
using ORM_TEST.Entities;

namespace ORM_TEST
{
    internal static class Program
    {
        private const string CONN_STRING = "Server=127.0.0.1;Database=game;Uid=game;Pwd=Ferrarie1!;";
        
        public static void Main(string[] args)
        {
            Cache.Initialize(MySQLConfiguration.Standard.ConnectionString(CONN_STRING).ShowSql());
            
            User user = new User {Name = "Bob"};
            Player p1 = new Player {Name = "Bill"};
            user.ActivePlayer = p1;

            User x = Entity.Get<User>(e => e.ActivePlayer == p1);
            Console.WriteLine(x.Id == user.Id);
            
            Thread.Sleep(8000);
            
            x.Delete();
            
            Console.WriteLine(user);
        }
    }
}