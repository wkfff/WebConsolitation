using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables
{
	public class FactTablesUI : BaseClsUI
	{
        public FactTablesUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public FactTablesUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Caption = "“аблицы фактов";
            clsClassType = ClassTypes.clsFactData;
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_FactTable_16.GetHicon()); }
        }

        public override Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_FactTable_16; }
		}

		public override Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_FactTable_24; }
		}

        public override void InitializeUI()
        {
            base.InitializeUI();
            vo.utbToolbarManager.Tools["disin"].SharedProps.Visible = false;
            vo.utcDataCls.Tabs[1].Visible = false;
            vo.utcLogSwitcher.Tabs[1].Visible = false;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            int? maxRecordInSelect = null;
            object recCount;
            string dbName = ActiveDataObj.FullDBName;
            // получаем пользовательский фильтр
            string userFilter;
            FilterParameters = null;
            List<UltraGridEx.FilterParamInfo> serverFilterParameters;

            vo.ugeCls.BuildServerFilter(out userFilter, out serverFilterParameters);
            FilterParameters = serverFilterParameters;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string checkRecCountSQL = String.Format("select count(ID) from {0}", dbName);
                if (GetDataSourcesFilter() != String.Empty)
                    checkRecCountSQL = String.Concat(checkRecCountSQL, " WHERE ", GetDataSourcesFilter());
                if (userFilter != String.Empty)
                    checkRecCountSQL = String.Concat(checkRecCountSQL, " AND ", userFilter);

                recCount = db.ExecQuery(checkRecCountSQL, QueryResultTypes.Scalar,
                    ParametersListToArray(FilterParameters));
                if (Convert.ToInt32(recCount) > MaxFactTableRecordCount)
                {
                    maxRecordInSelect = MaxFactTableRecordCount;
                }
            }
             
            dataQuery = String.Concat(GetDataSourcesFilter(), string.Empty);
            if (userFilter != String.Empty)
                dataQuery = String.Concat(dataQuery, " AND ", userFilter);

            if (maxRecordInSelect != null)
            {
                //  если записей слищком много - спрашиваем пользовател€ хочет ли он видеть первые 5000
                string messageStr = String.Concat("—лишком большой объект выборки ({0}).", Environment.NewLine,
                    "ћогут быть показаны только {1} записей.", Environment.NewLine, "ѕоказать?");
                messageStr = String.Format(messageStr, recCount, MaxFactTableRecordCount);
                // если нет - формируем фильтр таким образом чтобы
                if (MessageBox.Show(messageStr, "»нформационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    dataQuery = "(ID is NULL)";
                    FilterParameters = null;
                }
            }
            importDataQuery = dataQuery;

            filterStr = dataQuery;

            IDbDataParameter[] prms = ParametersListToArray(FilterParameters);

            return ActiveDataObj.GetDataUpdater(dataQuery, maxRecordInSelect, prms);
        }

	    public override bool HasDataSources()
        {
            return ((IFactTable)ActiveDataObj).IsDivided;
        }

        protected override void SetViewObjectCaption()
        {
            if (ActiveDataObj == null)
                return;
            Workplace.ViewObjectCaption = string.Format("{0}: {1}", "“аблица фактов", GetClsRusName());
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllFactTablesOperations.EditRecord, false);

            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)FactTableOperations.EditRecord, false);
        }

        public override void SetPermissionsToClassifier(Components.UltraGridEx gridEx)
        {
            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;

            if (viewOnly)
            {
                gridEx.IsReadOnly = true;
            }
            else
            {
                gridEx.IsReadOnly = false;

                // странно, дл€ таблиц фактов нихера не пон€тно..... 
                // ну да ладно, пусть добавл€ть записи можно если можно редактировать записи
                gridEx.AllowAddNewRecords = true;
                gridEx.AllowImportFromXML = true;
            }

            // ну а удал€ть и очищать можно ваще всегда

            gridEx.AllowClearTable = true;
            gridEx.AllowDeleteRows = true;

            // настройка прав на редактирование классификатора
            if (allowEditRecords)
            {
                gridEx.AllowEditRows = true;
            }
            else
            {
                gridEx.AllowEditRows = false;
            }

            gridEx.SetComponentSettings();
        }

        public override bool EnableServerFilter()
        {
            return true;
        }

        /// <summary>
        /// провер€ем, можно ли добавл€ть новые записи в таблицы фактов, где нужен ID задачи
        /// </summary>
        /// <returns></returns>
        internal override bool  AllowAddNewToFacts()
        {
            if (clsClassType == ClassTypes.clsFactData)
            {
                if (ActiveDataObj.Attributes.ContainsKey("TaskID"))
                {
                    if (CurrentTaskID > 0)
                        // задача выбрана
                        return true;
                    else
                        // задача не выбрана
                        return false;
                }
            }
            return true;
        }

        public override ObjectType GetClsObjectType()
        {
            return ObjectType.FactTable;
        }

        public override void RefreshVisibleTask()
        {
            this.RefreshTaskIDVisible();
        }

        public override object GetNewId()
        {
            try
            {
                return ((IFactTable)ActiveDataObj).GetGeneratorNextValue;
            }
            catch
            {
                return DBNull.Value;
            }
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            if (CurrentTaskID != -1)
                row.Cells["TASKID"].Value = CurrentTaskID;
        }

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();

            vo.utbToolbarManager.Toolbars["SelectTask"].Visible =
                (CheckAllowEdit() == DefaultableBoolean.True)
                && (dsObjData.Tables[0].Columns.Contains("TASKID"));
        }

        protected override IExportImporter GetExportImporter()
        {
            return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.FactTable);
        }
	}
}