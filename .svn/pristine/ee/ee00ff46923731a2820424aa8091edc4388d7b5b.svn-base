using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ProtocolsUI
{
    public partial class ProtocolsViewObject : BaseViewObj, IInplaceProtocolView
    {
        // установить фильтр по дате на текущий день.
        // установить сложный фильтр по дате
        // получить данные по этому фильтру отфильтрованные
        // что бы не горели кнопочки
        // форма для отображения справочника с пользователями
        private IUsersModal usersModalForm;
        // служит для получения данных по пользователям
        private IInplaceTasksPermissionsView taskPermissions;

        private DataTable auditDataTable = new DataTable();

        /// <summary>
        /// отображения данных аудита в протоколах
        /// </summary>
        private void ShowAuditData()
        {
            string usersFilter;
            IDbDataParameter[] usersParams;
            // получаем значения фильтра, установленного пользователем
            GetServerFilterAndParams(out usersFilter, out usersParams);
            // если фильтр пользователем задан
            {
                // показываем впервый раз, делаем фильтр ручками по текущему дню
                if (usersFilter == string.Empty)
                {
                    ChangeAuditDateTimeFilterCondition();
                    auditDataTable = GetAuditData(" where " + DateFilterCondition, DateTimeFilterParams);
                }
                else
                {
                    GetServerFilterAndParams(out usersFilter, out usersParams);
                    auditDataTable = GetAuditData(" where " + usersFilter, usersParams);
                }

                pView.ugeAudit.DataSource = auditDataTable;

                // впервый раз делаем вид, что стоит сложный фильтр
                if (usersFilter == string.Empty)
                {
                    DateTime todayTime;
                    DateTime tomorowTime;
                    todayTime = GetTodayDateTime();
                    tomorowTime = todayTime.AddDays(1);
                    pView.ugeAudit.SetServerFilterValue(pView.ugeAudit.ugFilter, "CHANGETIME", todayTime, tomorowTime);
                }
            }
            SetSortDirection(pView.ugeAudit.ugData);
        }

        /// <summary>
        /// получает фильтр и параметры из пользовательского фильтра
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        private void GetServerFilterAndParams(out string filter, out IDbDataParameter[] filterParams)
        {
            List<Components.UltraGridEx.FilterParamInfo> parameters;
            pView.ugeAudit.BuildServerFilter(out filter, out parameters);
            filterParams = ParametersListToArray(parameters);
        }

        // обновление данных
        public void RefreshAttachAuditData()
        {
            RefreshAttachAuditData(string.Empty, null);
        }

        public void RefreshAttachAuditData(int pumpId, ClassTypes classType, string objectName, string filter, params IDbDataParameter[] filterParams)
        {
            // сохраняем фильтр если был задан
            if (filter != string.Empty)
                AttachFilter = filter;
            // сохраняем параметры
            if (filterParams != null)
                AttachFilterParams.Init(filterParams);

            string usersFilter;
            IDbDataParameter[] usersParams;
            GetServerFilterAndParams(out usersFilter, out usersParams);

            if (!string.IsNullOrEmpty(usersFilter))
                filter = filter + " and " + usersFilter;
            if (usersParams != null && usersParams.Length > 0)
                usersParams.CopyTo(filterParams, filterParams.Length);

            Workplace.OperationObj.Text = "Запрос данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                // получаем данные
                LogDataTable = (DataTable)GetAuditData(" where " + filter, filterParams);
                // получаем данные из истории закачки
                DataRow pumpRow = GetPumpHistoryRow(pumpId);
                if (pumpRow != null)
                {
                    DataRow row = LogDataTable.NewRow();
                    row["ID"] = 0;
                    row["CHANGETIME"] = pumpRow["PUMPDATE"];
                    row["OBJECTTYPE"] = GetObjectType(classType);
                    row["OBJECTNAME"] = objectName;
                    row["USERNAME"] = pumpRow["USERNAME"];
                    row["SESSIONID"] = pumpRow["SESSIONID"];
                    row["PUMPID"] = pumpRow["id"];
                    row["KINDOFOPERATION"] = 0;
                    row["RECORDID"] = filterParams[1].Value;
                    LogDataTable.Rows.Add(row);
                }

                // и отображаем их
                pView.ugeAudit.DataSource = LogDataTable;

                ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTTYPE"]).Checked = false;
                ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTNAME"]).Checked = false;
                ((StateButtonTool)pView.ugeAudit.utmMain.Tools["RECORDID"]).Checked = false;

                foreach (UltraGridBand band in pView.ugeAudit.ugData.DisplayLayout.Bands)
                {
                    band.Columns["CHANGETIME"].SortIndicator = SortIndicator.Descending;
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        private DataRow GetPumpHistoryRow(int pumpId)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string query = "select PUMPDATE, USERNAME, SESSIONID, id from PUMPHISTORY where id = ?";
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, new DbParameterDescriptor("p0", pumpId));
                if (dt.Rows.Count == 0)
                    return null;
                return dt.Rows[0];
            }
        }

        /// <summary>
        /// обновление данных аудита во внедренном состоянии
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        public void RefreshAttachAuditData(string filter, params IDbDataParameter[] filterParams)
        {
            // сохраняем фильтр если был задан
            if (filter != string.Empty)
                AttachFilter = filter;
            // сохраняем параметры
            if (filterParams != null)
                AttachFilterParams.Init(filterParams);

            // восстанавливаем параметры
            IDbDataParameter[] tmpParams = AttachFilterParams.ToParamsArray();
            // получаем фильтр и параметры, которые выставляются в настройке фильтра пользователем
            string usersFilter;
            IDbDataParameter[] usersParams;
            GetServerFilterAndParams(out usersFilter, out usersParams);

            // теперь нужно соединить фильтры и параметры
            List<IDbDataParameter> paramsList = new List<IDbDataParameter>();
            foreach (IDbDataParameter param in tmpParams)
            {
                paramsList.Add(param);
            }

            filter = AttachFilter;

            if (usersFilter != string.Empty)
            {
                if (usersParams != null)
                    foreach (IDbDataParameter param in usersParams)
                    {
                        paramsList.Add(param);
                    }
                filter = filter + " and " + usersFilter;
            }
            this.Workplace.OperationObj.Text = "Запрос данных";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                // получаем данные
                LogDataTable = (DataTable)GetAuditData(" where " + filter, paramsList.ToArray());
                // и отображаем их
                pView.ugeAudit.DataSource = LogDataTable;

                if (showAuditForObject == AuditShowObjects.ClsObject)
                {
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTTYPE"]).Checked = false;
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTNAME"]).Checked = false;
                }
                else if (showAuditForObject == AuditShowObjects.RowObject)
                {
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTTYPE"]).Checked = false;
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["OBJECTNAME"]).Checked = false;
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["RECORDID"]).Checked = false;
                }
                else if (showAuditForObject == AuditShowObjects.TaskObject)
                {
                    ((StateButtonTool)pView.ugeAudit.utmMain.Tools["TASKID"]).Checked = false;
                }

                foreach (UltraGridBand band in pView.ugeAudit.ugData.DisplayLayout.Bands)
                {
                    band.Columns["CHANGETIME"].SortIndicator = SortIndicator.Descending;
                }
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
        }


        private AuditShowObjects showAuditForObject;

        /// <summary>
        /// внедрение аудита
        /// </summary>
        /// <param name="parentArea"></param>
        /// <param name="fileName"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        public void AttachAudit(Control parentArea, string fileName, AuditShowObjects auditObject, string filter, params IDbDataParameter[] filterParams)
        {
            if (fileName == string.Empty)
                pView.ugex1.SaveLoadFileName = "Аудит";
            showAuditForObject = auditObject;
            AttachModuleType = ModulesTypes.AuditModule;
            
            pView.ugeAudit.Parent = parentArea;
            pView.ugeAudit.Dock = DockStyle.Fill;
            pView.ugeAudit.SaveLoadFileName = fileName;
            RefreshAttachAuditData(filter, filterParams);
            AttachFilter = filter;
        }

        /// <summary>
        /// внедрение аудита с информацией 
        /// </summary>
        public void AttachAudit(Control parentArea, string fileName, ClassTypes classType, string objectName,
            int pumpId, string auditFilter, params IDbDataParameter[] auditParams)
        {
            if (fileName == string.Empty)
                pView.ugex1.SaveLoadFileName = "Аудит";
            showAuditForObject = AuditShowObjects.RowObject;
            AttachModuleType = ModulesTypes.AuditModule;
            pView.ugeAudit.Parent = parentArea;
            pView.ugeAudit.Dock = DockStyle.Fill;
            pView.ugeAudit.SaveLoadFileName = fileName;
            RefreshAttachAuditData(pumpId, classType, objectName, auditFilter, auditParams);
        }
        

        /// <summary>
        /// получение данных из таблицы аудита
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <param name="queryResult"></param>
        /// <param name="FilterParams"></param>
        /// <returns></returns>
        private DataTable GetAuditData(string filter, params IDbDataParameter[] FilterParams)
        {
            DataTable dt = new DataTable();
            IDataOperations dop = this.Workplace.ActiveScheme.GetAudit();
            try
            {
                dop.GetAuditData(ref dt, filter, FilterParams);
            }
            finally
            {
                dop.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// получение сегодняшней даты со временем 00:00
        /// </summary>
        /// <returns></returns>
        private DateTime GetTodayDateTime()
        {
            DateTime tmpTime = DateTime.Now;
            DateTime todayTime = new DateTime();
            todayTime = todayTime.AddDays(tmpTime.Day - 1);
            todayTime = todayTime.AddMonths(tmpTime.Month - 1);
            todayTime = todayTime.AddYears(tmpTime.Year - 1);
            return todayTime;
        }


        /// <summary>
        /// Изменение текущего фильтра для аудита по дате и событиям в зависимости от настроек фильтра
        /// </summary>
        private void ChangeAuditDateTimeFilterCondition()
        {
            // если начальная дата больше конечной поменяем их местами
            if (pView.udteBeginPeriod.DateTime > pView.udteEndPeriod.DateTime)
            {
                DateTime tmpTime = pView.udteBeginPeriod.DateTime;
                pView.udteBeginPeriod.Value = pView.udteEndPeriod.Value;
                pView.udteEndPeriod.Value = tmpTime;
            }

            DateFilterCondition = string.Empty;
            DateTime EndDate;
            DateTimeFilterParams = null;

            // Если выбран только фильтр по даwте
            if (pView.cbUsePeriod.Checked)
            {
                EndDate = pView.udteEndPeriod.DateTime.AddDays(1);
                DateFilterCondition = "(ChangeTime >= ? and ChangeTime <= ?)";
                DateTimeFilterParams = new IDbDataParameter[2];
                DateTimeFilterParams[0] = new System.Data.OleDb.OleDbParameter("p0", pView.udteBeginPeriod.DateTime);
                    //tmpDB.CreateParameter("p0", pView.udteBeginPeriod.DateTime, DbType.DateTime);
                DateTimeFilterParams[1] = new System.Data.OleDb.OleDbParameter("p1", EndDate);
                        //tmpDB.CreateParameter("p1", EndDate, DbType.DateTime);
            }
        }

        /// <summary>
        /// установка параметров грида
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        Krista.FM.Client.Components.GridColumnsStates ugeAudit_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Время операции";
            state.ColumnName = "CHANGETIME";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            states.Add("CHANGETIME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип операции";
            state.ColumnName = "KINDOFOPERATION";
            states.Add("KINDOFOPERATION", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Объект БД";
            state.ColumnName = "OBJECTNAME";
            states.Add("OBJECTNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип объекта БД";
            state.ColumnName = "OBJECTTYPE";
            states.Add("OBJECTTYPE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Пользователь";
            state.ColumnName = "USERNAME";
            states.Add("USERNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID сессии";
            state.ColumnName = "SESSIONID";
            state.IsHiden = true;
            states.Add("SESSIONID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID записи объекта";
            state.ColumnName = "RECORDID";
            states.Add("RECORDID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID задачи";
            state.ColumnName = "TASKID";
            states.Add("TASKID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID операции закачки";
            state.ColumnName = "PUMPID";
            states.Add("PUMPID", state);

            return states;
        }

        /// <summary>
        /// установка картинок и тултипов для записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAudit_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            UltraGridCell cell = row.Cells["KINDOFOPERATION"];

            int val = Convert.ToInt32(cell.Value);

            switch (val)
            {
                case 0:
                    cell.Appearance.Image = this.pView.ugeAudit.il.Images[2];
                    cell.ToolTipText = "Тип операции: 'Добавление'";
                    break;
                case 1:
                    cell.Appearance.Image = this.pView.ugeAudit.il.Images[0];
                    cell.ToolTipText = "Тип операции: 'Изменение'";
                    break;
                case 2:
                    cell.Appearance.Image = this.pView.ugeAudit.il.Images[1];
                    cell.ToolTipText = "Тип операции: 'Удаление'";
                    break;
            }

            cell = row.Cells["OBJECTTYPE"];
            val = Convert.ToInt32(cell.Value);
            switch (val)
            {
                case 0:
                    cell.Appearance.Image = this.pView.ilObjectType.Images[val];
                    cell.ToolTipText = "Тип объекта: 'Сопоставимый классификатор'";
                    break;
                case 1:
                    cell.Appearance.Image = this.pView.ilObjectType.Images[val];
                    cell.ToolTipText = "Тип объекта: 'Классификатор данных'";
                    break;
                case 2:
                    cell.Appearance.Image = this.pView.ilObjectType.Images[val];
                    cell.ToolTipText = "Тип объекта: 'Фиксированный классификатор'";
                    break;
                case 3:
                    cell.Appearance.Image = this.pView.ilObjectType.Images[val];
                    cell.ToolTipText = "Тип объекта: 'Таблица фактов'";
                    break;
                case 5:
                    cell.Appearance.Image = this.pView.ilObjectType.Images[4];
                    cell.ToolTipText = "Тип объекта: 'Системная таблица'";
                    break;
            }
        }

        void ugeAudit_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["CHANGETIME"];
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            clmn.Width = 125;
            clmn.Format = "dd.MM.yyyy HH:mm:ss"; 
        }

        private IDbDataParameter[] ParametersListToArray(List<Krista.FM.Client.Components.UltraGridEx.FilterParamInfo> parameters)
        {
            if ((parameters == null) || (parameters.Count == 0))
                return null;

            IDbDataParameter[] result = new IDbDataParameter[parameters.Count];
            int ind = 0;
            foreach (Krista.FM.Client.Components.UltraGridEx.FilterParamInfo paramInfo in parameters)
            {
                result[ind] = new System.Data.OleDb.OleDbParameter(paramInfo.ParamName, paramInfo.ParamValue);
                //db.CreateParameter(paramInfo.ParamName, paramInfo.ParamValue);
                ind++;
            }
            return result;
        }

        bool ugeAudit_OnGetHandbookValue(object sender, string columnName, ref object handbookValue)
        {
            int userID = -1;
            string userName = string.Empty;
            if (columnName == "USERNAME")
            {
                if (usersModalForm.ShowModal(NavigationNodeKind.ndAllUsers, ref userID, ref userName))
                {
                    handbookValue = userName;
                    return true;
                }
            }
            return false;
        }

        void ugeAudit_OnGetServerFilterCustomDialogColumnsList(object sender, Infragistics.Win.ValueList valueList, string columnName)
        {
            FillFilterList(valueList, columnName);
        }

        void ugeAudit_OnBeforeRowFilterDropDownPopulateEventHandler(object sender, BeforeRowFilterDropDownPopulateEventArgs e)
        {
            if (e.Column.Key == "USERNAME")
            {
                e.ValueList.ValueListItems.Add(null, CC.UltraGridEx.HandbookSelectCaption);
            }
        }

        void ugData_BeforeRowFilterDropDown(object sender, BeforeRowFilterDropDownEventArgs e)
        {
            pView.ugeAudit.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                ValueList valueList = e.ValueList;
                FillFilterList(valueList, e.Column.Key);
            }
            finally
            {
                pView.ugeAudit.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        private void FillFilterList(ValueList valueList, string columnName)
        {
            if (columnName == "KINDOFOPERATION")
            {
                ValueListItem item = null;

                item = valueList.ValueListItems.Add("item0");
                item.DisplayText = "Добавление";
                item.DataValue = 0;

                item = valueList.ValueListItems.Add("item1");
                item.DisplayText = "Изменение";
                item.DataValue = 1;

                item = valueList.ValueListItems.Add("item2");
                item.DisplayText = "Удаление";
                item.DataValue = 2;
            }
            else if (columnName == "OBJECTTYPE")
            {
                ValueListItem item = valueList.ValueListItems.Add("item0");
                item.DisplayText = "Сопоставимый классификатор";
                item.DataValue = 0;

                item = valueList.ValueListItems.Add("item1");
                item.DisplayText = "Классификатор данных";
                item.DataValue = 1;

                item = valueList.ValueListItems.Add("item2");
                item.DisplayText = "Фиксированный классификатор";
                item.DataValue = 2;

                item = valueList.ValueListItems.Add("item3");
                item.DisplayText = "Таблица фактов";
                item.DataValue = 3;

                item = valueList.ValueListItems.Add("item4");
                item.DisplayText = "Системная таблица";
                item.DataValue = 5;
            }
        }

        private int GetObjectType(ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Table:
                    return 5;
                case ClassTypes.clsFixedClassifier:
                    return 2;
                case ClassTypes.clsDataClassifier:
                    return 1;
                case ClassTypes.clsBridgeClassifier:
                    return 0;
                case ClassTypes.clsFactData:
                    return 3;
            }
            return -1;
        }
    }
}
