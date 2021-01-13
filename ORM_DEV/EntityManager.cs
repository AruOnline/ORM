using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace ORM_DEV
{
    internal static class EntityManager
    {
        private static string _connectionString;
        
        /// <summary>
        /// Connection works as kind of connection factory...
        /// </summary>
        internal static IDbConnection Connection => new MySqlConnection(_connectionString);
        
        public static void Initialize(string connectionString, bool createTablesIfNotExist = true)
        {
            _connectionString = connectionString;
            if (createTablesIfNotExist) CreateTables();
            Cache.Initialize();
        }

        public static string GetTableName(this Type type)
        {
            if (type.GetCustomAttribute<TableAttribute>() != null)
                return type.GetCustomAttribute<TableAttribute>().Name;
            
            if (type.Name.EndsWith("y"))
                return $"{type.Name.Substring(0, type.Name.Length - 1)}ies".ToLower();

            if (type.Name.EndsWith("s") || type.Name.EndsWith("sh") || type.Name.EndsWith("ch") ||
                type.Name.EndsWith("x") || type.Name.EndsWith("z"))
                return $"{type.Name}es".ToLower();
            
            return $"{type.Name}s".ToLower();
        }

        private static void CreateTables()
        {
            IEnumerable<Type> entities = Assembly.GetEntryAssembly()?.GetTypes();
            if (entities == null) return;

            foreach (Type entity in entities.Where(e => e.IsSubclassOf(typeof(EntityBase))))
            {
                if (entity.GetTableName().Contains("entity")) continue;
                if (entity.TableExists()) continue;
                
                Console.WriteLine($"Table {entity.GetTableName()} missing...");
                entity.CreateTableIfNotExists();
            }
        }

        internal static bool TableExists(this Type type)
        {
            using (Connection)
            {
                return Connection.Query($"SHOW TABLES LIKE '{type.GetTableName()}'").Any();
            }
        }

        private static bool CreateTableIfNotExists(this Type type)
        {
            return false;
        }
    }
}