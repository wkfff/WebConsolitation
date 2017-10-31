using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Interop.Renaming;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    internal class TasksDocumentsRenaming : IDisposable
    {
        // путь, где будут расположены все документы для переименования
        // там же будет лежать список переименований
        private List<int> tasksIDList;
        private IScheme scheme;
        private DocumentsHelper helper;
        private string renameListFileName;
        private RenamingToolClass renamingTool;
        private DataSet dsProtocol;
        private IWorkplace workplace;
        private XmlDocument renamingDom;

        internal TasksDocumentsRenaming(IWorkplace workplace, UltraGrid taskGrid, bool isAllTasks, bool childsTaskRename, string renameListFileName)
        {
            this.workplace = workplace;
            scheme = workplace.ActiveScheme;
            this.renameListFileName = renameListFileName;
            renamingDom = new XmlDocument();
            renamingDom.Load(renameListFileName);
            renamingTool = new RenamingToolClass();
            helper = new DocumentsHelper();
            dsProtocol = CreateHieararchyData();
            tasksIDList = GetTasksList(taskGrid, !isAllTasks, childsTaskRename, ref dsProtocol);
        }

        public void Dispose()
        {
            if (renamingTool != null && Marshal.IsComObject(renamingTool))
                Marshal.ReleaseComObject(renamingTool);
        }

        /// <summary>
        /// получение списка задач, которые подлежат обработке переименователем
        /// </summary>
        private List<int> GetTasksList(UltraGrid grid, bool getSelectedRows, bool childsTaskRename, ref DataSet ds)
        {
            tasksIDList = new List<int>();
            BindingSource bs = (BindingSource)grid.DataSource;
            DataView view = (DataView) bs.DataSource;
            if (getSelectedRows)
            {
                foreach (UltraGridRow gridRow in grid.Selected.Rows)
                {
                    DataRow newRow = ds.Tables[0].NewRow();
                    newRow[0] = gridRow.Cells["ID"].Value;
                    object val = gridRow.Cells["LockByUser"].Value;
                    if (val is DBNull || Convert.ToInt32(val) == -1)
                    {
                        int taskID = Convert.ToInt32(gridRow.Cells["ID"].Value);
                        if (!tasksIDList.Contains(taskID))
                        {
                            tasksIDList.Add(taskID);
                            newRow[1] = string.Format("Задача {0} {1} обработана", gridRow.Cells["ID"].Value, gridRow.Cells["HeadLine"].Value);
                        }
                    }
                    else
                    {
                        newRow[1] = string.Format("Задача {0} {1} пропущена. Причина - задача находится в состоянии редактирования", gridRow.Cells["ID"].Value, gridRow.Cells["HeadLine"].Value);
                    }
                    ds.Tables[0].Rows.Add(newRow);
                    if (childsTaskRename)
                        ProcessChildsTask(newRow[0], view.Table, ref ds);
                }
            }
            else
            {
                // идем по всем задачам, выбираем те, что не заблокированы
                view.Table.BeginLoadData();
                try
                {
                    foreach (DataRow row in view.Table.Rows)
                    {
                        DataRow newRow = ds.Tables[0].NewRow();
                        newRow[0] = row["ID"];
                        object val = row["LockByUser"];
                        if (val is DBNull || Convert.ToInt32(val) == -1)
                        {
                            int taskID = Convert.ToInt32(row["ID"]);
                            if (!tasksIDList.Contains(taskID))
                            {
                                tasksIDList.Add(taskID);
                                newRow[1] = string.Format("Задача {0} {1} обработана", row["ID"], row["HeadLine"]);
                            }
                            else
                            {
                                newRow[1] = string.Format("Задача {0} {1} пропущена. Причина - задача находится в состоянии редактирования", row["ID"], row["HeadLine"]);
                            }
                        }
                        ds.Tables[0].Rows.Add(newRow);
                    }
                }
                finally
                {
                    view.Table.EndLoadData();
                }
            }
            return tasksIDList;
        }

        /// <summary>
        /// обработка подчиненных задач
        /// </summary>
        private void ProcessChildsTask(object parentTaskID, DataTable dtTasks, ref DataSet ds)
        {
            DataRow[] childTasks = dtTasks.Select(string.Format("RefTasks = {0}", parentTaskID));
            {
                if (childTasks != null)
                {
                    foreach (DataRow taskRow in childTasks)
                    {
                        DataRow newRow = ds.Tables[0].NewRow();
                        newRow[0] = taskRow["ID"];
                        object val = taskRow["LockByUser"];
                        if (val is DBNull || Convert.ToInt32(val) == -1)
                        {
                            int taskID = Convert.ToInt32(taskRow["ID"]);
                            if (!tasksIDList.Contains(taskID))
                            {
                                tasksIDList.Add(taskID);
                                newRow[1] = string.Format("Задача {0} {1} обработана", taskRow["ID"], taskRow["HeadLine"]);
                            }
                            else
                            {
                                newRow[1] = string.Format(
                                        "Задача {0} {1} пропущена. Причина - задача находится в состоянии редактирования",
                                        taskRow["ID"], taskRow["HeadLine"]);
                            }
                        }
                        ds.Tables[0].Rows.Add(newRow);

                        ProcessChildsTask(taskRow["ID"], dtTasks, ref ds);
                    }
                }
            }
        }

        private ITask GetTask(int taskID)
        {
            return scheme.TaskManager.Tasks[taskID];
        }

        public String[] GetRenamingTablesID()
        {
            string[] value = new string[tasksIDList.Count];
            for (int i = 0; i < tasksIDList.Count; i++)
                value[i] = tasksIDList[i].ToString();
            return value;
        }

        /// <summary>
        /// получаем документы в задаче, сохраняем их на диск
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private void SaveToDataBase(ITask task)
        {
            DataTable dt = new DataTable("Documents");
            using (IDataUpdater updater = task.GetTaskDocumentsAdapter())
            {
                updater.Fill(ref dt);
                foreach (DataRow docRow in dt.Rows)
                {
                    helper.CheckDocumentTypeAndCRC(task, docRow);
                }
            }
        }

        /// <summary>
        /// берем документы задачи на диске
        /// </summary>
        /// <param name="task"></param>
        private bool SaveToDisk(ITask task)
        {
            DataTable dt = new DataTable("Documents");
            bool hasError = true;
            renamingTool.LoadSettings(renameListFileName);
            using (IDataUpdater updater = task.GetTaskDocumentsAdapter())
            {
                updater.Fill(ref dt);
                foreach (DataRow docRow in dt.Rows)
                {
                    string docName = helper.InternalSaveTaskDocument(task, Convert.ToInt32(docRow["ID"]), docRow["Name"].ToString(),
                        docRow["SourceFileName"].ToString());

                    string renameReport = string.Empty;
                    hasError = !renamingTool.Open(docName, out renameReport) && hasError;
                    if (!string.IsNullOrEmpty(renameReport))
                    {
                        DataRow row = dsProtocol.Tables[1].NewRow();
                        row[0] = renameReport;
                        row[1] = task.ID;
                        dsProtocol.Tables[1].Rows.Add(row);
                    }
                }
            }
            return hasError;
        }

        #region часть функционала переименователя
        /// <summary>
        /// //переименование имени измерения и иерархии в параметре
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="dimension"></param>
        /// <param name="dimName"></param>
        /// <param name="hierName"></param>
        /// <param name="oldFullDimName"></param>
        /// <param name="msg"></param>
        private bool RenameDimInTaskParam(XmlDocument dom, string dimension, out string dimName, 
            out string hierName, out string oldFullDimName, ref string msg)
        {
            // вытаскиваем исходные имена
            XmlNode dimNode = dom.SelectSingleNode("function_result/Dimension");
            dimName = XmlHelper.GetStringAttrValue(dimNode, "name", "");
            XmlNode hierNode = dom.SelectSingleNode("function_result/Hierarchy");
            hierName = XmlHelper.GetStringAttrValue(hierNode, "name", "");
            oldFullDimName = GetFullDimName(dimName, hierName, true);

            //находим узел с заменой
            string xpath = string.Format("//replace[@type='dimension' and @old='{0}']", dimension);
            XmlNode replacingNode = renamingDom.SelectSingleNode(xpath);

            //Если нашли - заменяем
            string newDimension;
            if (replacingNode != null)
            {
                newDimension = XmlHelper.GetStringAttrValue(replacingNode, "new", "");
                string[] newDimNHier = newDimension.Split('.');
                dimName = newDimNHier[0];
                hierName = newDimNHier[1];
                XmlHelper.SetAttribute(dimNode, "name", dimName);
                XmlHelper.SetAttribute(hierNode, "name", hierName);
                msg = string.Format("измерение заменено на \"{0}\"" , newDimension);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Переименование имен уровней в параметре
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="dimName"></param>
        /// <param name="hierName"></param>
        /// <param name="msg"></param>
        private bool RenameLevelsInTaskParam(XmlDocument dom, string dimName, string hierName, ref string msg)
        {
            bool result = false;
            XmlNodeList nodeList = dom.SelectNodes("function_result/Levels/Level");
            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    //берем исходное имя уровня
                    string oldName = XmlHelper.GetStringAttrValue(nodeList[i], "name", "");

                    //ищем узел с заменой
                    string xpath = string.Format("//replace[(@type='level') and (@old='{0}') " +
                        "and (@dimension='{1}') and (@hierarchy='{2}')]", oldName, dimName, hierName);
                    XmlNode replacingNode = renamingDom.SelectSingleNode(xpath);
                    if (replacingNode == null)
                        continue;

                    //если нашли - заменяем
                    string newName = XmlHelper.GetStringAttrValue(replacingNode, "new", "");
                    XmlHelper.SetAttribute(nodeList[i], "name", newName);
                    if (msg != "")
                        msg += "; ";
                    msg += string.Format("уровень {0} заменен на {1}", oldName, newName);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// получает имя элемента из его юникнейма, выделяя последнюю часть
        /// </summary>
        /// <param name="uName"></param>
        /// <returns></returns>
        private static string GetMemberNameFromUName(string uName)
        {
            string[] parsedName = uName.Split(new string[] { "].[" }, StringSplitOptions.None);
            string newName = parsedName[parsedName.Length - 1];
            newName = newName.Substring(0, newName.Length - 1);
            return newName;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="oldPattern"></param>
        /// <param name="newPattern"></param>
        /// <returns></returns>
        private static void ChangeUNamesHead(XmlNode root, string oldPattern, string newPattern)
        {            
            XmlNodeList descendants = root.SelectNodes("//Member[@unique_name]");
            if (descendants == null)
                return;
            
            for (int i = 0; i < descendants.Count; i++)
            {
                string uName = XmlHelper.GetStringAttrValue(descendants[i], "unique_name", string.Empty);
                if (uName.Contains(oldPattern))
                {
                    uName = uName.Replace(oldPattern, newPattern);
                    XmlHelper.SetAttribute(descendants[i], "unique_name", uName);
                }
            }
        }

        /// <summary>
        /// Переименование элементов в параметре
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="dimName"></param>
        /// <param name="hierName"></param>
        private bool RenameMembersInTaskParam(XmlDocument dom, string dimName, string hierName)
        {
            bool result = false;
            //получаем список замен мемберов
            string xpath = string.Format("//replace[@type='member' and @dimension='{0}' and" +
                   " @hierarchy='{1}']", dimName, hierName);
            XmlNodeList nodeList = renamingDom.SelectNodes(xpath);

            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    string oldUName = XmlHelper.GetStringAttrValue(nodeList[i], "old", "");
                    string newUName = XmlHelper.GetStringAttrValue(nodeList[i], "new", "");

                    //находим в документе элемент для переименования
                    xpath = string.Format("//Member[@unique_name='{0}']", oldUName);
                    XmlNode memberNode = dom.SelectSingleNode(xpath);

                    if (memberNode == null)
                        continue;
                    XmlHelper.SetAttribute(memberNode, "unique_name", newUName);
                    result = true;

                    /*Сейчас у нас юник-нэймы всегда строятся по именам. =>
                        Смена юник нэйма автоматически приводит к изменениям юник-нэймов у потомков.
                        Подкорректируем юник-нэймы потомков. Если у-нэйм начинается с у-нэйма замены,
                        то подменим на новый вариант.*/
                    ChangeUNamesHead(memberNode, oldUName, newUName);
                    
                    //получим нейм из юникнейма
                    string newName = GetMemberNameFromUName(newUName);
                    XmlHelper.SetAttribute(memberNode, "name", newName);

                }
            }
            return result;
        }

        /// <summary>
        /// переименование наборов элементов в параметре
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        private bool RenameMembersetsInTaskParam(XmlDocument dom)
        {
            bool result = false;
            XmlNodeList replaceNodes = renamingDom.SelectNodes("//replace[@type='memberset']");
            if (replaceNodes == null)
                return result;
            XmlNodeList members = dom.SelectNodes("//Member[@unique_name]");
            if (members == null)
                return result;
            foreach (XmlNode node in replaceNodes)
            {
                //получаем параметры замены - старый и новый паттерны
                string replacedOld = XmlHelper.GetStringAttrValue(node, "old", string.Empty);
                string replacedNew = XmlHelper.GetStringAttrValue(node, "new", string.Empty);
                if ((replacedOld == string.Empty) || (replacedNew == string.Empty))
                    continue;

                for (int i = 0; i < members.Count; i++)
                {
                    //если юникнейм текущего мембера не содержит искомого паттерна - пропускаем его
                    string uName = XmlHelper.GetStringAttrValue(members[i], "unique_name", string.Empty);
                    if (!uName.ToUpper().Contains(replacedOld.ToUpper()))
                        continue;

                    string newUName = string.Empty;
                    //Аццкий трындец по требованию FMQ 12280
                    //Специальное переименование конкретного измерения
                    if ((replacedOld.ToUpper() == "[ВАРИАНТ].[ПРОЕКТ ДОХОДОВ]") &&
                        (replacedNew.ToUpper() == "[ВАРИАНТ].[ПРОЕКТ ДОХОДОВ].[ВСЕ]"))
                    {
                        string year = XmlHelper.GetStringAttrValue(members[i], "Год", "");
                        if (year != string.Empty)
                        {
                            newUName = uName.Replace(replacedOld, replacedNew + ".[" + year + "]");
                        }
                    }
                    else//обычное переименование всех остальных измерений
                        newUName = uName.Replace(replacedOld, replacedNew);

                    
                    if (newUName == string.Empty)//Не должно такого быть
                        continue;
                    string newName = GetMemberNameFromUName(newUName);
                    XmlHelper.SetAttribute(members[i], "unique_name", newUName);
                    XmlHelper.SetAttribute(members[i], "name", newName);

                    result = true;
                }
            }
            return result;
        }

        private static string GetFullDimName(string dimName, string hierName, bool withBrackets)
        {
            if (!string.IsNullOrEmpty(hierName))
                if (withBrackets)
                    return string.Format("[{0}].[{1}]", dimName, hierName);
                else
                    return dimName + '.' + hierName;
            else
                if (withBrackets)
                    return '[' + dimName + ']';
                return dimName;

        }

        /// <summary>
        /// заменяет начало юникнеймов мемберов на переименованное измерение
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="oldDimension"></param>
        /// <param name="newDimension"></param>
        private static void RenameDimInMembers(XmlDocument dom, string oldDimension, string newDimension)
        {
            ChangeUNamesHead(dom.DocumentElement, oldDimension, newDimension);
        }

        /// <summary>
        /// переименование параметра задачи
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="members"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool RenameTaskParam(ref string dimension, ref string members, out string msg)
        {
            msg = string.Empty;
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(members);

            string dimName;
            string hierName;
            string oldFullDimName;
            string newFullDimName = "";
            //переименование измерения
            bool dimChanged = RenameDimInTaskParam(dom, dimension, out dimName, out hierName, 
                out oldFullDimName, ref msg);
            if (dimChanged)
            {
                dimension = GetFullDimName(dimName, hierName, false);
                newFullDimName = GetFullDimName(dimName, hierName, true);
            }

            //переименование уровней
            bool levelsChanged = RenameLevelsInTaskParam(dom, dimName, hierName, ref msg);

            //переименование элементов
            bool membersChanged = RenameMembersInTaskParam(dom, dimName, hierName) 
                || RenameMembersetsInTaskParam(dom);
            if (membersChanged)
            {
                if (msg != "")
                    msg += "; ";
                msg += "изменены ссылки на элементы";
            }

            if (dimChanged)
                RenameDimInMembers(dom, oldFullDimName, newFullDimName);
                      
            bool changed = dimChanged || levelsChanged || membersChanged;
            if (changed)
                members = dom.InnerXml;
            return changed;
        }

        #endregion

        /// <summary>
        /// переименовываем коллекцию параметров
        /// </summary>
        private void RenameTaskParams(ITask task)
        {
            ITaskParamsCollection paramsCollection = task.GetTaskParams();
            DataTable dtParams = paramsCollection.ItemsTable;
            foreach (DataRow row in dtParams.Rows)
            {
                ITaskParam param = paramsCollection.ParamByID(Convert.ToInt32(row["ID"]));
                if (param.Inherited)
                    continue;
                try
                {
                    string xmlDoc = param.Values.ToString();
                    string dimension = param.Dimension;
                    //***********
                    // тут происходит непосредственно само переименование
                    string renameReport = string.Empty;
                    //                if (renamingTool.RenameTaskParam(ref dimension, ref xmlDoc, out renameReport))
                    if (RenameTaskParam(ref dimension, ref xmlDoc, out renameReport))
                    {
                        if (!string.IsNullOrEmpty(renameReport))
                        {
                            DataRow dataRow = dsProtocol.Tables[1].NewRow();
                            dataRow[0] = string.Format("Параметр \"{0}\": {1}", param.Name, renameReport);
                            dataRow[1] = task.ID;
                            dsProtocol.Tables[1].Rows.Add(dataRow);
                        }
                        //***********;
                        param.Values = xmlDoc;
                        param.Dimension = dimension;

                    }
                }
                catch(Exception e)
                {
                    DataRow dataRow = dsProtocol.Tables[1].NewRow();
                    dataRow[0] = string.Format("Параметр \"{0}\" вызвал ошибку: {1}", param.Name, e.Message);
                    dataRow[1] = task.ID;
                    dsProtocol.Tables[1].Rows.Add(dataRow);
                }
            }
            paramsCollection.SaveChanges();
            paramsCollection.ReloadItemsTable();
        }

        /// <summary>
        /// При переименовании подчиненных задач приходится пропускать унаследованные параметры.
        /// При этом, если родительская задача не переименовывается, следует выдать предупреждение.
        /// </summary>
        private void InheritedParamsWarning()
        {           
            foreach (int ID in tasksIDList)
            {
                ITask task = GetTask(ID);
                int prntTask = task.RefTasks;
                //если задача не имеет родительской, то и унаследованных параметров у нее нет
                if (prntTask == -1)
                    continue;
                //если родительская задача присутсвует в списке переименования, то все ОК
                if (tasksIDList.Contains(prntTask))
                    continue;
                //если параметров в задаче нет, то и делать нечего
                if (task.GetTaskParams().Count == 0)
                    continue;

                MessageBox.Show("Обрабатываемые задачи содержат параметры, унаследованные от "  + 
                    "родительских задач, для которых переименование запущено не было. Для корректной " + 
                    "работы необходимо запустить переименование так же и для родительских задач.", 
                    "Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
        }

        /// <summary>
        /// переименование документов. 
        /// </summary>
        public bool RenameDocuments(out DataSet dsRenameReport)
        {
            InheritedParamsWarning();

            workplace.OperationObj.Text = "Обработка документов в задачах";
            workplace.OperationObj.StartOperation();
            try
            {
                bool hasError = true;
                foreach (int ID in tasksIDList)
                {
                    ITask task = GetTask(ID);
                    try
                    {
                        task.BeginUpdate("Редактировать");
                        // сохраняем документы на диск, производим операцию переименования над документами задачи
                        hasError = SaveToDisk(task) && hasError;
                        // сохраняем документы в базу
                        RenameTaskParams(task);
                        SaveToDataBase(task);
                    }
                    finally
                    {
                        task.EndUpdate();
                    }
                }
                dsRenameReport = dsProtocol.Copy();
                return !hasError;
            }
            finally
            {
                workplace.OperationObj.StopOperation();
            }            
        }

        /// <summary>
        /// создает иерархический объект DataSet для хранения лога переименовывания
        /// </summary>
        /// <returns></returns>
        private static DataSet CreateHieararchyData()
        {
            DataSet ds = new DataSet();
            DataTable dtTasks = new DataTable("Tasks");
            DataColumn dcParent = dtTasks.Columns.Add("ID", typeof(Int32));
            dcParent.Caption = "ID задачи";
            DataColumn column = dtTasks.Columns.Add("Caption", typeof(String));
            column.Caption = "Обработка задачи переименователем";
            ds.Tables.Add(dtTasks);
            DataTable dtMessages = new DataTable("Messages");
            column = dtMessages.Columns.Add("Message", typeof(String));
            column.Caption = "Переименование документа задачи";
            DataColumn dcChild = dtMessages.Columns.Add("RefTaskID", typeof(Int32));
            ds.Tables.Add(dtMessages);

            DataRelation dr = new DataRelation("relation", dcParent, dcChild);
            ds.Relations.Add(dr);

            return ds;
        }
    }
}
