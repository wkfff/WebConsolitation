using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Data.OracleClient;

namespace Krista.FM.Providers.MSOracleDataAccess
{
    public class OracleDbTransaction : DbTransaction
    {
        private OracleTransaction transaction = null;

        public OracleTransaction OracleTransaction
        {
			get { return transaction; }
        }

        public OracleDbTransaction(OracleTransaction transaction)
        {
            this.transaction = transaction; 
        }

        public override void Commit()
        {
            transaction.Commit();
        }

        public override void Rollback()
        {
            transaction.Rollback();
        }

        protected override DbConnection DbConnection
        {
            get 
            {
                OracleDbConnection conn = new OracleDbConnection(transaction.Connection);
                return conn;
            }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (transaction != null)
                        transaction.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
