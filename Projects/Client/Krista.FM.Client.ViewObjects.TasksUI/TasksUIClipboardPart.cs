using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    /*
    #region Слежение за буфером обмена
    /// <summary>
    /// Provides a way to receive notifications of changes to the 
    /// content of the clipboard using the Windows API.  
    /// </summary>
    /// <remarks>
    /// To be a part of the change notification you need to register a window in the 
    /// Clipboard Viewer Chain.  This provides notification messages to the window whenever the 
    /// clipboard changes, and also messages associated with managing the chain.  This class manages 
    /// the detail of keeping the chain intact and ensuring that the application is removed
    /// from the chain at the right point.
    /// 
    /// Use the <see cref="System.Windows.Forms.NativeWindow.AssignHandle"/> method to connect this class 
    /// up to a form to begin receiving notifications.
    /// Note that a Form can change its <see cref="System.Windows.Forms.Form.Handle"/>	
    /// under certain circumstances; for example, if you change the 
    /// <see cref="System.Windows.Forms.Form.ShowInTaskbar"/> property the Framework must re-create 
    /// the form from scratch since Windows ignores changes to  that style unless they are in place 
    /// when the Window is created. (As a consequence, you should try to set as many high-level Window 
    /// style details as possible prior to creating the Window, or at least, prior to making it visible).
    /// The <see cref="OnHandleChanged"/> method of this class is called when this happens.  You need to
    /// re-assign the handle again whenever this occurs.  Unfortunately <see cref="OnHandleChanged"/> 
    /// is not a useful event in which to do anything since the window handle at that point contains neither
    /// a valid old window or a valid new one.  Therefore you need to make the call to re-assign at 
    /// a point when you know the window is valid, for example, after styles have been changed, or 
    /// by overriding <see cref="System.Windows.Forms.Form.OnHandleCreated"/>.
    /// </remarks>		
    internal class ClipboardChangeNotifier : NativeWindow, IDisposable
    {
        #region Unmanaged Code
        [DllImport("user32")]
        private extern static IntPtr SetClipboardViewer(IntPtr hWnd);
        [DllImport("user32")]
        private extern static int ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_DESTROY = 0x0002;
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        #endregion

        #region Member Variables
        /// <summary>
        /// The next handle in the clipboard viewer chain when the 
        /// clipboard notification is installed, otherwise <see cref="IntPtr.Zero"/>
        /// </summary>
        protected IntPtr nextViewerHandle = IntPtr.Zero;
        /// <summary>
        /// Whether this class has been disposed or not.
        /// </summary>
        protected bool disposed = false;
        /// <summary>
        /// The Window clipboard change notification was installed for.
        /// </summary>
        protected IntPtr installedHandle = IntPtr.Zero;
        #endregion

        #region Events
        /// <summary>
        /// Notifies of a change to the clipboard's content.
        /// </summary>
        public event EventHandler ClipboardChanged;
        #endregion
        /*
        /// <summary>
        /// Provides default WndProc processing and responds to clipboard change notifications.
        /// </summary>
        /// <param name="e"></param>
        protected override void WndProc(ref Message e)
        {
            // if the message is a clipboard change notification
            switch (e.Msg)
            {
                case WM_CHANGECBCHAIN:
                    if (e.WParam == nextViewerHandle)
                    {
                        //
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
                        //
                        nextViewerHandle = e.LParam;
                    }
                    else
                    {
                        SendMessage(nextViewerHandle, e.Msg, e.WParam, e.LParam);
                    }

                    /*
                    // Store the changed handle of the next item in the clipboard chain:
                    this.nextViewerHandle = e.LParam;
                    if (!this.nextViewerHandle.Equals(IntPtr.Zero))
                    {
                        // pass the message on:
                        SendMessage(this.nextViewerHandle, e.Msg, e.WParam, e.LParam);
                    }*//*
                    // We have processed this message:
                    e.Result = IntPtr.Zero;
                    break;
                case WM_DRAWCLIPBOARD:
                    // content of clipboard has changed:
                    EventArgs clipChange = new EventArgs();
                    OnClipboardChanged(clipChange);
                    // pass the message on:
                    if (!nextViewerHandle.Equals(IntPtr.Zero))
                    {
                        //SendMessage(nextViewerHandle, e.Msg, e.WParam, e.LParam);
                    }
                    // We have processed this message:
                    e.Result = IntPtr.Zero;
                    break;
                case WM_DESTROY:
                    // Very important: ensure we are uninstalled.
                    Uninstall();
                    // And call the superclass:
                    base.WndProc(ref e);
                    break;
                default:
                    // call the superclass implementation:
                    base.WndProc(ref e);
                    break;
            }
        }
*//*
        /// <summary>
        /// Responds to Window Handle change events and uninstalls
        /// the clipboard change notification if it is installed.
        /// </summary>
        protected override void OnHandleChange()
        {
            // If we did get to this point, and we're still installed then the chain will be broken.
            // The response to the WM_TERMINATE message should prevent this.
            Uninstall();
            base.OnHandleChange();
        }

        /// <summary>
        /// Installs clipboard change notification.  The <see cref="AssignHandle"/> method of 
        /// this class must have been called first.
        /// </summary>
        public void Install()
        {
            Uninstall();
            if (!Handle.Equals(IntPtr.Zero))
            {
                installedHandle = Handle;
                nextViewerHandle = SetClipboardViewer(Handle);
            }
        }

        /// <summary>
        /// Uninstalls clipboard change notification.
        /// </summary>
        public void Uninstall()
        {
            if (!installedHandle.Equals(IntPtr.Zero))
            {
                ChangeClipboardChain(installedHandle, nextViewerHandle);
                //int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                /*Debug.Assert(error == 0,
                    String.Format("{0} Failed to uninstall from Clipboard Chain", this),
                    Win32Error.ErrorMessage(error));*//*
                nextViewerHandle = IntPtr.Zero;
                installedHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Raises the <c>ClipboardChanged</c> event.
        /// </summary>
        /// <param name="e">Blank event arguments.</param>
        protected virtual void OnClipboardChanged(EventArgs e)
        {
            if (ClipboardChanged != null)
            {
                ClipboardChanged(this, e);
            }
        }

        /// <summary>
        /// Uninstalls clipboard event notifications if necessary during dispose of this object.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                Uninstall();
                disposed = true;
            }
        }

        /// <summary>
        /// Constructs a new instance of this class.
        /// </summary>
        public ClipboardChangeNotifier()
            : base()
        {
            // intentionally blank
        }

    }
    #endregion
    */
    public partial class TasksViewObj : BaseViewObj
    {
        #region Форматы для CLIPBOARD
        public static string CLPB_COPY_TASKS_XML = "CLPB_COPY_TASKS_XML";
        public static string CLPB_CUT_TASKS_XML = "CLPB_CUT_TASKS_XML";
        public static string CLPB_TASKS_OPERATION_COMPLETED = "CLPB_TASKS_OPERATION_COMPLETED";

        public static string CLPB_CUT_DOCUMENTS_XML = "CLPB_CUT_DOCUMENTS_XML";
        public static string CLPB_COPY_DOCUMENTS_XML = "CLPB_COPY_DOCUMENTS_XML";
        public static string CLPB_DOCUMENTS_OPERATION_COMPLETED = "CLPB_DOCUMENTS_OPERATION_COMPLETED";

        public static string CLPB_OPERATION_CANCELED = "CLPB_OPERATION_CANCELED";
        #endregion

        #region Различные текстовые сообщения
        public static string COPY_TASKS_OPERATION = "Копирование задач в буфер обмена";
        public static string COPY_DOCUMENTS_OPERATION = "Копирование документов в буфер обмена";
        public static string PASTE_TASKS_OPERATION = "Вставка задач из буфера обмена";
        public static string PASTE_DOCUMЕNТS_OPERATION = "Вставка документов из буфера обмена";
        public static string DELETE_TASKS_OPERATION = "Удаление вырезанных задач";
        public static string DELETE_DOCUMENTS_OPERATION = "Удаление вырезанных документов";

        public static string ERROR_ON_TASKS_CUTTING = "Невозможно скопировать задачи";
        public static string ERROR_ON_DOCUMENTS_CUTTING = "Невозможно скопировать документы";
        #endregion

        bool inClipboardOperation = false;

        private void ShowClipboardOperationError(bool isTasks, string errorInfo, string caption)
        {
            ClipbpardHelper.ResetCuttedDocumentsIDs(_tasksView.ugDocuments);
            ClipbpardHelper.ResetCuttedTasksIDs(_tasksNavigation.ugTasks);
            inClipboardOperation = false;
            Workplace.OperationObj.StopOperation();
            if (isTasks)
                MessageBox.Show(errorInfo, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            if ((_tasksNavigation == null) || (_tasksView == null))
                return;

            List<string> formats = FormatsPresentInClipboard();
            // документы
            bool enabled = ActiveTask != null;
            if (enabled)
            {
                enabled = ((ActiveTask.InEdit) &&
                ((formats.Contains(CLPB_COPY_DOCUMENTS_XML)) ||
                (formats.Contains(CLPB_CUT_DOCUMENTS_XML))));
            }
            ToolBase tool = _tasksView.utbDocuments.Tools["btnPasteDocuments"];
            tool.SharedProps.Enabled = enabled;           

            #region Завершение операции "Вырезать документы"
            // была ли начата операция "Вырезать документы"?
            if (ClipbpardHelper.CuttedDocumentsIDs.Count > 0)
            {
                // была ли завершена операция "Вырезать задачи"?
                if (formats.Contains(CLPB_DOCUMENTS_OPERATION_COMPLETED))
                {
                    Workplace.OperationObj.Text = DELETE_DOCUMENTS_OPERATION;
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        // удаляем документы 
                        for (int i = ClipbpardHelper.CuttedDocumentsIDs.Count - 1; i >= 0; i--)
                        {
                            // .. из базы
                            Workplace.ActiveScheme.TaskManager.Tasks.DeleteDocument(ClipbpardHelper.CuttedDocumentsIDs[i]);
                        }
                    }
                    finally
                    {
                        ClipbpardHelper.CuttedDocumentsIDs.Clear();
                        Workplace.OperationObj.StopOperation();
                    }
                }
                else
                {
                    // не испортил-ли кто-нибьудь буфер между операциями?
                    if (!formats.Contains(CLPB_CUT_DOCUMENTS_XML))
                    {
                        UltraGridHelper.SetRowsTransparent(_tasksView.ugDocuments, ClipbpardHelper.CuttedDocumentsIDs, false);
                        ClipbpardHelper.CuttedDocumentsIDs.Clear();
                    }
                }
            }
            // если есть метка о завершении операции, надо перегрузить задачу (в частности - страницу документов)
            if (formats.Contains(CLPB_DOCUMENTS_OPERATION_COMPLETED))
            {
                _documentsPageLoaded = false;
                //LoadTaskData();
                LoadDocumentsPage();
                inClipboardOperation = false;
            }
            #endregion
        }

        public static bool TasksDataPresentInClipboard(out string data)
        {
            return DataPresentInClipboard(CLPB_COPY_TASKS_XML, out data) ||
                   DataPresentInClipboard(CLPB_CUT_TASKS_XML, out data);
        }

        public static bool DocumentsDataPresentInClipboard(out string data)
        {
            return DataPresentInClipboard(CLPB_COPY_DOCUMENTS_XML, out data) || 
                   DataPresentInClipboard(CLPB_CUT_DOCUMENTS_XML, out data);
        }

        private static bool DataPresentInClipboard(string formatName, out string data)
        {
            data = String.Empty;
            IDataObject dto = Clipboard.GetDataObject();
            foreach (string fmt in dto.GetFormats())
            {
                if (fmt == formatName)
                {
                    data = dto.GetData(formatName).ToString();
                    return true;
                }
            }
            return false;
        }

        private static List<string> FormatsPresentInClipboard()
        {
            List<string> result = new List<string>();
            IDataObject dto = Clipboard.GetDataObject();
            foreach (string fmt in dto.GetFormats())
            {
                result.Add(fmt);
            }
            return result;
        }

        public void GridsKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is UltraGrid))
                return;
            UltraGrid ug = (UltraGrid)sender;
            if ((ug.Name != "ugTasks") && (ug.Name != "ugDocuments"))
                return;

            bool processTasks = (ug.Name == "ugTasks");

            if ((e.Control) && (e.KeyCode == Keys.C))
            {
                if (processTasks)
                    CopyTasksIntoClipboard(ug);
                else
                    CopyDocumentsIntoClipboard(ug);
                e.Handled = true;
            }

            if (((e.Shift) && (e.KeyCode == Keys.Delete)) ||
                ((e.Control) && (e.KeyCode == Keys.X)))
            {
                if (processTasks)
                    CutTasksIntoClipboard(ug);
                else
                    CutDocumentsIntoClipboard(ug);
                e.Handled = true;
            }

            if (((e.Shift) && (e.KeyCode == Keys.Insert)) ||
                ((e.Control) && (e.KeyCode == Keys.Insert)) ||
                ((e.Control) && (e.KeyCode == Keys.V)))
            {
                if (processTasks)
                    InsertTasksFromClipboard(ug);
                else
                    InsertDocumentsFromClipboard(/*ug*/);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Clipboard.Clear();
            }
        }

        internal void CutTasksIntoClipboard(UltraGrid tasksGrid)
        {
            PutTasksIntoClipboard(tasksGrid, true);
        }

        internal void CopyTasksIntoClipboard(UltraGrid tasksGrid)
        {
            PutTasksIntoClipboard(tasksGrid, false);
        }

        private bool CanCutTask(UltraGridRow row)
        {
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            int taskID = Convert.ToInt32(row.Cells["ID"].Value);
            int refTaskTypes = Convert.ToInt32(row.Cells["RefTasksTypes"].Value);
            // текущий пользователь может удалять задачу?
            bool canDelete = um.CheckPermissionForTask(taskID, refTaskTypes, (int)TaskTypeOperations.DelTaskAction, false);
            if (!canDelete)
				throw new Exception(String.Format("Пользователь '{0}' не обладает достаточными правами для удаления задачи ID = {1}", um.GetUserNameByID(ClientAuthentication.UserID), taskID));

            // задача свободна?
            if ((row.Cells["LockByUser"].Value == DBNull.Value) ||
                (Convert.ToInt32(row.Cells["LockByUser"].Value) == -1))
            {
                ClipbpardHelper.CuttedTasksIDs.Add(taskID);
                return true;
            }

            // задача заблокирована текущим пользователем?
            int lockByUser = Convert.ToInt32(row.Cells["LockByUser"].Value);
			if (ClientAuthentication.UserID == lockByUser)
            {
                ClipbpardHelper.CuttedTasksIDs.Add(taskID);
                return true;
            }
            
            // задача заблокирована другим пользователем?
            throw new Exception(String.Format("Задача ID = {0} заблокирована пользователем '{1}'", taskID, um.GetUserNameByID(lockByUser)));
            //return true;
        }

        private bool CanCutTasks(UltraGrid tasksGrid)
        {
            try
            {
                foreach (UltraGridRow row in tasksGrid.Selected.Rows)
                {
                    UltraGridHelper.EnumGridRows(tasksGrid, row, true, new UltraGridHelper.CheckRowDelegate(CanCutTask));
                }
                return true;
            }
            catch (Exception e)
            {
                // по каким-то причинам одна из задач не может быть вырезана
                ShowClipboardOperationError(true, e.Message, ERROR_ON_TASKS_CUTTING);
                return false;
            }
        }

        private void PutTasksIntoClipboard(UltraGrid tasksGrid, bool markAsCutted)
        {
            CheckChanges();
            ClipbpardHelper.ResetCuttedTasksIDs(tasksGrid);
            inClipboardOperation = true;

            List<int> selectedID;
            UltraGridHelper.GetSelectedIDs(tasksGrid, out selectedID);
            if (selectedID.Count == 0)
                return;

            Workplace.OperationObj.Text = COPY_TASKS_OPERATION;
            Workplace.OperationObj.StartOperation();

            try
            {
                if ((markAsCutted) && (!CanCutTasks(tasksGrid)))
                    return;

                string errStr = String.Empty;
                // получаем XML документ с экспортируемыми данными
                XmlDocument xmlData = TasksExportHelper.InnerExportTasks(Workplace, selectedID, TaskExportType.teIncludeChild, ref errStr, false);
                if (!String.IsNullOrEmpty(errStr))
                {
                    ShowClipboardOperationError(true, errStr, ERROR_ON_TASKS_CUTTING);
                    return;
                }
                // помещаем в клипбоард
                // если нужно - помечаем строки грида как выделенные
                if (markAsCutted)
                {
                    Clipboard.SetData(CLPB_CUT_TASKS_XML, xmlData.InnerXml);
                    UltraGridHelper.SetRowsTransparent(tasksGrid, ClipbpardHelper.CuttedTasksIDs, true);
                }
                else
                {
                    Clipboard.SetData(CLPB_COPY_TASKS_XML, xmlData.InnerXml);
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        internal void InsertTasksFromClipboard(UltraGrid tasksGrid)
        {
            string data;
            if (!TasksDataPresentInClipboard(out data))
                return;

            List<string> formats = FormatsPresentInClipboard();
            bool isCopy = formats.Contains(CLPB_COPY_TASKS_XML);

            inClipboardOperation = true;

            TaskImportType importType;
            int? parentID;
            if (!TasksExportHelper.ResolveImportType(Workplace, tasksGrid, out importType, out parentID))
                return;

            Workplace.OperationObj.Text = PASTE_TASKS_OPERATION;
            Workplace.OperationObj.StartOperation();
            try
            {
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(data);
                XmlNodeList tasksGroupsNodes = xmlData.SelectNodes("//tasks");
                if (parentID != null && tasksGroupsNodes.Count - tasksGrid.ActiveRow.Band.Index > TasksNavigation.MAX_TASKS_HIERARCHY_LEVEL - 1)
                {
                    Workplace.OperationObj.StopOperation();
                    MessageBox.Show("Невозможно вставить задачу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // если это копирование, подставляем к имени всех задач "(копия)"
                if (isCopy)
                {
                    //xmlData.Save(@"c:\test.xml");
                    XmlNodeList tasksNodes = xmlData.SelectNodes("//task");
                    foreach (XmlNode taskNode in tasksNodes)
                    {
                        string oldName = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskHeadlineTagName, String.Empty);
                        XmlHelper.SetAttribute(taskNode, TasksExportXmlConsts.TaskHeadlineTagName, oldName + " (копия)");
                    }
                }
                TasksExportHelper.InnerImportTasks(Workplace, xmlData, parentID, false);
                // помещаем метку о том что операция копирования завершилась
                Clipboard.SetData(CLPB_TASKS_OPERATION_COMPLETED, String.Empty);
                Application.DoEvents();
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
                inClipboardOperation = false;
            }

        }

        private void CutDocumentsIntoClipboard(UltraGrid documentsGrid)
        {
            PutDocumentsIntoClipboard(documentsGrid, true);
        }

        private void CopyDocumentsIntoClipboard(UltraGrid documentsGrid)
        {
            PutDocumentsIntoClipboard(documentsGrid, false);
        }

        private bool CanCutDocuments(UltraGrid documentsGrid)
        {
            try
            {
                // задача заблокирована текущим пользователем?
                int lockByUser = ActiveTask.LockByUser;

				if (ClientAuthentication.UserID != lockByUser)
                    throw new Exception("Задача должна находится на редактировании у текущего пользователя");

                foreach (UltraGridRow row in documentsGrid.Selected.Rows)
                {
                    ClipbpardHelper.CuttedDocumentsIDs.Add(Convert.ToInt32(row.Cells["ID"].Value));
                }
                return true;
            }
            catch (Exception e)
            {
                // по каким-то причинам один из документов не может быть вырезан
                ShowClipboardOperationError(true, e.Message, ERROR_ON_DOCUMENTS_CUTTING);
                return false;
            }
        }

        static void AppendXMLAttribute(XmlTextWriter writer, string attributeName, object attributeValue)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(Convert.ToString(attributeValue));
            writer.WriteEndAttribute();
        }

        private string GetDocumentsClipboardXml(UltraGrid documentsGrid)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = new XmlTextWriter(ms, Encoding.GetEncoding(1251));
            wr.Formatting = Formatting.Indented;
            string result;
            //wr.Settings.CheckCharacters = false;
            try
            {
                wr.WriteStartDocument(true);
                wr.WriteStartElement(TasksExportXmlConsts.DocumentsTagName);
                int taskID = ActiveTask.ID;
                foreach (UltraGridRow row in documentsGrid.Selected.Rows)
                {
                    wr.WriteStartElement(TasksExportXmlConsts.DocumentTagName);

                    int docID = Convert.ToInt32(UltraGridHelper.GetRowCells(row).Cells["ID"].Value);
                    string docName = Convert.ToString(UltraGridHelper.GetRowCells(row).Cells["Name"].Value);
                    string docSourceFileName = Convert.ToString(UltraGridHelper.GetRowCells(row).Cells["SourceFileName"].Value);
                    string docComment = Convert.ToString(UltraGridHelper.GetRowCells(row).Cells["Description"].Value);
                    string addedFileName = documentsHelper.CheckLocalTaskDocument(Workplace.OperationObj,
                        ActiveTask, taskID, docID, docName, docSourceFileName);

                    AppendXMLAttribute(wr, TasksExportXmlConsts.DocSourceFileNameTagName, addedFileName);
                    AppendXMLAttribute(wr, TasksExportXmlConsts.DocNameTagName, docName);
                    AppendXMLAttribute(wr, TasksExportXmlConsts.DocDescriptionTagName, docComment);

                    //InternalAddDocument(AddedTaskDocumentType.ndtExisting, ActiveTask.TaskProxy, addedFielName, docName, docComment, false);

                    wr.WriteEndElement();
                }
                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
            finally
            {
                wr.Flush();
                wr.Close();
                ms.Flush();
                byte[] xmlData = ms.GetBuffer();
                ms.Close();
                result = Encoding.GetEncoding(1251).GetString(xmlData);
            }
            return result;
        }

        private void PutDocumentsIntoClipboard(UltraGrid documentsGrid, bool markAsCutted)
        {
            ClipbpardHelper.ResetCuttedDocumentsIDs(_tasksView.ugDocuments); 
            inClipboardOperation = true;

            List<int> selectedDocsID;
            UltraGridHelper.GetSelectedIDs(documentsGrid, out selectedDocsID);
            if (selectedDocsID.Count == 0)
                return;

            Workplace.OperationObj.Text = COPY_DOCUMENTS_OPERATION;
            Workplace.OperationObj.StartOperation();

            try
            {
                if ((markAsCutted) && (!CanCutDocuments(documentsGrid)))
                    return;

                string errStr = String.Empty;
                // получаем XML документ с экспортируемыми данными
                string xmlData = GetDocumentsClipboardXml(documentsGrid);
                if (!String.IsNullOrEmpty(errStr))
                {
                    ShowClipboardOperationError(true, errStr, ERROR_ON_DOCUMENTS_CUTTING);
                    return;
                }
                // помещаем в клипбоард
                // если нужно - помечаем строки грида как выделенные
                if (markAsCutted)
                {
                    Clipboard.SetData(CLPB_CUT_DOCUMENTS_XML, xmlData);
                    UltraGridHelper.SetRowsTransparent(documentsGrid, ClipbpardHelper.CuttedDocumentsIDs, true);
                }
                else
                {
                    Clipboard.SetData(CLPB_COPY_DOCUMENTS_XML, xmlData);
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        private bool CanInsertDocumentsFromClipboard()
        {
            return ActiveTask.LockByCurrentUser();
        }
        
        private void InsertDocumentsFromClipboard(/*UltraGrid tasksGrid*/)
        {
            //inClipboardOperation = false;
            string data;
            if (!DocumentsDataPresentInClipboard(out data))
                return;

            if (!CanInsertDocumentsFromClipboard())
            {
                MessageBox.Show("Задача должна быть на редактировании у текущего пользователя", "Невозможно вставить документы",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> formats = FormatsPresentInClipboard();
            bool isCopy = formats.Contains(CLPB_COPY_DOCUMENTS_XML);

            inClipboardOperation = true;

            Workplace.OperationObj.Text = PASTE_DOCUMЕNТS_OPERATION;
            Workplace.OperationObj.StartOperation();
            try
            {
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(data);
                XmlNodeList documentsNodes = xmlData.SelectNodes("//" + TasksExportXmlConsts.DocumentTagName);
                foreach (XmlNode nd in documentsNodes)
                {
                    string addedFileName = XmlHelper.GetStringAttrValue(nd, TasksExportXmlConsts.DocSourceFileNameTagName, String.Empty);
                    string docName = XmlHelper.GetStringAttrValue(nd, TasksExportXmlConsts.DocNameTagName, String.Empty);
                    string docComment = XmlHelper.GetStringAttrValue(nd, TasksExportXmlConsts.DocDescriptionTagName, String.Empty);

                    // если это копирование, подставляем к имени всех задач "(копия)"
                    if (isCopy)
                        docName = docName + " (копия)";

                    // добавляем документ в задачу
                    InternalAddDocument(AddedTaskDocumentType.ndtExisting, ActiveTask, 
                        addedFileName, docName, docComment, false);
                    // пытаемся удалить старый с диска
                    if (!isCopy)
                    {
                        try
                        {
                            File.Delete(addedFileName);
                        }
                        catch { }
                    }
                }
                // фиксируем изменения в базе
                CheckChanges();
                // помещаем метку о том что операция копирования завершилась
                Clipboard.SetData(CLPB_DOCUMENTS_OPERATION_COMPLETED, String.Empty);
                // обрабатываем событие сами
                Application.DoEvents();
                // даем другим приложениям обработать
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
                inClipboardOperation = false;
            }


        }

    
    }
}