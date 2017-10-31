using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoClassifierDesign : SmoDataSourceDividedClassDesign, IClassifier
    {
        public SmoClassifierDesign(IClassifier serverControl)
            : base(serverControl)
        {
        }
        
        [Browsable(false)]
        public new IClassifier ServerControl
        {
            get { return (IClassifier)serverControl; }
        }

        #region IClassifier Members

        [Browsable(false)]
        public IDimensionLevelCollection Levels
        {
            get
            {
                return
                    ServerControl.Levels;
            }
        }

        public Dictionary<int, string> GetDataSourcesNames()
        {
            return ServerControl.GetDataSourcesNames();
        }

        public int FormCubesHierarchy()
        {
            return ServerControl.FormCubesHierarchy();
        }

        public int DivideAndFormHierarchy(int SourceID, bool setFullHierarchy)
        {
            return ServerControl.DivideAndFormHierarchy(SourceID, setFullHierarchy);
        }

        public int DivideAndFormHierarchy(int SourceID, IDatabase database)
        {
            return ServerControl.DivideAndFormHierarchy(SourceID, database);
        }

        public int DivideAndFormHierarchy(int SourceID, int dataPumpID, ref System.Data.DataSet clsDataSet)
        {
            return ServerControl.DivideAndFormHierarchy(SourceID, dataPumpID, ref clsDataSet);
        }

        public int DivideClassifierCode(int SourceID)
        {
            return ServerControl.DivideClassifierCode(SourceID);
        }

        public int UpdateFixedRows(int sourceID)
        {
            return ServerControl.UpdateFixedRows(sourceID);
        }

        public int UpdateFixedRows(IDatabase db, int sourceID)
        {
            return ServerControl.UpdateFixedRows(db, sourceID);
        }

        public void CreateOlapObject()
        {
            ServerControl.CreateOlapObject();
        }

        public object GetLookupAttribute(IDatabase db, int rowID, string attributeName)
        {
            return ServerControl.GetLookupAttribute(db, rowID, attributeName);
        }

        public object GetLookupAttribute(int rowID, string attributeName)
        {
            return ServerControl.GetLookupAttribute(rowID, attributeName);
        }

        public System.Data.DataTable GetFixedRowsTable()
        {
            return ServerControl.GetFixedRowsTable();
        }

        public void SetFixedRowsTable(System.Data.DataTable dt)
        {
            ServerControl.SetFixedRowsTable(dt);
            CallOnChange();
        }

        public int[] ReverseRowsRange(int[] rowsID)
        {
            return ServerControl.ReverseRowsRange(rowsID);
        }

        #endregion
    }
}
