using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Data.OracleClient;

namespace Krista.FM.Providers.MSOracleDataAccess
{
    class OracleDbCommandBuilder : DbCommandBuilder
    {
        OracleCommandBuilder commandBuilder = null;

        public OracleDbCommandBuilder()
        {
            commandBuilder = new OracleCommandBuilder();
        }

        public OracleDbCommandBuilder(OracleDataAdapter adapter)
        {
            commandBuilder = new OracleCommandBuilder(adapter);
        }

        public OracleDbCommandBuilder(OracleCommandBuilder commandBuilder)
        {
            //this.commandBuilder = commandBuilder;
            base.DataAdapter = commandBuilder.DataAdapter;
        }

        public override string QuoteIdentifier(string unquotedIdentifier)
        {
            if (unquotedIdentifier == null)
            {
                throw new ArgumentNullException("Unquoted identifier parameter cannot be null");
            }

            return String.Format("{0}{1}{2}", this.QuotePrefix, unquotedIdentifier, this.QuoteSuffix);
        }

        public override string UnquoteIdentifier(string quotedIdentifier)
        {
            if (quotedIdentifier == null)
            {
                throw new ArgumentNullException("Quoted identifier parameter cannot be null");
            }

            string unquotedIdentifier = quotedIdentifier.Trim();

            if (unquotedIdentifier.StartsWith(this.QuotePrefix))
            {
                unquotedIdentifier = unquotedIdentifier.Remove(0, 1);
            }
            if (unquotedIdentifier.EndsWith(this.QuoteSuffix))
            {
                unquotedIdentifier = unquotedIdentifier.Remove(unquotedIdentifier.Length - 1, 1);
            }

            return unquotedIdentifier;
        }

        protected override void ApplyParameterInfo(DbParameter p, DataRow row, StatementType statementType, bool whereClause)
        {
            DbParameter parameter = (DbParameter)p;

            parameter.Size = int.Parse(row["ColumnSize"].ToString());

			parameter.DbType = (DbType)row["ProviderType"];

			switch (((Type)row["DataType"]).Name)
			{
				case "Decimal": parameter.DbType = DbType.Int32; break;
			}

        }

        protected override string GetParameterName(int parameterOrdinal)
        {
            return string.Format(":p{0}", parameterOrdinal);
        }

        protected override string GetParameterName(string parameterName)
        {
            return string.Format(":{0}", parameterName);
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            return this.GetParameterName(parameterOrdinal);
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            /*if (!(adapter is DataAdapter))
            {
                throw new InvalidOperationException("adapter needs to be a FbDataAdapter");
            }

            this.rowUpdatingHandler = new FbRowUpdatingEventHandler(this.RowUpdatingHandler);
            ((FbDataAdapter)adapter).RowUpdating += this.rowUpdatingHandler;
             * */
        }
    }
}
