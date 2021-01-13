using System;
using System.Collections.Generic;
using ORM_DEV.Entities;

namespace ORM_DEV
{
    internal static class Program
    {
        private const string ConnString = "Server=127.0.0.1;Database=game;Uid=game;Pwd=Ferrarie1!;";
        
        public static void Main(string[] args)
        {
            EntityManager.Initialize(ConnString);

            /*foreach (User user in User.FindAll())
            {
                Console.WriteLine(user.Id);
                Console.WriteLine(user.Name);
            }

            User user2 = User.Find(e => e.Id == 3);
            Console.WriteLine(user2?.Delete());
            */
        }
    }
}