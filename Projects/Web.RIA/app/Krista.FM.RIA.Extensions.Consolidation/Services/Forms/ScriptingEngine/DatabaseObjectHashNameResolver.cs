using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    /// <summary>
    /// create table HashObjectsNames
    /// (
    ///     HashName varchar2(30) not null,
    ///     LongName varchar2(2048) not null,
    ///     ObjectType number(10) not null,
    ///     constraint PKHashObjectsNames primary key (HashName, ObjectType)
    /// );
    /// </summary>
    public class DatabaseObjectHashNameResolver : IDatabaseObjectHashNameResolver
    {
        private readonly IScheme scheme;

        public DatabaseObjectHashNameResolver(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// Возвращает сужествующее хеш-имя, для имен длиннее 30 символов, или null, если хеш-имя не существует.
        /// Формат имени: [первые 20 символов от оригинального мени]$[хеш (7 символов base64)]$[количество коллизий]
        /// </summary>
        /// <param name="longName">Имя объекта.</param>
        /// <param name="objectType">Тип объекта.</param>
        public string Get(string longName, ObjectTypes objectType)
        {
            if (longName.Length <= 30)
            {
                return longName;
            }

            using (var db = scheme.SchemeDWH.DB)
            {
                var exist = (DataTable)db.ExecQuery(
                    "select HashName, LongName from HashObjectsNames where LongName = ? and ObjectType = ?",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("LongName", longName.ToUpper()),
                    new DbParameterDescriptor("ObjectType", (int)objectType));

                if (exist.Rows.Count == 1)
                {
                    return '"' + Convert.ToString(exist.Rows[0]["HashName"]) + '"';
                }

                return null;
            }
        }

        /// <summary>
        /// Создает хеш-имя, для имен длиннее 30 символов. 
        /// Если такое имя уже зарегистрировано, то кидает исключение InvalidOperationException.
        /// Формат имени: [первые 20 символов от оригинального мени]$[хеш (7 символов base64)]$[количество коллизий]
        /// </summary>
        /// <param name="longName">Имя объекта.</param>
        /// <param name="objectType">Тип объекта.</param>
        public string Create(string longName, ObjectTypes objectType)
        {
            if (longName.Length <= 30)
            {
                return longName;
            }

            longName = longName.ToUpper();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(longName);
            byte[] hashBytes = MD5.Create().ComputeHash(plainTextBytes);
            var hash = Convert.ToBase64String(hashBytes, 0, 5).Substring(0, 7);
            int collisions = 0;

            using (new Core.ServerContext())
            using (var db = scheme.SchemeDWH.DB)
            {
                var exist = (DataTable)db.ExecQuery(
                    "select HashName, LongName from HashObjectsNames where LongName = ? and ObjectType = ?",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("LongName", longName),
                    new DbParameterDescriptor("ObjectType", (int)objectType));

                if (exist.Rows.Count != 0)
                {
                    throw new DatabaseObjectHashNameException("Для имени \"{0}\" хеш-имя уже создано.".FormatWith(longName));
                }

                while (true)
                {
                    try
                    {
                        var hashName = "{0}${1}${2}".FormatWith(longName.Substring(0, 20), hash, collisions);
                        db.ExecQuery(
                            "insert into HashObjectsNames (HashName, LongName, ObjectType) values (?, ?, ?)",
                            QueryResultTypes.NonQuery,
                            new DbParameterDescriptor("HashName", hashName),
                            new DbParameterDescriptor("LongName", longName),
                            new DbParameterDescriptor("ObjectType", (int)objectType));

                        return '"' + hashName + '"';
                    }
                    catch (InvalidOperationException e)
                    {
                        if (e.InnerException.Message.StartsWith("ORA-00001"))
                        {
                            collisions += 1;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет хеш-имя, для имен длиннее 30 символов.
        /// </summary>
        /// <param name="longName">Имя объекта.</param>
        /// <param name="objectType">Тип объекта.</param>
        public void Delete(string longName, ObjectTypes objectType)
        {
            if (longName.Length <= 30)
            {
                return;
            }

            using (var db = scheme.SchemeDWH.DB)
            {
                db.ExecQuery(
                    "delete from HashObjectsNames where LongName = ? and ObjectType = ?",
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("LongName", longName.ToUpper()),
                    new DbParameterDescriptor("ObjectType", (int)objectType));
            }
        }
    }
}
