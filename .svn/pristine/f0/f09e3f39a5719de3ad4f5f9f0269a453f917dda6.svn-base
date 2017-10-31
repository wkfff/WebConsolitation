using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Krista.FM.Providers.OracleDataAccess
{
    public class OracleDbDataAdapter : DbDataAdapter, IDbDataAdapter
    {
        private OracleDataAdapter adapter = null;

        public OracleDbDataAdapter()
        {
            adapter = new OracleDataAdapter();
        }

        public new DbCommand SelectCommand
        {
            get 
            {
                return (DbCommand)base.SelectCommand; 
            }
            set 
            {
                base.SelectCommand = value; 
            }
        }

        public new DbCommand InsertCommand
        {
            get 
            {
                return (DbCommand)base.InsertCommand; 
            }
            set 
            {
                base.InsertCommand = value; 
            }
        }
	
        public new DbCommand UpdateCommand
        {
            get 
            {
                return (DbCommand)base.UpdateCommand; 
            }
            set 
            { 
                base.UpdateCommand = value; 
            }
        }

        public new DbCommand DeleteCommand
        {
            get 
            {
                return (DbCommand)base.DeleteCommand; 
            }
            set 
            {
                base.DeleteCommand = value; 
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (this.adapter != null)
                        this.adapter.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
