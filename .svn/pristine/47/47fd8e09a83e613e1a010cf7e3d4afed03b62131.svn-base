using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client; // For Oracle

namespace Krista.FM.Utils.SQLEditor
{
    class OneTimeVars
    {
        private static OneTimeVars instance;
        private OracleConnection con = null;
        //private string strDatabase = null;
        private bool bIsHighPriv = false;
        private string strDbaPriv = null;
        private string strDbVersion = null;
        private int nMainFrmWidth = 0;
        private int nMainFrmHeight = 0;
        private int nTreeWidth = 0;

        // Constructor
        protected OneTimeVars(string connectionString)
        {
            con = new OracleConnection();
            con.ConnectionString = connectionString;
            con.Open();
        }

        public static OneTimeVars Instance(string connectionString)
        {
            if (instance == null)
            {
                instance = new OneTimeVars(connectionString);
            }

            return instance;
        }

        public OracleConnection OracleConn
        {
            get
            {
                return con;
            }

            set
            {
                this.con = value;
            }
        }

        public string getDatabase()
        {
            return con.DataSource;
        }

        public bool getIsHighPriv()
        {
            return bIsHighPriv;
        }

        public void setIsHighPriv(bool bNewStatus)
        {
            if (!bNewStatus)
            {
                strDbaPriv = "";
            }
            bIsHighPriv = bNewStatus;
        }

        public string getDbaPriv()
        {
            return strDbaPriv;
        }

        public void setDbaPriv(string strNewPriv)
        {
            if (strNewPriv == "SYSDBA" || strNewPriv == "SYSOPER")
            {
                strDbaPriv = strNewPriv;
                bIsHighPriv = true;
            }
            else
            {
                bIsHighPriv = false;
            }
        }

        public string getDbVersion()
        {
            return strDbVersion;
        }

        public void setDbVersion(string strDbNewVer)
        {
            strDbVersion = strDbNewVer;
        }

        public int getMainFrmWidth()
        {
            return nMainFrmWidth;
        }

        public void setMainFrmWidth(int nWidth)
        {
            nMainFrmWidth = nWidth;
        }

        public int getMainFrmHeight()
        {
            return nMainFrmHeight;
        }

        public void setMainFrmHeight(int nHeight)
        {
            nMainFrmHeight = nHeight;
        }

        public int getTreeWidth()
        {
            return nTreeWidth;
        }

        public void setTreeWidth(int nWidth)
        {
            nTreeWidth = nWidth;
        }

    }
}
