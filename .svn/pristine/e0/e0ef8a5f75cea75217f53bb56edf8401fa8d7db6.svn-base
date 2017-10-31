using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Common.TaskParamEditors;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using CC = Krista.FM.Client.Components;
using Krista.FM.Common; 

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    /// <summary>
    /// Класс для манипуляций с задачами
    /// </summary>
    public partial class TasksViewObj : BaseViewObj
    {
        private DataTable GetParamsTable()
        {
            DataTable dt = (DataTable)_tasksView.ugeParams.DataSource;
            return dt;
        }

        private DataTable GetConstsTable()
        {
            DataTable dt = (DataTable)_tasksView.ugeConsts.DataSource;
            return dt;
        }

        private static bool IsConsts(object sender)
        {
            if (!((sender is UltraGrid) || (sender is CC.UltraGridEx)))
                throw new Exception(String.Format("Недопустимый тип параметра '{0}'", sender.GetType()));

            CC.UltraGridEx uge;

            if (sender is UltraGrid)
                uge = ((UltraGrid)sender).Parent.Parent.Parent as CC.UltraGridEx;
            else
                uge = sender as CC.UltraGridEx;

            string ugeName = String.Empty;
            if (uge != null)
                ugeName = uge.Name.ToUpper();

            return ugeName == "UGECONSTS";
        }

        private static bool CheckDuplicateNames(DataTable changedRows)
        {
            changedRows.DefaultView.Sort = "Name ASC";
            DataTable sorted = changedRows.DefaultView.ToTable("dublicates", false, "Name", "Inherited");
            string lastName = String.Empty;
            foreach (DataRow row in sorted.Rows)
            {
                if (Convert.ToInt32(row["Inherited"]) == 1)
                    continue;
                string curName = Convert.ToString(row[0]).ToUpper();
                if (lastName == curName)
                    return false;
                lastName = curName;
            }
            return true;
        }

        private static bool CheckNullValues(DataTable changesRows, string columnName)
        {
            String filterStr = String.Format("({0} is Null) or ({0} = '')", columnName);
            DataRow[] nullableValues = changesRows.Select(filterStr);
            return (nullableValues == null) || (nullableValues.Length == 0);
        }

        private static bool CheckMultipleValues(DataTable changedRows)
        {
            bool result = true;
            XmlDocument valueDom = new XmlDocument();
            foreach (DataRow row in changedRows.Rows)
            {
                bool multiSelection = Convert.ToBoolean(row["AllowMultiSelect"]);
                if (multiSelection)
                    continue;
                string newValue = Convert.ToString(row["ParamValues"]);
                valueDom.LoadXml(newValue);
                XmlNodeList nl = valueDom.SelectNodes("//Member[@checked='true']");
                if (nl != null)
                  result = !(nl.Count > 1);
                if (!result)
                    break;
            }
            return result;
        }

        private bool ugeConstsParams_OnSaveChanges(object sender)
        {
            if ((ActiveTask == null) || (!ActiveTask.InEdit))
                return true;

            DataTable dt;
            ITaskItemsCollection prms;
            if (!IsConsts(sender))
            {
                dt = GetParamsTable();
                prms = ActiveTask.GetTaskParams();
            }
            else
            {
                dt = GetConstsTable();
                prms = ActiveTask.GetTaskConsts();
            }
            if (dt != null)
            {
                ITaskParamBase prm;

                // обрабатываем добавленные и модифицированные
                DataTable changedRows = dt.Clone();
                DataTable addedRows = dt.GetChanges(DataRowState.Added);
                if ((addedRows != null) && (addedRows.Rows.Count > 0))
                    changedRows.Merge(addedRows);
                DataTable modifiedRows = dt.GetChanges(DataRowState.Modified);
                if ((modifiedRows != null) && (modifiedRows.Rows.Count > 0))
                    changedRows.Merge(modifiedRows);
                if (changedRows.Rows.Count > 0)
                {
                    // все ли названия заполнены?
                    if (!CheckNullValues(changedRows, "Name"))
                    {
                        CanDeactivate = false;
                        MessageBox.Show("Поле 'Название' должно быть заполнено", "Ошибка в задаче", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    // если это параметры - все ли измерения заполнены?
                    if ((!IsConsts(sender)) && (!CheckNullValues(changedRows, "Dimension")))
                    {
                        CanDeactivate = false;
                        MessageBox.Show("Поле 'Измерение' должно быть заполнено", "Ошибка в задаче", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    // все ли значения заполнены?
                    if (!CheckNullValues(changedRows, "ParamValues"))
                    {
                        CanDeactivate = false;
                        MessageBox.Show("Поле 'Значение' должно быть заполнено", "Ошибка в задаче", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    // нет ли повторяющихся имен?
                    if (!CheckDuplicateNames(dt))
                    {
                        CanDeactivate = false;
                        MessageBox.Show("Обнаружены дублирующиеся названия элементов", "Ошибка в задаче", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (!IsConsts(sender) && !CheckMultipleValues(changedRows))
                    {
                        CanDeactivate = false;
                        MessageBox.Show("Обнаружены параметры с запретом на множественный выбор элементов и несколькими выбранными элементами", 
                            "Ошибка в задаче", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    foreach (DataRow changed in changedRows.Rows)
                    {
                        int id = Convert.ToInt32(changed["ID"]);
                        if (!IsConsts(sender))
                        {
                            if (!Convert.ToBoolean(changed["Inherited"]))
                            {
                                prm = ((ITaskParamsCollection)prms).ParamByID(id);
                                if (prm != null)
                                {
                                    ((ITaskParam)prm).Name = Convert.ToString(changed["Name"]);
                                    ((ITaskParam)prm).Comment = Convert.ToString(changed["Description"]);
                                    ((ITaskParam)prm).Dimension = Convert.ToString(changed["Dimension"]);
                                    ((ITaskParam)prm).AllowMultiSelect = Convert.ToBoolean(changed["AllowMultiSelect"]);
                                    ((ITaskParam)prm).Values = Convert.ToString(changed["ParamValues"]);
                                }
                            }
                        }
                        else
                        {
                            prm = ((ITaskConstsCollection)prms).ConstByID(id);
                            if (prm != null)
                            {
                                ((ITaskConst)prm).Name = Convert.ToString(changed["Name"]);
                                ((ITaskConst)prm).Values = Convert.ToString(changed["ParamValues"]);
                                ((ITaskConst)prm).Comment = Convert.ToString(changed["Description"]);
                            }
                        }
                    }
                }

                // Обрабатываем удаленные записи
                DataTable deletedRows = dt.GetChanges(DataRowState.Deleted);
                if ((deletedRows != null) && (deletedRows.Rows.Count > 0))
                {
                    foreach (DataRow deleted in deletedRows.Rows)
                    {
                        int id = Convert.ToInt32(deleted["ID", DataRowVersion.Original]);
                        if (!IsConsts(sender))
                        {
                            prm = ((ITaskParamsCollection)prms).ParamByID(id);
                            if (prm != null)
                                ((ITaskParamsCollection)prms).Remove((ITaskParam)prm);
                        }
                        else
                        {
                            prm = ((ITaskConstsCollection)prms).ConstByID(id);
                            if (prm != null)
                                ((ITaskConstsCollection)prms).Remove((ITaskConst)prm);
                        }
                    }
                }
            }

            prms.SaveChanges();
            //SetParentTaskConstPatams(IsConsts(sender));
            if (dt != null)
                dt.AcceptChanges();
            ugeConstsParams_OnRefreshData(sender);
            CanDeactivate = true;
            return true;
        }

        /// <summary>
        /// похоже на сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        void ugeConstsParams_OnClearCurrentTable(object sender)
        {
            string deleteMessage;
            if (!IsConsts(sender))
                deleteMessage = "Удалить все параметры текущей задачи?";
            else
                deleteMessage = "Удалить все константы текущей задачи?";

            if (MessageBox.Show(deleteMessage, "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string operationText = "Удаление данных";

            Workplace.OperationObj.Text = operationText;
            Workplace.OperationObj.StartOperation();
            try
            {
                DataTable dt = null;
                ITaskItemsCollection prms = null;

                GetConstsParamsCollections(sender, ref operationText, ref dt, ref prms);

                Workplace.OperationObj.Text = operationText;

                foreach (DataRow deletetedRow in dt.Rows)
                {
                    if (!Convert.ToBoolean(deletetedRow["Inherited"]))
                        deletetedRow.Delete();
                }

                SaveDeletedRows(sender, dt, prms);

                prms.SaveChanges();
                dt.AcceptChanges();
                ugeConstsParams_OnRefreshData(sender);
                CanDeactivate = true;
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        private void GetConstsParamsCollections(object sender, ref string operationText, ref DataTable dt, ref ITaskItemsCollection prms)
        {
            if (!IsConsts(sender))
            {
                dt = GetParamsTable();
                prms = ActiveTask.GetTaskParams();
                operationText = "Удаление параметров текущей задачи";
            }
            else
            {
                dt = GetConstsTable();
                prms = ActiveTask.GetTaskConsts();
                operationText = "Удаление констант текущей задачи";
            }
        }

        private static void SaveDeletedRows(object sender, DataTable table, ITaskItemsCollection prms)
        {
            DataTable deletedRows = table.GetChanges(DataRowState.Deleted);
            if ((deletedRows != null) && (deletedRows.Rows.Count > 0))
            {
                foreach (DataRow deleted in deletedRows.Rows)
                {
                    ITaskParamBase prm;
                    int id = Convert.ToInt32(deleted["ID", DataRowVersion.Original]);
                    if (!IsConsts(sender))
                    {
                        prm = ((ITaskParamsCollection)prms).ParamByID(id);
                        if (prm != null)
                            ((ITaskParamsCollection)prms).Remove((ITaskParam)prm);
                    }
                    else
                    {
                        prm = ((ITaskConstsCollection)prms).ConstByID(id);
                        if (prm != null)
                            ((ITaskConstsCollection)prms).Remove((ITaskConst)prm);
                    }
                }
            }
        }

        private void ugeConstsParams_OnCancelChanges(object sender)
        {
            if (!IsConsts(sender))
            {
                _tasksView.ugeParams.DataSource = null;
                ActiveTask.GetTaskParams().CancelChanges();
            }
            else
            {
                _tasksView.ugeConsts.DataSource = null;
                ActiveTask.GetTaskConsts().CancelChanges();
            }
            ugeConstsParams_OnRefreshData(sender);
        }

        private bool ugeConstsParams_OnRefreshData(object sender)
        {
            CanDeactivate = true;
            DataTable dt;
            ITaskItemsCollection items;
            if (!IsConsts(sender))
            {
                dt = GetParamsTable();
                items = ActiveTask.GetTaskParams();
            }
            else
            {
                dt = GetConstsTable();
                items = ActiveTask.GetTaskConsts();
            }

            if (((dt != null) && (dt.GetChanges() != null)) || 
                (items.HasChanges()))
            {
                ugeConstsParams_OnSaveChanges(sender);
            }
            
            if (!IsConsts(sender))
            {
                ActiveTask.GetTaskParams().ReloadItemsTable();
                dt = ActiveTask.GetTaskParams().ItemsTable;
                _tasksView.ugeParams.DataSource = dt;
            }
            else
            {
                ActiveTask.GetTaskConsts().ReloadItemsTable();
                dt = ActiveTask.GetTaskConsts().ItemsTable;
                _tasksView.ugeConsts.DataSource = dt;
            }
             
            // была произведена разыменовка, фиксируем
            dt.AcceptChanges();
            return true;
        }

        CC.GridColumnsStates paramsColumnsStates = null;

        public CC.GridColumnsStates ugeParams_OnGetGridColumnsState(object sender)
        {
            if (paramsColumnsStates != null)
                return paramsColumnsStates;

            paramsColumnsStates = new CC.GridColumnsStates();
            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.ColumnPosition = 0;
            state.ColumnWidth = 40;
            state.IsSystem = true;
            paramsColumnsStates.Add(state.ColumnName, state);

            // Name 
            state = new CC.GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Название";
            paramsColumnsStates.Add(state.ColumnName, state);
            // Dimension 
            state = new CC.GridColumnState();
            state.ColumnName = "Dimension";
            state.ColumnCaption = "Измерение";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            state.IsLookUp = true;
            state.IsReadOnly = true;
            state.ColumnWidth = 200;

            paramsColumnsStates.Add(state.ColumnName, state);
            // AllowMultiSelect 
            state = new CC.GridColumnState();
            state.ColumnName = "AllowMultiSelect";
            state.ColumnCaption = "Множественный выбор";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            state.ColumnWidth = 40;
            paramsColumnsStates.Add(state.ColumnName, state);
            // Description 
            state = new CC.GridColumnState();
            state.ColumnName = "Description";
            state.ColumnCaption = "Описание";
            paramsColumnsStates.Add(state.ColumnName, state);
            // ParamValues 
            state = new CC.GridColumnState();
            state.ColumnName = "ParamValues";
            state.IsHiden = true;
            state.IsLookUp = true;
            paramsColumnsStates.Add(state.ColumnName, state);
            //Разыменовка значений
            state = new CC.GridColumnState();
            state.ColumnName = "ParsedParamValues";
            state.ColumnCaption = "Значение";
            state.IsReadOnly = true;
            state.IsLookUp = true;
            //state.ColumnWidth = 1000;
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            paramsColumnsStates.Add(state.ColumnName, state);
            // ParamType 
            state = new CC.GridColumnState();
            state.ColumnName = "ParamType";
            state.IsHiden = true;
            paramsColumnsStates.Add(state.ColumnName, state);
            // RefTasks
            state = new CC.GridColumnState();
            state.ColumnName = "RefTasks";
            state.IsHiden = true;
            paramsColumnsStates.Add(state.ColumnName, state);
            // Inherited    
            state = new CC.GridColumnState();
            state.ColumnName = "Inherited";
            state.IsHiden = true;
            paramsColumnsStates.Add(state.ColumnName, state);
            return paramsColumnsStates;
        }

        CC.GridColumnsStates constsColumnsStates = null;
        
        public CC.GridColumnsStates ugeConsts_OnGetGridColumnsState(object sender)
        {
            if (constsColumnsStates != null)
                return constsColumnsStates;

            constsColumnsStates = new CC.GridColumnsStates();
            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.ColumnPosition = 0;
            state.ColumnWidth = 40;
            state.IsSystem = true;
            constsColumnsStates.Add(state.ColumnName, state);

            // Name 
            state = new CC.GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Название";
            constsColumnsStates.Add(state.ColumnName, state);
            // Description 
            state = new CC.GridColumnState();
            state.ColumnName = "Description";
            state.ColumnCaption = "Описание";
            constsColumnsStates.Add(state.ColumnName, state);
            // Dimension 
            state = new CC.GridColumnState();
            state.ColumnName = "Dimension";
            state.IsHiden = true;
            constsColumnsStates.Add(state.ColumnName, state);
            // AllowMultiSelect 
            state = new CC.GridColumnState();
            state.ColumnName = "AllowMultiSelect";
            state.IsHiden = true;
            constsColumnsStates.Add(state.ColumnName, state);
            // ParamValues 
            state = new CC.GridColumnState();
            state.ColumnName = "ParamValues";
            state.ColumnCaption = "Значение";
            //state.ColumnWidth = 1000;
            constsColumnsStates.Add(state.ColumnName, state);
            // ParamType 
            state = new CC.GridColumnState();
            state.ColumnName = "ParamType";
            state.IsHiden = true;
            constsColumnsStates.Add(state.ColumnName, state);
            // RefTasks
            state = new CC.GridColumnState();
            state.ColumnName = "RefTasks";
            state.IsHiden = true;
            constsColumnsStates.Add(state.ColumnName, state);
            // Inherited    
            state = new CC.GridColumnState();
            state.ColumnName = "Inherited";
            state.IsHiden = true;
            constsColumnsStates.Add(state.ColumnName, state);
            return constsColumnsStates;
        }

        private static void ugeConstsParams_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //UltraGrid ug = (UltraGrid)sender;
            e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            //if (IsConsts(sender))
            //    return;
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                band.ColHeaderLines = 2;
                UltraGridColumn clmn;
                if (!IsConsts(sender))
                {
                    clmn = band.Columns["Dimension"];
                    clmn.AutoSizeMode = ColumnAutoSizeMode.AllRowsInBand;
                    clmn.CellMultiLine = DefaultableBoolean.True;
                    //clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    //Разыменовка значений
                    clmn = band.Columns["ParsedParamValues"];
                    //clmn.Width = 1000;
                    //clmn.Header.Caption = "Значение";
                    clmn.AutoSizeMode = ColumnAutoSizeMode.AllRowsInBand;
                    clmn.CellMultiLine = DefaultableBoolean.True;
                    //clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                }
                else
                {
                    clmn = band.Columns["ParamValues"];
                    clmn.AutoSizeMode = ColumnAutoSizeMode.AllRowsInBand;
                    clmn.CellMultiLine = DefaultableBoolean.True;
                    //clmn.Width = 1000;
                }
            }
        }

        private void ugeConstsParams_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //if (e.Row.IsAddRow)
            //    return;
            //UltraGrid ug = (UltraGrid)sender;
            bool isConsts = IsConsts(sender);

            // доступность колонки для редактирования
            bool isInherited = false;
            try
            {
                isInherited = Convert.ToInt32(CC.UltraGridHelper.GetRowCells(e.Row).Cells["Inherited"].Value) == 1;
            }
            catch
            {
            }
            if (isInherited)
                e.Row.Activation = Activation.Disabled;
            else
                e.Row.Activation = Activation.AllowEdit;

            // разыменовка значений
            if (!isConsts)
            {
                // разыменовка значений параметров
                string xmlValue = Convert.ToString(e.Row.Cells["ParamValues"].Value);
                //string oldParsedValue = Convert.ToString(e.Row.Cells["ParsedParamValues"].Value);
                string newParsedValue = TaskParamDimEditor.GetMembersText(xmlValue);//soapDimEditorHelper.GetTextMemberList(xmlValue);

                //if (!String.IsNullOrEmpty(newParsedValue))
                //    newParsedValue = newParsedValue.Replace("\n", Environment.NewLine);
                try
                {
                    e.Row.Cells["ParsedParamValues"].Value = newParsedValue;
                    e.Row.Update();
                }
                catch
                {
                    // заглушка: при удалении строки грида производится попытка обращения к удаленной строке дататэйбла
                    // ничего не будет делать, пусть глючит пока
                }
            }
        }

        private void ugeParams_ClickCellButton(object sender, CellEventArgs e)
        {
            int _authType = 0;
            string _login = "";
            string _pwdHash = "";
            try
            {
                ClientSession.GetAuthenticationInfo(Workplace.ActiveScheme, out _authType, out _login, out _pwdHash);
            }
            catch { }

            string serverAddress = string.Format("{0}:{1}", Workplace.ActiveScheme.Server.Machine,
                            Workplace.ActiveScheme.Server.GetConfigurationParameter("ServerPort"));
            switch (e.Cell.Column.Key.ToUpper())
            {
                case "PARSEDPARAMVALUES":
                    string oldDimensionName = Convert.ToString(e.Cell.Row.Cells["Dimension"].Value);
                    if (String.IsNullOrEmpty(oldDimensionName))
                    {
                        MessageBox.Show("Измерение не выбрано", "Выбор значения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string oldValue = Convert.ToString(e.Cell.Row.Cells["ParamValues"].Value);

                    TaskParamDimEditor taskParamDimEditor = new TaskParamDimEditor(ActiveTask.InEdit);
                    string newValue = taskParamDimEditor.EditDimension(Workplace, oldDimensionName, oldValue);

                   // если была ошибка - сообщаем о ней
                   if (taskParamDimEditor.LastError != string.Empty)
                    {
                        MessageBox.Show(taskParamDimEditor.LastError, "Выбор значения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if ((!String.IsNullOrEmpty(newValue)) 
                        &&((oldValue != newValue))
                        &&(!ActiveTask.GetTaskParams().IsReadOnly))
                    {
                        e.Cell.Row.Cells["ParamValues"].Value = newValue;

                        bool multiSelection = Convert.ToBoolean(e.Cell.Row.Cells["AllowMultiSelect"].Value);
                        XmlDocument valueDom = new XmlDocument();
                        valueDom.LoadXml(newValue);
                        XmlNodeList nl = valueDom.SelectNodes("//Member[@checked='true']");
                        if (nl != null)
                            e.Cell.Row.Cells["AllowMultiSelect"].Value = multiSelection || (nl.Count > 1);

                        e.Cell.Row.Update();
                        e.Cell.Row.Refresh(RefreshRow.FireInitializeRow, false);
                    }
                    break;
                case "DIMENSION":
                    oldDimensionName = Convert.ToString(e.Cell.Row.Cells["Dimension"].Value);

                    TaskParamDimChooser taskParamDimChooser = new TaskParamDimChooser(ActiveTask.InEdit);
                    string newDimensionName = taskParamDimChooser.SelectDimension(Workplace, oldDimensionName);
                    
                    // если была ошибка - сообщаем о ней
                    if (taskParamDimChooser.LastError != string.Empty)
                    {
                        MessageBox.Show(taskParamDimChooser.LastError, "Выбор измерения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    if ((!ActiveTask.GetTaskParams().IsReadOnly)
                        && (oldDimensionName != newDimensionName) && (newDimensionName != string.Empty))
                    {
                        e.Cell.Row.Cells["Dimension"].Value = newDimensionName;
                        e.Cell.Row.Cells["ParamValues"].Value = String.Empty;
                        e.Cell.Row.Update();
                        e.Cell.Row.Refresh(RefreshRow.FireInitializeRow, false);
                    }
                    break;
            }
        }

        private void ugeConstsParams_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            if ((!row.IsAddRow) || (!(String.IsNullOrEmpty(row.Cells["ID"].Value.ToString())))) 
                return;
            UltraGrid ug = (UltraGrid)sender;
            bool isConsts = IsConsts(sender);

            ug.EventManager.SetEnabled(EventGroups.AllEvents, false);
            try
            {
                ITaskParamBase prm;
                if (!isConsts)
                {
                    prm = ActiveTask.GetTaskParams().AddNew();
                    row.Cells["ID"].Value = ((ITaskParam)prm).ID;
                    row.Cells["AllowMultiSelect"].Value = ((ITaskParam)prm).AllowMultiSelect;
                    row.Cells["Name"].Value = ((ITaskParam)prm).Name;
                    row.Cells["Description"].Value = ((ITaskParam)prm).Comment;
                    row.Cells["Dimension"].Value = ((ITaskParam)prm).Dimension;
                    row.Cells["ParamValues"].Value = ((ITaskParam)prm).Values;
                    row.Cells["Inherited"].Value = ((ITaskParam)prm).Inherited;
                    row.Cells["ParamType"].Value = (int)TaskParameterType.taskParameter;
                    row.Cells["RefTasks"].Value = ActiveTask.ID;
                }
                else
                {
                    prm = ActiveTask.GetTaskConsts().AddNew();
                    row.Cells["ID"].Value = ((ITaskConst)prm).ID;
                    row.Cells["AllowMultiSelect"].Value = false;
                    row.Cells["Name"].Value = ((ITaskConst)prm).Name;
                    row.Cells["Description"].Value = ((ITaskConst)prm).Comment;
                    row.Cells["ParamValues"].Value = ((ITaskConst)prm).Values;
                    row.Cells["Inherited"].Value = ((ITaskConst)prm).Inherited;
                    row.Cells["ParamType"].Value = (int)TaskParameterType.taskConst;
                    row.Cells["RefTasks"].Value = ActiveTask.ID;
                }
                row.Update();
            }
            finally
            {
                ug.EventManager.SetEnabled(EventGroups.AllEvents, true);
            }
        }

        /*
        private static void ugeParams_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            /*UltraGrid ug = (UltraGrid)sender;
            UltraGridCell cell = ug.ActiveCell;
            if ((cell == null) || (!cell.DataChanged) || (cell.Column.Key.ToUpper() != "NAME"))
                return;

            string newName = Convert.ToString(cell.EditorResolved.CurrentEditText);
            DataRow[] existing = GetParamsTable().Select(String.Format("Name LIKE '{0}'", newName));
            if (existing.Length == 0)
                return;

            cell.EditorResolved.Value = cell.OriginalValue;

            if ((!e.ForceExit) && (!e.CancellingEditOperation))
            {
                e.Cancel = true;
                cell.CancelUpdate();
                MessageBox.Show(String.Format("Коллекция уже содержит элемент с именем '{0}'", newName),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/
        
        private void SetParentTaskConstPatams(bool setConst)
        {
            List<int> tasksIDs = new List<int>();
            DataTable dtTasks = Workplace.ActiveScheme.TaskManager.Tasks.GetTasksInfo();
            GetTasksList(tasksIDs, dtTasks, ActiveTask.ID);
            tasksIDs.Remove(ActiveTask.ID);
            foreach (int childsID in tasksIDs)
            {
                if (_tasksNavigation.returnedTasks.Contains(childsID))
                {
                    TaskStub taskStub = _tasksNavigation.returnedTasks[childsID];
                    if (setConst)
                        taskStub.GetTaskConsts().ReloadItemsTable();
                    else
                        taskStub.GetTaskParams().ReloadItemsTable();
                }
                else
                {
                    ITask task = Workplace.ActiveScheme.TaskManager.Tasks[childsID];
                    if (setConst)
                        task.GetTaskConsts().ReloadItemsTable();
                    else
                        task.GetTaskParams().ReloadItemsTable();
                }
            }
        }

        private static void GetTasksList(List<int> taskIDs, DataTable dtTasks, int currentTaskID)
        {
            DataRow[] childs = dtTasks.Select(string.Format("REFTASKS = {0}", currentTaskID));
            foreach (DataRow childsRow in childs)
            {
                int id = Convert.ToInt32(childsRow["ID"]);
                taskIDs.Add(id);
                GetTasksList(taskIDs, dtTasks, id);
            }
        }
    }
}