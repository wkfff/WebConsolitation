using System;

using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using System.Collections.Generic;


using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Primitives;
using Dundas.Maps.WebControl;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using System.IO;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebNavigator;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;


namespace Krista.FM.Server.Dashboards.ORG_0001_0010
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetRegion;



        DataSetComboPeriod SetComboPeriod = new DataSetComboPeriod();

        ICustomizerSize CustomizerSize;

        CustomParam Period { get { return (UserParams.CustomParam("Period")); } }

        CustomParam PeriodWith { get { return (UserParams.CustomParam("PeriodWith")); } }

        CustomParam PeriodRF { get { return (UserParams.CustomParam("PeriodRF")); } }

        CustomParam PeriodRFWith { get { return (UserParams.CustomParam("PeriodRFWith")); } }

        CustomParam ChosenField { get { return (UserParams.CustomParam("ChosenField")); } }

        CustomParam ChosenFieldWith { get { return (UserParams.CustomParam("ChosenFieldWith")); } }

        CustomParam ChosenFieldRF { get { return (UserParams.CustomParam("ChosenFieldRF")); } }

        CustomParam ChosenFieldRFWith { get { return (UserParams.CustomParam("ChosenFieldRFWith")); } }


        CustomParam ChosenRegionGrid { get { return (UserParams.CustomParam("ChosenRegionGrid")); } }



        CustomParam SelectField { get { return (UserParams.CustomParam("SelectField")); } }
        CustomParam SelectLastRow { get { return (UserParams.CustomParam("=)")); } }
        CustomParam lastActiveIndex { get { return (UserParams.CustomParam("lastActiveIndex")); } }

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
                        return 730;
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
                        return 730;
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
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 6) / (13);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
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
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 6) / (13);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }
        #endregion

        static class DataBaseHelper
        {
            public static DataProvider ActiveDataPorvider = DataProvidersFactory.SpareMASDataProvider;

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
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "");

                    this.AddItem(DisplayNAme, int.Parse(row["LevelNumber"].ToString()), UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
                }
            }
        }


        class DataSetComboPeriod : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LYear = "";


                foreach (DataRow Row in Table.Rows)
                {
                    string m = Row["Mounth"].ToString();
                    string y = Row["Year"].ToString();
                    string uname = Row["Uname"].ToString();

                    if (y != LYear)
                    {
                        this.AddItem(y + " год", 0, "");
                        LYear = y;
                    }

                    m = GetAlowableAndFormatedKey(m + " " + y + " года", "").ToLower();

                    this.AddItem(m, 1, uname);
                    this.addOtherInfo(Table, Row, m);
                }


            }

        }
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

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

            Grid.Width = CustomizerSize.GetGridWidth();
            Grid.Height = Unit.Empty;

            ChartLine.Width = CustomizerSize.GetChartWidth();
            ChartLine.Height = 400;

            ComboCurDay.Width = 250;



        }

        DataTable GetTableMOField()
        {
            #region d:\TEMP\ExcelParse.py

            DataTable Table1 = new DataTable();
            Table1.Columns.Add("Значение параметра", typeof(string));
            Table1.Columns.Add("Показатели, используемые в отчете", typeof(string));
            Table1.Columns.Add("имя в очтёте", typeof(string));
            Table1.Rows.Add("к предыдущему периоду", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на все товары и услуги к предыдущему периоду]", "Всего:");
            Table1.Rows.Add("к предыдущему периоду", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на продовольственные товары к предыдущему периоду]", "продовольственные товары");
            Table1.Rows.Add("к предыдущему периоду", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на непродовольственные товары к предыдущему периоду]", "непродовольственные товары");
            Table1.Rows.Add("к предыдущему периоду", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на услуги к предыдущему периоду]", "платные услуги");
            Table1.Rows.Add("к декабрю предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на все товары и услуги за период с начала года]", "Всего:");
            Table1.Rows.Add("к декабрю предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на продовольственные товары за период с начала года]", "продовольственные товары");
            Table1.Rows.Add("к декабрю предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на непродовольственные товары за период с начала года]", "непродовольственные товары");
            Table1.Rows.Add("к декабрю предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на услуги за период с начала года]", "платные услуги");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на все товары и услуги к соответствующему периоду предыдущего года]", "Всего:");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на продовольственные товары к соответствующему периоду предыдущего года]", "продовольственные товары");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на непродовольственные товары к соответствующему периоду предыдущего года]", "непродовольственные товары");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на услуги к соответствующему периоду предыдущего года]", "платные услуги");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на все товары и услуги за период с начала отчетного года в % к соответствующему периоду предыдущего года]                      ", "Всего:");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на продовольственные товары за период с начала отчетного года в % к соответствующему периоду предыдущего года]", "продовольственные товары");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на непродовольственные товары за период с начала отчетного года в % к соответствующему периоду предыдущего года]", "непродовольственные товары");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на услуги за период с начала отчетного года в % к соответствующему периоду предыдущего года]", "платные услуги");






            return Table1;

            #endregion
        }


        DataTable GetRFTable()
        {
            #region d:\TEMP\ExcelParse.py
            DataTable Table1 = new DataTable();
            Table1.Columns.Add("Значение параметра", typeof(string));
            Table1.Columns.Add("Показатели, используемые в отчете", typeof(string));
            Table1.Columns.Add("имя в очтёте", typeof(string));
            Table1.Rows.Add("к предыдущему периоду", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен, в % к предыдущему периоду]", "Всего:");
            Table1.Rows.Add("к предыдущему периоду", " [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на продовольственные товары в % к предыдущему периоду]", "продовольственные товары");
            Table1.Rows.Add("к предыдущему периоду", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на непродовольственные товары в % к предыдущему периоду]", "непродовольственные товары");
            Table1.Rows.Add("к предыдущему периоду", " [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен (тарифов) на услуги в % к предыдущему периоду]", "платные услуги");
            Table1.Rows.Add("к декабрю предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен]", "Всего:");
            Table1.Rows.Add("к декабрю предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на продовольственные товары]", "продовольственные товары");
            Table1.Rows.Add("к декабрю предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на непродовольственные товары]", "непродовольственные товары");
            Table1.Rows.Add("к декабрю предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен (тарифов) на услуги]", "платные услуги");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен в % к соответствующему периоду предыдущего года]", "Всего:");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на продовольственные товары в % к соответствующему периоду предыдущего года]", "продовольственные товары");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на непродовольственные товары в % к соответствующему периоду предыдущего года]", "непродовольственные товары");
            Table1.Rows.Add("к аналогичному периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен (тарифов) на услуги в % к соответствующему периоду предыдущего года]", "платные услуги");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен нарастающим итогом с начала года]", "Всего:");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на продовольственные товары нарастающим итогом с начала года]", "продовольственные товары");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен на непродовольственные товары нарастающим итогом с начала года]", "непродовольственные товары");
            Table1.Rows.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[Данные всех источников].[СТАТ\\0001 Отчетность - вариант: Без варианта].[Цены и тарифы].[Индекс потребительских цен (тарифов) на услуги нарастающим итогом с начала года]", "платные услуги");
            return Table1;
            #endregion
        }

        private string genAllField()
        {
            DataTable TableField = GetTableMOField();
            string result = "";

            foreach (DataRow row in TableField.Rows)
            {
                result += "\n" + row["Показатели, используемые в отчете"].ToString() + ",";
            }

            return result.Remove(result.Length - 1);
        }

        Node GetlastParent(CustomMultiCombo combo)
        {
            return combo.GetLastNode(0);
        }


        private void FillComboPeriod()
        {
            Period.Value = genAllField();

            ComboCurDay.MultiSelect = true;
            ComboCurDay.MultipleSelectionType = MultipleSelectionType.CascadeMultiple;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboPeriod");

            SetComboPeriod.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurDay.FillDictionaryValues(SetComboPeriod.DataForCombo);
                //ComboCurDay.SelectLastNode();
                //ComboCurDay.SetAllСheckedState(true, false); 
                ComboCurDay.SetСheckedState(GetlastParent(ComboCurDay).Text, true);

            }


        }

        private void FillComboComareType()
        {

            if (!Page.IsPostBack)
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                dict.Add("к предыдущему периоду", 0);
                dict.Add("к декабрю предыдущего года", 0);
                dict.Add("к аналогичному периоду предыдущего года", 0);
                dict.Add("период с начала отчетного года в % к соответствующему периоду предыдущего года", 0);

                ComboTypeCompare.FillDictionaryValues(dict);
            }

        }


        private void InsertUList(List<Node> Nods, Node n)
        {
            if (!Nods.Contains(n))
            {
                Nods.Add(n);
            }
        }

        private void AddAllChildNode(Node n, List<Node> Nods)
        {
            InsertUList(Nods, n);
            foreach (Node child in n.Nodes)
            {
                AddAllChildNode(child, Nods);
            }
        }

        private void ConfCombo()
        {
            int d = CustomizerSize is CustomizerSize_1280x1024 ? 1 : 2;

            ComboCurDay.Width = 300 / d;
            ComboTypeCompare.Width = 500 / d;
            
            ComboTypeCompare.Title = "Период для сравнения";
        }

        private void FillCombo()
        {
            FillComboPeriod();
            FillComboComareType();
            ConfCombo();
        }

        private List<Node> AllListNode(CustomMultiCombo Combo)
        {
            List<Node> Nods = new List<Node>();

            foreach (Node n in Combo.SelectedNodes)
            {
                AddAllChildNode(n, Nods);
            }
            return Nods;
        }



        private string GetAllSelectDateWith()
        {
            string res = "";
            foreach (Node node in AllSelectedNode)
            {
                res += string.Format("member [Период__День].[Период__День].[{0}] as '{1}'", node.Text, SetComboPeriod.DataUniqeName[node.Text]);
            }
            return res;
        }

        private string GetAllSelectDate()
        {
            string res = "";



            foreach (Node node in AllSelectedNode)
            {
                res += string.Format("[Период__День].[Период__День].[{0}],", node.Text);
            }
            if (res != "")
            {
                return res.Remove(res.Length - 1);
            } return "";
        }


        private string GetFieldRfWith(string p)
        {
            string res = "";

            DataTable TableField = GetRFTable();

            foreach (DataRow row in TableField.Rows)
            {
                if (row["Значение параметра"].ToString() == p)
                {
                    res += string.Format("member [СЭП__Годовой сборник].[СЭП__Годовой сборник].[{0}] as '{1}'", row["имя в очтёте"], row["Показатели, используемые в отчете"]);
                }
            }

            return res;//.Remove(res.Length - 1);
        }

        private string GetFieldWith(string p)
        {
            string res = "";

            DataTable TableField = GetTableMOField();

            foreach (DataRow row in TableField.Rows)
            {

                if (row["Значение параметра"].ToString() == p)
                {
                    res += string.Format("member [Организации__Товары и услуги].[Организации__Товары и услуги].[{0}] as '{1}'", row["имя в очтёте"], row["Показатели, используемые в отчете"]);
                }
            }

            return res;//.Remove(res.Length - 1);
        }


        private string GetFieldRf(string p)
        {
            string res = "";

            DataTable TableField = GetRFTable();

            foreach (DataRow row in TableField.Rows)
            {
                if (row["Значение параметра"].ToString() == p)
                {
                    res += "[СЭП__Годовой сборник].[СЭП__Годовой сборник].[" + row["имя в очтёте"] + "],";
                }
            }

            return res.Remove(res.Length - 1);
        }

        private string GetField(string p)
        {
            string res = "";

            DataTable TableField = GetTableMOField();

            foreach (DataRow row in TableField.Rows)
            {
                if (row["Значение параметра"].ToString() == p)
                {
                    res += "\n[Организации__Товары и услуги].[Организации__Товары и услуги].[" + row["имя в очтёте"] + "],";
                }
            }

            return res.Remove(res.Length - 1);
        }

        List<Node> AllSelectedNode = null;


        private List<Node> RemoveParentNode(List<Node> list)
        {
            List<Node> noparent = new List<Node>();
            foreach (Node n in list)
            {
                if (n.Nodes.Count == 0)
                {
                    noparent.Add(n);
                }
            }
            return noparent;
        }

        private void ChoseParam()
        {

            AllSelectedNode = RemoveParentNode(AllListNode(ComboCurDay));

            ChosenField.Value = GetField(ComboTypeCompare.SelectedValue);

            ChosenFieldRF.Value = GetFieldRf(ComboTypeCompare.SelectedValue);

            ChosenFieldWith.Value = GetFieldWith(ComboTypeCompare.SelectedValue);

            ChosenFieldRFWith.Value = GetFieldRfWith(ComboTypeCompare.SelectedValue);

            Period.Value = GetAllSelectDate();

            PeriodRF.Value = Period.Value.Replace(
                        "[Период__День].[Период__День]",
                        "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]");


            PeriodWith.Value = GetAllSelectDateWith();

            PeriodRFWith.Value = PeriodWith.Value.Replace(
                        "[Период__День].[Период__День]",
                        "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]");


        }




        class CoolTable : DataTable
        {
            public DataRow AddRow()
            {
                DataRow NewRow = this.NewRow();
                this.Rows.Add(NewRow);
                return NewRow;
            }


            public DataRow AddOrFindRow(string Cap, string ColName)
            {
                foreach (DataRow Row in this.Rows)
                {
                    if (Row[ColName].ToString() == Cap)
                    {
                        return Row;
                    }
                }

                DataRow ResRow = this.AddRow();
                ResRow[ColName] = Cap;

                return ResRow;
            }

            public decimal getFirstRealVal()
            {

                foreach (DataColumn col in this.Columns)
                {
                    foreach (DataRow row in this.Rows)
                    {
                        if (row[col] is decimal)
                        {
                            return (decimal)row[col];
                        }
                    }
                }
                return -1;
            }

            public decimal GetExtremum(Func<decimal, decimal, bool> comparer)
            {
                decimal extremum = this.getFirstRealVal();
                foreach (DataColumn col in this.Columns)
                {
                    foreach (DataRow row in this.Rows)
                    {
                        if (row[col] is decimal)
                        {
                            if (comparer(extremum, (decimal)row[col]))
                            {
                                extremum = (decimal)row[col];
                            }
                        }

                    }
                }
                return extremum;
            }

        }
        CoolTable ReportTable = null;
        private void DataBindGrid()
        {
            ReportTable = new CoolTable();
            DataTable BaseTableHmao = DataBaseHelper.ExecQueryByID("GridHMAO", "Field");

            DataBaseHelper.ActiveDataPorvider = DataProvidersFactory.SecondaryMASDataProvider;
            DataTable BaseTableRF = DataBaseHelper.ExecQueryByID("GridRF", "Field");
            DataBaseHelper.ActiveDataPorvider = DataProvidersFactory.SpareMASDataProvider;

            ReportTable.Columns.Add("Field");

            foreach (DataColumn col in BaseTableHmao.Columns)
            {
                if (col.ColumnName != "Field")
                {
                    ReportTable.Columns.Add(col.ColumnName + ";XMAO", typeof(decimal));
                    ReportTable.Columns.Add(col.ColumnName + ";РФ", typeof(decimal));
                }
            }

            foreach (DataRow Row in BaseTableHmao.Rows)
            {
                DataRow ReportRow = ReportTable.AddRow();

                foreach (DataColumn col in ReportTable.Columns)
                {
                    if (col.ColumnName != "Field")
                    {
                        string ColName = col.ColumnName.Split(';')[0];

                        if (col.ColumnName.Contains("XMAO"))
                        {
                            ReportRow[col] = Row[ColName];
                        }
                    }
                    else
                    {
                        ReportRow["Field"] = Row["Field"];
                    }
                }
            }

            foreach (DataRow Row in BaseTableRF.Rows)
            {
                DataRow ReportRow = ReportTable.AddOrFindRow(Row["Field"].ToString(), "Field");

                foreach (DataColumn col in ReportTable.Columns)
                {
                    if (col.ColumnName != "Field")
                    {
                        string ColName = col.ColumnName.Split(';')[0];

                        if (col.ColumnName.Contains("РФ"))
                        {
                            ReportRow[col] = Row[ColName];
                        }
                    }
                    else
                    {
                        ReportRow["Field"] = Row["Field"];
                    }
                }
            }

            if (ReportTable.Columns.Count <= 1)
            {
                ReportTable = null;
            }

            Grid.DataSource = ReportTable;
            Grid.DataBind();
        }
        GridHeaderLayout GHL = null;
        private void SetHeader()
        {
            GHL = new GridHeaderLayout(Grid);
            string PreParent = "";

            GridHeaderCell prevparcell = null;

            foreach (UltraGridColumn col in Grid.Columns)
            {
                if (col.BaseColumnName.Contains(";"))
                {
                    string parent = col.BaseColumnName.Split(';')[0];
                    string Child = col.BaseColumnName.Split(';')[1];
                    if (parent != PreParent)
                    {
                        prevparcell = GHL.AddCell(parent);
                        prevparcell.AddCell(Child);
                    }
                    else
                    {
                        prevparcell.AddCell(Child);
                    }

                    PreParent = parent;
                }
                else
                {
                    GHL.AddCell("Уровень инфляции");
                }
            }
            GHL.ApplyHeaderInfo();
        }

        private void CustomizerCol()
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if (col.BaseColumnName.Contains("Field"))
                {
                }
                else
                {
                    CRHelper.FormatNumberColumn(col, "#0.00");
                }
                col.CellStyle.Wrap = true;
            }

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.Wrap = true;
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        private void formatRow()
        {

            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells.FromKey("Field").Text.Contains("Всего"))
                {
                    row.Style.Font.Bold = true;
                }
                else
                {
                    row.Cells.FromKey("Field").Style.Padding.Left = 10;
                }
            }

        }

        private void OtherCunf()
        {
            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        private void ConfGrid()
        {
            SetHeader();

            CustomizerCol();

            formatRow();

            OtherCunf();

        }

        private void ClearGrid()
        {
            Grid.Columns.Clear();
            Grid.Bands.Clear();
            Grid.Rows.Clear();
        }

        private void GenerateGrid()
        {
            ClearGrid();

            DataBindGrid();

            ConfGrid();
            if (Grid.Columns.Count > 0)
            {
                CustomizerSize.ContfigurationGrid(Grid);
            }

        }

        


        int GetChekedRadioButton()
        {
            if (RadioButton1.Checked)
            {
                return 1;
            }
            if (RadioButton2.Checked)
            {
                return 2;
            }
            return 3;

        }

        private void SetHeaderReport()
        {
            Hederglobal.Text = "Уровень инфляции в субъекте РФ";
            Page.Title = Hederglobal.Text;
            PageSubTitle.Text = string.Format("Ежемесячная информация об уровне инфляции <b>{1}{0}</b> в Ханты-Мансийском автономном округе -Югре", ComboTypeCompare.SelectedValue, ComboTypeCompare.SelectedValue.Contains("%")?"за ":"");

            if (Grid.Columns.Count > 1)
            {
                RadioButton1.Text = Grid.Rows[1].Cells.FromKey("Field").Text;

                RadioButton2.Text = Grid.Rows[2].Cells.FromKey("Field").Text;

                RadioButton3.Text = Grid.Rows[3].Cells.FromKey("Field").Text;

                Label4.Text = string.Format("Уровень инфляции в целом в сравнении с инфляцией на <b>{0}</b> в ХМАО-Югре, %", Grid.Rows[GetChekedRadioButton()].Cells.FromKey("Field"));

            }

            RadioButton1.Visible = RadioButton2.Visible = RadioButton3.Visible = Grid.Columns.Count > 1;
        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChoseParam();

            GenerateGrid();

            GenChart();

            SetHeaderReport();


        }

        void DelRowFromKey(string key, DataTable table)
        {
            DataRow Row = null;

            foreach (DataRow row in table.Rows)
            {
                if (row["Field"].ToString() == key)
                {
                    Row = row;
                    break;
                }
            }

            if (Row != null)
            {
                table.Rows.Remove(Row);
            }
        }


        private static CoolTable Transformtable(DataTable BaseTableChart, int indexRow)
        {
            CoolTable TableChart = new CoolTable();

            TableChart.Columns.Add("Field");
            TableChart.Columns.Add("ХМАО, всего", typeof(decimal));
            TableChart.Columns.Add("РФ, всего", typeof(decimal));

            string prevdate = "";

            DataRow lastRow = null;


            foreach (DataColumn Col in BaseTableChart.Columns)
            {

                if (Col.ColumnName.Contains("Field"))
                {
                    continue;
                }

                string date = Col.ColumnName.Split(';')[0];
                string region = Col.ColumnName.Split(';')[1];

                if (prevdate != date)
                {
                    lastRow = TableChart.AddRow();

                    lastRow["Field"] = date;

                    lastRow["ХМАО, всего"] = BaseTableChart.Rows[indexRow][Col];
                }
                else
                {
                    lastRow["РФ, всего"] = BaseTableChart.Rows[indexRow][Col];
                }
                prevdate = date;
            }
            return TableChart;
        }


        bool isEmptyTable(DataTable Table)
        {
            foreach (DataColumn col in Table.Columns)
            {
                if (!col.DataType.ToString().ToLower().Contains("string"))
                {
                    foreach (DataRow row in Table.Rows)
                    {
                        if (row[col] != DBNull.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        private bool IsEmptyRow(DataRow Row)
        {
            foreach (DataColumn col in Row.Table.Columns)
            {
                if (!col.DataType.ToString().ToLower().Contains("string"))
                {
                    if (Row[col] != DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        DataRow GetRowFromKey(DataTable Table, string Key, string ColKey)
        {
            foreach (DataRow row in Table.Rows)
            {
                if (row[ColKey].ToString() == Key)
                {
                    return row;
                }
            }
            throw new Exception("Невероятная ошибка");
        }


        void ClearEmptyRow(DataTable Table, DataTable TableControl)
        {
            List<DataRow> RemRow = new List<DataRow>();

            foreach (DataRow Row in Table.Rows)
            {
                if ((IsEmptyRow(Row)) && (IsEmptyRow(GetRowFromKey(TableControl, Row["Field"].ToString(), "Field"))))
                {
                    RemRow.Add(Row);
                }
            }
            foreach (DataRow row in RemRow)
            {
                Table.Rows.Remove(row);
            }
        }

        

        private bool BindChart()
        {
            DataTable BaseTableChart = ReportTable.Copy();

            int indexRow = 0;

            CoolTable TableChart = Transformtable(BaseTableChart, indexRow); 

            if (isEmptyTable(TableChart) && isEmptyTable(Transformtable(BaseTableChart, GetChekedRadioButton())))
            {
                return false;
            }

            ClearEmptyRow(TableChart, Transformtable(BaseTableChart, GetChekedRadioButton()));

            ChartLine.DataSource = TableChart;

            ChartLine.DataBind();

            return true;
        }

        private void CostomizeChart()
        {
            ChartLine.BackColor = Color.White;

            ChartLine.Data.SwapRowsAndColumns = false;

            ChartLine.ChartType = ChartType.ColumnChart;
            ChartLine.ColumnChart.SeriesSpacing = 1;

            CRHelper.FillCustomColorModelLight(ChartLine, 2, false);
            
            ChartLine.Axis.X.Extent = 120;
            ChartLine.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartLine.Axis.X.Labels.Visible = true;

            ChartLine.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            ChartLine.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            ChartLine.Legend.SpanPercentage = 10;
            ChartLine.Legend.Visible = true;
            ChartLine.Legend.Location = LegendLocation.Bottom;
            ChartLine.Legend.SpanPercentage = 15;

            ChartLine.Legend.FormatString = "ХМАО, непродовольственные товары";

            ChartLine.Axis.Y.RangeMax = 1.1 * (double)(ReportTable.GetExtremum((a, b) => { return b > a; }));
            ChartLine.Axis.Y.RangeMin = 0.9 * (double)(ReportTable.GetExtremum((a, b) => { return b < a; }));
            ChartLine.Axis.Y.RangeType = AxisRangeType.Custom;

            ChartLine.Axis.Y.Extent = 50;

            ChartLine.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";


            ChartLine.Tooltips.FormatString = "<ITEM_LABEL><br><SERIES_LABEL><br><b><DATA_VALUE:N2></b>%";

            ChartLine.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartLine_FillSceneGraph);

            ChartLine.ColorModel.ModelStyle = ColorModels.LinearRange;
            ChartLine.ColorModel.ColorBegin = Color.Blue;
            ChartLine.ColorModel.ColorEnd = Color.Red; 
        }


        private void GenChart()
        {
            if (ReportTable != null)
            {
                if (BindChart())
                {
                    CostomizeChart();
                }
            }

            ChartLine.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

        }

        

        



        int GetCorXFromText(string caption, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, string capFind)
        {

            decimal optima = -1;

            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Text)
                {
                    if (((Text)p).GetTextString().Contains(caption))
                    {
                        Text t = (Text)p;
                        optima = t.bounds.X + t.bounds.Width / 2;
                    }
                }
            }
            Text tOptima = null;
            decimal maxDelta = decimal.MaxValue;

            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Text)
                {
                    if (((Text)p).GetTextString().Contains(capFind))
                    {
                        Text t = (Text)p;
                        if (Math.Abs(optima - (t.bounds.X + t.bounds.Width / 2)) < maxDelta)
                        {
                            maxDelta = Math.Abs(optima - (t.bounds.X + t.bounds.Width / 2));
                            tOptima = t;
                            
                        }
                    }
                }
            }
            try
            {
                return tOptima.bounds.X + tOptima.bounds.Width / 2;
            }
            catch
            {
                throw new Exception("Ненайдена подпись на оси X");
            }
        }


        private void GenTriangle(int x, int y, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, Color color, double value)
        {
            int mul = 2;

            Ellipse Circle = new Ellipse(new Point(x, y), 5);
            Circle.PE.FillGradientStyle = GradientStyle.Horizontal;
            Circle.PE.Fill = color;

            if (value !=double.MaxValue)
            {
                Text t = new Text(new Point(x-12,y-6*mul),string.Format("{0:N2}",value));
                e.SceneGraph.Add(t);
            }

            TrianglePostListAdeded.Add(Circle);
        }

        private void Conflegend(Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            List<Primitive> DelPrim = new List<Primitive>();

            List<Primitive> LegendCaptio = new List<Primitive>();

            foreach (Primitive p in e.SceneGraph)
            {
                if (p.Path != null)
                {
                    if (p is Text)
                    {
                        if (p.Path.ToLower().Contains("legend"))
                        {
                            LegendCaptio.Add(p);
                        }
                        if (((Text)p).GetTextString().Contains("more"))
                        {
                            DelPrim.Add(p);
                        }
                    }
                }

            }
            foreach (Primitive p in DelPrim)
            {
                e.SceneGraph.Remove(p);
            }

            if (LegendCaptio.Count > 1)
            {

                Text Caption1 = (Text)LegendCaptio[0];
                Caption1.SetTextString("ХМАО, всего");

                GenTriangle(Caption1.bounds.X - 8, Caption1.bounds.Y + 25, e, Color.Yellow, double.MaxValue);
                Text t = new Text(new Point(Caption1.bounds.X, Caption1.bounds.Y + 25),
                    string.Format("ХМАО, {0}", Grid.Rows[GetChekedRadioButton()].Cells.FromKey("Field").Text));
                e.SceneGraph.Add(t);



                Text Caption2 = (Text)LegendCaptio[1];
                Caption2.SetTextString("РФ, всего");

                GenTriangle(Caption2.bounds.X - 8, Caption2.bounds.Y + 25, e, Color.Green, double.MaxValue);

                t = new Text(new Point(Caption2.bounds.X, Caption2.bounds.Y + 25),
                    string.Format("РФ, {0}", Grid.Rows[GetChekedRadioButton()].Cells.FromKey("Field").Text));
                e.SceneGraph.Add(t);

            }




        }


        private void DrawGraph(Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, DataTable table, string FindCaption, string ColValue, Color ColorLine, Color ColorTraingle)
        {
            string prevX = "";
            double prevY = -1;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            foreach (DataRow row in table.Rows)
            {
                if (prevX == "")
                {
                    prevX = row["Field"].ToString();
                    if (row[ColValue] != DBNull.Value)
                    {
                        prevY = (double)(decimal)row[ColValue];
                        try
                        {
                            GenTriangle(GetCorXFromText(prevX, e, FindCaption), (int)yAxis.Map(prevY), e, ColorTraingle, prevY);
                        }
                        catch { }
                    }

                }
                else
                {
                    string CurX = row["Field"].ToString();
                    if (row[ColValue] != DBNull.Value)
                    {
                        double CurY = (double)(decimal)row[ColValue];

                        Line l = new Line(
                            new Point(GetCorXFromText(prevX, e, FindCaption), (int)yAxis.Map(prevY)),
                            new Point(GetCorXFromText(CurX, e, FindCaption), (int)yAxis.Map(CurY)));

                        l.PE.Stroke = ColorLine;
                        l.PE.StrokeWidth = 2;
                        e.SceneGraph.Add(l);

                        prevX = CurX;
                        prevY = CurY;
                        try
                        {
                            GenTriangle(GetCorXFromText(prevX, e, FindCaption), (int)yAxis.Map(prevY), e, ColorTraingle, prevY);
                        }
                        catch { }
                    }
                }
            }
        }

        List<Primitive> TrianglePostListAdeded = new List<Primitive>();
        void ChartLine_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            

            DataTable table = Transformtable(ReportTable.Copy(), GetChekedRadioButton());

            DrawGraph(e, table, "ХМАО", "ХМАО, всего", Color.Green, Color.Yellow);

            DrawGraph(e, table, "РФ", "РФ, всего", Color.Purple, Color.Green);

            Conflegend(e);

            foreach (Primitive p in TrianglePostListAdeded)
            {
                e.SceneGraph.Add(p);
            }

            List<Primitive> DelList = new List<Primitive>();
            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Text)
                {
                    
                    if (p.Path !=null &&(p.Path.Contains("Border.Title.Grid.X")))
                    {
                        Text t = (Text)p;
                        if (t.GetTextString().Contains("ХМАО") || (t.GetTextString().Contains("РФ")))
                        {
                            DelList.Add(t);
                        }
                        else
                        {
                            t.SetTextString(t.GetTextString().Insert(t.GetTextString().IndexOf(' '), "\n"));
                            t.bounds.X -= t.bounds.Width / 2;
                            t.bounds.Width *= 2;
                            t.labelStyle.Font = new Font("Arial", 9);
                            
                        }
                        
                    }
                }
            }

            foreach (Primitive p in DelList)
            {
                e.SceneGraph.Remove(p);
            }
        }

 

        #region Экспорт в Excel


        string GetTitleCaption(string text)
        {
            return text.Replace("<b>", "").Replace("</b>", "");
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = GetTitleCaption(Hederglobal.Text);
            ReportExcelExporter1.WorksheetSubTitle = GetTitleCaption(PageSubTitle.Text);

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            SetExportGridParams(GHL.Grid);

            ReportExcelExporter1.HeaderCellHeight = 25;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(GHL, sheet1, 2);
            ChartLine.Width = 920;
            ReportExcelExporter1.Export(ChartLine, Label4.Text, sheet2, 3);
            sheet2.Columns[0].Width = 25 * 900;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.1;
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion


        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = GetTitleCaption(Hederglobal.Text);

            Report report = new Report();
            ISection section1 = report.AddSection();

            //foreach (UltraGridRow row in Grid.Rows)
            //    row.Cells[1].Value = Regex.Replace(row.Cells[1].Text, "<[\\s\\S]*?>", String.Empty);

            ReportPDFExporter1.HeaderCellHeight = 20;
            ReportPDFExporter1.Export(GHL, section1);


            ISection section2 = report.AddSection();
            ChartLine.Width = 1000;
            ReportPDFExporter1.Export(ChartLine, GetTitleCaption(Label4.Text), section2);


        }

        #endregion











    }
}