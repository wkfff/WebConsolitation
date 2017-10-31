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
/// Вспомогательные классы для работы с объектами MS Office
/// Для обеспечения независимости от версии MS Office используется позднее связывание
/// Должно работать с OfficeXP и выше
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
    /// класс для работы с офисной надстройкой
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
            // проверяем зарегистрированность и доступность в качестве плагина Excel
            // ..в HKLM
            if (CheckPlaginInApplication(true))
                return true;
            if (!_isPluginInstalled && CheckPlaginInApplication(false))
                return true;
            return false;
        }

        protected OfficeHelper officeHelper;

        /// <summary>
        /// Проверка зарегистрированности плагина в MS Office
        /// </summary>
        /// <param name="forAllUsers">где проверять в HKLM или в HKCU</param>
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
        /// Получить обменный интерфейс нашего плагина
        /// </summary>
        internal IFMPlanningExtension GetFMPlanningExtensionInterface(object appObj)
        {
            if (!IsPluginInstalled)
            {
                return null;//throw new Exception("Надстройка для MS Office (Криста) не установлена");
            }

            // в случае криво установленного офиса могут возникать исключения доступа 
            IFMPlanningExtension extension = null;
            // глушим их безжалостно
            try
            {
                // получаем коллекцию  аддинов
                object comAddIns = ReflectionHelper.GetProperty(appObj, "COMAddIns");
                // получаем количество установленных аддинов
                int comAddInsCount = Convert.ToInt32(ReflectionHelper.GetProperty(comAddIns, "Count"));
                // ищем наш плагин
                object comAddIn = null;
                for (int i = 1; i <= comAddInsCount; i++)
                {
                    comAddIn = ReflectionHelper.CallMethod(comAddIns, "Item", i);
                    string objName = Convert.ToString(ReflectionHelper.GetProperty(comAddIn, "ProgID"));
                    if (string.Compare(FMAddinProgID, objName, true) == 0)
                        break;
                    comAddIn = null;
                }
                // если нашли - получаем наш обменный интерфейс
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
            // загружаем ексел, открываем файл
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
        /// Является-ли документ документом плагина
        /// </summary>
        /// <param name="dt">тип документа</param>
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
                // получаем интерфейс плагина
                extension = externalPlaginItf ?? GetFMPlanningExtensionInterface(appObj);
                // если это присоединение к задаче - сбасываем старые настройки и устанавливаем 
                // необходимые флаги
                if (extension != null)
                {
                    extension.IsLoadingFromTask = true;
                    extension.IsSilentMode = !showDocument;
                }

                if (showDocument)
                    officeHelper.RestoreAddIns(appObj);

                // заказывали подключение указывать всегда
                if (extension != null)
                {
                    // путь к файлу передается и в реадонли режиме тоже
                    if (!taskInEdit)
                        extension.SetPropValueByName(FMOfficeAddinsConsts.pspDocPath, originalFileName);
                    SetPlanningSettings(extension, taskName, taskID, documentName, documentID,
                            doerName, docType, originalFileName);
                    // если документ открыт в режиме редактирования - устанавливаем параметры листа    
                    if (taskInEdit)
                    {                        
                        // если это присоединение к задаче - устанавливаем дополнительные флаги
                        if (isConnectToTask)
                            extension.OnTaskConnection(true);
                    }

                    // контекст задачи тоже устанавливается всегда
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

                    // глушим на всякий случай исключение
                    // подключение устанавливаем только после того, как передали параметры аутентификации
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
                // интерфейс плагина освобождаем если сами его запрашивали
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
    /// Базовый абстрактный класс для работы с объектами MS Office
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

        // список proxy загруженных объектов, некоторые могут быть невалидны (закрыты пользователем)
        protected ArrayList objectsList = new ArrayList();

        // Статический конструктор.
        // Определяем доступность плагина
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
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    // пытаемся закрыть объекты
                    foreach (object obj in objectsList)
                    {
                        object tmpObj = obj;
                        CloseOfficeObj(ref tmpObj);
                    }
                    // очищаем список
                    objectsList.Clear();
                    // Вызываем сборщик мусора для немедленной очистки памяти
                    GC.GetTotalMemory(true);
                }
            }
        }

        /// <summary>
        /// Текущий Locale ID - требуется некоторым функциям в качестве параметра
        /// </summary>
        public static int CurrentLCID
        {
            get { return System.Globalization.CultureInfo.CurrentCulture.LCID; }
        }

        /// <summary>
        /// Абстрактный метод получения ProgID объекта MS Office. Используется для
        /// создания объектов. Должен быть реализован в потомках.
        /// </summary>
        /// <returns>ProgID объекта MS Office, например для Excel это будет "Excel.Application"</returns>
        protected abstract string GetProgID();

        /// <summary>
        /// Абстрактный метод получения расширения файлов для объекта MS Office.
        /// Используется для проверки корректности передаваемых в параметрах имен файлов.
        /// Должен быть реализован в потомках.
        /// </summary>
        /// <returns>Расширение файла, например для Excel это будет ".xls"</returns>
        [System.Obsolete("С учетом 2007 офиса расширение имени файла определено неоднозначно")]
        public abstract string GetFileExt();

        protected bool isConnected = false;

        protected virtual void CustomizeOfficeObject(object appObj)
        {
        }

        /// <summary>
        /// Освобождение COM-ссылки на объект MS Ofiice
        /// </summary>
        /// <param name="obj">объект MS Ofiice</param>
        public static void ReleaseComReference(ref object obj)
        {
            try
            {
                // пытаемся уничтожить COM-объект
                if (Marshal.IsComObject(obj))
                    Marshal.ReleaseComObject(obj);
                // Всякие Excel'ы не всегда сразу удаляют свои объект
                // Поэтому хотя бы уничтожаем все ссылки в памяти .NET Framework
                // Это тоже не всегда помогает...
            }
            catch 
            { 

            }
            obj = null;
            GC.GetTotalMemory(true);
        }

        #region работа с плагинами

        /// <summary>
        /// Получить обменный интерфейс нашего плагина
        /// </summary>
        /// <returns>обменный интерфейс</returns>
        public IFMPlanningExtension GetFMPlanningExtensionInterface(object appObj)
        {
            return PluginHelper.GetFMPlanningExtensionInterface(appObj);
        }

        /// <summary>
        /// Признак доступности плагина
        /// </summary>
        public bool PlaginInstalled
        {
            get
            {
                return PluginHelper.IsPluginInstalled;
            }
        }

        /// <summary>
        /// Получение ProgID нашего плагина с которым работает данный тип хэлпера
        /// </summary>
        /// <returns>ProgID</returns>
        public abstract string GetFMAddinProgID();

        /// <summary>
        /// Путь к установленному плагину в реесте. Используется для быстрой проверки доступности плагина.
        /// </summary>
        /// <returns>Путь в реестре</returns>
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
        /// Является-ли документ документом плагина
        /// </summary>
        /// <param name="dt">тип документа</param>
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

        #region методы, импортированные из внешних COM библиотек

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

        #region Работа с офисным приложением

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
        /// Создать объект MS Office
        /// </summary>
        /// <param name="progID">Prog ID объекта</param>
        /// <returns>объект MS Office</returns>
        private static object GetOfficeObject(string progID, bool connectToExisting, bool forceCreate,
            out bool isConnected)
        {
            object obj = null;
            // если нужно подключится к существующему объекту - ищем его в ROT
            isConnected = false;
            if (connectToExisting)
            {
                try
                {
                    // найти способ узнать, запущено ли приложение
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
            // если не получилось - создаем новый экземпляр
            // получаем тип объекта
            Type objectType = Type.GetTypeFromProgID(progID);

            // если не удалось - генерируем исключение об ошибке
            if (objectType == null)
                throw new Exception("Невозможно создать объект " + progID);
            // создаем объект MS Office
            obj = Activator.CreateInstance(objectType);
            // возвращаем указатель для дальнейшей работы
            return obj;
        }

        /// <summary>
        /// Создание объекта MS Office
        /// </summary>
        /// <returns></returns>
        public virtual object GetOfficeObject()
        {
            object appObj = GetOfficeObject(GetProgID(), _connectToExistingObject, true, out isConnected);
            CustomizeOfficeObject(appObj);
            return appObj;
        }

        /// <summary>
        /// Запретить/разрешить обновление окна
        /// </summary>
        /// <param name="obj">объект MS Office</param>
        /// <param name="updating">значение</param>
        public static void SetScreenUpdating(object obj, bool updating)
        {
            ReflectionHelper.SetProperty(obj, "ScreenUpdating", updating);
        }

        /// <summary>
        /// Закрыть объект MS Office. Уничтожает proxy com-объекта, специфические процедуры
        /// закрытия (как правило Quit c разными наборами параметров должны вызываться в потомках)
        /// </summary>
        /// <param name="obj">Указатель на объект MS Office</param>
        public virtual void CloseOfficeObj(ref object obj)
        {
            // если указатель невалиден - выходим
            if (obj == null)
                return;
            // определяем индекс объекта 
            int index = objectsList.IndexOf(obj);
            if (index != -1)
                // обнуляем указатель во внутреннем списке
                objectsList[index] = null;
            ReleaseComReference(ref obj);
        }

        /// <summary>
        /// Показать/скрыть объект MS Office
        /// </summary>
        /// <param name="obj">объект MS Office</param>
        /// <param name="visible">признак видимости</param>
        public static void SetObjectVisible(object obj, bool visible)
        {
            ReflectionHelper.SetProperty(obj, "Visible", visible);
        }

        public abstract void SetDisplayAlert(object obj, bool displayAlert);

        /// <summary>
        /// получаем экземпляр офисного приложения. Текущий или создаем новый
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
        /// получаем экземпляр объекта офисного приложения
        /// </summary>
        /// <param name="appObj"></param>
        /// <param name="showDocument"></param>
        private void GetAppObj(ref object appObj, bool showDocument)
        {
            if (appObj == null)
            {
                // если документ будте показан пользователю - коннектимся к сущесвующему приложению
                ConnectToExistingObject = showDocument;
                appObj = GetOfficeObject();

                // Если нас не устраивает версия уже запущенного офиса - создадим свой экземпляр
                if (ConnectToExistingObject)
                    CheckExistingObject(ref appObj);
            }
        }

        /// <summary>
        /// восстановление надстроек для офиса
        /// </summary>
        public abstract void RestoreAddIns(object appObj);

        protected virtual void DeactivateDocument(object appObj){}

        #endregion

        #region Работа с документами

        private enum FileState
        {
            New,
            Occupied,
            Vacant
        } ;

        /// <summary>
        /// Создать пустой документ MS Office и сохранить его по указанному адресу
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public object CreateEmptyDocument(string fileName)
        {
            object obj = GetOfficeObject();
            CreateEmptyDocument(obj, fileName);
            return obj;
        }

        /// <summary>
        /// Проверка имени файла на существование и принадлежность к объекту данного типа (по расширению).
        /// Используется для проверки параметров функций
        /// </summary>
        /// <param name="fileName">имя файла</param>
        protected void CheckFile(string fileName)
        {
            // проверяем принадлежность
            if (!CheckFileExt(fileName))
                throw new Exception("Файл '" + fileName + "' имеет неверный формат");
            // проверяем наличие файла
            if (!File.Exists(fileName))
                throw new Exception("Файл '" + fileName + "' не существует");
        }

        /// <summary>
        /// Может ли файл быть обработан хэлпером
        /// </summary>
        /// <param name="fileName">имя файла</param>
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
        /// Метод открытия документа MS Office
        /// </summary>
        /// <param name="fileName">Имя файла с документом</param>
        /// <param name="openReadOnly">признак "открыть только для чтения"</param>
        /// <param name="show">показать объект MS Office после открытия</param>
        /// <returns>указатель на объект MS Office</returns>
        /// <summary>
        /// Открыть документ Excel
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="openReadOnly">открыть только для чтения</param>
        /// <param name="show">показать Excel</param>
        /// <returns>указатель на объект Excel для дальнейшей работы</returns>
        public object OpenFile(string fileName, bool openReadOnly, bool show)
        {
            // проверяем корректность файла
            CheckFile(fileName);
            // загружаем Excel
            object appObj = GetOfficeObject();
            // загружаем файл
            //object workbook = 
            LoadFile(appObj, fileName, openReadOnly);
            // показываем Excel если необходимо
            if (show)
                SetObjectVisible(appObj, true);
            // возвращаем указатель на объект для дальнейшей работы
            return appObj;
        }

        public abstract object LoadFile(object appObj, string fileName, bool openReadOnly);


        /// <summary>
        /// Абстрактный метод проверки соответствия расширения файла типу хелпера
        /// </summary>
        /// <param name="fileName">имя файла</param>
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
        /// открытие документа из задачи
        /// </summary>
        public void OpenDocumentFromTask(
            string originalFileName, bool showDocument, bool isConnectToTask,
            bool taskInEdit, IScheme scheme, string taskName,
            string taskID, string documentName, string documentID,
            string doerName, int docType, object taskContext,
            object externalOfficeObject, IFMPlanningExtension externalPlaginItf
        )
        {
            // получаем состояние файла
            FileState fileState = GetFileState(originalFileName);
            // это лист планирования?
            TaskDocumentType dt = (TaskDocumentType)docType;
            bool isPlanningSheet = IsPlanningDocument(dt);

            object document = null;
            object appObj = externalOfficeObject;
            GetAppObj(ref appObj, showDocument);

            if (showDocument)
            {
                // показываем документ
                SetObjectVisible(appObj, true);
                // делаем его на время недоступным для действий
                DeactivateDocument(appObj);
            }

            IFMPlanningExtension extension = null;
            try
            {
                // получаем интерфейс плагина
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
                // если документ новый - создаем
                if (fileState == FileState.New)
                {
                    document = CreateEmptyDocument(appObj, originalFileName);
                }
                else
                {
                    // пытаемся загрузить существующий
                    document = LoadFile(appObj, originalFileName, !taskInEdit);
                    if (document == null)
                        return;
                }
                // передаем основные параметры листу
                if (isPlanningSheet)
                {
                    PluginHelper.SetTaskContext(originalFileName, showDocument, isConnectToTask, taskInEdit, scheme,
                                                taskName, taskID, documentName, documentID, doerName, docType,
                                                taskContext,
                                                appObj, externalPlaginItf);
                }
                // если задача на редактировании, пытаемся сохранить данные 
                if (taskInEdit)
                {
                    if (fileState == FileState.Occupied)
                    {
                        string errStr = String.Format("Файл '{0}' используется другим процессом. Файл будет открыт в режиме только для чтения.", originalFileName);
                        MessageBox.Show(errStr, "Ошибка сохранения файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // интерфейс плагина освобождаем если сами его запрашивали
                if ((extension != null) && (externalPlaginItf == null))
                    Marshal.ReleaseComObject(extension);
                //  в зависимости от видимости объекта ексела и если это мы его создали..
                if ((appObj != null) && (externalOfficeObject == null))
                {
                    if (!showDocument)
                        // ..закрываем его совсем
                        CloseOfficeObj(ref appObj);
                    else
                    {
                        ActivateDocument(appObj);
                        // ..освобождаем ссылки COM
                        if (document != null)
                            Marshal.ReleaseComObject(document);
                        Marshal.ReleaseComObject(appObj);
                    }
                }
            }
        }

        private static string GetOffice2007CustomDocumentProperty(string fileName, string propName)
        {
            // проверяем доступность файла
            string curFileName = fileName;
            if (!File.Exists(fileName))
                new FileNotFoundException(String.Format("Файл '{0}' не найден", fileName));

            // позволят ли нам открыть файл в эксклюзивном режиме?
            bool needClone = !FileHelper.FileAllowExclusiveAccess(curFileName);
            if (needClone)
            {
                // если нет - временно копируем файл во временную директорию
                string newFileName = Path.GetTempPath() + Path.GetRandomFileName() + Path.GetExtension(curFileName);
                //string newFileName = Path.GetTempPath() + Guid.NewGuid().ToString() + Path.GetExtension(curFileName);
                byte[] fileData = FileHelper.ReadFileData(fileName);
                FileStream fs = new FileStream(newFileName, FileMode.CreateNew);
                fs.Write(fileData, 0, fileData.Length);
                fs.Close();
                curFileName = newFileName;
            }

            // файлы нового офиса представляют собой зип-архивы
            // интересующие нас пользовательские свойства хранятся в файле "docProps/custom.xml"
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
                // если файл был скопирован во временный каталог - удаляем его
                if ((needClone) && File.Exists(curFileName))
                {
                    File.Delete(curFileName);
                }
            }
        }

        /// <summary>
        /// проверяем доступность файла
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        private static FileState GetFileState(string originalFileName)
        {
            // это новый файл?
            bool fileIsNew = !File.Exists(originalFileName);
            if (fileIsNew)
                return FileState.New;
            // файл не занят другими процессами?
            bool fileIsVacant = FileHelper.FileIsVacant(originalFileName);
            if (fileIsVacant)
                return FileState.Vacant;
            return FileState.Occupied;
        }

        protected abstract void ActivateDocument(object appObj);

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public abstract void SaveChanges(object appObj, object docObj, string fileName);

        #endregion

        #region абстрактные методы

        public abstract string GetExtension();

        /// <summary>
        /// Достает из реестра ProgId и возвращает цифру версии
        /// </summary>
        public abstract int GetVersionNumber();

        #endregion
    }

    /// <summary>
    /// Класс для работы с MS Excel
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
        /// Закрытие объекта Excel
        /// </summary>
        /// <param name="obj"></param>
        public override void CloseOfficeObj(ref object obj)
        {
            if (obj == null)
                return;
            // если Excel еще не закрыт - пытаемся закрыть
            try
            {
                // отключаем предупреждения 
                SetDisplayAlert(obj, false);
                // вызываем метод Quit
                ReflectionHelper.CallMethod(obj, "Quit");
            }
            catch 
            { 

            }
            base.CloseOfficeObj(ref obj);
        }

        /// <summary>
        /// Отключение различных предупреждений
        /// </summary>
        /// <param name="obj">объект Excel</param>
        /// <param name="displayAlert">включить/выключить предупрежения</param>
        public override void SetDisplayAlert(object obj, bool displayAlert)
        {
            ReflectionHelper.SetProperty(obj, "DisplayAlerts", displayAlert);
        }

        /// <summary>
        /// Расширение для файлов Excel
        /// </summary>
        /// <returns>расширение</returns>
        [System.Obsolete("С учетом 2007 офиса расширение имени файла определено неоднозначно")]
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
        /// получение коллекции Workbooks
        /// </summary>
        /// <param name="applicationObject">объект Excel</param>
        /// <returns>коллекция Workbooks</returns>
        public static object GetWorkbooks(object applicationObject)
        {
            return ReflectionHelper.GetProperty(applicationObject, "Workbooks");
        }

        #region Восстановление функциональности плагинов

        #region Константы
        // название XLL файла надстройки "Анализ данных"
        private const string ANALYS_XLL_NAME = "ANALYS32.XLL";
        // название XLA файла надстройки "Анализ данных"
        private const string ANALYS_XLA_NAME = "atpvbaen.xla";
        #endregion

        #region Импортированные интерфейсы
        // кусок интерфеса AddIn, нужный для проверки наличия надстроек
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

        // Кусок интерфейса AddIns, нужный для проверки наличия плагинов
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
        /// Восстановить функиональность AddIn'ов, в частности надстройки "Анализ данных"
        /// </summary>
        /// <param name="appObj">Объект приложения Excel</param>
        public override void RestoreAddIns(object appObj)
        {
            // если подключились к ранее созданому экземпляру - ничего делать не нужно
            if (isConnected)
                return;

            // для гарантированной выгрузки Excel декларируем все используемые интерфейсы заранее
            AddIns addIns = null;
            AddIn addIn = null;
            object xla = null;
            try
            {
                // получаем коллекцию  аддинов
                addIns = ReflectionHelper.GetProperty(appObj, "AddIns") as AddIns;
                int cnt = addIns.Count;
                for (int i = 1; i <= cnt; i++)
                {
                    addIn = addIns[i];
                    string addInName = addIn.Name;
                    // ищем надстройку "Анализ данных"
                    // .. если она инстллирована
                    if ((String.Compare(ANALYS_XLL_NAME, addInName, true) == 0) 
                        // .. и доступна в Excel, загружаемом обычным образом
                        && (addIn.Installed))
                    {
                        // запрещаем перерисовку экрана
                        ReflectionHelper.SetProperty(appObj, "ScreenUpdating", false);
                        // формируем имя XLA файла, где содержатся инициализирующие макросы
                        string xlaFileName = addIn.Path + "\\" + ANALYS_XLA_NAME;
                        // загружаем его
                        xla = this.LoadFile(appObj, xlaFileName, true);
                        // выполняем макросы
                        ReflectionHelper.CallMethod(xla, "RunAutoMacros", 1);
                        // закрываем книгу
                        ReflectionHelper.CallMethod(xla, "Close");
                        return;
                    }
                    else
                        // это не наша надстройка или инициализировать ее не нужно - освобождаем интерфейс
                        Marshal.ReleaseComObject(addIn);
                }
            }
            catch
            {
                // могут произойти всякие неожиданности, в этом случае надстройка "Пакет Анализа" загружена не будет
            }
            finally
            {
                // разрешаем перерисовку экрана
                ReflectionHelper.SetProperty(appObj, "ScreenUpdating", true);
                // освобождаем все интерфейсы
                if (xla != null)
                    Marshal.ReleaseComObject(xla);
                if (addIn != null)
                    Marshal.ReleaseComObject(addIn);
                if (addIns != null)
                    Marshal.ReleaseComObject(addIns);
                // инициируем принудительную сборку мусора
                GC.GetTotalMemory(true);
            }
        }
        #endregion


        /// <summary>
        /// Загрузить документ
        /// </summary>
        /// <param name="appObj">объект Excel</param>
        /// <param name="fileName">имя файла документа</param>
        /// <param name="openReadOnly">открыть только для чтения</param>
        public override object LoadFile(object appObj, string fileName, bool openReadOnly)
        {
            try
            {
                // получаем коллекцию Workbooks
                object workbooksObj = GetWorkbooks(appObj);
                // Пробуем открыть книгу
                return ReflectionHelper.CallMethod(workbooksObj, "Open",
                  fileName, Type.Missing, openReadOnly, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing);
            }
            catch
            {
                string errStr = String.Format("Невозможно открыть файл '{0}'. Возможно он поврежден или используется другим процессом.", fileName);
                MessageBox.Show(errStr, "Ошибка открытия файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        private void InternalSaveChanges(object workbook, string fileName)
        {
            // сохраняем в файл
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
                        "Невозможно сохранить изменения в документе {0}. " + 
                        "Возможно файл занят другим процессом или имеет некорректное наименование.",
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
        /// Закрыть коллекцию книг
        /// </summary>
        /// <param name="excelObj">объект Excel</param>
        public static void CloseWorkBooks(object excelObj)
        {
            object workbooks = GetWorkbooks(excelObj);
            ReflectionHelper.CallMethod(workbooks, "Close");
            Marshal.ReleaseComObject(workbooks);
        }

        public override object CreateEmptyDocument(object excelObject, string fileName)
        {
            // добавляем пустую книгу
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
    /// Класс для работы с MS Word
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
        /// Отключение различных предупреждений
        /// </summary>
        /// <param name="obj">объект Excel</param>
        /// <param name="displayAlert">включить/выключить предупрежения</param>
        public override void SetDisplayAlert(object obj, bool displayAlert)
        {
            uint alertsLevel = displayAlert ? 0xFFFFFFFF : 0;
            ReflectionHelper.SetProperty(obj, "DisplayAlerts", alertsLevel);
        }

        /// <summary>
        /// Расширение для файлов MS Word (".doc")
        /// </summary>
        /// <returns>расширение</returns>
        [System.Obsolete("С учетом 2007 офиса расширение имени файла определено неоднозначно")]
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
                string errStr = String.Format("Невозможно открыть файл '{0}'. Возможно он поврежден или используется другим процессом.", fileName);
                MessageBox.Show(errStr, "Ошибка открытия файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        public static void ReleaseNormalDot(object appObj)
        {
            // освобождаем нормал дот
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
        /// Создать пустой документ MS Word и сохранить его по указанному адресу
        /// </summary>
        public override object CreateEmptyDocument(object appObj, string fileName)
        {
            // получаем коллекцию Documents
            object documents = ReflectionHelper.GetProperty(appObj, "Documents");
            // добавляем новый пустой документ
            object document = ReflectionHelper.CallMethod(documents, "Add",
                Missing.Value,	// var Template: OleVariant; 
                Missing.Value,	// var NewTemplate: OleVariant; 
                0,              //var DocumentType: OleVariant = wdNewBlankDocument 
                true			// var Visible: OleVariant
            );
            // сохраняем
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
            // показываем офис
            SetObjectVisible(appObj, true);
            ReleaseNormalDot(appObj);
            return document;
        }

        /// <summary>
        /// Закрыть объект MS Word
        /// </summary>
        /// <param name="obj"></param>
        public override void CloseOfficeObj(ref object obj)
        {
            if (obj == null)
                return;
            // если Word еще не закрыт - пытаемся закрыть
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
