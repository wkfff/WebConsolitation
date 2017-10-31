using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Оболочка для фабрики объектов доступа к базе данных
    /// </summary>
    public class DbProviderFactoryWrapper : DbProviderFactory
    {
        /// <summary>
        /// Реальная фабрика
        /// </summary>
        private DbProviderFactory factory;

        /// <summary>
        /// Инициализирует новый класс-оболочку для фабрики классов.
        /// </summary>
        /// <param name="factory">Обрамляемая фабрика классов</param>
        public DbProviderFactoryWrapper(DbProviderFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Тип подключения
        /// </summary>
        internal ProviderTypes ProviderType
        {
            get
            {
                switch (factory.GetType().Name)
                {
                    case "OracleClientFactory":
                    case "OracleDataAccessFactory":
                        return ProviderTypes.OracleClient;
                    case "SqlClientFactory":
                        return ProviderTypes.SqlClient;
                    default:
                        throw new Exception(String.Format("Тип провайдера {0} не поддерживается.", factory.GetType().Name));
                }
            }
        }

        /// <summary>
        /// Specifies whether the specific System.Data.Common.DbProviderFactory supports the System.Data.Common.DbDataSourceEnumerator class.
        /// </summary>
        public override bool CanCreateDataSourceEnumerator 
        {
            get { return factory.CanCreateDataSourceEnumerator; }
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbCommand class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbCommand.</returns>
        public override DbCommand CreateCommand()
        {
            return factory.CreateCommand();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbCommandBuilder class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbCommandBuilder.</returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return factory.CreateCommandBuilder();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbConnection class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbConnection.</returns>
        public override DbConnection CreateConnection()
        {
            return new DbConnectionWrapper(factory.CreateConnection());
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbConnectionStringBuilder class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbConnectionStringBuilder.</returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return factory.CreateConnectionStringBuilder();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbDataAdapter class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbDataAdapter.</returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return factory.CreateDataAdapter();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbDataSourceEnumerator class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbDataSourceEnumerator.</returns>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return factory.CreateDataSourceEnumerator();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbParameter class.
        /// </summary>
        /// <returns>A new instance of System.Data.Common.DbParameter.</returns>
        public override DbParameter CreateParameter()
        {
            return factory.CreateParameter();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the provider's version of the System.Security.CodeAccessPermission class.
        /// </summary>
        /// <param name="state">One of the System.Security.Permissions.PermissionState values.</param>
        /// <returns>A System.Data.Common.CodeAccessPermission object for the specified System.Security.Permissions.PermissionState.</returns>
        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            return factory.CreatePermission(state);
        }
    }
}
