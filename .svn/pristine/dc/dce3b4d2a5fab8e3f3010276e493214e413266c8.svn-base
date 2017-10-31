using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;

namespace Krista.FM.Domain.Reporitory.NHibernate.MsSql
{
    public class MsSqlIdGenerator : TableGenerator
    {
        public override object Generate(ISessionImplementor session, object obj)
        {
            var generatorName = obj.GetType().Name;
            
            // Костыль. 
            // Для Sql маппинг генерации ключей немного отличается от Oracle.
            // Поэтому приходится для 'x' таблиц явно подсовывать другой генератор.
            if (generatorName[0] == 'x')
            {
                generatorName = "D_Report_Row";
            }

            ITransaction t = ((SessionImpl)session).BeginTransaction();
            using (var cmd = session.Connection.CreateCommand())
            {
                t.Enlist(cmd);

                cmd.CommandText = "usp_Generator";
                cmd.CommandType = CommandType.StoredProcedure;

                var pagam = cmd.CreateParameter();
                pagam.ParameterName = "generator";
                pagam.Value = generatorName;
                pagam.DbType = DbType.String;
                pagam.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(pagam);

                pagam = cmd.CreateParameter();
                pagam.ParameterName = "return";
                pagam.DbType = DbType.Int32;
                pagam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pagam);

                cmd.ExecuteNonQuery();

                return Convert.ToInt32(pagam.Value);
            }
        }
    }
}
