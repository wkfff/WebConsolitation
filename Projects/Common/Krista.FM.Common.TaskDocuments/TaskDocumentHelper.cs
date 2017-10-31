using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Krista.FM.Common.FileUtils;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters;
using Krista.FM.ServerLibrary;
using zlib;

namespace Krista.FM.Common.TaskDocuments
{
    /// <summary>
    /// Вспомогательный класс для работы с докупентами плагина в контексте задачи.
    /// </summary>
    public class TaskDocumentHelper
    {
        public void OpenDocumentFromTask(
            string originalFileName, bool showDocument,
            bool isConnectToTask, bool taskInEdit,
            IScheme scheme, string taskName,
            string taskId, string documentName,
            string documentId, string doerName,
            int docType, ITaskContext taskContext,
            OfficeMessageDelegate messageDelegate
        )
        {
            OpenDocumentFromTask(
                originalFileName, showDocument, isConnectToTask, 
                taskInEdit, scheme, taskName, Convert.ToInt32(taskId),
                documentName, Convert.ToInt32(documentId), doerName, docType, taskContext,
                messageDelegate, null, DocumentActionType.None);
        }

        public delegate void OfficeMessageDelegate(string message);

        /// <summary>
        /// открытие документа из задачи
        /// </summary>
        public void OpenDocumentFromTask(
            string originalFileName, bool showDocument, bool isConnectToTask,
            bool taskInEdit, IScheme scheme, string taskName,
            int taskId, string documentName, int documentId,
            string doerName, int docType, ITaskContext taskContext,
            OfficeMessageDelegate messageDelegate,
            OfficeApplication externalOfficeApp, DocumentActionType documentActionType)
        {
            object document = null;

            // получаем состояние файла
            FileState fileState = GetFileState(originalFileName);

            // Если документ новый, то создаем его
            if (fileState == FileState.New)
            {
                OfficeApplication officeForNew = externalOfficeApp ?? OfficeHelper.GetOfficeAppForFile(originalFileName);
                try
                {
                    document = officeForNew.CreateEmptyDocument(originalFileName);
                    officeForNew.Quit();
                    if (document != null)
                        Marshal.ReleaseComObject(document);
                }
                finally
                {
                    if (externalOfficeApp == null && officeForNew != null)
                        officeForNew.Dispose();
                }
            }

            if (fileState == FileState.Occupied)
            {
                messageDelegate(
                            String.Format(
                                "Файл '{0}' уже открыт или используется другим приложением",
                                originalFileName));
                return;
            }

            SetTaskContext(originalFileName, docType, showDocument, isConnectToTask, taskInEdit,
                           scheme, taskName, taskId, documentName, documentId, doerName, taskContext,
                           documentActionType);

            // Открываем документ
            OfficeApplication office = externalOfficeApp ?? OfficeHelper.GetOfficeAppForFile(originalFileName);
            try
            {
                if (showDocument)
                {
                    // показываем документ
                    office.Visible = true;
                    // делаем его на время недоступным для действий
                    office.Deactivate();
                }

                // пытаемся загрузить существующий
                document = office.LoadFile(originalFileName, !taskInEdit);

                // если задача на редактировании, пытаемся сохранить данные 
                if (taskInEdit)
                {
                    if (fileState == FileState.Occupied)
                    {
                        messageDelegate(
                            String.Format(
                                "Файл '{0}' используется другим процессом. Файл будет открыт в режиме только для чтения.",
                                originalFileName));
                    }
                    else
                    {
                        office.SaveChanges(document, originalFileName);
                    }
                }
            }
            finally
            {
                if (!showDocument)
                {
                    ReflectionHelper.CallMethod(document, "Close", false, String.Empty, false);
                    //office.Quit();
                }
                else
                {
                    office.Activate();
                }

                // ..освобождаем ссылки COM
                if (document != null)
                    Marshal.ReleaseComObject(document);

                // Освобождаем приложение офиса
                if (externalOfficeApp == null)
                    office.Dispose();
            }
        }

        public static void SetTaskContext(string originalFileName, int docType, 
            bool showDocument, bool isConnectToTask, bool taskInEdit, 
            IScheme scheme, string taskName, int taskId, string documentName, int documentId, 
            string doerName, ITaskContext taskContext,
            DocumentActionType documentActionType)
        {
            // это лист планирования?
            bool isPlanningSheet = IsPlanningDocument((TaskDocumentType)docType);

            // передаем основные параметры листу
            if (isPlanningSheet)
            {
                // Передаем контекст задачи в документ
                using (OfficeCustomPropertiesAdapter properties = OfficeCustomPropertiesFactory.Create(originalFileName))
                {
                    // Удаляем все свойства документа
                    properties.Clear();

                    // Устанавливаем основные свойства контекста
                    SetTaskContextProperties(originalFileName, properties, showDocument, 
                        isConnectToTask, taskInEdit, scheme, taskName, 
                        taskId, documentName, documentId, doerName, docType);

                    // Устанавливаем параметры операций
                    SetActions(properties, taskId, documentId, documentActionType);

                    // Устанавливаем параметры и константы задачи
                    SetTaskContextParameters(taskContext, properties);
                    
                    properties.Save();
                }
            }
        }

        private static void SetActions(OfficeCustomPropertiesAdapter properties, int taskId, int documentId, DocumentActionType documentActionType)
        {
            if (documentActionType != DocumentActionType.None)
            {
                properties.SetProperty("fm.tc.Action", (int)documentActionType);
            }
        }

        private static void SetTaskContextProperties(
            string originalFileName,
            OfficeCustomPropertiesAdapter properties,
            bool showDocument,
            bool isConnectToTask,
            bool taskInEdit,
            IScheme scheme,
            string taskName,
            int taskId,
            string documentName,
            int documentId,
            string doerName,
            int docType
        )
        {
            try
            {
                properties.SetProperty(FMOfficeAddinConsts.pspDocumentName, documentName);
                properties.SetProperty(FMOfficeAddinConsts.pspDocumentId, documentId);
                properties.SetProperty(FMOfficeAddinConsts.pspTaskName, taskName);
                properties.SetProperty(FMOfficeAddinConsts.pspTaskId, taskId);
                properties.SetProperty(FMOfficeAddinConsts.pspOwner, doerName);
                properties.SetProperty(FMOfficeAddinConsts.pspDocPath, originalFileName);
                if (docType != -1)
                    properties.SetProperty(FMOfficeAddinConsts.pspSheetType, docType.ToString());

                string serverPort = scheme.Server.GetConfigurationParameter("ServerPort");
                string webServiceUrl = scheme.Server.GetConfigurationParameter("WebServiceUrl");
                string serverIp = scheme.Server.GetConfigurationParameter("ServerIP");
                if (string.IsNullOrEmpty(serverIp))
                    serverIp = scheme.Server.Machine;

                properties.SetProperty("fm.ConnectionStr", String.Format("{0}:{1}", serverIp, serverPort));
                properties.SetProperty("fm.AlterConnection", webServiceUrl);
                        
                properties.SetProperty("fm.SchemeName", scheme.Name);


                properties.SetProperty("fm.tc.LoadingFromTask", true);
                properties.SetProperty("fm.tc.SilentMode", !showDocument);

                // если документ открыт в режиме редактирования - устанавливаем параметры листа    
                if (taskInEdit)
                {
                    // если это присоединение к задаче - устанавливаем дополнительные флаги
                    if (isConnectToTask)
                    {
                        properties.SetProperty("fm.tc.IsTaskConnect", true);
                    }
                }

                int authType;
                string login;
                string pwdHash;
                ClientSession.GetAuthenticationInfo(scheme, out authType, out login, out pwdHash);
                properties.SetProperty("fm.tc.AuthType", authType);
                properties.SetProperty("fm.tc.Login", login);
                properties.SetProperty("fm.tc.PwdHash", pwdHash);

                properties.SetProperty("fm.tc.ContextType", 0);
            }
            catch(Exception e)
            {
                throw new Exception(String.Format("Ошибка установки контекста: {0}", e.Message), e);
            }
        }

        private static void SetTaskContextParameters(ITaskContext taskContext, OfficeCustomPropertiesAdapter properties)
        {
            DataSet ds = new DataSet("TaskContext");

            taskContext.GetTaskConsts().ReloadItemsTable();
            DataTable dt = taskContext.GetTaskConsts().ItemsTable;
            dt.TableName = "Constant";
            dt.Columns.Remove("PARAMTYPE"); 
            ds.Tables.Add(dt);

            taskContext.GetTaskParams().ReloadItemsTable();
            dt = taskContext.GetTaskParams().ItemsTable;
            dt.TableName = "Parameter";
            dt.Columns.Remove("PARAMTYPE");
            ds.Tables.Add(dt);

            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(stream, Encoding.Default);
                ds.WriteXml(sw);
                
                using (MemoryStream compressedStream = new MemoryStream())
                { 
                    ZOutputStream packer = new ZOutputStream(compressedStream, zlibConst.Z_DEFAULT_COMPRESSION);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.WriteTo(packer);
                    packer.finish();

                    compressedStream.Position = 0;
                    properties.WriteData(FMOfficeAddinConsts.pspTaskContextData, compressedStream);

                    packer.Close();
                    compressedStream.Close();
                }
                sw.Close();
            }
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

        
        public enum FileState
        {
            New,
            Occupied,
            Vacant
        };

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
            string Path { [return: MarshalAs(UnmanagedType.BStr)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x123)] get; }
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
        public static void RestoreAddIns(OfficeApplication appObj)
        {
            // для гарантированной выгрузки Excel декларируем все используемые интерфейсы заранее
            AddIns addIns = null;
            AddIn addIn = null;
            object xla = null;
            bool originScreenUpdating = appObj.ScreenUpdating;

            try
            {
                // получаем коллекцию  аддинов
                addIns = ReflectionHelper.GetProperty(appObj.OfficeApp, "AddIns") as AddIns;
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
                        appObj.ScreenUpdating = false;
                        // формируем имя XLA файла, где содержатся инициализирующие макросы
                        string xlaFileName = addIn.Path + "\\" + ANALYS_XLA_NAME;
                        // загружаем его
                        xla = appObj.LoadFile(xlaFileName, true);
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
                appObj.ScreenUpdating = originScreenUpdating;
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
    }

    public enum DocumentActionType
    {
        /// <summary>
        /// Не выполнять ни каких действий.
        /// </summary>
        None = 0,

        /// <summary>
        /// Обновить книгу.
        /// </summary>
        Refresh = 1,

        /// <summary>
        /// Выполнить обратную запись.
        /// </summary>
        WriteBack = 2,

        /// <summary>
        /// При обратной записи записывать пустые ячейки.
        /// </summary>
        Rewrite = 4,

        /// <summary>
        /// После обратной записи выполнить расчет кубов.
        /// </summary>
        ProcessCube = 8,

        RewriteAndProcess = Rewrite | ProcessCube,
        
        RefreshAfter = 16,
        RefreshWritebackRefresh = Refresh | WriteBack | RefreshAfter
    }
}
