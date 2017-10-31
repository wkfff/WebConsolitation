using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class TasksSearch : Form
    {
        private DataTable findedTasks = null;
        public DataTable FindedTasks
        {
            get { return findedTasks; }
            set
            {
                findedTasks = value;
                ugFindedTasks.DataSource = null;
                ugFindedTasks.ResetLayouts();
                ugFindedTasks.DataSource = findedTasks;
            }
        }
        
        private DataTable findedDocuments = null;
        public DataTable FindedDocuments
        {
            get { return findedDocuments; }
            set
            {
                findedDocuments = value;
                ugFindedDocs.DataSource = null;
                ugFindedDocs.ResetLayouts();
                ugFindedDocs.DataSource = findedDocuments;
            }
        }

        public TasksSearch()
        {
            InitializeComponent();
            ComponentCustomizer.CustomizeInfragisticsControls(this);
            ComponentCustomizer.CustomizeInfragisticsComponents(components);
            ugFindedDocs.BeforeCustomRowFilterDialog += new BeforeCustomRowFilterDialogEventHandler(CustomizeUltraGridRowFilterDialog);
            ugFindedTasks.BeforeCustomRowFilterDialog += new BeforeCustomRowFilterDialogEventHandler(CustomizeUltraGridRowFilterDialog);
        }

        // Обработчик кустомизации диалога фильтрации UltraWinGrid - самый простой способ это разместить его здесь
        public void CustomizeUltraGridRowFilterDialog(object sender, BeforeCustomRowFilterDialogEventArgs e)
        {
            if ((e.CustomRowFiltersDialog != null) && (e.CustomRowFiltersDialog.Grid != null))
            {
                UltraGridHelper.CustomizeRowFilerDialog(e.CustomRowFiltersDialog, e.Column.Header.Caption);
            }
        }

        public enum SearchKind {skUndefined, skTasks, skDocuments};

        public SearchKind CurrentSearchKind
        {
            get            
            { 
                switch (utcSearchParameters.ActiveTab.Key)
                {
                    case "utTasks":
                        return SearchKind.skTasks;
                    case "utDocuments":
                        return SearchKind.skDocuments;
                    default:
                        return SearchKind.skUndefined;
                }
            }
        }

        private static string GetSQLStrPrmValue(string value)
        {
            string res = value.Replace((char)39, ' ');
            res = res.Replace('"', ' ');
            return res;
        }

        private static void AppendSQLStrPrmExpression(StringBuilder expressions, string prmName, string prmValue,
            bool useUpper)
        {
            if (!String.IsNullOrEmpty(prmValue))
            {
                if (useUpper)
                    expressions.AppendFormat("(UPPER({0}) LIKE UPPER('%{1}%'))", prmName, GetSQLStrPrmValue(prmValue));
                else
                    expressions.AppendFormat("({0} LIKE '%{1}%')", prmName, GetSQLStrPrmValue(prmValue));
            }
            else
            {
                expressions.AppendFormat("(({0} is Null) or ({1} = ''))", prmName, prmName);
            }
        }

        private static string DateTimeToFilterParam(DateTime val, bool roundToUpper)
        {
            if (roundToUpper)
                return val.ToString("yyyy/MM/dd 23:59:59");
            else
                return val.ToString("yyyy/MM/dd 00:00:00");
        }

        private Dictionary<string, string> lastTaskSearchConditions = new Dictionary<string, string>();

        private string GetTasksSearchConditions()
        {
            StringBuilder sb = new StringBuilder();
            //IUsersManager um = this.activeScheme.UsersManager;
            int? userID;
            lastTaskSearchConditions.Clear();

            #region Основные поля
            if ((cbTaskNumber.Checked) && (uneTaskNumber.Value.ToString() != String.Empty))
            {
                sb.AppendFormat("(ID = {0})", Convert.ToInt32(uneTaskNumber.Value));
                lastTaskSearchConditions.Add("ID", Convert.ToString(uneTaskNumber.Value));
            };

            if ((cbOwner.Checked) && (tbOwner.Text != String.Empty))
            {
                userID = (int?)tbOwner.Tag;
                if (userID != null)
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.AppendFormat("(Owner = {0})", (int)userID);
                    //lastTaskSearchConditions.Add("Владелец", 
                }
            };

            if ((cbDoer.Checked) && (tbDoer.Text != String.Empty))
            {
                userID = (int?)tbDoer.Tag;
                if (userID != null)
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.AppendFormat("(Doer = {0})", (int)userID);
                }
            };

            if ((cbCurator.Checked) && (tbCurator.Text != String.Empty))
            {
                userID = (int?)tbCurator.Tag;
                if (userID != null)
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.AppendFormat("(Curator = {0})", (int)userID);
                }
            };

           if (cbLocked.Checked)
           {
                // если выбран конкретный пользователь
                if ((cbLockedUser.Checked) && (tbLocked.Text != String.Empty))
                {
                    userID = (int?)tbLocked.Tag;
                    if (userID != null)
                    {
                        if (sb.Length != 0) sb.Append("AND");
                        sb.AppendFormat("(LockByUser = {0})", (int)userID);
                    }
                }
                // если выбран только признак блокировки
                else
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.Append("(not(LockByUser is NULL))");
                }
            };


            if (cbState.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "State", cbxState.Text, false);
            };

            if (cbTaskName.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "Headline", tbTaskName.Text, false);
            };

            if (cbJob.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "Job", tbJob.Text, false);
            };

            if (cbComment.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "Description", tbComment.Text, false);
            };

            if (cbTaskTypes.Checked)
            {
                if (udeTaskTypes.SelectedItem != null)
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.AppendFormat("(REFTASKSTYPES = {0})", udeTaskTypes.SelectedItem.DataValue);
                }
            }
            #endregion

            #region Даты
            if (cbStartDateFrom.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                sb.Append(String.Format("(FromDate >= '{0}')", 
                    DateTimeToFilterParam(dtpStartDateFrom.Value, false)));
            }

            if (cbStartDateTo.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                sb.Append(String.Format("(FromDate <= '{0}')",
                    DateTimeToFilterParam(dtpStartDateTo.Value, true)));
            }

            if (cbEndDateFrom.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                sb.Append(String.Format("(ToDate >= '{0}')", 
                    DateTimeToFilterParam(dtpEndDateFrom.Value, false)));
            }

            if (cbEndDateTo.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                sb.Append(String.Format("(ToDate <= '{0}')", 
                    DateTimeToFilterParam(dtpEndDateTo.Value, true)));
            }
            #endregion

            return sb.ToString();
        }

        private string BuildFindedTasksConstraint()
        {
            // если найденных задач нет - делаем таак чтобы запрос всегда возвращал 0 записей
            if ((FindedTasks == null) || (FindedTasks.Rows.Count == 0))
                return ("(doc.ID is NULL)");

            // формируем ограничение на ID задач
            StringBuilder sb = new StringBuilder();
            sb.Append("(doc.RefTasks in (");
            for (int i = 0; i <= FindedTasks.Rows.Count - 1; i++ )
            {
                DataRow row = FindedTasks.Rows[i];
                sb.Append(row["ID"]);
                if (i < FindedTasks.Rows.Count - 1)
                    sb.Append(", ");
            }
            sb.Append("))");
            return sb.ToString();
        }

        private string GetDocumentsSearchConditions()
        {
            StringBuilder sb = new StringBuilder();
            if ((cbDocNumber.Checked) && (uneDocNumber.Value.ToString() != String.Empty))
            {
                sb.AppendFormat("(doc.ID = {0})", Convert.ToInt32(uneDocNumber.Value));
            };

            // эти ограничения идут в SQL-выражения, поэтому делаем регистронезависимый поиск
            if (cbDocName.Checked) 
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "doc.Name", tbDocName.Text, true);
            };

            if (cbDocComment.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                AppendSQLStrPrmExpression(sb, "doc.Description", tbDocComment.Text, true);
            };

            if (cbSearchInSearchedTasks.Checked)
            {
                if (sb.Length != 0) sb.Append("AND");
                sb.Append(BuildFindedTasksConstraint());
            }
            return sb.ToString();
        }

        private IScheme activeScheme = null;
        private IUsersModal usersForm = null;

        public void PerformSearch(IScheme scheme, IUsersModal users, out int? taskID, out int? docID)
        {
            if (scheme == null)
                throw new Exception("Должен быть задан параметр 'scheme'");

            activeScheme = scheme;
            usersForm = users;
            taskID = null;
            docID = null;

            // список допустимых состояний
            if (cbxState.Items.Count == 0)
            {
                string[] allCaptions = activeScheme.TaskManager.Tasks.GetAllStatesCaptions();
                for (int i = 0; i < allCaptions.Length; i++)
                {
                    cbxState.Items.Add(allCaptions[i]);
                }
            }

            if (udeTaskTypes.Items.Count == 0)
            {
                DataTable dtTaskTypes = scheme.UsersManager.GetTasksTypes();
                foreach (DataRow row in dtTaskTypes.Rows)
                {
                    udeTaskTypes.Items.Add(row["ID"], row["Name"].ToString());
                }
            }

            if (ShowDialog() != DialogResult.OK)
                return;
            switch (CurrentSearchKind)
            {
                case SearchKind.skTasks:
                    taskID = UltraGridHelper.GetActiveID(ugFindedTasks);
                    if (taskID == -1)
                        taskID = null;
                    break;
                case SearchKind.skDocuments:
                    UltraGridRow row = UltraGridHelper.GetActiveRowCells(ugFindedDocs);
                    if (row != null)
                    {
                        taskID = Convert.ToInt32(row.Cells["REFTASKS"].Value);
                        docID = Convert.ToInt32(row.Cells["ID"].Value);
                    }
                    break;
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            switch (cb.Name)
            {
                #region Задачи
                case "cbTaskNumber":
                    uneTaskNumber.Enabled = cb.Checked;
                    break;
                case "cbOwner":
                    tbOwner.Enabled = cb.Checked;
                    btnSelectOwner.Enabled = cb.Checked;
                    break;
                case "cbDoer":
                    tbDoer.Enabled = cb.Checked;
                    btnSelectDoer.Enabled = cb.Checked;
                    break;
                case "cbCurator":
                    tbCurator.Enabled = cb.Checked;
                    btnSelectCurator.Enabled = cb.Checked;
                    break;
                case "cbLocked":
                    cbLockedUser.Enabled = cb.Checked;
                    tbLocked.Enabled = (cbLockedUser.Enabled) && (cbLockedUser.Checked);
                    btnSelectLocked.Enabled = (cbLockedUser.Enabled) && (cbLockedUser.Checked);
                    break;
                case "cbLockedUser":
                    tbLocked.Enabled = cb.Checked;
                    btnSelectLocked.Enabled = cb.Checked;
                    break;
                case "cbState":
                    cbxState.Enabled = cb.Checked;
                    break;
                case "cbTaskName":
                    tbTaskName.Enabled = cb.Checked;
                    break;
                case "cbJob":
                    tbJob.Enabled = cb.Checked;
                    break;
                case "cbComment":
                    tbComment.Enabled = cb.Checked;
                    break;
                case "cbStartDateFrom":
                    dtpStartDateFrom.Enabled = cb.Checked;
                    break;
                case "cbStartDateTo":
                    dtpStartDateTo.Enabled = cb.Checked;
                    break;
                case "cbEndDateFrom":
                    dtpEndDateFrom.Enabled = cb.Checked;
                    break;
                case "cbEndDateTo":
                    dtpEndDateTo.Enabled = cb.Checked;
                    break;
                case "cbCustomFilter":
                    cbxCustomFilter.Enabled = cb.Checked;
                    break;
                case "cbTaskTypes":
                    udeTaskTypes.Enabled = cb.Checked;
                    break;
                #endregion

                #region Документы
                case "cbDocNumber":
                    uneDocNumber.Enabled = cb.Checked;
                    break;
                case "cbDocName":
                    tbDocName.Enabled = cb.Checked;
                    break;
                case "cbDocComment":
                    tbDocComment.Enabled = cb.Checked;
                    break;
                #endregion
            }
        }

        private void btnShowTasks_Click(object sender, EventArgs e)
        {
            DataTable allTasks = activeScheme.TaskManager.Tasks.GetTasksInfo();
            DataTable filteredTasks = allTasks.Clone();
            string vsblFilter = String.Format("(visible = {0})", (int)TaskVisibleInNavigation.tvVisible);
            string addFilter = GetTasksSearchConditions();
            string fullFilter = vsblFilter;
            if (addFilter != String.Empty)
                fullFilter = fullFilter + " AND " + addFilter;
            DataRow[] filteredRows = allTasks.Select(fullFilter);
            if (filteredRows.Length > 0)
            {
                filteredTasks.BeginLoadData();
                try
                {
                    foreach (DataRow row in filteredRows)
                    {
                        filteredTasks.Rows.Add(row.ItemArray);
                    }
                }
                finally
                {
                    filteredTasks.EndLoadData();
                }
            }
            FindedTasks = filteredTasks;
        }

        private void btnShowDocuments_Click(object sender, EventArgs e)
        {
            DataTable allTasks = activeScheme.TaskManager.Tasks.GetTasksInfo();
            StringBuilder sb = new StringBuilder();
            sb.Append("(tsks.ID in (");
            foreach (DataRow row in allTasks.Rows)
            {
                TaskVisibleInNavigation vsbl = (TaskVisibleInNavigation)Convert.ToInt32(row["visible"]);
                if (vsbl == TaskVisibleInNavigation.tvVisible)
                    sb.AppendFormat("{0}, ", Convert.ToString(row["ID"]));
            }
            string idFilter = sb.ToString();
            idFilter = idFilter.Substring(0, idFilter.Length - 2) + "))";
            string addFilter = GetDocumentsSearchConditions();

            string fullFilter = idFilter;
            if (addFilter != String.Empty)
                fullFilter = fullFilter + " AND " + addFilter;

            DataTable dt = activeScheme.TaskManager.Tasks.FindDocuments(fullFilter);

            FindedDocuments = dt;
        }

        private void btnSelectUserClick(object sender, EventArgs e)
        {
            string userName = string.Empty;
            int userID = 0;
            Button btn = (Button)sender;
            if (usersForm == null)
                return;
            if (usersForm.ShowModal(NavigationNodeKind.ndAllUsers, ref userID, ref userName))
            {
                switch (btn.Name)
                {
                    case "btnSelectOwner":
                        tbOwner.Text = userName;
                        tbOwner.Tag = userID;
                        break;
                    case "btnSelectDoer":
                        tbDoer.Text = userName;
                        tbDoer.Tag = userID;
                        break;
                    case "btnSelectCurator":
                        tbCurator.Text = userName;
                        tbCurator.Tag = userID;
                        break;
                    case "btnSelectLocked":
                        tbLocked.Text = userName;
                        tbLocked.Tag = userID;
                        break;
                }
            }
        }

        #region Инициализация  гридов
        private void ugFindedTasks_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.RowSizing = RowSizing.Default; 
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand gb = e.Layout.Bands[i];
                UltraGridColumn clmn;

                if (gb.Columns.IndexOf("clmnGoTo") < 0)
                {
                    clmn = gb.Columns.Add("clmnGoTo");
                    UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, 4);
                    clmn.Header.VisiblePosition = 0;
                }
                
                if (gb.Columns.IndexOf("clmnStatePic") < 0)
                {
                    clmn = gb.Columns.Add("clmnStatePic");
                    clmn.AllowGroupBy = DefaultableBoolean.False;
                    clmn.AllowRowFiltering = DefaultableBoolean.False;
                    clmn.AllowRowSummaries = AllowRowSummaries.False;
                    clmn.CellActivation = Activation.NoEdit;
                    clmn.Header.Caption = "";
                    clmn.Header.VisiblePosition = 1;
                    clmn.LockedWidth = true;
                    clmn.Width = 20;
                }
                
                clmn = gb.Columns["ID"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.VisiblePosition = 2;
                clmn.Width = 38;

                clmn = gb.Columns["HeadLine"];
                clmn.Header.Caption = "Наименование";
                clmn.Header.VisiblePosition = 3;
                clmn.Width = 146;

                clmn = gb.Columns["State"];
                clmn.Header.Caption = "Состояние";
                clmn.Header.VisiblePosition = 4;

                clmn = gb.Columns["FromDate"];
                clmn.Header.Caption = "Дата начала";
                clmn.Header.VisiblePosition = 5;
                clmn.Width = 89;

                clmn = gb.Columns["ToDate"];
                clmn.Header.Caption = "Дата окончания";
                clmn.Header.VisiblePosition = 6;
                clmn.Width = 93;

                clmn = gb.Columns["Owner"];
                clmn.Header.Caption = "Владелец";
                clmn.Hidden = true;

                clmn = gb.Columns["Doer"];
                clmn.Header.Caption = "Исполнитель";
                clmn.Width = 63;
                clmn.Hidden = true;

                clmn = gb.Columns["Curator"];
                clmn.Width = 63;
                clmn.Hidden = true;

                clmn = gb.Columns["LockByUser"];
                clmn.Width = 63;
                clmn.Hidden = true;

                clmn = gb.Columns["visible"];
                clmn.Hidden = true;

                clmn = gb.Columns["RefTasksTypes"];
                clmn.Hidden = true;

                clmn = gb.Columns["RefTasks"];
                clmn.Hidden = true;

                clmn = gb.Columns["LockedUserName"];
                clmn.Hidden = true;

                clmn = gb.Columns["StateCode"];
                clmn.Hidden = true;

                // OwnerName
                clmn = gb.Columns["OwnerName"];
                clmn.Header.Caption = "Владелец";
                clmn.Header.VisiblePosition = 7;

                // DoerName
                clmn = gb.Columns["DoerName"];
                clmn.Header.Caption = "Исполнитель";
                clmn.Header.VisiblePosition = 8;

                // CuratorName
                clmn = gb.Columns["CuratorName"];
                clmn.Header.Caption = "Куратор";
                clmn.Header.VisiblePosition = 9;

                clmn = gb.Columns["Job"];
                clmn.Header.Caption = "Задание";
                clmn.Width = 150;
                clmn.Header.VisiblePosition = 10;

                clmn = gb.Columns["Description"];
                clmn.Header.Caption = "Комментарий";
                clmn.Width = 150;
                clmn.Header.VisiblePosition = 11;

            }

        }

        private void ugFindedDocs_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.RowSizing = RowSizing.Default; 
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand band = e.Layout.Bands[i];
                UltraGridColumn clmn;

                if (band.Columns.IndexOf("clmnGoTo") < 0)
                {
                    clmn = band.Columns.Add("clmnGoTo");
                    UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, 4);
                    clmn.Header.VisiblePosition = 0;
                }
             
                clmn = band.Columns["ID"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.VisiblePosition = 1;
                clmn.Width = 38;

                // поля от документа
                clmn = band.Columns["DocumentType"];
                clmn.Hidden = true;

                clmn = band.Columns["Name"];
                clmn.Header.Caption = "Название документа";
                clmn.Header.VisiblePosition = 2;
                clmn.Width = 344;

                clmn = band.Columns["Version"];
                clmn.Header.Caption = "Версия";
                clmn.Header.VisiblePosition = 3;

                clmn = band.Columns["Description"];
                clmn.Header.Caption = "Комментарий";
                clmn.Header.VisiblePosition = 4;

                clmn = band.Columns.Add("DocumentTypeName");
                clmn.Header.Caption = "Тип документа";
                clmn.Header.VisiblePosition = 5;
                clmn.Width = 130;

                clmn = band.Columns["Ownership"];
                clmn.Hidden = true;

                clmn = band.Columns.Add("clmnOwnershipName");
                clmn.Header.Caption = "Принадлежность";
                clmn.Header.VisiblePosition = 6;
                clmn.Width = 200;

                clmn = band.Columns["SOURCEFILENAME"];
                clmn.Header.Caption = "Название";
                clmn.Width = 130;
                clmn.Hidden = true;

                clmn = band.Columns["REFTASKS"];
                clmn.Hidden = true;

                // поля от задачи
                clmn = band.Columns["HeadLine"];
                clmn.Header.Caption = "Наименование задачи";
                clmn.Header.VisiblePosition = 7;
                clmn.Width = 146;

                clmn = band.Columns["State"];
                clmn.Header.Caption = "Состояние задачи";
                clmn.Header.VisiblePosition = 8;

                clmn = band.Columns["FromDate"];
                clmn.Header.Caption = "Дата начала";
                clmn.Header.VisiblePosition = 9;
                clmn.Width = 89;

                clmn = band.Columns["ToDate"];
                clmn.Header.Caption = "Дата окончания";
                clmn.Header.VisiblePosition = 10;
                clmn.Width = 93;

                clmn = band.Columns["Owner"];
                clmn.Hidden = true;

                clmn = band.Columns["Doer"];
                clmn.Hidden = true;

                clmn = band.Columns["Curator"];
                clmn.Hidden = true;

                clmn = band.Columns.Add("OwnerName");
                clmn.Header.Caption = "Владелец задачи";
                clmn.Header.VisiblePosition = 11;

                clmn = band.Columns.Add("DoerName");
                clmn.Header.Caption = "Исполнитель задачи";
                clmn.Header.VisiblePosition = 12;

                clmn = band.Columns.Add("CuratorName");
                clmn.Header.Caption = "Куратор задачи";
                clmn.Header.VisiblePosition = 13;
            }
        }
      
        private void ugFindedTasks_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            IUsersManager um = activeScheme.UsersManager;

            // хинт для кнопки перехода
            e.Row.Cells["clmnGoTo"].ToolTipText = "Перейти к задаче";
            // заблокированность задачи
            int? lockedUser = null;
            try
            {
                lockedUser = Convert.ToInt32(e.Row.Cells["LockByUser"].Value);
            }
            catch { }
            // устанавливаем нужную картинку
            UltraGridCell picCell = e.Row.Cells["clmnStatePic"];
            // по умолчанию - активна и свободна
            picCell.Appearance.Image = 0;
            if ((lockedUser == null) || (lockedUser == -1))
            {
                // проверим не закрыта ли задача
                // состояние задачи
                TaskStates ts = TaskStates.tsUndefined;
                try
                {
                    string stateCaption = e.Row.Cells["State"].Value.ToString();
                    ts = activeScheme.TaskManager.Tasks.FindStateFromCaption(stateCaption);
                }
                catch { }
                if (ts == TaskStates.tsClosed)
                {
                    picCell.Appearance.Image = 1;
                    picCell.ToolTipText = "Задача закрыта";
                }
                else
                {
                    picCell.Appearance.Image = 0;
                    picCell.ToolTipText = "Задача свободна";
                }
            }
            else
            {
                // заблокировано текущим пользователем
                if ((int)lockedUser == activeScheme.UsersManager.GetCurrentUserID())
                {
                    picCell.Appearance.Image = 3;
                    picCell.ToolTipText = "Задача заблокирована текущим пользователем";
                }
                // заблокировано кем-то другим
                else
                {
                    picCell.Appearance.Image = 2;
                    picCell.ToolTipText = String.Format("Задача заблокирована пользователем '{0}'", um.GetUserNameByID((int)lockedUser));
                }
            }

            // владелец
            int userID = Convert.ToInt32(e.Row.Cells["Owner"].Value);
            e.Row.Cells["OwnerName"].Value = um.GetUserNameByID(userID);

            // исполнитель
            userID = Convert.ToInt32(e.Row.Cells["Doer"].Value);
            e.Row.Cells["DoerName"].Value = um.GetUserNameByID(userID);

            // куратор
            userID = Convert.ToInt32(e.Row.Cells["Curator"].Value);
            e.Row.Cells["CuratorName"].Value = um.GetUserNameByID(userID);
        }

        private void ugFindedDocs_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            IUsersManager um = activeScheme.UsersManager;

            // хинт для кнопки перехода
            e.Row.Cells["clmnGoTo"].ToolTipText = "Перейти к документу";

            // владелец
            int userID = Convert.ToInt32(e.Row.Cells["Owner"].Value);
            e.Row.Cells["OwnerName"].Value = um.GetUserNameByID(userID);

            // исполнитель
            userID = Convert.ToInt32(e.Row.Cells["Doer"].Value);
            e.Row.Cells["DoerName"].Value = um.GetUserNameByID(userID);

            // куратор
            userID = Convert.ToInt32(e.Row.Cells["Curator"].Value);
            e.Row.Cells["CuratorName"].Value = um.GetUserNameByID(userID);


            // тип файла
            TaskDocumentType dt = TaskDocumentType.dtArbitraryDocument;
            try
            {
                int dtInt = Convert.ToInt32(e.Row.Cells["DocumentType"].Value);
                dt = (TaskDocumentType)dtInt;
            }
            catch { }
            // разыменовка типа документа
            string fileExt = String.Empty;
            try
            {
                fileExt = Path.GetExtension(e.Row.Cells["SourceFileName"].Value.ToString());
            }
            catch { };
            UltraGridCell cell = e.Row.Cells["DocumentTypeName"];
            cell.Value = TaskUtils.TaskDocumentTypeToString(dt, fileExt);
            // Принадлежность
            cell = e.Row.Cells["Ownership"];
            TaskDocumentOwnership ownership = (TaskDocumentOwnership)Convert.ToInt32(cell.Value);
            e.Row.Cells["clmnOwnershipName"].Value = TaskUtils.TaskDocumentOwnershipToString(ownership);
        }
        #endregion

        private void ugGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            e.Cell.Activated = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static void OnControl_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                tb.SelectAll();
                return;
            }
            if (sender is UltraNumericEditor)
            {
                UltraNumericEditor ed = (UltraNumericEditor)sender;
                ed.SelectAll();
                return;
            }
        }

    }
}