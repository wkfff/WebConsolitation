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
using Infragistics.Documents.Reports;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.MO_SP_0001.Default.reports.MO_SP_0001
{
    public partial class _default : CustomReportPage
    {
        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }

        CustomParam SelectField { get { return (UserParams.CustomParam("Field")); } }
        CustomParam SelectYear { get { return (UserParams.CustomParam("Year-Quart")); } }
        CustomParam SelectRegion { get { return (UserParams.CustomParam("Region")); } }
        CustomParam SelectPeriod { get { return (UserParams.CustomParam("SelectPeriod")); } }
        string pokType = "";
        Decimal ValueXMAO = 0;

        bool ReveceField = false;
        string UnitField = "";
        int MaxGridRang = 0;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        ICustomizerSize CustomizerSize;

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            protected abstract void ContfigurationGridIE(UltraWebGrid Grid);
            protected abstract void ContfigurationGridFireFox(UltraWebGrid Grid);
            protected abstract void ContfigurationGridGoogle(UltraWebGrid Grid);

            public virtual void ContfigurationGrid(UltraWebGrid Grid)
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        this.ContfigurationGridIE(Grid);
                        return;
                    case BrouseName.FireFox:
                        this.ContfigurationGridFireFox(Grid);
                        return;
                    case BrouseName.SafariOrHrome:
                        this.ContfigurationGridGoogle(Grid);
                        return;
                    default:
                        throw new Exception("Охо!");
                }
            }

            public ICustomizerSize(BrouseName NameBrouse)
            {
                this.NameBrouse = NameBrouse;
            }

            public static BrouseName GetBrouse(string _BrouseName)
            {
                if (_BrouseName == "IE") { return BrouseName.IE; };
                if (_BrouseName == "FIREFOX") { return BrouseName.FireFox; };
                if (_BrouseName == "APPLEMAC-SAFARI") { return BrouseName.SafariOrHrome; };

                return BrouseName.IE;
            }

            public static ScreenResolution GetScreenResolution(int Width)
            {
                ////CRHelper.SaveToErrorLog(Width.

                if (Width < 801)
                {
                    return ScreenResolution._800x600;
                }
                if (Width < 1281)
                {
                    return ScreenResolution._1280x1024;
                }
                return ScreenResolution.Default;
            }
        }

        class CustomizerSize_800x600 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 740;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 755;
                    default:
                        return 800;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 740;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 750;
                    default:
                        return 800;
                }
            }

            public CustomizerSize_800x600(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {

                    col.Width = onePercent * (100 - 63 - 5 - 10) / (Grid.Columns.Count - 3);


                }
                Grid.Columns[2].Width = onePercent * 20;
                Grid.Columns[1].Width = onePercent * 50;
                Grid.Columns[0].Width = onePercent * 4;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 65 - 5 - 10) / (Grid.Columns.Count - 3);
                }
                Grid.Columns[2].Width = onePercent * 20;
                Grid.Columns[1].Width = onePercent * 55;
                Grid.Columns[0].Width = onePercent * 4;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 67 - 5 - 10) / (Grid.Columns.Count - 3);
                }
                Grid.Columns[2].Width = onePercent * 20;
                Grid.Columns[1].Width = onePercent * 60;
                Grid.Columns[0].Width = onePercent * 4;
            }

            #endregion

        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    default:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                }
            }

            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 45) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 40;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 45) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 40;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 45) / (Grid.Columns.Count - 1);
                }
                
                Grid.Columns[0].Width = onePercent * 40;
            }

            #endregion

        }
        #endregion

        #region SettingHeader

        protected void SettingHeader()
        {
            HeaderGlobal.Text = "Сравнение населенных пунктов по показателям социально-экономического развития";
            if (Page.IsPostBack)
            {
                SubHeaderGlobal.Text = string.Format("Анализ социально-экономического положения территории по показателю  «{0}» в разрезе СП, {2}, {1}.", ComboField.SelectedValue, ComboYear.SelectedValue,ComboRegion.SelectedValue);
            }
            else
            {
                EmptyLabel.Visible = false;
                SubHeaderGlobal.Text = "";
            }
            
            HeaderChart.Text = string.Format("Распределение населенных пунктов по показателю «{0}»" + (string.IsNullOrEmpty(UnitField) ? "" : ", " + UnitField.ToLower()), ComboField.SelectedValue);

            Page.Title = HeaderGlobal.Text;
        }
        

        #endregion

        IDataSetCombo SetYear;
        IDataSetCombo SetField;
        IDataSetCombo SetRegion;


        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SpareMASDataProvider;

            public static DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, QueryId);
            }

            public static DataTable ExecQueryByID(string QueryId, string FirstColName)
            {
                string QueryText = DataProvider.GetQueryText(QueryId);
                DataTable Table = ExecQuery(QueryText, FirstColName);
                return Table;
            }

            public static DataTable ExecQuery(string QueryText, string FirstColName)
            {
                DataTable Table = new DataTable();
                ActiveDataPorvider.GetDataTableForChart(QueryText, FirstColName, Table);
                return Table;
            }

        }

        #region Combo Binds
        abstract class IDataSetCombo
        {
            public enum TypeFillData
            {
                Linear, TwoLevel
            }

            public int LevelParent = 2;

            public string LastAdededKey { get; protected set; }

            public readonly Dictionary<string, string> DataUniqeName = new Dictionary<string, string>();

            public readonly Dictionary<string, int> DataForCombo = new Dictionary<string, int>();

            public readonly Dictionary<string, Dictionary<string, string>> OtherInfo = new Dictionary<string, Dictionary<string, string>>();

            protected void addOtherInfo(DataTable Table, DataRow Row, string Key)
            {
                Dictionary<string, string> Info = new Dictionary<string, string>();
                foreach (DataColumn col in Table.Columns)
                {
                    Info.Add(col.ColumnName, Row[col].ToString());
                }
                OtherInfo.Add(Key, Info);
            }

            public delegate string FormatedDispalyText(string ParentName, string ChildrenName);
            protected FormatedDispalyText FormatingText;
            public void SetFormatterText(FormatedDispalyText FormatterText)
            {
                FormatingText = FormatterText;
            }

            public delegate string FormatedDispalyTextParent(string ParentName);
            protected FormatedDispalyTextParent FormatingTextParent;
            public void SetFormatterTextParent(FormatedDispalyTextParent FormatterText)
            {
                FormatingTextParent = FormatterText;
            }

            protected string GetAlowableAndFormatedKey(string BaseKey, string Parent)
            {
                string Key = BaseKey;//FormatingText(Parent, BaseKey);

                while (DataForCombo.ContainsKey(Key))
                {
                    Key += " ";
                }

                return Key;
            }

            protected void AddItem(string Child, int level, string UniqueName)
            {
                string RealChild = Child;

                DataForCombo.Add(RealChild, level);

                DataUniqeName.Add(RealChild, UniqueName);

                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);

        }

        class DataSetComboLinear : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();
                    string Child = "";// ParserDataUniqName.GetDouwnLevel(UniqueName);

                    DataForCombo.Add(Child, 0);

                    DataUniqeName.Add(Child, UniqueName);
                }
            }
        }

        class DataSetComboHierarhy : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "");

                    this.AddItem(DisplayNAme, int.Parse(row["LevelNumber"].ToString()), UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
                } 
            }
        }

        class DataSetComboYearQuart : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();
                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "");

                    this.AddItem(DisplayNAme, 0, UniqueName);
                    this.addOtherInfo(Table, row, DisplayNAme);

                    UniqueName = row["UniqueName"].ToString() + ".[Полугодие 1].[Квартал 1]";
                    DisplayNAme = this.GetAlowableAndFormatedKey("Квартал 1", "");

                    this.AddItem(DisplayNAme, 1, UniqueName);
                    this.addOtherInfo(Table, row, DisplayNAme);

                    UniqueName = row["UniqueName"].ToString() + ".[Полугодие 1].[Квартал 2]";
                    DisplayNAme = this.GetAlowableAndFormatedKey("Квартал 2", "");

                    this.AddItem(DisplayNAme, 1, UniqueName);
                    this.addOtherInfo(Table, row, DisplayNAme);

                    UniqueName = row["UniqueName"].ToString() + ".[Полугодие 2].[Квартал 3]";
                    DisplayNAme = this.GetAlowableAndFormatedKey("Квартал 3", "");

                    this.AddItem(DisplayNAme, 1, UniqueName);
                    this.addOtherInfo(Table, row, DisplayNAme);

                    UniqueName = row["UniqueName"].ToString() + ".[Полугодие 2].[Квартал 4]";
                    DisplayNAme = this.GetAlowableAndFormatedKey("Квартал 4", "");

                    this.AddItem(DisplayNAme, 1, UniqueName);
                    this.addOtherInfo(Table, row, DisplayNAme);
                }
            }
        }
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Visible = false;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            int add = 0;
            if (BN == "IE")
            {
                add = -50;
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }else

                if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
                {
                    CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
                }
                else
                {
                    CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
                }

            Grid.Width = CustomizerSize.GetGridWidth();
            Grid.Height = Unit.Empty;

            Chart.Width = CustomizerSize.GetChartWidth();

            Chart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(Chart_InvalidDataReceived);

            HyperLink1.NavigateUrl = "~/reports/MO_SP_0002/default.aspx";
            HyperLink1.Text = "Паспорт&nbsp;населенного&nbsp;пункта";
        }

        void Chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        

        #region ComboSetting

        private void FillComboField()
        {
            ComboField.ParentSelect = false;            
            ComboField.Width = 550;            

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboField", "DisplayName");            
            SetField = new DataSetComboHierarhy();
            SetField.LoadData(Table);
            if (!Page.IsPostBack) 
            {
                ComboField.FillDictionaryValues(SetField.DataForCombo);
                
            }
        }

        private void FillComboRegion()
        {
            ComboRegion.Width = 300;
            ComboRegion.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboRegion", "DisplayName");

            SetRegion = new DataSetComboHierarhy();
            SetRegion.LoadData(Table);
            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(SetRegion.DataForCombo);
                //ComboRegion.SetСheckedState("2010", 1 == 1);
            }
        }


        private void FillComboYear()
        {   
            ComboYear.Width = 250;
            ComboYear.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");



            Dictionary<string, int> yearLevelDictionary = new Dictionary<string, int>();

            for (int i = 1; i < Table.Rows.Count; i++)
            {
                if (Table.Rows[i][2].ToString() == "Год")
                {
                    yearLevelDictionary.Add(Table.Rows[i][0].ToString() + " год", 0);
                }
                if (Table.Rows[i][2].ToString() == "Квартал")
                {
                    yearLevelDictionary.Add(Table.Rows[i][0].ToString() + " " + Table.Rows[i][1].ToString().Split('[')[4].TrimEnd('.').TrimEnd(']') + " года", 1);
                }
            }


            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(yearLevelDictionary);
            }
        }

        

        private void FillCombo()
        {
            FillComboField();
            FillComboYear();
            FillComboRegion();
        }
        #endregion

        List<string> GetListUniqueFromCombo(IDataSetCombo ComboSet, CustomMultiCombo Combo)
        {
            List<string> list = new List<string>();

            foreach (string Caption in Combo.SelectedValues)
            {   
                list.Add(ComboSet.DataUniqeName[Caption]);                
            }
            return list;
        }     

        
        private void FillParam()
        {
            SelectField.Value = SetField.DataUniqeName[ComboField.SelectedValue];
            ReveceField = SetField.OtherInfo[ComboField.SelectedValue]["reverce"] == "1";
            UnitField =
                (SetField.OtherInfo[ComboField.SelectedValue]["Unit"] == "Неизвестные данные" ? "" : SetField.OtherInfo[ComboField.SelectedValue]["Unit"]);
            if (ComboYear.SelectedNode.Level==1)
            {
                if (ComboYear.SelectedValue.Split(' ')[1]=="1" || ComboYear.SelectedValue.Split(' ')[1]=="2")
                {
                    SelectYear.Value ="[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].["+ComboYear.SelectedNode.Parent.Text.Split(' ')[0]+"].[Полугодие 1].[Квартал "+ComboYear.SelectedValue.Split(' ')[1]+"]" ;//SetYear.DataUniqeName[ComboYear.SelectedValue];
                }
                else
                {
                    SelectYear.Value ="[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].["+ComboYear.SelectedNode.Parent.Text.Split(' ')[0]+"].[Полугодие 2].[Квартал "+ComboYear.SelectedValue.Split(' ')[1]+"]" ;
                }
            }
            else
            {
                SelectYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Text.Split(' ')[0] + "]";
            }
            
            SelectRegion.Value = SetRegion.DataUniqeName[ComboRegion.SelectedValue];

        }

        #region DataBindGrid
        struct Region_Value
        {
            public string region;
            public System.Decimal Value;
        }

        class Comparer_reverce : Comparer<Region_Value>
        {

            public override int Compare(Region_Value x, Region_Value y)
            {
                if (x.Value > y.Value)
                { return 1; }
                else
                    if (x.Value < y.Value)
                    { return -1; }
                    else
                    { return 0; }
            }
        }

        class Comparer_ : Comparer<Region_Value>
        {

            public override int Compare(Region_Value x, Region_Value y)
            {
                if (x.Value < y.Value)
                { return 1; }
                else
                    if (x.Value > y.Value)
                    { return -1; }
                    else
                    { return 0; }
            }
        }

        private List<Region_Value> CalculateRang(DataTable BaseTable)
        {
            List<Region_Value> ListRang = new List<Region_Value>();

            foreach (DataRow row in BaseTable.Rows)
            {
                Region_Value r_v = new Region_Value();
                r_v.region = row[0].ToString();
                if (row[2] == DBNull.Value)
                {
                    r_v.Value = Decimal.MinValue;
                }
                else
                {
                    r_v.Value = (decimal)row[2];
                }
                ListRang.Add(r_v);
            }
            //
            if (ReveceField)
            {
                ListRang.Sort(new Comparer_reverce());
            }
            else
            {
                ListRang.Sort(new Comparer_());      
            }
            

            return ListRang;
        }

        private string GetRang(List<Region_Value> RList, string RegionName)
        {
            int indexForRegion ;
            
            for (indexForRegion = 0; RList[indexForRegion].region != RegionName; indexForRegion++) ;                      

            Decimal RegionVal = RList[indexForRegion].Value;

            int MinRang = int.MaxValue;
            int MaxRang = int.MinValue;

            for (int i = 0; i < RList.Count; i++)
            {
                if (RegionVal == RList[i].Value)
                {
                    if (MinRang == int.MaxValue)
                    {
                        MinRang = i + 1;
                    }
                    MaxRang = i + 1;
                  
                }
            }

            if (MinRang != MaxRang)
            {
                return MinRang.ToString();// + "-" + MaxRang.ToString();
            }
            else
            {
                return MinRang.ToString();
            }
        }

        DataTable BaseTable;

        double urfoAverage = 0;
        private void DataBindGridQuart()
        {
            BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Region");
            if (BaseTable.Rows[0][2] != DBNull.Value)
            {
                ValueXMAO = (Decimal)BaseTable.Rows[0][2];
            }
            else
            {
                ValueXMAO = Decimal.MinValue;
            }
            if (BaseTable.Rows[0].ItemArray[2]!=DBNull.Value)
            {
                urfoAverage = double.Parse(BaseTable.Rows[0].ItemArray[2].ToString());
            }
            else
            {
                urfoAverage=0;
            }
            pokType = BaseTable.Rows[0].ItemArray[BaseTable.Columns.Count - 1].ToString();
            BaseTable.Columns.Remove(BaseTable.Columns[BaseTable.Columns.Count - 1]);
            BaseTable.Rows.Remove(BaseTable.Rows[0]);

            DataTable GridTable = new DataTable();
            GridTable.Columns.Add("Region");
            GridTable.Columns.Add("CurVal", typeof(decimal));
            GridTable.Columns.Add("Deviation", typeof(decimal));
            GridTable.Columns.Add("temp",typeof(decimal));
            GridTable.Columns.Add("Rang");
            for (int i = 0; i < BaseTable.Rows.Count;i++)
            {
                if (BaseTable.Rows[i]["Parent"].ToString() != SelectRegion.Value)
                {
                    BaseTable.Rows[i].Delete();
                    i--;
                }                
            }

            List<Region_Value> Region_Val = CalculateRang(BaseTable);

            MaxGridRang = Region_Val.Count;

            foreach (DataRow BaseRow in BaseTable.Rows) 
            {
                if (BaseRow["Parent"].ToString() != SelectRegion.Value)//SetRegion.DataUniqeName[SelectRegion.Value])
                {                    
                    continue;
                }

                DataRow GridRow = GridTable.NewRow();
                GridTable.Rows.Add(GridRow);

                GridRow["Region"] = BaseRow["Region"];
                GridRow["CurVal"] = BaseRow[2];
                if ((BaseRow[1] != DBNull.Value) & (BaseRow[2] != DBNull.Value))
                    {
                        GridRow["Deviation"] = (decimal)BaseRow[2] - (decimal)BaseRow[1];
                        GridRow["temp"] = (decimal)BaseRow[2] / (decimal)BaseRow[1];
                        
                    }
                if ((BaseRow[2] != DBNull.Value))
                {
                    GridRow["Rang"] = 0;
                    //GridRow["Rang"] = GetRang(Region_Val, BaseRow["Region"].ToString());
                }
            }



            Grid.DataSource = GridTable;
            Grid.DataBind();
        }
        #endregion


        #region ConfGrid
        private void ConfHeader()
        {
            Grid.Columns.FromKey("Region").Header.Caption = "Территория";
            Grid.Columns.FromKey("CurVal").Header.Caption = "Значение" + (string.IsNullOrEmpty(UnitField) ? "" : ", "+UnitField.ToLower());
            Grid.Columns.FromKey("Deviation").Header.Caption = "Абсолютное отклонение от предыдущего периода";
            Grid.Columns.FromKey("temp").Header.Caption = "Темп роста к предыдущему периоду";
            Grid.Columns.FromKey("Rang").Header.Caption = "Ранг по СП в МО";

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.Header.Style.Wrap = true;
                col.Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        } 

        private void ConfColumn()
        {             
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("CurVal"),"N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("Deviation"),"N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("temp"), "### ### ##0.00%");
            Grid.Columns.FromKey("Rang").CellStyle.HorizontalAlign = HorizontalAlign.Center;            
        }

        private void ConfCol()
        {
            ConfHeader();
            ConfColumn();

            CustomizerSize.ContfigurationGrid(Grid);
        }

        public static void SetImageFromGrowth(UltraGridRow row, string ColumnGrowthName, bool inverce)
        {
            if ((row.Cells.FromKey(ColumnGrowthName).Value == null) || ((decimal)row.Cells.FromKey(ColumnGrowthName).Value == 1))
            {
                return;
            }
            bool IsReverce = inverce;
            decimal valShift = (decimal)row.Cells.FromKey(ColumnGrowthName).Value;
            string UpOrDouwn = valShift > 1 ? "Up" : "Down";
            string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) ||
                                ((UpOrDouwn == "Down") && (IsReverce))) ?
                                              "Green" : "Red";
            string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";
            string ImagePath = "~/images/" + ImageName;
            row.Cells.FromKey(ColumnGrowthName).Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            row.Cells.FromKey(ColumnGrowthName).Style.BackgroundImage = ImagePath;
                        
        }

        private void SetImageFromRang(UltraGridRow row, string ColName, bool reverce)
        {

            string MaxRangCaption = MaxGridRang.ToString();
            CRHelper.SaveToErrorLog(MaxRangCaption);

            if (row.Cells.FromKey(ColName).Value == null)
            {
                return;
            }

            if ((row.Cells.FromKey(ColName).Text == "1-" + MaxRangCaption))
            {
                row.Cells.FromKey(ColName).Title = "Наилучшее значение по ХМАО-Югре";
                row.Cells.FromKey(ColName).Style.BackgroundImage = "~/images/starYellowBB.png";
                row.Cells.FromKey(ColName).Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
            else
            { 

                if ((row.Cells.FromKey(ColName).Text == "1") || ((row.Cells.FromKey(ColName).Text.Contains("1-") && (row.Cells.FromKey(ColName).Text[1] == '-'))))
                {
                    row.Cells.FromKey(ColName).Title = "Наилучшее значение по МО";
                    row.Cells.FromKey(ColName).Style.BackgroundImage = reverce?"~/images/starGrayBB.png":"~/images/starYellowBB.png";
                    row.Cells.FromKey(ColName).Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if ((row.Cells.FromKey(ColName).Text == MaxRangCaption) || row.Cells.FromKey(ColName).Text.Contains(MaxRangCaption))
                {
                    row.Cells.FromKey(ColName).Title = "Наихудшее значение по МО";
                    row.Cells.FromKey(ColName).Style.BackgroundImage = !reverce ? "~/images/starGrayBB.png" : "~/images/starYellowBB.png"; ;
                    row.Cells.FromKey(ColName).Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }

        private void ConfRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                SetImageFromGrowth(Row, "temp", ReveceField);
              //  SetImageFromRang(Row, "rang", false);
            //    Row.Cells.FromKey("rang").Style.Font.Bold = true;
            }
        }                

        private void ConfDisplayGrid()
        {
            ConfCol();
            ConfRow();            
        }
        #endregion

        #region Conf-Bind-Chart
        private void ConfChart()
        {
            Chart.Axis.Y.Extent  = 40;
            Chart.Axis.Y.Labels.SeriesLabels.FormatString = "### ### ##0";

            Chart.Tooltips.FormatString = "<ITEM_LABEL> <br><b><DATA_VALUE:### ##0.##></b>" + (string.IsNullOrEmpty(UnitField) ? "" : ", " + UnitField.ToLower());

            Chart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(Chart_FillSceneGraph);
            
        }

        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (SetField.OtherInfo[ComboField.SelectedValue]["Comporable"] != "0")
            {
                //Line l = new Line(
                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

               /* #region Лини
                if (ValueXMAO != Decimal.MinValue)
                {
                    double y = yAxis.Map(ValueXMAO);
                    double x0 = xAxis.MapMaximum;
                    double x1 = xAxis.MapMinimum;
                    Line l_ = new Line(new Point((int)x0, (int)y), new Point((int)x1, (int)y));
                    l_.PE.Fill = Color.Red;                    
                    l_.PE.StrokeWidth = 3;
                    e.SceneGraph.Add(l_);


                    Text CaptionLine = new Text(new Point((int)x0, (int)y), "Ханты-Мансийский автономный округ Югра");
                    e.SceneGraph.Add(CaptionLine);

                }
                #endregion*/
            }
        }

        private void DataBindChart()
        {
            DataTable TableChart = BaseTable.Copy();
            for (int i = 0; i < TableChart.Rows.Count; i++)
            {
                if (TableChart.Rows[i]["Parent"].ToString() != SelectRegion.Value)
                {
                    TableChart.Rows[i].Delete();
                    i--;
                }
            }

            TableChart.Columns.Remove(TableChart.Columns[1]);
            TableChart.Columns.Remove(TableChart.Columns[0]);

            Chart.DataSource = TableChart;
            Chart.DataBind();
        }
        #endregion


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                GridTable.Visible = false;
                ChartTable.Visible = false;
            }
            
            FillCombo();            



            FillParam();
            if (ComboYear.SelectedNode.Level==0)
            {
                SelectPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Text+ "]";
            }
            else
            {
                if (ComboYear.SelectedNode.Text.EndsWith("3") || ComboYear.SelectedNode.Text.EndsWith("4"))
                {
                    SelectPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[" + ComboYear.SelectedNode.Text+"]";
                }
                else
                {
                    SelectPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 1].[" + ComboYear.SelectedNode.Text + "]";
                }
            }
            
            DataBindGridQuart();
            calculateRank(Grid, 1);
            
            ConfDisplayGrid();

            ConfChart();

            DataBindChart();

            

            if (BaseTable.Rows.Count <= 1 ||
                (ComboField.SelectedValue == "Среднемесячная заработная плата  местной администрации (исполнительно-распорядительные органы муниципальных образований)"))
            {
                GridTable.Visible = false;
                ChartTable.Visible = false;
                EmptyLabel.Text = "Нет данных<br>";
                EmptyLabel.Visible = true;
            }
            if (BaseTable.Rows.Count > 1 || 
                 (ComboYear.SelectedNode.Level == 0 &&
                    (ComboField.SelectedValue == "Фонд заработной платы всех работников организаций" ||
                    ComboField.SelectedValue == "Среднемесячная заработная плата работников организаций" ||
                    ComboField.SelectedValue == "Среднемесячная заработная плата работников местных администраций (исполнительно-распорядительных органов муниципальных образований)" ||
                    ComboField.SelectedValue == "Официально зарегистрированные безработные, получающие пособие по безработице" ||
                    ComboField.SelectedValue == "Поголовье крупного рогатого скота в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)" ||
                    ComboField.SelectedValue == "Поголовье коров в сельскохозяйственных организациях" ||
                    ComboField.SelectedValue == "Поголовье свиней в сельскохозяйственных организациях" ||
                    ComboField.SelectedValue == "Поголовье птиц в сельскохозяйственных организациях"))
              )
            {
                if (BaseTable.Rows.Count > 1)
                {
                    GridTable.Visible = true;
                    ChartTable.Visible = true;
                    EmptyLabel.Text = "Нет данных<br>";
                    EmptyLabel.Visible = false;
                }
            }
            SettingHeader();
        }


        #region Осторожно! бЫдлА-кот =^_^=
        //todo
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = HeaderGlobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = SubHeaderGlobal.Text ;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Таблица"].MergedCellsRegions.Add(1,0,1,9);
            ExportGridToExcel(Grid, e.CurrentWorksheet.Workbook.Worksheets["Таблица"], 3, true);
            e.Workbook.Worksheets["Таблица"].Rows[1].Height = 600;
            e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Justify;
            e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            try
            {
                e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = HeaderChart.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0], Chart);
            }
            catch { }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            //if (IndexActiveRow.Value != "")
            //    ActionRowActive(Grid.Rows[int.Parse(IndexActiveRow.Value)]);
            //GenerationChart();
            DataBindChart();
             
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }


            Chart.Width = (int)(Chart.Width.Value * 0.75);
            Chart.Legend.Margins.Right = (int)(Chart.Legend.Margins.Right * 0.5);
            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 5;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";
            ReportExcelExporter1.ExcelExporter.Export(Grid, sheet1);
        }
        #region Експор Грида
        protected void SetStyleHeadertableFromExcel(IWorksheetCellFormat CellFormat)
        {
            CellFormat.WrapText = ExcelDefaultableBoolean.True;
            CellFormat.FillPatternBackgroundColor = System.Drawing.Color.FromArgb(200, 200, 200);
            CellFormat.FillPatternForegroundColor = System.Drawing.Color.FromArgb(200, 200, 200);
            CellFormat.FillPattern = FillPatternStyle.Default;
            CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            CellFormat.Alignment = HorizontalCellAlignment.Center;

            CellFormat.FillPatternForegroundColor = Color.LightGray;
            CellFormat.FillPatternBackgroundColor = Color.LightGray;
            CellFormat.TopBorderColor = Color.Gray;
            CellFormat.BottomBorderColor = Color.Gray;
            CellFormat.LeftBorderColor = Color.Gray;
            CellFormat.RightBorderColor = Color.Gray;
        }

        object FormatVal(object value)
        { return value; }

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
                { }
            }

            maxRow++;

            for (int i = rowFirst; i < maxRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 40;
            }

            return maxRow;
            
        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {
            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);
            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 30 * 40;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
            }
            sheet.Columns[0].Width = 400 * 36;
            sheet.Rows[3].Height = 40 * 40;
            for (int i = 1; i < 20; i++)
            {
                sheet.Columns[i].Width = 90 * 36;
            }
        }

        #endregion
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report r = new Report();

            ISection e_ = r.AddSection();
            e_.PageSize = new PageSize(1000, 600);
            IText title = e_.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;
            title.AddContent(HeaderGlobal.Text);

            title = e_.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;
            title.AddContent(SubHeaderGlobal.Text);

            Grid.Bands[0].HeaderLayout.Clear();
            UltraGridExporter1.PdfExporter.Export(Grid, e_);
            ISection is_ = r.AddSection();
            is_.PageSize = new PageSize(950, 500);
            title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;

            Chart.Height = 400;

            DataBindChart();
            title.AddContent(HeaderChart.Text);
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveTo(imageStream, Infragistics.UltraChart.Shared.Styles.RenderingType.Image);
            Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);

            IImage ima = is_.AddImage(img);
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

            title = is_.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title.Alignment.Horizontal = Alignment.Center;
        }


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



        int[] PDFHeaderHeightsLevel = {  30 };


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


        private void ExportHeader(ITable Table)
        {
            CRHelper.SaveToErrorLog("1");
            ITableRow RootRow = Table.AddRow();

            ApplyHeader();
            ConfDisplayGrid();//SetDisplayGrid();
            int sumW = 0;

            ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();

            AddTableCell(SelectorCol, ".", 16, PDFGetLevelHeight(0, 1));

            sumW = 16;

            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsRootLevel(Header))
                {
                    sumW += CreateHierarhyHeader(Header, RootRow);
                }
            }
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



        private void PreProcessing(MarginCellExportingEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;
            e.ReportCell.Parent.Height = new FixedHeight(0);
        }

        #region Кривая н рабочая
        private void PdfExporter_HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            if (headerBackground != null)
            {
                return;
            }
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            e.ReportCell.Parent.Height = new FixedHeight(0);


            ITable Table = e.ReportCell.Parent.Parent;

            ITableRow row = Table.AddRow();

            AddTableCell(row, "", 268, 30);


            Double WidthSubject =
                Grid.Columns.FromKey("ValueSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("RangFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("RangRF").Width.Value * 0.75;

            //AddTableCell(row, ComboRegion.SelectedValue, WidthSubject, 10);

            Double WidthFO =
                Grid.Columns.FromKey("ValueFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthFO").Width.Value * 0.75;
            //AddTableCell(row, ComboRegion.SelectedNodeParent, WidthFO, 10);


            Double WidthRF =
                Grid.Columns.FromKey("ValueRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthRF").Width.Value * 0.75;
            //AddTableCell(row, "Российская федерация", WidthRF, 10);



            row = Table.AddRow();
            AddTableCell(row, "Показатель", 268, 30);
            for (int i = Grid.Columns.FromKey("SEP").Index + 1; i < Grid.Columns.Count; i++)
            {
                AddTableCell(row, Grid.Columns[i].Header.Caption, Grid.Columns[i].Width.Value * 0.75, 30);
            }
        }
        #endregion

        void setHederWidth(ITableCell tableCell, Double width)
        {
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
        }

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



            //cellText = cellText.Replace("&nbsp;", " ");
            //cellText = cellText.Replace("<br/>", Environment.NewLine);

            text.AddContent(cellText);

            return tableCell;
        }
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }

        #endregion

        protected void Chart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            
            if (BaseTable.Rows.Count >1)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                int xMin = (int)xAxis.MapMinimum;
                int xMax = (int)xAxis.MapMaximum; 
               
                if (urfoAverage!=0)
                {
                    


                    int fmY = (int)yAxis.Map(urfoAverage);
                    Line line = new Line();
                    line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Solid;
                    line.PE.Stroke = Color.Red;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(xMin, fmY); 
                    line.p2 = new Point(xMax, fmY);
                    e.SceneGraph.Add(line);

                    Text text = new Text();
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(xMin - 46, fmY, 780, 15);
                    text.SetTextString("Ханты-Мансийский автономный округ - "+String.Format("{0:0.##}",urfoAverage));
                    e.SceneGraph.Add(text);
                }
            }
        }



        #endregion

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
            }
        }

        protected void calculateRank(UltraWebGrid Grid, int colNumber)
        {
                string style = "";
               if (IsSmallResolution)
               { style = "background-repeat: no-repeat;background-position: 15px"; }
               else
               { style = "background-repeat: no-repeat;background-position: 50px"; }
            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                {
                    m += 1;
                }
            }

            if (m != 0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                        Grid.Rows[i].Cells[Grid.Columns.IndexOf("Rang")].Text = "0";
                    }
                    else
                    {
                        Grid.Rows[i].Cells[Grid.Columns.IndexOf("Rang")].Text = String.Empty;
                    }

                }
                Array.Sort(rank);
                
                    if (pokType != "1")
                    {
                        m = 1;
                    }
                    else
                    {
                        m = rank.Length;
                    }

                
                for (int i = rank.Length - 1; i >= 0; i--)
                {

                    for (int j = 0; j < Grid.Rows.Count; j++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j].Cells[colNumber].Text))
                        {
                            if (Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Text == "0")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Text = String.Format("{0:0}", m);
                                if ((m) == 1)
                                {
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.CustomRules = style;
                                }
                                else
                                {
                                    if (m == rank.Length)
                                    {

                                        Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.BackgroundImage = "~/images/starGrayBB.png";
                                        Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.CustomRules = style;
                                    }
                                }
                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            if (pokType != "1")
                            {
                                m += 1;
                            }
                            else
                            { m -= 1; }
                        }
                    }
                    else
                    {
                        if (pokType != "1")
                        {
                            m += 1;
                        }
                        else
                        { m -= 1; }
                    }

                }

                CRHelper.SaveToUserAgentLog(m.ToString());

                if (m!=2)
                {

                double max = GetNumber(Grid.Rows[0].Cells[colNumber].Text);
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) < max)
                        {
                            max = GetNumber(Grid.Rows[j].Cells[colNumber].Text);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.CustomRules = style;
                            if (pokType != "1")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.BackgroundImage = "~/images/starGrayBB.png";
                            }
                            else
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Rang")].Style.BackgroundImage = "~/images/starYellowBB.png";
                            }
                            
                        }
                    }
                }
            }
            }
            
        }

        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
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
