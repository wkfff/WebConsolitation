using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SmoDataSourceManager : ServerManagedObject<IDataSourceManager>, IDataSourceManager
    {
        public SmoDataSourceManager(IDataSourceManager serverObject)
            : base(serverObject)
        {
        }

        public override string ToString()
        {
            return String.Empty;
        }

        #region IDataSourceManager Members

        public string BaseDirectory
        {
            get { return serverControl.BaseDirectory; }
        }

        public string ArchiveDirectory
        {
            get { return serverControl.ArchiveDirectory; }
        }

        public IPumpRegistryCollection PumpRegistryCollection
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IDataSourceCollection DataSources
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IScheme Scheme
        {
            get { return serverControl.Scheme; }
        }

        public IDataSupplierCollection DataSuppliers
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public DataTable GetDataSourcesInfo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetDataSourceName(int SourceID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<int, string> GetDataSourcesNames(string tableName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDataUpdater DataSourcesDataUpdater
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IDataSourceManager Members


        public DataTable GetDataSourcesInfo(string dataSourceKinds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
