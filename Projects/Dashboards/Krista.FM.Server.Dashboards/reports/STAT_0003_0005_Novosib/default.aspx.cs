using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
 
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0005_Novosib
{
    public partial class Default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;

        IDataSetCombo DataSetComboDay;

        RadioButtons NaviagatorChart;

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            public abstract int GetMapHeight();

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
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 15;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
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
                    col.Width = onePercent * (100 - 24) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 16;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }
            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }
        #endregion

        DataBaseHelper DBHelper;
        DataBaseHelper DBRFHelper;

        CustomParam SelectMapAndChartParam { get { return (UserParams.CustomParam("SelectMapAndChartParam")); } }
        CustomParam SelectMapAndChartParamRF { get { return (UserParams.CustomParam("SelectMapAndChartParamRF")); } }

        CustomParam ChosenCurPeriodRF { get { return (UserParams.CustomParam("ChosenCurPeriodRF")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
        CustomParam ChosenPrevPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPrevPeriod")); } }

        class DataBaseHelper
        {
            public DataProvider ActiveDP;

            public DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, QueryId);
            }

            public DataTable ExecQueryByID(string QueryId, string FirstColName)
            {
                string QueryText = DataProvider.GetQueryText(QueryId);
                DataTable Table = ExecQuery(QueryText, FirstColName);
                return Table;
            }

            public DataTable ExecQuery(string QueryText, string FirstColName)
            {
                DataTable Table = new DataTable();
                ActiveDP.GetDataTableForChart(QueryText, FirstColName, Table);
                return Table;
            }

            public DataBaseHelper(DataProvider ActiveDataProvaider)
            {
                this.ActiveDP = ActiveDataProvaider;
            }

        }

        #region ComboClass
        abstract class IDataSetCombo
        {
            public enum TypeFillData
            {
                Linear, TwoLevel
            }

            public int LevelParent = 2;

            public string LastAdededKey { get; protected set; }
            public string PrevLastAdededKey { get; protected set; }

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
                this.PrevLastAdededKey = LastAdededKey;
                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);

        }

        class SetComboDay : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LaseYear = "";
                string LastMounth = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string PUniqueName = row["PUniqueName"].ToString();                    

                    string Day = row["Day"].ToString();

                    string Mounth = row["Mounth"].ToString();

                    string Year = row["Year"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(Day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(Mounth)) + " " + Year + " года", "");
                                                            
                    if (LaseYear != Year)
                    {
                        this.AddItem(Year + " год", 0, UniqueName);
                        this.AddItem(Mounth + " " + Year + " года", 1, PUniqueName + ".[Данные месяца]");
                        LastMounth = "";
                    }
                    else

                    if (LastMounth != Mounth)
                    {
                        this.AddItem(Mounth + " " + Year + " года", 1, PUniqueName + ".[Данные месяца]");
                    }


                    LaseYear = Year;
                    LastMounth = Mounth;

                    this.AddItem(DisplayNAme, 2, UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
                }
            }
        }

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DBHelper = new DataBaseHelper(DataProvidersFactory.PrimaryMASDataProvider);

            DBRFHelper = new DataBaseHelper(DataProvidersFactory.SecondaryMASDataProvider);

            #region ini CustomizerSize
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }
            else

                if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
                {
                    CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
                }
                else
                {
                    CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
                }

            //Grid.Width = CustomizerSize.GetGridWidth();
            //Grid.Height = 600;
            DundasMap1.Width = CustomizerSize.GetChartWidth();
            DundasMap1.Height = 600;

            UltraChart1.Width = CustomizerSize.GetChartWidth();
            UltraChart1.Height = 500;
            
            #endregion

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //ReportExcelExporter1.ExcelExportButton.Visible = false;

            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion

            UltraChart1.Border.Color = Color.Transparent;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart1_InvalidDataReceived);
        }

        void SelectFirstChild()
        {
            for (; ComboCurrentPeriod.SelectedNode.Nodes.Count > 0; )
            {
                ComboCurrentPeriod.SetСheckedState(ComboCurrentPeriod.SelectedNode.Nodes[0].Text, true);
            }
        }

        private void FillComboPeriod()
        {
            ComboCurrentPeriod.ParentSelect = false;
            ComboCurrentPeriod.Title = "Выберите дату";
            ComboCurrentPeriod.Width = 300;

            DataTable Table = DBHelper.ExecQueryByID("ComboDates", "Day");

            DataSetComboDay = new SetComboDay();
            DataSetComboDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurrentPeriod.FillDictionaryValues(DataSetComboDay.DataForCombo);
                ComboCurrentPeriod.SetСheckedState(DataSetComboDay.LastAdededKey, 1 == 1);
            }
            
            SelectFirstChild();
        }
        

        private void FillCombo()
        {
            FillComboPeriod();
        }

        class CoolDate
        {
            public DateTime dt;
            bool DataMounth = false;

            public CoolDate(DateTime dt)
            {
                this.dt = dt;
            }

            public CoolDate(string MDXName)
            {
                this.dt = ConvertToDateTime(MDXName);
            }

            private DateTime ConvertToDateTime(string MDXName)
            {
                string[] firstsplit = MDXName.Replace("[", "").Replace("]", "").Split('.');
                int Year = int.Parse(firstsplit[3]);
                int Mounth = CRHelper.MonthNum(firstsplit[6]);
                int Day = 1;
                try
                {
                    Day = int.Parse(firstsplit[7]);
                }
                catch {
                    DataMounth = true;
                }
                return new DateTime(Year, Mounth, Day);

            }

            public string ConvertToMDXName()
            {
                return string.Format(@"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                    dt.Year,
                CRHelper.HalfYearNumByMonthNum(dt.Month),
                CRHelper.QuarterNumByMonthNum(dt.Month),
                DataMounth?"Данные месяца":CRHelper.RusMonth(dt.Month),
                dt.Day);
            }

            public string ReportDate()
            {
                if (DataMounth)
                {
                    return CRHelper.RusMonth(dt.Month) + " " + dt.Year.ToString(); 
                        //string.Format("{0:00}.{1:00}", dt.Month, dt.Year);
                }
                else
                {
                    return string.Format("{0:00}.{1:00}.{2:00}", dt.Day, dt.Month, dt.Year);
                }
            }

            public string ConvertToMDXNameRF()
            {
                DateTime dt_ = this.dt.AddMonths(-1);
                return string.Format(@"[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                    dt_.Year,
                CRHelper.HalfYearNumByMonthNum(dt_.Month),
                CRHelper.QuarterNumByMonthNum(dt_.Month),
                CRHelper.RusMonth(dt_.Month));
            }

        }

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode == null)
            {
                if (n.Parent.PrevNode == null)
                {
                    return null;
                }
                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
            }
            return n.PrevNode;

        }
        CoolDate CurDate;
        CoolDate CompareDate;

        #region GetCompareDate
        private CoolDate ConstructPrevDate(CoolDate cd)
        {
            return new CoolDate(cd.dt.AddMonths(-1));
        }

        private void GetCompareDate()
        {
            
            CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboCurrentPeriod.SelectedValue]);
            //CompareDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboComparePeriod.SelectedValue]);
            //if (ComboComparePeriod.SelectedIndex > ComboCurrentPeriod.SelectedIndex)
            //{
            //    CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboComparePeriod.SelectedValue]);
            //    CompareDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboCurrentPeriod.SelectedValue]);
            //}
        }
        #endregion

        private void SetQueryParam()
        {
            ChosenCurPeriod.Value = CurDate.ConvertToMDXName();
            //ChosenPrevPeriod.Value = CompareDate.ConvertToMDXName();
            ChosenCurPeriodRF.Value = CurDate.ConvertToMDXNameRF();
        }
        class InfoRow
        {
            public string ReportDisplay;
            public string FormatString;
            public string Hint = "";
            public bool Revece;
            public bool ReveceRang;
            public bool Rang;
            public Color BeginColor;
            public Color CenterColor;
            public Color EndColor;
            public string RFString;
            public string Caption;
            public string EdIzm;

            public InfoRow(string BaseName)
            { 
                switch (BaseName)
                {
                    case "Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте":
                        {
                            ReportDisplay = "Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте";

                            Caption = "Уровень регистрируемой безработицы";
                            EdIzm = "% от численности трудоспособного населения в трудоспособном возрасте";

                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;
                            return;
                        }

                    case "Уровень напряженности на рынке труда":
                        {
                            Caption = "Уровень напряженности на рынке труда";
                            EdIzm = "ед.";
                            ReportDisplay = "Уровень напряженности на рынке труда, ед.";
                            FormatString = "N2";
                            Rang = true;
                            Revece = true;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;  
                            EndColor = Color.Red;
                            return;
                        }

                    case "Средняя продолжительность безработицы":
                        {
                            Caption = "Средняя продолжительность безработицы";
                            EdIzm = "мес.";
                            ReportDisplay = "Средняя продолжительность безработицы, мес.";
                            FormatString = "N2";
                            Rang = true;
                            Revece = true;
                            ReveceRang = false; 
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;
                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";
                            return;
                        }

                    case "Уровень трудоустройства":
                        {
                            Caption = "Уровень трудоустройства (с начала года)";
                            EdIzm = "%";
                            ReportDisplay = "Уровень трудоустройства (с начала года), %";
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = false;
                            BeginColor = Color.Red;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Green;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";
                            return;
                        }
                    case "Число заявленных вакансий":
                        {
                            ReportDisplay = "Число заявленных вакансий";
                            EdIzm = "ед.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = false;
                            return;
                        }
                    
                };
            }
        }

        class CoolTable : DataTable
        {
            public DataRow AddRow()
            {
                DataRow NewRow = this.NewRow();
                this.Rows.Add(NewRow);
                return NewRow;
            }

            public DataRow GetRowFormValueFirstCell(string caption)
            {
                foreach (DataRow Row in this.Rows)
                {
                    if (Row[0].ToString() == caption)
                    {
                        return Row;
                    }
                }
                return null;
            }

            public DataRow AddOrGetRow(string caption)
            {
                DataRow Row = this.GetRowFormValueFirstCell(caption);
                if (Row != null)
                {
                    return Row;
                }

                DataRow NewRow = this.AddRow();
                NewRow[0] = caption;
                return NewRow;
            }


        }

        private void SetHeader()
        {
            PageTitle.Text = "Мониторинг ситуации на рынке труда (рейтинг территорий)";
            PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга ситуации на рынке труда {1} по состоянию на {0}", CurDate.ReportDate(),"Новосибирской области");
            Page.Title = PageTitle.Text;

            LabelMap2.Text = string.Format("«{0}», {1} на {2}", GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm, CurDate.ReportDate());

            LabelChart1.Text = string.Format("Распределение территорий по показателю «{0}», {1} на {2}",                               GlobalNavigationInfoRow.Caption, 
                GlobalNavigationInfoRow.EdIzm, 
                CurDate.ReportDate());  
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



            void Bind()
            {
                Buttons = new Dictionary<RadioButton, DataRow>();
                foreach (DataRow Row in Table.Rows)
                {
                    RadioButton RB = new RadioButton();
                    this.ConfigurateButton(RB);
                    RB.Text = Row[0].ToString();
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

            public RadioButtons(DataTable Table, EventHandler EventC)
            {
                this.EventClick = EventC;
                this.Table = Table;
                Bind();
            }
        }

        #endregion

        private void FillNaviagatorPanel()
        {
            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте]";
            TableListForChart.AddOrGetRow("Уровень напряженности на рынке труда, ед.")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень напряженности на рынке труда]";
            TableListForChart.AddOrGetRow("Средняя продолжительность безработицы, мес.")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Средняя продолжительность безработицы]";

            TableListForChart.AddOrGetRow("Уровень трудоустройства (с начала года), %")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень трудоустройства]";

            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);
            NaviagatorChart.FillPLaceHolder(PlaceHolder1);
        }

        InfoRow GlobalNavigationInfoRow;

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];
            ChangeMapAndChartParam();
            GenerateMap();
            GenChart();
            SetHeader();
        }

        #region Map
        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
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
        Legend l = null;

        private void CustomaizeMapko()
        {
            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = true;
            DundasMap1.NavigationPanel.Visible = true;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.Zoom = 100;
            DundasMap1.ColorSwatchPanel.Visible = false;

            DundasMap1.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            l = legend;
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Left;
            legend.DockAlignment = DockAlignment.Far;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;  
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;

            legend.Title = SetBr(NaviagatorChart.ActiveRow[0].ToString(), 30);
                //"Уровень регистрируемой\nбезработицы";
            legend.AutoFitMinFontSize = 7;

            // добавляем легенду с символами
            Legend legend2 = new Legend("SymbolLegend");
            
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Right;
            legend2.DockAlignment = DockAlignment.Far;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Общая численность\n зарегистрированных безработных граждан";
            legend2.AutoFitMinFontSize = 7;

            DundasMap1.Legends.Add(legend2);
            DundasMap1.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CostRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Cost";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;

            rule.FromColor = GlobalNavigationInfoRow.BeginColor;
            rule.MiddleColor = GlobalNavigationInfoRow.CenterColor;
            rule.ToColor = GlobalNavigationInfoRow.EndColor;

            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInLegend = "CostLegend";
            rule.LegendText = "#FROMVALUE{N1}% - #TOVALUE{N1}%";
            DundasMap1.ShapeRules.Add(rule);

            // добавляем поля
            DundasMap1.Shapes.Clear();
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("Cost");
            DundasMap1.ShapeFields["Cost"].Type = typeof(double);
            DundasMap1.ShapeFields["Cost"].UniqueIdentifier = false;

            string mapFolderName = "Субъекты\\Новособл";

            DundasMap1.Layers.Clear();
            //AddMapLayer(DundasMap1, mapFolderName, "Водные", CRHelper.MapShapeType.WaterObjects);
            AddMapLayer(DundasMap1, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            AddMapLayer(DundasMap1, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
            AddMapLayer(DundasMap1, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            #endregion
        }

        enum ShapeType
        {
            CallOutCity, MO, City
        }

        string GetUniversalRegionName(string Name)
        {
            return Name.Replace("_callout","").Replace("Город ", "г. ").Replace("муниципальный район", "район").Replace(" область", "").Replace("Республика ", "").Replace(" обл.", "").Replace(" автономный округ", "").Replace(" край", "").Replace("Р. ", "");
        }

        ShapeType GetShapeType(Shape shape)
        {
            if (shape.Name.Contains("_callout"))
            {
                return ShapeType.CallOutCity;
            }
            if (shape.Name.Contains("район"))
            {
                return ShapeType.MO;
            }
            return ShapeType.City;
        }

        DataRow GetShapeValue(DataTable BaseTable, Shape Shape)
        {
            string ShapeName = Shape.Name;
            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                if (GetUniversalRegionName(BaseRow["Region"].ToString()) == GetUniversalRegionName(ShapeName))
                {
                    if (BaseRow[0] != DBNull.Value)
                    {
                        return BaseRow;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        string GetMapDisplayRegionName(string Name)
        {
            return Name.Replace("муницыпальный ", "").Replace("_callout", "");
        }

        DataTable TableChart = null;

        #region Ранжировщик
        class RankingField
        {
            class SortKeyVal : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val > y.Val)
                    {
                        return -1;
                    }
                    if (x.Val < y.Val)
                    {
                        return 1;
                    }
                    return 0;
                }

                #endregion
            }

            class SortKeyValReverce : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val < y.Val)
                    {
                        return -1;
                    }
                    if (x.Val > y.Val)
                    {
                        return 1;
                    }
                    return 0;
                }

                #endregion
            }

            struct KeyVal
            {
                public string Key;
                public decimal Val;
            }

            List<KeyVal> Fields = new List<KeyVal>();

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            public string GetMinRang()
            {
                KeyVal Min = Fields[0];
                foreach (KeyVal kv in Fields)
                {
                    if (Min.Val < kv.Val)
                    {
                        Min = kv;
                    }
                }
                return Min.Key;
            }

            public string GetMaxRang()
            {
                KeyVal Max = Fields[0];
                foreach (KeyVal kv in Fields)
                {
                    if (Max.Val > kv.Val)
                    {
                        Max = kv;
                    }
                }

                return Max.Key;
            }

            public object GetRang(string Key, bool reverce)
            {
                if (reverce)
                    Fields.Sort(new SortKeyVal());
                else
                    Fields.Sort(new SortKeyValReverce());

                int i = 0;
                foreach (KeyVal kv in Fields)
                {
                    i++;

                    if (kv.Key.Split(';')[0] == Key.Split(';')[0])
                    {
                        return i;
                    }
                }
                return DBNull.Value;
            }

        }
        #endregion

        private void GenerateRangInTable(DataTable TableMap, bool Reverce)
        {
            RankingField Rk = new RankingField();

            TableMap.Columns.Add("Rang");
            foreach (DataRow Row in TableMap.Rows)
            {
                Rk.AddItem(Row[0].ToString(), (decimal)Row[1]);
            }

            foreach (DataRow Row in TableMap.Rows)
            {
                Row["Rang"] = Rk.GetRang(Row[0].ToString(),Reverce);
            }
        }

        public void FillMap1Data()
        {
            string query = DataProvider.GetQueryText("STAT_0003_0001_map1");
            DataTable TableMap = DBHelper.ExecQueryByID("Map", "Region");

            if (TableMap.Rows.Count < 1)
            {                 
                l.Visible = false;
            }

            TableChart = TableMap;
            InfoRow Info = new InfoRow(UserComboBox.getLastBlock(NaviagatorChart.ActiveRow[1].ToString()));

            GenerateRangInTable(TableMap,Info.ReveceRang);

            string Hint = @"{0} \n{1:" + Info.FormatString + "}, {2}\nРанг по Новосибирской области: {3}";

            foreach (Shape shape in DundasMap1.Shapes)
            {
                try
                {
                    DataRow ShapeRow = GetShapeValue(TableMap, shape);

                    string ShapeReportName = shape.Name.Replace("_callout", "");

                    ShapeType TypeShape = GetShapeType(shape);

                    shape.ToolTip = string.Format(Hint,
                        ShapeReportName,
                        //NaviagatorChart.ActiveRow[0].ToString(),
                        //CurDate.ReportDate(),
                        ShapeRow[1],
                        Info.EdIzm,
                        ShapeRow["Rang"]);
                    shape.TextVisibility = TextVisibility.Shown;
                    if (TypeShape == ShapeType.City)
                    {   
                        shape.TextVisibility = TextVisibility.Hidden;
                    }
                    
                    shape.Text = string.Format("{0}\n {1:" + Info.FormatString + "}", ShapeReportName, ShapeRow[1]);
                    shape["Cost"] = double.Parse(ShapeRow[1].ToString());
                    //shape.Text = ShapeReportName;
                    if (TypeShape == ShapeType.MO)
                    {

                    }
                }
                catch { }
            }          
        }

        

        private void GenerateMap()
        {
            CustomaizeMapko();
            FillMap1Data();
        }

        #endregion

        CustomParam LastSelectComboBox { get { return (UserParams.CustomParam("_")); } }

        DataRow GetFirstDictRow()
        {
            foreach (DataRow Row in NaviagatorChart.Buttons.Values)
            {
                return Row;
            }
            return null;
        }

        private void ChangeMapAndChartParam()
        {
            if (!Page.IsPostBack)
            {
                if (NaviagatorChart.ActiveRow == null)
                {
                    NaviagatorChart.ActiveRow = GetFirstDictRow();
                    foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
                    {
                        rb.Checked = true;
                        break;
                    }
                }
            }
            else
            {
                if (NaviagatorChart.ActiveRow == null)
                {
                    foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
                    {
                        if (rb.Text == LastSelectComboBox.Value)
                        {
                            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[rb]; 
                        }
                    }
                }
            }

            LastSelectComboBox.Value = NaviagatorChart.ActiveRow[0].ToString();
            SelectMapAndChartParam.Value = NaviagatorChart.ActiveRow["UName"].ToString();
            
            GlobalNavigationInfoRow = new InfoRow(UserComboBox.getLastBlock(SelectMapAndChartParam.Value));
            SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
        }

        DataTable SortDataTable(DataTable Table)
        {
            DataTable SortTable = new DataTable();
            foreach (DataColumn col in Table.Columns)
            {
                SortTable.Columns.Add(col.ColumnName, col.DataType);
            }
            DataRow[] Rows = Table.Select("", "Value ASC");

            foreach (DataRow Row in Rows)
            {
                SortTable.Rows.Add(Row.ItemArray);
            }
            SortTable.Columns.Remove(SortTable.Columns[0]);
            return SortTable;
        }

        DataTable TableFOValue = null;

        private void GenChart()
        {   


            UltraChart1.ChartType = ChartType.ColumnChart;
            TableChart = SortDataTable(TableChart);

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart1_InvalidDataReceived);
            if (TableChart.Rows.Count <= 0)
            {
                UltraChart1.DataSource = null;
                UltraChart1.DataBind();
                try
                {
                    UltraChart1.InvalidDataReceived -= new ChartDataInvalidEventHandler(UltraChart1_InvalidDataReceived);
                }
                catch { }
                return;
            }

            decimal LastValuyFromChart = (decimal)TableChart.Rows[TableChart.Rows.Count - 1][0];

            DataTable TableFOValue = DBHelper.ExecQueryByID("ChartFOLine");

            decimal FoValue = (decimal)TableFOValue.Rows[0][1];

            LastValuyFromChart = LastValuyFromChart > FoValue ? LastValuyFromChart : FoValue;

            UltraChart1.Axis.Y.RangeMax = (double)LastValuyFromChart * 1.1;
            UltraChart1.Axis.Y.RangeMin = 0;
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;

            UltraChart1.DataSource = TableChart;
            UltraChart1.DataBind();

            UltraChart1.Border.Color = Color.Transparent;

            UltraChart1.Axis.X.Extent = 200;
            UltraChart1.Axis.Y.Extent = 60;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 9);

            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Axis.X.Margin.Far.Value = 2;
            UltraChart1.Axis.X.Margin.Near.Value = 2;

            UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart1.ColorModel.ColorBegin = Color.LimeGreen;
            UltraChart1.ColorModel.ColorEnd = Color.Lime;
            UltraChart1.Tooltips.FormatString = string.Format("<SERIES_LABEL><br><b><DATA_VALUE:N2></b>, {0}", 
                //SetBr(GlobalNavigationInfoRow.ReportDisplay,35) 
                GlobalNavigationInfoRow.EdIzm
                //CurDate.ReportDate()
                );
            
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);
        }

        void UltraChart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        #region Линии на диограмке
        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 3;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text(); 
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 11, FontStyle.Italic);
            textLabel.labelStyle.FontColor = Color.Black;
            //textLabel.PE.f

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 17, 900, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX+ 50, StartY + 1, 900, 15);
            }


            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);


        }


        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                if (TableChart.Rows.Count < 1)
                    return;
                    
                

                

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];



                double FOStartLineX = 0;
                double FOStartLineY = 0;
                double FOEndLineX = 0;
                double FOEndLineY = 0;
                string FOheader = "";

                TableFOValue = DBHelper.ExecQueryByID("ChartFOLine");
                try
                {
                    if (TableFOValue.Rows[0]["Value"] != DBNull.Value)
                    {
                        FOheader = string.Format("{0}: {1:N2}, {2}", "Новосибирская обл.", TableFOValue.Rows[0]["Value"],GlobalNavigationInfoRow.EdIzm);

                        FOStartLineX = xAxis.Map(xAxis.Minimum);
                        FOStartLineY = yAxis.Map((double)(decimal)TableFOValue.Rows[0]["Value"]);

                        FOEndLineX = xAxis.Map(xAxis.Maximum);
                        FOEndLineY = yAxis.Map((double)(decimal)TableFOValue.Rows[0]["Value"]);
                    }
                }
                catch { }

                
                

                DataTable TableRFValue = DBRFHelper.ExecQueryByID("ChartRFLine");
                double RFStartLineX = 0;
                double RFStartLineY = 0;

                double RFEndLineX = 0;
                double RFEndLineY = 0;
                string RFheader = "";
                try
                {
                    if (TableRFValue.Rows[0][1] != DBNull.Value)
                    {
                        RFheader = string.Format("{0}: {1:N2}, {2}", "Российская федерация", TableRFValue.Rows[0][1],
                            GlobalNavigationInfoRow.EdIzm);


                        RFStartLineX = xAxis.Map(xAxis.Minimum);
                        RFStartLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);

                        RFEndLineX = xAxis.Map(xAxis.Maximum);
                        RFEndLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);


                    }

                }
                catch { }
                


                bool RFUP = true;

                bool FOUP = true;

                if ((Math.Abs(FOStartLineY - RFStartLineY) < 22))
                {
                    RFUP = RFStartLineY < FOStartLineY;

                    FOUP = FOStartLineY < RFStartLineY;

                }
                
                if (!string.IsNullOrEmpty(RFheader))
                {
                    if ((NaviagatorChart.ActiveRow[0].ToString() != "Средняя продолжительность безработицы, мес.")&&
                    (NaviagatorChart.ActiveRow[0].ToString() != "Уровень трудоустройства (с начала года), %"))
                    
                    {
                        GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                                Color.Red, RFheader, e, RFUP);
                    }
                }

                if (!string.IsNullOrEmpty(FOheader))
                {
                    GenHorizontalLineAndLabel((int)FOStartLineX, (int)FOStartLineY, (int)FOEndLineX, (int)FOEndLineY,
                            Color.Blue, FOheader, e, FOUP);
                }
            }
            catch { }

        }

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            
                base.Page_Load(sender, e);
                FillCombo();
                GetCompareDate();
                SetQueryParam();

                //GenerateGrid();           

                FillNaviagatorPanel();

                ChangeMapAndChartParam();

                GenerateMap();

                GenChart();

                SetHeader();

                DundasMap1.RenderType = RenderType.InteractiveImage;
        }

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            //ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            

            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);


            title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            //title = section2.AddText();
            //font = new Font("Verdana", 12);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

            //GridHeaderLayout headerLayout = GenExportLayot();

            //ReportPDFExporter1.Export(headerLayout, section1);

            ///section2.PageSize = new PageSize(section2.PageSize.Height, section1.PageSize.Width);
            DundasMap1.Height = 600;
            DundasMap1.Height = 600; 
            ReportPDFExporter1.Export(DundasMap1,LabelMap2.Text,section2);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section3);
        }

               #endregion

        #region Экспорт в Excel

        void MoveTitle(Worksheet w)
        {
            w.Rows[0].Cells[0].Value = w.Rows[2].Cells[0].Value;
            w.Rows[0].Cells[0].Value = "";

        }

        void ClearTitle(Worksheet w)
        {
            w.Rows[0].Cells[0].Value = "";
            w.Rows[1].Cells[0].Value = "";
            w.Rows[2].Cells[0].Value = "";
            w.Rows[3].Cells[0].Value = "";
            w.Rows[1].Cells[0].Value = "";
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            //ReportExcelExporter1.TitleStartRow = 10;
            ReportExcelExporter1.SheetColumnCount = 15;

            Workbook workbook = new Workbook();
            Worksheet sheet4 = workbook.Worksheets.Add("Карта");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");            

            DundasMap1.Width = 800;
            DundasMap1.Height = 550;
            UltraChart1.Width = 800;

            ReportExcelExporter1.Export( UltraChart1, LabelChart1.Text, sheet2, 3);
            ReportExcelExporter1.Export(DundasMap1, LabelMap2.Text, sheet4, 4);

            ClearTitle(sheet2);
            MoveTitle(sheet2);
            sheet2.Rows[0].Cells[0].Value = LabelChart1.Text;

            sheet4.Columns[0].Width = 100 * 1000;
            sheet2.Columns[0].Width = 100 * 3000;
            sheet2.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
                //Font = new Font("Verdana", 10);
        }

        private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
        {
            //WorksheetCell cell = sheet.Rows[9 + row * 3].Cells[column];
            //cell.CellFormat.Font.Name = "Verdana";
            //cell.CellFormat.Font.Height = 200;
            //cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.TopBorderColor = Color.Black;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.BottomBorderColor = Color.Black;

            //cell = sheet.Rows[10 + row * 3].Cells[column];
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.TopBorderColor = Color.Black;

            //cell = sheet.Rows[11 + row * 3].Cells[column];
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.TopBorderColor = Color.Black;
        }

        private void SetCellFormat(WorksheetCell cell)
        {
            //cell.CellFormat.Font.Name = "Verdana";
            //cell.CellFormat.Font.Height = 200;
            //cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.TopBorderColor = Color.Black;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            //e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            //e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            //e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            //e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            //e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            //string exportFontName = "Verdana";
            //int fontSize = 10;
            //double coeff = 1.0;
            //foreach (UltraGridColumn column in grid.Columns)
            //{
            //    //column.Width = Convert.ToInt32(column.Width.Value * coeff);
            //    column.CellStyle.Font.Name = exportFontName;
            //    column.Header.Style.Font.Name = exportFontName;
            //    column.CellStyle.Font.Size = fontSize;
            //    column.Header.Style.Font.Size = fontSize;
            //}
        }

        #endregion
    }
}