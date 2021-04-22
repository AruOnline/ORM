using System;
using System.Collections.Generic;
using System.Diagnostics;
using DBCache;
using FluentNHibernate.Cfg.Db;
using ORM_TEST.Entities;

namespace ORM_TEST
{
    internal static class Program
    {
        private const string CONN_STRING = "Server=127.0.0.1;Database=game;Uid=game;Pwd=somepwd;";
        
        public static void Main(string[] args)
        {
            Cache.Initialize(MySQLConfiguration.Standard.ConnectionString(CONN_STRING));

            Stopwatch stopwatch = new Stopwatch();
            
            User user = new User {Name = "Bob"};
            Player p1 = new Player {Name = "Bill", Owner = user};
            user.ActivePlayer = p1;
            
            User x = Entity.Get<User>(e => e.ActivePlayer == p1);
            
            Console.WriteLine($"Has same id: {x.Id == user.Id}");
            Console.WriteLine($"Is same instance: {x.Equals(user)}");

            x.Name = "Sam";
            
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
            
            //Thread.Sleep(8000);
            
            //x.Delete();
            
            Console.WriteLine(user);

            User y = Entity.Get<User>(e => e.Id == 78941237408123);
            Console.WriteLine(y == null);

            IEnumerable<User> userList = Entity.GetAll<User>();
            foreach (User usr in userList)
            {
                Console.WriteLine($"ID: {usr.Id}, Name: {usr.Name}");
            }

            stopwatch.Start();
            for (int i = 0; i < 100000; i++)
            {
                new Player {Name = $"FancyName{i}"};
            }
            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
