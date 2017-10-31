using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Common;
using Krista.FM.Common.MdxExpertHelpers;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.OfficePluginServices;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Common.TaskDocuments;
using Krista.FM.Common.FileUtils;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    /// <summary>
    /// Класс для манипуляций с задачами
    /// </summary>
    public partial class TasksViewObj : BaseViewObj
    {
        /// <summary>
        /// Удаление документа
        /// </summary>
        private void DelTaskDocument()
        {
            UltraGrid ug = _tasksView.ugDocuments;
            if (ug.ActiveRow != null && ug.Selected.Rows.Count == 0)
                ug.ActiveRow.Selected = true;
            // Если в гриде документов нет выделенных строк - выходим
            if ((ug.Selected.Rows == null) ||
                (ug.Selected.Rows.Count == 0))
                return;
            List<int> deletedDocs = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Выполняется удаление:");
            sb.AppendLine();
            foreach (UltraGridRow row in ug.Selected.Rows)
            {
                sb.AppendLine(UltraGridHelper.GetRowCells(row).Cells["Name"].Value.ToString());
                deletedDocs.Add(UltraGridHelper.GetRowID(row));
            }
            sb.AppendLine();
            sb.AppendLine("Продолжить?");
            if (MessageBox.Show(sb.ToString(), "Предупреждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // выходим из режима редактирования (иначе остается висеть редактор)
            ug.PerformAction(UltraGridAction.ExitEditMode);

            // удаляем файл с диска
            foreach (UltraGridRow docRow in ug.Selected.Rows)
            {
                // получаем всякие нужные параметры
                string documentName = UltraGridHelper.GetRowCells(docRow).Cells["Name"].Value.ToString();
                string sourceFileName = UltraGridHelper.GetRowCells(docRow).Cells["SourceFileName"].Value.ToString();
                int documentID = UltraGridHelper.GetRowID(docRow);
                string localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID,
                    documentID, documentName, sourceFileName);
                // пытаемся удалить с диска - не получится, ничего страшного
                try
                {
                    File.Delete(localDocumentName);
                }
                catch {}
            }
            // удаляем документ из базы и из грида
            DataTable dt = (DataTable)ug.DataSource;
            foreach (int id in deletedDocs)
            {
                DataRow row = dt.Rows.Find(id);
                if (row != null)
                    row.Delete();
            }
            deletedDocs.Clear();
        }

        private void InternalSaveTaskDocument(int documentID, string documentName, string sourceFileName, string dirName)
        {
            // получаем параметры задачи
            int taskID = ActiveTask.ID;

            string fileExt = Path.GetExtension(sourceFileName);
            string localDocumentName = documentsHelper.CheckLocalTaskDocument(Workplace.OperationObj, ActiveTask, taskID, documentID, documentName, sourceFileName);
            string newFileName;

            // если не указана директория - это одиночное сохранение файла, показываем диалог
            if (String.IsNullOrEmpty(dirName))
            {
                if (MessageBox.Show("Сохранение документа под другим именем или по другому адресу приведет к тому," + 
                    " что изменения в нем не будут сохраняться в базу данных.", 
                    "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, 
                    MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                        return;
                // показываем диалог "сохранить файл"
                _tasksView.sdSave.DefaultExt = fileExt;
                _tasksView.sdSave.FileName = Path.GetFileName(localDocumentName);
                _tasksView.sdSave.Filter = string.Format("(*{0})|*{1}", fileExt, fileExt);

                if (_tasksView.sdSave.ShowDialog() == DialogResult.OK)
                {
                    newFileName = _tasksView.sdSave.FileName;                      
                }
                else
                    return;
            }
            else
            {
                newFileName = String.Concat(dirName, "\\", Path.GetFileName(localDocumentName));
            }
            Workplace.OperationObj.Text = "Сохранение файла " + newFileName;
            // если это одиночное сохранение документа - показываем окно операции
            if (String.IsNullOrEmpty(dirName))
                Workplace.OperationObj.StartOperation();
            try
            {
                // если файл сохраняется в кэш с временными файлами, то сохранять будем через временное хранение данных файла
                if (string.Compare(localDocumentName, newFileName, true) == 0)
                {
                    byte[] fileData = FileHelper.ReadFileData(localDocumentName);
                    FileStream fs = new FileStream(localDocumentName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Write(fileData, 0, fileData.Length);
                    fs.Flush();
                    fs.Close();
                    fileData = null;
                }
                else
                    File.Copy(localDocumentName, newFileName, true);
            }
            finally
            {
                // если это одиночное сохранение документа - скрываем окно операции
                if (String.IsNullOrEmpty(dirName))
                    Workplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Сохранить документ"
        /// </summary>
        /// <param name="cell"></param>
        public void SaveTaskDocument(UltraGridCell cell)
        {
            InternalSaveTaskDocument(
                Convert.ToInt32(cell.Row.Cells["ID"].Value),
                cell.Row.Cells["Name"].Value.ToString(),
                cell.Row.Cells["SourceFileName"].Value.ToString(),
                String.Empty
            );
        }
      
        public void InitDocumentRow(int ID, int documentType, string name, string sourceFileName,
            int version, string comment)
        {
            DataTable dt = (DataTable)_tasksView.ugDocuments.DataSource;
            dt.Rows.Add(
                ID,
                documentType,
                name,
                sourceFileName,
                version,
                _activeTask.ID,
                comment,
                (int)TaskDocumentOwnership.doGeneral // первичное состояние документа - общий
            );
        }

        [DllImport("ole32.dll")]
        static extern void CoFreeUnusedLibraries();

        [DllImport("user32.dll")]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        #region Для листа версии 2.2.2

        List<int> selectedId = null;
        
        public void DoPlanningOperation(ToolBase btn)
        {
            // Установлен ли плагин?
            // Проверка на ворд убрана, чтобы не обламывать массовые операции
            // из-за устаревшей неиспользуемой надстройки или отсутствия самого ворда на машине
            if (!ExcelPluginService.PluginInstalled)
                return;
            // если таблица документов не инициализирована - выходим
            if (_tasksView.ugDocuments.DataSource == null)
                return;
            DataTable documents = (DataTable)_tasksView.ugDocuments.DataSource;
            // если таблица документов пуста - выходим
            if (documents.Rows.Count == 0)
                return;

           // EnableWindow((IntPtr)Workplace.WndHandle, false);
            ExcelApplication excel = null;
            IFMPlanningExtension excelPlanningItf = null;

            WordApplication word = null;
            IFMPlanningExtension wordPlanningItf = null;

            IProcessForm processFrm = null;

            int allProcessed = 0;
            int succProcessed = 0;
            int processedWithErrors = 0;
            int skipped = 0;
            try
            {
                // параметры обратной записи
                bool rewriteData = true;
                bool processMD = true;

                try
                {
                    Workplace.OperationObj.Text = "Обработка данных";
                    Workplace.OperationObj.StartOperation();
                    
                    excel = OfficeHelper.CreateExcelApplication();
                    excel.ScreenUpdating = false;
                    excel.Interactive = false;
                    excel.DisplayAlerts = false;

                    excelPlanningItf = ExcelPluginService.GetPlanningExtensionInterface(excel.OfficeApp);
                    excelPlanningItf.IsSilentMode = true;

                    // Если ворд не установлен, надстройки для него нет, массовая операция не должна прерваться
                    try
                    {
                        word = OfficeHelper.CreateWordApplication();
                        word.ScreenUpdating = false;
                        word.DisplayAlerts = false;

                        wordPlanningItf = WordPluginService.GetPlanningExtensionInterface(word.OfficeApp);
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
                
                processFrm = excelPlanningItf.ProcessForm;
                processFrm.NewProcessClear = false;
                //wordPlanningItf.ProcessForm = processFrm;

                DocumentActionType documentActionType = DocumentActionType.None;

                switch (btn.Key)
                {
                    case "btnRefreshAll":
                        documentActionType = DocumentActionType.Refresh;
                        processFrm.OpenProcess(Workplace.WndHandle, 
                            "Обновление документов", "Обновление завершено", "Обновление завершено с ошибкой", false);
                        //processFrm.ProcessTitle = "Обновление документов";
                        processFrm.OpenOperation("Обновление всех документов задачи", false, true, (int)Const.otProcess);
                        break;
                    case "btnRefreshSelect":
                        documentActionType = DocumentActionType.Refresh;
                        processFrm.OpenProcess(Workplace.WndHandle,
                            "Обновление документов", "Обновление завершено", "Обновление завершено с ошибкой", false);
                        processFrm.OpenOperation("Обновление выделенных документов задачи", false, true, (int)Const.otProcess);
                        break;
                    case "btnWriteBackAll":
                        UltraGridHelper.GetSelectedIDs(_tasksView.ugDocuments, out selectedId);
                        bool processSelectedDocuments = selectedId.Count > 0;
                        selectedId = null;
                        if (FormWritebackOptions.ShowWritebackOptions(Workplace.WindowHandle, ref processSelectedDocuments, ref rewriteData, ref processMD))
                        {
                            documentActionType = DocumentActionType.WriteBack;
                            if (rewriteData)
                                documentActionType |= DocumentActionType.Rewrite;
                            if (processMD)
                                documentActionType |= DocumentActionType.ProcessCube;
                            if (processSelectedDocuments)
                                UltraGridHelper.GetSelectedIDs(_tasksView.ugDocuments, out selectedId);
                        }
                        else
                            return;
                        processFrm.OpenProcess(Workplace.WndHandle, "Обратная запись", "Обратная запись завершена", "Обратная запись завершена с ошибкой", false);
                        if (processSelectedDocuments)
                        {
                            processFrm.OpenOperation("Обратная запись с выделенных листов задачи", false, true, (int)Const.otProcess);
                        }
                        else
                        {
                            processFrm.OpenOperation("Обратная запись со всех листов задачи", false, true, (int)Const.otProcess);
                        }                     
                        break;
                }

                List<DataRow> selectedRows = new List<DataRow>();
                if (selectedId != null)
                {
                    foreach (int id in selectedId)
                    {
                        selectedRows.Add(documents.Select(string.Format("ID = {0}", id))[0]);
                    }
                }
                else
                {
                    foreach (DataRow row in documents.Rows)
                    {
                        selectedRows.Add(row);
                    }
                }
                selectedId = null;

                for (int i = 0; i < selectedRows.Count; i++)
                {
                    DataRow row = selectedRows[i];
                    if (row.RowState == DataRowState.Deleted)
                        continue;
                    int docID = Convert.ToInt32(row["ID"]);
                    string docName = Convert.ToString(row["Name"]);
                    string sourceFileName = Convert.ToString(row["SourceFileName"]);
                    // 
                    string localDocumentName = documentsHelper.CheckLocalTaskDocument(
                        null,
                        ActiveTask,
                        ActiveTask.ID,
                        docID,
                        docName,
                        sourceFileName
                    );
                    string fileName = Path.GetFileNameWithoutExtension(localDocumentName);
                    TaskDocumentType dt = documentsHelper.ResolveDocumentType(localDocumentName);
                    
                    if (TaskDocumentHelper.IsPlanningDocument(dt))
                    {
                        try
                        {
                            allProcessed++;
                            switch (btn.Key)
                            {
                                case "btnRefreshAll":
                                case "btnRefreshSelect":
                                    processFrm.OpenOperation(
                                        String.Format("Обновление документа '{0}' ({1} из {2})", docName, i + 1, selectedRows.Count),
                                        false, true, (int)Const.otProcess);
                                    break;
                                case "btnWriteBackAll":
                                case "btnWriteBackSelect":
                                    processFrm.OpenOperation(
                                        String.Format("Обратная запись данных документа '{0}' ({1} из {2})", docName, i + 1, selectedRows.Count),
                                        false, true, (int)Const.otProcess);
                                    break;
                            }

                            processFrm.PostInfo("Открытие документа");

                            // файл свободен?
                            if (!FileHelper.FileIsVacant(localDocumentName))
                            {
                                processFrm.PostError("Файл занят другим приложением и будет пропущен");
                                skipped++;
                                continue;
                            }

                            OfficeApplication officeApp = ExcelApplication.IsApplicableFile(localDocumentName)
                                ? (OfficeApplication)excel
                                : (OfficeApplication)word;

                            // открываем лист
                            new TaskDocumentHelper().OpenDocumentFromTask(
                                localDocumentName,
                                false,
                                false,
                                true,
                                Workplace.ActiveScheme,
                                ActiveTask.Headline,
                                ActiveTask.ID,
                                fileName,
                                docID,
                                Workplace.ActiveScheme.UsersManager.GetUserNameByID(ActiveTask.Doer),
                                (int)dt,
                                ActiveTask,
                                OfficeMessageSilentHandler,
                                officeApp,
                                documentActionType
                            );

                            
                            // сохраняем изменения всегда, чтобы не терялись записи во 
                            // внутренней истории документов
                            processFrm.PostInfo("Сохранение изменений");
                           // officeApp.SaveChanges(null, localDocumentName);

                            // Получаем результат выполнения операции
                            bool succ = false;
                            using (OfficeCustomPropertiesAdapter customPropertiesAdapter
                                = OfficeCustomPropertiesFactory.Create(localDocumentName))
                            {
                                object result = customPropertiesAdapter.GetProperty("fm.Result.Success");
                                if (result != null)
                                    succ = Convert.ToBoolean(result);
                            }

                            // сохраняем изменения всегда, чтобы не терялись записи во 
                            // внутренней истории документов
                            //processFrm.PostInfo("Сохранение изменений");
                            //officeApp.SaveChanges(null, localDocumentName);

                            if (succ)
                            {
                                succProcessed++;
                                // вынесено на уровень выше, чтобы операция всегда закрывалась
                                processFrm.CloseOperation();
                            }
                            else
                            {
                                processedWithErrors++;
                                processFrm.PostError("Обработка документа завершена с ошибкой");
                            }
                        }
                        catch (Exception e)
                        {
                            processedWithErrors++;
                            processFrm.PostError(String.Format("Необработанное исключение: {0}", e.Message));
                        }

                        // закрываем лист
                        if (ExcelApplication.IsApplicableFile(localDocumentName))
                        {
                            excel.DisplayAlerts = false;
                            excel.CloseWorkBooks();
                        }
                        else
                        {
                            word.DisplayAlerts = false;
                            word.CloseFirstDocument();
                        }
                    }
                }
            }
            finally
            {
                // пишем стат. информацию
                if (processFrm != null)
                try
                {
                    processFrm.PostInfo(String.Format("Обработано документов: {0}", allProcessed));
                    processFrm.PostInfo(String.Format("Обработано успешно: {0}", succProcessed));
                    processFrm.PostInfo(String.Format("Обработано с ошибками: {0}", processedWithErrors));
                    processFrm.PostInfo(String.Format("Пропущено: {0}", skipped));
                    // если были ошибки и пропущенные документы - закрываем всю операцию с ошибкой
                    if ((processedWithErrors > 0) || (skipped > 0))
                        processFrm.PostError("Операция завершена с ошибками");
                    else
                        processFrm.CloseOperation();

                    processFrm.CloseProcess();

                    while (processFrm.Showing)
                    {
                        Application.DoEvents();
                        Thread.Sleep(50);
                    }
                } catch {}

               // EnableWindow((IntPtr)Workplace.WndHandle, true);
         
                if (processFrm != null)
                try
                {
                    //processFrm.ParentWnd = 0;
                    Marshal.ReleaseComObject(processFrm);
                } catch {}
                if (excelPlanningItf != null)
                try
                {
                    excelPlanningItf.ProcessForm = null;
                    Marshal.ReleaseComObject(excelPlanningItf);
                } catch {}
                if (wordPlanningItf != null)
                try
                {
                    wordPlanningItf.ProcessForm = null;
                    Marshal.ReleaseComObject(wordPlanningItf);
                } catch {}

                if (excel != null)
                    excel.Dispose();

                if (word != null)
                {
                    word.Quit();
                    word.Dispose();
                }

                GC.GetTotalMemory(true);
                CoFreeUnusedLibraries();
            }

        }

        private void OfficeMessageHandler(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OfficeMessageSilentHandler(string message)
        {
        }

        
        #endregion

        /// <summary>
        /// Добавление документа в задачу
        /// </summary>
        /// <param name="addedDocumentType">тип добавляемого документа</param>
        /// <param name="activeTask">активная задача</param>
        /// <param name="addedFileName">название файла</param>
        /// <param name="documentName">название документа</param>
        /// <param name="docComment">каментарий к документу</param>
        /// <param name="showDocument">отображать документ или нет</param>
        private void InternalAddDocument(
            AddedTaskDocumentType addedDocumentType,
            TaskStub activeTask,
            string addedFileName,
            string documentName,
            string docComment,
            bool showDocument
        )
        {
            object obj = null;
            try
            {
                // получаем ID документа
                int documentID = ActiveTask.GetNewDocumentID();

                string localDocumentName = String.Empty;
                string userName = Workplace.ActiveScheme.UsersManager.GetUserNameByID(ActiveTask.Doer);

                string fileName = String.Empty;
                TaskDocumentType dt = TaskDocumentType.dtArbitraryDocument;
                switch (addedDocumentType)
                {
                    #region существующий документ
                    case AddedTaskDocumentType.ndtExisting:
                        if (!DocumentProperties.CheckFileNameAndSize(addedFileName))
                            return;
                        fileName = Path.GetFileName(addedFileName);
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        // копируем документ в локальную папку
                        File.Copy(addedFileName, localDocumentName);
                        File.SetAttributes(localDocumentName, FileAttributes.Normal);
                        // если это документ плагина - устанавливаем ему свойства
                        dt = documentsHelper.ResolveDocumentType(localDocumentName);
                        if (TaskDocumentHelper.IsPlanningDocument(dt))
                        {
                            // При добавлении существующего документа открывать и добавлять контекст задачи смысла нет
                            // он будет добавлен при обратной записи или открытии документа
                            
                            new TaskDocumentHelper().OpenDocumentFromTask(
                                        localDocumentName, showDocument, true, true, Workplace.ActiveScheme,
                                        ActiveTask.Headline, ActiveTask.ID.ToString(),
                                        documentName, documentID.ToString(), userName,
                                        (int)dt, ActiveTask, OfficeMessageHandler);
                        }
                        else
                        {
                            if (showDocument)
                                documentsHelper.OpenTaskDocument(Workplace.OperationObj, ActiveTask, Workplace.ActiveScheme, documentID,
                                    documentName, localDocumentName, (int)dt, true);
                        }
                        break;
                    #endregion

                    #region новый документ планирования (инициализируется как форма ввода)
                    case AddedTaskDocumentType.ndtNewCalcSheetExcel:
                        dt = TaskDocumentType.dtCalcSheet;
                        fileName = "Новый документ" + OfficeHelper.GetExcelExtension();
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        new TaskDocumentHelper().OpenDocumentFromTask(
                            localDocumentName, true, true, true, Workplace.ActiveScheme,
                            ActiveTask.Headline, ActiveTask.ID.ToString(),
                            documentName, documentID.ToString(), userName,
                            (int)dt, ActiveTask, OfficeMessageHandler);
                        break;
                    case AddedTaskDocumentType.ndtNewCalcSheetWord:
                        dt = TaskDocumentType.dtReport;
                        fileName = "Новый документ" + OfficeHelper.GetWordExtension();
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        new TaskDocumentHelper().OpenDocumentFromTask(
                            localDocumentName, true, true, true, Workplace.ActiveScheme,
                            ActiveTask.Headline, ActiveTask.ID.ToString(),
                            documentName, documentID.ToString(), userName,
                            (int)dt, ActiveTask, OfficeMessageHandler);
                        break;
                    #endregion

                    #region новый документ Excel
                    case AddedTaskDocumentType.ndtNewExcel:
                        dt = TaskDocumentType.dtExcelDocument;
                        fileName = "Новый документ" + OfficeHelper.GetExcelExtension();
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        using (ExcelApplication excelApp = OfficeHelper.CreateExcelApplication())
                        {
                            excelApp.CreateEmptyDocument(localDocumentName);
                            excelApp.Visible = true;
                        }
                        break;
                    #endregion

                    #region новый документ MDX-expert
                    case AddedTaskDocumentType.ndtNewMDXExpert:
                        dt = TaskDocumentType.dtMDXExpertDocument;
                        fileName = "Новый документ.exd";
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        // создать новый документ MDXExpert
                        MdxExpertHelper.CreateDocument(localDocumentName, Workplace.ActiveScheme.SchemeMDStore.ServerName, Workplace.ActiveScheme.SchemeMDStore.CatalogName);
                        break;
                    #endregion

                    #region новый документ Word
                    case AddedTaskDocumentType.ndtNewWord:
                        dt = TaskDocumentType.dtWordDocument;
                        fileName = "Новый документ" + OfficeHelper.GetWordExtension();
                        localDocumentName = DocumentsHelper.GetLocalDocumentName(ActiveTask.ID, documentID, documentName, fileName);
                        using (WordApplication wordApp = OfficeHelper.CreateWordApplication())
                        {
                            wordApp.CreateEmptyDocument(localDocumentName);
                            wordApp.Visible = true;
                        }
                        break;
                    #endregion
                }

                // создаем новую запись в гриде документов
                InitDocumentRow(documentID, (int)dt, documentName, fileName, 0, docComment);
            }
            finally
            {
                // удаляем ссылку на объект офиса
                if ((obj != null) && (Marshal.IsComObject(obj)))
                {
                    Marshal.ReleaseComObject(obj);
                    GC.GetTotalMemory(true);
                }
            }
        }

        /// <summary>
        /// Добавление документа
        /// </summary>
        /// <param name="adt">тип документа</param>
        private void AddTaskDocument(AddedTaskDocumentType adt)
        {
            string documentName = string.Empty;
            string fileName = string.Empty;
            string comment = string.Empty;
            // показываем форму задания свойств документа (название, комментарий, путь к файлу)
            if (DocumentProperties.SelectDocument(adt, ref documentName, ref fileName, ref comment))
            {
                Workplace.OperationObj.Text = "Добавление документа";
                Workplace.OperationObj.StartOperation();
                try
                {
                    InternalAddDocument(adt, ActiveTask, fileName, documentName, comment, false);
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопки и выпадающий список тулбара грида с документами
        /// </summary>
        private void utbDocuments_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                // Удалить документ
                case "btnDelDocument":
                    DelTaskDocument();
                    break;
                // Добавить произвольный документ (сама кнопка со списком)
                case "pmAddDocument":
                    AddTaskDocument(AddedTaskDocumentType.ndtExisting);
                    break;
                // Добавить произвольный документ (пункт списка)
                case "btnSelectDocumentFile":
                    AddTaskDocument(AddedTaskDocumentType.ndtExisting);
                    break;
                case "btnSelectDocumentsFiles":
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Все файлы (*.*)|*.*";
                    ofd.Multiselect = true;
                    if ((ofd.ShowDialog() == DialogResult.OK) && (ofd.FileNames.Length > 0))
                    {
                        _tasksNavigation.Refresh();
                        _tasksView.Refresh();
                        Workplace.ProgressObj.MaxProgress = ofd.FileNames.Length;
                        Workplace.ProgressObj.Position = 0;
                        Workplace.ProgressObj.Caption = "Добавление файлов";
                        Workplace.ProgressObj.Text = String.Empty;
                        Workplace.ProgressObj.StartProgress();
                        try
                        {
                            for (int i = 0; i < ofd.FileNames.Length; i++)
                            {
                                string curFile = ofd.FileNames[i];

                                InternalAddDocument(AddedTaskDocumentType.ndtExisting, ActiveTask, curFile,
                                    Path.GetFileNameWithoutExtension(curFile), String.Empty, false);

                                Workplace.ProgressObj.Position++;
                            }
                        }
                        finally
                        {
                            Workplace.ProgressObj.StopProgress();
                        }
                    }
                    break;
                // Добавить новый документ планирования (Excel)
                case "btnNewCalcSheetExcel":
                    AddTaskDocument(AddedTaskDocumentType.ndtNewCalcSheetExcel);
                    break;
                // Добавить новый документ планирования (Word)
                case "btnNewCalcSheetWord":
                    AddTaskDocument(AddedTaskDocumentType.ndtNewCalcSheetWord);
                    break;
                // Добавить новый документ MDX-Expert
                case "btnNewMdxExpertDocument":
                    AddTaskDocument(AddedTaskDocumentType.ndtNewMDXExpert);
                    break;
                // Добавить новый документ Word
                case "btnNewWordDocument":
                    AddTaskDocument(AddedTaskDocumentType.ndtNewWord);
                    break;
                // Добавить новый документ Excel
                case "btnNewExcelDocument":
                    AddTaskDocument(AddedTaskDocumentType.ndtNewExcel);
                    break;
                case "btnSaveAllDocuments":
                    // если таблица документов не инициализирована - выходим
                    if (_tasksView.ugDocuments.DataSource == null)
                        return;
                    DataTable documents = (DataTable)_tasksView.ugDocuments.DataSource;
                    // если таблица документов пуста - выходим
                    if (documents.Rows.Count == 0)
                        return;
                    // если показ диалога завершился неудачей - выходим
                    if (_tasksView.fbSelectDir.ShowDialog() != DialogResult.OK)
                        return;
                    string outputDir = _tasksView.fbSelectDir.SelectedPath;
                    try
                    {
                        Workplace.OperationObj.Text = "Сохранение файла";
                        Workplace.OperationObj.StartOperation();
                        foreach (DataRow row in documents.Rows)
                        {
                            InternalSaveTaskDocument(
                                Convert.ToInt32(row["ID"]),
                                Convert.ToString(row["Name"]),
                                Convert.ToString(row["SourceFileName"]),
                                outputDir
                            );
                        }
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                    }
                    break;
                case "btnOpenAllDocuments":
                    // если таблица документов не инициализирована - выходим
                    if (_tasksView.ugDocuments.DataSource == null)
                        return;
                    documents = (DataTable)_tasksView.ugDocuments.DataSource;
                    // если таблица документов пуста - выходим
                    if (documents.Rows.Count == 0)
                        return;

                    foreach (DataRow row in documents.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        documentsHelper.OpenTaskDocument(
                            Workplace.OperationObj,
                            ActiveTask,
                            Workplace.ActiveScheme,
                            Convert.ToInt32(row["ID"]),
                            Convert.ToString(row["Name"]),
                            Convert.ToString(row["SourceFileName"]),
                            Convert.ToInt32(row["DOCUMENTTYPE"]),
                            false
                        );
                    }
                    break;
                case "btnCutDocuments":
                    CutDocumentsIntoClipboard(_tasksView.ugDocuments);
                    break;
                case "btnCopyDocuments":
                    CopyDocumentsIntoClipboard(_tasksView.ugDocuments);
                    break;
                case "btnPasteDocuments":
                    InsertDocumentsFromClipboard(/*_tasksView.ugDocuments*/);
                    break;
                case "btnRefreshAll":
                case "btnWriteBackAll":
                    //CheckChanges();
                    Application.DoEvents();
                    DoPlanningOperation(e.Tool);
                    break;
                case "btnRefreshSelect":
                //case "btnWriteBackSelect":
                    UltraGridHelper.GetSelectedIDs(_tasksView.ugDocuments, out selectedId);
                    if (selectedId.Count == 0)
                    {
                        MessageBox.Show("Необходимо выбрать хотя бы один документ", "Документы", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //CheckChanges();
                    Application.DoEvents();
                    DoPlanningOperation(e.Tool);
                    break;
            }
        }

        /// <summary>
        /// Обрабочик нажатия на кнопку открытия документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugDocuments_ClickCellButton(object sender, CellEventArgs e)
        {
            e.Cell.Row.Update();
            //ugDocuments_AfterCellUpdate(sender, new CellEventArgs(e.Cell.Row.Cells["Name"]));
            switch (e.Cell.Column.Key)
            {
                case "clmnEdit":
                    documentsHelper.OpenTaskDocument(
                        Workplace.OperationObj,
                        ActiveTask,
                        Workplace.ActiveScheme,
                        Convert.ToInt32(e.Cell.Row.Cells["ID"].Value),
                        e.Cell.Row.Cells["Name"].Value.ToString(),
                        e.Cell.Row.Cells["SourceFileName"].Value.ToString(),
                        Convert.ToInt32(e.Cell.Row.Cells["DOCUMENTTYPE"].Value),
                        true);
                    //Workplace.WorkplaceState = FormWindowState.Minimized;
    
                    break;
                case "clmnSave":
                    SaveTaskDocument(e.Cell);
                    break;
            }
            e.Cell.Activated = false;
        }

        /// <summary>
        /// Обновление ячеек грида с документами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugDocuments_AfterCellUpdate(object sender, CellEventArgs e)
        {
            string columnName = e.Cell.Column.Key.ToLower();

            switch (columnName)
            {
                // проверка изменения названия документа - может измениться только 
                // в режиме редактирования
                case "name":
                    // получаем новые значения названий
                    string oldName = e.Cell.OriginalValue.ToString();
                    string newName = e.Cell.Value.ToString();
                    if (oldName != newName)
                    {
                        int docID = Convert.ToInt32(e.Cell.Row.Cells["ID"].Value);
                        int taskID = ActiveTask.ID;
                        string sfName = Convert.ToString(e.Cell.Row.Cells["SourceFileName"].Value);
                        // формируем имена файлов
                        oldName = documentsHelper.CheckLocalTaskDocument(Workplace.OperationObj,
                            ActiveTask, taskID, docID, oldName, sfName);
                        newName = DocumentsHelper.GetLocalDocumentName(taskID, docID, newName, sfName);
                        // меняем название файла на диске
                        File.Copy(oldName, newName);
                        File.Delete(oldName);
                    }
                    break;
            }
        }

        /// <summary>
        /// Обработчик инициализации строки грида с документами (разыменовка типа документа, 
        /// типа файла, настройка колонок с кнопками открыть/сохранить документ)
        /// </summary>
        private void ugDocuments_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // кнопка открытия документа
            UltraGridCell cell = e.Row.Cells["clmnEdit"];
            if (cell != null)
                cell.ToolTipText = "Открыть документ";
            // кнопка сохранения документа
            cell = e.Row.Cells["clmnSave"];
            if (cell != null)
                cell.ToolTipText = "Сохранить документ";
            // расширение файла
            string fileExt = String.Empty;
            cell = e.Row.Cells["clmnFileExt"];
            if (cell != null)
            {
                try
                {
                    fileExt = Path.GetExtension(e.Row.Cells["SourceFileName"].Value.ToString());
                    cell.Value = fileExt.ToLower();
                }
                catch { }
            }

            // тип файла
            TaskDocumentType dt = TaskDocumentType.dtArbitraryDocument;
            try
            {
                int dtInt = Convert.ToInt32(e.Row.Cells["DocumentType"].Value);
                dt = (TaskDocumentType)dtInt;
            }
            catch { }
            // разыменовка типа документа
            cell = e.Row.Cells["clmnDocumentTypePic"];
            cell.Value = TaskUtils.TaskDocumentTypeToString(dt, fileExt);
            // Принадлежность
            cell = e.Row.Cells["Ownership"];
            TaskDocumentOwnership ownership = (TaskDocumentOwnership)Convert.ToInt32(cell.Value);
            e.Row.Cells["clmnOwnershipName"].Value = TaskUtils.TaskDocumentOwnershipToString(ownership);
            // если идет операция копирования и документ находится в состоянии "вырезано" - ставим строке соотв. стиль
            if ((ClipbpardHelper.CuttedDocumentsIDs != null) && (ClipbpardHelper.CuttedDocumentsIDs.Count > 0))
            {
                int id = UltraGridHelper.GetRowID(e.Row);
                if (ClipbpardHelper.CuttedDocumentsIDs.Contains(id))
                    UltraGridHelper.SetRowTransparentColor(e.Row, true);
            }
        }

        private void ugDocuments_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand gb = e.Layout.Bands[i];
                UltraGridColumn clmn;

                // в зависимости от состояния задачи некоторые поля могут быть недоступны
                Activation act = Activation.ActivateOnly;
                if (ActiveTask.InEdit) act = Activation.AllowEdit;

                // Открыть
                if (gb.Columns.Exists("clmnEdit"))
                    clmn = gb.Columns["clmnEdit"];
                else
                    clmn = gb.Columns.Add("clmnEdit");
                clmn.Header.VisiblePosition = 0;
                UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, 2);

                // Сохранить
                if (gb.Columns.Exists("clmnSave"))
                    clmn = gb.Columns["clmnSave"];
                else
                    clmn = gb.Columns.Add("clmnSave");
                clmn.Header.VisiblePosition = 1;
                UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, 3);

                // ID
                clmn = gb.Columns["ID"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Header.VisiblePosition = 2;
                clmn.Width = 37;

                // Тип документа
                if (gb.Columns.Exists("clmnDocumentTypePic"))
                    clmn = gb.Columns["clmnDocumentTypePic"];
                else
                    clmn = gb.Columns.Add("clmnDocumentTypePic");
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Header.Caption = "Тип документа";
                clmn.Header.VisiblePosition = 3;
                clmn.Width = 130;

                // Название
                clmn = gb.Columns["Name"];
                clmn.CellActivation = act;
                clmn.Header.Caption = "Название";
                clmn.Header.VisiblePosition = 4;
                clmn.Width = 344;

                // Комментарий
                clmn = gb.Columns["Description"];
                clmn.CellActivation = act;
                clmn.Header.Caption = "Комментарий";
                clmn.Header.VisiblePosition = 5;
                clmn.Width = 200;

                // Тип файла
                if (gb.Columns.Exists("clmnFileExt"))
                    clmn = gb.Columns["clmnFileExt"];
                else
                    clmn = gb.Columns.Add("clmnFileExt");
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Header.Caption = "Тип файла";
                clmn.Header.VisiblePosition = 6;
                clmn.Width = 76;

                // Принадлежность
                if (gb.Columns.Exists("clmnOwnershipName"))
                    clmn = gb.Columns["clmnOwnershipName"];
                else
                    clmn = gb.Columns.Add("clmnOwnershipName");
                clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                clmn.CellActivation = act;
                clmn.Header.Caption = "Принадлежность";
                clmn.Header.VisiblePosition = 7;
                clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                clmn.Width = 120;
                clmn.ValueList = e.Layout.ValueLists[0];

                // Версия
                clmn = gb.Columns["Version"];
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Header.Caption = "Версия";
                clmn.Width = 50;
                clmn.Header.VisiblePosition = 8;


                #region Невидимые
                clmn = gb.Columns["DocumentType"];
                clmn.Hidden = true;

                clmn = gb.Columns["Ownership"];
                clmn.Hidden = true;

                clmn = gb.Columns["SourceFileName"];
                clmn.Hidden = true;

                if (gb.Columns.Exists("REFTASKS"))
                {
                    clmn = gb.Columns["RefTasks"];
                    clmn.Hidden = true;
                }

                if (gb.Columns.Exists("REFTASKSTEMP"))
                {
                    clmn = gb.Columns["RefTasksTemp"];
                    clmn.Hidden = true;
                }
                #endregion

            }
        }

        private void ugDocuments_BeforeCellListDropDown(object sender, CancelableCellEventArgs e)
        {
            UltraGridColumn clmn = e.Cell.Column;
            // список принадлежности документа
            if (clmn.Key == "clmnOwnershipName")
            {
                // определяем текущее состояние документа
                TaskDocumentOwnership curOwnership = (TaskDocumentOwnership)Convert.ToInt32(e.Cell.Row.Cells["Ownership"].Value);

                ValueList vl = (ValueList)clmn.ValueList; //Layout.ValueLists["clmnOwnershipNameDropDownList"];

                // формируем список доступных типов
                vl.ValueListItems.Clear();
                // элемент документ кому-то принадлежит, то его можно вернуть в "Общие"
                if (curOwnership != TaskDocumentOwnership.doGeneral)
                    vl.ValueListItems.Add((int)TaskDocumentOwnership.doGeneral, "Общий документ");
                // если документ "Общий" - можно перевести его в состояние...
                if (ActiveTask == null)
                    return;
                if (curOwnership == TaskDocumentOwnership.doGeneral)
                {
					int curUser = ClientAuthentication.UserID;
                    if (curUser == ActiveTask.Owner)
                        vl.ValueListItems.Add((int)TaskDocumentOwnership.doOwner, "Документ владельца");
                    if (curUser == ActiveTask.Doer)
                        vl.ValueListItems.Add((int)TaskDocumentOwnership.doDoer, "Документ исполнителя");
                    if (curUser == ActiveTask.Curator)
                        vl.ValueListItems.Add((int)TaskDocumentOwnership.doCurator, "Документ куратора");
                }
                //vl.ReSyncContents();
                // если список пуст, то его показывать не надо
                if (vl.ValueListItems.Count == 0)
                    e.Cancel = true;
            }
        }

        private void ugDocuments_CellListSelect(object sender, CellEventArgs e)
        {
            UltraGridColumn clmn = e.Cell.Column;
            // список принадлежности документа
            if (clmn.Key == "clmnOwnershipName")
            {
                _tasksView.ugDocuments.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    // смотрим что выбрали
                    ValueList vl = (ValueList)clmn.ValueList;
                    int selInd = vl.SelectedIndex;
                    TaskDocumentOwnership selOwnership = (TaskDocumentOwnership)vl.ValueListItems[selInd].DataValue;
                    // устанавливаем новое значение
                    e.Cell.Row.Cells["Ownership"].Value = (int)selOwnership;
                    _tasksView.ugDocuments.PerformAction(UltraGridAction.ExitEditMode);
                    vl.ValueListItems.Clear();
                }
                finally
                {
                    _tasksView.ugDocuments.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    // инциируем обновление разыменовки принадлежности документа
                    e.Cell.Row.Refresh(RefreshRow.FireInitializeRow, false);
                }
            }
        }

        #region добавление документов через драг дроп

        void ugDocuments_DragEnter(object sender, DragEventArgs e)
        {
            if (ActiveTask.InEdit)
                if (e.Data.GetDataPresent("FileDrop"))
                    e.Effect = DragDropEffects.Copy;
        }

        void ugDocuments_DragDrop(object sender, DragEventArgs e)
        {
            // если перетаскиваем не файлы и не каталоги, выходим
            if (!e.Data.GetDataPresent("FileDrop")) return;
            // получем имена файлов, которые были выделены и перетащены на грид с документами
            string[] files = (string[]) e.Data.GetData("FileDrop");
            List<string> dropFiles = new List<string>();
            foreach (string file in files)
            {
                FileAttributes attr = File.GetAttributes(file);
                // если это директория 
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    dropFiles.AddRange(Directory.GetFiles(file));
                else
                    dropFiles.Add(file);
            }
            // добавляем файлы в задачу
            Workplace.OperationObj.Text = String.Format("Добавление файлa: {0}", "...");
            Workplace.OperationObj.StartOperation();
            try
            {
                foreach (string fileName in dropFiles)
                {
                    InternalAddDocument(AddedTaskDocumentType.ndtExisting, ActiveTask, fileName,
                            Path.GetFileNameWithoutExtension(fileName), String.Empty, false);
                    Workplace.OperationObj.Text = String.Format("Добавление файлa: {0}", fileName);
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        #endregion
    }
}
