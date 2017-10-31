using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

using Krista.FM.Common;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Common.FileUtils;
using Krista.FM.ServerLibrary;


/// <summary>
/// ��������������� ������ ��� ������ � ��������� MS Office
/// ��� ����������� ������������� �� ������ MS Office ������������ ������� ����������
/// ������ �������� � OfficeXP � ����
/// </summary>
namespace Krista.FM.Client.Common
{
    public struct OfficeFileExt 
    {
        public const string ofWordDocument = ".doc";
        public const string ofExcelDocument = ".xls";
        public const string ofWordDocumentNew = ".docx";
        public const string ofExcelDocumentNew = ".xlsx";
    }

    /// <summary>
    /// ����� ��� ������ � ������� �����������
    /// </summary>
    public abstract class PluginHelper : DisposableObject
    {
        private string fmAddinProgID;

        public string FMAddinProgID
        {
            get { return fmAddinProgID; }
            set { fmAddinProgID = value; }
        }

        private string fmAddinRegPath;
        public string FMAddinRegPath
        {
            get { return fmAddinRegPath; }
            set { fmAddinRegPath = value; }
        }


        private bool _isPluginInstalled;
        internal bool IsPluginInstalled
        {
            get
            {
                return _isPluginInstalled;
            }
            set
            {
                _isPluginInstalled = value;
            }
        }

        internal bool CheckPlaginInstalled()
        {
            if (!Utils.CheckLibByProgID(FMAddinProgID, true))
            {
                return false;
            }
            // ��������� �������������������� � ����������� � �������� ������� Excel
            // ..� HKLM
            if (CheckPlaginInApplication(true))
                return true;
            if (!_isPluginInstalled && CheckPlaginInApplication(false))
                return true;
            return false;
        }

        protected OfficeHelper officeHelper;

        /// <summary>
        /// �������� �������������������� ������� � MS Office
        /// </summary>
        /// <param name="forAllUsers">��� ��������� � HKLM ��� � HKCU</param>
        /// <returns>true/false</returns>
        private bool CheckPlaginInApplication(bool forAllUsers)
        {
            RegistryKey key = forAllUsers ? Registry.LocalMachine.OpenSubKey(FMAddinRegPath, false) : Registry.CurrentUser.OpenSubKey(FMAddinRegPath, false);

            if (key == null)
                return false;

            int loadBehavior = -1;
            try
            {
                loadBehavior = Convert.ToInt32(key.GetValue("LoadBehavior"));
            }
            catch
            {

            }
            key.Close();
            return (loadBehavior == 3);
        }

        /// <summary>
        /// �������� �������� ��������� ������ �������
        /// </summary>
        internal IFMPlanningExtension GetFMPlanningExtensionInterface(object appObj)
        {
            if (!IsPluginInstalled)
            {
                return null;//throw new Exception("���������� ��� MS Office (������) �� �����������");
            }

            // � ������ ����� �������������� ����� ����� ��������� ���������� ������� 
            IFMPlanningExtension extension = null;
            // ������ �� �����������
            try
            {
                // �������� ���������  �������
                object comAddIns = ReflectionHelper.GetProperty(appObj, "COMAddIns");
                // �������� ���������� ������������� �������
                int comAddInsCount = Convert.ToInt32(ReflectionHelper.GetProperty(comAddIns, "Count"));
                // ���� ��� ������
                object comAddIn = null;
                for (int i = 1; i <= comAddInsCount; i++)
                {
                    comAddIn = ReflectionHelper.CallMethod(comAddIns, "Item", i);
                    string objName = Convert.ToString(ReflectionHelper.GetProperty(comAddIn, "ProgID"));
                    if (string.Compare(FMAddinProgID, objName, true) == 0)
                        break;
                    comAddIn = null;
                }
                // ���� ����� - �������� ��� �������� ���������
                if (comAddIn != null)
                {
                    object comAddInCoClass = ReflectionHelper.GetProperty(comAddIn, "Object");
                    extension = comAddInCoClass as IFMPlanningExtension;
                }
            }
            catch
            {

            }
            return extension;
        }

        internal static void ResetPlanningSetings(IFMPlanningExtension extension)
        {
            SetPlanningSettings(extension, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, -1, String.Empty);
        }

        internal void SetPlanningSettings(object obj, string taskName, string taskID, string documentName,
            string documentID, string doerName, int sheetType, string originalFileName)
        {
            // ��������� �����, ��������� ����
            object appObj = obj;
            IFMPlanningExtension extension = null;
            try
            {
                extension = GetFMPlanningExtensionInterface(appObj);
                SetPlanningSettings(extension, taskName, taskID, documentName,
                    documentID, doerName, sheetType, originalFileName);
            }
            finally
            {
                Marshal.ReleaseComObject(extension);
            }
        }

        internal static void SetPlanningSettings(IFMPlanningExtension extension, string taskName, string taskID, string documentName,
            string documentID, string doerName, int sheetType, string originalFileName)
        {
            if (extension == null)
                return;

            extension.SetPropValueByName(FMOfficeAddinsConsts.pspDocumentName, documentName);
            extension.SetPropValueByName(FMOfficeAddinsConsts.pspDocumentId, documentID);
            extension.SetPropValueByName(FMOfficeAddinsConsts.pspTaskName, taskName);
            extension.SetPropValueByName(FMOfficeAddinsConsts.pspTaskId, taskID);
            extension.SetPropValueByName(FMOfficeAddinsConsts.pspOwner, doerName);
            extension.SetPropValueByName(FMOfficeAddinsConsts.pspDocPath, originalFileName);
            if (sheetType != -1)
                extension.SetPropValueByName(FMOfficeAddinsConsts.pspSheetType, sheetType.ToString());
        }

        /// <summary>
        /// ��������-�� �������� ���������� �������
        /// </summary>
        /// <param name="dt">��� ���������</param>
        /// <returns>true/false</returns>
        internal static bool IsPlanningDocument(TaskDocumentType dt)
        {
            return (dt == TaskDocumentType.dtCalcSheet) ||
                (dt == TaskDocumentType.dtDataCaptureList) ||
                (dt == TaskDocumentType.dtInputForm) ||
                (dt == TaskDocumentType.dtPlanningSheet) ||
                (dt == TaskDocumentType.dtReport) ||
                (dt == TaskDocumentType.dtDummyValue);
        }

        internal static void SetPlaginParams(IFMPlanningExtension extension, bool showDocument)
        {
            if (extension != null)
            {
                extension.IsLoadingFromTask = true;
                // 
                extension.IsSilentMode = !showDocument;
            }
        }

        internal IFMPlanningExtension SetPlaginParams(object obj, bool showDocument)
        {
            IFMPlanningExtension extension = GetFMPlanningExtensionInterface(obj);

            if (extension != null)
            {
                extension.IsLoadingFromTask = true;
                // 
                extension.IsSilentMode = !showDocument;
            }
            return extension;
        }

        internal static void RestoreParams(IFMPlanningExtension extension)
        {
            extension.IsLoadingFromTask = false;
            extension.IsSilentMode = false;
        }

        internal void SetTaskContext(
            string originalFileName,
            bool showDocument,
            bool isConnectToTask,
            bool taskInEdit,
            IScheme scheme,
            string taskName,
            string taskID,
            string documentName,
            string documentID,
            string doerName,
            int docType,
            object taskContext,
            object appObj,
            IFMPlanningExtension externalPlaginItf
        )
        {
            IFMPlanningExtension extension = null;
            try
            {
                // �������� ��������� �������
                extension = externalPlaginItf ?? GetFMPlanningExtensionInterface(appObj);
                // ���� ��� ������������� � ������ - ��������� ������ ��������� � ������������� 
                // ����������� �����
                if (extension != null)
                {
                    extension.IsLoadingFromTask = true;
                    extension.IsSilentMode = !showDocument;
                }

                if (showDocument)
                    officeHelper.RestoreAddIns(appObj);

                // ���������� ����������� ��������� ������
                if (extension != null)
                {
                    // ���� � ����� ���������� � � �������� ������ ����
                    if (!taskInEdit)
                        extension.SetPropValueByName(FMOfficeAddinsConsts.pspDocPath, originalFileName);
                    SetPlanningSettings(extension, taskName, taskID, documentName, documentID,
                            doerName, docType, originalFileName);
                    // ���� �������� ������ � ������ �������������� - ������������� ��������� �����    
                    if (taskInEdit)
                    {                        
                        // ���� ��� ������������� � ������ - ������������� �������������� �����
                        if (isConnectToTask)
                            extension.OnTaskConnection(true);
                    }

                    // �������� ������ ���� ��������������� ������
                    extension.SetTaskContext(taskContext);

                    try
                    {
                        
                        int AuthType;
                        string Login;
                        string PwdHash;
                        ClientSession.GetAuthenticationInfo(scheme, out AuthType, out Login, out PwdHash);
                        extension.SetAuthenticationInfo(AuthType, Login, PwdHash);
                    }
                    catch { }

                    // ������ �� ������ ������ ����������
                    // ����������� ������������� ������ ����� ����, ��� �������� ��������� ��������������
                    bool connectedOK = false;
                    try
                    {
                        string serverName = string.Format("{0}:{1}", scheme.Server.Machine,
                            scheme.Server.GetConfigurationParameter("ServerPort"));
                        connectedOK = extension.SetConnectionStr(serverName, scheme.Name) == 0;
                    }
                    catch { }
                    if (!connectedOK)
                        try
                        {
                            extension.SetConnectionStr(scheme.Server.GetConfigurationParameter("WebServiceUrl"), scheme.Name);
                        }
                        catch
                        {
                        }
                }
            }
            finally
            {
                // ��������� ������� ����������� ���� ���� ��� �����������
                if ((extension != null) && (externalPlaginItf == null))
                    Marshal.ReleaseComObject(extension);
            }
        }
    }

    public class WordPluginHelper : PluginHelper
    {
        public WordPluginHelper(OfficeHelper officeHelper)
        {
            FMAddinProgID = FMOfficeAddinsConsts.wordAddinProgID;
            FMAddinRegPath = FMOfficeAddinsConsts.wordAddinRegPath;
            this.officeHelper = officeHelper;
            IsPluginInstalled = CheckPlaginInstalled();
        }
    }

    public class ExcelPluginHelper : PluginHelper
    {
        public ExcelPluginHelper(OfficeHelper officeHelper)
        {
            FMAddinProgID = FMOfficeAddinsConsts.excelAddinProgID;
            FMAddinRegPath = FMOfficeAddinsConsts.excelAddinRegPath;
            this.officeHelper = officeHelper;
            IsPluginInstalled = CheckPlaginInstalled();
        }
    }

    /// <summary>
    /// ������� ����������� ����� ��� ������ � ��������� MS Office
    /// </summary>
    public abstract class OfficeHelper : DisposableObject
    {

        internal PluginHelper pluginHelper;
        internal virtual PluginHelper PluginHelper
        {
            get
            {
                return pluginHelper;
            }
        }

        // ������ proxy ����������� ��������, ��������� ����� ���� ��������� (������� �������������)
        protected ArrayList objectsList = new ArrayList();

        // ����������� �����������.
        // ���������� ����������� �������
        public OfficeHelper()
        {
        }

        private bool _connectToExistingObject = false;
        public bool ConnectToExistingObject
        {
            get { return _connectToExistingObject; }
            set { _connectToExistingObject = value; }
        }

        public OfficeHelper(bool connectToExistingObject)
        {
            _connectToExistingObject = connectToExistingObject;
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    // �������� ������� �������
                    foreach (object obj in objectsList)
                    {
                        object tmpObj = obj;
                        CloseOfficeObj(ref tmpObj);
                    }
                    // ������� ������
                    objectsList.Clear();
                    // �������� ������� ������ ��� ����������� ������� ������
                    GC.GetTotalMemory(true);
                }
            }
        }

        /// <summary>
        /// ������� Locale ID - ��������� ��������� �������� � �������� ���������
        /// </summary>
        public static int CurrentLCID
        {
            get { return System.Globalization.CultureInfo.CurrentCulture.LCID; }
        }

        /// <summary>
        /// ����������� ����� ��������� ProgID ������� MS Office. ������������ ���
        /// �������� ��������. ������ ���� ���������� � ��������.
        /// </summary>
        /// <returns>ProgID ������� MS Office, �������� ��� Excel ��� ����� "Excel.Application"</returns>
        protected abstract string GetProgID();

        /// <summary>
        /// ����������� ����� ��������� ���������� ������ ��� ������� MS Office.
        /// ������������ ��� �������� ������������ ������������ � ���������� ���� ������.
        /// ������ ���� ���������� � ��������.
        /// </summary>
        /// <returns>���������� �����, �������� ��� Excel ��� ����� ".xls"</returns>
        [System.Obsolete("� ������ 2007 ����� ���������� ����� ����� ���������� ������������")]
        public abstract string GetFileExt();

        protected bool isConnected = false;

        protected virtual void CustomizeOfficeObject(object appObj)
        {
        }

        /// <summary>
        /// ������������ COM-������ �� ������ MS Ofiice
        /// </summary>
        /// <param name="obj">������ MS Ofiice</param>
        public static void ReleaseComReference(ref object obj)
        {
            try
            {
                // �������� ���������� COM-������
                if (Marshal.IsComObject(obj))
                    Marshal.ReleaseComObject(obj);
                // ������ Excel'� �� ������ ����� ������� ���� ������
                // ������� ���� �� ���������� ��� ������ � ������ .NET Framework
                // ��� ���� �� ������ ��������...
            }
            catch 
            { 

            }
            obj = null;
            GC.GetTotalMemory(true);
        }

        #region ������ � ���������

        /// <summary>
        /// �������� �������� ��������� ������ �������
        /// </summary>
        /// <returns>�������� ���������</returns>
        public IFMPlanningExtension GetFMPlanningExtensionInterface(object appObj)
        {
            return PluginHelper.GetFMPlanningExtensionInterface(appObj);
        }

        /// <summary>
        /// ������� ����������� �������
        /// </summary>
        public bool PlaginInstalled
        {
            get
            {
                return PluginHelper.IsPluginInstalled;
            }
        }

        /// <summary>
        /// ��������� ProgID ������ ������� � ������� �������� ������ ��� �������
        /// </summary>
        /// <returns>ProgID</returns>
        public abstract string GetFMAddinProgID();

        /// <summary>
        /// ���� � �������������� ������� � ������. ������������ ��� ������� �������� ����������� �������.
        /// </summary>
        /// <returns>���� � �������</returns>
        public abstract string GetFMAddinRegPath();

        public string GetPlanningDocumentType(string fileName, bool isNewOffice)
        {
            if (isNewOffice)
                return GetOffice2007CustomDocumentProperty(fileName, FMOfficeAddinsConsts.pspSheetType);

            return StgStorageHelper.GetStgFileCustomProp(fileName, FMOfficeAddinsConsts.pspSheetType);
        }

        public void ResetPlanningSettings(object obj)
        {
            PluginHelper.SetPlanningSettings(obj, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, -1, String.Empty);
        }

        protected static void ResetPlanningSetings(IFMPlanningExtension extension)
        {
            PluginHelper.SetPlanningSettings(extension, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, -1, String.Empty);
        }

        /// <summary>
        /// ��������-�� �������� ���������� �������
        /// </summary>
        /// <param name="dt">��� ���������</param>
        /// <returns>true/false</returns>
        public static bool IsPlanningDocument(TaskDocumentType dt)
        {
            return (dt == TaskDocumentType.dtCalcSheet) ||
                (dt == TaskDocumentType.dtDataCaptureList) ||
                (dt == TaskDocumentType.dtInputForm) ||
                (dt == TaskDocumentType.dtPlanningSheet) ||
                (dt == TaskDocumentType.dtReport) ||
                (dt == TaskDocumentType.dtDummyValue);
        }

        #endregion

        #region ������, ��������������� �� ������� COM ���������

        [DllImport("user32.dll")]
        protected static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("User32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        protected static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        protected static extern bool IsIconic(IntPtr hWnd);

        protected const int SW_RESTORE = 9;
        protected const int SW_MAXIMIZE = 3;

        #endregion

        #region ������ � ������� �����������

        public static object GetOfficeObject(string progID, bool connectToExisting)
        {
            bool isConnected;
            return GetOfficeObject(progID, connectToExisting, true, out isConnected);
        }

        public static object GetOfficeObject(string progID, bool connectToExisting, bool forceCreate)
        {
            bool isConnected;
            return GetOfficeObject(progID, connectToExisting, forceCreate, out isConnected);
        }

        /// <summary>
        /// ������� ������ MS Office
        /// </summary>
        /// <param name="progID">Prog ID �������</param>
        /// <returns>������ MS Office</returns>
        private static object GetOfficeObject(string progID, bool connectToExisting, bool forceCreate,
            out bool isConnected)
        {
            object obj = null;
            // ���� ����� ����������� � ������������� ������� - ���� ��� � ROT
            isConnected = false;
            if (connectToExisting)
            {
                try
                {
                    // ����� ������ ������, �������� �� ����������
                    obj = Marshal.GetActiveObject(progID);
                }
                catch
                {

                }
                if (obj == null)
                {
                    if (!forceCreate)
                        return obj;
                }
                else
                {
                    isConnected = true;
                    return obj;
                }
            }
            // ���� �� ���������� - ������� ����� ���������
            // �������� ��� �������
            Type objectType = Type.GetTypeFromProgID(progID);

            // ���� �� ������� - ���������� ���������� �� ������
            if (objectType == null)
                throw new Exception("���������� ������� ������ " + progID);
            // ������� ������ MS Office
            obj = Activator.CreateInstance(objectType);
            // ���������� ��������� ��� ���������� ������
            return obj;
        }

        /// <summary>
        /// �������� ������� MS Office
        /// </summary>
        /// <returns></returns>
        public virtual object GetOfficeObject()
        {
            object appObj = GetOfficeObject(GetProgID(), _connectToExistingObject, true, out isConnected);
            CustomizeOfficeObject(appObj);
            return appObj;
        }

        /// <summary>
        /// ���������/��������� ���������� ����
        /// </summary>
        /// <param name="obj">������ MS Office</param>
        /// <param name="updating">��������</param>
        public static void SetScreenUpdating(object obj, bool updating)
        {
            ReflectionHelper.SetProperty(obj, "ScreenUpdating", updating);
        }

        /// <summary>
        /// ������� ������ MS Office. ���������� proxy com-�������, ������������� ���������
        /// �������� (��� ������� Quit c ������� �������� ���������� ������ ���������� � ��������)
        /// </summary>
        /// <param name="obj">��������� �� ������ MS Office</param>
        public virtual void CloseOfficeObj(ref object obj)
        {
            // ���� ��������� ��������� - �������
            if (obj == null)
                return;
            // ���������� ������ ������� 
            int index = objectsList.IndexOf(obj);
            if (index != -1)
                // �������� ��������� �� ���������� ������
                objectsList[index] = null;
            ReleaseComReference(ref obj);
        }

        /// <summary>
        /// ��������/������ ������ MS Office
        /// </summary>
        /// <param name="obj">������ MS Office</param>
        /// <param name="visible">������� ���������</param>
        public static void SetObjectVisible(object obj, bool visible)
        {
            ReflectionHelper.SetProperty(obj, "Visible", visible);
        }

        public abstract void SetDisplayAlert(object obj, bool displayAlert);

        /// <summary>
        /// �������� ��������� �������� ����������. ������� ��� ������� �����
        /// </summary>
        /// <param name="appObj"></param>
        private void CheckExistingObject(ref object appObj)
        {
            string[] versionString = new string[2];
            try
            {
                versionString = ReflectionHelper.GetProperty(appObj, "Version").ToString().Split('.');
            }
            catch
            {
                ConnectToExistingObject = false;
                appObj = GetOfficeObject();
            }
            
            int versionFromApp = Convert.ToInt32(versionString[0]);
            int versionFromRegistry = GetVersionNumber();

            if (versionFromApp != versionFromRegistry)
            {
                ConnectToExistingObject = false;
                appObj = GetOfficeObject();
            }
        }

        /// <summary>
        /// �������� ��������� ������� �������� ����������
        /// </summary>
        /// <param name="appObj"></param>
        /// <param name="showDocument"></param>
        private void GetAppObj(ref object appObj, bool showDocument)
        {
            if (appObj == null)
            {
                // ���� �������� ����� ������� ������������ - ����������� � ������������ ����������
                ConnectToExistingObject = showDocument;
                appObj = GetOfficeObject();

                // ���� ��� �� ���������� ������ ��� ����������� ����� - �������� ���� ���������
                if (ConnectToExistingObject)
                    CheckExistingObject(ref appObj);
            }
        }

        /// <summary>
        /// �������������� ��������� ��� �����
        /// </summary>
        public abstract void RestoreAddIns(object appObj);

        protected virtual void DeactivateDocument(object appObj){}

        #endregion

        #region ������ � �����������

        private enum FileState
        {
            New,
            Occupied,
            Vacant
        } ;

        /// <summary>
        /// ������� ������ �������� MS Office � ��������� ��� �� ���������� ������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        public object CreateEmptyDocument(string fileName)
        {
            object obj = GetOfficeObject();
            CreateEmptyDocument(obj, fileName);
            return obj;
        }

        /// <summary>
        /// �������� ����� ����� �� ������������� � �������������� � ������� ������� ���� (�� ����������).
        /// ������������ ��� �������� ���������� �������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        protected void CheckFile(string fileName)
        {
            // ��������� ��������������
            if (!CheckFileExt(fileName))
                throw new Exception("���� '" + fileName + "' ����� �������� ������");
            // ��������� ������� �����
            if (!File.Exists(fileName))
                throw new Exception("���� '" + fileName + "' �� ����������");
        }

        /// <summary>
        /// ����� �� ���� ���� ��������� ��������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        /// <returns>true/false</returns>
        public bool IsApplicableFile(string fileName)
        {
            bool isApplicable = false;
            try
            {
                CheckFile(fileName);
                isApplicable = true;
            }
            catch
            {

            }
            return isApplicable;
        }

        public abstract object CreateEmptyDocument(object appObj, string fileName);

        /// <summary>
        /// ����� �������� ��������� MS Office
        /// </summary>
        /// <param name="fileName">��� ����� � ����������</param>
        /// <param name="openReadOnly">������� "������� ������ ��� ������"</param>
        /// <param name="show">�������� ������ MS Office ����� ��������</param>
        /// <returns>��������� �� ������ MS Office</returns>
        /// <summary>
        /// ������� �������� Excel
        /// </summary>
        /// <param name="fileName">��� �����</param>
        /// <param name="openReadOnly">������� ������ ��� ������</param>
        /// <param name="show">�������� Excel</param>
        /// <returns>��������� �� ������ Excel ��� ���������� ������</returns>
        public object OpenFile(string fileName, bool openReadOnly, bool show)
        {
            // ��������� ������������ �����
            CheckFile(fileName);
            // ��������� Excel
            object appObj = GetOfficeObject();
            // ��������� ����
            //object workbook = 
            LoadFile(appObj, fileName, openReadOnly);
            // ���������� Excel ���� ����������
            if (show)
                SetObjectVisible(appObj, true);
            // ���������� ��������� �� ������ ��� ���������� ������
            return appObj;
        }

        public abstract object LoadFile(object appObj, string fileName, bool openReadOnly);


        /// <summary>
        /// ����������� ����� �������� ������������ ���������� ����� ���� �������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        /// <returns></returns>
        public abstract bool CheckFileExt(string fileName);

        public void OpenDocumentFromTask(
            string originalFileName, bool showDocument,
            bool isConnectToTask, bool taskInEdit,
            IScheme scheme, string taskName,
            string taskID, string documentName,
            string documentID, string doerName,
            int docType, object taskContext
        )
        {
            OpenDocumentFromTask(originalFileName, showDocument, isConnectToTask, taskInEdit, scheme, taskName, taskID,
                documentName, documentID, doerName, docType, taskContext, null, null);
        }

        /// <summary>
        /// �������� ��������� �� ������
        /// </summary>
        public void OpenDocumentFromTask(
            string originalFileName, bool showDocument, bool isConnectToTask,
            bool taskInEdit, IScheme scheme, string taskName,
            string taskID, string documentName, string documentID,
            string doerName, int docType, object taskContext,
            object externalOfficeObject, IFMPlanningExtension externalPlaginItf
        )
        {
            // �������� ��������� �����
            FileState fileState = GetFileState(originalFileName);
            // ��� ���� ������������?
            TaskDocumentType dt = (TaskDocumentType)docType;
            bool isPlanningSheet = IsPlanningDocument(dt);

            object document = null;
            object appObj = externalOfficeObject;
            GetAppObj(ref appObj, showDocument);

            if (showDocument)
            {
                // ���������� ��������
                SetObjectVisible(appObj, true);
                // ������ ��� �� ����� ����������� ��� ��������
                DeactivateDocument(appObj);
            }

            IFMPlanningExtension extension = null;
            try
            {
                // �������� ��������� �������
                if (isPlanningSheet)
                {
                    if (externalPlaginItf == null)
                        extension = PluginHelper.SetPlaginParams(appObj, showDocument);
                    else
                    {
                        PluginHelper.SetPlaginParams(externalPlaginItf, showDocument);
                        extension = externalPlaginItf;
                    }
                }
                // ���� �������� ����� - �������
                if (fileState == FileState.New)
                {
                    document = CreateEmptyDocument(appObj, originalFileName);
                }
                else
                {
                    // �������� ��������� ������������
                    document = LoadFile(appObj, originalFileName, !taskInEdit);
                    if (document == null)
                        return;
                }
                // �������� �������� ��������� �����
                if (isPlanningSheet)
                {
                    PluginHelper.SetTaskContext(originalFileName, showDocument, isConnectToTask, taskInEdit, scheme,
                                                taskName, taskID, documentName, documentID, doerName, docType,
                                                taskContext,
                                                appObj, externalPlaginItf);
                }
                // ���� ������ �� ��������������, �������� ��������� ������ 
                if (taskInEdit)
                {
                    if (fileState == FileState.Occupied)
                    {
                        string errStr = String.Format("���� '{0}' ������������ ������ ���������. ���� ����� ������ � ������ ������ ��� ������.", originalFileName);
                        MessageBox.Show(errStr, "������ ���������� �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SaveChanges(appObj, document, originalFileName);
                    }
                    if (isPlanningSheet)
                    {
                        PluginHelper.RestoreParams(extension);
                    }
                }
            }
            finally
            {
                // ��������� ������� ����������� ���� ���� ��� �����������
                if ((extension != null) && (externalPlaginItf == null))
                    Marshal.ReleaseComObject(extension);
                //  � ����������� �� ��������� ������� ������ � ���� ��� �� ��� �������..
                if ((appObj != null) && (externalOfficeObject == null))
                {
                    if (!showDocument)
                        // ..��������� ��� ������
                        CloseOfficeObj(ref appObj);
                    else
                    {
                        ActivateDocument(appObj);
                        // ..����������� ������ COM
                        if (document != null)
                            Marshal.ReleaseComObject(document);
                        Marshal.ReleaseComObject(appObj);
                    }
                }
            }
        }

        private static string GetOffice2007CustomDocumentProperty(string fileName, string propName)
        {
            // ��������� ����������� �����
            string curFileName = fileName;
            if (!File.Exists(fileName))
                new FileNotFoundException(String.Format("���� '{0}' �� ������", fileName));

            // �������� �� ��� ������� ���� � ������������ ������?
            bool needClone = !FileHelper.FileAllowExclusiveAccess(curFileName);
            if (needClone)
            {
                // ���� ��� - �������� �������� ���� �� ��������� ����������
                string newFileName = Path.GetTempPath() + Path.GetRandomFileName() + Path.GetExtension(curFileName);
                //string newFileName = Path.GetTempPath() + Guid.NewGuid().ToString() + Path.GetExtension(curFileName);
                byte[] fileData = FileHelper.ReadFileData(fileName);
                FileStream fs = new FileStream(newFileName, FileMode.CreateNew);
                fs.Write(fileData, 0, fileData.Length);
                fs.Close();
                curFileName = newFileName;
            }

            // ����� ������ ����� ������������ ����� ���-������
            // ������������ ��� ���������������� �������� �������� � ����� "docProps/custom.xml"
            ZipFile zipFile = new ZipFile(curFileName);
            try
            {
                ZipEntry Entry = zipFile.GetEntry("docProps/custom.xml");
                if (Entry == null)
                    return string.Empty;

                XmlDocument doc = new XmlDocument();
                doc.Load(zipFile.GetInputStream(Entry));
                XmlNode node = doc.SelectSingleNode(string.Format("//*[@name = '{0}']", propName));
                try
                {
                    return node.FirstChild.FirstChild.Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
            finally
            {
                zipFile.Close();
                // ���� ���� ��� ���������� �� ��������� ������� - ������� ���
                if ((needClone) && File.Exists(curFileName))
                {
                    File.Delete(curFileName);
                }
            }
        }

        /// <summary>
        /// ��������� ����������� �����
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        private static FileState GetFileState(string originalFileName)
        {
            // ��� ����� ����?
            bool fileIsNew = !File.Exists(originalFileName);
            if (fileIsNew)
                return FileState.New;
            // ���� �� ����� ������� ����������?
            bool fileIsVacant = FileHelper.FileIsVacant(originalFileName);
            if (fileIsVacant)
                return FileState.Vacant;
            return FileState.Occupied;
        }

        protected abstract void ActivateDocument(object appObj);

        /// <summary>
        /// ���������� ���������
        /// </summary>
        public abstract void SaveChanges(object appObj, object docObj, string fileName);

        #endregion

        #region ����������� ������

        public abstract string GetExtension();

        /// <summary>
        /// ������� �� ������� ProgId � ���������� ����� ������
        /// </summary>
        public abstract int GetVersionNumber();

        #endregion
    }

    /// <summary>
    /// ����� ��� ������ � MS Excel
    /// </summary>
    public class ExcelHelper : OfficeHelper
    {
        public ExcelHelper(): base()
        {
        }

        public ExcelHelper(bool connectToExistingObject) : base(connectToExistingObject)
        {
        }

        internal override PluginHelper PluginHelper
        {
            get
            {
                if (pluginHelper == null)
                    pluginHelper = new ExcelPluginHelper(this);
                return pluginHelper;
            }
        }

        private const int xlWorkbookNormal = -4143;
        private const int xlWorkbookDefault = 51;

        /// <summary>
        /// ProgID Excel
        /// </summary>
        /// <returns>ProgID</returns>
        protected override string GetProgID()
        {
            return "Excel.Application";
        }

        public override string GetFMAddinRegPath()
        {
            return FMOfficeAddinsConsts.excelAddinRegPath;
        }

        public override string GetFMAddinProgID()
        {
            return FMOfficeAddinsConsts.excelAddinProgID;
        }


        /// <summary>
        /// �������� ������� Excel
        /// </summary>
        /// <param name="obj"></param>
        public override void CloseOfficeObj(ref object obj)
        {
            if (obj == null)
                return;
            // ���� Excel ��� �� ������ - �������� �������
            try
            {
                // ��������� �������������� 
                SetDisplayAlert(obj, false);
                // �������� ����� Quit
                ReflectionHelper.CallMethod(obj, "Quit");
            }
            catch 
            { 

            }
            base.CloseOfficeObj(ref obj);
        }

        /// <summary>
        /// ���������� ��������� ��������������
        /// </summary>
        /// <param name="obj">������ Excel</param>
        /// <param name="displayAlert">��������/��������� �������������</param>
        public override void SetDisplayAlert(object obj, bool displayAlert)
        {
            ReflectionHelper.SetProperty(obj, "DisplayAlerts", displayAlert);
        }

        /// <summary>
        /// ���������� ��� ������ Excel
        /// </summary>
        /// <returns>����������</returns>
        [System.Obsolete("� ������ 2007 ����� ���������� ����� ����� ���������� ������������")]
        public override string GetFileExt()
        {
            return OfficeFileExt.ofExcelDocument;
        }

        public override bool CheckFileExt(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return ((ext == OfficeFileExt.ofExcelDocument) || (ext == OfficeFileExt.ofExcelDocumentNew)); 
        }

        /// <summary>
        /// ��������� ��������� Workbooks
        /// </summary>
        /// <param name="applicationObject">������ Excel</param>
        /// <returns>��������� Workbooks</returns>
        public static object GetWorkbooks(object applicationObject)
        {
            return ReflectionHelper.GetProperty(applicationObject, "Workbooks");
        }

        #region �������������� ���������������� ��������

        #region ���������
        // �������� XLL ����� ���������� "������ ������"
        private const string ANALYS_XLL_NAME = "ANALYS32.XLL";
        // �������� XLA ����� ���������� "������ ������"
        private const string ANALYS_XLA_NAME = "atpvbaen.xla";
        #endregion

        #region ��������������� ����������
        // ����� ��������� AddIn, ������ ��� �������� ������� ���������
        [ComImport, InterfaceType((short)2), Guid("00020857-0000-0000-C000-000000000046"), TypeLibType((short)0x1000)]
        private interface AddIn
        {
            [DispId(550)]
            bool Installed
            {
                [PreserveSig, MethodImpl(MethodImplOptions.InternalCall,
                 MethodCodeType = MethodCodeType.Runtime), DispId(550)]
                get;
                [param: In]
                [PreserveSig, MethodImpl(MethodImplOptions.InternalCall,
                MethodCodeType = MethodCodeType.Runtime), DispId(550)]
                set;
            }
            [DispId(110)]
            string Name { [return: MarshalAs(UnmanagedType.BStr)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(110)] get; }
            [DispId(0x123)]
            string Path { [return: MarshalAs(UnmanagedType.BStr)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x123)] get; }
        }

        // ����� ���������� AddIns, ������ ��� �������� ������� ��������
        [ComImport, InterfaceType((short)2), Guid("00020858-0000-0000-C000-000000000046"), TypeLibType((short)0x1000)]
        private interface AddIns
        {
            [DispId(0x76)]
            int Count { [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x76)] get; }
            [DispId(0)]
            AddIn this[object Index] { [return: MarshalAs(UnmanagedType.Interface)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0), TypeLibFunc((short)0x400)] get; }
        }
        #endregion

        /// <summary>
        /// ������������ ��������������� AddIn'��, � ��������� ���������� "������ ������"
        /// </summary>
        /// <param name="appObj">������ ���������� Excel</param>
        public override void RestoreAddIns(object appObj)
        {
            // ���� ������������ � ����� ��������� ���������� - ������ ������ �� �����
            if (isConnected)
                return;

            // ��� ��������������� �������� Excel ����������� ��� ������������ ���������� �������
            AddIns addIns = null;
            AddIn addIn = null;
            object xla = null;
            try
            {
                // �������� ���������  �������
                addIns = ReflectionHelper.GetProperty(appObj, "AddIns") as AddIns;
                int cnt = addIns.Count;
                for (int i = 1; i <= cnt; i++)
                {
                    addIn = addIns[i];
                    string addInName = addIn.Name;
                    // ���� ���������� "������ ������"
                    // .. ���� ��� �������������
                    if ((String.Compare(ANALYS_XLL_NAME, addInName, true) == 0) 
                        // .. � �������� � Excel, ����������� ������� �������
                        && (addIn.Installed))
                    {
                        // ��������� ����������� ������
                        ReflectionHelper.SetProperty(appObj, "ScreenUpdating", false);
                        // ��������� ��� XLA �����, ��� ���������� ���������������� �������
                        string xlaFileName = addIn.Path + "\\" + ANALYS_XLA_NAME;
                        // ��������� ���
                        xla = this.LoadFile(appObj, xlaFileName, true);
                        // ��������� �������
                        ReflectionHelper.CallMethod(xla, "RunAutoMacros", 1);
                        // ��������� �����
                        ReflectionHelper.CallMethod(xla, "Close");
                        return;
                    }
                    else
                        // ��� �� ���� ���������� ��� ���������������� �� �� ����� - ����������� ���������
                        Marshal.ReleaseComObject(addIn);
                }
            }
            catch
            {
                // ����� ��������� ������ �������������, � ���� ������ ���������� "����� �������" ��������� �� �����
            }
            finally
            {
                // ��������� ����������� ������
                ReflectionHelper.SetProperty(appObj, "ScreenUpdating", true);
                // ����������� ��� ����������
                if (xla != null)
                    Marshal.ReleaseComObject(xla);
                if (addIn != null)
                    Marshal.ReleaseComObject(addIn);
                if (addIns != null)
                    Marshal.ReleaseComObject(addIns);
                // ���������� �������������� ������ ������
                GC.GetTotalMemory(true);
            }
        }
        #endregion


        /// <summary>
        /// ��������� ��������
        /// </summary>
        /// <param name="appObj">������ Excel</param>
        /// <param name="fileName">��� ����� ���������</param>
        /// <param name="openReadOnly">������� ������ ��� ������</param>
        public override object LoadFile(object appObj, string fileName, bool openReadOnly)
        {
            try
            {
                // �������� ��������� Workbooks
                object workbooksObj = GetWorkbooks(appObj);
                // ������� ������� �����
                return ReflectionHelper.CallMethod(workbooksObj, "Open",
                  fileName, Type.Missing, openReadOnly, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing);
            }
            catch
            {
                string errStr = String.Format("���������� ������� ���� '{0}'. �������� �� ��������� ��� ������������ ������ ���������.", fileName);
                MessageBox.Show(errStr, "������ �������� �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// ��������� ����
        /// </summary>
        private void InternalSaveChanges(object workbook, string fileName)
        {
            // ��������� � ����
            try
            {
                ReflectionHelper.CallMethod(workbook, "_SaveAs",
                    fileName,       // Filename: OleVariant; 
                    ReflectionHelper.GetProperty(workbook, "FileFormat"),//xlWorkbookDefault, //xlWorkbookNormal,
                    //Missing.Value,  // FileFormat: OleVariant; 
                    Missing.Value,  // Password: OleVariant; 
                    Missing.Value,  // WriteResPassword: OleVariant; 
                    Missing.Value,  // ReadOnlyRecommended: OleVariant; 
                    Missing.Value,  // CreateBackup: OleVariant; 
                    Missing.Value,  // AccessMode: XlSaveAsAccessMode; 
                    Missing.Value,  // ConflictResolution: OleVariant; 
                    Missing.Value,  // AddToMru: OleVariant; 
                    Missing.Value,  // TextCodepage: OleVariant; 
                    Missing.Value   // TextVisualLayout: OleVariant; 
                );
            }
            catch
            {
                throw new Exception(
                    String.Format(
                        "���������� ��������� ��������� � ��������� {0}. " + 
                        "�������� ���� ����� ������ ��������� ��� ����� ������������ ������������.",
                        fileName)
                    );
            }
        }

        private object GetFirstWorkbook(object excelObj)
        {
            object workbooks = GetWorkbooks(excelObj);
            return ReflectionHelper.GetProperty(workbooks, "Item", 1);
        }

        public override void SaveChanges(object appObj, object docObj, string fileName)
        {
            object document = docObj;
            if (document == null)
            {
                object workbooks = GetWorkbooks(appObj);
                int bookcount = (int)ReflectionHelper.GetProperty(workbooks, "Count");
                for (int i = 1; i <= bookcount; i++)
                {
                    object book = ReflectionHelper.GetProperty(workbooks, "Item", i);
                    if (ReflectionHelper.GetProperty(book, "Fullname").ToString() == fileName)
                    {
                        document = book;
                        break;
                    }
                }

            }
            //document = GetFirstWorkbook(appObj);
            SetDisplayAlert(appObj, false);
            InternalSaveChanges(document, fileName);
            SetDisplayAlert(appObj, true);
            if (document != null)
                Marshal.ReleaseComObject(document);
        }

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        /// <param name="excelObj">������ Excel</param>
        public static void CloseWorkBooks(object excelObj)
        {
            object workbooks = GetWorkbooks(excelObj);
            ReflectionHelper.CallMethod(workbooks, "Close");
            Marshal.ReleaseComObject(workbooks);
        }

        public override object CreateEmptyDocument(object excelObject, string fileName)
        {
            // ��������� ������ �����
            object workbooks = GetWorkbooks(excelObject);
            object workbook = ReflectionHelper.CallMethod(workbooks, "Add");
            InternalSaveChanges(workbook, fileName);
            return workbook;
        }

        public override int GetVersionNumber()
        {
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\{00024500-0000-0000-C000-000000000046}\\ProgID");
            if (rk != null)
            {
                string[] keyValue = rk.GetValue("").ToString().Split('.');
                rk.Close();
                return Convert.ToInt32(keyValue[2]);
            }
            return 0;
        }

        public override string GetExtension()
        {
            if (GetVersionNumber() < 12)
                 return OfficeFileExt.ofExcelDocument;
            else
                 return OfficeFileExt.ofExcelDocumentNew;
        }

        protected override void ActivateDocument(object appObj)
        {
            int excelWnd = (int)ReflectionHelper.GetProperty(appObj, "Hwnd");
            object activeBook = ReflectionHelper.GetProperty(appObj, "ActiveWorkbook");
            if (activeBook != null)
                ReflectionHelper.CallMethod(activeBook, "Activate");

            if (excelWnd > 0)
            {
                EnableWindow((IntPtr)excelWnd, true);
                if (IsIconic((IntPtr)excelWnd))
                    ShowWindow((IntPtr)excelWnd, SW_MAXIMIZE);
                else
                    SetForegroundWindow((IntPtr)excelWnd);
                ShowWindow((IntPtr)excelWnd, SW_MAXIMIZE);
            }
        }

        protected override void DeactivateDocument(object appObj)
        {
            int excelWnd = (int)ReflectionHelper.GetProperty(appObj, "Hwnd");
            EnableWindow((IntPtr)excelWnd, false);
        }
    }

    /// <summary>
    /// ����� ��� ������ � MS Word
    /// </summary>
    public class WordHelper : OfficeHelper
    {
        public WordHelper(): base()
        {
        }

        public WordHelper(bool connectToExistingObject) : base(connectToExistingObject)
        {
        }

        internal override PluginHelper PluginHelper
        {
            get
            {
                if (pluginHelper == null)
                    pluginHelper = new WordPluginHelper(this);
                return pluginHelper;
            }
        }

        private const int wdFormatDocument = 0;

        /// <summary>
        /// ProgID MS Word
        /// </summary>
        /// <returns>ProgID</returns>
        protected override string GetProgID()
        {
            return "Word.Application";
        }

        public override string GetFMAddinRegPath()
        {
            return FMOfficeAddinsConsts.wordAddinRegPath;
        }

        public override string GetFMAddinProgID()
        {
            return FMOfficeAddinsConsts.wordAddinProgID;
        }


        /// <summary>
        /// ���������� ��������� ��������������
        /// </summary>
        /// <param name="obj">������ Excel</param>
        /// <param name="displayAlert">��������/��������� �������������</param>
        public override void SetDisplayAlert(object obj, bool displayAlert)
        {
            uint alertsLevel = displayAlert ? 0xFFFFFFFF : 0;
            ReflectionHelper.SetProperty(obj, "DisplayAlerts", alertsLevel);
        }

        /// <summary>
        /// ���������� ��� ������ MS Word (".doc")
        /// </summary>
        /// <returns>����������</returns>
        [System.Obsolete("� ������ 2007 ����� ���������� ����� ����� ���������� ������������")]
        public override string GetFileExt()
        {
            return OfficeFileExt.ofWordDocument;
        }

        public override bool CheckFileExt(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return ((ext == OfficeFileExt.ofWordDocument) || (ext == OfficeFileExt.ofWordDocumentNew));
        }

        public override void RestoreAddIns(object appObj)
        {
        }

        public override object LoadFile(object appObj, string fileName, bool openReadOnly)
        {
            try
            {
                object documents = GetDocuments(appObj);
                return ReflectionHelper.CallMethod(documents, "Open",
                    fileName,      // FileName
                    Missing.Value, // ConfirmConversions
                    openReadOnly,  // ReadOnly
                    Missing.Value, // AddToRecentFiles
                    Missing.Value, // PasswordDocument
                    Missing.Value, // PasswordTemplate
                    Missing.Value, // Revert
                    Missing.Value, // WritePasswordDocument
                    Missing.Value, // WritePasswordTemplate
                    Missing.Value, // Format
                    Missing.Value, // Encoding
                    Missing.Value, // Visible
                    Missing.Value, // OpenAndRepair
                    Missing.Value, // DocumentDirection
                    Missing.Value  // NoEncodingDialog
                );
            }
            catch
            {
                string errStr = String.Format("���������� ������� ���� '{0}'. �������� �� ��������� ��� ������������ ������ ���������.", fileName);
                MessageBox.Show(errStr, "������ �������� �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        public static void ReleaseNormalDot(object appObj)
        {
            // ����������� ������ ���
            object normalTemplate = null;
            try
            {
                normalTemplate = ReflectionHelper.GetProperty(appObj, "NormalTemplate");
                ReflectionHelper.SetProperty(normalTemplate, "Saved", true);
            }
            finally
            {
                if (normalTemplate != null)
                    Marshal.ReleaseComObject(normalTemplate);
            }
        }

        /// <summary>
        /// ������� ������ �������� MS Word � ��������� ��� �� ���������� ������
        /// </summary>
        public override object CreateEmptyDocument(object appObj, string fileName)
        {
            // �������� ��������� Documents
            object documents = ReflectionHelper.GetProperty(appObj, "Documents");
            // ��������� ����� ������ ��������
            object document = ReflectionHelper.CallMethod(documents, "Add",
                Missing.Value,	// var Template: OleVariant; 
                Missing.Value,	// var NewTemplate: OleVariant; 
                0,              //var DocumentType: OleVariant = wdNewBlankDocument 
                true			// var Visible: OleVariant
            );
            // ���������
            ReflectionHelper.CallMethod(document, "SaveAs2000",
                fileName,		// var FileName: OleVariant; 
                wdFormatDocument,
                //Missing.Value,	// var FileFormat: OleVariant; 
                Missing.Value,	// var LockComments: OleVariant; 
                Missing.Value,	// var Password: OleVariant; 
                false,			// var AddToRecentFiles: OleVariant; 
                Missing.Value,	// var WritePassword: OleVariant; 
                Missing.Value,	// var ReadOnlyRecommended: OleVariant; 
                Missing.Value,	// var EmbedTrueTypeFonts: OleVariant; 
                Missing.Value,	// var SaveNativePictureFormat: OleVariant; 
                Missing.Value,	// var SaveFormsData: OleVariant; 
                Missing.Value 	// var SaveAsAOCELetter: OleVariant
            );
            // ���������� ����
            SetObjectVisible(appObj, true);
            ReleaseNormalDot(appObj);
            return document;
        }

        /// <summary>
        /// ������� ������ MS Word
        /// </summary>
        /// <param name="obj"></param>
        public override void CloseOfficeObj(ref object obj)
        {
            if (obj == null)
                return;
            // ���� Word ��� �� ������ - �������� �������
            try
            {
                ReleaseNormalDot(obj);
                ReflectionHelper.CallMethod(obj, "Quit",
                    false,			// SaveChanges: OleVariant; 
                    Missing.Value,	// OriginalFormat: OleVariant; 
                    Missing.Value	// RouteDocument: OleVariant
                );
            }
            catch 
            { 

            }
            base.CloseOfficeObj(ref obj);
        }

        public static object GetDocuments(object appObj)
        {
            return ReflectionHelper.GetProperty(appObj, "Documents");
        }

        [Obsolete]
        public static void CloseDocuments(object appObj)
        {
            object documents = GetDocuments(appObj);
            ReleaseNormalDot(appObj);
            try
            {
                bool oldVisible = (bool)ReflectionHelper.GetProperty(appObj, "Visible");
                ReflectionHelper.CallMethod(documents, "Close",
                    false,         // SaveChanges
                    Missing.Value, // OriginalFormat
                    Missing.Value  // RouteDocument
                );
                bool newVisible = (bool)ReflectionHelper.GetProperty(appObj, "Visible");
                if (oldVisible != newVisible)
                    ReflectionHelper.SetProperty(appObj, "Visible", oldVisible);
            }
            catch
            {
            }
            Marshal.ReleaseComObject(documents);
        }

        public void CloseFirstDocument(object appObj)
        {
            object firstDocument = GetFirstDocument(appObj);
            ReleaseNormalDot(appObj);
            if (firstDocument != null)
            {
                ReflectionHelper.CallMethod(firstDocument, "Close", false, Missing.Value, Missing.Value);
                Marshal.ReleaseComObject(firstDocument);
            }

        }
        
        public object GetFirstDocument(object appObj)
        {
            object documents = GetDocuments(appObj);
            return ReflectionHelper.CallMethod(documents, "Item", 1);
        }
        
        public override void SaveChanges(object appObj, object docObj, string fileName)
        {
            object document = docObj ?? GetFirstDocument(appObj);
            object oldDisplayAlerts = ReflectionHelper.GetProperty(appObj, "DisplayAlerts");
            SetDisplayAlert(appObj, false);
            ReflectionHelper.CallMethod(document, "SaveAs2000",
                fileName,      // FileName
                wdFormatDocument,
                //Missing.Value, // FileFormat
                Missing.Value, // LockComments
                Missing.Value, // Password
                Missing.Value, // AddToRecentFiles
                Missing.Value, // WritePassword
                Missing.Value, // ReadOnlyRecommended
                Missing.Value, // EmbedTrueTypeFonts
                Missing.Value, // SaveNativePictureFormat
                Missing.Value, // SaveFormsData
                Missing.Value  // SaveAsAOCELetter
            );
            ReflectionHelper.SetProperty(appObj, "DisplayAlerts", oldDisplayAlerts);
        }

        public override int GetVersionNumber()
        {
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\{000209FF-0000-0000-C000-000000000046}\\ProgID");
            if (rk != null)
            {
                string[] keyValue = rk.GetValue("").ToString().Split('.');
                rk.Close();
                return Convert.ToInt32(keyValue[2]);
            }
            return 0;
        }

        public override string GetExtension()
        {
            if (GetVersionNumber() < 12)
                return OfficeFileExt.ofWordDocument;

            return OfficeFileExt.ofWordDocumentNew;
        }

        protected override void ActivateDocument(object appObj)
        {
            ReflectionHelper.CallMethod(appObj, "Activate");
        }
    }

}
