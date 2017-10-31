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
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport; 
using Infragistics.WebUI.UltraWebChart;

using System.Globalization;

using Infragistics.Documents.Reports.Report.Text;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.OMCY_0003.REGION_01_YANAO
{
    public partial class _default : CustomReportPage
    {
        bool TwoYear = true;

        string SelectCurrentYear;
        string SelectPrevYear;

        IDataSetCombo DataSetYearCombo;

        IDataSetCombo DataSetScopeCombo;

        IDataSetCombo DataSetRegionCombo;

        CustomParam Scope { get { return (UserParams.CustomParam("Scope")); } }

        CustomParam Field { get { return (UserParams.CustomParam("Field")); } }

        CustomParam FilterGrid { get { return (UserParams.CustomParam("FilterGrid")); } }

        CustomParam FilterChart2 { get { return (UserParams.CustomParam("FilterChart2")); } }

        CustomParam SelectRegion { get { return (UserParams.CustomParam("SelectRegion")); } }

        CustomParam SelectYearCaption { get { return (UserParams.CustomParam("SelectYearCaption")); } }

        CustomParam SelectYearCaptionPrev { get { return (UserParams.CustomParam("SelectYearCaptionPrev")); } }


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
                ////String.Format(Width.

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
                Grid.Columns[0].Width = onePercent * 5;
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
                Grid.Columns[0].Width = onePercent * 5;
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
                Grid.Columns[0].Width = onePercent * 5;
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
                    
                    col.Width = onePercent * (100 - 63 - 5 -10) / (Grid.Columns.Count - 3);
                    

                }
                Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 60;
                Grid.Columns[0].Width = onePercent * 5;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 65 - 5 -10) / (Grid.Columns.Count - 3);
                }
                Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 60;
                Grid.Columns[0].Width = onePercent * 5;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 67 - 5-10) / (Grid.Columns.Count - 3);
                }
                Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 67;
                Grid.Columns[0].Width = onePercent * 5;
            }

            #endregion

        }
        #endregion
        
        DataTable GetDT(string QID, string FirstColumn)
        {
            DataTable Table = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(
                DataProvider.GetQueryText(QID),
                FirstColumn, Table);
            return Table;
        }

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
                string Key = FormatingText(Parent, BaseKey);

                while (DataForCombo.ContainsKey(Key))
                {
                    Key += " ";
                }

                return Key;
            }

            protected void AddItem(string Child, string Parent, string UniqueName)
            {
                string RealChild = Child;

                DataForCombo.Add(RealChild, 1);

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
                    string UniqueName = row["UniqueNameYear"].ToString();
                    string Child = row["Year"].ToString();

                    this.DataForCombo.Add(Child, 0);

                    this.DataUniqeName.Add(Child, UniqueName);

                    this.addOtherInfo(Table, row, Child);
                }
            }
        }

        class DataSetComboLinearXml : IDataSetCombo
        {
            public override void LoadData(DataTable T)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(T.TableName + "\\" + "\\reports\\REGION_0010\\UnEffectLess\\table.xml");
                XmlNode root = xmlDoc.ChildNodes[1];
                DataTable Table = new DataTable();
                Table.Columns.Add("Name");
                Table.Columns.Add("UniqeName");
                Table.Columns.Add("StringToQuery");

                foreach (XmlNode node in root.ChildNodes)
                {
                    DataRow Row = Table.NewRow();
                    Row["Name"] = node.ChildNodes[0].InnerText;
                    Row["UniqeName"] = node.ChildNodes[1].InnerText;
                    Row["StringToQuery"] = node.ChildNodes[2].InnerText;

                    Table.Rows.Add(Row);

                    this.DataForCombo.Add(Row["Name"].ToString(), 0);
                    this.addOtherInfo(Table, Row, Row["Name"].ToString());
                }
            }
        }

        private void InitCustomizerSize()
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
            {
                CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution.Default)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.PdfExportButton.Visible = false;

            InitCustomizerSize();

            G.Width = CustomizerSize.GetGridWidth();
            G.Height = Unit.Empty;

            C.Width = CustomizerSize.GetChartWidth(); 
        }

        private void FillCombo()
        {
            DataSetYearCombo = new DataSetComboLinear();
            DataSetYearCombo.LoadData(GetDT("AllYear", "Year"));
            
            DataTable Region = GetDT("RegionAll", "Year");
            //Region.Rows[0].Delete();

            DataSetRegionCombo = new DataSetComboLinear();
            DataSetRegionCombo.LoadData(Region);


            if (!Page.IsPostBack)
            {
                YearCombo.FillDictionaryValues(DataSetYearCombo.DataForCombo);
                YearCombo.Title = "Период";
                YearCombo.Width = 150;

                RegionCombo.Title = "Территория";
                RegionCombo.FillDictionaryValues(DataSetRegionCombo.DataForCombo);


                DataTable dt = new DataTable();
            }
        }

        private void ChoseParam()
        {
            SelectCurrentYear = YearCombo.SelectedValue;
            SelectPrevYear = (int.Parse(YearCombo.SelectedValue) - 1).ToString();

            SelectYearCaption.Value = YearCombo.SelectedValue;

            SelectYearCaptionPrev.Value = SelectPrevYear;

            SelectRegion.Value = RegionCombo.SelectedValue;


        }

        struct Region_Value
        {
            public string region;
            public System.Decimal Value;
        }

        class NumberOfstruct
        {
            public int a;
            public int b;
            public int c;

            public string ToStrSortFormat()
            {
                return string.Format("{0:#0}.{1:#0}.{2:00}", a, b, c);
            }

            public string ToStrDisplayFormat()
            {
                if (a <= 0)
                {
                    return "";
                }

                if (ParentB)
                {
                    return string.Format("{0:#0}", a);
                }

                if (ParentC)
                {
                    return string.Format("{0:#0}.{1:#0}", a, b);
                }
                
                return string.Format("{0:#0}.{1:#0}.{2:#0}", a, b, c);
            }

            public bool ParentC {
                get
                {
                    return c<=0;
                }                
            }
            public bool ParentB
            {
                get
                {
                    return b <= 0;
                }                
            }

            public NumberOfstruct(string str)
            {
                string[] sp = str.Split('.');
                if (sp.Length > 1)
                {

                    a = int.Parse(sp[0]);
                    b = int.Parse(sp[1]);
                    c = int.Parse(sp[2]);
                    return;
                }
                else
                {
                    if (str.Length>1)
                    {
                        a = int.Parse(str[0]+"");
                        b = int.Parse(str[1]+""+str[2]);
                        c = 0;
                    }
                    else
                    {
                        a = 0;
                        b = 0;
                        c = 0;  
                    }
                }
                              
            }
        }

        #region DataBindGrid
        string CalculatedRangRow(DataRow dataRow, int StartCol, string FindMainColCaption,bool revece)
        {

            int Loop = 1;

            Region_Value MainRegion = new Region_Value();

            List<Region_Value> RVal = new List<Region_Value>();

            for (int i = StartCol; i < dataRow.Table.Columns.Count; i += Loop)
            {

                if (dataRow[i] != DBNull.Value)
                {

                    Region_Value rv = new Region_Value();

                    rv.region = dataRow.Table.Columns[i].Caption;

                    rv.Value = (System.Decimal)dataRow[i];

                    if (rv.region == FindMainColCaption)
                    {
                        MainRegion = rv;
                    }

                    if ((dataRow[1].ToString()[0] == '2') && (dataRow[1].ToString()[1] == '.') && (rv.region == "г.Ханты-Мансийск"))
                    {

                    }
                    else
                    {
                        RVal.Add(rv);
                    }
                    
                }
            }

            if (revece)
            {
                RVal.Sort(new Comparer_());
            }
            else            
            {
                RVal.Sort(new Comparer_reverce());
            }


            decimal value = RVal[RVal.IndexOf(MainRegion)].Value;

            int MinRang = int.MaxValue;
            int MaxRang = int.MinValue;

            for (int i = 0; i < RVal.Count; i++)
            {
                if (value == RVal[i].Value)
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
                return MinRang.ToString() + "-" + MaxRang.ToString();
            }
            else
            {
                return MinRang.ToString();
            }



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

        private void CalculatedRang(DataTable Table)
        {
            Table.Columns.Add("RangCurYear", typeof(Decimal));
            Table.Columns.Add("RangPrevYear", typeof(Decimal));

            foreach (DataRow row in Table.Rows)
            {
                //row["RangCurYear"] = CalculatedRangRow(row, 6 + 2, SelectRegion.Value + "; " + "PrevPeriiod");
                //row["RangPrevYear"] = CalculatedRangRow(row, 5 + 2, SelectRegion.Value + "; " + "CurrentPeriiod");
            }
        }

        DataTable Table = null;


        DataRow FindRow(DataTable Table, string FindString)
        {
            foreach (DataRow Row in Table.Rows)
            {
                if (Row[1].ToString() == FindString)
                {
                    return Row;
                }
            }
            DataRow NewRow = Table.NewRow();
            NewRow[1] = FindString;
            Table.Rows.Add(NewRow);
            return NewRow;
        }

        private void DataBindGrid(UltraWebGrid Grid, string SqlID)
        {
            Grid.Bands.Clear();
            Table = GetDT(SqlID, "Field");

            DataTable RootTable = GetDT("RootGrid", "Field");

            //RootGrid

            foreach (DataRow row in RootTable.Rows)
            {
                foreach (DataColumn col in RootTable.Columns)
                {
                    DataRow Row = FindRow(Table, row[1].ToString());
                    if (col.ColumnName == "Code")
                    {
                        Row[col.ColumnName] = row[col].ToString();
                    }
                    else
                    {
                        Row[col.ColumnName] = row[col];
                    }
                }
            }


            int ColCurYear = Table.Columns.IndexOf(SelectRegion.Value);

            DataTable TableGrid = new DataTable();
            
            TableGrid.Columns.Add("NumberOfStruct", typeof(string));            

            TableGrid.Columns.Add("Field", typeof(string));

            TableGrid.Columns.Add("Unit", typeof(string));            
            TableGrid.Columns.Add("Value", typeof(decimal));
            TableGrid.Columns.Add("Rang", typeof(string));
            

            foreach (DataRow row in Table.Rows)
            {
                DataRow Row = TableGrid.NewRow();

                Row["Field"] = row["Field"];

                NumberOfstruct ns = new NumberOfstruct(row[1].ToString());

                Row["NumberOfStruct"] = ns.ToStrSortFormat();//string.Format("{0:#0}.{1:#0}.{2:00}",ns.a,ns.b,ns.c);
                Row["Unit"] = row[2] == DBNull.Value ? "Коэффициент" : row[2].ToString();

                Row["Value"] = row[ColCurYear];

                bool reverce = row["revece"].ToString() == "0";
                try
                {
                    Row["Rang"] = CalculatedRangRow(row, 4, SelectRegion.Value,reverce);
                }
                catch { }

                TableGrid.Rows.Add(Row);
            }

            DataRow[] drs = TableGrid.Select("true", "NumberOfStruct");
            foreach (DataRow dr in drs)
            {
                TableGrid.Rows.Add(dr.ItemArray);
                TableGrid.Rows.Remove(dr);
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }
        #endregion

        #region DisplayGrid
        private void SetStyleHeaderCol(ColumnHeader columnHeader)
        {
            columnHeader.Style.HorizontalAlign = HorizontalAlign.Center;
            columnHeader.Style.Wrap = true;
        }

        private void SetCellStylle(UltraWebGrid Grid)
        {
            ColumnsCollection cols = Grid.Columns;
            cols[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            cols[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            cols[1].CellStyle.Wrap = true;

            cols[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            cols[2].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(cols[3], "### ### ### ##0.###");
            CRHelper.FormatNumberColumn(cols[4], "N0");

            
        }

        private void SetHeader(UltraWebGrid G)
        {
            G.Columns[0].Header.Caption = "№ п/п";
            G.Columns[1].Header.Caption = "Показатели";
            G.Columns[2].Header.Caption = "Единица измерения";
            G.Columns[3].Header.Caption = "Значение";
            G.Columns[4].Header.Caption = "Место";
           

            foreach (UltraGridColumn col in G.Columns)
            {
                SetStyleHeaderCol(col.Header);
            }

        }

        private void SetStar(UltraWebGrid G)
        {
            
            
        }

        private void ConfigurateGrid(UltraWebGrid Grid)
        {
            G.Rows[0].Delete();
            //G.Rows[1].Delete();

            SetHeader(Grid);
            CustomizerSize.ContfigurationGrid(Grid);
            SetCellStylle(Grid);

            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;


            SetStar(Grid);

            SetHierarhyRow(Grid);
        }

        string RemoveDataMembercaption(string str)
        {
            if (str.Contains("ДАННЫЕ)"))
            {
                return str.Remove(0, 1).Replace("ДАННЫЕ)", "");
            }
            return str;
        }
        

        void SetHierarhyRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow row in Grid.Rows)
            { 
                NumberOfstruct NOS = new NumberOfstruct(row.Cells.FromKey("NumberOfStruct").Text);

                if ((NOS.a == 2) && (RegionCombo.SelectedValue == "г.Ханты-Мансийск") && (NOS.b >0))
                {
                    row.Cells.FromKey("Rang").Text = "";
                }

                row.Cells[1].Text = RemoveDataMembercaption(row.Cells[1].Text);

                if (!CheckBox1.Checked)
                {
                    if (!NOS.ParentB)
                    {
                        row.Hidden = true;
                    }
                }

                row.Cells.FromKey("NumberOfStruct").Text = NOS.ToStrDisplayFormat();
                if (NOS.ParentC)
                {
                    row.Style.Font.Bold = true;
                }

                //
                if ((row.Cells.FromKey("Rang").Text == "1-22"))
                {
                    row.Cells.FromKey("Rang").Title = "Наилучшее значение по субъекту РФ";
                    row.Cells.FromKey("Rang").Style.BackgroundImage = "~/images/starYellowBB.png";
                    row.Cells.FromKey("Rang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                else
                {
                    try
                    {

                        if ((row.Cells.FromKey("Rang").Text == "1") || (row.Cells.FromKey("Rang").Text.Contains("1-")))
                        {
                            row.Cells.FromKey("Rang").Title = "Наилучшее значение по субъекту РФ";
                            row.Cells.FromKey("Rang").Style.BackgroundImage = "~/images/starYellowBB.png";
                            row.Cells.FromKey("Rang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                        if ((row.Cells.FromKey("Rang").Text == "22") || row.Cells.FromKey("Rang").Text.Contains("-22"))
                        {
                            row.Cells.FromKey("Rang").Title = "Наихудшее значение по субъекту РФ";
                            row.Cells.FromKey("Rang").Style.BackgroundImage = "~/images/starGrayBB.png";
                            row.Cells.FromKey("Rang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                    catch { }
                }
            }
        }

        #endregion


        string SetBr(string s, int limit)
        {
            for (int i = 0; i < s.Length; i += limit)
            {
                for (int j = i; (j < i + limit) & (j < s.Length); j++)
                {
                    if (s[j] == ' ')
                    {
                        i = j ; 
                         s = s.Insert(j, '\n'+"");
                        break;
                    }
                }

            }
            return s;
        }


        string GenshortName(string name)
        {
            return name.Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", "");
        }

        private void DataBindChart()
        {
            #region Conf

            C.ChartType = ChartType.ColumnChart;
            C.ColumnChart.SeriesSpacing = 0;
            C.ColumnChart.ColumnSpacing = 0;

            C.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:#0.00>";

            C.Legend.Visible = false;

            C.Axis.X.Extent = 120;
            C.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            C.Axis.X.Margin.Far.Value = 0;
            C.Axis.X.Margin.Near.Value = 0;

            C.Axis.X.Labels.SeriesLabels.Visible = false;

            C.Tooltips.FormatString = "<ITEM_LABEL><br> <b><DATA_VALUE:#0.000></b>";

            #endregion

            #region DataBind
               
            DataTable Table1 = GetDT("C", "Field");
            
            DataTable TableChart = new DataTable();
            TableChart.Columns.Add("Field");
            TableChart.Columns.Add("Value", typeof(System.Decimal));            

            int ColVal = Table1.Columns.IndexOf(SelectRegion.Value);            

            for (int i = 0; i < 5; i++)
            {
                TableChart.Rows.Add(new Object[2] { SetBr(Table1.Rows[i][0].ToString(), 12), Table1.Rows[i][ColVal] });
            }
            C.DataSource = TableChart;
            C.DataBind();
            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChoseParam();

            DataBindGrid(G, "G");

            ConfigurateGrid(G);

            DataBindChart();

            SetHeaderReport();


        }

        


       

        private void SetHeaderReport()
        {

            Hederglobal.Text = "Оценка эффективности деятельности органов местного самоуправления (по муниципальным образованиям)";
            Page.Title = Hederglobal.Text;
            Label2.Text = "Оценка проведена в соответствии с распоряжением Правительства Российской Федерации от 11 сентября 2008 г. № 1313-р, в редакции  распоряжения Правительства Российской Федерации от 15 мая 2010 г. № 758-р (с учетом изменений) по выбранному муниципальному образованию";

            Label4.Text = string.Format("Оценка эффективности деятельности органов местного самоуправления в {0} году, {1} ",YearCombo.SelectedValue,RegionCombo.SelectedValue);
        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0.###";
            }

            FormatGridSheet(e.CurrentWorksheet);

            try
            {
                e.Workbook.Worksheets["Диаграмма 1"].Rows[0].Cells[0].Value = Label4.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0], C);
            }
            catch { }
        }

        private void FormatGridSheet(Worksheet w)
        {
            w.Rows[3].Cells[0].Value = "Показатели";

            w.Columns[1].Width = 700 * 40;
            w.Columns[0].Width = 100 * 40;
            w.Columns[2].Width = 100 * 40;
            w.Columns[3].Width = 100 * 40;
            w.Columns[4].Width = 40 * 40;

            SetStyleHeadertableFromExcel(w.Rows[3].Cells[0].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[1].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[2].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[3].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[4].CellFormat);

            w.Rows[3].Cells[0].Value = G.Columns[0].Header.Caption;
            w.Rows[3].Cells[1].Value = G.Columns[1].Header.Caption;
            w.Rows[3].Cells[2].Value = G.Columns[2].Header.Caption;
            w.Rows[3].Cells[3].Value = G.Columns[3].Header.Caption;
            w.Rows[3].Cells[4].Value = G.Columns[4].Header.Caption;
            int startrow = 5;

            for (int i = 0; i < G.Rows.Count; i++)
            {
                //sheet.Rows[i + startrow].Height = 22 * 40;
                w.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                w.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                for (int j = 0; j < 10; j++)
                    try
                    {

                        w.Rows[i + startrow].Cells[j].Value = w.Rows[i + startrow].Cells[j].Value.ToString() + "";
                        double Value = double.Parse(w.Rows[i + startrow].Cells[j].Value.ToString().Replace(',', '.'));

                        w.Rows[i + startrow].Cells[j].CellFormat.FormatString = "########0.#######";

                        //sheet.Rows[i + startrow].Cells[j].Value = string.Format("{0:########0.########}", Value);
                        w.Rows[i + startrow].Cells[j].Value = Value;
                    }
                    catch { }

            }
        }



        private void SetSizeMapAndChart()
        {
            C.Width = (int)(C.Width.Value * 0.75);

            //DundasMap.Width = (int)(C.Width.Value * 0.85);
            //DundasMap.Height = (int)(DundasMap.Height.Value * 0.75);
        }

        private void CleraHidenColGrid()
        {
            foreach (UltraGridColumn col in G.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    G.Bands[0].HeaderLayout.Remove(col.Header);
                    G.Columns.Remove(col);
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");

            //Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

            SetSizeMapAndChart();

            CleraHidenColGrid();

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 5;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";


            ReportExcelExporter1.ExcelExporter.Export(G, sheet1);
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
                {
                    //TODO ≡ατεß≡α≥±  ± δσΓ√∞Φ ⌡σΣσ≡α∞Φ или так Ω±∩ε≡ ≡ΦΣα
                }
            }

            maxRow++;

            for (int i = rowFirst; i < maxRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 60;
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
                    sheet.Columns[i].Width = 0;
                }
                else
                {
                    sheet.Columns[i].Width = (int)G.Columns[i].Width.Value * 36;
                }
                sheet.Columns[i].CellFormat.FormatString = G.Columns[i].Format;
            }
        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {
            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);

            for (int i = 0; i < G.Rows.Count; i++)
            {
                //sheet.Rows[i + startrow].Height = 32 * 40;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                //sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
            }
            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 12 * 40;
            }

            sheet.Columns[0].Width = 200 * 36;

            for (int i = 1; i < 14; i++)
            {
                sheet.Columns[i].Width = 80 * 36;
            }

           
        }

        #endregion

        #endregion
    }
}
