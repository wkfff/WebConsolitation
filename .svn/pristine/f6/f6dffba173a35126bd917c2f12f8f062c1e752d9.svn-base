using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.Generic;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using System.Web.SessionState;

using Krista.FM.ServerLibrary;

using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using Krista.FM.Common;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;

namespace Krista.FM.Server.Dashboards.OMCY_0002.Default.reports.OMCY_002_YANAO
{
    public partial class _default : CustomReportPage
    {

        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }

        SetFromMultiCombo SetChildrenRegion;

        SetFromMultiCombo SetStatusData;

        CustomParam SelectYear;

        CustomParam SelectChildrenRegion;

        CustomParam CurentBaseRegion;

        CustomParam IdRegion;

        CustomParam CheckDoc;

        CustomParam MarkSbor;

        CustomParam MarkSborRevert;

        CustomParam StatusData_;

        CustomParam SourceID;

        CustomParam RegionName;

        #region Loaders
        private static IDatabase GetDataBase()
        {
            HttpSessionState sessionState = HttpContext.Current.Session;
            LogicalCallContextData cnt =
                sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
            if (cnt != null)
                LogicalCallContextData.SetContext(cnt);
            IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];

            return scheme.SchemeDWH.DB;

        }

        private DataTable GetDBWar(string Query)
        {
            IDatabase db = GetDataBase();
            DataTable dt = null;

            try
            {
                dt = (DataTable)db.ExecQuery(Query, QueryResultTypes.DataTable);
                CRHelper.SaveToQueryLog(Query);
            }
            //catch (Exception e)
            //{
            //    String.Format(CRHelper.GetExceptionInfo(e));
            //}
            finally
            {
                db.Dispose();
            }
            return dt;

        }


        DataTable GetDataTableFromChart(string QueryId)
        {
            string Query = DataProvider.GetQueryText(QueryId);

            DataTable NewTable = new DataTable(QueryId);

            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(Query, QueryId, NewTable);

            return NewTable;
        }
        #endregion

        #region MultiCombo
        #region Class SetFromMultiCombo
        private class SetFromMultiCombo
        {
            Dictionary<string, int> SetOfDisplaName;

            Dictionary<int, string> ArrayOfDisplayName;

            Dictionary<string, int> DisplayNameOfId;


            public string First
            {
                get
                {
                    return this.ArrayOfDisplayName[0];
                }
            }

            public string Last
            {
                get
                {
                    return this.ArrayOfDisplayName[this.count - 1];
                }
            }            

            public string this[int index]
            {
                get
                {
                    return this.ArrayOfDisplayName[index];
                }
            }

            public int count
            {
                get
                {
                    return ArrayOfDisplayName.Count;
                }
            }

            public Dictionary<string, int> SetForMultiCombo
            {
                get
                {
                    return SetOfDisplaName;
                }
            }

            public int GetIDByName(string Name)
            {
                return DisplayNameOfId[Name];
            }

            private void GenerateSetFromDataTable(DataTable Table, int ColDisplayName)
            {
                this.SetOfDisplaName = new Dictionary<string, int>();

                this.ArrayOfDisplayName = new Dictionary<int, string>();

                this.DisplayNameOfId = new Dictionary<string, int>();

                int Count = 0;

                foreach (DataRow row in Table.Rows)
                {
                    try
                    {
                        string DisplayName = row[ColDisplayName].ToString();

                        this.DisplayNameOfId.Add(DisplayName, (int)row[1]);

                        this.SetOfDisplaName.Add(DisplayName, 0);

                        this.ArrayOfDisplayName.Add(Count++, DisplayName);
                    }
                    catch { }

                    
                }
            }

            public SetFromMultiCombo(DataTable Table)
            {
                this.GenerateSetFromDataTable(Table, 0);
            }

            public SetFromMultiCombo(DataTable Table, int ColDisplayName)
            {
                this.GenerateSetFromDataTable(Table, ColDisplayName);
            }

        }
        #endregion

        SetFromMultiCombo FillSetMultiCombo(string QueryId)
        {             
            DataTable Table = GetDataTableFromChart(QueryId);

            return new SetFromMultiCombo(Table);
        }

        void ConfigurationMultiCombo()
        {
            //ComboYear.SetСheckedState(SetYear.First, true);
            //ComboYear.ParentSelect = false;
            
            ComboRegion.SetСheckedState(SetChildrenRegion.First, true);
            ComboRegion.ParentSelect = false;
            ComboRegion.Width = 400;
            
            ComboRegion.Title = "Муниципальное образование";


            DataSources.Width = 200; 
            DataSources.Title = "Год";

            StatusData.Width = 300;
        }

        protected DataTable GetChildrenRegion(string Parent)
        {
            string Query = DataProvider.GetQueryText("ChildrenRegion");
            Query = string.Format(Query, Parent);
            return GetDBWar(Query);
        }

        protected DataTable GetStatusData(string StatusData)
        {
            string Query = DataProvider.GetQueryText("StatusData");
            Query = string.Format(Query, StatusData);
            return GetDBWar(Query);
        }

        private Dictionary<string, int> LoadDataSourceDict(Dictionary<string, string> DataSourceId)
        {
            DataTable Table = GetDBWar(DataProvider.GetQueryText("DataSource"));

            Dictionary<string, int> MultiComboDict = new Dictionary<string, int>();
            foreach (DataRow row in Table.Rows)
            {
                string Year = string.Format("{0} год", row["DataSourceName"].ToString().Split('-')[1]);
                string ID = row["SourceID"].ToString();
                MultiComboDict.Add(Year, 0);
                DataSourceId.Add(Year, ID);
            }
            return MultiComboDict;

        }

        Dictionary<string, string> DataSourceId = new Dictionary<string, string>();
        void FillMultiCombo()
        {
            //SetYear = FillSetMultiCombo("AllYear");

            //ComboYear.FillDictionaryValues(SetYear.SetForMultiCombo);

            DataTable regions = GetChildrenRegion("Ханты-Мансийский автономный округ Югра");

            SetChildrenRegion = new SetFromMultiCombo(regions);

            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(SetChildrenRegion.SetForMultiCombo);
                //все состояния
            }

            regions = GetStatusData("Ханты-Мансийский автономный округ Югра");

            SetStatusData = new SetFromMultiCombo(regions);

            Dictionary<string, int> DatasourceComboDict = LoadDataSourceDict(DataSourceId);

            if (!Page.IsPostBack)
            { 
                ComboRegion.FillDictionaryValues(SetChildrenRegion.SetForMultiCombo);
                SetStatusData.SetForMultiCombo.Add("Все состояния", 0);
                StatusData.FillDictionaryValues(SetStatusData.SetForMultiCombo);

                StatusData.SetСheckedState("Все состояния", true);

                DataSources.FillDictionaryValues(DatasourceComboDict);
                //DataSources.SelectLastNode(); 
            }

        }
        #endregion

        #region CustomParam

        private void InicializeCustomParams()
        {

            SourceID = UserParams.CustomParam("SourceID");

            RegionName = UserParams.CustomParam("RegionName");

            CurentBaseRegion = UserParams.CustomParam("CurentBaseRegion");

            SelectChildrenRegion = UserParams.CustomParam("SelectChildrenRegion");

            SelectYear = UserParams.CustomParam("SelectYear");

            IdRegion = UserParams.CustomParam("RegionId");

            CheckDoc = UserParams.CustomParam("CheckDoc");

            MarkSbor = UserParams.CustomParam("MarkSbor");

            MarkSborRevert = UserParams.CustomParam("MarkSborRevert");

            StatusData_ = UserParams.CustomParam("StatusData_");          


        }

        private void LoadRegionSettingParam()
        {
            CurentBaseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
        }

        private void LoadUserParam()
        {
            //SelectYear.Value = ComboYear.SelectedValue;

            //SelectChildrenRegion.Value = ComboRegion.SelectedValue;

            //Сказали год зашить в отчёте) 
            SelectYear.Value = DataSources.SelectedValue;
                //"2010";

            SourceID.Value = DataSourceId[DataSources.SelectedValue];

            RegionName.Value = ComboRegion.SelectedValue;

            IdRegion.Value = SetChildrenRegion.GetIDByName(ComboRegion.SelectedValue).ToString();

            //CheckDoc.Value = CheckBox2.Checked ? "" : "--";

            MarkSbor.Value = CheckBox3.Checked ? "" : "--";

            MarkSborRevert.Value = CheckBox3.Checked ? "--" : "";

            if (StatusData.SelectedValue != "Все состояния")
            {
                StatusData_.Value = SetStatusData.GetIDByName(StatusData.SelectedValue).ToString();
                CheckDoc.Value = "";
            }
            else
            {
                CheckDoc.Value = "--";
            }
            //Data
            
        }

        #endregion

        #region DataBindGrid

        private void CustomizeReportTableColumn(DataTable Table)
        {
            Table.Columns.Add("Capacity", typeof(string));

            Table.Columns.Add("grouping", typeof(string));
            //
            Table.Columns.Add("MO", typeof(string));

            Table.Columns.Add("formula", typeof(string));

            Table.Columns.Add("FieldShort", typeof(string));

            Table.Columns.Add("IsInverse", typeof(string));

            Table.Columns.Add("Code", typeof(string));

            Table.Columns.Add("number", typeof(string));

            Table.Columns.Add("Field", typeof(string));

            Table.Columns.Add("Unit", typeof(string));

            //Table.Columns.Add("Code", typeof(string));            

            Table.Columns.Add("Prev", typeof(decimal));

            Table.Columns.Add("Current", typeof(decimal));

            Table.Columns.Add("Shift", typeof(object));

            Table.Columns.Add("Current+1", typeof(decimal));

            Table.Columns.Add("Current+2", typeof(decimal));

            Table.Columns.Add("Current+3", typeof(decimal));

            Table.Columns.Add("Note", typeof(string));           

        }

        private object GetRateOfIncrease(decimal prev, decimal curr)
        {
            decimal result;
            try
            {

                int znak = 1;
                if (curr < 0)
                {
                    znak = -1;
                }

                result = (curr / prev - 1) * 100*znak;

                return result;
            }
            catch {
                return null;
            }
        }

        bool isEmpty(object o)
        {

            if (o != DBNull.Value)
            {
                String.Format(o.ToString());
                if (!string.IsNullOrEmpty(o.ToString()))
                {
                    return false;
                }
            }

            return true;
        }


        private void CreateDataRow(DataRow BaseRow, DataRow NewRow)
        {
            NewRow["MO"] = BaseRow["MO"];
            
            NewRow["grouping"] = BaseRow["grouping"];

            NewRow["formula"] = BaseRow["formula"];

            NewRow["Capacity"] = BaseRow["Capacity"];

            NewRow["FieldShort"] = BaseRow["FieldShort"];

            NewRow["number"] = BaseRow["number"];                

            NewRow["Code"] = BaseRow["Code"];            
            
            NewRow["Field"] =
                isEmpty(NewRow["FieldShort"]) ?
                BaseRow["Field"] :
                NewRow["FieldShort"];

            NewRow["Unit"] = BaseRow["Unit"];

            NewRow["IsInverse"] = BaseRow["IsInverse"];

            //NewRow["Code"] = BaseRow["Code"];

            NewRow["Prev"] = BaseRow["Prev"];

            NewRow["Current"] = BaseRow["Current"];
            try
            {
                NewRow["Shift"] = GetRateOfIncrease((System.Decimal)NewRow["Prev"], (System.Decimal)NewRow["Current"]);
            }
            catch { }
           
            NewRow["Current+1"] = BaseRow["Current+1"];

            NewRow["Current+2"] = BaseRow["Current+2"];

            NewRow["Current+3"] = BaseRow["Current+3"];

            NewRow["Note"] = BaseRow["Note"];

            NewRow.Table.Rows.Add(NewRow);

        }

        private void CreateSubTitleRow(DataRow BaseRow, DataRow NewRow)
        {
            NewRow["Field"] = RulingScope.GetSubTitleScope(
                 RulingScope.GetOneStringScope(BaseRow["upLevelScope"].ToString(), BaseRow["Scope"].ToString())
                );
            NewRow["Unit"] = "SubTitle";

            //TODO
            if (!string.IsNullOrEmpty(NewRow["Field"].ToString()))
            {
                NewRow.Table.Rows.Add(NewRow);
            }

            CreateDataRow(BaseRow, NewRow.Table.NewRow());
        }

        private void CreateTitleRow(DataRow BaseRow, DataRow NewRow)
        {
            NewRow["Field"] = RulingScope.GetTitleScope(
                 RulingScope.GetOneStringScope(BaseRow["upLevelScope"].ToString(), BaseRow["Scope"].ToString())
                );
            NewRow["Unit"] = "Title";

            NewRow.Table.Rows.Add(NewRow);

            CreateSubTitleRow(BaseRow, NewRow.Table.NewRow());
        }

        private bool Filtred(DataRow BaseRow)
        {
            string Scope = BaseRow["Scope"].ToString();
            if (string.IsNullOrEmpty(Scope))
            {
                return false;
            }
            return true;
        }
        
        class RulingScope
        {
            public enum Operation
            {
                Nothing,
                newTitleScope,
                newSubTitleScope
            }

            string OldTitleScope = "";
            string OldSubTitleScope = "";

            public static string GetScopeByID(int id)
            {
                if (id == 1)
                {
                    return "Экономическое развитие";
                }
                return "";
            }


            public static string GetOneStringScope(string scopeId,string scope)
            {
                string upLevelScope = scopeId;

                string Scope = scope;

                if (!string.IsNullOrEmpty(upLevelScope))
                {
                    int IDScope;
                    if (int.TryParse(scopeId, out IDScope))
                    {
                        upLevelScope = RulingScope.GetScopeByID(IDScope);
                    }
                    else
                    {
 
                    }
                    return upLevelScope + "." + scope;
                }
                return scope;
            }

            public static string GetSubTitleScope(string FullScope)
            {
                try
                {
                    return FullScope.Split('.')[1];
                }
                catch
                {
                    return "";
                }
            }

            public static string GetTitleScope(string FullScope)
            {
                return FullScope.Split('.')[0];
            }

            public Operation CheckScope(string Scope)
            {
                string NewTitleScope = RulingScope.GetTitleScope(Scope);
                string NewSubTitleScope = RulingScope.GetSubTitleScope(Scope);

                if (NewTitleScope != OldTitleScope)
                {
                    OldTitleScope = NewTitleScope;
                    OldSubTitleScope = NewSubTitleScope;
                    return Operation.newTitleScope;
                }
                else
                    if (NewSubTitleScope != this.OldSubTitleScope)
                    {
                        OldSubTitleScope = NewSubTitleScope;
                        return Operation.newSubTitleScope;
                    }

                return Operation.Nothing;
            }
        }

        private void ImportRow(DataTable BaseTable, DataTable ReportTable)
        {
            RulingScope RulScope = new RulingScope();

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                if (Filtred(BaseRow))
                {
                    DataRow ReportRow = ReportTable.NewRow();

                    string Scope = RulingScope.GetOneStringScope( BaseRow["upLevelScope"].ToString(),BaseRow["Scope"].ToString());

                    RulingScope.Operation Operation = RulScope.CheckScope(Scope);

                    if (Operation == RulingScope.Operation.newTitleScope)
                    {
                        CreateTitleRow(BaseRow, ReportRow);
                    }
                    else
                        if (Operation == RulingScope.Operation.newSubTitleScope)
                        {
                            CreateSubTitleRow(BaseRow, ReportRow);
                        }
                        else
                        {
                            CreateDataRow(BaseRow, ReportRow);
                        }
                }

            }


        }




        void Grid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("GridSqlQ");

            DataTable BaseTable = GetDBWar(query);

            DataTable ReportTable = new DataTable("reportGrid");

            CustomizeReportTableColumn(ReportTable);

            ImportRow(BaseTable, ReportTable);

            Grid.DataSource = ReportTable;

        }



        private void GenerationGrid()
        {
            Grid.DataBinding += new EventHandler(Grid_DataBinding);
            Grid.DataBind();
        }

        #endregion

        #region CustomizeGrid
        protected void SettingColumnName()
        {
            int SelectYearNum = int.Parse(SelectYear.Value.Replace(" год",""));

            Infragistics.WebUI.UltraWebGrid.ColumnsCollection Columns = Grid.Columns;

            Columns.FromKey("IsInverse").Header.Caption = "Hidden";
            Columns.FromKey("number").Header.Caption = "№. п.п.";
            Columns.FromKey("Field").Header.Caption = "Показатель";
            Columns.FromKey("Unit").Header.Caption = "Единица измерения";            

            Columns.FromKey("Prev").Header.Caption = (SelectYearNum - 1).ToString();
            Columns.FromKey("Current").Header.Caption = (SelectYearNum).ToString();
            Columns.FromKey("Shift").Header.Caption = "Изменение";

            Columns.FromKey("Current+1").Header.Caption = (SelectYearNum + 1).ToString();
            Columns.FromKey("Current+2").Header.Caption = (SelectYearNum + 2).ToString();
            Columns.FromKey("Current+3").Header.Caption = (SelectYearNum + 3).ToString();

            Columns.FromKey("Note").Header.Caption = "Примечание";
        }

        protected void SetHeaderPos(ColumnHeader HeaderCol, int OriginX, int OriginY, int SpanX, int SpanY)
        {
            HeaderCol.RowLayoutColumnInfo.OriginX = OriginX;

            HeaderCol.RowLayoutColumnInfo.OriginY = OriginY;

            HeaderCol.RowLayoutColumnInfo.SpanX = SpanX;

            HeaderCol.RowLayoutColumnInfo.SpanY = SpanY;
        }

        protected void GenerationHeader(UltraWebGrid Grid, string Caption, int OriginX, int OriginY, int SpanX, int SpanY)
        {
            ColumnHeader HeaderCol = new ColumnHeader();

            SetHeaderPos(HeaderCol, OriginX, OriginY, SpanX, SpanY);

            HeaderCol.Caption = Caption;

            Grid.Bands[0].HeaderLayout.Add(HeaderCol);
        }

        protected void SettingButifulHeader()
        {
            Infragistics.WebUI.UltraWebGrid.ColumnsCollection Columns = Grid.Columns;

            SetHeaderPos(Columns.FromKey("number").Header, 0, 0, 1, 2);

            SetHeaderPos(Columns.FromKey("Field").Header, 1, 0, 1, 2);

            SetHeaderPos(Columns.FromKey("Unit").Header, 2, 0, 1, 2);

            GenerationHeader(Grid,"Завершённый период",3,0,1,1);
            SetHeaderPos(Columns.FromKey("Prev").Header, 3, 1, 1, 1);

            GenerationHeader(Grid, "Отчётный период", 4, 0, 1, 1);
            SetHeaderPos(Columns.FromKey("Current").Header, 4, 1, 1, 1);

            GenerationHeader(Grid, "Плановый период", 5, 0, 3, 1);
            SetHeaderPos(Columns.FromKey("Current+1").Header, 5, 1, 1, 1);
            SetHeaderPos(Columns.FromKey("Current+2").Header, 6, 1, 1, 1);
            SetHeaderPos(Columns.FromKey("Current+3").Header, 7, 1, 1, 1);

            SetHeaderPos(Columns.FromKey("Note").Header, 8, 0, 1, 2);
        }

        protected void SettingHeaderStyle()
        {
            foreach (ColumnHeader head in Grid.Bands[0].HeaderLayout)
            {
                head.Style.Wrap = true;
                head.Style.VerticalAlign = VerticalAlign.Middle;
                head.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void SettingHeaderGrid()
        {
            SettingColumnName();
            SettingButifulHeader();
            SettingHeaderStyle();

        }

        protected void SettingColWidth()
        {
            int GridWidth = (int)(Grid.Width.Value);


            int OnePercentWidth = (GridWidth) / 100;

            Infragistics.WebUI.UltraWebGrid.ColumnsCollection Columns = Grid.Columns;

            Columns.FromKey("number").Width = OnePercentWidth * 3;

            Columns.FromKey("Field").Width = OnePercentWidth * 30   ;

            Columns.FromKey("Unit").Width = OnePercentWidth * 7;            
            
            Columns.FromKey("Prev").Width = OnePercentWidth * 8;
            Columns.FromKey("Current").Width = OnePercentWidth * 8;

            Columns.FromKey("Current+1").Width = OnePercentWidth * 8;
            Columns.FromKey("Current+2").Width = OnePercentWidth * 8;
            Columns.FromKey("Current+3").Width = OnePercentWidth * 8;

            Columns.FromKey("Note").Width = OnePercentWidth * 19;
             
        }

        protected void SettingCellStyle()
        {
            Infragistics.WebUI.UltraWebGrid.ColumnsCollection Columns = Grid.Columns;

            Columns.FromKey("number").CellStyle.HorizontalAlign = HorizontalAlign.Center;

            Columns.FromKey("Unit").CellStyle.HorizontalAlign = HorizontalAlign.Center;
            
            Columns.FromKey("Field").CellStyle.Wrap = true;

            Columns.FromKey("Unit").CellStyle.Wrap = true;

            Columns.FromKey("IsInverse").Hidden = true;

            Columns.FromKey("grouping").Hidden = true;
            
            Columns.FromKey("MO").Hidden = true;

            Columns.FromKey("formula").Hidden = true;

            Columns.FromKey("Code").Hidden = true;

            Columns.FromKey("FieldShort").Hidden = true;

            Columns.FromKey("Capacity").Hidden = true;

            string formatNumVal = "N2";

            Columns.FromKey("Prev").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Prev").CellStyle.Padding.Right = 10;

            Columns.FromKey("Current").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Current").CellStyle.Padding.Right = 10;

            Columns.FromKey("Current").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Current").CellStyle.Padding.Right = 10;

            Columns.FromKey("Current+1").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Current+1").CellStyle.Padding.Right = 10;

            Columns.FromKey("Current+2").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Current+2").CellStyle.Padding.Right = 10;

            Columns.FromKey("Current+3").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Columns.FromKey("Current+3").CellStyle.Padding.Right = 10;

            Columns.FromKey("Shift").Hidden = true;
        }

        protected void ClearRow(UltraGridRow row, int startIndex)
        {
            for (int i = startIndex; i < row.Cells.Count; i++)
            {
                row.Cells[i].Value = null;
            }
        }

        protected void SettingTitleRow(UltraGridRow row,int NumberTitle)
        {
            ClearRow(row, row.Cells.FromKey("Field").Column.Index+1);
            row.Cells.FromKey("Field").ColSpan = row.Cells.Count-2;

            row.Cells.FromKey("Field").Style.Font.Bold = true;

            row.Cells.FromKey("Field").Style.HorizontalAlign = HorizontalAlign.Left;

            row.Cells.FromKey("Field").Text = string.Format("Раздел {0}. {1}", NumberTitle, row.Cells.FromKey("Field").Text);

            row.Style.BackColor = Color.FromArgb(241, 241, 242);
        }

        private void SettingSubTitleRow(UltraGridRow row)
        {            
            ClearRow(row, row.Cells.FromKey("Field").Column.Index + 1);

            row.Cells.FromKey("Field").ColSpan = row.Cells.Count - 2;
            row.Cells.FromKey("Field").Style.HorizontalAlign = HorizontalAlign.Left;
            row.Cells.FromKey("Field").Style.Font.Bold = true;

            row.Style.BackColor = Color.FromArgb(241, 241, 242);
        }

        private string FormatShiftValue(object old)
        {
            
            string Formated = string.Format("{0:N2}",old);
            if (!old.ToString().Contains("-"))
            {
                Formated = "+" + Formated;                
            }
            return Formated;
        }


        private void SetImageFromRow(UltraGridRow row)
        {
            object valFlag = row.Cells.FromKey("IsInverse").Value;

            bool IsReverce = bool.Parse(valFlag.ToString());

            decimal valShift = (decimal)row.Cells.FromKey("Shift").Value;

            string UpOrDouwn = valShift > 0 ? "Up" : "Down";

            string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) || ((UpOrDouwn == "Down") && (IsReverce))) ? "Green" : "Red";

            string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";

            string ImagePath = "~/images/" + ImageName;

            row.Cells.FromKey("Shift").Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            row.Cells.FromKey("Shift").Style.BackgroundImage = ImagePath;

            row.Cells.FromKey("Shift").Text = FormatShiftValue(row.Cells.FromKey("Shift").Value);

        }

        void SetYesOrNo(UltraGridCell Cell)
        {
            if (Cell.Value == null) return;            

            decimal Val = decimal.Parse(Cell.Value.ToString());

            if (Val > 0)//(Cell.Text == "1")
            {
                Cell.Text = "Да";
            }
            else            
            {
                Cell.Text = "Нет";
            };
        }

        protected void HandlingTheSpecialCase(UltraGridRow row)        
        {
            string Unit = row.Cells.FromKey("Unit").Text;

            if (Unit == "Год")
            {
                row.Cells.FromKey("Shift").Value = null;
                row.Cells.FromKey("Shift").Text = "";
            }

            if ((Unit == "Группировка") || (Unit == "Неизвестные данные"))
            {
                row.Cells.FromKey("Unit").Value = null;
                row.Cells.FromKey("Unit").Text = "";
            }

            if (Unit == "да/нет")
            {
                SetYesOrNo(row.Cells.FromKey("Prev"));
                SetYesOrNo(row.Cells.FromKey("Current"));
                SetYesOrNo(row.Cells.FromKey("Current+1"));
                SetYesOrNo(row.Cells.FromKey("Current+2"));
                SetYesOrNo(row.Cells.FromKey("Current+3"));

                row.Cells.FromKey("Shift").Value = null;
                row.Cells.FromKey("Shift").Text = "";                
            }
            
 
        }

        private void SetFileldName(UltraGridRow row)
        {
            string ShortFieldName = row.Cells.FromKey("FieldShort").Text;

            if (!string.IsNullOrEmpty(ShortFieldName))
            {
                row.Cells.FromKey("FieldShort").Text = ShortFieldName;
            }
        }

        void FormatCell(UltraGridCell cell, int Capacity)
        {
            if (cell.Value != null)
            {
                decimal out_;
                if (decimal.TryParse(cell.Value.ToString(), out out_))  
                {
                    string ForrmatS = "{"+string.Format("0:N{0}", Capacity)+"}";
                    cell.Value = string.Format(ForrmatS, out_);
                }
            }
        }

        private void FormatRowValue(UltraGridRow row)
        {           

            int Capacity = 2;
            if (row.Cells.FromKey("Capacity").Value != null)
            {
                 Capacity = int.Parse(row.Cells.FromKey("Capacity").Value.ToString());                 
            }

            FormatCell(row.Cells.FromKey("Prev"), Capacity);

            FormatCell(row.Cells.FromKey("Current"),Capacity);

            FormatCell(row.Cells.FromKey("Current+1"),Capacity);

            FormatCell(row.Cells.FromKey("Current+2"),Capacity);

            FormatCell(row.Cells.FromKey("Current+3"),Capacity);

            if (row.Cells.FromKey("number").Value != null)
            {
                if (row.Cells.FromKey("number").Value.ToString() == "33")
                {
                    row.Cells.FromKey("Prev").Value = "...";
                }
            }
            
        }

        private void SetingDataRow(UltraGridRow row)
        {
            row.Style.BackColor = Color.Transparent;
            
            if ((row.Cells.FromKey("Shift").Value != null) && ((decimal)row.Cells.FromKey("Shift").Value != 0))
            {
                SetImageFromRow(row);
            }

            HandlingTheSpecialCase(row);

            FormatRowValue(row);

            row.Cells.FromKey("Current").Style.Font.Bold = true;
        }

        protected int GetShiftFieldFromCode(string Code)
        {
            string Level3 = Code.Remove(0, Code.Length - 2);

            string Level2 = Code.Remove(0, Code.Length - 4);

            if (Level3 != "00")
            {
                return 30;
            }

            if (Level2 != "0000")
            {
                return 15;
            }
            return 0;
        }

        protected void SetShiftField(UltraGridRow row)
        {
            string Code = row.Cells.FromKey("Code").Text;
            if (!string.IsNullOrEmpty(Code))
            {
                int Shift = GetShiftFieldFromCode(Code);
                row.Cells.FromKey("Field").Style.Padding.Left = Shift;
            }
        }


        private void SetSpecialColorRow(UltraGridRow row)
        {
            if ((row.Cells.FromKey("MO").Value != null) && (row.Cells.FromKey("MO").Value.ToString() == "False"))
            {
                row.Cells.FromKey("Prev").Style.BackColor = Color.FromArgb(241, 241, 242);
                row.Cells.FromKey("Current").Style.BackColor = Color.FromArgb(241, 241, 242);
            }
            if ((row.Cells.FromKey("formula").Value != null) && (!string.IsNullOrEmpty(row.Cells.FromKey("formula").Value.ToString())))
            {
                row.Style.BackColor = Color.FromArgb(241, 241, 242);                
            }
            if ((row.Cells.FromKey("grouping").Value != null) && (row.Cells.FromKey("grouping").Value.ToString() == "True"))
            {
                row.Style.BackColor = Color.FromArgb(241, 241, 242);
            }
        }

        protected void ProcessingRow()
        {
            RowsCollection rows = Grid.Rows;

            int indexUnitCol = Grid.Columns.FromKey("Unit").Index;

            int CountTitle = 0;

            foreach (UltraGridRow row in rows)
            {
                SetShiftField(row);
                

                if (row.Cells[indexUnitCol].Text == "Title")
                {
                    SettingTitleRow(row, ++CountTitle);
                    
                }
                else
                    if (row.Cells[indexUnitCol].Text == "SubTitle")
                    {
                        SettingSubTitleRow(row);
                    }
                    else
                    {
                        SetingDataRow(row);
                    }
                SetSpecialColorRow(row);
                
 
            }
        }

        

        



        protected void CustomizeGrid()
        {
            SettingHeaderGrid();
            SettingColWidth();
            SettingCellStyle();
            ProcessingRow();
            
            
            //TODO
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.RowSelectorStyleDefault.Width = 1;
            Grid.Height = Unit.Empty;

            Grid.Columns.FromKey("number").Header.Style.BorderDetails.ColorLeft = Color.FromArgb(200, 200, 200);
            Grid.Columns.FromKey("number").Header.Style.BorderDetails.WidthLeft = 1;

            Grid.Columns.FromKey("number").CellStyle.BorderDetails.ColorLeft = Color.FromArgb(200, 200, 200);

            Grid.Columns.FromKey("Note").CellStyle.Wrap = true;

            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;

        }

        #endregion

        #region SettingHeader

        protected void SettingHeader()
        {
            HeaderGlobal.Text = "Доклад главы администраций о достигнутых значениях показателей эффективности деятельности органов местного самоуправления";
            SubHeaderGlobal.Text = string.Format("Показатели эффективности деятельности органов местного самоуправления за {1}, {0}", ComboRegion.SelectedValue, DataSources.SelectedValue);

            Page.Title = HeaderGlobal.Text;
        }
        

        #endregion

        private void CustomizeAndGenerationGrid()
        {
            GenerationGrid();
            CustomizeGrid();
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            int add = 0;
            if (BN == "IE")
            {
                add = -50;
            }
            Grid.Width = ScreenWidth-20+add;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);
            UltraGridExporter1.PdfExporter.RowExported += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs>(PdfExporter_RowExported);
            
        }

        



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InicializeCustomParams();

            LoadRegionSettingParam();

            FillMultiCombo();
            if (!Page.IsPostBack)
            {
                ConfigurationMultiCombo();
            }
                
            LoadUserParam();

            SettingHeader();

            CustomizeAndGenerationGrid();
        }

        private void BindTestGrid()
        {
            string query = DataProvider.GetQueryText("GridSqlQ");

            DataTable BaseTable = GetDBWar(query);

            Grid.DataSource = BaseTable;
            Grid.DataBind();
        }

        #region Экспорт в Excel
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;

            e.CurrentWorksheet.PrintOptions.PrintRowAndColumnHeaders = true;//
            e.CurrentWorksheet.PrintOptions.Header = "2:2";
            
            
            CustomView alternateView = e.Workbook.CustomViews.Add("Alternate Settings", true, true);
            
            PrintOptions po = alternateView.GetPrintOptions(e.CurrentWorksheet);
            po.PrintGridlines = 1 == 1;

            //ExcelDefaultableBoolean

            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 8);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 8);

            e.CurrentWorksheet.Rows[0].Cells[0].Value = "Показатели эффективности деятельности органов местного самоуправления городского округа (муниципального района) за 2010 год";//HeaderGlobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True; ;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 50 * 5;
            e.CurrentWorksheet.Rows[0].Height = 20 * 40;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 8);
            e.CurrentWorksheet.MergedCellsRegions.Add(3, 0, 3, 8);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = ComboRegion.SelectedValue;//SubHeaderGlobal.Text;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Italic = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Height = 50 * 4;
            e.CurrentWorksheet.Rows[2].Height = 10 * 40;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            ExportGrid(Grid, e.CurrentWorksheet, 4);

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 6;

            UltraGridExporter1.ExcelExporter.Export(Grid, sheet1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion

        #region Експор Грида

        protected void SetStyleHeadertableFromExcel(IWorksheetCellFormat CellFormat)
        {

            CellFormat.WrapText = ExcelDefaultableBoolean.True;

            CellFormat.FillPatternBackgroundColor = System.Drawing.Color.FromArgb(200, 200, 200);

            CellFormat.FillPatternForegroundColor = System.Drawing.Color.FromArgb
(200, 200, 200);

            CellFormat.FillPattern = FillPatternStyle.Default;

            CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            CellFormat.Alignment = HorizontalCellAlignment.Center;

            
        }

        object FormatVal(object value)
        {
            return value;
            object resValue = value;

            if (typeof(string) == value.GetType())
            {

                resValue = value.ToString().Replace("<br>", "\n");

            }

            return resValue;
        }

        int ExportHeaderGrid(HeadersCollection Headers, Worksheet WorkSheet, int StartRow)
        {
            int rowFirst = StartRow;

            int maxRow = 0;

            for (int i = 0; i < Headers.Count; i++)
            {
                int FirstAbscissaMearge = Headers[i].RowLayoutColumnInfo.OriginX;

                int FirstOrdinateMearge = Headers[i].RowLayoutColumnInfo.OriginY + StartRow;

                int LastAbscissaMearge = Headers[i].RowLayoutColumnInfo.SpanX + FirstAbscissaMearge - 1;

                int LastOrdinateMearge = Headers[i].RowLayoutColumnInfo.SpanY + FirstOrdinateMearge - 1;

                if (LastOrdinateMearge > maxRow)
                {
                    maxRow = LastOrdinateMearge;
                }

                try
                {
                    WorkSheet.MergedCellsRegions.Add(FirstOrdinateMearge, FirstAbscissaMearge, LastOrdinateMearge, LastAbscissaMearge);

                    SetStyleHeadertableFromExcel(WorkSheet.Rows[FirstOrdinateMearge].Cells[FirstAbscissaMearge].CellFormat);

                    WorkSheet.Rows[FirstOrdinateMearge].Cells[FirstAbscissaMearge].Value = FormatVal(Headers[i].Caption);        
                }
                catch
                {
                    //TODO разобратся с левыми хедерами
                }
            }

            maxRow++;

            for (int i = rowFirst; i < maxRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 40;
            }

            return maxRow;

        }

        void ExportGrid(UltraWebGrid G, Worksheet sheet, int startrow)
        {
            ExportGridToExcel(G, sheet, startrow, 1 == 1);
        }

        void ExportWidthGrid(UltraWebGrid G, Worksheet sheet)
        {
            for (int i = 0; i < G.Columns.Count; i++)
            {
                if (G.Columns[i].Hidden)
                {
                    G.Columns.Remove(G.Columns[i]);
                    i--;
                }
                
                
            }
            for (int i = 0; i < G.Columns.Count; i++)
            {
                sheet.Columns[i].Width = (int)G.Columns[i].Width.Value * 36;
                
                sheet.Columns[i].CellFormat.FormatString = G.Columns[i].Format;
            }
        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {


            foreach (UltraGridColumn col in G.Columns)
            {
                if ((col.Hidden)||(col.Width.Value == 0))
                {
                    G.Bands[0].HeaderLayout.Remove(col.Header);
                    G.Columns.Remove(col);
                }
            }
            G.Columns.Remove(G.Columns.FromKey("Code"));
            ExportWidthGrid(G, sheet);

            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);

            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 22 * 40;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                for (int j = 0; j < 10;j++ )
                    try
                    {
                        sheet.Rows[i + startrow].Cells[j].Value = sheet.Rows[i + startrow].Cells[j].Value.ToString() + "";
                        double Value = double.Parse(sheet.Rows[i + startrow].Cells[j].Value.ToString());
                        sheet.Rows[i + startrow].Cells[j].Value = string.Format("{0:########0.#########}", Value);
                    }
                    catch { }
                
            }
        }
        
        #endregion

        #region Экспорт в PDF


        void PDFAddTitleReport(ISection Sectin, string TextHeader)
        {
            IText title = Sectin.AddText();

            Font font = new Font("Verdana", 16);

            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Middle;

            //title.AddContent("Показатели эффективности деятельности органов местного самоуправления городского округа (муниципального района) за 2010 год");
            title.AddContent(TextHeader);
            title.Height = new FixedHeight(60); 
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report r = new Report();
            
            ISection e_ = r.AddSection();

            //PDFAddTitleReport(


            e_.PageSize = new PageSize(1000, 600);
            IText title = e_.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Middle;
            title.AddContent("Показатели эффективности деятельности органов местного самоуправления городского округа (муниципального района) за 2010 год");
            title.Height = new FixedHeight(60);

            title = e_.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Italic = true;
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Top;
            title.AddContent(ComboRegion.SelectedValue+"\n");
            title.Height = new FixedHeight(30);

            Grid.Bands[0].HeaderLayout.Clear();
            UltraGridExporter1.PdfExporter.Export(Grid, e_);
        }

        #region Рисовалка иерархичного хидера
        Background headerBackground = null;
        Borders headerBorders = null;        
        private bool HeaderIsChildren(HeaderBase Parent, HeaderBase Children)
        {
            if (Parent == Children)
            {
                return false;
            }

            if (((Parent.RowLayoutColumnInfo.OriginY + Parent.RowLayoutColumnInfo.SpanY) == Children.RowLayoutColumnInfo.OriginY)
                &&
                ((Parent.RowLayoutColumnInfo.OriginX <= Children.RowLayoutColumnInfo.OriginX) &&
                ((Parent.RowLayoutColumnInfo.OriginX + Parent.RowLayoutColumnInfo.SpanX) > Children.RowLayoutColumnInfo.OriginX)))
            {
                return true;
            }
            return false;
        }

        private List<HeaderBase> GetChildHeader(HeaderBase ParentHeder)
        {
            List<HeaderBase> ChildHeader = new List<HeaderBase>();

            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsChildren(ParentHeder, Header))
                {
                    ChildHeader.Add(Header);
                }
            }

            return ChildHeader;
        }

        protected bool HeaderIsRootLevel(HeaderBase Header)
        {
            return Header.RowLayoutColumnInfo.OriginY == 0;
        }

        ITableRow CreateChildrenRow(ITableCell row)
        {
            return row.Parent.Parent.AddRow();
        }

        int[] PDFHeaderHeightsLevel = { 20, 20, 20 };

        int PDFGetLevelHeight(int level, int span)
        {
            int sumHeightLevel = 0;
            for (int i = level; i < level + span; i++)
            {
                sumHeightLevel += PDFHeaderHeightsLevel[i];
            }
            return sumHeightLevel;
        }

        private int CreateHierarhyHeader(HeaderBase header, ITableRow row)
        {
            List<HeaderBase> ChildHeaders = GetChildHeader(header);
            row = row.AddCell().AddTable().AddRow();

            ITableCell ParentCell = row.AddCell();

            int width = AddTableCell(ParentCell, header, header.RowLayoutColumnInfo.SpanX, PDFGetLevelHeight(header.RowLayoutColumnInfo.OriginY, header.RowLayoutColumnInfo.SpanY));

            if (ChildHeaders.Count > 0)
            {
                width = 0;
                ITableRow ChildrenRow = row.Parent.AddRow();
                foreach (HeaderBase ChildHeader in ChildHeaders)
                {
                    width += CreateHierarhyHeader(ChildHeader, ChildrenRow);
                }
                
                setHederWidth(ParentCell, width);
            }
            return width;

        }        

        private int CreateAllRootHeader(ITableRow RootRow)
        {
            int sumW = 0;
            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsRootLevel(Header))
                {
                    sumW += CreateHierarhyHeader(Header, RootRow);
                }
            }
            return sumW;
        }

        private void ExportHeader(ITable Table)
        {
            ITableRow RootRow = Table.AddRow();

            ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();

            int sumW = CreateAllRootHeader(RootRow);
            Table.Width = new FixedWidth(sumW);
        }
       
        private void ApplyHeader()
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden))
                {
                    continue;
                }
                Grid.Bands[0].HeaderLayout.Add(col.Header);
            }
        }

        class sortHeder : IComparer
        {
            public int Compare(object x, object y)
            {

                if (((HeaderBase)x).RowLayoutColumnInfo.OriginX > ((HeaderBase)y).RowLayoutColumnInfo.OriginX)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        string KeyMainColumn = "Field";
        private void PreProcessing(MarginCellExportingEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            //Скрываем хедер рисуемый по умолчанию
            e.ReportCell.Parent.Height = new FixedHeight(0);

            ApplyHeader();

            SettingHeaderGrid();

            SettingColWidth();

            Grid.Bands[0].HeaderLayout.Sort(new sortHeder());
        }

        private void PdfExporter_Test(object sender, MarginCellExportingEventArgs e)
        {
            if (headerBackground != null)
            {
                return;
            }

            PreProcessing(e);

            ITable Table = e.ReportCell.Parent.Parent;

            ExportHeader(Table);
        }

        #region UtilsFromExportGridToPDF
        private int AddTableCell(ITableCell tableCell, HeaderBase header, Double width, Double Height)
        {
            if (header.Column != null)
            {
                width = 0.75 * (int)header.Column.Width.Value;
            }

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();
            SetFontStyle(text);

            text.AddContent(header.Caption);

            return (int)width;
        }
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }
        private ITableCell AddTableCell(ITableRow row, string cellText, Double width, Double Height)
        {
            ITableCell tableCell = row.AddCell();

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();

            text.Style.Font.Size = 1;
            text.Paddings.Left = 100;
            SetFontStyle(text);

            text.AddContent(cellText);

            return tableCell;
        }

        void setHederWidth(ITableCell tableCell, Double width)
        {
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
        }
        #endregion
        #endregion 
         
        #region Рисовалка шапки у грида на каждой страничке в ПДФе        
        Graphics graphics = Graphics.FromImage(new Bitmap(1000, 500));
        private int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = graphics.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            if (rect.Height < 23)
            {
                return 23;
            }

            return rect.Height;
        }
        private Font GetSystemFont(FontInfo baseFont)
        {
            //FontInfo f;

            //FontStyle styleFont = FontStyle.Regular;
            //if(baseFont.Bold)
            //{
            //    styleFont = FontStyle.Bold;
            //}

            baseFont = Grid.DisplayLayout.EditCellStyleDefault.Font;

            Font font = new Font(baseFont.Name, (float)8.5);//, (int)baseFont.Size.Unit.Value,styleFont); 

            return font;
        }

        int PreExportedHeight = 240;

        int GetRowHeight(UltraGridRow row)
        {
            int maxHeight = 0;
            foreach (UltraGridCell cell in row.Cells)
            {
                int CurHeight = GetStringHeight(cell.Text, GetSystemFont(cell.Column.CellStyle.Font), (int)cell.Column.Width.Value);
                maxHeight = CurHeight > maxHeight ? CurHeight : maxHeight;
            }

            return maxHeight;
        }

        void PdfExporter_RowExported(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs e)
        {
            e.ReportRow.KeepWithNext = true;
            if (PreExportedHeight > 1000)
            {
                e.ReportRow.KeepWithNext = false;

                PreExportedHeight = 60;

                ITableRow headerMainRow = e.ReportRow.Parent.AddRow();

                headerMainRow.KeepWithNext = true;

                CreateAllRootHeader(headerMainRow);
            }

            e.ReportRow.Margins.All = 0;

            PreExportedHeight += GetRowHeight(e.GridRow);
        }
        #endregion

        #endregion

        #region -_-
        #region o_-
        #region O_-
        #region O_o
        #region o_O
        #region -_O
        #region -_o
        #region -_-
        #region o_o
        #region p_p
        #region q_q
        #region Q_Q
        #region O_O
        #region *_*
        #region &_&
        #region 6_6
        #region !_!
        #region ~_~
        #region >_<
        #region э_e
        #region b_d
        #region ё_ё
        #region $_$
        #region o,O
        #region o/O
        #region -/O
        #region v_v
        #region W_W
        #region ^.^
        #region @_@
        #region /_\
        #region "_"
        #region М_М
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
    }
}
