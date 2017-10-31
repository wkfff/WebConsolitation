using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Krista.FM.Common;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.OfficePluginServices;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Common.Xml;
using Krista.FM.Common.TaskDocuments;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public sealed class TasksExportHelper
    {
        #region Общие методы
        /// <summary>
        /// Добавить информацию о задаче в XmlDocument
        /// </summary>
        /// <param name="taskRow">DataRow из таблицы с информацией о задаче</param>
        /// <param name="parentNode">родительский узел (tasks)</param>
        /// <returns>созданный узел</returns>
        private static XmlNode AppendTaskInfoToXml(DataRow taskRow, XmlNode parentNode)
        {
            return XmlHelper.AddChildNode(
                parentNode,
                TasksExportXmlConsts.TaskTagName,
                new string[2] { TasksExportXmlConsts.IDTagName, Convert.ToString(taskRow["ID"]) },
                new string[2] { TasksExportXmlConsts.TaskHeadlineTagName, Convert.ToString(taskRow["Headline"]) },
                new string[2] { TasksExportXmlConsts.TaskJobTagName, Convert.ToString(taskRow["Job"]) },
                new string[2] { TasksExportXmlConsts.TaskDescriptionTagName, Convert.ToString(taskRow["Description"]) },
                new string[2] { TasksExportXmlConsts.TaskStateTagName, Convert.ToString(taskRow["State"]) },
                new string[2] { TasksExportXmlConsts.TaskFromDateTagName,  Convert.ToString(taskRow["FromDate"])},
                new string[2] { TasksExportXmlConsts.TaskToDateTagName,  Convert.ToString(taskRow["ToDate"])},
                new string[2] { TasksExportXmlConsts.TaskOwnerTagName, Convert.ToString(taskRow["Owner"]) },
                new string[2] { TasksExportXmlConsts.TaskDoerTagName, Convert.ToString(taskRow["Doer"]) },
                new string[2] { TasksExportXmlConsts.TaskCuratorTagName, Convert.ToString(taskRow["Curator"]) },
                new string[2] { TasksExportXmlConsts.TaskTypeTagName, Convert.ToString(taskRow["RefTasksTypes"]) },
                new string[2] { TasksExportXmlConsts.TaskLockByUser, Convert.ToString(taskRow["LockByUser"]) },
                new string[2] { TasksExportXmlConsts.TaskLockedUserName, Convert.ToString(taskRow["LockedUserName"]) }
            );
        }


        private static bool AppendTaskConstParamToXml(DataRow row, TaskParameterType paramType, 
            XmlNode parentNode, ref string errStr)
        {
            try
            {
                XmlNode nd;
                // сначала добавим основные параметры
                if (paramType == TaskParameterType.taskConst)
                {
                    nd = XmlHelper.AddChildNode(
                        parentNode, 
                        TasksExportXmlConsts.TaskConstTagName,
                        new string[2] { TasksExportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsNameTagName, Convert.ToString(row["Name"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsDescriptionTagName, Convert.ToString(row["Description"]) }
                    );
                }
                else
                {
                    nd = XmlHelper.AddChildNode(
                        parentNode, 
                        TasksExportXmlConsts.TaskParamTagName,
                        new string[2] { TasksExportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsNameTagName, Convert.ToString(row["Name"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsDescriptionTagName, Convert.ToString(row["Description"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsAllowMultiSelectTagName, Convert.ToString(row["AllowMultiSelect"]) },
                        new string[2] { TasksExportXmlConsts.TaskConstsParamsDimensionTagName, Convert.ToString(row["Dimension"]) }
                    );
                }
                #region Значения большие, их надо сжимать
                // если значение пустое - ничего не делаем
                if (row["ParamValues"] == DBNull.Value)
                    return true;
                XmlElement valuesNode = nd.OwnerDocument.CreateElement(TasksExportXmlConsts.TaskConstsParamsValuesTagName);
                nd.AppendChild(valuesNode);
                // получаем данные документа
                string elemDataStr = Convert.ToString(row["ParamValues"]);
                byte[] elemData = Encoding.GetEncoding(1251).GetBytes(elemDataStr);
                // CRC и Size
                // считаем црц и помещаем его в аттрибут
                uint crc32 = CRCHelper.CRC32(elemData, 0, elemData.Length);
                XmlAttribute crcAttr = valuesNode.OwnerDocument.CreateAttribute(TasksExportXmlConsts.CRCTagName);
                crcAttr.Value = crc32.ToString();
                valuesNode.Attributes.Append(crcAttr);
                // на всякий случай запишем и размер несжатого файла
                XmlAttribute sizeAttr = valuesNode.OwnerDocument.CreateAttribute(TasksExportXmlConsts.SizeTagName);
                sizeAttr.Value = elemData.Length.ToString();
                valuesNode.Attributes.Append(sizeAttr);
                // создаем стрим для архивации
                MemoryStream baseStream = new MemoryStream();
                // создаем архивирующий стрим и сжимаем данные документа
                DeflateStream compressedStream = new DeflateStream(baseStream, CompressionMode.Compress, true);
                compressedStream.Write(elemData, 0, elemData.Length);
                compressedStream.Close();
                // преобразуем сжатые данные в строку
                baseStream.Position = 0;
                // пишем цдату к узлу
                XmlHelper.AppendCDataSection(valuesNode, Convert.ToBase64String(baseStream.ToArray(), Base64FormattingOptions.InsertLineBreaks));
                baseStream.Close();
                GC.Collect();
                #endregion

                return true;
            }
            catch (Exception e)
            {
                errStr = e.Message;
                return false;
            }
        }

        private static bool AppendTaskConstsParamsToXml(IWorkplace workplace, DataRow taskRow, 
            XmlNode parentNode, bool includeParent, ref string errStr)
        {
            int id = Convert.ToInt32(taskRow["ID"]);
            DataTable consts = new DataTable("Consts");
            DataTable parameters = new DataTable("Params");
            workplace.ActiveScheme.TaskManager.Tasks.GetTaskConstsParams(id, includeParent, ref consts, ref parameters);
            // если есть константы - добавляем
            XmlNode nd;
            if (consts.Rows.Count > 0)
            {
                nd = parentNode.OwnerDocument.CreateElement(TasksExportXmlConsts.TaskConstsTagName);
                parentNode.AppendChild(nd);
                foreach (DataRow row in consts.Rows)
                {
                    if (!AppendTaskConstParamToXml(row, TaskParameterType.taskConst, nd, ref errStr))
                        return false;
                }
            }
            // если есть параметры - добавляем
            if (parameters.Rows.Count > 0)
            {
                nd = parentNode.OwnerDocument.CreateElement(TasksExportXmlConsts.TaskParamsTagName);
                parentNode.AppendChild(nd);
                foreach (DataRow row in parameters.Rows)
                {
                    if (!AppendTaskConstParamToXml(row, TaskParameterType.taskParameter, nd, ref errStr))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Рекурсивный метод обработки задачи и всех ее подчиненных задач
        /// </summary>
        /// <param name="workplace">Workplace</param>
        /// <param name="allTasks">DataTable с информацией обо всех задачах</param>
        /// <param name="taskRow">DataRow обрабатываемой задачи</param>
        /// <param name="parentNode">родительский узел (tasks)</param>
        /// <param name="exportType">тип экпорта (нужно ли обрабатывать дочерние)</param>
        /// <param name="isRoot"></param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <returns>результат операции</returns>
        private static bool AddTaskToExportList(IWorkplace workplace, DataTable allTasks, 
            DataRow taskRow, XmlNode parentNode, TaskExportType exportType, bool isRoot, 
            ref string errStr)
        {
            int taskID = Convert.ToInt32(taskRow["ID"]);
            // проверяем видимость задачи пользователю. Фантомные задачи экспортировать нельзя
            TaskVisibleInNavigation vsbl = (TaskVisibleInNavigation)Convert.ToInt32(taskRow["visible"]);
            if (vsbl != TaskVisibleInNavigation.tvVisible)
            {
                errStr = String.Format("В соответствии с настройками прав доступа задача {0} не может быть экспортирована. Исключите ее из списка или выберите другой режим экспорта.", taskID);
                return false;
            }
            // сама задача
            XmlNode taskNode = AppendTaskInfoToXml(taskRow, parentNode);
            // константы и параметры
            if (!AppendTaskConstsParamsToXml(workplace, taskRow, taskNode, isRoot, ref errStr))
                return false;

            // если нужно экспортировать и дочерние...
            if (exportType == TaskExportType.teIncludeChild)
            {
                // выполняем процедуру рекурсивно для всех потомков
                string childFilter = String.Format("REFTASKS = {0}", taskID);
                DataRow[] childTasks = allTasks.Select(childFilter);
                if (childTasks.Length > 0)
                {
                    XmlNode childTasksNode = XmlHelper.AddChildNode(taskNode, TasksExportXmlConsts.TasksTagName);
                    foreach (DataRow childTask in childTasks)
                    {
                        // если где-нибудь произошла ошибка - выходим
                        if (!AddTaskToExportList(workplace, allTasks, childTask, childTasksNode, exportType, false, ref errStr))
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Метод для получения иерархического XML экпортируемых задач (без документов)
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="selectedID">список выделенных задач верхнего уровня</param>
        /// <param name="exportType">тип экспорта (нужно ли обрабатывать дочерние задачи)</param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <returns>результат операции</returns>
        public static XmlDocument GetTasksListXml(IWorkplace workplace, List<int> selectedID, TaskExportType exportType, ref string errStr)
        {
            // получаем все задачи доступные пользователю
            DataTable tasksList = workplace.ActiveScheme.TaskManager.Tasks.BeginExport();
            try
            {
                // формируем фильтр на выбранные задачи
                StringBuilder sb = new StringBuilder("ID in (");
                for (int i = 0; i < selectedID.Count; i++)
                {
                    sb.Append(selectedID[i]);
                    if (i < selectedID.Count - 1)
                        sb.Append(", ");
                }
                sb.Append(')');
                string filter = sb.ToString();

                // получаем выбранные задачи
                DataRow[] selectedTasks = tasksList.Select(filter);
                // проверяем соотвествие запрошенного количества задач фактическому
                if (selectedTasks.Length != selectedID.Count)
                {
                    errStr = "Обнаружено несоответствие между запрошенным и разрешенным набором задач. Обновите список задач и выполните экспорт заново.";
                    return null;
                }
                // создаем хмл-документ для экспорта
                XmlDocument tasksListXML = new XmlDocument();

                // создаем аттрибут для версии
                XmlNode rootNode = tasksListXML.CreateElement(TasksExportXmlConsts.RootNodeTagName);
                tasksListXML.AppendChild(rootNode);
                XmlAttribute versionAttr = tasksListXML.CreateAttribute(TasksExportXmlConsts.VersionTagName);
                versionAttr.Value = AppVersionControl.GetAssemblyVersion(Assembly.GetExecutingAssembly());
                rootNode.Attributes.Append(versionAttr);

                // создаем родительский узел для выбранных задач
                XmlNode rootTasksNode = tasksListXML.CreateElement(TasksExportXmlConsts.TasksTagName);
                rootNode.AppendChild(rootTasksNode);

                // добавляем каждую из выбранных задач в список для экспорта
                foreach (DataRow row in selectedTasks)
                {
                    // если где-нибудь произошла ошибка - выходим
                    if (!AddTaskToExportList(workplace, tasksList, row, rootTasksNode, exportType, true, ref errStr))
                        return null;
                }
                // возвращаем список с задачами для дальнейшей обработки
                tasksListXML.Normalize();
                return tasksListXML;
            }
            finally
            {
                workplace.ActiveScheme.TaskManager.Tasks.EndExport();
            }
        }

        /// <summary>
        /// Получить список задач всех уровней
        /// </summary>
        /// <param name="doc">XML Document c задачами</param>
        /// <returns>список задач</returns>
        private static XmlNodeList GetAllTasks(XmlDocument doc)
        {
            return doc.SelectNodes(String.Format("//{0}", TasksExportXmlConsts.TaskTagName));
        }
        #endregion

        #region Методы экспорта в XML
        /// <summary>
        /// Сжимает данные документа и записывает их в секцию CDATA родительского узла
        /// </summary>
        /// <param name="row">DataRow c информацией о документе</param>
        /// <param name="docNode">родительский узел (документа)</param>
        private static void AppendCompressedDocumentData(DataRow row, XmlNode docNode)
        {
            // получаем данные документа
            byte[] documentData = (byte[])row["Document"];
            // считаем црц и помещаем его в аттрибут
            uint crc32 = CRCHelper.CRC32(documentData, 0, documentData.Length);
            XmlAttribute crcAttr = docNode.OwnerDocument.CreateAttribute(TasksExportXmlConsts.CRCTagName);
            crcAttr.Value = crc32.ToString();
            docNode.Attributes.Append(crcAttr);
            // на всякий случай запишем и размер несжатого файла
            XmlAttribute sizeAttr = docNode.OwnerDocument.CreateAttribute(TasksExportXmlConsts.SizeTagName);
            sizeAttr.Value = documentData.Length.ToString();
            docNode.Attributes.Append(sizeAttr);
            // создаем стрим для архивации
            MemoryStream baseStream = new MemoryStream();
            // создаем архивирующий стрим и сжимаем данные документа
            DeflateStream compressedStream = new DeflateStream(baseStream, CompressionMode.Compress, true);
            compressedStream.Write(documentData, 0, documentData.Length);
            compressedStream.Close();
            // преобразуем сжатые данные в строку
            baseStream.Position = 0;
            // пишем цдату к узлу
            XmlHelper.AppendCDataSection(docNode, Convert.ToBase64String(baseStream.ToArray(), Base64FormattingOptions.InsertLineBreaks));
            baseStream.Close();
            GC.Collect();
        }

        private static string tasksFilter = String.Format("{0}/{1}", TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName);
        private static string taskDocumentsFilter = "Select ID from {0} where {1} = {2} order by ID";
        private static string taskDocumentInfoFilter = "Select Name, SourceFileName, DocumentType, Description, Ownership  from {0} where ID = {1}";
        private static string taskDocumentDataFilter = "Select Document  from {0} where ID = {1}";
        private static string taskDocumentsMainTableName = "Documents";
        private static string taskDocumentsTempTableName = "DocumentsTemp";
        private static string taskDocumentsMainRefName = "RefTasks";
        private static string taskDocumentsTempRefName = "RefTasksTemp";
        private static string documentCompressCaption = "Сжатие документа: {0}";

        /// <summary>
        /// Рекурсивный метод обработки документов текущей и дочерних задач
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="db">интерфейс IDatabase</param>
        /// <param name="taskNode">узел задачи</param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <param name="appendData"></param>
        /// <param name="showProgress"></param>
        /// <returns>результат операции</returns>
        private static bool ProcessTaskDocument(IWorkplace workplace, IDatabase db, XmlNode taskNode, 
            ref string errStr, bool appendData, bool showProgress)
        {
            try
            {
                if (showProgress)
                    workplace.ProgressObj.Position++;
                // получаем ID задачи
                int taskID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.IDTagName, 0);
                // определяем откуда брать документы - из основной таблицы или из кэша
                int lockByUser = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskLockByUser, -1);
				bool useCash = ClientAuthentication.UserID == lockByUser;
                // получаем документы 
                string queryStr;
                if (useCash)
                    queryStr = String.Format(taskDocumentsFilter, taskDocumentsTempTableName, taskDocumentsTempRefName, taskID);
                else
                    queryStr = String.Format(taskDocumentsFilter, taskDocumentsMainTableName, taskDocumentsMainRefName, taskID);
                DataTable documents = (DataTable)db.ExecQuery(queryStr, QueryResultTypes.DataTable);
                // если документы есть - добавляем их
                if ((documents != null) && (documents.Rows.Count > 0))
                {
                    // создаем родительский узел для всех документов
                    XmlNode rootDocumentsNode = taskNode.OwnerDocument.CreateElement(TasksExportXmlConsts.DocumentsTagName);
                    taskNode.InsertBefore(rootDocumentsNode, taskNode.FirstChild);
                    // добавляем каждый документ
                    foreach (DataRow row in documents.Rows)
                    {
                        int docID = Convert.ToInt32(row["ID"]);
                        if (useCash)
                            queryStr = String.Format(taskDocumentInfoFilter, taskDocumentsTempTableName, docID);
                        else
                            queryStr = String.Format(taskDocumentInfoFilter, taskDocumentsMainTableName, docID);

                        DataTable document = (DataTable)db.ExecQuery(queryStr, QueryResultTypes.DataTable);
                        DataRow docRow = document.Rows[0];
                        string docName = Convert.ToString(docRow["Name"]);
                        if (showProgress)
                            workplace.ProgressObj.Text = String.Format(documentCompressCaption, docName);
                        // основные параметры
                        XmlNode docNode = XmlHelper.AddChildNode(
                        rootDocumentsNode,
                        TasksExportXmlConsts.DocumentTagName,
                            new string[2] { TasksExportXmlConsts.IDTagName, docID.ToString() },
                            new string[2] { TasksExportXmlConsts.DocNameTagName, docName },
                            new string[2] { TasksExportXmlConsts.DocSourceFileNameTagName, Convert.ToString(docRow["SourceFileName"]) },
                            new string[2] { TasksExportXmlConsts.DocTypeTagName, Convert.ToString(docRow["DocumentType"]) },
                            new string[2] { TasksExportXmlConsts.DocDescriptionTagName, Convert.ToString(docRow["Description"]) },
                            new string[2] { TasksExportXmlConsts.DocOwnershipTagName, Convert.ToString(docRow["Ownership"]) }
                        );
                        // данные дописываем только если нужно
                        if (appendData)
                        {
                            if (useCash)
                                queryStr = String.Format(taskDocumentDataFilter, taskDocumentsTempTableName, docID);
                            else
                                queryStr = String.Format(taskDocumentDataFilter, taskDocumentsMainTableName, docID);

                            DataTable docDataTable = (DataTable)db.ExecQuery(queryStr, QueryResultTypes.DataTable);
                            DataRow docDataRow = docDataTable.Rows[0];

                            // данные документа
                            XmlNode docData = XmlHelper.AddChildNode(docNode, TasksExportXmlConsts.DocDataTagName);
                            AppendCompressedDocumentData(docDataRow, docData);
                        }
                    }
                    GC.Collect();
                    Application.DoEvents();
                }
                // теперь рекурсия по потомкам
                XmlNodeList childTasks = taskNode.SelectNodes(tasksFilter);
                if ((childTasks != null) && (childTasks.Count > 0))
                {
                    for (int i = 0; i < childTasks.Count; i++)
                    {
                        if (!ProcessTaskDocument(workplace, db, childTasks[i], ref errStr, appendData, showProgress))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                errStr = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Главный метод дополнения XML задач документами
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="tasksList">XML экспорта</param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <param name="appendData"></param>
        /// <param name="showProgress"></param>
        /// <returns>результат операции</returns>
        public static bool AppendTasksDocuments(IWorkplace workplace, XmlDocument tasksList, 
            ref string errStr, bool appendData, bool showProgress)
        {
            IDatabase db = null;
            try
            {
                // получаем объект для доступа к базе
                db = workplace.ActiveScheme.SchemeDWH.DB;
                // устанавливаем прогресс по общему количеству задач
                if (showProgress)
                    workplace.ProgressObj.MaxProgress = GetAllTasks(tasksList).Count;
                // получаем список задач верхнего уровня
                XmlNodeList tasks = tasksList.FirstChild.SelectNodes(tasksFilter);
                // для каждой задачи и всех ее потомков вызываем метод добавления документов
                for (int i = 0; i < tasks.Count; i++)
                {
                    // если хотя бы один документ не добавился - прекращаем экспорт
                    if (!ProcessTaskDocument(workplace, db, tasks[i], ref errStr, appendData, showProgress))
                        return false;
                }
                return true;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Главный метод дополнения XML экспорта данными об используемых типах задач
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="tasksListXml">XML экспорта</param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <returns>результат операции</returns>
        public static bool AppendTaskTypes(IWorkplace workplace, XmlDocument tasksListXml, ref string errStr)
        {
            try
            {
                // получаем все задачи в один список
                XmlNodeList allTasks = GetAllTasks(tasksListXml);
                List<string> usedTaskTypes = new List<string>();
                // формируем список с уникальными ID типов
                foreach (XmlNode task in allTasks)
                {
                    string taskTypeID = XmlHelper.GetStringAttrValue(task, TasksExportXmlConsts.TaskTypeTagName, String.Empty);
                    if (!usedTaskTypes.Contains(taskTypeID))
                        usedTaskTypes.Add(taskTypeID);
                }
                // получаем все содержимое справочника типов задач
                DataTable taskTypesTable = workplace.ActiveScheme.UsersManager.GetTasksTypes();
                // формируем фильтр на ID по созданному ранее списку
                string usedIDs = String.Join(", ", usedTaskTypes.ToArray());
                usedIDs = String.Format("ID in ({0})", usedIDs);
                // фильтруем справочник типов задач
                DataRow[] usedTaskTypesRows = taskTypesTable.Select(usedIDs);
                // создаем секцию и добавляем необходмую информацию по всем используемым типам
                XmlNode taskTypesNode = tasksListXml.CreateElement(TasksExportXmlConsts.TaskTypesTagName);
                tasksListXml.FirstChild.InsertBefore(taskTypesNode, tasksListXml.FirstChild.FirstChild);
                foreach (DataRow row in usedTaskTypesRows)
                {
                    XmlHelper.AddChildNode(
                        taskTypesNode,
                        TasksExportXmlConsts.TaskTypeTagName,
                        new string[2] { TasksExportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                        new string[2] { TasksExportXmlConsts.TaskTypeNameTagName, Convert.ToString(row["Name"]) },
                        new string[2] { TasksExportXmlConsts.TaskTypeCodeTagName, Convert.ToString(row["Code"]) },
                        new string[2] { TasksExportXmlConsts.TaskTypeTaskTypeTagName, Convert.ToString(row["TaskType"]) },
                        new string[2] { TasksExportXmlConsts.TaskTypeDescriptionTagName, Convert.ToString(row["Description"]) }
                    );
                }
                return true;
            }
            catch (Exception e)
            {
                errStr = e.Message;
                return false;
            }
        }

        public static XmlDocument InnerExportTasks(IWorkplace workplace, List<int> selectedID, TaskExportType exportType, ref string errStr)
        {
            return InnerExportTasks(workplace, selectedID, exportType, ref errStr, true);
        }

        /// <summary>
        /// Основной метод формирования полного XML экпорта
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="selectedID">список выбранных ID задач верхнего уровня</param>
        /// <param name="exportType">тип экспорта</param>
        /// <param name="errStr">сообщение об ошибке</param>
        /// <param name="showProgress"></param>
        /// <returns>результат операции</returns>
        public static XmlDocument InnerExportTasks(IWorkplace workplace, List<int> selectedID, 
            TaskExportType exportType, ref string errStr, bool showProgress)
        {
            // Получаем список с основными параметрами задач
            if (showProgress)
                workplace.ProgressObj.Text = "Построение списка задач";
            XmlDocument tasksListXml = GetTasksListXml(workplace, selectedID, exportType, ref errStr);
            // если список задач получить не удалось - ничего более не делаем
            if (tasksListXml == null)
                return null;
            // делаем секцию с подмножеством справочника типов задач
            if (showProgress)
                workplace.ProgressObj.Text = "Построение списка типов задач";
            if (!AppendTaskTypes(workplace, tasksListXml, ref errStr))
                return null;
            // дополняем его документами
            if (!AppendTasksDocuments(workplace, tasksListXml, ref errStr, true, showProgress))
                return null;
            return tasksListXml;
        }

        /// <summary>
        /// Экспорт выбранных задач в XML
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="ug">грид в котором выбраны необходимые задачи</param>
        public static void ExportTasksNew(IWorkplace workplace, UltraGrid ug)
        {
            // есть-ли выделенные задачи?
            if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
            {
                MessageBox.Show("Необходимо выделить нужные задачи. Нет задач для экспорта", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // есть-ли у выделенных задач подчиненные?
            int hasChildCount = 0;
            int byCurrentUserLockCount = 0;
            string currentUserName = workplace.ActiveScheme.UsersManager.GetCurrentUserName();
            foreach (UltraGridRow row in ug.Selected.Rows)
            {
                if (row.HasChild())
                    hasChildCount++;

                if (string.Compare(row.Cells["LockedUserName"].Value.ToString(), currentUserName) == 0)
                    byCurrentUserLockCount++;
            }

            if (byCurrentUserLockCount > 0)
            {
                if (MessageBox.Show(workplace.WindowHandle,
                    "Одна или несколько задач из выделенных находятся в состоянии редактирования. Продолжить экспорт задач?",
                    "Экспорт задач", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            // в зависимости от наличия дочерних выбираем режим экспорта
            TaskExportType exportType = TaskExportType.teSelectedOnly;
            if (hasChildCount > 0)
                exportType = FormSelectExportMode.SelectTasksExportType();
            // если нажали отмену - выходим
            if (exportType == TaskExportType.teUndefined)
            {
                MessageBox.Show("Операция экспорта задач отменена", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Имя файла - название первой выбранной задачи
            String defFileName = String.Empty;
            try
            {
                defFileName = Convert.ToString(UltraGridHelper.GetRowCells(ug.Selected.Rows[0]).Cells["HeadLine"].Value);
            }
            catch { }
            // получаем корректное название файла
            // двойные кавычки заменяем одинарными
            defFileName = defFileName.Replace(Convert.ToChar(34), Convert.ToChar(39));
            defFileName = ExportImportHelper.GetCorrectFileName(defFileName);
            // выбираем имя файла
            SaveFileDialog sd = new SaveFileDialog();
            sd.DefaultExt = "xml";
            sd.Filter = "XML файлы|*.xml";
            sd.OverwritePrompt = false;
            sd.CreatePrompt = false;
            sd.FileName = defFileName + ".xml";
            if ((sd.ShowDialog() != DialogResult.OK) || (sd.FileName == String.Empty))
            {
                MessageBox.Show("Операция экспорта задач отменена", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string fileName = sd.FileName;
            Application.DoEvents();

            // формируем список выделенных ID
            List<int> selectedID;
            UltraGridHelper.GetSelectedIDs(ug, out selectedID);
            // показываем прогресс
            workplace.OperationObj.Text = "Экспорт задач";
            workplace.OperationObj.StartOperation();
            FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            try
            {
                
                workplace.ActiveScheme.TaskManager.Tasks.ExportTask(stream, GetTasksList(exportType, selectedID, ug));
            }
            finally
            {
                stream.Close();
                workplace.OperationObj.StopOperation();
            }
        }

        private static int[] GetTasksList(TaskExportType taskExportType, List<int> selectedID, UltraGrid grid)
        {
            if (taskExportType == TaskExportType.teSelectedOnly)
                return selectedID.ToArray();
            return UltraGridHelper.GetSelectedAndChildsIDs(grid);
        }

        /// <summary>
        /// Экспорт выбранных задач в XML
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="ug">грид в котором выбраны необходимые задачи</param>
        public static void ExportTasks(IWorkplace workplace, UltraGrid ug)
        {
            // есть-ли выделенные задачи?
            if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
            {
                MessageBox.Show("Необходимо выделить нужные задачи. Нет задач для экспорта", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // есть-ли у выделенных задач подчиненные?
            int hasChildCount = 0;
            int byCurrentUserLockCount = 0;
            string currentUserName = workplace.ActiveScheme.UsersManager.GetCurrentUserName();
            foreach (UltraGridRow row in ug.Selected.Rows)
            {
                if (row.HasChild())
                    hasChildCount++;

                if (string.Compare(row.Cells["LockedUserName"].Value.ToString(), currentUserName) == 0)
                    byCurrentUserLockCount++;
            }

            if (byCurrentUserLockCount > 0)
            {
                if (MessageBox.Show(workplace.WindowHandle,
                    "Одна или несколько задач из выделенных находятся в состоянии редактирования. Продолжить экспорт задач?",
                    "Экспорт задач", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            // в зависимости от наличия дочерних выбираем режим экспорта
            TaskExportType exportType = TaskExportType.teSelectedOnly;
            if (hasChildCount > 0)
                exportType = FormSelectExportMode.SelectTasksExportType();
            // если нажали отмену - выходим
            if (exportType == TaskExportType.teUndefined)
            {
                MessageBox.Show("Операция экспорта задач отменена", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Имя файла - название первой выбранной задачи
            String defFileName = String.Empty;
            try
            {
                defFileName = Convert.ToString(UltraGridHelper.GetRowCells(ug.Selected.Rows[0]).Cells["HeadLine"].Value);
            }
            catch { }
            // получаем корректное название файла
            // двойные кавычки заменяем одинарными
            defFileName = defFileName.Replace(Convert.ToChar(34), Convert.ToChar(39));
            defFileName = ExportImportHelper.GetCorrectFileName(defFileName);
            // выбираем имя файла
            SaveFileDialog sd = new SaveFileDialog();
            sd.DefaultExt = "xml";
            sd.Filter = "XML файлы|*.xml";
            sd.OverwritePrompt = false;
            sd.CreatePrompt = false;
            sd.FileName = defFileName + ".xml";
            if ((sd.ShowDialog() != DialogResult.OK) || (sd.FileName == String.Empty))
            {
                MessageBox.Show("Операция экспорта задач отменена", "Экспорт задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string fileName = sd.FileName;
            Application.DoEvents();

            // формируем список выделенных ID
            List<int> selectedID;
            UltraGridHelper.GetSelectedIDs(ug, out selectedID);

            // показываем прогресс
            workplace.ProgressObj.Caption = "Экспорт задач";
            workplace.ProgressObj.Text = String.Empty;
            workplace.ProgressObj.StartProgress();
            //int startTime = Environment.TickCount;
            try
            {
                string errStr = String.Empty;
                // получаем XML документ с экспортируемыми данными
                XmlDocument xmlData = InnerExportTasks(workplace, selectedID, exportType, ref errStr);
                // если все прошло без ошибок - сохраняем файл
                if ((errStr == null) || (errStr != String.Empty))
                {
                    MessageBox.Show("Произошла ошибка: " + errStr, "Экспорт завершился неудачей", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    workplace.ProgressObj.Text = "Сохранение документа";
                    XmlHelper.Save(xmlData, fileName);
                    XmlHelper.ClearDomDocument(ref xmlData);
                    workplace.ProgressObj.StopProgress();
                    //int elapsed = Environment.TickCount - startTime;
                    //MessageBox.Show(String.Format("Экспорт успешно завершен. Затрачено {0} ms.", elapsed.ToString()));
                }
            }
            finally
            {
                workplace.ProgressObj.StopProgress();
                GC.Collect();
            }
        }
        #endregion

        #region Методы импорта из XML
        private static void ImportTaskTypesSection(IWorkplace workplace, XmlDocument importXml, ref DataTable importedTasksTypesIds, bool showProgress)
        {
            if (showProgress)
                workplace.ProgressObj.Text = "Синхронизация справочника типов задач";

            string filter = String.Format("{0}/{1}/{2}", TasksExportXmlConsts.RootNodeTagName, TasksExportXmlConsts.TaskTypesTagName, TasksExportXmlConsts.TaskTypeTagName);
            XmlNodeList importedTaskTypes = importXml.SelectNodes(filter);
            if (importedTaskTypes == null)
                throw new Exception("Не найдена секция справочника типов задач");
            // обрабатываем каждый из импортируемых типов
            foreach (XmlNode taskType in importedTaskTypes)
            {
                // получаем параметры экпортируемого типа
                int taskTypeID = XmlHelper.GetIntAttrValue(taskType, TasksExportXmlConsts.IDTagName, 0);
                int taskTypeTaskType = XmlHelper.GetIntAttrValue(taskType, TasksExportXmlConsts.TaskTypeTaskTypeTagName,
                                                                 0);
                int taskTypeCode = XmlHelper.GetIntAttrValue(taskType, TasksExportXmlConsts.TaskTypeCodeTagName, 0);
                string taskTypeName = XmlHelper.GetStringAttrValue(taskType, TasksExportXmlConsts.TaskTypeNameTagName,
                                                                   String.Empty);
                string taskTypeDescription = XmlHelper.GetStringAttrValue(taskType,
                                                                          TasksExportXmlConsts.
                                                                              TaskTypeDescriptionTagName, String.Empty);
                if (importedTasksTypesIds == null)
                    importedTasksTypesIds = workplace.ActiveScheme.TaskManager.Tasks.ImportTaskTypes(taskTypeID,
                    taskTypeTaskType, taskTypeCode, taskTypeName, taskTypeDescription);
                else
                    importedTasksTypesIds.Merge(workplace.ActiveScheme.TaskManager.Tasks.ImportTaskTypes(taskTypeID,
                        taskTypeTaskType, taskTypeCode, taskTypeName, taskTypeDescription));
            }
        }

        private static byte[] GetUncompressData(XmlNode mainNode, string dataTagName)
        {
            XmlNode cDataNode = mainNode.SelectSingleNode(dataTagName);
            if (cDataNode == null)
                return null;
            cDataNode = cDataNode.FirstChild;
            if (cDataNode == null)
                return null;

            byte[] compressedBytes = Convert.FromBase64String(cDataNode.InnerText);

            MemoryStream baseStream = new MemoryStream();
            baseStream.Write(compressedBytes, 0, compressedBytes.Length);
            baseStream.Position = 0;
            // создаем архивирующий стрим и сжимаем данные документа
            DeflateStream uncompressedStream = new DeflateStream(baseStream, CompressionMode.Decompress, true);
            int size = XmlHelper.GetIntAttrValue(mainNode.FirstChild, TasksExportXmlConsts.SizeTagName, 0);
            // преобразуем сжатые данные в строку
            byte[] uncompressedBytes = new byte[size];
            uncompressedStream.Read(uncompressedBytes, 0, uncompressedBytes.Length);
            uncompressedStream.Close();
            baseStream.Close();
            // проверяем СРС
            uint newCRC = CRCHelper.CRC32(uncompressedBytes, 0, uncompressedBytes.Length);
            string oldCRCStr = XmlHelper.GetStringAttrValue(mainNode.FirstChild, TasksExportXmlConsts.CRCTagName, String.Empty);
            uint oldCRC = Convert.ToUInt32(oldCRCStr);
            if (newCRC != oldCRC)
                throw new Exception("Элемент данных поврежден. Контрольные суммы не совпадают");
            return uncompressedBytes;
        }

/*
        private static void SetPlanningSheetSettings(IWorkplace workplace, 
            ExcelHelper excelHelper, WordHelper wordHelper, 
            ref object excelObject, ref object wordObject, 
            ref IFMPlanningExtension excelPlanningItf, ref IFMPlanningExtension wordPlanningItf,
            ref int excelPlanningDocumentsProcessed, ref int wordPlanningDocumentsProcessed,
            ref byte[] fileData, string taskHeadLine, string taskID, 
            string documentName, string documentID, string sourceFileName)
        {
            if ((excelHelper.IsApplicableFile(sourceFileName)) && (!ExcelPluginService.PluginInstalled))
                return;

            if ((wordHelper.IsApplicableFile(sourceFileName)) && (!WordPluginService.PluginInstalled))
                return;

            // получаем имя временного файла
            string tempFilePath = DocumentsHelper.GetLocalDocumentName(
                Convert.ToInt32(taskID), Convert.ToInt32(documentID), documentName, sourceFileName);

            try
            {
                // создаем файл и пишем в него данные листа
                FileStream fs = File.Create(tempFilePath);
                fs.Position = 0;
                fs.Write(fileData, 0, fileData.Length);
                fs.Flush();
                fs.Close();
                if (excelHelper.IsApplicableFile(tempFilePath))
                {
                    new TaskDocumentHelper(excelHelper).OpenDocumentFromTask(
                        tempFilePath,
                        false,
                        true,//false,
                        true,
                        workplace.ActiveScheme,
                        taskHeadLine,
                        taskID,
                        documentName,
                        documentID,
                        workplace.ActiveScheme.UsersManager.GetCurrentUserName(),
                        -1,
                        null,
                        excelObject,
                        excelPlanningItf
                    );
                    ExcelHelper.CloseWorkBooks(excelObject);
                    // *********************** Заплата *************************
                    // Excel не освобождает память после закрытия документов !!!
                    // Не понятно чья это утечка - плагина или Excel'a, но память течет очень сильно.
                    // Будем перезагружать Excel если был загружен документ большого размера 
                    // или если было обработано много документов
                    if ((fileData.Length > (10 * 1024 * 1024)) || (wordPlanningDocumentsProcessed >= 50))
                    {
                        CloseOfficeObjectAndExtension(excelHelper, ref excelObject, ref excelPlanningItf);
                        GetExcel(out excelObject, out excelPlanningItf);
                        wordPlanningDocumentsProcessed = 0;
                    }
                    // **********************************************************
                    wordPlanningDocumentsProcessed++;
                }
                else if (wordHelper.IsApplicableFile(tempFilePath))
                {
                    new TaskDocumentHelper(wordHelper).OpenDocumentFromTask(
                        tempFilePath,
                        false,
                        true,//false,
                        true,
                        workplace.ActiveScheme,
                        taskHeadLine,
                        taskID,
                        documentName,
                        documentID,
                        workplace.ActiveScheme.UsersManager.GetCurrentUserName(),
                        -1,
                        null,
                        wordObject,
                        wordPlanningItf
                    );
                    // тут наверное надо закрыть документ?
                    //WordHelper.CloseDocuments(wordObject);
                    wordHelper.CloseFirstDocument(wordObject);
                    wordPlanningDocumentsProcessed++;
                }
                // читаем измененные данные
                fs = File.OpenRead(tempFilePath);
                fs.Position = 0;
                fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                fs.Flush();
                fs.Close();
            }
            finally
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                { }
            }
        }
*/

        private static KeyValuePair<int, byte[]> ImportTaskDocument(IDatabase db, IWorkplace workplace,
            XmlNode documentNode,
            ref DataTable importedDocumentsTbl, int parentTaskID, string headline, bool showProgress)
        {
            string documentName = XmlHelper.GetStringAttrValue(documentNode, TasksExportXmlConsts.DocNameTagName, String.Empty);
            int documentID = db.GetGenerator("G_DOCUMENTS");
            TaskDocumentType docType = (TaskDocumentType)XmlHelper.GetIntAttrValue(documentNode, TasksExportXmlConsts.DocTypeTagName, 0);
            if (showProgress)
                workplace.ProgressObj.Text = String.Format(taskImportMsg, headline, 
                    String.Format("распаковка документа{0}      {1}", Environment.NewLine, documentName));

            byte[] documentData = GetUncompressData(documentNode, TasksExportXmlConsts.DocDataTagName);
            string sourceFileName = XmlHelper.GetStringAttrValue(documentNode, TasksExportXmlConsts.DocSourceFileNameTagName, String.Empty);
            /*// если это лист планирования - меняем ему свойства
            if ((docType == TaskDocumentType.dtCalcSheet) || 
                (docType == TaskDocumentType.dtDataCaptureList) ||
                (docType == TaskDocumentType.dtInputForm) || 
                (docType == TaskDocumentType.dtPlanningSheet ||
                (docType == TaskDocumentType.dtReport)))
            {
                if (showProgress)
                    workplace.ProgressObj.Text = String.Format(taskImportMsg, headline,
                        String.Format("установка свойств листа{0}      {1}", Environment.NewLine, documentName));
                SetPlanningSheetSettings(workplace, 
                    excelHelper, wordHelper, 
                    ref excelObj, ref wordObject, 
                    ref excelPlanningItf, ref wordPlanningItf,
                    ref excelPlanningDocumentsProcessed, ref wordPlanningDocumentsProcessed,
                    ref documentData, headline, 
                    parentTaskID.ToString(), documentName, documentID.ToString(), sourceFileName);
            }*/

            //importedDocumentsTbl.Rows.Clear();
            importedDocumentsTbl.BeginLoadData();                   
            DataRow newDocument = importedDocumentsTbl.Rows.Add();
            try
            {
                newDocument.BeginEdit();
                newDocument["ID"] = documentID;
                newDocument["DocumentType"] = XmlHelper.GetIntAttrValue(documentNode, TasksExportXmlConsts.DocTypeTagName, 0);
                newDocument["Name"] = documentName;
                newDocument["SourceFileName"] = sourceFileName;
                newDocument["Version"] = 0;
                newDocument["RefTasks"] = parentTaskID;
                newDocument["Description"] = XmlHelper.GetStringAttrValue(documentNode, TasksExportXmlConsts.DocDescriptionTagName, String.Empty);
                newDocument["Ownership"] = 0;
                //newDocument["Document"] = documentData;
                newDocument.EndEdit();
                importedDocumentsTbl.EndLoadData();
                // сохраняем в базу
                if (showProgress)
                    workplace.ProgressObj.Text = String.Format(taskImportMsg, headline,
                        String.Format("сохранение документа{0}      {1}", Environment.NewLine, documentName));

                // данные документа
                return new KeyValuePair<int, byte[]>(documentID, documentData);
            }
            finally
            {
                // буфера под распаковку большие - освобождаем память
                GC.GetTotalMemory(true);
            }
        }

        private static int GetNewTaskTypeID(int oldID, DataTable IDs)
        {
            string filter = String.Format("ID = {0}", oldID);
            DataRow[] oldId = IDs.Select(filter);
            if (oldId.Length == 0)
            {
                throw new Exception("Внутренняя ошибка: не найдено oldTaskTypeID");
            }

            return Convert.ToInt32(oldId[0]["newID"]);
        }

        private static string taskImportMsg = "Задача: {0}" + Environment.NewLine + "Операция: {1}";

        private static void ImportTask(IDatabase db, IWorkplace workplace, XmlNode taskNode, 
            DataTable importedTasksTypesIds, object parentTaskID, DataTable importedTasksTbl,
            DataTable importedDocumentsTbl, DataTable importedParamsTable, DateTime usedDate,
            bool showProgress)           
        {
            if (showProgress)
                workplace.ProgressObj.Position++;

            // получаем нужные параметры задачи
            int oldTaskTypeID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskTypeTagName, 0);
            int newTaskTypeID = GetNewTaskTypeID(oldTaskTypeID, importedTasksTypesIds);
            string headline = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskHeadlineTagName, String.Empty);
            if (showProgress)
                workplace.ProgressObj.Text = String.Format(taskImportMsg, headline, "Восстановление параметров" + Environment.NewLine);
            string job = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskJobTagName, String.Empty);
            string description = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskDescriptionTagName, String.Empty);

            int newID = workplace.ActiveScheme.TaskManager.Tasks.ImportTask(headline, job, description, usedDate,
                                                                            newTaskTypeID, importedTasksTbl,
                                                                            parentTaskID);
            // записываем в базу задачу
            if (showProgress)
                workplace.ProgressObj.Text = String.Format(taskImportMsg, headline, "Сохранение задачи" + Environment.NewLine);
            //workplace.ActiveScheme.TaskManager.Tasks.ImportTasks(db, importedTasksTbl.GetChanges(DataRowState.Added));
            importedTasksTbl.Rows.Clear();

            // константы 
            importedParamsTable.Rows.Clear();
           
            try
            {
                importedParamsTable.BeginLoadData();

                XmlNodeList consts =
                    taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.TaskConstsTagName,
                                                          TasksExportXmlConsts.TaskConstTagName));
                if ((consts != null) && (consts.Count > 0))
                {
                    foreach (XmlNode constNode in consts)
                    {
                        workplace.ActiveScheme.TaskManager.Tasks.ImportTaskConstParameter(constNode.OuterXml, true,
                                                                                          ref importedParamsTable, newID);
                    }
                }
                //параметры
                XmlNodeList parameters =
                    taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.TaskParamsTagName,
                                                          TasksExportXmlConsts.TaskParamTagName));
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (XmlNode paramNode in parameters)
                    {
                        workplace.ActiveScheme.TaskManager.Tasks.ImportTaskConstParameter(paramNode.OuterXml, false,
                                                                                          ref importedParamsTable, newID);
                    }
                }

                importedParamsTable.EndLoadData();
                workplace.ActiveScheme.TaskManager.Tasks.ImportTaskConstsParameters(parentTaskID, importedParamsTable);
            }
            finally
            {
                importedParamsTable.Rows.Clear();
                GC.GetTotalMemory(true);
            }
            // теперь документы
            if (showProgress)
                workplace.ProgressObj.Text = String.Format(taskImportMsg, headline,
                                                           "Построение списка документов" + Environment.NewLine);
            XmlNodeList documents =
                taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.DocumentsTagName,
                                                   TasksExportXmlConsts.DocumentTagName));
            //WordHelper.SetObjectVisible(wordObject, true);
            Dictionary<int, byte[]> documentsData = new Dictionary<int,byte[]>();
            if ((documents != null) && (documents.Count > 0))
            {
                foreach (XmlNode docNode in documents)
                {
                    KeyValuePair<int, byte[]> kvp = ImportTaskDocument(db, workplace,
                        docNode, ref importedDocumentsTbl,
                        newID, headline, showProgress);
                    documentsData.Add(kvp.Key, kvp.Value);
                }
            }
            // информация о документе
            workplace.ActiveScheme.TaskManager.Tasks.ImportTaskDocuments(importedDocumentsTbl, documentsData);

            documentsData.Clear();
            importedTasksTbl.Rows.Clear();
            importedDocumentsTbl.Rows.Clear();
            Application.DoEvents();
            // рекурсия по потомкам
            XmlNodeList childTasks = taskNode.SelectNodes(tasksFilter);
            if ((childTasks != null) && (childTasks.Count > 0))
            {
                foreach (XmlNode childTask in childTasks)
                    ImportTask(db, workplace, childTask, importedTasksTypesIds, newID,
                        importedTasksTbl, importedDocumentsTbl, importedParamsTable, usedDate,
                        showProgress);
            }
        }

        private static void GetExcel(out object excelObject, out IFMPlanningExtension extension)
        {
            ExcelApplication excel = OfficeHelper.CreateExcelApplication();
            excel.ScreenUpdating = false;
            excel.Interactive = false;
            extension = ExcelPluginService.GetPlanningExtensionInterface(excel.OfficeApp);

            excelObject = excel.OfficeApp;
        }

        private static void GetWord(out object wordObject, out IFMPlanningExtension extension)
        {
            WordApplication word = OfficeHelper.CreateWordApplication();
            word.Visible = false;
            word.ScreenUpdating = false;
            extension = WordPluginService.GetPlanningExtensionInterface(word.OfficeApp);

            wordObject = word.OfficeApp;
        }

        private static void ImportTasksSection(IDatabase db, IWorkplace workplace, XmlDocument doc, DataTable importedTasksTypesIds, int? parentTaskID, bool showProgress)
        {
            // считаем общее количество задач, инициализируем прогресс
            workplace.ProgressObj.MaxProgress = GetAllTasks(doc).Count;
            workplace.ProgressObj.Text = "Импорт задач";
            // получаем таблицы для экспорта задач и документов
            DataTable importedTasksTbl = workplace.ActiveScheme.TaskManager.Tasks.GetTasksImportTable();
            DataTable importedDocumentsTbl = workplace.ActiveScheme.TaskManager.Tasks.GetDocumentsImportTable();
            DataTable importedParamsTbl = workplace.ActiveScheme.TaskManager.Tasks.GetParamsImportTable();
            // запоминаем время (чтобы было одинаково для всех задач)
            DateTime curTime = DateTime.Now;

            // получаем все задачи верхнего уровня и обрабатываем их с ParentTask = null
            XmlNodeList tasks = doc.SelectNodes(String.Format("{0}/{1}/{2}", TasksExportXmlConsts.RootNodeTagName, TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            for (int i = 0; i < tasks.Count; i++)
            {
                if (parentTaskID != null)
                    ImportTask(db, workplace, tasks[i], importedTasksTypesIds, parentTaskID,
                        importedTasksTbl, importedDocumentsTbl, importedParamsTbl, 
                        curTime,
                        showProgress);
                else
                    ImportTask(db, workplace, tasks[i], importedTasksTypesIds, DBNull.Value,
                        importedTasksTbl, importedDocumentsTbl, importedParamsTbl, 
                        curTime,
                        showProgress);
            }
        }

        public static bool InnerImportTasks(IWorkplace workplace, XmlDocument data, int? parentID, bool showProgress)
        {
            bool res;
            // проверяем наш ли это Xml
            XmlNode rootNode = data.SelectSingleNode(TasksExportXmlConsts.RootNodeTagName);
            if (rootNode == null)
                throw new Exception("Документ не является экспортным файлом задач");
            IDatabase db = null;
            try
            {
                // получаем интерфейс IDatabase, открываем транзакцию
                db = workplace.ActiveScheme.TaskManager.Tasks.GetTaskDB();
                workplace.ActiveScheme.TaskManager.Tasks.BeginDbTransaction();
                //db.BeginTransaction();
                // синхронизируем справочник типов задач
                DataTable importedTasksTypes = null;
                ImportTaskTypesSection(workplace, data, ref importedTasksTypes, showProgress);
                // импортируем задачи
                ImportTasksSection(db, workplace, data, importedTasksTypes, parentID, showProgress);
                importedTasksTypes.Clear();
                //importedTasksTypes = null;
                workplace.ActiveScheme.TaskManager.Tasks.CommitDbTransaction();
                //db.Commit();
                if (showProgress)
                    workplace.ProgressObj.StopProgress();
                res = true;
            }
            catch (Exception e)
            {
                if (db != null)
                {
                    //db.Rollback();
                    workplace.ActiveScheme.TaskManager.Tasks.RollbackDbTransaction();
                    db = null;
                    workplace.ActiveScheme.TaskManager.Tasks.ClearDB();
                }
                // перезагружаем список объектов системы на сервере
                workplace.ActiveScheme.UsersManager.LoadObjects();
                if (showProgress)
                    workplace.ProgressObj.StopProgress();
                string errStr = String.Format("Произведен откат транзакции. Произошла ошибка: {0}", e.Message); // + Environment.NewLine + e.StackTrace
                MessageBox.Show(errStr, "Во время импорта произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
                XmlHelper.ClearDomDocument(ref data);
                GC.Collect();
            }
            return res;
        }

        public static bool ResolveImportType(IWorkplace workplace, UltraGrid ug, out TaskImportType importType, out int? parentID)
        {
            //importType = TaskImportType.tiUndefined;
            parentID = null;
            bool result = true;
            // есть-ли выделенные задачи?
            if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
            {
                importType = TaskImportType.tiAsRootTasks;
            }
            else
            {
                importType = FormSelectImportMode.SelectTasksImportType();
            }
            // если нажали отмену - выходим
            if (importType == TaskImportType.tiUndefined)
            {
                MessageBox.Show("Операция отменена", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                result = false;
                return result;
            }
            if (importType == TaskImportType.tiAsChildForSelected)
            {
                // не заблокирована ли родительская задача?
                int? lockedUser = null;
                try
                {
                    lockedUser = Convert.ToInt32(UltraGridHelper.GetRowCells(ug.Selected.Rows[0]).Cells["LockByUser"].Value);
                }
                catch { };
				bool allowed = (lockedUser == -1) || (lockedUser == null) || (lockedUser == ClientAuthentication.UserID);
                if (!allowed)
                {
                    MessageBox.Show("Родительская задача заблокирована. Невозможно произвести импорт в этом режиме", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = false;
                    return result;
                }
                parentID = UltraGridHelper.GetRowID(ug.Selected.Rows[0]);
                // не является ли задача только что созданной?
                ITask parentTask = null;
                try
                {
                    parentTask = workplace.ActiveScheme.TaskManager.Tasks[(int)parentID];
                    if (parentTask.PlacedInCacheOnly)
                    {
                        MessageBox.Show("Родительская задача только что создана. Невозможно произвести импорт в этом режиме", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        result = false;
                        return result;
                    }
                }
                finally
                {
                    if (parentTask != null)
                        parentTask.Dispose();
                }
            }
            return result;
        }

        public static void ImportTasks(IWorkplace workplace, UltraGrid ug)
        {
            TaskImportType importType;
            int? parentID;
            if (!ResolveImportType(workplace, ug, out importType, out parentID))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "xml";
            ofd.Filter = "XML файлы|*.xml";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            //workplace.ProgressObj.MaxProgress = ofd.FileNames.Length;
            workplace.OperationObj.Text = "Импорт задач";
            workplace.OperationObj.StartOperation();
            try
            {
                foreach (string fileName in ofd.FileNames)
                {
                    // инициализируем прогресс
                    workplace.OperationObj.Text = String.Format("Импорт задач из файла: {0}", Path.GetFileName(fileName));
                    //workplace.ProgressObj.Text = "Загрузка документа";
                    using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        workplace.ActiveScheme.TaskManager.Tasks.ImportTask(stream, parentID);
                    }
                    //workplace.ProgressObj.Position++;
                }
                workplace.OperationObj.StopOperation();
            }
            catch(Exception e)
            {
                workplace.OperationObj.StopOperation();
                throw new Exception(e.Message, e.InnerException);
            }
        }
        /*
        public static bool ImportTasks(IWorkplace workplace, UltraGrid ug)
        {
            TaskImportType importType;
            //int start = Environment.TickCount;
            int? parentID;
            if (!ResolveImportType(workplace, ug, out importType, out parentID))
                return false;

            bool res = false;
            // получаем имя файла
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "xml";
            ofd.Filter = "XML файлы|*.xml";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return false;
            Application.DoEvents();

            foreach (string fileName in ofd.FileNames)
            {
                // инициализируем прогресс
                workplace.ProgressObj.MaxProgress = 0;
                workplace.ProgressObj.Caption = String.Format("Импорт задач из файла: {0}", Path.GetFileName(fileName));
                workplace.ProgressObj.Text = "Загрузка документа";
                workplace.ProgressObj.StartProgress();
                //int startTime = Environment.TickCount;
                XmlDocument doc = new XmlDocument();
                try
                {
                    // загружаем документ
                    doc.Load(fileName);

                    if (parentID != null)
                    {
                        XmlNodeList nodesList = doc.SelectNodes("//tasks");
                        if (nodesList.Count - ug.ActiveRow.Band.Index > 6)
                        {
                            if (ofd.FileNames.Length > 1)
                                continue;
                            else
                            {
                                workplace.ProgressObj.StopProgress();
                                MessageBox.Show("Импорт невозможен, уровень вложенности задач в файле больше максимально возможного", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false ;
                            }
                        }
                        //  только если есть подчиненная задача
                        // смотрим разницу между уровнем родительской задачи и уровнем вложений
                        //if (nodesList.Count )
                    }
                    res = InnerImportTasks(workplace, doc, parentID, true);
                    if (!res)
                        break;
                }
                catch //(Exception e)
                {
                    //
                }
                finally
                {
                    workplace.ProgressObj.StopProgress();
                    // int elapsed = Environment.TickCount - start;
                    // MessageBox.Show("Elapsed: " + elapsed + " ms");
                }
            }
            return res;
        }*/
        #endregion
    }

}

