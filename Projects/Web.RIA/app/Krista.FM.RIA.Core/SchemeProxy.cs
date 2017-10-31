using System;
using System.Data;
using System.Web;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.WriteBackLibrary;

namespace Krista.FM.RIA.Core
{
    public class SchemeProxy : IScheme
    {
        public string Name
        {
            get { return Instance.Name; }
            set { throw new NotImplementedException(); }
        }

        public IServer Server
        {
            get { return Instance.Server; }
            set { throw new NotImplementedException(); }
        }

        public ISchemeDWH SchemeDWH
        {
            get { return Instance.SchemeDWH; }
        }

        public ISchemeMDStore SchemeMDStore
        {
            get { return Instance.SchemeMDStore; }
        }

        public IDataSourceManager DataSourceManager
        {
            get { return Instance.DataSourceManager; }
        }

        public IDataVersionManager DataVersionsManager
        {
            get { throw new NotImplementedException(); }
        }

        public IDataPumpManager DataPumpManager
        {
            get { return Instance.DataPumpManager; }
        }

        public ITaskManager TaskManager
        {
            get { return Instance.TaskManager; }
        }

        public ITemplatesService TemplatesService
        {
            get { return Instance.TemplatesService; }
        }

        public IUsersManager UsersManager
        {
            get { return Instance.UsersManager; }
        }

        public IArchive Archive
        {
            get { throw new NotImplementedException(); }
        }

        public string BaseDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public string ArchiveDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public string DataSourceDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public IGlobalConstsManager GlobalConstsManager
        {
            get { return Instance.GlobalConstsManager; }
        }

        public IPackage RootPackage
        {
            get { return Instance.RootPackage; }
        }

        public IPackageCollection Packages
        {
            get { return Instance.Packages; }
        }

        public IEntityCollection<IClassifier> Classifiers
        {
            get { return Instance.Classifiers; }
        }

        public IEntityCollection<IFactTable> FactTables
        {
            get { return Instance.FactTables; }
        }

        public IEntityAssociationCollection Associations
        {
            get { return Instance.Associations; }
        }

        public IEntityAssociationCollection AllAssociations
        {
            get { return Instance.AllAssociations; }
        }

        public IConversionTableCollection ConversionTables
        {
            get { throw new NotImplementedException(); }
        }

        public ISemanticsCollection Semantics
        {
            get { return Instance.Semantics; }
        }

        public IPlaningProvider PlaningProvider
        {
            get { throw new NotImplementedException(); }
        }

        public IWriteBackServer WriteBackServer
        {
            get { throw new NotImplementedException(); }
        }

        public IDisintRules DisintRules
        {
            get { throw new NotImplementedException(); }
        }

        public IFinSourcePlanningFace FinSourcePlanningFace
        {
            get { return Instance.FinSourcePlanningFace; }
        }

        public IForecastService ForecastService
        {
            get { return Instance.ForecastService; }
        }

        public IForm2pService Form2pService
        {
            get { return Instance.Form2pService; }
        }

        public ISessionManager SessionManager
        {
            get { return Instance.SessionManager; }
        }

        public bool IsLocked
        {
            get { throw new NotImplementedException(); }
        }

        public int LockedByUserID
        {
            get { throw new NotImplementedException(); }
        }

        public string LockedByUserName
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEndPoint
        {
            get { throw new NotImplementedException(); }
        }

        public ServerSideObjectStates State
        {
            get { throw new NotImplementedException(); }
        }

        public IServerSideObject OwnerObject
        {
            get { throw new NotImplementedException(); }
        }

        public int Identifier
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsClone
        {
            get { throw new NotImplementedException(); }
        }

        public IServerSideObject ICloneObject
        {
            get { throw new NotImplementedException(); }
        }

        public IProcessor Processor
        {
            get { return Instance.Processor; }
        }

        public DataTable ServerSystemInfo
        {
            get { throw new NotImplementedException(); }
        }

        public IVSSFacade VSSFacade
        {
            get { throw new NotImplementedException(); }
        }

        public string ConfigurationXMLDocumentation
        {
            get { throw new NotImplementedException(); }
        }

        public bool MultiServerMode
        {
            get { throw new NotImplementedException(); }
        }

        public IReportsTreeService ReportsTreeService
        {
            get { throw new NotImplementedException(); }
        }

        public IMessageManager MessageManager
        {
            get
            {
                return Instance.MessageManager;
            }
        }

        private static IScheme Instance
        {
            get
            {
                var scheme = HttpContext.Current.Session[ConnectionHelper.SchemeKeyName] as IScheme;
                if (scheme == null)
                {
                    throw new Exception("Вызов метода SchemeProxy.Instance вне контекста сессии.");
                }

                return scheme;
            }
        }

        public IModificationItem GetChanges(IModifiable toObject)
        {
            return Instance.GetChanges(toObject);
        }

        public IModificationItem GetChanges()
        {
            return Instance.GetChanges();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IServerSideObject Lock()
        {
            throw new NotImplementedException();
        }

        public void Unlock()
        {
            throw new NotImplementedException();
        }

        public SMOSerializationInfo GetSMOObjectData()
        {
            return Instance.GetSMOObjectData();
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            return Instance.GetSMOObjectData(level);
        }

        public object GetService(Type serviceType)
        {
            return Instance.GetService(serviceType);
        }

        public IBaseProtocol GetProtocol(string callerAssemblyName)
        {
            return Instance.GetProtocol(callerAssemblyName);
        }

        public IDataOperations GetAudit()
        {
            throw new NotImplementedException();
        }

        public IExportImportManager GetXmlExportImportManager()
        {
            throw new NotImplementedException();
        }

        public ICommonDBObject GetObjectByKey(string key)
        {
            return Instance.GetObjectByKey(key);
        }

        public IModificationContext CreateModificationContext()
        {
            throw new NotImplementedException();
        }

        public TransferDBToNewYearState TransferDBToNewYear(int currentYear)
        {
            throw new NotImplementedException();
        }
    }
}
