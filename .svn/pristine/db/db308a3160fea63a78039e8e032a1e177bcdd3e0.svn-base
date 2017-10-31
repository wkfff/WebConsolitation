using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class SystemDataSourcesClassifier : Entity, IClassifier
    {
        public SystemDataSourcesClassifier(ServerSideObject owner)
            : base(SystemSchemeObjects.SystemDataSources_ENTITY_KEY, owner, "System", "DataSources", ClassTypes.clsFixedClassifier, SubClassTypes.System, ServerSideObjectStates.Consistent, SchemeClass.ScriptingEngineFactory.NullScriptingEngine)
		{
            this.ID = -1;
            this.Caption = "Источники данных";
            this.Description = "Системный классификатор \"Источники данных\"";
        }

        public override string TablePrefix
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return "fx"; }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return TablePrefix + "." + base.FullName; }
        }

        public override string FullDBName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return "DataSources";
            }
        }

        public override string OlapName
        {
            get { return Caption; }
        }

        public override bool CurrentUserCanViewThisObject()
        {
            return false;
        }

        #region IClassifier Members

        public IDimensionLevelCollection Levels
        {
            get { return null; }
        }

        public Dictionary<int, string> GetDataSourcesNames()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FormCubesHierarchy()
        {
            return 0;
        }

        public int DivideAndFormHierarchy(int SourceID, bool setFullHierarchy)
        {
            return 0;
        }

        public int DivideAndFormHierarchy(int SourceID, int dataPumpID, ref System.Data.DataSet clsDataSet)
        {
            return 0;
        }

        public int DivideAndFormHierarchy(int SourceID, IDatabase database)
        {
            return 0;
        }

        public int DivideClassifierCode(int SourceID)
        {
            return 0;
        }

        public int UpdateFixedRows(int sourceID)
        {
            return 0;
        }

        public int UpdateFixedRows(IDatabase db, int sourceID)
        {
            return 0;
        }

        public void CreateOlapObject()
        {
        }

        public object GetLookupAttribute(IDatabase db, int rowID, string attributeName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetLookupAttribute(int rowID, string attributeName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.DataTable GetFixedRowsTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetFixedRowsTable(System.Data.DataTable dt)
        {
        }

        public int[] ReverseRowsRange(int[] rowsID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDataSourceDividedClass Members

        public ParamKindTypes DataSourceParameter(int dataSource)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsDivided
        {
            get { return false; }
        }

        #endregion

        #region IDataSourceDividedClass Members


        public string DataSourceKinds
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
