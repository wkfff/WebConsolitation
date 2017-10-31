using System.Configuration;

using Krista.FM.Common;

namespace Krista.FM.RIA.Core.Helpers
{
    public class DatabaseInfo
    {
        public string FactoryName { get; set; }

        public string ServerVersion { get; set; }

        public ConnectionString ConnectionString { get; set; }

        public static DatabaseInfo Detect(string cs)
        {
            var info = new DatabaseInfo();

            info.ConnectionString = new ConnectionString();
            info.ConnectionString.Parse(cs);
            if (info.ConnectionString.Provider.ToUpper().Contains("ORA"))
            {
                info.FactoryName = "Oracle";
                var connection = new Oracle.DataAccess.Client.OracleConnection(info.ConnectionString.ToString());
                connection.Open();
                info.ServerVersion = connection.ServerVersion;
                connection.Close();
            }
            else if (info.ConnectionString.Provider.ToUpper().Contains("SQL"))
            {
                info.FactoryName = "Sql";
                var connection = new System.Data.SqlClient.SqlConnection(info.ConnectionString.ToString());
                connection.Open();
                info.ServerVersion = connection.ServerVersion;
                connection.Close();
            }
            else
            {
                throw new ConfigurationErrorsException("В строке подключения указан недопустимый провайдер.");
            }
            
            return info;
        }
    }
}
