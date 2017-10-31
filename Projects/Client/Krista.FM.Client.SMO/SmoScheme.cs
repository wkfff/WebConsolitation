using System;
using System.ComponentModel;
using System.Diagnostics;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.TemplatesService;


namespace Krista.FM.Client.SMO
{
    public class SmoScheme : SMOSerializableObject<IScheme>, IScheme
    {
        public SmoScheme(IScheme serverControl)
            : base(serverControl)
        {
        }

        public SmoScheme(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        [DisplayName("Объект сервера (ServerControl)")]
        [Description("Объект сервера")]
        private new IScheme ServerControl
        {
            get { return serverControl; }
        }
        
        #region IScheme Members

        [DisplayName("Наименование схемы (Name)")]
        [Description("Наименование схемы")]
        public string Name
        {
            get
            {
                return cached ? Convert.ToString(GetCachedValue("Name")) : ServerControl.Name;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IServer Server
        {
            get
            {
                return ServerControl.Server;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISchemeDWH SchemeDWH
        {
            get { return ServerControl.SchemeDWH; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISchemeMDStore SchemeMDStore
        {
            get { return ServerControl.SchemeMDStore; }
        }

		[Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IProcessor Processor
		{
            get { return ServerControl.Processor; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDataSourceManager DataSourceManager
        {
            get { return ServerControl.DataSourceManager; }
        }
        
        [Browsable(true)]
        [DisplayName("Ссылка на объект (DataSourceManager2)")]
        [Description("Возвращает ссылку на объект, управляющий источниками данных схемы")]
        public SmoDataSourceManager DataSourceManager2
        {
            get { return new SmoDataSourceManager(ServerControl.DataSourceManager); }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ITaskManager TaskManager
        {
            get { return ServerControl.TaskManager; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IUsersManager UsersManager
        {
            get { return ServerControl.UsersManager; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IArchive Archive
        {
            get { return ServerControl.Archive; }
        }

        [DisplayName("Базовый каталог схемы (BaseDirectory)")]
        [Description("Возвращает базовый каталог схемы")]
        public string BaseDirectory
        {
            get { return cached ? Convert.ToString(GetCachedValue("BaseDirectory")) : ServerControl.BaseDirectory; }
        }

        [DisplayName("Архивный каталог схемы (ArchiveDirectory)")]
        [Description("Путь к расшаренному архивному каталогу")]
        public string ArchiveDirectory
        {
            get { return cached ? Convert.ToString(GetCachedValue("ArchiveDirectory")) : ServerControl.ArchiveDirectory; }
        }

        [DisplayName("Каталог источников данных (DataSourceDirectory)")]
        [Description("Путь к расшаренному каталогу источников данных")]
        public string DataSourceDirectory
        {
            get { return cached ? Convert.ToString(GetCachedValue("DataSourceDirectory")) : ServerControl.DataSourceDirectory; }
        }

        [Browsable(false)]
        public IBaseProtocol GetProtocol(string CallerAssemblyName)
        {
            return ServerControl.GetProtocol(CallerAssemblyName);
        }

        [Browsable(false)]
        public IDataOperations GetAudit()
        {
            return ServerControl.GetAudit();
        }

        [Browsable(false)]
        public IExportImportManager GetXmlExportImportManager()
        {
            return ServerControl.GetXmlExportImportManager();
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IGlobalConstsManager GlobalConstsManager
        {
            get { return ServerControl.GlobalConstsManager; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IFinSourcePlanningFace FinSourcePlanningFace
        {
            get { return new SmoFinSourcePlanningFace(ServerControl.FinSourcePlanningFace); }
        }

		[Browsable(false)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IForecastService ForecastService
		{
			get { return ServerControl.ForecastService; }
		}

		[Browsable(false)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IForm2pService Form2pService
		{
			get { return ServerControl.Form2pService; }
		}


        [Browsable(false)]
        public IEntityCollection<IClassifier> Classifiers
        {
            get
            {
                return cached ? 
                    (IEntityCollection<IClassifier>)SmoObjectsCache.GetSmoObject(
                                                    typeof(SmoClassifierCollection),
                                                    GetCachedObject("Classifiers")) 
                    : ServerControl.Classifiers;
            }
        }

        [Browsable(false)]
        public IEntityCollection<IFactTable> FactTables
        {
            get
            {
                return cached
                           ? (IEntityCollection<IFactTable>)SmoObjectsCache.GetSmoObject(
                                                                  typeof (SmoFactTableCollection),
                                                                  GetCachedObject("FactTables"))
                           : ServerControl.FactTables;
            }
        }

        [Browsable(false)]
        public IEntityAssociationCollection Associations
        {
            get
            {
                return cached
                        ? (IEntityAssociationCollection)SmoObjectsCache.GetSmoObject(
                                                               typeof(SmoEntityAssociationColection),
                                                               GetCachedObject("Associations"))
                        : ServerControl.Associations;
            }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IConversionTableCollection ConversionTables
        {
            get { return ServerControl.ConversionTables; }
        }

        [Browsable(false)]
        public ISemanticsCollection Semantics
        {
            get {
                return
                    cached
                        ? (ISemanticsCollection)SmoObjectsCache.GetSmoObject(typeof(SmoSemanticsCollection), GetCachedObject("Semantics"))
                        : ServerControl.Semantics; }
            set { throw new Exception("The method or operation is not implemented."); }
        }

       
        [Browsable(true)]
        [DisplayName("Список семантик (Semantics)")]
        [Description("Выводит список семантических принадлежностей")]
        public SmoDictionary2<ISemanticsCollection, string, string> Semantics2
        {
            get
            {
                return new SmoDictionary2<ISemanticsCollection, string, string>(ServerControl.Semantics);
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
                
        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPlaningProvider PlaningProvider
        {
            get { return ServerControl.PlaningProvider; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Krista.FM.WriteBackLibrary.IWriteBackServer WriteBackServer
        {
            get { return ServerControl.WriteBackServer; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisintRules DisintRules
        {
            get { return ServerControl.DisintRules; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDataPumpManager DataPumpManager
        {
            get { return ServerControl.DataPumpManager; }
        }

        [Browsable(false)]
        public IPackage RootPackage
        {
            get
            {
            	return
            		cached
					? (IPackage)SmoObjectsCache.GetSmoObject(typeof(SmoPackage), ServerControl.RootPackage)
					: ServerControl.RootPackage;
            }
        }

        [Browsable(false)]
        public IPackageCollection Packages
        {
            get { return ServerControl.Packages; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISessionManager SessionManager
        {
            get { return ServerControl.SessionManager; }
        }

        public ICommonDBObject GetObjectByKey(string key)
        {
            return ServerControl.GetObjectByKey(key);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public System.Data.DataTable ServerSystemInfo
        {
            get { return serverControl.ServerSystemInfo; }
        }

        public IModificationContext CreateModificationContext()
        {
            return serverControl.CreateModificationContext();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IVSSFacade VSSFacade
        {
            get { return serverControl.VSSFacade; }
        }


        [Browsable(false)]
        public ITemplatesService TemplatesService 
        {
            get { return serverControl.TemplatesService; }
        }

        public IEntityAssociationCollection AllAssociations
        {
            get { return ServerControl.AllAssociations; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ConfigurationXMLDocumentation
        {
            get { return ServerControl.ConfigurationXMLDocumentation; }
        }

        public bool MultiServerMode
        {
            get { return ServerControl.MultiServerMode; }
        }

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDataVersionManager DataVersionsManager
        {
            get { return ServerControl.DataVersionsManager; }
        }

        public IReportsTreeService ReportsTreeService
        {
            get { return ServerControl.ReportsTreeService; }
        }

        [Browsable(false)]
        public IMessageManager MessageManager
        {
            get { return serverControl.MessageManager; }
        }

        [Browsable(false)]
        public TransferDBToNewYearState TransferDBToNewYear(int currentYear)
        {
            return serverControl.TransferDBToNewYear(currentYear);
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return ServerControl.GetService(serviceType);
        }

        #endregion
        
        #region IModifiable Members

        public IModificationItem GetChanges(IModifiable toObject)
        {
            return ServerControl.GetChanges(toObject);
        }

        #endregion

        #region IMinorModifiable Members

/*        public void Update(IModifiable toObject)
        {
            ServerControl.Update(toObject);
        }*/

        #endregion

        #region IMajorModifiable Members

        public IModificationItem GetChanges()
        {
            return ServerControl.GetChanges();
        }

        #endregion

    }
}
