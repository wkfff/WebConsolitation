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

namespace Krista.FM.Server.Dashboards.reports.OMCY_0003.REGION_Sahalin
{
    public partial class _default : CustomReportPage
    {

        bool RootScope = false;

        private CustomParam StartDetailScope { get { return (UserParams.CustomParam("StartDetailScope")); } }
        private CustomParam EndDetailScope { get { return (UserParams.CustomParam("EndDetailScope")); } }

        private CustomParam BC_R { get { return (UserParams.CustomParam("BC_R")); } }
        private CustomParam BC_C { get { return (UserParams.CustomParam("BC_C")); } }

        private CustomParam Scope { get { return (UserParams.CustomParam("Scope")); } }

        private CustomParam ScopeName { get { return (UserParams.CustomParam("ScopeName")); } }

        private CustomParam SelectYear { get { return (UserParams.CustomParam("SelectYear")); } }
        private CustomParam SelectYearCaption { get { return (UserParams.CustomParam("SelectYearCaption")); } }
        

        ICustomizerSize CustomizerSize;

        RadioButtons NaviagatorChart;

        DataTable TableListForChart = null;

        IDataSetCombo DataSetYearCombo;

        IDataSetCombo DataSetScopeCombo;

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
                int onePercent = 1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = 1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = 1024 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
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
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 10;
                }
                Grid.Columns[0].Width = onePercent * 15;
            }

            #endregion

        }
        #endregion

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
                    string UniqueName = row["UniqueName"].ToString();
                    string Child = row["Caption"].ToString();

                    this.DataForCombo.Add(Child, 0);

                    this.DataUniqeName.Add(Child, UniqueName);

                    this.addOtherInfo(Table, row, Child);
                }
            }
        }

        DataTable GetDT(string QID, string FirstColumn)
        {
            DataTable Table = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(
                DataProvider.GetQueryText(QID),
                FirstColumn, Table);
            return Table;
        }

        private void FillCombo()
        {
            DataSetYearCombo = new DataSetComboLinear();
            DataSetYearCombo.LoadData(GetDT("AllYear", "Caption"));
            //AllScope
            DataSetScopeCombo = new DataSetComboLinear();
            DataSetScopeCombo.LoadData(GetDT("AllScope", "Caption")); ;

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
            RootScope =
                "Оценка эффективности деятельности ОМСУ" == ScopeCombo.SelectedValue;

            Scope.Value = DataSetScopeCombo.OtherInfo[ScopeCombo.SelectedValue]["UniqueName"];

            SelectYear.Value = DataSetYearCombo.OtherInfo[YearCombo.SelectedValue]["UniqueName"];
            SelectYearCaption.Value = YearCombo.SelectedValue;
            //DataSetYearCombo.OtherInfo[YearCombo.SelectedValue]["UniqueName"];

            ScopeName.Value = "«" + ScopeCombo.SelectedValue + "»";

            BC_C.Value = "columns";
            BC_R.Value = "rows";

            CheckBox1.Visible = !RootScope;

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

            C.Height = 300+200;

            DundasMap.Width = CustomizerSize.GetChartWidth();
            DundasMap.Height = 700;

            UltraChart.Width = CustomizerSize.GetChartWidth();
            UltraChart.Height = 600;

        }

        #region Ранжирование

        void FormatGridRang()
        {

            bool TwoYear = false;
            for (int k = 0; k < G.Columns.Count; k++)
            {
                if (G.Columns[k].Header.Caption.Split(';')[0] == "null")
                {
                    bool reverce = double.Parse(G.Rows[2].Cells[k - 1].Value.ToString().Replace(" ", "")) == 0;
                    
                    SetRang(G, k - 1, k, 3, reverce);//double.Parse(G.Rows[2].Cells[k - 1].Value.ToString().Replace(" ", "")) == 0);                    
                    //continue;
                    for (int i = 0; i < G.Rows.Count; i++)
                    {
                        if ((G.Rows[i].Cells[0].Text == "г.Ханты-Мансийск") && (ScopeCombo.SelectedValue == "Обеспечение здоровья") && (k != 2))
                        {
                            continue;
                        }

                        int max_r = i;
                        int min_r = i;
                        for (int j = 0; j < G.Rows.Count; j++)
                        {
                            if ((G.Rows[j].Cells[0].Text == "г.Ханты-Мансийск") && (ScopeCombo.SelectedValue == "Обеспечение здоровья") && (k != 2))
                            {
                                continue;
                            }

                            try
                            {
                                if (reverce)
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
                                else
                                {
                                    if (G.Rows[i].Cells[k - (TwoYear ? 2 : 1)].Value.ToString() == G.Rows[j].Cells[k - (TwoYear ? 2 : 1)].Value.ToString())
                                    {

                                        if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) < System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString()))
                                        {
                                            max_r = j;
                                        }
                                        if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) > System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString()))
                                        {
                                            min_r = j;
                                        }

                                    }
                                }
                            }
                            catch { }
                        }
                        try
                        {
                            if (min_r != max_r)
                            {
                                string s = (reverce ? G.Rows[min_r].Cells[k].Value.ToString() + " - " + G.Rows[max_r].Cells[k].Value.ToString() : G.Rows[max_r].Cells[k].Value.ToString() + " - " + G.Rows[min_r].Cells[k].Value.ToString());
                                System.Decimal max_r_ = System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString());
                                System.Decimal min_r_ = System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString());
                                for (int j = 0; j < G.Rows.Count; j++)
                                {
                                    try
                                    {
                                        if (reverce)
                                        {
                                            if ((System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) <= max_r_) &&
                                                (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) >= min_r_))
                                            {
                                                G.Rows[j].Cells[k].Text = s;
                                            }
                                        }
                                        else
                                        {
                                            if ((System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) >= max_r_) &&
                                               (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) <= min_r_))
                                            {
                                                G.Rows[j].Cells[k].Text = s;
                                            }
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

        }

        void SetRang(UltraWebGrid dt, int ColVal, int ColRang, int StartRow, bool reverce)
        {
            int RowCount = dt.Rows.Count;
            int L_max = StartRow;
            int L_min = StartRow;
            int rang = 0;
            for (int i = StartRow; i < RowCount; i++)
            {
                if ((G.Rows[i].Cells[0].Text == "г.Ханты-Мансийск") && (ScopeCombo.SelectedValue == "Обеспечение здоровья") && (ColVal != 1))
                {
                    continue; ;
                }

                for (int j = StartRow; j < RowCount; j++)
                {
                    if ((G.Rows[j].Cells[0].Text == "г.Ханты-Мансийск") && (ScopeCombo.SelectedValue == "Обеспечение здоровья") && (ColVal != 1))
                    {
                        continue;
                    }



                    if (dt.Rows[j].Cells[ColVal].Value != null)
                    {
                        try
                        {
                            if (reverce)
                            {
                                if ((System.Decimal.Parse(dt.Rows[j].Cells[ColVal].Text) >= System.Decimal.Parse(dt.Rows[L_max].Cells[ColVal].Text)) && (dt.Rows[j].Cells[ColRang].Value == null))
                                {
                                    L_max = j;
                                }
                                if ((System.Decimal.Parse(dt.Rows[j].Cells[ColVal].Text) < System.Decimal.Parse(dt.Rows[L_min].Cells[ColVal].Text)) && (dt.Rows[j].Cells[ColRang].Value == null))
                                {
                                    L_min = j;
                                }
                            }
                            else
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
                        }
                        catch { }
                    }
                    else
                    {
                        //                        minys++;
                    }

                }
                if (true)
                {

                    if (dt.Rows[L_max].Cells[ColRang].Value == null)
                        dt.Rows[L_max].Cells[ColRang].Value = ++rang;


                }
                //else
                //{
                //    if (dt.Rows[L_min].Cells[ColRang].Value == null)
                //        dt.Rows[L_min].Cells[ColRang].Value = ++rang;
                //}


                L_max = L_min;
            }


        }
        #endregion

        #region DataBindGrid

        DataRow FindRow(DataTable Table, string FindString)
        {
            foreach (DataRow Row in Table.Rows)
            {
                if (Row[0].ToString() == FindString)
                {
                    return Row;
                }
            }
            DataRow NewRow = Table.NewRow();
            NewRow[0] = FindString;
            Table.Rows.Add(NewRow);
            return NewRow;
        }

        class NumberOfstruct
        {
            public int a;
            public int b;
            public int c;
            public bool HidenNum = false;

            public string ToStrSortFormat()
            {
                return string.Format("{0:#0}.{1:00}.{2:00}", a, b, c);
            }

            public string ToStrDisplayFormat()
            {
                if (HidenNum)
                {
                    //return "";
                }
                if (ParentB)
                {
                    return string.Format("{0:#0}", a);
                }

                if (ParentC)
                {
                    return string.Format("{0:#0}.{1:##}", a, b);
                }

                return string.Format("{0:#0}.{1:#0}.{2:##}", a, b, c);
            }

            public bool ParentC
            {
                get
                {
                    return c == 0;
                }
            }
            public bool ParentB
            {
                get
                {
                    return b == 0;
                }
            }

            public NumberOfstruct(string str)
            {
                string[] sp = str.Split('.');

                a = 0;
                b = 0;
                c = 0;

                if (sp.Length > 1)
                {
                    try
                    {
                        a = int.Parse(sp[0]);
                        b = int.Parse(sp[1]);
                        c = int.Parse(sp[2]);
                    }
                    catch { }
                    return;
                }
                else
                {
                    if (str.Length == 4)
                    { 
                        HidenNum = true;
                        a = int.Parse(str.Remove(2)); 
                        b = int.Parse(str.Remove(0, 2));
                    }
                    else
                    {
                        a = int.Parse(str[0] + "");

                        b = int.Parse(str.Remove(0, 1));
                        
                    }
                    c = 0;
                }

            }
        }

        string ClearDataMemberCaption(string str)
        {
            if (str[0] == '(')
            {
                return str.Replace("ДАННЫЕ)", "").Remove(0, 1);

            }
            return str;
        }

        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare_(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }



            public int Compare(DataRow x, DataRow y)
            {
                //String.Format(x[1].ToString() + "|" + y[1].ToString());
                
                decimal X = decimal.Parse(x[1].ToString().Replace(" ",""));
                decimal Y = decimal.Parse(y[1].ToString().Replace(" ", "")); ;
                if (X > Y)
                {
                    return -1;
                }

                if (X < Y)
                {
                    return 1;
                }

                return 0;
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort                 
                //= Table.Copy();
                = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }
            
            


            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();
            int i = 0;
            foreach (DataRow row in Table.Rows)
            {
                if (i > 2)
                {
                    LR.Add(row);
                }
                i++;
            }

            LR.Sort(new SortDataRow());

            TableSort.Rows.Add(Table.Rows[0].ItemArray);
            TableSort.Rows.Add(Table.Rows[1].ItemArray);
            TableSort.Rows.Add(Table.Rows[2].ItemArray);

            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }


        private void DataBindGrid()
        {
            G.Bands.Clear();

            DataTable MainTable = GetDT(RootScope ? "GRoot" : "G", "Region");

            DataTable BufTable = new DataTable();

            foreach (DataColumn col in MainTable.Columns)
            {
                if (col.ColumnName != "Code")
                {
                    DataColumn LastAdededCol = BufTable.Columns.Add(col.ColumnName, col.DataType);
                    foreach (DataRow row in MainTable.Rows)
                    {
                        DataRow dr = FindRow(BufTable, row[0].ToString());
                        dr[LastAdededCol] = row[col];
                    }
                }
                else
                {
                    DataColumn LastAdededCol = BufTable.Columns.Add(col.ColumnName, typeof(string));
                    foreach (DataRow row in MainTable.Rows)
                    {
                        DataRow dr = FindRow(BufTable, row[0].ToString());
                        dr[LastAdededCol] = row[col].ToString();
                    }
                }
            }

            MainTable = BufTable;
            //foreach (DataRow row in MainTable.Rows)
                
                foreach (DataRow row in MainTable.Rows)
                    if (!RootScope)
                    {
                        row[0] = row[0].ToString() + " в сфере «" + ScopeCombo.SelectedValue + "»";
                    }
                    else
                    {
                        if (!row[0].ToString().Contains("Оценка") && !row[0].ToString().Contains("Интегральный"))
                        {
                            row[0] = string.Format("Оценка эффективности деятельности ОМСУ в сфере «{0}»", row[0]);
                        } 
                    } 

            TableListForChart = MainTable.Copy();

            
            int startDetailField = RootScope ? 0 : int.Parse(exjectnumS(MainTable) + "");
            int endDetailField = startDetailField + 1;
            MainTable.Rows[1][1] = MainTable.Rows[0][1];
            if (!RootScope)
                MainTable.Rows[0].Delete();




            DataTable TableGrid = new DataTable();

            TableGrid.Columns.Add("Region");

            foreach (DataRow row in MainTable.Rows)
            {
                row[0] = ClearDataMemberCaption(row[0].ToString());
                TableGrid.Columns.Add(row[0].ToString());
                TableGrid.Columns.Add("null;" + row[0].ToString());

                for (int i = 1; i < MainTable.Columns.Count; i++)
                {
                    DataColumn col = MainTable.Columns[i];
                    DataRow RowVal = FindRow(TableGrid, col.ColumnName);
                    try
                    {
                        RowVal[row[0].ToString()] = string.Format("{0:### ### ##0.000}", decimal.Parse(row[col].ToString()));
                    }
                    catch
                    {
                        RowVal[row[0].ToString()] = row[col];
                    }
                }
            }

            TableListForChart = MainTable.Copy();

            if ((CheckBox1.Checked) & (!RootScope))
            {
                StartDetailScope.Value = string.Format("{0:00}",startDetailField);
                EndDetailScope.Value = string.Format("{0:00}",endDetailField);

                DataTable MainTableParent = MainTable;

                MainTable = GetDT("Grid_Detail", "Region");

                foreach (DataRow dr in MainTable.Rows)
                {
                    DataRow Row = MainTableParent.NewRow();
                    MainTableParent.Rows.Add(Row);
                    foreach (DataColumn col in MainTable.Columns)
                    {
                        Row[col.ColumnName] = dr[col];
                    }
                    //MainTableParent.Rows.Add(dr.ItemArray);
                }

                MainTable = MainTableParent;

                foreach (DataRow dr in MainTable.Rows)
                {
                    dr["Code"] = new NumberOfstruct(dr["Code"].ToString()).ToStrSortFormat();

                }

                DataRow[] drs = MainTable.Select("true", "Code");
                foreach (DataRow dr in drs)
                {
                    MainTable.Rows.Add(dr.ItemArray);
                    MainTable.Rows.Remove(dr);
                }

                foreach (DataRow dr in MainTable.Rows)
                {
                    dr["Code"] = new NumberOfstruct(dr["Code"].ToString()).ToStrDisplayFormat();

                }

                TableGrid = new DataTable();

                TableGrid.Columns.Add("Region");

                foreach (DataRow row in MainTable.Rows)
                {
                    row[0] = ClearDataMemberCaption(row[0].ToString());
                    TableGrid.Columns.Add(row[0].ToString());
                    TableGrid.Columns.Add("null;" + row[0].ToString());

                    for (int i = 1; i < MainTable.Columns.Count; i++)
                    {
                        DataColumn col = MainTable.Columns[i];
                        DataRow RowVal = FindRow(TableGrid, col.ColumnName);
                        try
                        {
                            if ((row["Code"].ToString().Length == 1) || 
                                (new NumberOfstruct(row["Code"].ToString()).ParentC))
                            {
                                RowVal[row[0].ToString()] = string.Format("{0:### ### ##0.000}", decimal.Parse(row[col].ToString()));
                            }
                            else
                            {
                                RowVal[row[0].ToString()] = string.Format("{0:### ### ##0.##}", row[col]);
                            }
                        }
                        catch
                        {

                            RowVal[row[0].ToString()] = row[col];
                        }
                    }
                }

            }
            foreach (DataColumn colname in TableGrid.Columns)
            {
                colname.ColumnName = colname.ColumnName.Replace('"', '_');
                colname.ColumnName = colname.ColumnName.Replace("'", "_");
                colname.ColumnName = colname.ColumnName.Replace("\n", "_");
                colname.Caption = colname.ColumnName.Replace('"', '_');
                colname.Caption = colname.ColumnName.Replace("'", "_");
                colname.Caption = colname.ColumnName.Replace("\n", "_"); 
            }
            G.DataSource = 
                //TableGrid;
                SortTable(TableGrid);
            G.DataBind();
        }

        private static string exjectnumS(DataTable MainTable)
        {
            if (MainTable.Rows[0][1].ToString().Length == 3)
            {
                return MainTable.Rows[0][1].ToString()[0] + "";
            }

            string s = MainTable.Rows[0][1].ToString().Remove(2);
            try
            {
                s = s.Remove(2);
            }
            catch { }
            return s;
        }
        #endregion

        #region DisplayGrid
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

        string ParserOfStruct(string s)
        {
            if (s.Split('.').Length > 1)
            {
                return s;
            }
            else
            {
                try
                {
                    s = s.Replace(" ", "");
                    s = s.Split(',')[0];
                    if (s.Length == 4)
                    {
                        //узри же порядок редукций!
                        return s.Insert(2, ".").Replace("10", "1$").Replace("0", "").Replace("1$", "10");
                    }
                    else
                    { 
                    return s[0] + "." + (s[2] == '0' ? "" : (s[2] + ""));
                    }
                }
                catch { }
                return s;

            }
        }

        string FormatHeder(string s)
        {
            return s.Replace("\"", "``").Replace("'", "``").Replace("Уровень эффективности деятельности ОМСУ", "Оценка эффективности деятельности ОМСУ").Replace("ДАННЫЕ)","").Replace("(","");                   
        }

        private void FormatHeaderGrid()
        {
            
            ConfHeader(G.Columns[0].Header, "Территория", 0, 0, 1, 2);
            for (int i = 1; i < G.Columns.Count; i += 2)
            {


                ColumnHeader HeaderValue = G.Columns[i].Header;
                ColumnHeader HeaderRang = G.Columns[i + 1].Header;

                G.Bands[0].HeaderLayout.Add(
                    CreateHeader(
                    (RootScope ? ((i / 2) > 2 ? ((i / 2)-2).ToString() + "." : "") : 
                    ParserOfStruct(G.Rows[0].Cells[i].Text)) 
                    +                    
                    " "
                    + FormatHeder(HeaderValue.Caption)
                    , i, 0, 2, 1));

                 
                CRHelper.FormatNumberColumn(G.Columns[i], "### ##0.##");
                CRHelper.FormatNumberColumn(G.Columns[i + 1], "### ##0.##");

                ConfHeader(HeaderValue, G.Rows[1].Cells[i].Text, i, 1, 1, 1);
                ConfHeader(HeaderRang, "Место", i + 1, 1, 1, 1);
            }
            G.Columns[0].CellStyle.Wrap = true;

        }

        private void FormatRow()
        {
            foreach (UltraGridRow Row in G.Rows)
            {
                string MO = Row.Cells[0].Text;
                //if (MO[0] != 'г')
                //{
                //    Row.Cells[0].Text += " район";
                //}
            }
        }

        private void DisplayGrid()
        {
            
            FormatGridRang();

            FormatHeaderGrid();

            CustomizerSize.ContfigurationGrid(G);

            SetImageGrid();

            FormatRow();

            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;

            G.Rows[0].Hidden = true;
            G.Rows[1].Hidden = true;
            G.Rows[2].Hidden = true;
        }



        int GetMaxRowFromCol(UltraWebGrid dt, int col)
        {
            int MaxIndex = 3;
            for (int i = 3; i < dt.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(G.Rows[i].Cells[col + 1].Text))
                    try
                    {
                        if (null != dt.Rows[i].Cells[col].Value)
                            if (System.Decimal.Parse(dt.Rows[i].Cells[col].Text) >
                                System.Decimal.Parse(dt.Rows[MaxIndex].Cells[col].Text))
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
            int MaxIndex = 3;
            for (int i = 3; i < dt.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(G.Rows[i].Cells[col + 1].Text))
                    try
                    {
                        if (null != dt.Rows[i].Cells[col].Value)
                            if (System.Decimal.Parse(dt.Rows[i].Cells[col].Text) <
                                System.Decimal.Parse(dt.Rows[MaxIndex].Cells[col].Text))
                            {
                                MaxIndex = i;
                            }
                    }
                    catch { }

            }
            return MaxIndex;
        }


        private void SetImageGrid()
        {
            for (int i = 2; i < G.Columns.Count - 1; i += 2)
            {
                string MaxRegion = (((ScopeCombo.SelectedValue == "Обеспечение здоровья") && (i != 2)) ? "21" : "22");

                for (int j = 3; j < G.Rows.Count; j++)
                {
                    UltraGridRow row = G.Rows[j];
                    if (!string.IsNullOrEmpty(row.Cells[i].Text))
                    {
                        if ((row.Cells[i].Text == "1 - " + MaxRegion))
                        {
                            if (((decimal.Parse(G.Rows[2].Cells[i - 1].Text.Replace(" ", "")) == 0)))
                            {
                                if (((decimal.Parse(G.Rows[3].Cells[i - 1].Text) == 0)))
                                {
                                    row.Cells[i].Title = "Наилучшее значение по субъекту РФ";
                                    row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else
                                {
                                    row.Cells[i].Title = "Наилучшее значение по субъекту РФ";
                                    row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }
                            else
                            {
                                if (((decimal.Parse(G.Rows[3].Cells[i - 1].Text) == 0)))
                                {
                                    row.Cells[i].Title = "Наилучшее значение по субъекту РФ";
                                    row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else
                                {
                                    row.Cells[i].Title = "Наилучшее значение по субъекту РФ";
                                    row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }

                            }

                        }
                        else
                        {

                            if ((row.Cells[i].Text == "1") || (row.Cells[i].Text.Contains("1-")) || (row.Cells[i].Text.Contains("1 -")))
                            {
                                row.Cells[i].Title = "Наилучшее значение по субъекту РФ";
                                row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            }
                            if ((row.Cells[i].Text == MaxRegion) || row.Cells[i].Text.Contains("-" + MaxRegion) || row.Cells[i].Text.Contains("- " + MaxRegion))
                            {
                                row.Cells[i].Title = "Наихудшее значение по субъекту РФ";
                                row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            }
                        }
                    }
                }
            }
        }


        #endregion

        DataTable TableChart = null;

        private void DataBindChart_1()
        {
            #region Bind
            if (NaviagatorChart.ActiveRow == null)
            {
                NaviagatorChart.ActiveRow = TableListForChart.Rows[0];
                foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
                {
                    rb.Checked = true;
                    break;
                }

            }

            DataRow CopyRow = NaviagatorChart.ActiveRow;

            TableChart = new DataTable();

            TableChart.Columns.Add("Region", typeof(string));

            TableChart.Columns.Add("Значение", typeof(decimal));

            for (int i = 4; i < TableListForChart.Columns.Count; i++)
            {
                try
                {
                    object[] o = new object[2] { TableListForChart.Columns[i].ColumnName, 
                    decimal.Parse(CopyRow[TableListForChart.Columns[i].ColumnName].ToString()) };

                    o[0] = o[0].ToString().Replace("муниципальный район", "м-р").Replace("Городской округ ","").Replace("городской округ","").Replace("\"",""); 
                    TableChart.Rows.Add(o);
                }
                catch { }

            }
            DataRow[] drs = TableChart.Select("true", "Значение");
            foreach (DataRow dr in drs)
            {
                TableChart.Rows.Add(dr.ItemArray);
                TableChart.Rows.Remove(dr);
            }

            C.DataSource = TableChart;

            C.Data.SwapRowsAndColumns = false;

            C.DataBind();
            #endregion

            #region conf
            string[] ActiveCaption = NaviagatorChart.ActiveRow[0].ToString().Split(' ');
            C.Tooltips.FormatString = "<SERIES_LABEL>\n" + ActiveCaption[0] + " " + ActiveCaption[1] + ":<b><DATA_VALUE:### ### ##0.##></b>";

            C.Axis.X.Extent = 200;
            #endregion
        }

        DataTable dtChart = null;

        bool ShowTop = true;

        private void DataBindChart_2()
        {
            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.Border.Thickness = 0;
            // UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.Margin.Near.Value = 3;
            UltraChart.Axis.X.Margin.Far.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 7;
            UltraChart.Axis.Y.Margin.Far.Value = 7;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 45;
            UltraChart.Axis.Y.Labels.Visible = true;

            UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;            
            

            UltraChart.Axis.Y2.Visible = true;
            UltraChart.Axis.Y2.Extent = 65;
            UltraChart.Axis.Y2.LineThickness = 0;
            UltraChart.Axis.Y2.Margin.Near.Value = 10;
            UltraChart.Axis.Y2.Margin.Far.Value = 10;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ##0.#>";
            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 23;


            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<b> <DATA_VALUE:N2></b>";
            UltraChart.Tooltips.Display = TooltipDisplay.MouseMove;
            UltraChart.ColorModel.ModelStyle = ColorModels.Office2007Style;
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            AddLineAppearencesUltraChart1();


            #endregion

            #region Bind
            string query = DataProvider.GetQueryText(RootScope ? "C2Root" : "C2");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Columns[2].DataType.Name != "String")
            {
                dtChart.Columns.Remove(dtChart.Columns[0]);
                ShowTop = 1 == 1;
                UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                //UltraChart.Axis.X.Extent = 60;
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {
                    dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString();//, SetBr(10);
                    for (int j = 0; j < dtChart.Columns.Count; j++)
                    {
                        if (dtChart.Rows[i][j] == DBNull.Value)
                        {
                            // dtChart.Rows[i].Delete();
                        }
                    }
                }
                foreach(DataColumn col in dtChart.Columns)
                {
                    col.ColumnName = SetBr(col.ColumnName,20);
                }
            }
            else
            {
                UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
                UltraChart.Axis.X.Extent = 180;
                ShowTop = 1 == 2;
                DataTable dt = new DataTable();
                dt.Columns.Add("aa");
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    dt.Columns.Add(dtChart.Rows[0][i].ToString(), typeof(System.Decimal));
                }
                object[] o = new object[dt.Columns.Count];
                for (int i = 1; i < dtChart.Rows.Count; i++)
                {
                    dt.Rows.Add(dtChart.Rows[i].ItemArray);
                }
                dtChart = dt;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][0] = SetCavuch(dt.Rows[i][0].ToString());
                }
            }

            foreach (DataRow Row in dtChart.Rows)
            {
                Row[0] = Row[0].ToString().Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", "");
            }

            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName = col.ColumnName.Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", "");
            }

            UltraChart.DataSource = dtChart;
            UltraChart.DataBind();

            #endregion
        }

        private void DataBindChart()
        {
            DataBindChart_1();

            DataBindChart_2();
        }


        void rb_CheckedChanged(object sender, EventArgs e)
        {
            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];

            DataBindChart_1();

            ConfigurationMap();

            SetHeaderReport();
        }

        #region NavigatorRadioBox

        class RadioButtons
        {
            public Dictionary<RadioButton, DataRow> Buttons = null;

            DataTable Table = null;

            string Scope = "";

            public DataRow ActiveRow = null;

            public EventHandler EventClick = null;

            void ConfigurateButton(RadioButton rb)
            {
                rb.AutoPostBack = true;
                rb.GroupName = this.ToString();// +" в сфере «" + Scope + "»";

                rb.Font.Name = "Verdana";
                rb.Font.Size = 10;

                rb.CheckedChanged += new EventHandler(EventClick);
            }

            string FormatHeder(string s)
            {
                return s.Replace("\"", "``").Replace("'", "``").Replace("Уровень эффективности деятельности ОМСУ", "Оценка эффективности деятельности ОМСУ").Replace("ДАННЫЕ)", "").Replace("(", "");
            }


            void Bind()
            {
                Buttons = new Dictionary<RadioButton, DataRow>();
                foreach (DataRow Row in Table.Rows)
                {
                    RadioButton RB = new RadioButton();
                    this.ConfigurateButton(RB);
                    RB.Text = FormatHeder(Row[0].ToString());
                    Buttons.Add(RB, Row);
                }
            }

            public void FillPLaceHolder(PlaceHolder pl)
            {
                bool b = false; ;
                foreach (RadioButton rb in Buttons.Keys)
                {
                    Label br = new Label();

                    if (b)
                    {
                        br.Text = "<br>";
                    }

                    b = true;

                    pl.Controls.Add(br);
                    pl.Controls.Add(rb);
                }
            }

            public RadioButtons(DataTable Table, EventHandler EventC, string Scope)
            {
                this.EventClick = EventC;
                this.Table = Table;
                Bind();
            }
        }

        #endregion

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

        string SetBr(string s, int limit)
        {
            for (int i = 0; i < s.Length; i += limit)
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

        protected void AddLegend()
        {
            DundasMap.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            //legend1.Title = string.Format("{0}:\n{1}", ComboDirection.SelectedNodeParent, SetHyphen(ComboDirection.SelectedValue));
            string[] spl = new string[1] { " в " };
            legend1.Title = SetBr(NaviagatorChart.ActiveRow[0].ToString().Split(spl, StringSplitOptions.None)[0], 20);//"Доля неэффективных\n расходов";

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
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
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

        decimal GetValueFromNameRegion(string Region)
        {

            foreach (UltraGridRow Row in G.Rows)
            {
                
                if (GetNormalName(Row.Cells[0].Text).Contains(GetNormalName(Region)))
                {
                    try
                    {
                        return decimal.Parse(Row.Cells.FromKey(NaviagatorChart.ActiveRow[0].ToString()).Text);
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }

            //return thri;
            throw new Exception();
        }

        string GetRangFromNameRegion(string Region)
        {

            foreach (UltraGridRow Row in G.Rows)
            {
                if (Row.Cells[0].Text == Region)
                {
                    try
                    {
                        return Row.Cells[Row.Cells.FromKey(NaviagatorChart.ActiveRow[0].ToString()).Column.Index + 1].Text;
                    }
                    catch
                    {
                        return "";
                    }
                }
            }

            return "";
        }

        enum ShapeType
        {
            City, Region, City_out
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

        protected void DataBindMap()
        {
            foreach (Shape sh in DundasMap.Shapes)
            {
                try
                {
                    double ValueShape = (double)GetValueFromNameRegion(GetNormalName(sh.Name));
                    string RangShape = (string)GetRangFromNameRegion(GetNormalName(sh.Name));

                    sh["Value"] = ValueShape;


                    string[] ActiveCaption = NaviagatorChart.ActiveRow[0].ToString().Split(' ');
                    string Caption = "" + ActiveCaption[0] + " " + ActiveCaption[1];

                    ShapeType TypeShape = GetShapeType(sh.Name);

                    string TollTips = string.Format("{0}\n{2}: {1:### ##0.##}", GetNormalName(sh.Name), ValueShape, Caption);

                    sh.ToolTip = TollTips;
                    if (ShapeType.City == TypeShape)
                    {
                        sh.Text = string.Format("{0} {1:### ##0.##}", (sh.Name.ToString().Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", "")), ValueShape);
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
                            sh.Text = string.Format("{0} {1:### ##0.##}", (sh.Name).ToString().Replace("муниципальный район", "м-р").Replace("Городской округ ", "").Replace("городской округ", "").Replace("\"", ""), ValueShape);
                            sh.Visible = true;
                            sh.TextVisibility = TextVisibility.Shown;

                        }
                }
                catch { }

            }


        }

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e); 

            FillCombo();

            ChoseParam();

            DataBindGrid();

            DisplayGrid();
            //FormatListChart(TableListForChart)
            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged, ScopeCombo.SelectedValue);

            NaviagatorChart.FillPLaceHolder(PlaceHolder1);

            DataBindChart();

            ConfigurationMap();

            SetHeaderReport();
        }

        private DataTable FormatListChart(DataTable TableListForChart)
        {
            foreach (DataRow row in TableListForChart.Rows)
            {
                row[0] = FormatHeder(row[0].ToString());
            }
            return TableListForChart;
        }

        #region SetStar

        protected void SetStar(UltraWebGrid G, int Col, int RowBaseVaslue, string Star, string Title)
        {
            for (int i = 0; G.Rows.Count > i; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(G.Rows[i].Cells[Col + 1].Text))
                        if (G.Rows[i].Cells[Col].Value.ToString() == G.Rows[RowBaseVaslue].Cells[Col].Value.ToString())
                        {
                            G.Rows[i].Cells[Col + 1].Title = Title;
                            G.Rows[i].Cells[Col + 1].Style.BackgroundImage = Star;
                            G.Rows[i].Cells[Col + 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                }
                catch { }
            }


        }

        #endregion

        private void SetHeaderReport()
        {

            Hederglobal.Text = "Оценка эффективности деятельности органов местного самоуправления ";

            Page.Title = Hederglobal.Text;
            Label2.Text = "Оценка проведена в соответствии с распоряжением Правительства Российской Федерации от 11 сентября 2008 г. № 1313-р, в редакции  распоряжения Правительства Российской Федерации от 15 мая 2010 г. № 758-р (с учетом изменений)";

            Label3.Text = string.Format("Оценка эффективности деятельности органов местного самоуправления в {0} году", YearCombo.SelectedValue);


            Label4.Text = string.Format("Распределение территорий по значению показателя «{0}»", NaviagatorChart.ActiveRow[0].ToString().Replace('«', '"').Replace('»', '"'));
            Label1.Text = FormatHeder(NaviagatorChart.ActiveRow[0].ToString());

            Label6.Visible = false;//(ScopeCombo.SelectedValue == "Обеспечение здоровья");
        }

        #region CrazyChart %)

        string SetCavuch(string s)
        {
            bool b = 2 == 1;
            string ns = "";
            for (int i = 0; i < s.Length; i++)
            {

                char b_ = s[i];
                if (b_ == '"')
                {
                    b = !b;
                    b_ = b ? '«' : '»';

                }

                ns += b_;

            }
            return ns;
        }

        private void AddLineAppearencesUltraChart1()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 22; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                    case 8:
                        {
                            color = Color.Yellow;
                            break;
                        }
                    case 9:
                        {
                            color = Color.YellowGreen;
                            break;
                        }
                    case 10:
                        {
                            color = Color.Tomato;
                            break;
                        }
                    case 11:
                        {
                            color = Color.Sienna;
                            break;
                        }
                    case 12:
                        {
                            color = Color.SandyBrown;
                            break;
                        }
                    case 13:
                        {
                            color = Color.Salmon;
                            break;
                        }
                    case 14:
                        {
                            color = Color.RosyBrown;
                            break;
                        }
                    case 15:
                        {
                            color = Color.Purple;
                            break;
                        }
                    case 16:
                        {
                            color = Color.Orchid;
                            break;

                        }
                    case 17:
                        {
                            color = Color.Moccasin;
                            break;
                        }
                    case 18:
                        {
                            color = Color.MediumSeaGreen;
                            break;
                        }
                    case 19:
                        {
                            color = Color.Linen;
                            break;
                        }
                    case 20:
                        {
                            color = Color.LightCoral;
                            break;
                        }
                    case 21:
                        {
                            color = Color.Khaki;
                            break;
                        }
                    case 22:
                        {
                            color = Color.Indigo;
                            break;
                        }

                }
                pe.Fill = color;
                pe.StrokeWidth = 0;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;
                LineAppearance lineAppearance2 = new LineAppearance();

                lineAppearance2.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;

                UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

                UltraChart.LineChart.Thickness = 0;
            }
        }

        private int GetMaxRowIndex(string col)
        {
            int result = 0;
            double value = 0;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][col] != DBNull.Value)
                    if (value < Convert.ToDouble(dtChart.Rows[i][col]))
                    {
                        value = Convert.ToDouble(dtChart.Rows[i][col]);
                        result = i;
                    }
            }
            return result;
        }

        private int GetMinRowIndex(string col)
        {
            int result = 0;
            double value = 10;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][col] != DBNull.Value)
                    if (value > Convert.ToDouble(dtChart.Rows[i][col]))
                    {
                        value = Convert.ToDouble(dtChart.Rows[i][col]);
                        result = i;
                    }
            }
            return result;
        }

        private double GetValue(int row, string col)
        {
            return Convert.ToDouble(dtChart.Rows[row][col].ToString());
        }

        private string GetRegion(int row)
        {
            return dtChart.Rows[row][0].ToString();
        }

        private bool PrimitiveIsAxisXLabel(Primitive p)
        {
            if (p is Text)
            {
                if (p.Path != null)
                {
                    if (p.Path.Contains("Grid.X"))
                    {
                        return true;
                    }

                }
            }
            return false;
        }


        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            //return;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                
                

                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null &&
                    primitive.Path.Contains("Grid.X"))
                {
                    Text t = (Text)primitive;
                    if (t.GetTextString().Contains("Александровск-Сахалинский р"))
                    {
                        //t.labelStyle.HorizontalAlign = StringAlignment.Near;
                        t.labelStyle.VerticalAlign = StringAlignment.Near;
                        t.bounds.Width *= 2;
                    }
                }

                for (int j = i + 1; j < e.SceneGraph.Count; j++)
                {
                    Infragistics.UltraChart.Core.Primitives.PointSet p;

                    if (e.SceneGraph[i] is Box)
                    {
                        Box b = (Box)e.SceneGraph[i];
                        if (b.rect.Height == b.rect.Width)
                        {
                            if (e.SceneGraph[j] is Box)
                            {
                                Box b2 = (Box)e.SceneGraph[j];
                                if (b2.rect.Height == b2.rect.Width)
                                {
                                    if ((b.rect.X == b2.rect.X) & (Math.Abs(b.rect.Y - b2.rect.Y) <= 4))
                                    {
                                        b2.rect.X += b2.rect.Width + 1;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            

            if (!ShowTop)
            {
                return;
            }


            Text lasttext = null;
            foreach (Primitive p in e.SceneGraph)
            {
                if (PrimitiveIsAxisXLabel(p))
                {
                    lasttext = (Text)p;
                    Text cur = (Text)p;
                    cur.bounds.Height *= 2;
                }
            }

            lasttext.bounds.X -= 125/5;

            //
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            Collection<string> indicators = new Collection<string>();
            Collection<string> maxRegions = new Collection<string>();
            Collection<double> maxValue = new Collection<double>();
            Collection<string> minRegions = new Collection<string>();
            Collection<double> minValue = new Collection<double>();
            Collection<int> leftBound = new Collection<int>();
            Collection<int> leftWidth = new Collection<int>();

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {

                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null &&
                    primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;                    

                    int maxRowIndex = GetMaxRowIndex(text.GetTextString());
                    int minRowIndex = GetMinRowIndex(text.GetTextString());

                    indicators.Add(text.GetTextString());
                    maxRegions.Add(GetRegion(maxRowIndex));
                    maxValue.Add(GetValue(maxRowIndex, text.GetTextString()));

                    minRegions.Add(GetRegion(minRowIndex));
                    minValue.Add(GetValue(minRowIndex, text.GetTextString()));

                    leftBound.Add(text.bounds.X);
                    leftWidth.Add(text.bounds.Width);
                }
            }

            for (int i = 0; i < indicators.Count    ; i++)
            {

                if (i == indicators.Count - 1)
                {
                    //leftBound[i] -= 125;
                }

                Text newMaxText = new Text();
                newMaxText.labelStyle.Font = new Font("Verdana", 8);




                newMaxText.PE.Fill = Color.Black;
                newMaxText.bounds = new Rectangle(leftBound[i], (int)yAxis.Map(maxValue[i]) - 20, leftWidth[i], 15);
                newMaxText.labelStyle.VerticalAlign = StringAlignment.Center;
                newMaxText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMaxText.SetTextString(RegionsNamingHelper.ShortName(maxRegions[i]));
                e.SceneGraph.Add(newMaxText);


                Text newMinText = new Text();
                newMinText.labelStyle.Font = new Font("Verdana", 8);
                newMinText.PE.Fill = Color.Black;
                newMinText.bounds = new Rectangle(leftBound[i], (int)yAxis.Map(minValue[i]) + 10, leftWidth[i], 15);
                newMinText.labelStyle.VerticalAlign = StringAlignment.Center;
                newMinText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMinText.SetTextString(RegionsNamingHelper.ShortName(minRegions[i]));
                e.SceneGraph.Add(newMinText);
            }
        }

        


        #endregion

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if ((RadioButton)(sender) == RadioButton1)
            {
                BC_C.Value = "columns";
                BC_R.Value = "rows";
            }
            else
            {
                BC_C.Value = "rows";
                BC_R.Value = "columns";

            }
            DataBindChart_2();



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

            ExportGridToExcel(G, e.Workbook.Worksheets["Таблица"], 2, false);

            try
            {
                e.Workbook.Worksheets["Диаграмма 1"].Rows[0].Cells[0].Value = Label4.Text;
                //e.Workbook.Worksheets["Диаграмма 2"].Rows[0].Cells[0].Value = Label4.Text;
                e.Workbook.Worksheets["Диаграмма 2"].Rows[0].Cells[0].Value = Label6.Text;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].Value = Label1.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0], C);
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0], UltraChart);
                ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[1].Cells[0], DundasMap);
            }
            catch { }
        }



        private void SetSizeMapAndChart()
        {
            C.Width = (int)(C.Width.Value * 0.75);

            UltraChart.Width = (int)(C.Width.Value * 0.75);

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
            workbook.Worksheets.Add("Диаграмма 1");
            workbook.Worksheets.Add("Диаграмма 2");
            workbook.Worksheets.Add("Карта");

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
