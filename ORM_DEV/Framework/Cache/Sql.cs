using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using ORM_DEV.Framework.Entities;
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
                .Where("Id", entity.Id)
                .Update(entity.GetType().GetDbFields(entity));
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
                .FirstOrDefault<T>();
        }

        internal static bool Exists(EntityBase entity)
        {
            return _database
                .Query(entity.GetType().GetTableName())
                .Where("Id", entity.Id)
                .FirstOrDefault() != null;
        }

        internal static void CreateTable(this Type type)
        {
            //List<string> fieldDefinitions = new List<string>();
            //foreach (PropertyInfo field in type.GetDbFields(false))
            //{
                //fieldDefinitions.Add($"`{field.Name}` {}");
            //}
            /*
            _database.Connection.Execute($@"CREATE TABLE IF NOT EXISTS `{type.GetTableName()}` (
                
            )");
            */
        }
    }
}