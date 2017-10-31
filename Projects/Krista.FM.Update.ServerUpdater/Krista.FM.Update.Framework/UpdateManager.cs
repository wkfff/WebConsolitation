using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

using Krista.Diagnostics;
using Krista.FM.Update.Framework.Forms;
using Krista.FM.Update.Framework.UpdateObjects;
using Krista.FM.Update.Framework.Utils;
using Microsoft.Practices.Unity;

namespace Krista.FM.Update.Framework
{
    [ComVisible(false)]
    public sealed class UpdateManager : MarshalByRefObject, IDisposable, IUpdateManager
    {
        public const string ServiceName = "FMUpdateService";

        public const string InstalledUpdatesFileName = "InstalledUpdates.xml";
        public const string InstalledUpdatesFileNameTemp = "InstalledUpdatesTemp.xml";
        public const string ReceivedUpdatesFileName = "ReceivedUpdates.xml";
        public const string ReceivedUpdatesFileNameTemp = "ReceivedUpdatesTemp.xml";

        private const string InstalledUpdatesFolder = "InstalledUpdates";

        public const string InstallerFeed = "Installer.Feed.xml";
        
        #region Singleton ���������������� ���������� ��� ������������� �����������, �� ��� ���������� �������������

        private static readonly UpdateManager instance = new UpdateManager();

        /// <summary>
        /// ����������� ����������� ��������� ��� ���������� ������������� ����������� ����� 
        /// (������������� ���������� ��������������� ����� ������ ����������). 
        /// ������������� ����������� �� ���������������� �� ����� ����� Main(). 
        /// </summary>
        static UpdateManager()
        { }

        private UpdateManager()
        {
            KristaApplication = GetApplication();

            if (ReadConfiguration())
            {
                State = UpdateProcessState.NotChecked;
                ApplicationPath = GetProcessModule().FileName;
                BackupFolder = Path.Combine(Path.GetDirectoryName(ApplicationPath), "Backup");
                UpdateProcessName = String.Format("{0}UpdateSync", KristaApplication);
                UpdateFeedReader = UnitySingleton.Instance.Resolve<IUpdateFeedReader>();
                UpdateSource = CreateUpdateSource();
                ExecuteOnAppRestart = new Dictionary<string, object>();
                Feeds = new Dictionary<string, IUpdateFeed>();
                TempFolder =
                    Path.Combine(Path.GetTempPath(),
                                 String.Format("Krista\\{0}\\Updates", KristaApplication));
                if (String.IsNullOrEmpty(DestBaseUrl))
                {
                    DestBaseUrl = Path.GetDirectoryName(ApplicationPath);
                }
            }
        }

        private IUpdateSource CreateUpdateSource()
        {
            if (SourceBaseUri.ToLower().StartsWith("ftp"))
            {
                return UnitySingleton.Instance.Resolve<IUpdateSource>(
                    new ParameterOverride("connectionType", ConnectionType.ftp)
                    , new ParameterOverride("proxy", Proxy ?? String.Empty)
                    , new ParameterOverride("proxyPort", ProxyPort ?? String.Empty)
                    , new ParameterOverride("user", User ?? String.Empty)
                    , new ParameterOverride("password", Password ?? String.Empty));
            }

            return UnitySingleton.Instance.Resolve<IUpdateSource>(
                    new ParameterOverride("connectionType", ConnectionType.http)
                    , new ParameterOverride("proxy", Proxy ?? String.Empty)
                    , new ParameterOverride("proxyPort", ProxyPort ?? String.Empty)
                    , new ParameterOverride("user", User ?? String.Empty)
                    , new ParameterOverride("password", Password ?? String.Empty));
        }

        public void InitializeNotifyIconForm()
        {
            if (!NeedCreateNewNotifyIconForm())
                return;

            if (form != null)
            {
                StartUpdates();
                return;
            }

            if (!isOfflineMode)
            {
                form = new NotifyIconForm(this);
                form.Visible = false;
                form.Activate();

                StartUpdates();
            }
        }

        /// <summary>
        /// ��� ���� ���������� �����������, ������� ���� ����� ����������
        /// </summary>
        /// <returns></returns>
        private bool NeedCreateNewNotifyIconForm()
        {
            string syncName = string.Format("{0}Sync", KristaApplication);

            bool createNew;
            syncSemaphore = new Semaphore(1, 1, syncName, out createNew);
            if (createNew)
            {
                return true;
            }

            return false;
        }

        public static UpdateManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region Properties and Fields

        /// <summary>
        /// ����� ��������������� ���������� ��� ������� ������������
        /// </summary>
        public bool AutoUpdateMode { get; set; }
        
        private Semaphore syncSemaphore;

        /// <summary>
        /// ������ � �������� ������� �������
        /// </summary>
        public string ServerModulesString { get; set; }

        /// <summary>
        /// ������� ������� � ��������������� ������������
        /// </summary>
        public string SourceBaseUri { get; set; }

        // ���������� ����� ������
        private bool isOfflineMode = false;

        private readonly Object thisLock = new Object();

        /// <summary>
        /// ������ �����
        /// </summary>
        public object Scheme { get; set; }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        public KristaApp KristaApplication { get; set; }
        
        private NotifyIconForm form;

        private bool disposed = false;

       /// <summary>
        /// true - ���� ���������� ���������� �� ���������������� �������, false - �� ���������� ������
        /// </summary>
        public bool IsServerMode
        {
            get
            {
                return
                    Process.GetCurrentProcess().MainModule.ModuleName ==
                        "Krista.FM.Update.ShedulerUpdateService.exe" || Process.GetCurrentProcess().MainModule.ModuleName == "Krista.FM.Update.DebugApp.exe";
            }
        }

        /*/// <summary>
        /// ���������������� ������� ����������
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal Logger Logger
        {
            get { return logger; }
        }*/

        internal string TempFolder { get; set; }
        public string UpdateProcessName { get; set; }
        internal string ApplicationPath;
        public Dictionary<string, object> ExecuteOnAppRestart { get; set; }
        public Dictionary<string, IUpdateFeed> Feeds { get; set; }

        /// <summary>
        /// ��� �����. ������������ ��� �������������� ���������� ������������ ������.
        /// </summary>
        public string OKTMO { get; set; }

        /// <summary>
        /// ������ ������������ ����������
        /// </summary>
        public Version AppVersion { get; set; }

        private string _backupFolder;
        internal string BackupFolder
        {
            set
            {
                if (this.State == UpdateProcessState.NotChecked || this.State == UpdateProcessState.Checked)
                    _backupFolder = Path.IsPathRooted(value) ? value : Path.Combine(this.TempFolder, value);
                else
                    throw new ArgumentException("BackupFolder can only be specified before update has started");
            }
            get
            {
                return _backupFolder;
            }
        }

        /// <summary>
        /// ������� ������� ���������� ��� ������� ����������
        /// </summary>
        internal string DestBaseUrl { get; set; }

        private UpdateProcessState _state;

        /// <summary>
        /// ������� ��������� �������� ����������
        /// </summary>
        public UpdateProcessState State
        {
            get { return _state; }
            set
            {
                if (_state != value && (value == UpdateProcessState.LastVersion 
                                     || value == UpdateProcessState.AppliedSuccessfully
                                     || value == UpdateProcessState.Error
                                     || value == UpdateProcessState.Warning
                                     || value == UpdateProcessState.Checked
                                     || value == UpdateProcessState.Prepared))
                {
                    Trace.TraceWarning("New final state: {0}", value);
                    AsyncReceiveStateToClients(value);
                }

                _state = value;
            }
        }

        public IUpdateSource UpdateSource { get; set; }

        /// <summary>
        /// ����� ����������
        /// </summary>
        internal IUpdateFeedReader UpdateFeedReader { get; set; }

        private Thread _updatesThread;
        private volatile bool _shouldStop;
        private string _currentLog;

        /// <summary>
        /// ��������� �������� ��� ��� ����������� ����� ����������
        /// </summary>
        private bool IsWorking { get { return _updatesThread != null && _updatesThread.IsAlive; } }

        /// <summary>
        /// ����� ����������
        /// </summary>
        /// 
        private string FeedPath { get; set; }

        // ������
        public string Proxy { get; set; }
        public string ProxyPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        // �������
        private static Hashtable clients = new Hashtable();

        #endregion

        #region Initialize Methods

        #region Diagnostics

        public void InitializeDiagnostics()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(ApplicationPath), "UpdateLog"))
                    && PermissionsCheck.HaveWritePermissionsForFolder(string.Format("{0}\\",
                                                                                    Path.GetDirectoryName(ApplicationPath))))
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(ApplicationPath), "UpdateLog"));
                }

                TraceSource source = KristaDiagnostics.GetTraceSource("Krista.FM.Update.Framework");
                SourceSwitch sourceSwitch = new SourceSwitch("SourceSwitch", "Verbose");
                source.Switch = sourceSwitch;
                source.Listeners.Clear();
                _currentLog = Path.Combine(Path.Combine(Path.GetDirectoryName(ApplicationPath), "UpdateLog"),
                                           string.Format("log_{0:yyyy-MM-dd_HH.mm.ss}.txt", DateTime.Now));
                source.Listeners.Add(
                    new TextWriterTraceListener(_currentLog, "textlog"
                        ));
                source.Listeners.Add(new ColoredConsoleTraceListener());
            }
            catch (Exception e)
            {
                using (var writer =
                   new StreamWriter(string.Format("{0}\\CrashLog_{1:yyyy-MM-dd_HH.mm.ss}.txt",
                                                  Path.GetDirectoryName(ApplicationPath), DateTime.Now)))
                {
                    writer.Write(e.Message);
                    writer.Write(e.StackTrace);
                }
            }
        }

        public void ClearTraceSource()
        {
            TraceSource source = KristaDiagnostics.GetTraceSource("Krista.FM.Update.Framework");
            Trace.TraceVerbose("�������� ��������� � ������� ������� ����");
            TraceListener listener = source.Listeners["textlog"];
            if (listener != null)
            {
                listener.Flush();
                source.Listeners.Remove(listener);
                listener.Close();
            }
        }

        /// <summary>
        /// ������� ������� ��������� ���
        /// </summary>
        private void RemoveCurrentLog()
        {
            string logname = _currentLog;

            ClearTraceSource();

            string directoryName = Path.Combine(Path.GetDirectoryName(ApplicationPath), "UpdateLog");

            // �������� �� ������� ���� �� �������� �����
            if (PermissionsCheck.HaveWritePermissionsForFolder(directoryName))
            {
                int retries = 100;
                while (File.Exists(logname) && retries > 0)
                {
                    try
                    {
                        File.Delete(logname);
                    }
                    catch (IOException)
                    {
                        // ���� ���������� ����������, ����������� � ������ ������� ������� ��-�� ����� ������ ��� ������� ���� ������ ������������,
                        // �� �������� ���������� ������ � ��������� ������
                        retries--;
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        #endregion

        private bool ReadConfiguration()
        {
            if (IsServerMode) AutoUpdateMode = true;

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename =
                    Path.Combine(
                        Path.GetDirectoryName(
                            GetProcessModule().FileName),
                        "Krista.FM.Update.ShedulerUpdateService.exe.config")
            };

            Configuration config;
            try
            {
                if (!File.Exists(fileMap.ExeConfigFilename))
                {
                    throw new ConfigurationErrorsException("���� ������������ �� ������");
                }

                config =
                    ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                                                                    ConfigurationUserLevel.None);
            }
            catch (ConfigurationErrorsException e)
            {
                Trace.TraceError(String.Format("������ ��� �������� ����� ������������: {0}",
                                                    e.Message));
                Trace.TraceInformation("���������� ����� �������� � ���������� ������ ��� ��������������� ����������.");
                isOfflineMode = true;
                return false;
            }

            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                switch (appSetting.Key)
                {
                    case "PublicFMPatchStore":
                        if (IsServerMode)
                        {
                            SourceBaseUri = appSetting.Value;
                            FeedPath = Path.Combine(Path.Combine(appSetting.Value, "Global"),
                                                    String.Format("Global.Feed.xml"));
                        }
                        continue;
                    case "BaseUri":
                        Uri uri;

                        if (Uri.TryCreate(appSetting.Value, UriKind.Absolute, out uri))
                        {
                            if (!IsServerMode)
                            {
                                SourceBaseUri = uri.OriginalString;
                                if (uri.OriginalString.Contains(".xml"))
                                    FeedPath = uri.OriginalString;
                                else
                                {
                                    string feedName = GetFeedName();
                                    if (String.IsNullOrEmpty(feedName))
                                        return false;
                                    FeedPath = Path.Combine(uri.OriginalString, feedName);
                                }
                            }
                        }
                        else
                            Trace.TraceError(
                                "��������� ������������ ��������� BaseUri � Krista.FM.SchedulerServerUpdater.exe.config");
                        break;
                    case "LocalFMPatchStore":
                        Uri uriLocalFMPatchStore;

                        if (Uri.TryCreate(appSetting.Value, UriKind.Absolute, out uriLocalFMPatchStore))
                        {
                            if (IsServerMode)
                            {
                                DestBaseUrl = uriLocalFMPatchStore.OriginalString;
                            }
                        }
                        else
                            Trace.TraceError(
                                "��������� ������������ ��������� LocalFMPatchStore � Krista.FM.SchedulerServerUpdater.exe.config");
                        break;
                    case "Proxy":
                        if (IsServerMode)
                        {
                            Proxy = appSetting.Value;
                        }
                        break;
                    case "ProxyPort":
                        if (IsServerMode)
                        {
                            ProxyPort = appSetting.Value;
                        }
                        break;
                    case "User":
                        if (IsServerMode)
                        {
                            User = appSetting.Value;
                        }
                        break;
                     case "Password":
                        if (IsServerMode)
                        {
                            Password = SecureConfigStringProvider.DecryptString(appSetting.Value);
                        }
                        break;
                    case "AutoUpdateClient":
                        switch (KristaApplication)
                        {
                            case KristaApp.SchemeDesigner:
                            case KristaApp.OlapAdmin:
                            case KristaApp.Workplace:
                                AutoUpdateMode = appSetting.Value == "1";
                                break;
                        }
                        break;
                    case "AutoUpdateMdx":
                        switch (KristaApplication)
                        {
                            case KristaApp.MDXExpert:
                                AutoUpdateMode = appSetting.Value == "1";
                                break;
                        }
                        break;
                    case "AutoUpdateOffice":
                        switch (KristaApplication)
                        {
                            case KristaApp.OfficeAddIn:
                                AutoUpdateMode = appSetting.Value == "1";
                                break;
                        }
                        break;
                    case "OKTMO":
                        {
                            OKTMO = appSetting.Value;
                            break;
                        }
                    default:
                        continue;
                }
            }

            return true;
        }

        #endregion

        #region NotifyIconEvents

        internal string GetMessageBoxText(bool rollback)
        {
            string message =
                Feeds.Values.Where(updateFeed => !updateFeed.IsBase).Where(i => i.Patches.Count() > 0).Aggregate(
                (rollback) ? "����� ����������." : "�������� ����� ����������.\n\n",
                    (current1, updateFeed) =>
                    updateFeed.UpdatesToApply.Aggregate(current1,
                                                        (current, updatePatch) =>
                                                        current +
                                                        string.Format("{0} - {1}\n", updatePatch.Name,
                                                                      updatePatch.Description)));
            switch (KristaApplication)
            {
                case KristaApp.OfficeAddIn:
                    message += "\n��� ����������� ��������� ���������� ������� ��" + (!AutoUpdateMode ? " � �������� ��� ���������� Workplace � ��� ���������� MS Office." : String.Empty);
                    break;
                case KristaApp.OlapAdmin:
                case KristaApp.SchemeDesigner:
                case KristaApp.MDXExpert:
                case KristaApp.Workplace:
                    message +=
                        String.Format(
                        "\n��� ����������� ��������� ���������� ������� ��" + (!AutoUpdateMode ? " � �������� ���������� {0}." : String.Empty),
                            KristaApplication);
                    break;
                case KristaApp.Updater:
                    message += "\n��� ����������� ��������� ���������� ������������� ������ ��������������� ����������.";
                    break;
                default:
                    throw new Exception(String.Format("�������������� ���������� {0}.", KristaApplication));
            }

            return message;
        }

        /// <summary>
        /// ���������, ����� �� ������������ ���������� ��� ����������
        /// </summary>
        /// <returns></returns>
        private bool CheckNeedRestartApp()
        {
            if (IsServerMode)
                return false;

            return ExecuteOnAppRestart.Keys.Where(str => !str.StartsWith("ENV:")).Count() > 0;
        }

        #endregion

        #region Step 00 Enter point

        public void StartUpdates()
        {
            Thread thread = new Thread(delegate()
            {
                try
                {
                    InitializeDiagnostics();
                    
                    if (!String.IsNullOrEmpty(FeedPath))
                        StartUpdates(FeedPath);
                }
                catch (Exception e)
                {
                    Trace.TraceError(String.Format("������ ��� ������ ����������: {0}", e));
                    State = UpdateProcessState.Error;
                }
            }) { IsBackground = true };
            thread.Start();
        }

        #endregion

        #region Step 0 - Get updates feed

        private void StartUpdates(string feed)
        {
            if (State != UpdateProcessState.NotChecked)
            {
                CleanUp();
            }

            Trace.TraceInformation(String.Format("����� �������� ���������� {0}", DateTime.Now));
            Trace.TraceInformation("********************************************");
            Trace.TraceInformation("������ ������� ���������� ������� ����������");
            Feeds.Clear();

            Trace.Indent();
            bool initializeUpdatesFeed;
            try
            {
                initializeUpdatesFeed = InitializeUpdatesFeeds(feed);
            }
            finally
            {
                Trace.Unindent();    
            }
            
            if (initializeUpdatesFeed)
            {
                CheckForUpdateAsync(UpdateSource, OnCheckForUpdateComplited);
            }
        }

        private bool InitializeUpdatesFeeds(string feed)
        {
            bool initializeUpdatesFeed = InitializeUpdatesFeed(feed, String.Empty, String.Empty, false);

            // ��� ������ ��������������� ���������� �������� ����������� ����� ����������
            if (IsServerMode)
            {
                initializeUpdatesFeed = InitializeUpdatesFeed(Path.Combine(SourceBaseUri, GetFeedName())
                    , "Installer.Feed.xml"
                    , String.Format("Installer\\{0}", GetApplicationVersion())
                    , true);
            }

            return initializeUpdatesFeed;
        }

        private bool InitializeUpdatesFeed(string feed, string feedName, string feedUrl, bool readinstaller)
        {
            try
            {
                if (!readinstaller && feedName.ToLower().Contains("installer"))
                {
                    return false;
                }

                Uri uri;
                string feedPath = Uri.TryCreate(feed, UriKind.Absolute, out uri)
                    ? uri.AbsoluteUri
                    : new Uri(new Uri(Path.GetDirectoryName(ApplicationPath), UriKind.Absolute), feed).ToString();

                var updatesFeed = GetFeedDescriptionThrowProtocol(feedPath);
                if (updatesFeed != null)
                {
                    XmlDocument feedsDocument = new XmlDocument();
                    feedsDocument.LoadXml(updatesFeed);
                    XmlNode root = feedsDocument.SelectSingleNode(@"Feeds");
                    if (root == null)
                    {
                        Trace.TraceInformation(String.Format("������ ������ {0}", feed));
                        UpdateFeed updateFeed = new UpdateFeed(feedName, feedUrl);
                        IList<IUpdatePatch> patchs = null;
                        try
                        {
                            patchs = UpdateFeedReader.Read(updatesFeed, updateFeed);
                        }
                        catch (FileDownloaderException e)
                        {
                            Trace.TraceError("������ ������ ������ ����������: {0}", e.Message);
                            State = UpdateProcessState.Error;
                        }

                        updateFeed.Patches = patchs;
                        if (!Feeds.ContainsKey(updateFeed.ObjectKey))
                        {
                            Feeds.Add(updateFeed.ObjectKey, updateFeed);
                        }

                        return true;
                    }

                    // TODO ���������� ��� ������ �.�. ������� ����������� ������ ���������� ����������, ����� �� ��������� ���
                    XmlNodeList nodeListVersions = root.SelectNodes("./Version");
                    if (nodeListVersions != null)
                    {
                        foreach (XmlNode nodeListVersion in nodeListVersions)
                        {
                            XmlNodeList nodeListFeeds = nodeListVersion.SelectNodes("./Feed");
                            foreach (XmlNode nodeListFeed in nodeListFeeds)
                            {
                                if (nodeListFeed.Attributes != null)
                                {
                                    string value = nodeListFeed.Attributes["name"].Value;
                                    string url = nodeListFeed.Attributes["url"].Value;
                                    feedPath = Path.Combine(SourceBaseUri, Path.Combine(url, value));

                                    /*if (value.StartsWith("Base"))
                                    {
                                        var baseFeed = new UpdateFeed(value, url) {IsBase = true};
                                        Feeds.Add(baseFeed.Name, baseFeed);
                                    }*/

                                    InitializeUpdatesFeed(feedPath, value, url, false);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                             String.Format("��� ������ ������ ���������� ������� ��������������� ���������� �������� ����������: {0}", e.Message));
                State = UpdateProcessState.Error;
                return false;
            }
        }

        private string GetFeedDescriptionThrowProtocol(string feedPath)
        {
            string updatesFeed;
            try
            {
                Trace.TraceInformation(String.Format("��������� ������ ����������: {0}", feedPath));
                updatesFeed = UpdateSource.GetUpdatesFeed(feedPath);
            }
            catch (FileDownloaderException e)
            {
                Trace.TraceError(String.Format("��� ���������� ������ ���������� {0} �������� ���������� {1}",
                                               feedPath, e.Message));
                return null;
            }

            return updatesFeed;
        }

        #endregion

        #region Step 1 - Check for updates

        private bool CheckForUpdates(IUpdateSource source, Action<int> callback)
        {
            Trace.TraceInformation(String.Format("����� ����������"));
            Trace.Indent();
            try
            {
                if (UpdateFeedReader == null)
                {
                    Trace.TraceError("�� ����� UpdateFeedReader");
                    return false;
                }

                if (source == null)
                {
                    Trace.TraceError("�� ������ �������� ����������!");
                    return false;
                }

                Trace.TraceVerbose(String.Format("Main module: {0}", GetProcessModule().ModuleName));
                Trace.TraceVerbose(String.Format("IsServerUpdater: {0}", IsServerMode));
                Trace.TraceVerbose(String.Format("BaseIRl: {0}", DestBaseUrl));

                lock (thisLock)
                {
                    foreach (var updateFeed in Feeds.Values.Where(updateFeed => !updateFeed.IsBase).Where(i => i.Patches.Count() > 0))
                    {
                        Trace.TraceInformation(String.Format("����� ��������� ������ {0}", updateFeed.Name));
                        Trace.Indent();
                        try
                        {
                            foreach (IUpdatePatch patch in updateFeed.Patches)
                            {
                                if (patch.Use == Use.Prohibited)
                                {
                                    Trace.TraceVerbose(String.Format("���� {0} - ��������", patch.Name));
                                    continue;
                                }
                                patch.CheckForUpdates(updateFeed);
                            }
                            #region logging

                            if (updateFeed.UpdatesToApply.Count > 0)
                            {
                                Trace.TraceInformation(string.Format("������� {0} ����������", updateFeed.UpdatesToApply.Count));
                                Trace.Indent();
                                try
                                {
                                    foreach (IUpdatePatch updatePatch in updateFeed.UpdatesToApply)
                                    {
                                        Trace.TraceVerbose(string.Format("Patch version - {0}, description - {1}, use - {2}",
                                                                         updatePatch.Name, updatePatch.Description, updatePatch.Use));

                                        Trace.Indent();
                                        try
                                        {
                                            foreach (IUpdateTask updateTask in updatePatch.Tasks)
                                            {
                                                Trace.TraceVerbose(string.Format("Task description - {0}",
                                                                                 updateTask.Description));
                                            }
                                        }
                                        finally
                                        {
                                            Trace.Unindent();    
                                        }
                                    }
                                }
                                finally
                                {
                                    Trace.Unindent();    
                                }
                            }
                            else
                            {
                                //State = UpdateProcessState.LastVersion;
                                Trace.TraceInformation(String.Format("� ������ {0} ��� ��������� ����������", updateFeed.Name));
                            }

                            #endregion
                        }
                        finally
                        {
                            Trace.Unindent();
                        }

                        Trace.TraceInformation(String.Format("���������� ��������� ������ {0}", updateFeed.Name));
                    }
                }

                if (_shouldStop) return false;
            }
            finally
            {
                Trace.Unindent();    
            }

            if (callback != null) callback.BeginInvoke(UpdatesToApplyCount(), null, null);
            if (UpdatesToApplyCount() > 0)
            {
                State = UpdateProcessState.Checked;
                return true;
            }

            // ���������� � ������ ������ ����������������� ������� ����������
            SaveUpdatesToXml();

            State = UpdateProcessState.LastVersion;
            Trace.TraceInformation("�� ����������� ��������� ������ ����������.");

            // ������� ��������� ���...�� �� �������������
            var thread = new Thread(RemoveCurrentLog) {IsBackground = true};
            thread.Start();

            return false;
        }

        /// <summary>
        /// ����� ���������� ��������� ���������� �� ���� �������
        /// </summary>
        /// <returns></returns>
        private int UpdatesToApplyCount()
        {
            return Feeds.Values.Sum(updateFeed => updateFeed.UpdatesToApply.Count);
        }

        /// <summary>
        /// ����������� �������� ����������
        /// </summary>
        /// <param name="source"></param>
        /// <param name="callback"></param>
        private void CheckForUpdateAsync(IUpdateSource source, Action<int> callback)
        {
            if (State != UpdateProcessState.NotChecked)
            {
                Trace.TraceError("������� ���������� ��� �������; ������� ���������: " + State);
                return;
            }

            if (!IsWorking)
            {
                _updatesThread = new Thread(delegate()
                    {
                        try
                        {
                            CheckForUpdates(source, callback);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(String.Format("������ ��� ����� ����������: {0}", e));
                            State = UpdateProcessState.Error;
                            callback.BeginInvoke(0, null, null); /* TODO: Handle error */
                        }
                    }) {IsBackground = true};
                _updatesThread.Start();
            }
        }

        private void OnCheckForUpdateComplited(int count)
        {
            if (count != 0)
            {
                PrepareUpdatesAsync(OnPrepareUpdatesComplited);
            }
        }

        #endregion

        #region Step 2 - Prepare to execute update tasks

        private bool PrepareUpdates(Action<bool> callback)
        {
            Trace.TraceInformation("���������� ���������� � ���������");
            Trace.Indent();
            try
            {
                if (UpdatesToApplyCount() == 0)
                {
                    if (callback != null) callback.BeginInvoke(false, null, null);
                    return false;
                }

                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }

                lock (thisLock)
                {
                    foreach (var updateFeed in Feeds.Values.Where(updateFeed => !updateFeed.IsBase).Where(i => i.Patches.Count() > 0))
                    {
                        Trace.TraceInformation(String.Format("����� ��������� ������ {0}", updateFeed.Name));
                        if (_shouldStop)
                        {
                            return false;
                        }

                        foreach (IUpdatePatch patch in updateFeed.UpdatesToApply)
                        {
                            patch.Prepare(UpdateSource);
                        }

                        Trace.TraceInformation(String.Format("���������� ��������� ������ {0}", updateFeed.Name));
                    }
                }

                State = UpdateProcessState.Prepared;

                if (_shouldStop)
                {
                    return false;
                }
                
                if (callback != null)
                {
                    callback.BeginInvoke(true, null, null);
                }

                return true;
            }
            catch (PreparetaskException e)
            {
                if (callback != null) callback.BeginInvoke(false, null, null);
                throw new NAppUpdateException(e.Message);
            }
            finally
            {
                Trace.Unindent();
            }
        }

        private void PrepareUpdatesAsync(Action<bool> callback)
        {
            Trace.TraceVerbose("����������� ���������� ����������");

            for (int i = 0; i < 5; i++)
            {
                if (!IsWorking)
                    break;

                if (IsWorking)
                {
                    Trace.TraceVerbose("����������� ����� ���������� ��� ���. ������??");
                    Thread.Sleep(1000);
                }
            }

            if (!IsWorking)
            {
                _updatesThread = new Thread(delegate()
                {
                    try
                    {
                        PrepareUpdates(callback);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(String.Format("������ ��� ���������� ����������: {0}", e));
                        State = UpdateProcessState.Error;
                        callback.BeginInvoke(false, null, null); /* TODO: Handle error */
                    }
                }) {IsBackground = true};

                _updatesThread.Start();
            }
            else
            {
                Trace.TraceError("�� ������� ��������� ������� ���������� ����������. ��.���� ����������");
            }
        }

        private void OnPrepareUpdatesComplited(bool obj)
        {
            if (!obj)
                return;

            if (AutoUpdateMode)
            {
                ApplyUpdates();
            }
        }

        #endregion

        #region Step 3 - Apply updates

        public bool ApplyUpdates()
        {
            if (CheckNeedRestartApp())
            {
                IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;

                string message = GetMessageBoxText(false);
                if (MessageBox.Show(new WindowWrapper(hwnd), message, "��������!", (AutoUpdateMode ? MessageBoxButtons.OK : MessageBoxButtons.OKCancel), MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                    == DialogResult.OK)
                {
                    return ApplyUpdates(OnAplayUpdatesComplited);
                }
            }
            else
            {
                Trace.TraceVerbose("��� ����� ��� \"���������\" ����������!");
                return ApplyUpdates(OnAplayUpdatesComplited);
            }

            return false;
        }

        /// <summary>
        /// ��������� updater - ���������-���������� � ���������� ������ ���������� ����� ����������
        /// ����� updater �������� ��������� ���������� ���������� ���, ��� ������ ���������� ����� ���������
        /// </summary>
        /// <returns>������ � ������ ������ (���� ���������� �� ����������)</returns>
        private bool ApplyUpdates(Action<ExecuteState> callback)
        {
            Trace.TraceVerbose("����������� ����������");

            for (int i = 0; i < 5; i++)
            {
                if (!IsWorking)
                    break;

                if (IsWorking)
                {
                    Trace.TraceVerbose("����������� ����� ���������� ��� ���. ������??");
                    Thread.Sleep(1000);
                }
            }

            if (!IsWorking)
            {
                _updatesThread = new Thread(delegate()
                {
                    try
                    {
                        ApplyUpdates(true, callback);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(String.Format("������ ��� ���������� ����������: {0}", e));
                        State = UpdateProcessState.Error;
                        callback.BeginInvoke(ExecuteState.ExecuteWithError, null, null); /* TODO: Handle error */
                    }
                }) { IsBackground = true };
                _updatesThread.Start();
            }
            else
            {
                Trace.TraceError("�� ������� ��������� ������� ����������. ��.���� ����������");
                return false;
            }

            return true;
        }

        /// <summary>
        /// ��������� updater ���������� ��������� � ���������� ������ ���������� 
        /// </summary>
        /// <param name="RelaunchApplication">true - ��� ����������� ����������; false �����</param>
        /// <returns>������ � ������ ������ (���� ���������� �� ����������)</returns>
        private void ApplyUpdates(bool RelaunchApplication, Action<ExecuteState> callback)
        {
            Trace.TraceInformation("��������� ����������");
            Trace.Indent();
            try
            {
                ExecuteState success = ExecuteState.ExecuteSuccess;

                ExecuteOnAppRestart.Clear();

                lock (thisLock)
                {
                    // ��������������, ��� ������� ��������� ����� �������� ��� ����, ����� �������� � ��� ��������� �����
                    if (!PermissionsCheck.HaveWritePermissionsForFolder(BackupFolder))
                    {
                        _backupFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            string.Format("{0}\\UpdateBackups", UpdateProcessName));
                    }

                    if (!Directory.Exists(BackupFolder))
                        Directory.CreateDirectory(BackupFolder);

                    Trace.TraceVerbose(String.Format("����� Backup: {0}", _backupFolder));

                    State = UpdateProcessState.RollbackRequired;

                    XDocument installedUpdatesDocument = UpdateFeed.GetUpdatesDocument(Path.Combine(GetInstalledUpdateFolder(), InstalledUpdatesFileName));
                    XDocument receivedUpdatesDocument = UpdateFeed.GetUpdatesDocument(Path.Combine(GetInstalledUpdateFolder(), ReceivedUpdatesFileName));

                    foreach (var updateFeed in Feeds.Values.Where(updateFeed => !updateFeed.IsBase).Where(i => i.Patches.Count() > 0))
                    {
                        Trace.TraceInformation(String.Format("����� ��������� ������ {0}", updateFeed.Name));

                        foreach (IUpdatePatch updatePatch in updateFeed.UpdatesToApply)
                        {
                            switch (updatePatch.Execute())
                            {
                                case ExecuteState.ExecuteSuccess:
                                    {
                                        Trace.TraceInformation(String.Format("���� {0} ����������� � ���������.",
                                                                             updatePatch.Name));

                                        // ���������� �� ������� ��� ����������� ��������� �� �������������� ������������
                                        if (IsServerMode && !updateFeed.Name.Equals(InstallerFeed))
                                        {
                                            SaveToInstalledDocument(receivedUpdatesDocument, updatePatch);
                                        }
                                        else
                                        {
                                            SaveToInstalledDocument(installedUpdatesDocument, updatePatch);
                                        }

                                        continue;
                                    }
                                case ExecuteState.ExecuteWithError:
                                    {

                                        Trace.TraceError(String.Format("� �������� ���������� ����� {0} ���� ������!",
                                                                       updatePatch.Name));
                                        success = ExecuteState.ExecuteWithError;
                                        break;
                                    }
                                case ExecuteState.ExecuteWithWarning:
                                    {

                                        Trace.TraceError(String.Format("� �������� ���������� ����� {0} ���� ��������������!",
                                                                       updatePatch.Name));
                                        success = ExecuteState.ExecuteWithWarning;
                                        break;
                                    }
                            }
                        }

                        Trace.TraceInformation(String.Format("���������� ��������� ������ {0}", updateFeed.Name));
                    }

                    // ��������� �� ��������� ��������, ����� ��������� ���������� ������ ��������� � ��������
                    string appDir = Path.Combine(Path.GetDirectoryName(ApplicationPath), String.Format("{0}\\{1}", InstalledUpdatesFolder, GetApplicationVersion()));
                    DirectoryInfo directoryInfo = new DirectoryInfo(appDir);
                    directoryInfo.CreateDirectory();

                    installedUpdatesDocument.Save(Path.Combine(GetInstalledUpdateFolder(), InstalledUpdatesFileNameTemp));
                    if (IsServerMode)
                        receivedUpdatesDocument.Save(Path.Combine(GetInstalledUpdateFolder(), ReceivedUpdatesFileNameTemp));

                    // ���������� � ������ ������ ����������������� ������� ����������
                    SaveUpdatesToXml();

                    // ���� ���������� ������� �����������);
                    if (ExecuteOnAppRestart.Count > 0)
                    {
                        ExecuteOnAppRestart["ENV:AppPath"] = Process.GetCurrentProcess().MainModule.FileName;
                        ExecuteOnAppRestart["ENV:OfficeAddInPath"] = Path.GetDirectoryName(GetProcessModule().FileName);
                        ExecuteOnAppRestart["ENV:TempFolder"] = TempFolder;
                        ExecuteOnAppRestart["ENV:BackupFolder"] = BackupFolder;
                        ExecuteOnAppRestart["ENV:RelaunchApplication"] = RelaunchApplication;
                        ExecuteOnAppRestart["ENV:AppVersion"] = GetApplicationVersion();
                        if (IsServerMode)
                        {
                            ExecuteOnAppRestart["ENV:ServiceName"] = ServiceName;
                        }
                        ExecuteOnAppRestart["ENV:AutoUpdateMode"] = AutoUpdateMode;

                        ExecuteOnAppRestart["ENV:ProcessName"] = Process.GetCurrentProcess().ProcessName;

                        UpdateStarter updStarter = new UpdateStarter(Path.Combine(TempFolder, "updaterfm.exe"), ExecuteOnAppRestart, UpdateProcessName);
                        updStarter.Start();
                        if (!IsServerMode)
                        {
                            State = UpdateProcessState.WaitRestart;
                        }

                        return;
                    }

                    // ���� ���������� �� ������� ����������� ����������
                    ReplaceInstalledUpdatesTemp(Path.GetDirectoryName(ApplicationPath));

                    State = UpdateProcessState.AppliedSuccessfully;
                    foreach (var updateFeed in Feeds.Values)
                    {
                        updateFeed.UpdatesToApply.Clear();
                    }
                }

                if (callback != null) callback(success);
            }
            finally
            {
                Trace.Unindent();
            }
        }

        private void ReplaceInstalledUpdatesTemp(string appDir)
        {
            try
            {
                appDir = Path.Combine(appDir, String.Format("{0}\\{1}", InstalledUpdatesFolder, GetApplicationVersion()));
                DirectoryInfo directoryInfo = new DirectoryInfo(appDir);
                directoryInfo.CreateDirectory();

                if (File.Exists(Path.Combine(appDir, InstalledUpdatesFileNameTemp)))
                {
                    File.Copy(Path.Combine(appDir, InstalledUpdatesFileNameTemp),
                              Path.Combine(appDir, InstalledUpdatesFileName), true);
                    File.Delete(Path.Combine(appDir, InstalledUpdatesFileNameTemp));
                }

                if (File.Exists(Path.Combine(appDir, ReceivedUpdatesFileNameTemp)))
                {
                    File.Copy(Path.Combine(appDir, ReceivedUpdatesFileNameTemp),
                              Path.Combine(appDir, ReceivedUpdatesFileName), true);
                    File.Delete(Path.Combine(appDir, ReceivedUpdatesFileNameTemp));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message);
            }
        }

        public string GetInstalledUpdateFolder()
        {
            return Path.Combine(Path.GetDirectoryName(ApplicationPath),
                                String.Format("{0}\\{1}", InstalledUpdatesFolder, GetApplicationVersion()));
        }

        private void OnAplayUpdatesComplited(ExecuteState obj)
        {
            if (obj == ExecuteState.ExecuteSuccess)
            {
                State = UpdateProcessState.AppliedSuccessfully;
                Trace.TraceInformation("���������� ��������� �������!");
            }
            else if (obj == ExecuteState.ExecuteWithError)
            {
                State = UpdateProcessState.Error;
                Trace.TraceInformation("���������� ��������� � ��������!");
            }

            else if (obj == ExecuteState.ExecuteWithWarning)
            {
                State = UpdateProcessState.Warning;
                Trace.TraceInformation("���������� ��������� � ����������������!");
            }
        }

        #region Save

        private static void SaveToInstalledDocument(XDocument installedUpdatesDocument, IUpdatePatch updatePatch)
        {
            UpdateFeed.SavePatchToFeed(installedUpdatesDocument, updatePatch);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveUpdatesToXml()
        {
            Trace.TraceInformation("���������� ������� ����������");

            if (!IsServerMode)
            {
                return;
            }

            foreach (var updateFeed in Feeds.Values)
            {
                updateFeed.Save();
            }
        }

        #endregion

        #endregion

        #region Step 4 - Rollback updates

        // ���� ���������� ������ 1 ����, ������� ���������� ������ �������� � ��������������� �����

        #endregion

        #region CleanUp

        private void Abort()
        {
            _shouldStop = true;
        }

        /// <summary>
        /// Delete the temp folder as a whole and fail silently
        /// </summary>
        private void CleanUp()
        {
            Abort();

            if (_updatesThread != null && _updatesThread.IsAlive)
                _updatesThread.Join();

            lock (thisLock)
            {
                foreach (var updateFeed in Feeds.Values)
                {
                    updateFeed.UpdatesToApply.Clear();
                }

                State = UpdateProcessState.NotChecked;

                try
                {
                    Directory.Delete(TempFolder, true);
                }
                catch { }

                _shouldStop = false;
            }
        }

        #endregion

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        form.NotifyIconControl.NotifyIcon.Visible = false;
                        syncSemaphore.Release();
                    }
                    catch
                    {
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Helper Methods

        public static ProcessModule GetProcessModule()
        {
            foreach (ProcessModule module in
                Process.GetCurrentProcess().Modules.Cast<ProcessModule>().Where(
                    module => module.ModuleName == "Krista.FM.Update.Framework.dll"))
            {
                return module;
            }

            return Process.GetCurrentProcess().MainModule;
        }

        /// <summary>
        /// ���������� � ������ ������������ ����������
        /// </summary>
        /// <returns></returns>
        public string GetApplicationVersion()
        {
            FileVersionInfo fileVersionInfo = null;
            switch (Process.GetCurrentProcess().MainModule.ModuleName.ToLower())
            {
                case "krista.fm.client.schemedesigner.exe":
                case "krista.fm.client.workplace.exe":
                case "krista.fm.client.olapadmin.exe":
                case "mdxexpert.exe":
                case "krista.fm.update.shedulerupdateservice.exe":
                case "krista.fm.update.debugapp.exe":
                    fileVersionInfo = Process.GetCurrentProcess().MainModule.FileVersionInfo;
                    break;
                case "excel.exe":
                case "winword.exe":
                    foreach (ProcessModule module in
               Process.GetCurrentProcess().Modules.Cast<ProcessModule>().Where(
                   module => module.ModuleName.ToLower() == "fmexceladdin.dll"))
                    {
                        fileVersionInfo = module.FileVersionInfo;
                    }
                    break;
                default:
                    {
                        Trace.TraceError(String.Format("����������� ���������� ���������� {0}",
                                                       Process.GetCurrentProcess().MainModule.ModuleName));
                        throw new Exception(String.Format("����������� ���������� ���������� {0}",
                                                          Process.GetCurrentProcess().MainModule.ModuleName));
                    }
            }

            if (fileVersionInfo == null)
            {
                throw new Exception("�� ������� ������� ������ ����������� ����������");
            }

            string version = (KristaApplication == KristaApp.OfficeAddIn)
                                 ? String.Format("{0}.{1}.{2}", fileVersionInfo.FileMajorPart,
                                                 fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart)
                                 : String.Format("{0}.{1}", fileVersionInfo.FileMajorPart,
                                                 fileVersionInfo.FileMinorPart);

            return version;
        }

        private KristaApp GetApplication()
        {
            if (IsServerMode)
            {
                return KristaApp.Updater;
            }

            switch (Process.GetCurrentProcess().MainModule.ModuleName.ToLower())
            {
                case "krista.fm.client.schemedesigner.exe":
                    return KristaApp.SchemeDesigner;
                case "krista.fm.client.workplace.exe":
                    return KristaApp.Workplace;
                case "krista.fm.client.olapadmin.exe":
                    return KristaApp.OlapAdmin;
                case "mdxexpert.exe":
                    return KristaApp.MDXExpert;
                case "excel.exe":
                case "winword.exe":
                    return KristaApp.OfficeAddIn;
                default:
                    {
                        Trace.TraceError(String.Format("����������� ���������� ���������� {0}", Process.GetCurrentProcess().MainModule.ModuleName));
                        throw new Exception(String.Format("����������� ���������� ���������� {0}", Process.GetCurrentProcess().MainModule.ModuleName));
                    }
            }
        }

        /// <summary>
        /// ��������� ������ ���������� ��� ���������� ����������.
        /// ������� � ������ 3.1, ������ ������� �� ������ ����������.
        /// </summary>
        /// <returns> ������������� ��� ������ ����������</returns>
        private string GetFeedName()
        {
            string version = GetApplicationVersion();

            switch (KristaApplication)
            {
                case KristaApp.SchemeDesigner:
                    return Path.Combine(String.Format("Client\\SchemeDesigner\\{0}", version), "Client.SchemeDesigner.Feed.xml");
                case KristaApp.Workplace:
                    return Path.Combine(String.Format("Client\\Workplace\\{0}", version), "Client.Workplace.Feed.xml");
                case KristaApp.OlapAdmin:
                    return Path.Combine(String.Format("Client\\OLAPAdmin\\{0}", version), "Client.OLAPAdmin.Feed.xml");
                case KristaApp.MDXExpert:
                    return Path.Combine(String.Format("MDXExpert\\{0}", version), "MDXExpert.Feed.xml");
                case KristaApp.OfficeAddIn:
                    return Path.Combine(String.Format("OfficeAddIn\\{0}", version), "OfficeAddIn.Feed.xml");
                case KristaApp.Updater:
                    return Path.Combine(String.Format("Installer\\{0}", version), "Installer.Feed.xml"); 
                default:
                    Trace.TraceError(String.Format("����������� ���������� ���������� {0}", Process.GetCurrentProcess().MainModule.ModuleName));
                    throw new Exception(String.Format("����������� ���������� ���������� {0}", Process.GetCurrentProcess().MainModule.ModuleName));
            }
        }

        #endregion

        #region IUpdateManager

        public void Activate()
        {
            //Trace.TraceInformation("������ ��������������� ���������� �����������");
        }

        public IList<IUpdatePatch> GetPatchList()
        {
            return
                BaseGetPatchList(Path.Combine(String.Format("{0}\\{1}", InstalledUpdatesFolder, GetApplicationVersion()),
                                              InstalledUpdatesFileName));
        }

        public IList<IUpdatePatch> GetReceivedPatchList()
        {
            return
                BaseGetPatchList(Path.Combine(
                    String.Format("{0}\\{1}", InstalledUpdatesFolder, GetApplicationVersion()), ReceivedUpdatesFileName));
        }

        private IList<IUpdatePatch> BaseGetPatchList(string fileName)
        {
            IList<IUpdatePatch> patches = new List<IUpdatePatch>();

            string file =
                    Path.Combine(Path.GetDirectoryName(GetProcessModule().FileName),fileName);

            Trace.TraceVerbose("���� ������ ������ ���: {0}", file);

            if (File.Exists(file))
            {
                patches = UpdateFeedReader.Read(File.ReadAllText(file), null);
            }

            Trace.TraceVerbose(patches.Count.ToString());
            return patches;
        }

        /// <summary>
        /// ��������� ������� � ��������� �� �������
        /// </summary>
        /// <param name="client"></param>
        public void AttachClient(INotifierClient client)
        {
            if (client == null)
                return;

            lock (clients)
            {
                
                clients[(KristaApplication == KristaApp.Updater) ?
                    RemotingServices.GetObjectUri((MarshalByRefObject)client)
                    : "Client"] = client;
            }
        }
        
        #endregion

        #region ���������� �������� �� ��������� ��������� �������� ����������

        /// <summary>
        /// �������. ����� ���������� ����������
        /// </summary>
        /// <param name="updateProcessState"></param>
        /// <returns></returns>
        internal delegate object ReceiveUpdateStateEventHandler(UpdateProcessState updateProcessState);

        /// <summary>
        /// ���������� �������� �� ��������� ��������� �������� ����������
        /// </summary>
        /// <param name="updateProcessState">����� ���������</param>
        private void AsyncReceiveStateToClients(UpdateProcessState updateProcessState)
        {
            lock (clients)
            {
                AsyncCallback asyncCallback = OurAsyncCallbackHandler;
                foreach (DictionaryEntry entry in clients)
                {
                    INotifierClient iClient = (INotifierClient)entry.Value;
                    ReceiveUpdateStateEventHandler remoteAsyncDelegate = iClient.ReceiveNewState;

                    // ������, ������� ���������� � ���� �������� �� ����������� ��������
                    AsyncCallBackData asyncCallBackData = new AsyncCallBackData
                                                              {
                                                                  RemoteAsyncDelegate = remoteAsyncDelegate,
                                                                  MbrBeingCalled = (MarshalByRefObject) iClient
                                                              };

                    remoteAsyncDelegate.BeginInvoke(updateProcessState, asyncCallback, asyncCallBackData);
                }
            }
        }

        private static void OurAsyncCallbackHandler(IAsyncResult ar)
        {
            AsyncCallBackData asyncCallBackData = (AsyncCallBackData)ar.AsyncState;

            try
            {
                object result = asyncCallBackData.RemoteAsyncDelegate.EndInvoke(ar);
                if (!(bool)result)
                {
                    Trace.TraceWarning(String.Format("������� �� ���������� �� ������� {0}", asyncCallBackData.MbrBeingCalled));
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("������ ����: {0}.", ex.Message);
                // ������� ��� ����� �� �����������
                lock (clients)
                {
                    clients.Remove(RemotingServices.GetObjectUri(asyncCallBackData.MbrBeingCalled));
                }
            }
        }

        /// <summary>
        /// ������, ������� ���������� � ���� �������� �� ����������� ��������
        /// </summary>
        private class AsyncCallBackData
        {
            /// <summary>
            /// ���������� � ��������
            /// </summary>
            public ReceiveUpdateStateEventHandler RemoteAsyncDelegate;

            /// <summary>
            /// ���������� � �������
            /// </summary>
            public MarshalByRefObject MbrBeingCalled;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

    }
}