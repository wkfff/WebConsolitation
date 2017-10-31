using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
	internal class frmDataSourceSelect : Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls.frmModalTemplate
    {
        public Infragistics.Win.UltraWinDataSource.UltraDataSource udsDataSources;
	    internal Krista.FM.Client.Components.UltraGridEx uge;
        private ImageList ilImages;

		private System.ComponentModel.IContainer components = null;

		public frmDataSourceSelect()
		{
			InitializeComponent();
			InfragisticComponentsCustomize.CustomizeUltraGridParams(this.uge.ugData);
            this.uge.OnGetGridColumnsState += new CC.GetGridColumnsState(uge_OnGetGridColumnsState);
            this.uge.IsReadOnly = true;//SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.readonlyState); 
            this.uge.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(uge_OnInitializeRow);
            this.uge.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(uge_OnGridInitializeLayout);
        }

        void uge_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridCell delCell = e.Row.Cells["Deleted"];
            // Если источник удален
            if (Convert.ToBoolean(delCell.Value))
            {
                // Скрываем его и больше ничего не делаем
                e.Row.Hidden = true;
                return;
            } 
            // Расставляем иконки и тултипы блокировки источников.
            UltraGridCell lockCell = e.Row.Cells["Locked"];
            lockCell.Column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            
            if (!(Convert.ToBoolean(lockCell.Value)))
            {
                lockCell.Appearance.ImageBackground = ilImages.Images[0];
                lockCell.ToolTipText = "Источник открыт для изменений";
            }
            else
            {
                lockCell.Appearance.ImageBackground = ilImages.Images[1];
                lockCell.ToolTipText = "Источник закрыт от изменений";
            }
        }

        void uge_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["Locked"];
            clmn.Header.VisiblePosition = 0;
            clmn.Header.Caption = string.Empty;
            clmn.Width = 16;
            clmn.AutoSizeMode = ColumnAutoSizeMode.None;

            clmn = band.Columns["Deleted"];
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;
        } 

        /// <summary>
        /// разыменовка полей
        /// </summary>
        /// <returns></returns>
        Krista.FM.Client.Components.GridColumnsStates uge_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID источника";
            state.ColumnType = CC.UltraGridEx.ColumnType.System;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "SupplierCode";
            state.ColumnCaption = "Поставщик информации";
            states.Add("SupplierCode", state);

            state = new CC.GridColumnState();
            state.ColumnName = "DataCode";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("DataCode", state);

            state = new CC.GridColumnState();
            state.ColumnName = "DataName";
            state.ColumnCaption = "Вид и наименование поступившей информации";
            states.Add("DataName", state);

            state = new CC.GridColumnState();
            state.ColumnName = "KindsOfParams";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("KindsOfParams", state);

            state = new CC.GridColumnState();
            state.ColumnName = "KindsOfParamsStr";
            state.ColumnCaption = "Тип параметров";
            state.ColumnPosition = 6;
            states.Add("KindsOfParamsStr", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Наименование бюджета";
            states.Add("Name", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Year";
            state.ColumnCaption = "Год";
            states.Add("Year", state);

            state = new CC.GridColumnState();
            state.ColumnName = "MonthStr";
            state.ColumnCaption = "Месяц";
            state.ColumnPosition = 7;
            states.Add("MonthStr", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Month";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("Month", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Variant";
            state.ColumnCaption = "Вариант";
            states.Add("Variant", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Quarter";
            state.ColumnCaption = "Квартал";
            states.Add("Quarter", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Territory";
            state.ColumnCaption = "Территория";
            states.Add("Territory", state);

            return states;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SupplierCode");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DataCode");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("KindsOfParams");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Year");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Month");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Variant");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Quarter");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataSourceSelect));
            this.udsDataSources = new Infragistics.Win.UltraWinDataSource.UltraDataSource(/*this.components*/);
            this.uge = new Krista.FM.Client.Components.UltraGridEx();
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.spcContainer.Panel1.SuspendLayout();
            this.spcContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udsDataSources)).BeginInit();
            this.SuspendLayout();
            // 
            // spcContainer
            // 
            // 
            // spcContainer.Panel1
            // 
            this.spcContainer.Panel1.Controls.Add(this.uge);
            // 
            // udsDataSources
            // 
            this.udsDataSources.AllowDelete = false;
            ultraDataColumn1.DataType = typeof(long);
            ultraDataColumn6.DataType = typeof(int);
            ultraDataColumn9.DataType = typeof(int);
            this.udsDataSources.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9});
            this.udsDataSources.Band.Key = "Main";
            this.udsDataSources.ReadOnly = true;
            // 
            // uge
            // 
            this.uge.AllowAddNewRecords = true;
            this.uge.AllowClearTable = true;
            this.uge.ColumnsToolbarVisible = false;
            this.uge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uge.ExportImportToolbarVisible = false;
            this.uge.InDebugMode = false;
            this.uge.IsReadOnly = true;
            this.uge.LoadMenuVisible = false;
            this.uge.Location = new System.Drawing.Point(0, 0);
            this.uge.MainToolbarVisible = false;
            this.uge.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.uge.MinCalendarDate = new System.DateTime(((long)(0)));
            this.uge.Name = "uge";
            this.uge.SaveLoadFileName = "";
            this.uge.SaveMenuVisible = false;
            this.uge.ServerFilterEnabled = false;
            this.uge.SingleBandLevelName = "";
            this.uge.Size = new System.Drawing.Size(632, 399);
            this.uge.sortColumnName = "";
            this.uge.StateRowEnable = false;
            this.uge.TabIndex = 0;
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Magenta;
            this.ilImages.Images.SetKeyName(0, "Check.bmp");
            this.ilImages.Images.SetKeyName(1, "ProtectForm.bmp");
            // 
            // frmDataSourceSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Name = "frmDataSourceSelect";
            this.Text = "Источники данных";
            this.spcContainer.Panel1.ResumeLayout(false);
            this.spcContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udsDataSources)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

	}

	public class DataSourcesHelper
	{
        /// <summary>
        /// получение данных по источникам
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataSourcesInfo(IScheme scheme)
        {
            IDatabase db = null;
            DataTable dt = null;
            try
            {
                db = scheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory, Locked from DataSources order by ID",
                    QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Возвращает параметры источника данных
        /// </summary>
        /// <param name="ID">ID источника данных</param>
        /// <returns>Возвращаемые параметры в виде таблицы</returns>
        public static DataTable GetDataSourcesInfo(int ID, IScheme scheme)
        {
            IDatabase db = null;
            DataTable dt = null;
            try
            {
                db = scheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory, Locked " +
                    string.Format("from DataSources where ID = {0}", ID),
                    QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Возвращает год источника данных
        /// </summary>
        /// <param name="ID">ID источника данных</param>
        /// <returns>Возвращаемые параметры в виде таблицы</returns>
        public static int GetDataSourceYear(int ID, IScheme scheme)
        {
            IDatabase db = null;
            DataTable dt = null;
            int year = 0;
            try
            {
                db = scheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory " +
                    string.Format("from DataSources where ID = {0}", ID),
                    QueryResultTypes.DataTable);
                if (dt.Rows.Count > 0)
                    year = Convert.ToInt32(dt.Rows[0]["Year"]);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return year;
        }

        /// <summary>
        /// установка месяца по номеру месяца
        /// </summary>
        /// <param name="table"></param>
        private static void SetMonth(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull("Month"))
                    continue;
                switch (Convert.ToInt32(row["Month"]))
                {
                    case 0:
                        row["MonthStr"] = string.Empty;
                        break;
                    case 1:
                        row["MonthStr"] = "Январь";
                        break;
                    case 2:
                        row["MonthStr"] = "Февраль";
                        break;
                    case 3:
                        row["MonthStr"] = "Март";
                        break;
                    case 4:
                        row["MonthStr"] = "Апрель";
                        break;
                    case 5:
                        row["MonthStr"] = "Май";
                        break;
                    case 6:
                        row["MonthStr"] = "Июнь";
                        break;
                    case 7: 
                        row["MonthStr"] = "Июль";
                        break;
                    case 8:
                        row["MonthStr"] = "Август";
                        break;
                    case 9:
                        row["MonthStr"] = "Сентябрь";
                        break;
                    case 10:
                        row["MonthStr"] = "Октябрь";
                        break;
                    case 11:
                        row["MonthStr"] = "Ноябрь";
                        break;
                    case 12:
                        row["MonthStr"] = "Декабрь";
                        break;
                }
            }
        }

        /// <summary>
        /// установка наименования параметров
        /// </summary>
        /// <param name="table"></param>
        private static void SetParamsKind(DataTable table)
        {
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                DataRow row = table.Rows[i];
                int intParam = Convert.ToInt32(row["KindsOfParams"]);
                row["KindsOfParamsStr"] = DataSourcesParametersTypesToString((ParamKindTypes)intParam);
            }
        }

        private static string DataSourcesParametersTypesToString(ParamKindTypes dspt)
        {
            switch (dspt)
            {
                case ParamKindTypes.Budget:
                    return "Финансовый орган, год";
                case ParamKindTypes.Year:
                    return "Год";
                case ParamKindTypes.YearMonth:
                    return "Год, месяц";
                case ParamKindTypes.YearMonthVariant:
                    return "Год, месяц, вариант";
                case ParamKindTypes.YearVariant:
                    return "Год, вариант";
                case ParamKindTypes.YearQuarter:
                    return "Год, квартал";
                case ParamKindTypes.YearQuarterMonth:
                    return "Год, квартал, месяц";
                case ParamKindTypes.YearTerritory:
                    return "Год, территория";
                case ParamKindTypes.WithoutParams:
                    return "Без параметров";
                case ParamKindTypes.Variant:
                    return "Вариант";
                case ParamKindTypes.YearVariantMonthTerritory:
                    return "Год, вариант, месяц, территория";
                default:
                    return "Неизвестный тип";
            }
        }

        /// <summary>
        /// Форма выбора источника с учетом настоенных в дизайнере источников
        /// </summary>
        /// <param name="dataSourceKinds"></param>
        /// <param name="dataSourceID"></param>
        /// <returns></returns>
        public static bool SelectDataSources(IScheme scheme, string dataSourceKinds, ref int dataSourceID)
        {
            return ViewDataSources(scheme, dataSourceKinds, ref dataSourceID);
        }

        /// <summary>
        /// выбор источника
        /// </summary>
        /// <param name="activeScheme"></param>
        /// <param name="dataSourceID"></param>
        /// <returns></returns>
        public static bool SelectDataSources(IScheme scheme, ref int dataSourceID)
        {
            return ViewDataSources(scheme, null, ref dataSourceID);
        }

        /// <summary>
        /// Форма выбора источника
        /// </summary>
        /// <param name="dataSourceKinds"></param>
        /// <param name="dataSourceID"></param>
        /// <returns></returns>
        private static bool ViewDataSources(IScheme scheme, string dataSourceKinds, ref int dataSourceID)
	    {
	        frmDataSourceSelect tmpFrm = new frmDataSourceSelect();
            if (dataSourceKinds != null)
                tmpFrm.Text = "Список доступных источников данных";
	        tmpFrm.uge.ugData.BeginUpdate();
	        try
	        {
	            tmpFrm.udsDataSources.Rows.Clear();
	            IDataSourceManager dsManager = scheme.DataSourceManager;
	            IDataSourceCollection dsc = dsManager.DataSources;
	            DataTable dt = (dataSourceKinds != null)
	                               ? dsManager.GetDataSourcesInfo(dataSourceKinds)
	                               : dsManager.GetDataSourcesInfo();

	            dt.Columns["SupplierCode"].MaxLength = 200;
	            dt.Columns.Add("MonthStr", typeof(string));
	            dt.Columns.Add("KindsOfParamsStr", typeof(string));
	            SetMonth(dt);
	            SetParamsKind(dt);

	            foreach (DataRow row in dt.Rows)
	            {
	                //DataRow row = dt.Rows[i];
	                row["DataName"] = row["DataCode"].ToString().PadLeft(4, '0') + "-" + row["DataName"];
	                IDataSupplier dsp = null;
	                // пытаемся получить поставщика по его имени
	                try
	                {
	                    dsp = dsManager.DataSuppliers[row["SupplierCode"].ToString()];

	                    row["SupplierCode"] = row["SupplierCode"].ToString() + '-' + dsp.Description;
	                }
	                catch
	                {
	                    // если по какой то причине не получаем поставщика, то ничего не делаем
	                }
	                if (row["Quarter"] != DBNull.Value)
	                    if (Convert.ToInt32(row["Quarter"]) == 0)
	                        row["Quarter"] = DBNull.Value;

	                if (Convert.ToInt32(row["Year"]) == 0)
	                    row["Year"] = DBNull.Value;
	            }

	            tmpFrm.uge.DataSource = dt;

	        }
	        finally
	        {
	            tmpFrm.uge.ugData.DisplayLayout.Bands[0].SortedColumns.Add("SupplierCode", false, true);
	            tmpFrm.uge.ugData.DisplayLayout.Bands[0].SortedColumns.Add("DataName", false, true);
	            //tmpFrm.uge.ugData.Rows.ExpandAll(true);
	            tmpFrm.uge.ugData.EndUpdate();
	        }
	        tmpFrm.uge.StateRowEnable = false;
	        tmpFrm.uge.IsReadOnly = true;

	        tmpFrm.ShowDialog();
	        if (tmpFrm.DialogResult == DialogResult.OK)
	        {
	            UltraGridRow row = tmpFrm.uge.ugData.ActiveRow;
	            if (row == null) return false;
	            // идем по потомкам до тех пор, пока не получим ячейки
	            while (row.Cells == null) row = row.ChildBands[0].Rows[0];

	            dataSourceID = System.Convert.ToInt32(row.Cells["ID"].Value);
	            return true;
	        }
	        else return false;
	    }
	}
}

