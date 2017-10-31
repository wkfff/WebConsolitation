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

namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLess
{
    public partial class _default : CustomReportPage
    {
        bool TwoYear = true;

        string SelectCurrentYear;
        string SelectPrevYear;

        IDataSetCombo DataSetYearCombo;

        IDataSetCombo DataSetScopeCombo;

        CustomParam Scope { get { return (UserParams.CustomParam("Scope")); } }
         
        CustomParam Field { get { return (UserParams.CustomParam("Field")); } }

        CustomParam FilterGrid { get { return (UserParams.CustomParam("FilterGrid")); } }

        CustomParam FilterChart2 { get { return (UserParams.CustomParam("FilterChart2")); } }

        CustomParam CommentToYear { get { return (UserParams.CustomParam("CommentToYear")); } }

        CustomParam CommentToZdrav { get { return (UserParams.CustomParam("CommentToZdrav")); } }

        ICustomizerSize CustomizerSize;


        List<string> MaxListMO;
        List<string> MinListMO;

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
                int onePercent = (int)1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);

                }
                Grid.Columns[0].Width = onePercent * 13;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);
                }
                Grid.Columns[0].Width = onePercent * 17;
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
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);

                }
                Grid.Columns[0].Width = onePercent * 13;                
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 17) / (Grid.Columns.Count - 1);
                }
                Grid.Columns[0].Width = onePercent * 17;                
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
                //xmlDoc.Load(dir + "\\reports\\OMCY_ACR_0003\\table.xml");
                //Server.MapPath("~")
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


            C.Width = CustomizerSize.GetChartWidth();
            C.Height = 500;
            
            DundasMap.Width = CustomizerSize.GetChartWidth();
            DundasMap.Height = 700;

            
        }

        private void FillCombo()
        {
            DataSetYearCombo = new DataSetComboLinear();
            DataSetYearCombo.LoadData(GetDT("AllYear", "Year"));            

            DataSetScopeCombo = new DataSetComboLinearXml();
            DataSetScopeCombo.LoadData(new DataTable(Server.MapPath("~")));

            if (!Page.IsPostBack)
            {
                YearCombo.FillDictionaryValues(DataSetYearCombo.DataForCombo);
                YearCombo.Title = "Период";
                YearCombo.Width = 150;

                ScopeCombo.FillDictionaryValues(DataSetScopeCombo.DataForCombo);
                ScopeCombo.Title = "Сфера";
            }
        }

        private void ChoseParam()
        {
            Scope.Value = DataSetScopeCombo.OtherInfo[ScopeCombo.SelectedValue]["UniqeName"];
            
            FilterGrid.Value = DataSetScopeCombo.OtherInfo[ScopeCombo.SelectedValue]["StringToQuery"];

            SelectCurrentYear = YearCombo.SelectedValue;
            SelectPrevYear = (int.Parse(YearCombo.SelectedValue) - 1).ToString();

            TwoYear = CheckBox1.Checked;

            CommentToYear.Value = TwoYear ? "" : "--";
            CommentToZdrav.Value = ScopeCombo.SelectedValue != "Здравоохранение" ? "--" : "";

        }


        #region DisplayGrid

        private void SetRang(int ColValue, int Colrang)
        {
            RowsCollection Rows = G.Rows;

            int StartRow = 2;

            double PrevMinVal = double.MinValue;
            for (int i = StartRow; i < Rows.Count; i++)
            {
                double MinVal = double.MaxValue;
                for (int j = StartRow; j < Rows.Count; j++)
                {
                    if ((double.Parse(Rows[j].Cells[ColValue].Text) < MinVal) && (double.Parse(Rows[j].Cells[ColValue].Text) > PrevMinVal))
                    {
                        MinVal = double.Parse(Rows[j].Cells[ColValue].Text);
                    }
                }


                for (int j = StartRow; j < Rows.Count; j++)
                {
                    if (double.Parse(Rows[j].Cells[ColValue].Text) > PrevMinVal)
                    {
                        try
                        {
                            Rows[j].Cells[Colrang].Value = int.Parse(Rows[j].Cells[Colrang].Text) + 1;
                        }
                        catch
                        {
                            Rows[j].Cells[Colrang].Value = "1";
                        }
                    }
                }
                PrevMinVal = MinVal;
            }
        }

        private void ConfRow(UltraGridRow Row)
        {
            if (RowZeroCount != null)
            {

                //if (Row.Cells.FromKey("Region").Text[0]!='г')
                //{
                //    Row.Cells.FromKey("Region").Text += " " + "район";
                //}

                for (int i = 1; i < Row.Cells.Count; i++)
                {
                    try
                    {
                        UltraGridCell Cell = Row.Cells[i];

                        double ValueCell = double.Parse(Cell.Text);
                        int CountZero = int.Parse(RowZeroCount.Cells[i].Text);
                        Cell.Value = string.Format("{0:N" + RowZeroCount.Cells[i].Text + "}", ValueCell);
                    }
                    catch { }
                }
            }
        }

        private void DelInfics(UltraGridRow Row)
        {
            foreach (UltraGridCell Cell in Row.Cells)
            {
                try
                {
                    Cell.Text = Cell.Text.Replace("str", "");
                }
                catch { }
            }
        }

        private void ConfHeder()
        {
            foreach (UltraGridColumn Col in G.Columns)
            {
                Col.Header.Style.Wrap = true;
                Col.Header.Style.Width = 200;
            }

            ConfHierarhy();
        }

        string DelDataMemberString(string s)
        {
            if (s[0] == '(')
            {
                return s.Replace("(", "").Replace(" ДАННЫЕ)", "").Replace("в том числе:", "").Replace("всего","").Replace(" - ","");
            }                
            return s; 
            
        } 


        private void ConfHierarhy()
        {
            HeaderBase h = null;

            foreach (UltraGridColumn Col in G.Columns)
            {
                string CaptionHeder = Col.Header.Caption;
                string[] CaptionsHeader = CaptionHeder.Split(';');
                if (CaptionsHeader.Length > 1)
                {
                    int ColIndex = Col.Index;

                    ConfHeader(Col.Header, CaptionsHeader[1], ColIndex, 1, 1, 1);                    

                    if (CaptionsHeader[1].Contains(TwoYear?"PrevPeriiod":"CurrentPeriiod"))
                    {
                        try
                        {
                            Col.Header.Caption = TwoYear ? SelectPrevYear : SelectCurrentYear;

                            string Unit = G.Rows[1].Cells[Col.Index].Text.ToLower().Replace("str", "");
                            G.Bands[0].HeaderLayout.Add(
                                CreateHeader(
                                    DelDataMemberString(CaptionsHeader[0]) + ", " + Unit, ColIndex, 0, TwoYear ? 2 : 1, 1));
                        }
                        catch { }
                    }
                    else
                    {
                        
                        Col.Header.Caption = SelectCurrentYear;
                    }
                }
                else
                {
                    ConfHeader(Col.Header, CaptionsHeader[0], Col.Index, 0, 1, 2);
                }
            }
            G.Bands[0].Columns[0].Header.Caption = "Территория";

            //h.Caption = "Text";//TwoYear ? SelectPrevYear : SelectCurrentYear;
            

            G.Bands[0].HeaderLayout.Add(
                            CreateHeader(
                                "Ранг",
                                G.Bands[0].HeaderLayout[G.Bands[0].HeaderLayout.Count - 1].RowLayoutColumnInfo.OriginX + (TwoYear?2:1)
                                , 0, TwoYear ? 2 : 1, 1));
            
        }

        private void ConfHeader(ColumnHeader Header, string Caption, int x, int y, int span_x, int span_y)
        {
            Header.Caption = Caption;
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = span_x;
            Header.RowLayoutColumnInfo.SpanY = span_y;
            Header.Style.HorizontalAlign = HorizontalAlign.Center;
            Header.Style.Wrap = true;
        }

        private ColumnHeader CreateHeader(string Caption, int x, int y, int span_x, int span_y)
        {
            ColumnHeader Header = new ColumnHeader();
            ConfHeader(Header, Caption, x, y, span_x, span_y);
            return Header;
        }



        private void ConfigurationGrid()
        {
            ConfHeder();

            ConfCol();

            ConfRows();
        }

        private void ConfCol()
        {
            ColumnsCollection Cols = G.Columns;
            Cols[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            for (int i = 1; i < Cols.Count; i++)
            {
                Cols[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Cols[i].CellStyle.Padding.Right = 3;
            }
            foreach (UltraGridColumn col in G.Columns)
            {
                col.CellStyle.Wrap = true;
            }

            CustomizerSize.ContfigurationGrid(G);

        }

        UltraGridRow RowZeroCount = null;

        private void ConfRows()
        {
            foreach (UltraGridRow Row in G.Rows)
            {
                DelInfics(Row);
                if (Row.Cells[0].Text == "Unit")
                {
                    //Строка с единицами измерения
                }
                else
                    if (Row.Cells[0].Text == "CountZero")
                    {
                        RowZeroCount = Row;
                    }
                    else
                        ConfRow(Row);                
                
            }

            if (TwoYear)
            {
                SetRang(G, G.Columns.Count - 4, G.Columns.Count - 2, 2);
                SetRang(G, G.Columns.Count - 3, G.Columns.Count - 1, 2);
            }
            else 
            {
                SetRang(G, G.Columns.Count - 2, G.Columns.Count - 1, 2);
            }


            G.Rows[0].Hidden = true;
            G.Rows[1].Hidden = true;
            //G.Rows[2].Hidden = true;
        }

        private void SetGrayStart(int p)
        {
            int MinIndex = G.Rows.Count-1;

            foreach (UltraGridRow row in G.Rows)
            {
                try
                {
                    if (!row.Hidden)
                        if (int.Parse(row.Cells[p].Text) > int.Parse(G.Rows[MinIndex].Cells[p].Text))
                        {
                            MinIndex = row.Index;
                        }
                }
                catch { }
            }

            for (int i = 0; G.Rows.Count > i; i++)
            {
                try
                {
                    if (G.Rows[i].Cells[p].Value.ToString() == G.Rows[MinIndex].Cells[p].Value.ToString())
                    {
                        G.Rows[i].Cells[p].Title = "Наиболшая доля неэффективных расходов";
                        G.Rows[i].Cells[p].Style.BackgroundImage ="~/images/starGrayBB.png";
                        G.Rows[i].Cells[p].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
                catch { }
            }

        }
        private void SetYellowStart(int p)
        {
            int MinIndex = G.Rows.Count - 1;

            foreach (UltraGridRow row in G.Rows)
            {
                try
                {
                    if (!row.Hidden)
                        if (int.Parse(row.Cells[p].Text)  < int.Parse(G.Rows[MinIndex].Cells[p].Text))
                        {
                            MinIndex = row.Index;
                        }
                }
                catch { }
            }

            for (int i = 0; G.Rows.Count > i; i++)
            {
                try
                {
                    if (G.Rows[i].Cells[p].Value.ToString() == G.Rows[MinIndex].Cells[p].Value.ToString())
                    {
                        G.Rows[i].Cells[p].Title = "Наименьшая доля неэффективных расходов";
                        G.Rows[i].Cells[p].Style.BackgroundImage = "~/images/starYellowBB.png";
                        G.Rows[i].Cells[p].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        //"Самый высокий интегральный показатель"
                    }
                }
                catch { }
            }

        }

        #endregion

        #region Ранжирование для грида

        void FormatGridRang()
        {

            for (int k = 0; k < G.Columns.Count; k++)
            {
                
                if ((k == (G.Columns.Count -1))||(TwoYear?(k== G.Columns.Count -2):false))
                {
                    for (int i = 0; i < G.Rows.Count; i++)
                    {
                        int max_r = i;
                        int min_r = i;
                        for (int j = 0; j < G.Rows.Count; j++)
                        {
                            try
                            {
                                if (G.Rows[i].Cells[k - (TwoYear ? 2 : 1)].Value.ToString() == G.Rows[j].Cells[k - (TwoYear ? 2 : 1)].Value.ToString())
                                {

                                    if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) > System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString()))
                                    {
                                        max_r = j;
                                    }
                                    if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) < System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString()))
                                    {
                                        min_r = j;
                                    }

                                }
                            }
                            catch { }
                        }
                        try
                        {
                            if (min_r != max_r)
                            {
                                string s = G.Rows[min_r].Cells[k].Value.ToString() + " - " + G.Rows[max_r].Cells[k].Value.ToString();
                                System.Decimal max_r_ = System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString());
                                System.Decimal min_r_ = System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString());
                                for (int j = 0; j < G.Rows.Count; j++)
                                {
                                    try
                                    {
                                        if ((System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) <= max_r_) &&
                                            (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) >= min_r_))
                                        {
                                            G.Rows[j].Cells[k].Text = s;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }


                    }
                }
            }
            for (int i = 0; i < G.Rows.Count; i++)
            {
                for (int j = 0; j < G.Columns.Count; j++)
                {
                   // G.Rows[i].Cells[j].Value = G.Rows[i].Cells[j].Value == null ? "-" : G.Rows[i].Cells[j].Value;
                }
            }


            int LastMaxIndex = GetMaxRowFromCol(G, (G.Columns.Count - (TwoYear ? 3 : 2)));
            int LastMinIndex = GetMinRowFromCol(G, (G.Columns.Count - (TwoYear ? 3 : 2)));

            MaxMOValu = G.Rows[LastMaxIndex].Cells[G.Columns.Count - (TwoYear ? 3 : 2)].Text;
            MinMOValu = G.Rows[LastMinIndex].Cells[G.Columns.Count - (TwoYear ? 3 : 2)].Text;


            MinListMO = SetStar(G, (G.Columns.Count - 1), LastMaxIndex, "~/images/starGrayBB.png", "Наибольшая доля неэффективных расходов");
            MaxListMO = SetStar(G, (G.Columns.Count - 1), LastMinIndex, "~/images/starYellowBB.png", "Наименьшая доля неэффективных расходов");
            if (TwoYear)
            {
                LastMaxIndex = GetMaxRowFromCol(G, (G.Columns.Count - 4));
                LastMinIndex = GetMinRowFromCol(G, (G.Columns.Count - 4));
                SetStar(G, (G.Columns.Count - 2), LastMaxIndex, "~/images/starGrayBB.png", "Наибольшая доля неэффективных расходов");
                SetStar(G, (G.Columns.Count - 2), LastMinIndex, "~/images/starYellowBB.png", "Наименьшая доля неэффективных расходов");
           
            }
        }

        string MaxMOValu = "";
        string MinMOValu = "";

        #region SetStar

        List<string> SetStar(UltraWebGrid G, int Col, int RowBaseVaslue, string Star, string Title)
        {
            List<string> MO = new List<string>();

            for (int i = 0; G.Rows.Count > i; i++)
            {
                try
                {
                    if (G.Rows[i].Cells[Col].Value.ToString() == G.Rows[RowBaseVaslue].Cells[Col].Value.ToString())
                    {
                        G.Rows[i].Cells[Col].Title = Title;
                        G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                        G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";

                        MO.Add(G.Rows[i].Cells[0].Text);
                        //"Самый высокий интегральный показатель"
                    }
                }
                catch { }
            }

            return MO;


        }

        #endregion

        #region GetMAX and GetMin
        int GetMaxRowFromCol(UltraWebGrid dt, int col)
        {
            int MaxIndex = 4;
            for (int i = 3; i < dt.Rows.Count; i++)
            {
                try
                {
                    if ((null != dt.Rows[i].Cells[col].Value))
                        if (double.Parse(dt.Rows[i].Cells[col].Text) > double.Parse(dt.Rows[MaxIndex].Cells[col].Text))
                        {
                            MaxIndex = i;
                        }
                }
                catch { }

            }
            return MaxIndex;
        }

        int GetMinRowFromCol(UltraWebGrid dt, int col)
        {
            int MaxIndex = 4;
            for (int i = 3; i < dt.Rows.Count; i++)
            {
                try
                {
                    if (null != dt.Rows[i].Cells[col].Value)
                        if (double.Parse(dt.Rows[i].Cells[col].Text) < double.Parse(dt.Rows[MaxIndex].Cells[col].Text))
                        {
                            MaxIndex = i;
                        }
                }
                catch { }

            }
            return MaxIndex;
        }

        #endregion

        void SetRang(UltraWebGrid dt, int ColVal, int ColRang, int StartRow)
        {
            int RowCount = dt.Rows.Count;
            int L_max = StartRow;
            int L_min = StartRow;
            int rang = 0;
            for (int i = StartRow; i < RowCount; i++)
            {

                for (int j = StartRow; j < RowCount; j++)
                {
                    
                    if (dt.Rows[j].Cells[ColVal].Value != null)
                    {
                        if ((System.Decimal.Parse(dt.Rows[j].Cells[ColVal].Text) <= System.Decimal.Parse(dt.Rows[L_max].Cells[ColVal].Text)) && (dt.Rows[j].Cells[ColRang].Value == null))
                        {
                            L_max = j;
                        }
                        if ((System.Decimal.Parse(dt.Rows[j].Cells[ColVal].Text) > System.Decimal.Parse(dt.Rows[L_min].Cells[ColVal].Text)) && (dt.Rows[j].Cells[ColRang].Value == null))
                        {
                            L_min = j;
                        }
                    }
                    else
                    {
                        //                        minys++;)
                    }

                }
                if (true)
                {
                    if (dt.Rows[L_max].Cells[ColRang].Value == null)
                        dt.Rows[L_max].Cells[ColRang].Value = ++rang;


                }
                else
                {
                    dt.Rows[L_max].Cells[ColRang].Value = i - StartRow + 1;
                }


                L_max = L_min;
            }
            FormatGridRang();

        }

        #endregion

        private void DataBindGrid()
        {
            G.Bands.Clear();
            DataTable Table = GetDT("Grid", "Region");

            Table.Columns.Add("Ранг;PrevPeriiod");
            if (TwoYear)
            {
                Table.Columns.Add("Ранг;CurrentPeriiod");
            }
            else
            {
            }

            G.DataSource = Table;
            G.DataBind();
        }

        private void ConfStackChart(UltraChart Chart)
        {
            //Chart.ChartType = ChartType.ColumnChart;
            //Chart.Data.SwapRowsAndColumns = false;

            //Chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            //Chart.Axis.X.Labels.Visible = true;
            //Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            //Chart.Axis.X.Labels.SeriesLabels.Visible = true;

            //C.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ##0>%";
         
            //Chart.Legend.Visible = true;
            //Chart.Legend.SpanPercentage = 20;

            
            


            C.Axis.X.Labels.Visible = TwoYear;
            
            

            Chart.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);

        }

        private void ConfChart()
        {         

            DataBindChart_1();
            //ConfStackChart(C);
        }


        #region Chart1
        void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            List<Primitive> PrimitiveAdd = new List<Primitive>();
            List<Primitive> PrimitiveDel = new List<Primitive>();

            bool LegendMainBox = false;

            int BoxWidth = 0;

            int CountLegendBox = 1;

            #region Коробочки
            /*
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive p = e.SceneGraph[i];

                if ((p is Infragistics.UltraChart.Core.Primitives.Box) && (string.IsNullOrEmpty(p.Path)))
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)p;
                    if (p.Column < (TwoYear?2:1))
                    {
                        Infragistics.UltraChart.Core.Primitives.Box box2 = (Infragistics.UltraChart.Core.Primitives.Box)e.SceneGraph[i + (TwoYear ? 2 : 1)];

                        BoxWidth = box.rect.Width;

                        if (p.Column < 1)
                        {
                            
                        }
                        else
                        {
                            box.rect.X += box.rect.Width;

                            //PrimitiveDel.Add(box);

                            
                        }
                        PrimitiveAdd.Add(box2);

                        box.rect.Width *= 2;
                         
                        box2.rect.X = box.rect.X+(TwoYear?0:0);
                        box.rect.X = box2.rect.X;                        
                        box2.rect.Y = box.rect.Y - box2.rect.Height;

                        box.rect.Width = box2.rect.Width * 2 + 1;
                        box2.rect.Width = box.rect.Width;
                        box2.rect.Height++;
                        box2.PE = box2.PE.Clone();
                        box2.PE.Fill = Color.RoyalBlue;

                        PrimitiveDel.Add(box);
                        PrimitiveAdd.Add(box);

                    }
                }
            }*/
            int CounterLegendLine = TwoYear ? 1 : 0;
            
            #endregion

            
            string[] TextLegendLine = TwoYear ? new string[2] { "Средний обьём неэффективных расходов по субьекту РФ за " + SelectCurrentYear + " год", "Средний обьём неэффективных расходов по субьекту РФ за " + SelectPrevYear + " год" } : new string[1] { "Средний обьём неэффективных расходов по субьекту РФ за " + SelectCurrentYear + " год" };

            Color[] ColorLine = TwoYear ? new Color[2] { Color.Red, Color.Blue } : new Color[1] { Color.Red };
            #region Легенды и Подписи =)

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive p = e.SceneGraph[i];

                if ((p is Text) && (((Text)p).GetTextString().Contains("more...")))
                {
                    PrimitiveDel.Add(p);
                }

                if (p.Path == "Border.Title.Legend")
                {
                    //continue;

                    if (p is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        if (LegendMainBox)
                        {
                            if (CountLegendBox > -1)
                            {
                                Box box_legend = (Box)p;
                                Text Text_legend = (Text)(e.SceneGraph[i + 1]);
                                Text_legend.bounds.Width *= 2;                                

                                ///if (CountLegendBox < 2)
                                {
                                    Text_legend.SetTextString("Доля неэффективных расходов");
                                }
                                //else 
                                {
                                    //Tex
                                    //Text_legend.SetTextString("Доля эффективных расходов");
                                    //box_legend.PE.Fill = Color.RoyalBlue;
                                }
                                 
                                CountLegendBox--;
                            }
                            else
                            {
                                return;
                                if (CounterLegendLine >= 0)
                                {
                                    //return;
                                    //continue;
                                    Box boxLine = (Box)p;
                                    //boxLine.rect.Height = 2;
                                    Line l = new Line(new Point((int)boxLine.rect.X, (int)boxLine.rect.Y+5), new Point((int)boxLine.rect.X + boxLine.rect.Width, (int)boxLine.rect.Y+5));
                                    l.PE.Fill = ColorLine[CounterLegendLine];
                                    l.PE.StrokeWidth = 3;
                                    PrimitiveAdd.Add(l);
                                    
                                     //boxLine.rect.X 
                                    Text Text_legend_line = (Text)(e.SceneGraph[i + 1]);
                                    Text_legend_line.bounds.Width *= 4;
                                    Text_legend_line.SetTextString(TextLegendLine[CounterLegendLine]);
                                    //Text_legend_line.

                                    CounterLegendLine--;
                                }
                                else
                                {
                                    
                                    PrimitiveDel.Add(e.SceneGraph[i + 1]);
                                }
                                PrimitiveDel.Add(p);
                            }
                        }
                        LegendMainBox = true;
                    }
                }
                else
                    if ("Border.Title.Grid.X" == p.Path)
                    {
                        if (p is Infragistics.UltraChart.Core.Primitives.Text)
                        {
                            Text LablelX = (Text)(p);
                            string caption = LablelX.GetTextString();
                            
                            //if (caption.Contains("del"))
                            if ((caption.Split(';').Length > 1) || (caption.Contains("del")))
                            {
                                PrimitiveDel.Add(p);
                            }
                            else
                            { 
                                //continue;
                                
                                if (caption.Contains("2009"))
                                {
                                    Text newText = new Text(new Point(LablelX.bounds.Left+5, (int)yAxis.Map( -12)), "2009",
                                        new LabelStyle(new Font("Arail",10),
                                            LablelX.labelStyle.FontColor,
                                            false,
                                            true,
                                            true,
                                            StringAlignment.Far,
                                            StringAlignment.Far,
                                            TextOrientation.VerticalLeftFacing));
                                    
                                    PrimitiveAdd.Add(newText);
                                    PrimitiveDel.Add(LablelX);

                                }
                                else
                                    if (caption.Contains("2010"))
                                    {
                                        Text newText = new Text(new Point(LablelX.bounds.Left+BoxWidth+5, (int)yAxis.Map(-12)), "2010",
                                            new LabelStyle(new Font("Arail", 10),
                                                LablelX.labelStyle.FontColor,
                                                false,
                                                true,
                                                true,
                                                StringAlignment.Far,
                                                StringAlignment.Far,
                                                TextOrientation.VerticalLeftFacing));

                                        if (TwoYear) PrimitiveAdd.Add(newText);
                                        PrimitiveDel.Add(LablelX);
                                    }
                                    else
                                    {
                                        LablelX.bounds.Y += (TwoYear?25:-30);
                                        LablelX.bounds.Height = (TwoYear ? LablelX.bounds.Height : LablelX.bounds.Height + 30);
                                        LablelX.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                                        LablelX.labelStyle.Font = new Font("Aral", 9);
                                        LablelX.labelStyle.HorizontalAlign = StringAlignment.Far;
                                        LablelX.labelStyle.VerticalAlign = StringAlignment.Center;
                                    }

                            }
                        }
                    }

            }

            #endregion

            #region Лини
            double y = yAxis.Map(AvgChartCurYear);
            double x0 = xAxis.MapMaximum;
            double x1 = xAxis.MapMinimum;
            Line l_ = new Line(new Point((int)x0, (int)y), new Point((int)x1, (int)y));
            l_.PE.Fill = ColorLine[0];
            //l_.lineStyle.DrawStyle = LineDrawStyle.Dash;
            l_.PE.StrokeWidth = 3;
            PrimitiveAdd.Add(l_);


            if (TwoYear)
            {
                y = yAxis.Map(AvgChartPrevYear);
                x0 = xAxis.MapMaximum;
                x1 = xAxis.MapMinimum;

                l_ = new Line(new Point((int)x0, (int)y), new Point((int)x1, (int)y));
                l_.PE.Fill = ColorLine[1];
                l_.PE.StrokeWidth = 3;
                //l_.lineStyle.DrawStyle = LineDrawStyle.Dash;                
                PrimitiveAdd.Add(l_);
            }
            #endregion


            foreach (Primitive p in PrimitiveDel)
            {
                e.SceneGraph.Remove(p);
            }

            foreach (Primitive p in PrimitiveAdd)
            {
                e.SceneGraph.Add(p);
            }



        }
        #endregion

        #region Chart2
        void ToStackChart(SceneGraph scene, int PeriodLoop, int CountLevel, Color[] Colors, string[] LegendCaption)
        {
            Double BoxWidth = 0;

            List<Primitive> PrimitiveAdd = new List<Primitive>();
            List<Primitive> PrimitiveDel = new List<Primitive>();
            if (true)
            {
                #region Столбики
                for (int i = 0; i < scene.Count; i++)
                {
                    Primitive p = scene[i];
                    if ((p is Infragistics.UltraChart.Core.Primitives.Box) && (string.IsNullOrEmpty(p.Path)))
                    {
                        Box box = (Box)p;
                        BoxWidth = box.rect.Width;
                        int CurCol = box.Column;

                        if (CurCol < (PeriodLoop * (CountLevel - 1)))
                        {
                            Box NextBox = (Box)scene[i + PeriodLoop];

                            NextBox.rect.Y = box.rect.Y - NextBox.rect.Height;
                            NextBox.rect.X = box.rect.X;

                            if (CurCol < PeriodLoop)
                            {
                                box.rect.X += CurCol * (CountLevel - 1) * box.rect.Width;
                                //box.rect.X += CurCol * (CountLevel) * box.rect.Width;
                                box.rect.Width *= CountLevel;
                            }

                            NextBox.rect.X = box.rect.X;
                            NextBox.rect.Width = box.rect.Width;

                            NextBox.PE = NextBox.PE.Clone();
                            NextBox.PE.Fill = Colors[CurCol / PeriodLoop];
                        }


                    }
                }
                #endregion
            }
            int Looper = 0;
            #region Подписи
            for (int i = 0; i < scene.Count; i++)
            {
                Primitive p = scene[i];
                if ((p is Infragistics.UltraChart.Core.Primitives.Text) && (p.Path == "Border.Title.Grid.X"))
                {
                    Text text = (Text)p;
                    int CurCol = text.Column;
                    if ((text.GetTextString().Split(';').Length > 1) || (text.GetTextString().Split(';')[0].Length < 6))
                    {
                        if (Looper >= (CountLevel * PeriodLoop))
                        {
                            Looper = 0;
                        }

                        
                        if (Looper < PeriodLoop)
                        {
                            if (!TwoYear)
                            {
                                text.bounds.X += -40;
                            }
                            else
                            if (CountLevel == 3)
                            {
                                text.bounds.X +=
                                   (Looper - ((CountLevel - 1) * (PeriodLoop)))
                                   * (PeriodLoop)
                                   * text.bounds.Width + 10;
                            }
                            else
                                if (CountLevel != 1)
                                {
                                    text.bounds.X +=
                                        (Looper - ((CountLevel - 1) * (PeriodLoop)))
                                        * (PeriodLoop)
                                        * text.bounds.Width;
                                }
                                else
                                {
                                    text.bounds.X += -15;
                                }
                            text.bounds.Height = text.bounds.Width;
                            text.bounds.Width = (TwoYear?(int)BoxWidth * CountLevel:(int)BoxWidth );
                            text.labelStyle.Orientation = TextOrientation.Horizontal;

                            text.SetTextString(text.GetTextString().Split(';')[1]);
                        }
                        else
                        {
                            if ((text.GetTextString().Split(';').Length > 1))
                            {
                                PrimitiveDel.Add(p);
                            }
                        }

                        Looper++;

                    }
                    else
                    {
                        text.bounds.Y -= 30;
                    }
                }
            }
            #endregion

            bool LegendMainBox = false;
            int CountLegendBox = LegenCaption.Length;

            #region Легенда
            for (int i = 0; i < scene.Count; i++)
            {
                Primitive p = scene[i];

                if ((p is Text) && (((Text)p).GetTextString().Contains("more...")))
                {
                    PrimitiveDel.Add(p);
                }

                if (p.Path == "Border.Title.Legend")
                {
                    if (p is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        if (LegendMainBox)
                        {
                            if (CountLegendBox > 0)
                            {
                                Box box_legend = (Box)p;
                                Text Text_legend = (Text)(scene[i + 1]);

                                if (CountLegendBox < 2)
                                {
                                    Text_legend.SetTextString(LegenCaption[0]);
                                    Text_legend.bounds.Width *= 7;
                                }
                                else
                                {
                                    try
                                    {
                                        Text_legend.SetTextString(LegenCaption[CountLegendBox - 1]);
                                        Text_legend.bounds.Width *= 7;

                                        box_legend.PE.Fill = Colors[CountLegendBox - 2];
                                    }
                                    catch { }
                                }

                                CountLegendBox--;
                            }
                            else
                            {
                                PrimitiveDel.Add(p);
                                PrimitiveDel.Add(scene[i + 1]);
                            }
                        }
                        LegendMainBox = true;
                    }
                }
            }
            #endregion

            foreach (Primitive p in PrimitiveDel)
            {
                scene.Remove(p);
            }

            foreach (Primitive p in PrimitiveAdd)
            {
                scene.Add(p);
            }
        }
        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChoseParam();

            DataBindGrid();

            ConfigurationGrid();

            ConfChart();

            ConfigurationMap();

            SetHeaderReport();

        }

        private void SetHeaderReport()
        {
            
            Hederglobal.Text = "Оценка неэффективных расходов";
            Page.Title = Hederglobal.Text;
            Label2.Text = "Оценка проведена в соответствии с распоряжением Правительства Российской Федерации от 11 сентября 2008 г. № 1313-р, в редакции  распоряжения Правительства Российской Федерации от 15 мая 2010 г. № 758-р (с учетом изменений)";

            Label4.Text = string.Format("Доля неэффективных расходов в общем объеме расходов на сферу «{0}»", ScopeCombo.SelectedValue);

            //Label7.Text = string.Format("Объем неэффективных расходов на сферу «{0}»", ScopeCombo.SelectedValue);

            Label1.Text = string.Format("Доля неэффективных расходов в сфере «{0}» за {1} год ", ScopeCombo.SelectedValue, YearCombo.SelectedValue);

            if (TwoYear)
            {
                Label4.Text = string.Format("Доля неэффективных расходов в общем объеме расходов на сферу «{0}»", ScopeCombo.SelectedValue);
            }
            else
            {
                Label4.Text = string.Format("Доля неэффективных расходов в общем объеме расходов на сферу «{0}» за {1} год ", ScopeCombo.SelectedValue, YearCombo.SelectedValue);
            }

            FormatReportText();
            Label6.Visible = false;
                //ScopeCombo.SelectedValue == "Здравоохранение";

            

        }

        private void FormatReportText()
        {

            string MaxMO = ""+MaxListMO[0];
            for (int i = 1; i < MaxListMO.Count; i++)
            {
                MaxMO += ", " + MaxListMO[i];
            }

            string MinMO = "" + MinListMO[0];
            for (int i = 1; i < MinListMO.Count; i++)
            {
                MaxMO += ", " + MinListMO[i];
            }

            if (MinMOValu == MaxMOValu)
            {
                Label5.Text
                    = string.Format(@"По итогам оценки за {0} год:<br>
&nbsp&nbspНеэффективные расходы в сфере «{1}» отсутствуют по всем муниципальным образованиям субьекта РФ.
", YearCombo.SelectedValue, ScopeCombo.SelectedValue);
            }
            else
            {

                string FormatNullMOUnefect = string.Format("&nbsp&nbspНеэффективные расходы отсутствуют в МО: {0}", MaxMO);

                string FormatMinMOUnefect = string.Format("&nbsp&nbspНаименьшая доля неэффективных расходов среди муниципальных образований субьекта РФ составила <b>{0}</b>% ({1})", MinMOValu, MaxMO);

                Label5.Text = string.Format(@"По итогам оценки за <b>{0}</b> год:<br>
&nbsp&nbspСамый высокий уровень неэффективных расходов в сфере «{1}» в {2} году наблюдался в МО: {3} и составил <b>{4}</b>%. <br>{5}", YearCombo.SelectedValue, ScopeCombo.SelectedValue, YearCombo.SelectedValue, MinMO, MaxMOValu, (double.Parse(MinMOValu) == 0 ? FormatNullMOUnefect : FormatMinMOUnefect));
            }
        }

        void Chart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            ToStackChart(e.SceneGraph, TwoYear?2:1, LegenCaption.Length, new Color[2] { Color.Red  , Color.Yellow }, LegenCaption);
        }
        string GenshortName(string name)
        {
            return name.Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", "").Replace("Александровск-Сахалинский район","Александровск-\nСахалинский район");
        }

        private void DataBindChart_1()
        {
            DataTable TableChart = GetDT("Chart", "Region");
            if (TableChart.Rows.Count > 0)
            {
                TableChart.Rows[0].Delete();
                foreach (DataColumn Col in TableChart.Columns)
                {
                    
                    try
                    {
                        //Col.Caption = "test";
                        Col.ColumnName = Col.ColumnName.Split(';')[1];
                    }
                    catch { };
                }
                foreach (DataRow row in TableChart.Rows)
                {
                    row["Region"] = GenshortName(row["Region"].ToString());
                }

                AvgChartCurYear = GetAVGFromtable(TableChart, TwoYear?2:1);
                AvgChartPrevYear =TwoYear? GetAVGFromtable(TableChart, 1):0;


                if (TwoYear)
                {
                    TableChart.Columns.Remove(TableChart.Columns[TableChart.Columns.Count - 2]);
                }
                TableChart.Columns.Remove(TableChart.Columns[TableChart.Columns.Count - 1]);                

                C.DataSource = TableChart;
                C.DataBind();
            }
            #region conf 
            C.Data.SwapRowsAndColumns = false;
            C.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            C.Legend.SpanPercentage = (TwoYear ? 13 : 8);
            C.Axis.X.Extent = (TwoYear ? 200 : 180);
            

            #endregion



            C.FillSceneGraph += new FillSceneGraphEventHandler(C_FillSceneGraph);

        }

        List<Primitive> InsertToScene = new List<Primitive>();
        private void GenLegendLinecaption(FillSceneGraphEventArgs e, Primitive p, string text, Color c
            )
        {
            Box boxLine = (Box)p;
            Line l = new Line(new Point((int)boxLine.rect.X + 100, (int)boxLine.rect.Y + 5), new Point((int)boxLine.rect.X + boxLine.rect.Width + 100, (int)boxLine.rect.Y + 5));
            l.PE.Fill = c;
            l.PE.StrokeWidth = 3;
            InsertToScene.Add(l);

            Text Text_legend_line = new Text();
            Text_legend_line.bounds.Width *= 4;
            Text_legend_line.bounds.X = l.p2.X+3;
            Text_legend_line.bounds.Y = l.p1.Y - 13;//Math.Abs(l.p2.Y - l.p1.Y);   
            Text_legend_line.bounds.Width = 500;
            Text_legend_line.bounds.Height = 30;            
            Text_legend_line.SetTextString(text);

            LabelStyle ls  = new LabelStyle();

            ls.Font = new Font("Arial",10); 

            Text_legend_line.SetLabelStyle(ls);

            InsertToScene.Add(Text_legend_line);
        }


        

        void C_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            
            int n = 0;
            foreach (Primitive p in e.SceneGraph)
            {
                if (p.Path != null)
                {
                    if (p.Path.ToLower().Contains("legend"))
                    {
                        if (p is Box)
                        {
                            if (n == 1)
                            {
                                string text = "Среднее значение за 2010";
                                GenLegendLinecaption(e, p, text, Color.Red);
                            }

                            if (n == 2)
                            {
                                string text = "Среднее значение за 2009";
                                GenLegendLinecaption(e, p, text, Color.Blue);
                            }

                            n++;
                        }
                    }
                }
            }
            

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            #region Лини
            double y = yAxis.Map(AvgChartCurYear);
            double x0 = xAxis.MapMaximum;
            double x1 = xAxis.MapMinimum;
            Line l_ = new Line(new Point((int)x0, (int)y), new Point((int)x1, (int)y));
            l_.PE.Fill = Color.Red;            
            l_.PE.StrokeWidth = 3;
            InsertToScene.Add(l_);

            if (TwoYear)
            {
                y = yAxis.Map(AvgChartPrevYear);
                x0 = xAxis.MapMaximum;
                x1 = xAxis.MapMinimum;
                l_ = new Line(new Point((int)x0, (int)y), new Point((int)x1, (int)y));
                l_.PE.Fill = Color.Blue;
                l_.PE.StrokeWidth = 3;                
                InsertToScene.Add(l_);
            }
            #endregion

            foreach (Primitive p in InsertToScene)
            {
                e.SceneGraph.Add(p);
            }

        }

        

        decimal AvgChartCurYear = 0;
        decimal AvgChartPrevYear = 0;

        Decimal GetAVGFromtable(DataTable dt,int col)
        {
            Decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    sum += (Decimal)row[col];
                }
                catch { }
            }
            return sum/dt.Rows.Count;
        }

        string[] LegenCaption = null;
        private void DataBindChart_2()
        {

            LegenCaption = GetChartData();

            DataTable TableChart = GetDT("Chart2", "Region");

            for (int i = 0; i < TableChart.Columns.Count; i++)
            {
                DataColumn Col = TableChart.Columns[i];
                try
                {
                    Col.ColumnName = i.ToString() + ";" + Col.ColumnName.Split(';')[1];
                }
                catch { }

            }
            //C2.DataSource = TableChart;
            //C2.DataBind();

            Decimal SumRow = 0;
            Decimal NewSumRow = 0;
            for (int i = 0; i < TableChart.Rows.Count; i += 1)
            {
                SumRow = SumRow > NewSumRow ? SumRow : NewSumRow;
                NewSumRow = 0;

                for (int j = 1; j < TableChart.Columns.Count; j += 2)
                {
                    try
                    {
                        NewSumRow += (Decimal)TableChart.Rows[i][j];
                    }
                    catch { }
                }
                SumRow = SumRow > NewSumRow ? SumRow : NewSumRow;
                NewSumRow = 0;
                for (int j = 2; j < TableChart.Columns.Count; j += 2)
                {
                    try
                    {
                        NewSumRow += (Decimal)TableChart.Rows[i][j];
                    }
                    catch { }
                }

            }

            //C2.Axis.Y.RangeMax = (int)SumRow;
            //C2.Axis.Y.RangeMin = 0;
            //C2.Axis.Y.RangeType = AxisRangeType.Custom;
        }

        string[] GetChartData()
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~") + "\\" + "\\reports\\REGION_0010\\UnEffectLess\\chart.xml");
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

            FilterChart2.Value = mainNode.ChildNodes[1].InnerText;

            return mainNode.ChildNodes[2].InnerText.Split(';');
            
        }

        #region DundasMap
        protected void CustomizeMapPanel()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = 1 == 1;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            DundasMap.NavigationPanel.Visible = 1 == 1;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;
        }

        protected void AddShapeField()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;
        }

        protected string SetHyphen(string BaseString)
        {


            string[] Word = BaseString.Split(' ');
            int Median = Word.Length / 2;

            string res = "";
            for (int i = 0; i < Median; i++)
            {
                res += Word[i] + " ";
            }
            res += "\n";
            for (int i = Median; i < Word.Length; i++)
            {
                res += Word[i] + " ";
            }

            return res;

        }

        protected void AddLegend()
        {
            DundasMap.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            //legend1.Title = string.Format("{0}:\n{1}", ComboDirection.SelectedNodeParent, SetHyphen(ComboDirection.SelectedValue));
            legend1.Title = "Доля неэффективных\n расходов";
            //"Значение";
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Right;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;

            legend1.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend1);
        }
          
        protected void FillRule()
        {
            
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Value";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            //rule.FromColor = !CurentInverce ? Color.Green : Color.Red;
            rule.MiddleColor = Color.Yellow;
            //rule.ToColor = CurentInverce ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";//"LegendText";
            DundasMap.ShapeRules.Add(rule);
        }

        protected string ShorteningFO(string LongName)
        {
            switch (LongName)
            {
                case "Центральный федеральный округ": { return "ЦФО"; }
                case "Северо-Западный федеральный округ": { return "СФО"; }
                case "Южный федеральный округ": { return "ЮФО"; }
                case "Северо-Кавказский федеральный округ": { return "СКФО"; }
                case "Приволжский федеральный округ": { return "ПФО"; }
                case "Уральский федеральный округ": { return "УрФО"; }
                case "Сибирский федеральный округ": { return "СФО"; }
                case "Дальневосточный федеральный округ": { return "ДФО"; }
            }

            throw new Exception("Неизвестный регион");
        }

        protected void LoadMap()
        {
            string RegionName = "Сахалин";

            string DirectoryName = RegionName; //ShorteningFO(RegionName);

            string FileName = RegionName;//ShorteningFO(RegionName);

            string MapPath = Server.MapPath(string.Format("../../../maps/Субъекты/{0}/Территор.shp", DirectoryName));
            DundasMap.LoadFromShapeFile(MapPath, "NAME", true);
            //MapPath = Server.MapPath(string.Format("../../../maps/Субъекты/{0}/Граница.shp", DirectoryName));
            //DundasMap.LoadFromShapeFile(MapPath, "NAME", true);
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }



        protected void ConfigurationMap()
        {
            CustomizeMapPanel();

            AddShapeField();

            AddLegend();

            FillRule();

            LoadMap();

            DataBindMap();
        }

        protected Shape GetShapefromRegionName(string RegionShortName, string RegionFullName)
        {
            foreach (Shape shape in DundasMap.Shapes)
            {
                if ((RegionShortName == shape["NAME"].ToString()) || (RegionFullName == shape["NAME"].ToString()))
                {
                    return shape;
                }
            }
            return null;
        }
        protected void SetShapeTooltip(Shape shape, DataRow dataRow)
        {
            if (dataRow["RangFO"] != DBNull.Value)
            {
                shape.ToolTip = string.Format("{0}\n{1:########0.00}, {2}\nРанг по ФО:{3}",
                    dataRow["Region"],
                    dataRow["Value"],
                    "Единица измерения".ToLower(),
                    dataRow["RangFO"]);
            }
            else
            {
                shape.ToolTip = string.Format("{0}\n{1:### ### ##0.00}, {2}",
                    dataRow["Region"],
                    dataRow["Value"],
                    "Единица измерения");
            }
        }

        private void FillShape(Shape shape, DataRow dataRow)
        {
            if (shape == null)
            {
                return;
            }

            if (dataRow["Value"] == DBNull.Value)
            {
                return;
            }


            shape["Value"] = dataRow["Value"];
            shape.Text = string.Format("{0}\n{1:### ### ##0.##}", dataRow["Region"], dataRow["Value"]);

            shape.Visible = true;

            shape.TextVisibility = TextVisibility.Shown;

            SetShapeTooltip(shape, dataRow);
        }

                    

        decimal GetValueFromNameRegion(DataTable Table, string Region)
        {

            foreach (DataRow row in Table.Rows)
            {
                if (GetNormalName(row[0].ToString()).Contains(Region))
                {                    
                    return (decimal)row[1];
                }
            }

            return 0;
        }

        enum ShapeType
        {
            City,Region,City_out
        }

        ShapeType GetShapeType(string region)
        {
            if (region[0] == 'г')
            {
                if (region.Split('_').Length == 1)
                {
                    return ShapeType.City;
                }
                else
                {
                    return ShapeType.City_out;
                }
            }
            else
            {
                return ShapeType.Region;
            }
        }

        string GetNormalName(string str)
        {
            return str.Replace(" муниципальный район", String.Empty).Replace("Город ", "г.").Replace(" р-н", "").Replace(" район", "").Replace("г.", "");
        }

        string GetNormalDisplay(string str)
        {

            if (str[0] == 'г')
            {
                return str;//.Replace(" ", "");
            }
            else
            {

                return str + " р-н";
            }

        }

        protected void DataBindMap()
        {

            DataTable Map = GetDT("MAP","Region");

            foreach (Shape sh in DundasMap.Shapes)
            {
                double ValueShape = (double)GetValueFromNameRegion(Map, GetNormalName(sh.Name));

                sh["Value"] = ValueShape;

                ShapeType TypeShape = GetShapeType(sh.Name);

                string TollTips = string.Format("{0}\nДоля неэффективных расходов\n{1:### ##0.##}%",GetNormalName(sh.Name), ValueShape);

                sh.ToolTip = TollTips;
                if (ShapeType.City == TypeShape)
                {
                    sh.Text = string.Format("{0}\n{1:### ##0.##}%", GenshortName(sh.Name).Replace("\n",""), ValueShape);
                    sh.Visible = true;
                    sh.TextVisibility = TextVisibility.Shown;
                    sh.CentralPointOffset.Y -= 0.3;
                }
                else
                if (ShapeType.City_out == TypeShape)
                {                    
                    
                }
                else
                {
                    sh.Text = string.Format("{0}\n{1:### ##0.##}%", GenshortName(sh.Name).Replace("\n", ""), ValueShape);
                    sh.Visible = true;
                    sh.TextVisibility = TextVisibility.Shown;

                }

            }

            
        }

        #endregion

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
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "##0";
            }

            ExportGridToExcel(G, e.Workbook.Worksheets["Таблица"], 2, false);

            try
            {
                e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value =Label4.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0], C);
                //ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[0].Cells[0], DundasMap);

                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].Value = Label1.Text;
                ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[1].Cells[0], DundasMap);
            }
            catch { }

            int startrow = 4;

            for (int i = 0; i < G.Rows.Count; i++)
            {
                e.CurrentWorksheet.Rows[i + startrow].Height = 22 * 40;
                e.CurrentWorksheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                for (int j = 0; j < 15; j++)
                    try
                    {
                        e.CurrentWorksheet.Rows[i + startrow].Cells[j].Value = e.CurrentWorksheet.Rows[i + startrow].Cells[j].Value.ToString() + "";
                        double Value = double.Parse(e.CurrentWorksheet.Rows[i + startrow].Cells[j].Value.ToString());
                        if (e.CurrentWorksheet.Rows[2].Cells[j].Value.ToString() == "Ранг")
                        {
                            e.CurrentWorksheet.Rows[i + startrow].Cells[j].CellFormat.FormatString = "##0";
                        }
                        else
                        {
                            e.CurrentWorksheet.Rows[i + startrow].Cells[j].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                        }
                        e.CurrentWorksheet.Rows[i + startrow].Cells[j].Value =  Value;
                        //e.CurrentWorksheet.Rows[i + startrow].Cells[j].Value = string.Format("{0:########0.########}", Value);
                    }
                    catch { }

            }
        }



        private void SetSizeMapAndChart()
        {
            C.Width = (int)(C.Width.Value * 0.75);

            DundasMap.Width = (int)(C.Width.Value * 0.85);
            DundasMap.Height = (int)(DundasMap.Height.Value * 0.75);
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
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");

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
