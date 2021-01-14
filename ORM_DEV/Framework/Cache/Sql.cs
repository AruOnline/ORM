using System;
using System.Data;
using System.Linq;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ORM_DEV.Framework.Cache
{
    internal static class Sql
    {
        private static QueryFactory _database;

        internal static void Initialize(IDbConnection connection)
        {
            _database = new QueryFactory(connection, new MySqlCompiler())
            {
                Logger = compiled => { Console.WriteLine(compiled.ToString()); }
            };
        }
        
        internal static int Update(EntityBase entity)
        {
            return _database
                .Query(entity.GetType().GetTableName())
                .Update(entity);
        }

        internal static int Insert(EntityBase entity)
        {
            return _database
                .Query(entity.GetType().GetTableName())
                .Insert(entity);
        }

        internal static bool Delete(EntityBase entity)
        {
            return _database
                .Query(entity.GetType().GetTableName())
                .Where("Id", entity.Id)
                .Delete()
                .Equals(1);
        }

        internal static T Get<T>()
        {
            return _database
                .Query(typeof(T).GetTableName())
                .Get<T>()
                .FirstOrDefault();
        }

        internal static bool Exists(EntityBase entity)
        {
            return _database
                .Query(entity.GetType().GetTableName())
                .Where("Id", entity.Id)
                .Get()
                .Any();
        }

        internal static void CreateTable<T>()
        {
            //_database
        }
    }
}