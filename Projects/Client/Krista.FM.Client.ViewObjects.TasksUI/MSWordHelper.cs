using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Common;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.Xml;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public sealed class MSWordReportHelper
    {
        #region внутренние переменные-объекты
        // текущий объект выборка... основной объект для работы
        private static object selection;
        // текущий документ ворда
        private static object activeDocument;
        // приложение ворда
        private static object wordApplication;
        // таблица, с которой нужно работать
        private static object table;
        #endregion

        // параметры ширины колонок таблицы для перечня задач
        private static int[] liteReportTable = {30, 75, 160, 70, 56, 56, 85, 85, 85, 70};
        // параметры ширины колонок таблицы для перечня документов в задаче
        private static int[] fullReportTable = { 50, 97, 100, 57, 53, 134 };

        private static MSWordReportsKind reportKind;

        /*
        /// <summary>
        /// конструктор класса
        /// </summary>
        public MSWordReportHelper()
        {

        }*/

        /// <summary>
        /// задание размера шрифта
        /// </summary>
        /// <param name="fontSize"></param>
        static private void SetSelectionFontSize(int fontSize)
        {
            object font = ReflectionHelper.GetProperty(selection, "Font");
            ReflectionHelper.SetProperty(font, "Size", fontSize);
        }

        /// <summary>
        /// ставим стиль для печати
        /// </summary>
        /// <param name="styleName"></param>
        static private void SetSelectionStyle(string styleName)
        {
            //object styles = ReflectionHelper.GetProperty(activeDocument, "Styles");
            try
            {
                ReflectionHelper.SetProperty(selection, "Style", styleName);
            }
            catch { }
        }

        enum Direction { MoveDown, MoveUp, MoveRight, MoveLeft };

        enum Units
        {
            wdCell = 10,
            wdCharacter = 1,
            wdCharacterFormatting = 13,
            wdColumn = 9,
            wdItem = 16,
            wdLine = 5,
            wdParagraph = 4,
            wdParagraphFormatting = 14,
            wdRow = 10,
            wdScreen = 7,
            wdSection = 8,
            wdSentence = 3,
            wdStory = 6,
            wdTable = 15,
            wdWindow = 11,
            wdWord = 2
        }

        enum Alignment
        {
            wdAlignParagraphCenter = 1,
            wdAlignParagraphLeft = 0,
            wdAlignParagraphRight = 2
        }

        /// <summary>
        /// переход на новую строку
        /// </summary>
        static void CreateNextLine()
        {
            ReflectionHelper.CallMethod(selection, "TypeParagraph");
        }

        /// <summary>
        /// перемещает курсор на некоторое количество строк и т.п. в любую сторону
        /// </summary>
        /// <param name="lineCount">количество объектов, через которое должен переместиться курсор</param>
        /// <param name="direction">направление перемещения</param>
        /// <param name="unit">объект перемещения</param>
        static void GoTo(int lineCount, Direction direction, Units unit)
        {
            ReflectionHelper.CallMethod(selection, direction.ToString(), (int)unit, lineCount);
        }

        private static void SetTextAligment(Alignment alignment)
        {
            object format = ReflectionHelper.GetProperty(selection, "ParagraphFormat");
            ReflectionHelper.SetProperty(format, "Alignment", (int)alignment);
        }

        /// <summary>
        /// создание таблицы 
        /// </summary>
        /// <param name="rowsCount">количество строк</param>
        /// <param name="columnsCount">количество столбцов</param>
        /// <returns></returns>
        static object CreateTable(int rowsCount, int columnsCount)
        {
            if (rowsCount == 0 || columnsCount == 0) return null;
            object tables = ReflectionHelper.GetProperty(activeDocument, "Tables");
            return ReflectionHelper.CallMethod(tables, "Add", 
                ReflectionHelper.GetProperty(selection, "Range"),
                rowsCount,
                columnsCount,
                1,
                false
            );
        }

        /// <summary>
        /// устанавливает параметры столбцов для таблицы
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnsWidth"></param>
        private static void SetTableParams(object table, int[] columnsWidth)
        {
            object columns = ReflectionHelper.GetProperty(table, "Columns");
            int columnsCount = (int)ReflectionHelper.GetProperty(columns, "Count");
            
            for (int i = 1; i <= columnsCount; i++)
            {
                object column = ReflectionHelper.CallMethod(columns, "Item", i);
                ReflectionHelper.CallMethod(column, "SetWidth", columnsWidth[i - 1], 0);
            }
            ReflectionHelper.SetProperty(table, "AllowAutoFit", false);
        }

        /// <summary>
        /// установка текста на текущей позиции курсора
        /// </summary>
        /// <param name="text"></param>
        static void SetText(object text)
        {
            ReflectionHelper.CallMethod(selection, "TypeText", text.ToString());
        }

        /// <summary>
        /// добавляет строку предположительно в таблицу
        /// </summary>
        static void AddRow()
        {
            ReflectionHelper.CallMethod(selection, "InsertRowsBelow", 1);
        }

        /// <summary>
        /// делаем структуру из отдельных значений
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="blockedBy"></param>
        /// <param name="headLine"></param>
        /// <param name="state"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="owner"></param>
        /// <param name="doer"></param>
        /// <param name="curator"></param>
        /// <param name="parentTask"></param>
        /// <returns></returns>
        static Dictionary<string, object> GetTaskDictionary(int taskID, string blockedBy, string headLine, string state,
            DateTime fromDate, DateTime toDate, string owner, string doer, string curator, int parentTask)
        {
            Dictionary<string, object> taskParams = new Dictionary<string, object>();
            taskParams.Add("ID", taskID);
            taskParams.Add("Заблокирована", blockedBy);
            taskParams.Add("Наименование", headLine);
            taskParams.Add("Состояние", state);
            taskParams.Add("Дата начала", fromDate);
            taskParams.Add("Дата окончания", toDate);
            taskParams.Add("Владелец", owner);
            taskParams.Add("Исполнитель", doer);
            taskParams.Add("Куратор", curator);
            if (parentTask > 0)
                taskParams.Add("Родительская задача", parentTask);
            else
                taskParams.Add("Родительская задача", "нет");
            return taskParams;
        }

        /// <summary>
        /// Рекурсивная процедура экспорта задачи и всех ее потомков
        /// </summary>
        /// <param name="taskNode">XmlNode c параметрами задачи</param>
        /// <param name="project">проект MSProject</param>
        /// <param name="app">приложение MSProject</param>
        /// <param name="parentTask">родительская задача (для верхнего уровня == null)</param>
        /// <param name="childIndex">порядковый номер дочерней задачи</param>
        /// <param name="level">текущий уровень иерархии</param>
        /// <param name="users">таблица пользователей</param>
        private static void ProcessTask(XmlNode taskNode, object wordApp,
            int childIndex, int level, DataTable users, int parentTaskID)
        {
            if (level > maxLevel)
                maxLevel = level;

            // получаем параметры задачи
            int taskID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.IDTagName, 0);
            string headLine = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskHeadlineTagName, String.Empty);
            string job = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskJobTagName, String.Empty);
            int ownerID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskOwnerTagName, 0);
            int doerID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskDoerTagName, 0);
            int curatorID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskCuratorTagName, 0);
            DateTime fromDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskFromDateTagName, String.Empty));
            DateTime toDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskToDateTagName, String.Empty));
            string taskState = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskStateTagName, String.Empty);
            int blockedByID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskLockByUser, 0);
            string blockedByUserName = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskLockedUserName, String.Empty);
            string comment = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskDescriptionTagName, String.Empty);

            // получаем список подчиненных задач
            XmlNodeList childTasks = taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            // получаем параметры задачи родителя если есть такая 
            XmlNode parentTask = taskNode.ParentNode;
            parentTask = parentTask.ParentNode;
            string parentTaskName = string.Empty;
            string parentTaskState = string.Empty;
            if (parentTask.Attributes[TasksExportXmlConsts.IDTagName] != null)
            {
                parentTaskID = XmlHelper.GetIntAttrValue(parentTask, TasksExportXmlConsts.IDTagName, 0);
                parentTaskName = XmlHelper.GetStringAttrValue(parentTask, TasksExportXmlConsts.TaskHeadlineTagName, String.Empty);
                parentTaskState = XmlHelper.GetStringAttrValue(parentTask, TasksExportXmlConsts.TaskStateTagName, String.Empty);
            }

            string owner = TaskUtils.DefineUserName(users, ownerID);
            string doer = TaskUtils.DefineUserName(users, doerID);
            string curator = TaskUtils.DefineUserName(users, curatorID);
            string blockedByUser;
            if (blockedByUserName != string.Empty)
                blockedByUser = TaskUtils.DefineUserName(users, blockedByID);
            else
                blockedByUser = "Нет";

            // заполняем отчет
            if (reportKind == MSWordReportsKind.Full)
            {
                int taskType = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskTypeTagName, 0);
                string taskTypeName = taskTypes.Select(string.Format("ID = {0}", taskType))[0]["Name"].ToString();
                // печатаем параметры задачи
                SetSelectionStyle(String.Format("Заголовок {0}", level));
                SetSelectionFontSize(14);
                SetText(string.Format("Задача: {0} <{1}> {2}", taskID, taskState, headLine));
                CreateNextLine();
                SetSelectionStyle("Обычный");
                SetSelectionFontSize(12);
                if (parentTaskID > 0)
                    SetBoldHeader("Родительская задача: ", string.Format("{0} <{1}> {2}", parentTaskID, parentTaskState, parentTaskName));
                else
                    SetBoldHeader("Родительская задача: ", "Нет");
                CreateNextLine();
                SetBoldHeader("Заблокирована: ", string.Format("{0}", blockedByUser));
                SetText(string.Format(" (текущее действие: {0})", taskState));
                CreateNextLine();
                SetBoldHeader("Дата начала: ", string.Format("{0}", fromDate));
                SetBoldHeader(" Дата завершения: ", string.Format("{0}", toDate));
                CreateNextLine();
                SetBoldHeader("Владелец: ", string.Format("{0}", owner));
                CreateNextLine();
                SetBoldHeader("Исполнитель: ", string.Format("{0}.", doer));
                CreateNextLine();
                SetBoldHeader("Куратор: ", string.Format("{0}", curator));
                CreateNextLine();
                SetBoldHeader("Вид задачи: ", string.Format("{0}", taskTypeName));
                CreateNextLine();
                SetBoldHeader("Задание: ", string.Format("{0}", job));
                CreateNextLine();
                SetBoldHeader("Комментарий: ", string.Format("{0}", comment));
                CreateNextLine();

                // получаем все документы задачи, для них делаем таблицу
                List<Dictionary<string, object>> docs = GetDocumentParams(taskNode);
                // если документы есть, то добавляем таблицу
                if (docs.Count > 0)
                {
                    SetBoldHeader("Документы задачи:", string.Empty);
                    CreateNextLine();
                    SetSelectionFontSize(10);
                    table = CreateTable(docs.Count + 1, fullReportTable.Length);
                    SetTableParams(table, fullReportTable);
                    
                    SetTableHeader(docs[0]);
                    SetTextAligment(Alignment.wdAlignParagraphLeft);
                    foreach (Dictionary<string, object> documentDict in docs)
                    {
                        SetDataToTable(table, documentDict);
                    }

                    GoTo(1, Direction.MoveRight, Units.wdCharacter);
                }
                //CreateNextLine();
            }
            else
            {
                Dictionary<string, object> dict = GetTaskDictionary(taskID, blockedByUser, headLine, taskState, fromDate, toDate, owner, doer, curator, parentTaskID);
                SetSelectionFontSize(10);
                // печатаем параметры задачи в таблицу
                if (table == null)
                {
                    table = CreateTable(2, liteReportTable.Length);
                    SetTableParams(table, liteReportTable);
                    SetTableHeader(dict);
                }

                SetDataToTable(table, dict);
                AddRow();
            }

            // рекурсивно обрабатываем все подчиненные
            if ((childTasks != null) && (childTasks.Count > 0))
            {
                for (int i = 0; i < childTasks.Count; i++)
                {
                    XmlNode childTask = childTasks[i];
                    ProcessTask(childTask, wordApp, i, level + 1, users, taskID);
                }
            }
        }

        private static List<Dictionary<string, object>> GetDocumentParams(XmlNode taskNode)
        {
            List<Dictionary<string, object>> docParams = new List<Dictionary<string,object>>(); 

            XmlNodeList childDocuments = taskNode.SelectNodes(string.Format("{0}/{1}", TasksExportXmlConsts.DocumentsTagName, TasksExportXmlConsts.DocumentTagName));
            
            foreach(XmlNode docNode in childDocuments)
            {
                Dictionary<string, object> document = new Dictionary<string, object>();
                document.Add("ID", XmlHelper.GetIntAttrValue(docNode, TasksExportXmlConsts.IDTagName, 0));
                string sourceFileName = XmlHelper.GetStringAttrValue(docNode, TasksExportXmlConsts.DocSourceFileNameTagName, String.Empty);
                TaskDocumentType dt = (TaskDocumentType)XmlHelper.GetIntAttrValue(docNode, TasksExportXmlConsts.DocTypeTagName, 0); 
                document.Add("Тип документа", TaskUtils.TaskDocumentTypeToString(dt, Path.GetExtension(sourceFileName)));
                document.Add("Название", XmlHelper.GetStringAttrValue(docNode, TasksExportXmlConsts.DocNameTagName, string.Empty));
                document.Add("Принадлежность", TaskUtils.TaskDocumentOwnershipToString((TaskDocumentOwnership)XmlHelper.GetIntAttrValue(docNode, TasksExportXmlConsts.DocOwnershipTagName, 0)));
                document.Add("Тип файла", GetFileType((TaskDocumentType)XmlHelper.GetIntAttrValue(docNode, TasksExportXmlConsts.DocTypeTagName, 0)));
                document.Add("Комментарий", XmlHelper.GetStringAttrValue(docNode, TasksExportXmlConsts.DocDescriptionTagName, string.Empty));
                docParams.Add(document);
            }
            return docParams;
        }

        private static string GetFileType(TaskDocumentType documentType)
        {
            switch (documentType)
            {
                case TaskDocumentType.dtArbitraryDocument:
                case TaskDocumentType.dtDummyValue:
                    return "";
                case TaskDocumentType.dtDataCaptureList:
                case TaskDocumentType.dtInputForm:
                case TaskDocumentType.dtCalcSheet:
                case TaskDocumentType.dtPlanningSheet:
                case TaskDocumentType.dtReport:
                case TaskDocumentType.dtExcelDocument:
                    return ".xls";
                case TaskDocumentType.dtWordDocument:
                    return ".doc";
                case TaskDocumentType.dtMDXExpertDocument:
                    return ".exd";
            }
            return string.Empty;
        }

        /// <summary>
        /// устанвливает загловки для таблиц
        /// </summary>
        /// <param name="dict"></param>
        private static void SetTableHeader(Dictionary<string, object> dict)
        {
            int counter = 0;
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                SetTextAligment(Alignment.wdAlignParagraphCenter);
                SetText(kvp.Key);

                if (counter <= dict.Count - 1)
                {
                    GoTo(1, Direction.MoveRight, Units.wdCharacter);
                    counter++;
                }
            }
            GoTo(1, Direction.MoveRight, Units.wdCharacter);
        }

        private static int maxLevel;

        private static void SetDocumentHeader()
        {
            ReflectionHelper.CallMethod(selection, "HomeKey", 6);
            object contents = ReflectionHelper.GetProperty(activeDocument, "TablesOfContents");
            object cont = ReflectionHelper.CallMethod(contents, "Add",
                ReflectionHelper.GetProperty(selection, "Range"),
                true,
                1,
                maxLevel,
                true,
                "T",
                true,
                true,
                null,
                true,
                true,
                true
            );

            ReflectionHelper.SetProperty(cont, "TabLeader", 1);
            ReflectionHelper.SetProperty(contents, "Format", 0);
        }

        /// <summary>
        /// таблицу вордовскую заполняет данными
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dict"></param>
        private static void SetDataToTable(object table, Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                SetText(kvp.Value);
                GoTo(1, Direction.MoveRight, Units.wdCharacter);
            }
        }

        private static void BuildMSWordReport(IWorkplace workplace, XmlDocument xmlData, object app)
        {
            // получаем активный документ ворда
            //object wordDocument = ReflectionHelper.GetProperty(app, "ActiveDocument");

            // получаем минимальную дату
            XmlNodeList allTasks = xmlData.SelectNodes(String.Format("//{0}", TasksExportXmlConsts.TaskTagName));
            DateTime minDate = DateTime.Now;
            foreach (XmlNode task in allTasks)
            {
                DateTime curDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(task, TasksExportXmlConsts.TaskFromDateTagName, String.Empty));
                if (curDate < minDate)
                    minDate = curDate;
            }
            // получаем таблицу пользователей
            DataTable users = workplace.ActiveScheme.UsersManager.GetUsers();
            // получаем список задач верхнего уровня
            XmlNodeList rootTasks = xmlData.SelectNodes(String.Format("{0}/{1}/{2}", TasksExportXmlConsts.RootNodeTagName, TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            // оставляем место для заголовка
            if (reportKind == MSWordReportsKind.Full)
            {
                CreateNextLine();
                CreateNextLine();
            }
            for (int i = 0; i < rootTasks.Count; i++)
            {
                // рекурсивно обрабатываем каждую
                ProcessTask(rootTasks[i], app, i, 1, users, -1);
            }

            // делаем заголовок
            if (reportKind == MSWordReportsKind.Full)
                SetDocumentHeader();

            try
            {
                object rows = ReflectionHelper.GetProperty(selection, "Rows");
                ReflectionHelper.CallMethod(rows, "Delete");
            }
            catch { }
        }

        private static DataTable taskTypes;

        public enum MSWordReportsKind { Lite, Full }

        /// <summary>
        /// Метод экспорта выделенных задач в MSProject2003
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="ug">UltraGrid c выделенными задачами</param>
        public static void CreateTaskReport(IWorkplace workplace, UltraGrid ug, MSWordReportsKind repKind)
        {
            // есть-ли выделенные задачи?
            if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
            {
                // нет, выходим
                MessageBox.Show("Необходимо выделить нужные задачи", "Нет задач для экспорта", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                Application.DoEvents();

                // формируем список выделенных ID
                List<int> selectedID;
                UltraGridHelper.GetSelectedIDs(ug, out selectedID);

                // показываем прогресс
                workplace.OperationObj.Text = "Создание отчета MS Word";
                workplace.OperationObj.StartOperation();
                string errStr = String.Empty;
                // получаем XML документ с экспортируемыми данными
                XmlDocument xmlData = TasksExportHelper.GetTasksListXml(workplace, selectedID, TaskExportType.teIncludeChild, ref errStr);
                if (repKind == MSWordReportsKind.Full)
                {
                    TasksExportHelper.AppendTasksDocuments(workplace, xmlData, ref errStr, false, false);
                    TasksExportHelper.AppendTaskTypes(workplace, xmlData, ref errStr);
                    taskTypes = workplace.ActiveScheme.UsersManager.GetTasksTypes();
                }
                //xmlData.Save("d:\\test.xml");
                // если все прошло без ошибок - начинаем формирование отчета
                if ((errStr == null) || (errStr != String.Empty))
                {
                    MessageBox.Show("Произошла ошибка: " + errStr, "Экспорт завершился неудачей", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // сохраняем вид отчета, что бы не передавать параметром постоянно
                    reportKind = repKind;

                    // создаем приложение ворда
                    object app = GetWordObject();

                    wordApplication = app;

                    selection = ReflectionHelper.GetProperty(app, "Selection");
                    activeDocument = ReflectionHelper.GetProperty(app, "ActiveDocument");
                    // отключаем автообновление
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", false);
                    // ставим альбомный вид страницам
                    if (repKind == MSWordReportsKind.Lite)
                        SetLandscapeStyle();
                    // создаем отчет
                    BuildMSWordReport(workplace, xmlData, app);

                    //xmlData = null;

                    workplace.OperationObj.StopOperation();
                    // включаем автообновление
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", true);
                    // показываем приложение
                    ReflectionHelper.SetProperty(app, "Visible", true);
                    ReflectionHelper.SetProperty(app, "WindowState", 1);
                    object activeWindow = ReflectionHelper.GetProperty(app, "ActiveWindow");
                    ReflectionHelper.CallMethod(activeWindow, "Activate");
                }
            }
            finally
            {
                ClearObjects();
                GC.GetTotalMemory(true);
                workplace.OperationObj.StopOperation();
            }
        }

        static void ClearObjects()
        {
            if (taskTypes != null)
            {
                taskTypes.Clear();
                taskTypes = null;
            }
            ComHelper.ReleaseComReference(ref selection);
            ComHelper.ReleaseComReference(ref table);
            ComHelper.ReleaseComReference(ref activeDocument);
            ReleaseNormalDot(wordApplication);
            ComHelper.ReleaseComReference(ref wordApplication);
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
        /// устанавливает лист ворда в альбомный формат
        /// </summary>
        private static void SetLandscapeStyle()
        {
            object pageSetup = ReflectionHelper.GetProperty(activeDocument, "PageSetup");
            ReflectionHelper.SetProperty(pageSetup, "Orientation", 1);
        }

        private static void SetBoldFont()
        {
            object font = ReflectionHelper.GetProperty(selection, "Font");
            ReflectionHelper.SetProperty(font, "Bold", 9999998);
        }

        private static void SetBoldHeader(string header, string message)
        {
            SetBoldFont();
            SetText(header);
            SetBoldFont();
            SetText(message);
        }

        /// <summary>
        /// Создание объекта MS Office
        /// </summary>
        /// <returns></returns>
        public static object GetWordObject()
        {
            WordApplication wordApp = OfficeHelper.CreateWordApplication();
            // получаем объект "Selection"
            selection = wordApp.Selection;
            object documents = wordApp.Documents;
            // добавляем новый пустой документ
            ReflectionHelper.CallMethod(documents, "Add",
                    Missing.Value,	// var Template: OleVariant; 
                    Missing.Value,	// var NewTemplate: OleVariant; 
                    Missing.Value,	// var DocumentType: OleVariant; 
                    true			// var Visible: OleVariant
            );
            // создаем 
            activeDocument = wordApp.ActiveDocument;
            // возвращаем указатель для дальнейшей работы
            return wordApp.OfficeApp;
        }
    }
}
