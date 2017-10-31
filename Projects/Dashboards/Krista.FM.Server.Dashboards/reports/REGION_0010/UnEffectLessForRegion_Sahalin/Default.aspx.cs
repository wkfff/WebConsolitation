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

namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessForRegion_Sahalin 
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

        CustomParam GO { get { return (UserParams.CustomParam("GO")); } }
        

        ICustomizerSize CustomizerSize;

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;
            
            public abstract int ChartHeight();            

            public abstract int ParamRegionWidth();
            public abstract int ParamScopeWidth();

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
                        return 740-50;
                    case BrouseName.FireFox:
                        return 750 - 50;
                    case BrouseName.SafariOrHrome:
                        return 750 - 50;
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
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);

                }
                Grid.Columns[0].Width = onePercent * 43;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);
                }
                Grid.Columns[0].Width = onePercent * 45;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);
                }
                Grid.Columns[0].Width = onePercent * 47;
            }

            #endregion


            public override int ParamRegionWidth()
            {
                return 150;
            }

            public override int ParamScopeWidth()
            {
                return 165;
            }

            public override int ChartHeight()
            {
                return 330;
            }
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
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);

                }
                Grid.Columns[0].Width = onePercent * 43;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);
                }
                Grid.Columns[0].Width = onePercent * 45;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Index % 2 != 0)
                        col.Width = onePercent * (100 - 67) / (Grid.Columns.Count - 2 - 2);
                    else
                        col.Width = onePercent * (100 - 27) / (Grid.Columns.Count - 2 - 2);
                }
                Grid.Columns[0].Width = onePercent * 47;
            }

            #endregion

            public override int ParamRegionWidth()
            {
                return 150*2;
            }

            public override int ParamScopeWidth()
            {
                return 165*2;
            }

            public override int ChartHeight()
            {
                return 370;
            }
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

                    if (Child[0] != 'г')
                    {
                        try
                        {
                            int a = int.Parse(Child);
                            Child = a.ToString();
                        }
                        catch 
                        {
                            //Child += " район";
                        }
                    }

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
                //xmlDoc.Load(dir + "\\reports\\OMCY_ACR_0003\\table.xml");
                //Server.MapPath("~")
                xmlDoc.Load(T.TableName + "\\" + "\\reports\\REGION_0010\\UnEffectLess_Sahalin\\table.xml");
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

                    //for (int i = 3; i < node.ChildNodes.Count; i++)
                    //{
                    //    Row["StringToQuery"] = Row["StringToQuery"].ToString() + "," + node.ChildNodes[i].InnerText;
                    //}
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


            C.Width = CustomizerSize.GetChartWidth()/2;
            C.Height = CustomizerSize.ChartHeight();

            C0.Width = CustomizerSize.GetChartWidth()/2;
            C0.Height = CustomizerSize.ChartHeight();

            G_1.Width = CustomizerSize.GetGridWidth();
            G_2.Width = CustomizerSize.GetGridWidth();
            G_3.Width = CustomizerSize.GetGridWidth();

            G_1.Height = Unit.Empty;
            G_2.Height = Unit.Empty;
            G_3.Height = Unit.Empty;

            RegionCombo.Width = CustomizerSize.ParamRegionWidth();
            ScopeCombo.Width = CustomizerSize.ParamScopeWidth();

            //DundasMap.Width = CustomizerSize.GetChartWidth();
            //DundasMap.Height = 700;


        }

        private void FillCombo()
        {
            DataSetYearCombo = new DataSetComboLinear();
            DataSetYearCombo.LoadData(GetDT("AllYear", "Year"));            

            DataTable Region = GetDT("RegionAll", "Year");
            //Region.Rows[0].Delete();

            DataSetRegionCombo = new DataSetComboLinear();
            DataSetRegionCombo.LoadData(Region);            

            DataSetScopeCombo = new DataSetComboLinearXml();
            DataSetScopeCombo.LoadData(new DataTable(Server.MapPath("~")));

            if (!Page.IsPostBack)
            {
                YearCombo.FillDictionaryValues(DataSetYearCombo.DataForCombo);
                YearCombo.Title = "Период";
                YearCombo.Width = 150;

                ScopeCombo.FillDictionaryValues(DataSetScopeCombo.DataForCombo);
                ScopeCombo.Title = "Сфера";

                RegionCombo.Title = "Территория";
                RegionCombo.FillDictionaryValues(DataSetRegionCombo.DataForCombo);
            } 
        }

        private void ChoseParam()
        {
            Scope.Value = DataSetScopeCombo.OtherInfo[ScopeCombo.SelectedValue]["UniqeName"];

            FilterGrid.Value = DataSetScopeCombo.OtherInfo[ScopeCombo.SelectedValue]["StringToQuery"];

            SelectCurrentYear = YearCombo.SelectedValue;
            SelectPrevYear = (int.Parse(YearCombo.SelectedValue) - 1).ToString();

            SelectYearCaption.Value = YearCombo.SelectedValue;

            SelectYearCaptionPrev.Value = SelectPrevYear;

            SelectRegion.Value = RegionCombo.SelectedValue;//.Replace(" район","");

            if (SelectRegion.Value.Contains("район"))
            {
                GO.Value = "Муниципальные районы";                
            }
            else
            {
                GO.Value = "Городские округа";
            }
            
        }

        struct Region_Value
        {
            public string region;
            public System.Decimal Value;
        }

        #region DataBindGrid
        string CalculatedRangRow(DataRow dataRow, int StartCol, string FindMainColCaption)
        {
            int Loop = 2;

            Region_Value MainRegion = new Region_Value();

            List<Region_Value> RVal = new List<Region_Value>();

            for (int i = StartCol; i < dataRow.Table.Columns.Count - 2; i += Loop)
            {

                if (dataRow[i] != DBNull.Value) 
                {

                    Region_Value rv = new Region_Value();

                    rv.region = dataRow.Table.Columns[i].Caption;

                    rv.Value = (System.Decimal)dataRow[i];

                    String.Format(rv.region + "|" + FindMainColCaption);
                    if (rv.region == FindMainColCaption)
                    {
                        MainRegion = rv;
                    }
                    
                    RVal.Add(rv);
                }
            }

            RVal.Sort(new Comparer_());


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
                return MinRang.ToString() +"-"+MaxRang.ToString();
            }
            else 
            {
                return MinRang.ToString();
            }


            
        }

        class Comparer_ : Comparer<Region_Value>
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

        private void CalculatedRang(DataTable Table)
        {
            Table.Columns.Add("RangCurYear", typeof(string));
            Table.Columns.Add("RangPrevYear", typeof(string));

            foreach (DataRow row in Table.Rows)
            {
                String.Format(SelectRegion.Value + "; " + "CurrentPeriiod");

                try
                {
                    row["RangCurYear"] = CalculatedRangRow(row, 5 + 2 + 2, SelectRegion.Value + "; " + "CurrentPeriiod").ToString();
                    row["RangPrevYear"] = CalculatedRangRow(row, 6 + 2 + 2, SelectRegion.Value + "; " + "PrevPeriiod").ToString();
                }
                catch { }
            }
        }

        private void DataBindGrid(UltraWebGrid Grid,string SqlID)
        {
            Grid.Bands.Clear();
            DataTable Table = GetDT(SqlID, "Field");

            //(RegionCombo.SelectedValue == "г.Ханты-Мансийск") & (ScopeCombo.SelectedValue == "Здравоохранение");
            //if (ScopeCombo.SelectedValue == "Здравоохранение")
            //{
            //    if (RegionCombo.SelectedValue != "г.Ханты-Мансийск")
            //    {
            //        Table.Columns.Remove("г.Ханты-Мансийск" + "; " + "CurrentPeriiod");
            //        Table.Columns.Remove("г.Ханты-Мансийск" + "; " + "PrevPeriiod");
            //    } 
            //}

            CalculatedRang(Table);

            int ColCurYear = Table.Columns.IndexOf(SelectRegion.Value + "; " + "CurrentPeriiod");
            int ColPreYear = Table.Columns.IndexOf(SelectRegion.Value + "; " + "PrevPeriiod");


            DataTable TableGrid = new DataTable();

            TableGrid.Columns.Add("Field", typeof(string));
            TableGrid.Columns.Add("NumberOfStruct", typeof(string));
            TableGrid.Columns.Add("reverce", typeof(string));

            TableGrid.Columns.Add("CountZero", typeof(string));

            TableGrid.Columns.Add("PrevValue", typeof(decimal));
            TableGrid.Columns.Add("PrevRang", typeof(string ));
            TableGrid.Columns.Add("CurrValue", typeof(decimal));
            TableGrid.Columns.Add("CurrRang", typeof(string));

            foreach (DataRow row in Table.Rows)
            {
                DataRow Row = TableGrid.NewRow();

                Row["Field"] = ((row[0].ToString()[0] == '(') ? row[0].ToString().Remove(0, 1).Replace("ДАННЫЕ)", "") : row[0]).ToString().Replace("из них:", "")
                    +", "+row[5].ToString().ToLower();///reverce

                Row["CountZero"] = row[7];

                Row["NumberOfStruct"] = row[3];
                Row["reverce"] = row[1];

                Row["PrevValue"] = row[ColPreYear];
                Row["PrevRang"] = row["RangPrevYear"];

                Row["CurrValue"] = row[ColCurYear];
                Row["CurrRang"] = row["RangCurYear"];

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

        }

        private void SetCellStylle(UltraWebGrid Grid)
        {
            ColumnsCollection cols = Grid.Columns;
            cols[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            cols[0].CellStyle.Wrap = true;

            cols[3+1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            cols[3 + 1].CellStyle.Padding.Right = 3;

            cols[4 + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            cols[4 + 1].CellStyle.Padding.Right = 3;

            cols[5 + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            cols[5 + 1].CellStyle.Padding.Right = 3;

            cols[6 + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            cols[6 + 1].CellStyle.Padding.Right = 3;

            CRHelper.FormatNumberColumn(cols[3 + 1], "### ### ##0.##");
            cols[3 + 1].DataType = "string";
            CRHelper.FormatNumberColumn(cols[5 + 1], "### ### ##0.##");
            cols[5 + 1].DataType = "string";
        }

        private void SetHeader(UltraWebGrid G)
        {
            G.Columns[0].Header.Caption = "Показатели";
            G.Columns[1].Hidden = true;
            G.Columns[2].Hidden = true;
            G.Columns[3].Hidden = true;

            G.Columns[3+1].Header.Caption = SelectPrevYear;//"2009<br> Значение предыдущего года";
            G.Columns[3 + 1].Header.Style.Wrap = true;

            G.Columns[4 + 1].Header.Caption = "Ранг";
            G.Columns[4 + 1].Header.Style.Wrap = true;

            G.Columns[5 + 1].Header.Caption = SelectCurrentYear;//"2010<br> Значение отчетного года";
            G.Columns[5 + 1].Header.Style.Wrap = true;

            G.Columns[6 + 1].Header.Caption = "Ранг";
            G.Columns[6 + 1].Header.Style.Wrap = true;

            foreach (UltraGridColumn col in G.Columns)
            {
                SetStyleHeaderCol(col.Header);
            }

        }

        private void ConfigurateGrid(UltraWebGrid Grid)
        {
            SetHeader(Grid);
            CustomizerSize.ContfigurationGrid(Grid);
            SetCellStylle(Grid);


            SetImageForRang(Grid);

            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        private void SetImageForRang(UltraWebGrid Grid)
        {

            //return;

            foreach (UltraGridRow row in Grid.Rows)
            {
                try
                {
                    if ((row.Cells[0].Text.Contains("Норматив") || row.Cells[0].Text.Contains("Целевое значение") || ((RegionCombo.SelectedValue == "г.Ханты-Мансийск") & (ScopeCombo.SelectedValue == "Здравоохранение"))))//целевое значение
                    {
                        row.Cells.FromKey("CurrRang").Text = "-";
                        row.Cells.FromKey("PrevRang").Text = "-";
                        continue;
                    }

                    if ((row.Cells.FromKey("CurrRang").Text == "1") || (row.Cells.FromKey("CurrRang").Text.Contains("1-")))
                    {
                        row.Cells.FromKey("CurrRang").Title = "Hаилучшее значение по субъекту РФ";
                        row.Cells.FromKey("CurrRang").Style.BackgroundImage = "~/images/starYellowBB.png";
                        row.Cells.FromKey("CurrRang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    else
                        if ((row.Cells.FromKey("CurrRang").Text == "22".ToString()) || (row.Cells.FromKey("CurrRang").Text.Contains("-22")))
                        {
                            row.Cells.FromKey("CurrRang").Title = "Наихудшее значение по субъекту РФ";
                            row.Cells.FromKey("CurrRang").Style.BackgroundImage = "~/images/starGrayBB.png";
                            row.Cells.FromKey("CurrRang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }

                    if ((row.Cells.FromKey("PrevRang").Text == "1") || (row.Cells.FromKey("PrevRang").Text.Contains("1-")))
                    {
                        row.Cells.FromKey("PrevRang").Title = "Hаилучшее значение по субъекту РФ";
                        row.Cells.FromKey("PrevRang").Style.BackgroundImage = "~/images/starYellowBB.png";
                        row.Cells.FromKey("PrevRang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    else
                        if ((row.Cells.FromKey("PrevRang").Text == "22".ToString()) || (row.Cells.FromKey("PrevRang").Text.Contains("-22")))
                        {
                            row.Cells.FromKey("PrevRang").Title = "Наихудшее значение по субъекту РФ";
                            row.Cells.FromKey("PrevRang").Style.BackgroundImage = "~/images/starGrayBB.png";
                            row.Cells.FromKey("PrevRang").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                }
                catch { }
            }
        }

        void SetHierarhyRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                int LevelRow = row.Cells[1].Text.Split('.').Length;

                if (LevelRow == 2)
                {
                    row.Cells[0].Style.Font.Bold = true;
                }
                else
                if (LevelRow == 3)
                {
                    row.Cells[0].Style.Padding.Left = 10;
                }
                else
                {
                    row.Cells[0].Style.Padding.Left = 20;
                }

                if (row.Cells.FromKey("CurrValue").Value == null)
                {
                    row.Hidden = true;
                }
                else
                {
                    string CaprionZero = ".";
                    int CountZero = int.Parse(row.Cells.FromKey("CountZero").Text);
                    for (int i = 0; i < CountZero; i++)
                    {
                        CaprionZero += "0";
                    }
                    if (CaprionZero == ".")
                    {
                        CaprionZero = "";
                    }
                    

                    row.Cells.FromKey("CurrValue").Value = string.Format("{0:### ### ### ##0" + CaprionZero + "}", row.Cells.FromKey("CurrValue").Value);
                    row.Cells.FromKey("PrevValue").Value = string.Format("{0:### ### ### ##0" + CaprionZero + "}", row.Cells.FromKey("PrevValue").Value);

                    
                    
                }
                
                

            }
        }

        void SetImageForRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow row in Grid.Rows)
            {

                try
                {
                    bool IsReverce = row.Cells[2].Text != "0";

                    if (row.Cells.FromKey("CurrValue").Text == row.Cells.FromKey("PrevValue").Text)
                        continue;

                    string UpOrDouwn = (System.Decimal.Parse(row.Cells.FromKey("CurrValue").Value.ToString()) > System.Decimal.Parse(row.Cells.FromKey("PrevValue").Value.ToString())) ? "Up" : "Down";

                    string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) || ((UpOrDouwn == "Down") && (IsReverce))) ? "Green" : "Red";

                    string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";

                    string ImagePath = "~/images/" + ImageName;

                    row.Cells.FromKey("CurrValue").Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
                    row.Cells.FromKey("CurrValue").Style.BackgroundImage = ImagePath;



                }
                catch { }
            }
        }

        #endregion        

        private void ConfChart1()
        {
            DataTable TableChart = GetDT("Chart1", "Column");
            if (TableChart.Rows.Count == 0)
                return;
            DataRow UnEfectRow = TableChart.Rows[0];

            UnEfectRow["Column"] = "Доля неэффективных расходов";

            C.DataSource = TableChart;

            C.Tooltips.FormatString = "<SERIES_LABEL><br><b><DATA_VALUE:### ### ### ##0.##></b> %";

            C.DataBind();

            decimal max = decimal.MinValue;
            for (int i = 0; i < TableChart.Rows.Count; i++)
            {
                for (int j = 1; j < TableChart.Columns.Count; j++)
                {
                    try
                    {
                        if (max < (decimal)TableChart.Rows[i][j])
                        {
                            max = (decimal)TableChart.Rows[i][j];
                        };
                    }
                    catch { }
                }
            }

            C.Axis.Y.RangeMax = (double)max + 0.5;
            C.Axis.Y.RangeMin = (double)0;
            C.Axis.Y.RangeType = AxisRangeType.Custom;



            C.ChartType = ChartType.LineChart;
            C.Data.SwapRowsAndColumns = false;

            C.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            C.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:#0.##>%";
        }        

        string GetChartData()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~") + "\\" + "\\reports\\REGION_0010\\UnEffectLessForRegion_Sahalin\\chart.xml");
            XmlNode root = xmlDoc.ChildNodes[1];
            string Scope = ScopeCombo.SelectedValue;

            XmlNode mainNode = null;

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.ChildNodes[0].InnerText == Scope)
                {
                    mainNode = n;
                    break;
                }
            }

            //FilterChart2.Value = 
            return mainNode.ChildNodes[1].InnerText;//mainNode.ChildNodes[2].InnerText.Split(';');
        }

        string SetBr(string s, int limit)
        {
            for (int i = limit; i < s.Length; i += limit)
            {
                for (int j = i; (j < i + limit) & (j < s.Length); j++)
                {
                    if (s[j] == ' ')
                    {
                        i = j;
                        s = s.Insert(j, '\n' + "");
                        break;
                    }
                }

            }
            return s;
        }

        private void ConfChart2()
        {
             FilterChart2.Value = GetChartData();
              
             
                 DataTable  tc =  GetDT("Chart2", "Column");

                 for (int i = 0; i < tc.Rows.Count; i++)
                 {
                     //tc.Rows[i][0] = SetBr(tc.Rows[i][0].ToString(), 55);
                     tc.Rows[i][0] = (tc.Rows[i][0].ToString());
                 }
            

            decimal max = decimal.MinValue;
            for(int i = 0;i<tc.Rows.Count;i++)
            {
                for (int j = 1; j < tc.Columns.Count; j++)
                {
                    try
                    {
                        if (max < (decimal)tc.Rows[i][j])
                        {
                            max = (decimal)tc.Rows[i][j];
                        };
                    }
                    catch { }
                }
            } 
            
            
             //C0.Axis.Y.RangeMax = (double)max+ 10;
             //C0.Axis.Y.RangeMin = (double)0;
             //C0.Axis.Y.RangeType = AxisRangeType.Custom;//RangeMin = (double)max;



             C0.DataSource = tc;

             C0.FillSceneGraph +=new FillSceneGraphEventHandler(C0_FillSceneGraph);

             C0.DataBind();

             C0.Axis.Y.Extent = 60;

             C0.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ##0>";

             C0.Tooltips.FormatString = "<SERIES_LABEL><br><b><DATA_VALUE:### ### ### ##0.##></b> тысяч рублей";

             C0.Data.SwapRowsAndColumns = true;

             if (ScopeCombo.SelectedValue == "Здравоохранение")
             {
                 C0.Legend.SpanPercentage = 35; 
             }

             if (ScopeCombo.SelectedValue == "Общее образование")
             {
                 C0.Legend.SpanPercentage = 26;
             }

             if (ScopeCombo.SelectedValue == "Жилищно-коммунальное хозяйство")
             {
                 C0.Legend.SpanPercentage = 26;
             }

             if (ScopeCombo.SelectedValue == "Организация муниципального управления")
             {
                 C0.Legend.SpanPercentage = 15;
             }
        }

        void C0_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            //Line l = new Line(new Point(80, 194), new Point(580, 194));
            //e.SceneGraph.Add(l);
            SceneGraph scene = e.SceneGraph;

            bool LegendMainBox = false;
            #region Легенда
            int Top = 0;

            for (int i = 0; i < scene.Count; i++)
            {
                Primitive p = scene[i];

                if (p.Path == "Border.Title.Legend")
                {
                    if (p is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        if (LegendMainBox)
                        {
                            Text CaptionLegend = (Text)scene[i + 1];
                            CaptionLegend.bounds.Y += Top;

                            CaptionLegend.SetTextString(SetBr(CaptionLegend.GetTextString(), 53));
                            Box b = (Box)p;
                            b.rect.Y += Top;
                            b.rect.Height = b.rect.Width;

                            Top += 15;
                        }
                        LegendMainBox = true;
                    }
                }
            }
            #endregion
        }

        private void ConfigurateChart()
        {
            ConfChart1();

            ConfChart2();
        }

        private void MainGridFornated()
        {
            G.Rows[0].Cells[0].Style.Font.Bold = 1 == 1;
            for (int i = 1; i < G.Rows.Count; i++)
            {
                G.Rows[i].Cells[0].Style.Padding.Left = 10;
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChoseParam();

            DataBindGrid(G, "Grid");

            ConfigurateGrid(G);

            SetImageForRow(G);

            ConfigurateChart();

            ConfGridDetail();

            SetHeaderReport();

            MainGridFornated();
        }        

        private void ConfGridDetail()
        {
            G1_Panel.Visible = false;
            G2_Panel.Visible = false;
            G3_Panel.Visible = false;

            CheckBox1.Visible = false;

            int MAinGridRowsCount = G.Rows.Count;


            if (ScopeCombo.SelectedValue == "Здравоохранение")
            {

                if (CheckBox1.Checked)
                {

                    DataBindGrid(G_1, ScopeCombo.SelectedValue + ";3.1");
                    ConfigurateGrid(G_1);

                    DataBindGrid(G_2, ScopeCombo.SelectedValue + ";3.2");
                    ConfigurateGrid(G_2);

                    DataBindGrid(G_3, ScopeCombo.SelectedValue + ";3.3");
                    ConfigurateGrid(G_3);

                    SetHierarhyRow(G_1);
                    SetHierarhyRow(G_2);
                    SetHierarhyRow(G_3);

                    SetImageForRow(G_1); 
                    SetImageForRow(G_2);
                    SetImageForRow(G_3);

                    GL1.Text = G.Rows[MAinGridRowsCount - 3].Cells[0].Text;
                    GL2.Text = G.Rows[MAinGridRowsCount - 2].Cells[0].Text;
                    GL3.Text = G.Rows[MAinGridRowsCount - 1].Cells[0].Text;

                    G1_Panel.Visible = true;
                    G2_Panel.Visible = true;
                    G3_Panel.Visible = true;
                }
                CheckBox1.Visible = true;
            }
            else
            if (ScopeCombo.SelectedValue == "Общее образование")
            {
                if (CheckBox1.Checked)
                {
                    DataBindGrid(G_1, ScopeCombo.SelectedValue + ";3.1");
                    ConfigurateGrid(G_1);                    

                    DataBindGrid(G_2, ScopeCombo.SelectedValue + ";3.2");
                    ConfigurateGrid(G_2);

                    SetHierarhyRow(G_1);
                    SetHierarhyRow(G_2);

                    SetImageForRow(G_1);
                    SetImageForRow(G_2);

                    GL1.Text = G.Rows[MAinGridRowsCount - 2].Cells[0].Text;
                    GL2.Text = G.Rows[MAinGridRowsCount - 1].Cells[0].Text;

                    G1_Panel.Visible = true;
                    G2_Panel.Visible = true;
                }
                CheckBox1.Visible = true;
            }
        }

        private void SetHeaderReport()
        {
            Hederglobal.Text = "Оценка неэффективных расходов (по муниципальным образованиям)";
            Page.Title = Hederglobal.Text;
            Label2.Text = "Оценка проведена в соответствии с распоряжением Правительства Российской Федерации от 11 сентября 2008 г. № 1313-р, в редакции  распоряжения Правительства Российской Федерации от 15 мая 2010    г. № 758-р (с учетом изменений) по выбранному муниципальному образованию";

            Label4.Text = string.Format("Доля неэффективных расходов в общем объеме расходов на сферу «{0}», процент", ScopeCombo.SelectedValue);

            Label6.Text = string.Format("Объем неэффективных расходов на сферу «{0}», тысяч рублей", ScopeCombo.SelectedValue);



            Label7.Visible = (RegionCombo.SelectedValue == "г.Ханты-Мансийск") & (ScopeCombo.SelectedValue == "Здравоохранение");
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
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0";
            }

            FormatGridSheet(e.CurrentWorksheet);

            try
            {
                e.Workbook.Worksheets["Диаграмма 1"].Rows[0].Cells[0].Value = Label4.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0], C);

                e.Workbook.Worksheets["Диаграмма 2"].Rows[0].Cells[0].Value = Label6.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0], C0);
            }
            catch { }
        }

        private void FormatGridSheet(Worksheet w)
        {
            w.Rows[3].Cells[0].Value = "Показатели";

            w.Columns[0].Width = 700*40;
            w.Columns[1].Width = 100 * 40;
            w.Columns[2].Width = 40 * 40;
            w.Columns[3].Width = 100 * 40;
            w.Columns[4].Width = 40 * 40;

            SetStyleHeadertableFromExcel(w.Rows[3].Cells[0].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[1].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[2].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[3].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[4].CellFormat);

            w.Rows[3].Cells[1].Value = SelectPrevYear; //"2009 Значение предыдущего года";
            w.Rows[3].Cells[2].Value = "Ранг";
            w.Rows[3].Cells[3].Value = SelectCurrentYear;//"2010 Значение отчетного года";
            w.Rows[3].Cells[4].Value = "Ранг";

            for (int i = 0; i < G.Rows.Count; i++)
            {
                //sheet.Rows[i + startrow].Height = 32 * 40;
                w.Rows[i + 4].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                w.Rows[i + 4].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                w.Rows[i + 4].CellFormat.FormatString = "### ### ##0.00";
            }
        }



        private void SetSizeMapAndChart()
        {
            //C.Width = (int)(C.Width.Value * 0.75);

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

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

            SetSizeMapAndChart();

            CleraHidenColGrid();

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 5;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";


            ReportExcelExporter1.ExcelExporter.Export(G, sheet1);

            if (ScopeCombo.SelectedValue == "Здравоохранение")
            {

                if (CheckBox1.Checked) 
                {

                    Worksheet sheet11 = workbook.Worksheets.Add("Таблица 1");

                    Worksheet sheet12 = workbook.Worksheets.Add("Таблица 2");

                    ReportExcelExporter1.ExcelExporter.Export(G_1, sheet11);

                    ReportExcelExporter1.ExcelExporter.Export(G_2, sheet12);
                }
            }

            if (ScopeCombo.SelectedValue == "Общее образование")
            {
                if (CheckBox1.Checked)
                {
                    Worksheet sheet11 = workbook.Worksheets.Add("Таблица 1");

                    Worksheet sheet12 = workbook.Worksheets.Add("Таблица 2");

                    Worksheet sheet13 = workbook.Worksheets.Add("Таблица 3");

                    ReportExcelExporter1.ExcelExporter.Export(G_1, sheet11);

                    ReportExcelExporter1.ExcelExporter.Export(G_2, sheet12);

                    ReportExcelExporter1.ExcelExporter.Export(G_3, sheet13);
                }
            }

            
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
                sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
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
