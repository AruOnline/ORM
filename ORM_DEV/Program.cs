using MySql.Data.MySqlClient;
using ORM_DEV.Entities;
using ORM_DEV.Framework.Cache;

namespace ORM_DEV
{
    internal static class Program
    {
        private const string CONN_STRING = "Server=127.0.0.1;Database=game;Uid=game;Pwd=Ferrarie1!;";
        
        public static void Main(string[] args)
        {
            //EntityManager.Initialize(CONN_STRING);
            Cache.Initialize(new MySqlConnection(CONN_STRING));

            User user1 = User.Get(1);
            user1.Name = "HELLO!2222222222";

            Player p1 = new Player { Name = "P1" };
            //user1.P1 = p1;
        }
    }
}