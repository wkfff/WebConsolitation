using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using bus.gov.ru.Imports;
using BytesRoad.Net.Ftp;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Extensions;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// The bus gov ru pump module.
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private string _clientKladrCode;
        private string _clientLocationOkatoCodePrefix;
        private List<D_OKATO_OKATO> _okatoCache;
        private Dictionary<string, D_Org_Structure> _orgStructuresByRegnumCache;
        private Dictionary<string, D_Org_PPO> _ppoCache;
        private string _pumpIdentifier;
        private string _rootDir;

        private string ClientLocationOkatoCodePrefix
        {
            get { return _clientLocationOkatoCodePrefix; }
        }

        private string ClientLocationRegionNumber
        {
            get { return _clientKladrCode.Substring(0, 2); }
        }

        private bool IsDocumentsPump()
        {
            //return
            //    _pumpIdentifier.Equals("institutionInfo") ||
            //    _pumpIdentifier.Equals("budgetaryCircumstances") ||
            //    _pumpIdentifier.Equals("stateTask");

            switch (_pumpIdentifier)
            {
                case "institutionInfo":
                case "budgetaryCircumstances":
                case "stateTask":
                case "pfhd":
                    return true;
                case "nsiOgs":
                case "nsiOkato":
                case "nsiPpo":
                case "nsiSubjectService":
                default:
                    return false;
            }
        }

        [UnitOfWork]
        private void ProcessFile(FileInfo fileInfo)
        {
            switch (_pumpIdentifier)
            {
                case "nsiOgs":
                    ProcessNsiOgsPumpFile(fileInfo);
                    break;
                case "nsiOkato":
                    ProcessNsiOkatoPumpFile(fileInfo);
                    break;
                case "nsiPpo":
                    ProcessNsiPpoPumpFile(fileInfo);
                    break;
                case "nsiOktmo":
                    ProcessNsiOktmoPumpFile(fileInfo);
                    break;
                case "nsiSubjectService":
                    ProcessNsiSubjectServicePumpFile(fileInfo);
                    break;
                case "services":
                    new Service2016Pump(ClientLocationRegionNumber).Pump(new StreamReader(fileInfo.FullName), DataPumpProtocol, fileInfo.Name);
                    break;
                case "servicesEb":
                    new Service2016EbPump(ClientLocationRegionNumber).Pump(new StreamReader(fileInfo.FullName), DataPumpProtocol, fileInfo.Name);
                    break;
                case "servicesPlanning":
                    new ServicePlanningPump(ClientLocationRegionNumber).Pump(new StreamReader(fileInfo.FullName, Encoding.GetEncoding("windows-1251")), DataPumpProtocol, fileInfo.Name);
                    break;
            }

            // для документов возможна корректная загрузка каждого отдельного документа 
            // возможные исключения логируем и давим
            try
            {
                using (new PersistenceContext())
                {
                    switch (_pumpIdentifier)
                    {
                        case "institutionInfo":
                            ProcessInstitutionInfoPumpFile(fileInfo);
                            break;
                        case "budgetaryCircumstances":
                            ProcessBudgetaryCircumstancesPumpFile(fileInfo);
                            break;
                        case "stateTask":
                            ProcessStateTaskPumpFile(fileInfo);
                            break;
                        case "pfhd":
                            ProcessPfhdPumpFile(fileInfo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError,
                    string.Format("Закачка файла {0} завершена с ошибками.", fileInfo.FullName),
                    ex);
            }
        }

        /// <summary>
        /// Закачка данных. 
        /// Переопределяется в потомках для выполнения действий по закачке данных.
        /// </summary>
        protected override void DirectPumpData()
        {
            if (IsDocumentsPump())
            {
                PumpDataDocument(RootDir);
            }
            else
            {
                PumpDataSource(RootDir);
            }
        }

        /// <summary>
        /// stub
        /// </summary>
        protected override void MarkClsAsInvalidate()
        {
        }

        protected override void DirectCheckData()
        {
            FixNsiOgs();
        }

        /// <summary>
        /// теперь по назначению используем
        /// </summary>
        protected override void DirectClsHierarchySetting()
        {
            AfterProcessFiles();
        }

        protected override void DirectDeleteData(int pumpID, int sourceID, string constr)
        {
            if (!IsDocumentsPump())
                throw new NotSupportedException("Сервис доступен только для закачек документов");

            try
            {
                DeleteDocumentsFromSource(sourceID);
            }
            catch (Exception e)
            {
                WriteToTrace(e.ExpandException(), TraceMessageKind.Error);
                throw;
            }

            UsedFacts = new IFactTable[] {};
            UsedClassifiers = new IClassifier[] {};
            base.DirectDeleteData(pumpID, sourceID, constr);
        }

        /// <summary>
        /// Выполняет действия по инициализации программы закачки
        /// </summary>
        public override void Initialize(IScheme scheme, string programIdentifier, string userParams)
        {
            base.Initialize(scheme, programIdentifier, userParams);

            //log4net.Config.XmlConfigurator.Configure();
            // инициализируем NHibernate
            UnityStarter.Initialize();

            NHibernateInitializer.Instance().InitializeNHibernateOnce(
                () => NHibernateSession.InitializeNHibernateSession(
                    new ThreadSessionStorage(),
                    scheme.SchemeDWH.ConnectionString,
                    scheme.SchemeDWH.FactoryName,
                    scheme.SchemeDWH.ServerVersion));

            // инициализируем поля
            _clientLocationOkatoCodePrefix =
                ((int) Region).ToString(CultureInfo.InvariantCulture).PadLeft(8, '0').TrimEnd('0');

            _clientKladrCode = scheme.GlobalConstsManager.Consts["KLADR"].Value.ToString();
            WriteToTrace(string.Format("Текущий регион: {0}, КЛАДР {1}", this.Region, _clientKladrCode), TraceMessageKind.Warning);

            _pumpIdentifier = programIdentifier.Replace("BusGovRuPump.", string.Empty);

            _rootDir = IsDocumentsPump()
                           ? Path.Combine(PumpRegistryElement.DataSourcesUNCPath, _clientKladrCode)
                           : PumpRegistryElement.DataSourcesUNCPath;
        }

        protected override void InitDBObjects()
        {
            if (State == PumpProcessStates.PumpData)
            {
                IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = PumpRegistryElement.SupplierCode;
                ds.DataCode = PumpRegistryElement.DataCode;
                ds.DataName = constDefaultClsName;
                ds.ParametersType = ParamKindTypes.Variant;
                ds.Variant = string.Format(
                    "{0} {1} {2}", _pumpIdentifier, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());

                int? dataSourceId =
                    ds.FindInDatabase()
                    ?? ds.Do(
                        source =>
                            {
                                try
                                {
                                    Scheme.DataSourceManager.DataSources.Add(ds);
                                }
                                    //catch (NullReferenceException)
                                catch (Exception)
                                {
                                    // TODO: [monkeypath] давим, пока серверный код источников не умеет работать без OLAP
                                }
                            })
                           .FindInDatabase();
                if (dataSourceId.HasValue)
                {
                    SetDataSource(dataSourceId.Value);
                }

                BeforeProcessFiles();
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            SetProgress(0, 0, "Выполняется распаковка архивов исходных данных", string.Empty, true);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Распаковка архивов исходных данных");
            DirectoryInfo archiveDir = CommonRoutines.GetTempDir();
            CommonRoutines.ExtractArchiveFiles(
                dir.FullName,
                archiveDir.FullName,
                ArchivatorName.Zip,
                FilesExtractingOption.SeparateSubDirs);
            try
            {
                //BeforeProcessFiles();
                ProcessFilesTemplate(archiveDir, "*.xml", ProcessFile, true, SearchOption.AllDirectories);
                //AfterProcessFiles();
            }
            finally
            {
                CommonRoutines.DeleteDirectory(archiveDir);
            }
        }

        protected override void PumpFinalizing()
        {
            // Вставляем связь с операцией закачки
            // Но сначала проверим, нет ли такой записи
            IDatabase db = Scheme.SchemeDWH.DB;

            try
            {
                db.BeginTransaction();

                int count = Convert.ToInt32(
                    db.ExecQuery(
                        "select count (RefDataSources) from DataSources2PumpHistory " +
                        "where RefDataSources = ? and RefPumpHistory = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("RefDataSources", DataSource.ID),
                        db.CreateParameter("RefPumpHistory", PumpHistoryElement.ID)));
                if (count == 0)
                {
                    db.ExecQuery(
                        "insert into DataSources2PumpHistory (RefDataSources, RefPumpHistory) values (?, ?)",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("RefDataSources", DataSource.ID),
                        db.CreateParameter("RefPumpHistory", PumpHistoryElement.ID));
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                db.Dispose();
            }
        }

        [UnitOfWork]
        private void AfterProcessFiles()
        {
            switch (_pumpIdentifier)
            {
                case "nsiOgs":
                    AfterProcessNsiOgsPumpFiles();
                    break;
                case "nsiOkato":
                    AfterProcessNsiOkatoPumpFiles();
                    break;
                case "nsiPpo":
                    AfterProcessNsiPpoPumpFiles();
                    break;
                case "nsiSubjectService":
                    AfterProcessNsiSubjectServicePumpFiles();
                    break;
            }
        }

        [UnitOfWork]
        private void BeforeProcessFiles()
        {
            switch (_pumpIdentifier)
            {
                case "nsiOgs":
                    BeforeProcessNsiOgsPumpFiles();
                    break;
                case "nsiPpo":
                    BeforeProcessNsiPpoPumpFiles();
                    break;
                case "nsiSubjectService":
                    BeforeProcessNsiSubjectServicePumpFiles();
                    break;
                case "institutionInfo":
                    BeforeDocumentsCommonPumpFiles();
                    BeforeProcessInstitutionInfoPumpFiles();
                    break;
                case "budgetaryCircumstances":
                    BeforeDocumentsCommonPumpFiles();
                    BeforeProcessBudgetaryCircumstancesPumpFiles();
                    break;
                case "stateTask":
                    BeforeDocumentsCommonPumpFiles();
                    break;
                case "pfhd":
                    BeforeDocumentsCommonPumpFiles();
                    BeforeProcessPfhdPumpFiles();
                    break;
            }
        }

        private Dictionary<string, D_Org_PPO> BuildPpoCache()
        {
            WriteToTrace("Построение кэша ППО", TraceMessageKind.Information);
            return Resolver.Get<ILinqRepository<D_Org_PPO>>().FindAll()
                .ToDictionary(ppo => ppo.Code, ppo => ppo);
        }

        private List<D_OKATO_OKATO> BuildOkatoCache()
        {
            WriteToTrace("Построение кэша OKATO", TraceMessageKind.Information);
            return Resolver.Get<ILinqRepository<D_OKATO_OKATO>>().FindAll().ToList();
        }

        private Dictionary<string, D_Org_Structure> BuildOrgStructureByRegnumCache()
        {
            WriteToTrace("Построение кэша организаций в срезе regNum", TraceMessageKind.Information);
            return Resolver.Get<ILinqRepository<D_Org_Structure>>().FindAll()
                .Join(
                    Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll(),
                    structure => new {inn = structure.INN, kpp = structure.KPP},
                    ogs => new {ogs.inn, ogs.kpp},
                    (structure, ogs) => new {ogs.regNum, structure})
                .ToDictionary(arg => arg.regNum, arg => arg.structure);
        }

        #region FTP

        private const int FTP_CONNECT_TIMEOUT = 10000;

        /// <summary>
        /// формирование источников с fpt.bus.gov.ru
        /// ftp://gmuext:YctTa34AdOPyld2@ftp.bus.gov.ru:21
        /// </summary>
        protected override void DirectPreviewData()
        {
            const string ftpServer = "ftp.bus.gov.ru";
            const int ftpPort = 21;
            const string ftpUsername = "gmuext";
            const string ftpPassword = "YctTa34AdOPyld2";
            //WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Старт закачки данных из каталога " + ftpAddress);
            using (var ftp = new FtpClient())
            {
                var remoteFiles = new List<FtpItem>();
                ftp.PassiveMode = true;
                ftp.ProxyInfo = ProxyInfo();

                ftp.Connect(FTP_CONNECT_TIMEOUT, ftpServer, ftpPort);
                ftp.Login(FTP_CONNECT_TIMEOUT, ftpUsername, ftpPassword);

                WriteEventIntoPreviewDataProtocol(
                    PreviewDataEventKind.dpeInformation, "Получение списка файлов с удаленного каталога");

                if (!IsDocumentsPump())
                {
                    remoteFiles = GetRemoteFilesList(ftp, "all")
                        .Where(item => item.Name.StartsWith(_pumpIdentifier))
                        .ToList();
                }
                else
                {
                    switch (_pumpIdentifier)
                    {
                        case "institutionInfo":
                            remoteFiles = GetRemoteFilesList(ftp, Path.Combine("GeneralInfo", _clientKladrCode))
                                .ToList();
                            break;
                        case "budgetaryCircumstances":
                            remoteFiles =
                                GetRemoteFilesList(ftp, Path.Combine("BudgetaryCircumstances", _clientKladrCode))
                                    .ToList();
                            break;
                        case "stateTask":
                            remoteFiles = GetRemoteFilesList(ftp, Path.Combine("StateTask", _clientKladrCode))
                                .ToList();
                            break;
                        case "pfhd":
                            remoteFiles = GetRemoteFilesList(ftp, Path.Combine("ActionGrant", _clientKladrCode))
                                .Concat(
                                    GetRemoteFilesList(ftp, Path.Combine("FinancialActivityPlan", _clientKladrCode)))
                                .ToList();
                            break;
                    }
                }
                ftp.Disconnect(FTP_CONNECT_TIMEOUT);

                var rootDir = new DirectoryInfo(_rootDir);

                rootDir.Unless(info => info.Exists)
                    .Do(info => info.Create())
                    .Do(info => info.Refresh());

                if (!remoteFiles.Select(item => item.Name)
                         .SequenceEqual(rootDir.GetFiles().Select(info => info.Name)))
                {
                    MoveFilesToArchiveFolder(rootDir);

                    ftp.Connect(FTP_CONNECT_TIMEOUT, ftpServer, ftpPort);
                    ftp.Login(FTP_CONNECT_TIMEOUT, ftpUsername, ftpPassword);

                    WriteEventIntoPreviewDataProtocol(
                        PreviewDataEventKind.dpeInformation, "Старт загрузка файлов с удаленного каталога");

                    SetProgress(remoteFiles.Count(), -1, "Загрузка файлов с удаленного каталога...", string.Empty, true);
                    WriteToTrace("Загрузка файлов с удаленного каталога...", TraceMessageKind.Information);

                    for (int i = 0, count = remoteFiles.Count; i < count; i++)
                    {
                        FtpItem remoteFile = remoteFiles[i];
                        SetProgress(-1, i, string.Empty, remoteFile.RefName, true);
                        WriteToTrace(
                            string.Format("Загрузка файла {0}", remoteFile.RefName),
                            TraceMessageKind.Information);
                        ftp.GetFile(FTP_CONNECT_TIMEOUT, Path.Combine(_rootDir, remoteFile.Name), remoteFile.RefName);
                    }

                    WriteEventIntoPreviewDataProtocol(
                        PreviewDataEventKind.dpeInformation, "Окончание загрузки файлов с удаленного каталога");
                    ftp.Disconnect(FTP_CONNECT_TIMEOUT);
                }
            }

            base.DirectPreviewData();
        }

        /// <summary>
        /// FtpProxyInfo в формате http://user:password@server:port
        /// </summary>
        /// <returns>ConfigurationManager.AppSettings.Get("BusGovRuPump.FtpProxyInfo"), null если свойство не установлено в конфиурации</returns>
        private static FtpProxyInfo ProxyInfo()
        {
            return ConfigurationManager.AppSettings
                .With(collection => collection.Get("BusGovRuPump.FtpProxyInfo"))
                .With(s => Regex.Match(s, @"http://(\w+):(\w+)@([a-zA-Z_0-9.]+):(\d{4})"))
                .With(
                    match =>
                    new FtpProxyInfo
                        {
                            Type = FtpProxyType.HttpConnect,
                            User = match.Groups[1].Value,
                            Password = match.Groups[2].Value,
                            Server = match.Groups[3].Value,
                            Port = Convert.ToInt32(match.Groups[4].Value),
                        });
        }

        private IEnumerable<FtpItem> GetRemoteFilesList(FtpClient ftp, string path)
        {
            return
                ftp
                    .GetDirectoryList(
                        FTP_CONNECT_TIMEOUT,
                        path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .Select(
                        item =>
                        new FtpItem(
                            item.RawString,
                            item.Name,
                            Path.Combine(path, item.Name)
                                .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                            item.Date,
                            item.Size,
                            item.ItemType));
        }

        private void MoveFilesToArchiveFolder(DirectoryInfo directoryInfo)
        {
            try
            {
                FileInfo[] targetFiles = directoryInfo.GetFiles();

                if (!targetFiles.Any())
                    return;

                WriteEventIntoPreviewDataProtocol(
                    PreviewDataEventKind.dpeInformation, "Старт переноса файлов источника архив");
                // Получаем путь к архивному каталогу
                string archivePath = Path.Combine(
                    Scheme.ArchiveDirectory,
                    string.Format(
                        "{0}{1} {2} {3} {4} {5}",
                        DateTime.Now.ToLongDateString(),
                        DateTime.Now.ToLongTimeString().Replace(':', '-'),
                        PumpRegistryElement.SupplierCode,
                        PumpRegistryElement.DataCode.PadLeft(4, '0'),
                        PumpID,
                        SourceID));

                DirectoryInfo archive = Directory.CreateDirectory(archivePath);

                SetProgress(targetFiles.Count(), -1, "Перенос файлов источника архив", string.Empty, true);
                WriteToTrace("Перенос файлов источника архив", TraceMessageKind.Information);

                for (int i = 0, count = targetFiles.Count(); i < count; i++)
                {
                    FileInfo targetFile = targetFiles[i];
                    SetProgress(-1, i, string.Empty, targetFile.FullName, true);
                    WriteToTrace(
                        string.Format("{0} -> {1}", targetFile.FullName, archive.FullName),
                        TraceMessageKind.Information);
                    targetFile.MoveTo(Path.Combine(archive.FullName, targetFile.Name));
                }

                WriteEventIntoPreviewDataProtocol(
                    PreviewDataEventKind.dpeInformation, "Окончание переноса файлов источника архив");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при переносе файлов источника в архив", ex);
            }
        }

        #endregion
    }
}
