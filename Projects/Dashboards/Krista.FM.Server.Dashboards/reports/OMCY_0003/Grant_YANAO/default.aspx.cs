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

namespace Krista.FM.Server.Dashboards.reports.OMCY_0003.Grant_YANAO
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

                    //  col.Width = onePercent * (100 - 63 - 5 -10) / (Grid.Columns.Count - 2);


                }
                //Grid.Columns[3].Width = onePercent * 10;                
                //Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    //  col.Width = onePercent * (100 - 65 - 5 -10) / (Grid.Columns.Count - 2);
                }
                //Grid.Columns[3].Width = onePercent * 10;
                //id.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    //col.Width = onePercent * (100 - 67 - 5-10) / (Grid.Columns.Count - 3);
                }
                //Grid.Columns[3].Width = onePercent * 10;
                //Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
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
                    
                  //  col.Width = onePercent * (100 - 63 - 5 -10) / (Grid.Columns.Count - 2);
                    

                }
                //Grid.Columns[3].Width = onePercent * 10;                
                //Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                  //  col.Width = onePercent * (100 - 65 - 5 -10) / (Grid.Columns.Count - 2);
                }
                //Grid.Columns[3].Width = onePercent * 10;
                //id.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    //col.Width = onePercent * (100 - 67 - 5-10) / (Grid.Columns.Count - 3);
                }
                //Grid.Columns[3].Width = onePercent * 10;
                //Grid.Columns[2].Width = onePercent * 10;
                Grid.Columns[1].Width = onePercent * 10;
                Grid.Columns[0].Width = onePercent * 25;
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

            GGO.Width = CustomizerSize.GetGridWidth();
            GGO.Height = Unit.Empty;

            GMR.Width = CustomizerSize.GetGridWidth();
            GMR.Height = Unit.Empty;            
        }

        private void FillCombo()
        {
            DataSetYearCombo = new DataSetComboLinear();
            DataSetYearCombo.LoadData(GetDT("AllYear", "Year"));


            DataTable Region = GetDT("RegionAll", "Year");
            Region.Rows[0].Delete();

            DataSetRegionCombo = new DataSetComboLinear();
            DataSetRegionCombo.LoadData(Region);


            if (!Page.IsPostBack)
            {
                YearCombo.FillDictionaryValues(DataSetYearCombo.DataForCombo);
                YearCombo.Title = "Период";
                YearCombo.Width = 150;

                


                DataTable dt = new DataTable();
            }
        }

        private void ChoseParam()
        {
            SelectCurrentYear = YearCombo.SelectedValue;
            SelectPrevYear = (int.Parse(YearCombo.SelectedValue) - 1).ToString();

            SelectYearCaption.Value = YearCombo.SelectedValue;

            SelectYearCaptionPrev.Value = SelectPrevYear;

            


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

        private void DisplayGrid(UltraWebGrid G)
        {

            G.Columns[0].Header.Caption = "Муниципальное образование";
            //G.Columns[1].Header.Caption = "Оценка эффективности деятельности ОМСУ";
            //CRHelper.FormatNumberColumn(G.Columns[1], "N3");

            //G.Columns[2].Header.Caption = "Место";
            //CRHelper.FormatNumberColumn(G.Columns[2], "### ### ##0");

            G.Columns[1].Header.Caption = "Размер гранта (% от общей суммы)";
            CRHelper.FormatNumberColumn(G.Columns[1], "### ### ##0.00%");

            foreach (UltraGridColumn Col in G.Columns)
            {
                Col.Header.Style.Wrap = true;
                Col.Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }

            CustomizerSize.ContfigurationGrid(G);

        }


        

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChoseParam();

            DataBindGrid();
            
            DisplayGrid(GGO);
            DisplayGrid(GMR);


            SetHeaderReport();
        }

        DataRow GetLiderFromTable(string field)
        {
            Field.Value = field;
            DataTable Table = GetDT("g", "Region");
            DataRow maxRow = Table.Rows[0];
            foreach (DataRow row in Table.Rows)
            {
                maxRow = (decimal)maxRow[1] >  (decimal)row[1] ? maxRow : row;
            }
            return maxRow;
        }

        private void InsertInTable(DataTable Table, DataRow AdeddRow)
        {
            foreach (DataRow Row in Table.Rows)
            {
                if (Row[0].ToString() == AdeddRow[0].ToString())
                {
                    Row["Grant"] = (decimal)((double)(decimal)Row["Grant"] + 0.3/4);
                    return;
                }
            }
            DataRow NewRow = Table.NewRow();
            Table.Rows.Add(NewRow); 
            NewRow["Region"] = AdeddRow["Region"];
            NewRow["Grant"] = 0.3 /4;
        }

        void addRowToTable(DataRow AdeddRow, DataTable MOTable, DataTable GOTable)
        {
            if (AdeddRow["Region"].ToString().Contains("муниципальный район"))
            {
                InsertInTable(MOTable, AdeddRow);
            }
            else
            {
                InsertInTable(GOTable, AdeddRow);
            }
        }

        

        private void DataBindGrid()
        {
            
             
            DataTable TableGridGO = new DataTable();

            TableGridGO.Columns.Add("Region");
            //TableGridGO.Columns.Add("Value",typeof(decimal));
            //TableGridGO.Columns.Add("Rang");
            TableGridGO.Columns.Add("Grant",typeof(decimal));

            DataTable TableGridMR = TableGridGO.Copy();

            DataRow Row = TableGridGO.NewRow();
            TableGridGO.Rows.Add(Row);

            CopyRow(Row, GetDT("Group1", "Region").Rows[0]);

            Row = TableGridGO.NewRow();
            TableGridGO.Rows.Add(Row);

            CopyRow(Row, GetDT("Group2", "Region").Rows[0]);

            Row = TableGridMR.NewRow();
            TableGridMR.Rows.Add(Row);

            CopyRow(Row, GetDT("Group3", "Region").Rows[0]);

            Row = TableGridMR.NewRow();
            TableGridMR.Rows.Add(Row);

            CopyRow(Row, GetDT("Group4", "Region").Rows[0]);

            addRowToTable(GetLiderFromTable("Организация муниципального управления и повышение инвестиционной привлекательности"), TableGridMR, TableGridGO);
            addRowToTable(GetLiderFromTable("Обеспечение здоровья"), TableGridMR, TableGridGO);
            addRowToTable(GetLiderFromTable("Образование"), TableGridMR, TableGridGO);
            addRowToTable(GetLiderFromTable("Жилищно-коммунальный комплекс"), TableGridMR, TableGridGO);

            //int Rang = 1;
            //foreach (DataRow BaseRow in BaseTable.Rows)
            //{
            //    DataRow NewRow =null;
            //    if (BaseRow[1].ToString() == "ГО")
            //    {
            //        NewRow = TableGridGO.NewRow();
            //        TableGridGO.Rows.Add(NewRow);
            //        NewRow["Region"] = BaseRow["Region"];
            //    }
            //    else
            //    {
            //        NewRow = TableGridMR.NewRow();
            //        TableGridMR.Rows.Add(NewRow);
            //        NewRow["Region"] = BaseRow["Region"] ;
            //    }
                
            //    NewRow["Value"] = BaseRow[2];
            //    NewRow["Grant"] = (Rang - 15.0) / (-60.0);
            //    NewRow["Rang"] = Rang++;
                
            //}

            GGO.DataSource = TableGridGO;
            GGO.DataBind();

            GMR.DataSource = TableGridMR;
            GMR.DataBind();
        }

        private void CopyRow(DataRow Row, DataRow dataRow)
        {
            Row["Region"] = dataRow["Region"];
            //Row["Value"] = dataRow[1];
            Row["Grant"] = 0.7 / 4;
        }

        


       

        private void SetHeaderReport()
        {

            Hederglobal.Text = "Распределение грантов";
            Page.Title = Hederglobal.Text;
            

            Label4.Text = string.Format("Распределение грантов по муниципальным районам за {0} год ", YearCombo.SelectedValue);

            Label5.Text = string.Format("Распределение грантов по городским округам за {0} год", YearCombo.SelectedValue);

            Label2.Text = "Порядок распределения грантов городским округам и муниципальным районам на основе оценки эффективности деятельности органов местного самоуправления";

            string GO_Comment = string.Format("Лучшими городскими округами по итогам проведенной оценки за {0} год признаны: ", YearCombo.SelectedValue);

            foreach (UltraGridRow row in GGO.Rows)
            {
                GO_Comment += row.Cells[0].Text + ", ";
            }

            Label6.Text = GO_Comment.Remove(GO_Comment.Length-2);


            string MR_Comment = string.Format("Лучшими муниципальными районами по итогам проведенной оценки за {0} год признаны: ", YearCombo.SelectedValue);
            foreach (UltraGridRow row in GMR.Rows)
            {
                MR_Comment += row.Cells[0].Text + ", ";
            }

            Label7.Text = MR_Comment.Remove(MR_Comment.Length - 2);

            Label2.Text = "Порядок распределения грантов городским округам и муниципальным районам на основе оценки эффективности деятельности органов местного самоуправления";

            //Label6.Text

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
        }

        private void FormatGridSheet(Worksheet w)
        {
            w.Rows[3].Cells[0].Value = "Показатели";

            w.Columns[1].Width = 100 * 40;
            w.Columns[0].Width = 180 * 40;
            w.Columns[2].Width = 100 * 40;
            w.Columns[3].Width = 100 * 40;
            

            SetStyleHeadertableFromExcel(w.Rows[3].Cells[0].CellFormat);
            SetStyleHeadertableFromExcel(w.Rows[3].Cells[1].CellFormat);
            //SetStyleHeadertableFromExcel(w.Rows[3].Cells[2].CellFormat);
            //SetStyleHeadertableFromExcel(w.Rows[3].Cells[3].CellFormat);
            

            w.Rows[3].Cells[0].Value = GGO.Columns[0].Header.Caption;
            w.Rows[3].Cells[1].Value = GGO.Columns[1].Header.Caption;
            //w.Rows[3].Cells[2].Value = GGO.Columns[2].Header.Caption; 
            //w.Rows[3].Cells[3].Value = GGO.Columns[3].Header.Caption;
            
            int startrow = 4;

            for (int i = 0; i < 4; i++)
            {
                //sheet.Rows[i + startrow].Height = 22 * 40;
                w.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                w.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                for (int j = 0; j < 10; j++)
                    try
                    {

                        //w.Rows[i + startrow].Cells[j].Value = w.Rows[i + startrow].Cells[j].Value.ToString() + "";
                        //double Value = double.Parse(w.Rows[i + startrow].Cells[j].Value.ToString().Replace(',', '.'));
                        if (w.Rows[startrow - 1].Cells[j].Value.ToString() == "Размер гранта (% от общей суммы)")
                        {
                            if (string.IsNullOrEmpty(w.Rows[i + startrow].Cells[j].Value.ToString()))
                            { }
                            else
                            {
                                //w.Rows[i + startrow].Cells[j].Value = w.Rows[i + startrow].Cells[j].Value.ToString() + "%";
                                w.Rows[i + startrow].Cells[j].CellFormat.FormatString = "### ### ##0.00%";
                            }
                        }

                        //sheet.Rows[i + startrow].Cells[j].Value = string.Format("{0:########0.########}", Value);
                        //w.Rows[i + startrow].Cells[j].Value = Value;
                    }
                    catch { }

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
            //foreach (UltraGridColumn col in G.Columns)
            //{
            //    if ((col.Hidden) || (col.Width.Value == 0))
            //    {
            //        G.Bands[0].HeaderLayout.Remove(col.Header);
            //        G.Columns.Remove(col);
            //    }
            //}
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица 1");

            Worksheet sheet2 = workbook.Worksheets.Add("Таблица 2");


            //Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

            CleraHidenColGrid();

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 5;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";


            ReportExcelExporter1.ExcelExporter.Export(GGO, sheet1);

            ReportExcelExporter1.ExcelExporter.Export(GMR, sheet2);
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
