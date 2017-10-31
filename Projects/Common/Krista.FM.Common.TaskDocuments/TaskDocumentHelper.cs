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
    /// ��������������� ����� ��� ������ � ����������� ������� � ��������� ������.
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
        /// �������� ��������� �� ������
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

            // �������� ��������� �����
            FileState fileState = GetFileState(originalFileName);

            // ���� �������� �����, �� ������� ���
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
                                "���� '{0}' ��� ������ ��� ������������ ������ �����������",
                                originalFileName));
                return;
            }

            SetTaskContext(originalFileName, docType, showDocument, isConnectToTask, taskInEdit,
                           scheme, taskName, taskId, documentName, documentId, doerName, taskContext,
                           documentActionType);

            // ��������� ��������
            OfficeApplication office = externalOfficeApp ?? OfficeHelper.GetOfficeAppForFile(originalFileName);
            try
            {
                if (showDocument)
                {
                    // ���������� ��������
                    office.Visible = true;
                    // ������ ��� �� ����� ����������� ��� ��������
                    office.Deactivate();
                }

                // �������� ��������� ������������
                document = office.LoadFile(originalFileName, !taskInEdit);

                // ���� ������ �� ��������������, �������� ��������� ������ 
                if (taskInEdit)
                {
                    if (fileState == FileState.Occupied)
                    {
                        messageDelegate(
                            String.Format(
                                "���� '{0}' ������������ ������ ���������. ���� ����� ������ � ������ ������ ��� ������.",
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

                // ..����������� ������ COM
                if (document != null)
                    Marshal.ReleaseComObject(document);

                // ����������� ���������� �����
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
            // ��� ���� ������������?
            bool isPlanningSheet = IsPlanningDocument((TaskDocumentType)docType);

            // �������� �������� ��������� �����
            if (isPlanningSheet)
            {
                // �������� �������� ������ � ��������
                using (OfficeCustomPropertiesAdapter properties = OfficeCustomPropertiesFactory.Create(originalFileName))
                {
                    // ������� ��� �������� ���������
                    properties.Clear();

                    // ������������� �������� �������� ���������
                    SetTaskContextProperties(originalFileName, properties, showDocument, 
                        isConnectToTask, taskInEdit, scheme, taskName, 
                        taskId, documentName, documentId, doerName, docType);

                    // ������������� ��������� ��������
                    SetActions(properties, taskId, documentId, documentActionType);

                    // ������������� ��������� � ��������� ������
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

                // ���� �������� ������ � ������ �������������� - ������������� ��������� �����    
                if (taskInEdit)
                {
                    // ���� ��� ������������� � ������ - ������������� �������������� �����
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
                throw new Exception(String.Format("������ ��������� ���������: {0}", e.Message), e);
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

        
        public enum FileState
        {
            New,
            Occupied,
            Vacant
        };

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
            string Path { [return: MarshalAs(UnmanagedType.BStr)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x123)] get; }
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
        public static void RestoreAddIns(OfficeApplication appObj)
        {
            // ��� ��������������� �������� Excel ����������� ��� ������������ ���������� �������
            AddIns addIns = null;
            AddIn addIn = null;
            object xla = null;
            bool originScreenUpdating = appObj.ScreenUpdating;

            try
            {
                // �������� ���������  �������
                addIns = ReflectionHelper.GetProperty(appObj.OfficeApp, "AddIns") as AddIns;
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
                        appObj.ScreenUpdating = false;
                        // ��������� ��� XLA �����, ��� ���������� ���������������� �������
                        string xlaFileName = addIn.Path + "\\" + ANALYS_XLA_NAME;
                        // ��������� ���
                        xla = appObj.LoadFile(xlaFileName, true);
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
                appObj.ScreenUpdating = originScreenUpdating;
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
    }

    public enum DocumentActionType
    {
        /// <summary>
        /// �� ��������� �� ����� ��������.
        /// </summary>
        None = 0,

        /// <summary>
        /// �������� �����.
        /// </summary>
        Refresh = 1,

        /// <summary>
        /// ��������� �������� ������.
        /// </summary>
        WriteBack = 2,

        /// <summary>
        /// ��� �������� ������ ���������� ������ ������.
        /// </summary>
        Rewrite = 4,

        /// <summary>
        /// ����� �������� ������ ��������� ������ �����.
        /// </summary>
        ProcessCube = 8,

        RewriteAndProcess = Rewrite | ProcessCube,
        
        RefreshAfter = 16,
        RefreshWritebackRefresh = Refresh | WriteBack | RefreshAfter
    }
}
