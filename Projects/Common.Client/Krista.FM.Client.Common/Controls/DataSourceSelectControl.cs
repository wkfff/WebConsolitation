using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.Common.Controls
{
    public partial class DataSourceSelectControl : UserControl
    {
        private IScheme scheme;

        public DataSourceSelectControl()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(this.uge.ugData);
            this.uge.IsReadOnly = true;

            this.uge.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(uge_OnGridInitializeLayout);
            this.uge.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(uge_OnInitializeRow);
            this.uge.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(uge_OnGetGridColumnsState);
        }

        Krista.FM.Client.Components.GridColumnsStates uge_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID источника";
            state.ColumnType = UltraGridEx.ColumnType.System;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "SupplierCode";
            state.ColumnCaption = "Поставщик информации";
            states.Add("SupplierCode", state);

            state = new GridColumnState();
            state.ColumnName = "DataCode";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("DataCode", state);

            state = new GridColumnState();
            state.ColumnName = "DataName";
            state.ColumnCaption = "Вид и наименование поступившей информации";
            states.Add("DataName", state);

            state = new GridColumnState();
            state.ColumnName = "KindsOfParams";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("KindsOfParams", state);

            state = new GridColumnState();
            state.ColumnName = "KindsOfParamsStr";
            state.ColumnCaption = "Тип параметров";
            state.ColumnPosition = 6;
            states.Add("KindsOfParamsStr", state);

            state = new GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Наименование бюджета";
            states.Add("Name", state);

            state = new GridColumnState();
            state.ColumnName = "Year";
            state.ColumnCaption = "Год";
            states.Add("Year", state);

            state = new GridColumnState();
            state.ColumnName = "MonthStr";
            state.ColumnCaption = "Месяц";
            state.ColumnPosition = 7;
            states.Add("MonthStr", state);

            state = new GridColumnState();
            state.ColumnName = "Month";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("Month", state);

            state = new GridColumnState();
            state.ColumnName = "Variant";
            state.ColumnCaption = "Вариант";
            states.Add("Variant", state);

            state = new GridColumnState();
            state.ColumnName = "Quarter";
            state.ColumnCaption = "Квартал";
            states.Add("Quarter", state);

            state = new GridColumnState();
            state.ColumnName = "Territory";
            state.ColumnCaption = "Территория";
            states.Add("Territory", state);

            return states;
        }

        void uge_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
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

        void uge_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
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
        /// выбор источника
        /// </summary>
        /// <param name="activeScheme"></param>
        /// <param name="dataSourceID"></param>
        /// <returns></returns>
        public bool SelectDataSources(ref int dataSourceID)
        {
            UltraGridRow row = uge.ugData.ActiveRow;
            if (row == null) return false;
            // идем по потомкам до тех пор, пока не получим ячейки
            while (row.Cells == null) row = row.ChildBands[0].Rows[0];

            dataSourceID = System.Convert.ToInt32(row.Cells["ID"].Value);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeScheme"></param>
        private void InitializeDataSourceGrid(IScheme activeScheme, string dataSourceKinds)
        {
            uge.ugData.BeginUpdate();
            try
            {
                udsDataSources.Rows.Clear();
                IDataSourceManager dsManager = activeScheme.DataSourceManager;
                IDataSourceCollection dsc = dsManager.DataSources;
                DataTable dt = dsManager.GetDataSourcesInfo(dataSourceKinds);

                dt.Columns["SupplierCode"].MaxLength = 200;
                dt.Columns.Add("MonthStr", typeof(string));
                dt.Columns.Add("KindsOfParamsStr", typeof(string));
                DataSourcesHelper.SetMonth(dt);
                DataSourcesHelper.SetParamsKind(dt);

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

                uge.DataSource = dt;

            }
            finally
            {
                uge.ugData.DisplayLayout.Bands[0].SortedColumns.Add("SupplierCode", false, true);
                uge.ugData.DisplayLayout.Bands[0].SortedColumns.Add("DataName", false, true);
                uge.ugData.Rows.ExpandAll(true);
                uge.ugData.EndUpdate();
            }
            uge.StateRowEnable = false;
            uge.IsReadOnly = true;
        }

        public void InitializeScheme(IScheme scheme, string dataSourceKinds)
        {
            this.scheme = scheme;
            InitializeDataSourceGrid(scheme, dataSourceKinds);
        }
    }

    public struct DataSourcesHelper
    {
        /// <summary>
        /// получение данных по источникам
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataSourcesInfo()
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
        public static DataTable GetDataSourcesInfo(int ID)
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
        public static int GetDataSourceYear(int ID)
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

        public static IScheme scheme;

        /// <summary>
        /// установка месяца по номеру месяца
        /// </summary>
        /// <param name="table"></param>
        public static void SetMonth(DataTable table)
        {
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                DataRow row = table.Rows[i];
                if (row["Month"] != DBNull.Value)
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
        public static void SetParamsKind(DataTable table)
        {
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                DataRow row = table.Rows[i];
                int intParam = Convert.ToInt32(row["KindsOfParams"]);
                row["KindsOfParamsStr"] = DataSourcesParametersTypesToString((ParamKindTypes)intParam);
            }
        }

        public static string DataSourcesParametersTypesToString(ParamKindTypes dspt)
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
    }
}
