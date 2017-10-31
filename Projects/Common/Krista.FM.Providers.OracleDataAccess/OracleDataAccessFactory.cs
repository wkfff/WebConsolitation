using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Security.Permissions;
using System.Security;

namespace Krista.FM.Providers.OracleDataAccess
{
    public class OracleDataAccessFactory : DbProviderFactory
    {
        public static readonly OracleDataAccessFactory Instance = new OracleDataAccessFactory();

        // Summary:
        //     Initializes a new instance of a System.Data.Common.DbProviderFactory class.
        protected OracleDataAccessFactory()
            : base()
        {

        }


        // Summary:
        //     Specifies whether the specific System.Data.Common.DbProviderFactory supports
        //     the System.Data.Common.DbDataSourceEnumerator class.
        //
        // Returns:
        //     true if the instance of the System.Data.Common.DbProviderFactory supports
        //     the System.Data.Common.DbDataSourceEnumerator class; otherwise false.
        public override bool CanCreateDataSourceEnumerator 
        { 
            get {return true; } 
        }

        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbCommand
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbCommand.
        public override DbCommand CreateCommand()
        {
            return new OracleDbCommand();
        }

        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbCommandBuilder
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbCommandBuilder.
        public override DbCommandBuilder/*DbCommandBuilder*/ CreateCommandBuilder()
        {
            return new OracleDbCommandBuilder();
        }

        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbConnection
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbConnection.
        public override DbConnection CreateConnection()
        {
            return new OracleDbConnection();
        }
        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbConnectionStringBuilder
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbConnectionStringBuilder.
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return null;
        }
        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbDataAdapter
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbDataAdapter.
        public override DbDataAdapter CreateDataAdapter()
        {
            return new OracleDbDataAdapter();
        }
        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbDataSourceEnumerator
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbDataSourceEnumerator.
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return null;
        }
        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the System.Data.Common.DbParameter
        //     class.
        //
        // Returns:
        //     A new instance of System.Data.Common.DbParameter.
        public override DbParameter CreateParameter()
        {
            return new OracleDbParameter();
        }
        //
        // Summary:
        //     Returns a new instance of the provider's class that implements the provider's
        //     version of the System.Security.CodeAccessPermission class.
        //
        // Parameters:
        //   state:
        //     One of the System.Security.Permissions.PermissionState values.
        //
        // Returns:
        //     A System.Data.Common.CodeAccessPermission object for the specified System.Security.Permissions.PermissionState.
        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            return null;
        }
    }
}
