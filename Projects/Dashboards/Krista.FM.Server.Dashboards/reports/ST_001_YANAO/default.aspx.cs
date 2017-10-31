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

using System.Collections.Generic;
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
using Infragistics.UltraGauge.Resources;


namespace Krista.FM.Server.Dashboards.ST_001_YANAO
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetRegion;

        ICustomizerSize CustomizerSize;

        bool SelectCity;

        CustomParam RegionBaseDimension { get { return (UserParams.CustomParam("RegionBaseDimension")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenYear { get { return (UserParams.CustomParam("ChosenYear")); } }
        CustomParam ChosenCustomers { get { return (UserParams.CustomParam("ChosenCustomers")); } }

        CustomParam FilterGrid { get { return (UserParams.CustomParam("FilterGrid")); } }



        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridHeight();
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
            public override int GetGridHeight()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 130;
                    case BrouseName.FireFox:
                        return 140;
                    case BrouseName.SafariOrHrome:
                        return 140;
                    default:
                        return 140;
                }
            }
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 710;
                    case BrouseName.FireFox:
                        return 720;
                    case BrouseName.SafariOrHrome:
                        return 730;
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
                int onePercent = (int)1280 / 100;

                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8;

            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;


                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;
                //прочих источников
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;
                //прочих источников
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {

            public override int GetGridHeight()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 440;
                    case BrouseName.FireFox:
                        return 440;
                    case BrouseName.SafariOrHrome:
                        return 440;
                    default:
                        return 440;
                }
            }

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
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
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
                int onePercent = (int)1280 / 100;

                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;
                
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8; 

            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                

                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;
                //прочих источников
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8; 
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                Grid.Columns.FromKey("Field").Width = onePercent * 20;
                try
                {
                    Grid.Columns.FromKey("Сметная стоимость;в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства").Width = onePercent * 10;
                    Grid.Columns.FromKey("Сметная стоимость;% освоения").Width = onePercent * 8;
                }
                catch { }
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;окружного бюджета").Width = onePercent * 8;
                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;местного бюджета").Width = onePercent * 8;

                Grid.Columns.FromKey("Освоено капитальных вложений в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;


                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;ВСЕГО").Width = onePercent * 8;
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;всего окружной бюджет").Width = onePercent * 8;

                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;бюджетов муниципальных образований").Width = onePercent * 8;
                //прочих источников
                Grid.Columns.FromKey("Финансирование за отчетный период в текущих ценах с начала строительства;в том числе за счет средств:;прочих источников").Width = onePercent * 8;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Width = onePercent * 16;
                Grid.Columns.FromKey("% освоенных капитальных вложений от финансирования;всего окружной бюджет").Width = onePercent * 8; 
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

        class Day
        {
            DateTime Date;
            public int Year;
            public int Mounth;
            public int day;
            public string BaseCaptionFORF;
            public string BaseCaptionSU;

            public Day(string caption)
            {
                string[] bb = new string[3] { "].[", "[", "]" };

                if (caption[0] == '[')
                {
                    string[] splCa = caption.Split(bb, StringSplitOptions.None);
                    Year = int.Parse(splCa[4]);
                    Mounth = CRHelper.MonthNum(splCa[7]);
                    day = int.Parse(splCa[8]);
                    BaseCaptionSU = caption;
                }
                else
                {


                    string[] b = new string[1] { "- " };
                    string[] ddmmyy = caption.Split(b, StringSplitOptions.None)[1].Split('.', ',');

                    Year = int.Parse(ddmmyy[2]);
                    Mounth = int.Parse(ddmmyy[1]);
                    day = int.Parse(ddmmyy[0]);
                    BaseCaptionFORF = caption;
                }
                BaseCaptionSU = string.Format("[Период__День].[Период__День].[Год].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                        Year,
                        CRHelper.HalfYearNumByMonthNum(Mounth),
                        CRHelper.QuarterNumByMonthNum(Mounth),
                        CRHelper.RusMonth(Mounth),
                        day);

                Date = new DateTime(Year, Mounth, day);


            }

            public string DisplayYear()
            {
                return Year.ToString() + " год";
            }

            public string DisplayMounth()
            {
                return CRHelper.RusMonth(Mounth) + " " + Year.ToString() + " года";
            }

            public string DisplayDay()
            {
                return day.ToString() + " " + CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }

            public string GridHeader()
            {
                return string.Format("{0:00}.{1:00}.{2:0000}", day, Mounth, Year);
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }


            public string ChartLabel()
            {
                return string.Format("{0:00}.{1:00}.{2:0000}", day, Mounth, Year);
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }

            public void RemoveDay(int count)
            {
                Date = Date.AddDays(-7);
                this.day = Date.Day;
                this.Mounth = Date.Month;
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

        class sortDay : System.Collections.Generic.IComparer<Day>
        {
            #region Члены IComparer<Day>

            public int Compare(Day x, Day y)
            {
                if (x.Year > y.Year)
                {
                    return 1;
                }
                else
                {
                    if (x.Year < y.Year)
                    {
                        return -1;
                    }
                    else
                    {
                        if (x.Mounth > y.Mounth)
                        {
                            return 1;
                        }
                        else
                        {
                            if (x.Mounth < y.Mounth)
                            {
                                return -1;
                            }
                            else
                            {
                                if (x.day > y.day)
                                {
                                    return 1;
                                }
                                else
                                {
                                    if (x.day == y.day)
                                        return 0;
                                    else
                                        return -1;
                                }
                            }
                        }

                    }
                }
            }

            #endregion
        }
        class DataSetComboPeriod : IDataSetCombo
        {
            string GetNumberQuart(string s)
            {
                return s.Split(' ')[1];
            }

            bool isquart(string s)
            {
                return s.Contains("Квартал");
            }

            string GenDisplayNameYear(string Year)
            {
                return string.Format("{0} год", Year);
            }

            string GenDisplayNameQuart(string Quart, string Year)
            {
                return string.Format("{0} квартал {1} года", Quart, Year);
            }

            public override void LoadData(DataTable Table)
            {
                string prevYear = "";

                foreach (DataRow Row in Table.Rows)
                {
                    string mounth = Row["mounth"].ToString();
                    string Year = Row["year"].ToString();
                    string uname = Row["uname"].ToString();

                    if (prevYear != Year)
                    {
                        this.AddItem(Year + " год", 0, "");
                        prevYear = Year;
                    }

                    mounth = GetAlowableAndFormatedKey(mounth+" " + Year+" года", "");

                    this.AddItem(mounth, 1, uname);
                    this.addOtherInfo(Table, Row, mounth);
                }


            }

        }

        class DataSetComboCustomer : IDataSetCombo
        {
            string GetNumberQuart(string s)
            {
                return s.Split(' ')[1];
            }

            bool isquart(string s)
            {
                return s.Contains("Квартал");
            }

            string GenDisplayNameYear(string Year)
            {
                return string.Format("{0} год", Year);
            }

            string GenDisplayNameQuart(string Quart, string Year)
            {
                return string.Format("{0} квартал {1} года", Quart, Year);
            }

            public override void LoadData(DataTable Table)
            {
                string prevYear = "";

                foreach (DataRow Row in Table.Rows)
                {
                    string mounth = Row["mounth"].ToString();
                    string Year = Row["year"].ToString();
                    string uname = Row["uname"].ToString();

                    if (prevYear != Year)
                    {
                        this.AddItem(Year, 0, "");
                        prevYear = Year;
                    }

                    mounth = GetAlowableAndFormatedKey(mounth, "");

                    this.AddItem(mounth, 1, uname);
                    this.addOtherInfo(Table, Row, mounth);
                }


            }

        }
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(sad);

            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            #endregion

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
            ComboCurDay.Width = 380;
            ComboCustomers.Width = 420; 
            Grid.Height = CustomizerSize.GetGridHeight();

            GridSearch1.LinkedGridId = Grid.ClientID;

            ComboCurDay.Title = "Период";
        }

        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");

            SetCurDay = new DataSetComboPeriod();
            SetCurDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurDay.FillDictionaryValues(SetCurDay.DataForCombo);
                ComboCurDay.SetСheckedState(SetCurDay.LastAdededKey, 1 == 1);
            }

            ComboCurDay.Width = 200;
        }

        private void FillComboCustomers()
        {
            ComboCustomers.ParentSelect = true;

            ComboCustomers.Title = "Заказчик";

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboCustomers", "DisplayName");

            SetRegion = new DataSetComboCustomer();
            SetRegion.LoadData(Table);

            if (!Page.IsPostBack)
            {
                //ComboCustomersFirst
                ChosenCurPeriod.Value = getMdxQuart(ComboCurDay.SelectedNode);
                ComboCustomers.FillDictionaryValues(SetRegion.DataForCombo);
                ComboCustomers.SetСheckedState("Все заказчики", true);
                    //SelectLastNode();
                //ComboCustomers.SetСheckedState( DataBaseHelper.ExecQueryByID("ComboCustomers").Rows[0][0].ToString() , true);
            }
        }

        Node GetFirstChild(Node n)
        {
            if (n.Nodes.Count > 0)
            {
                return GetFirstChild(n.Nodes[0]);
            }
            return n;
        }

        private void FillCombo()
        {
            RegionBaseDimension.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            FillComboPeriodCur();
            FillComboCustomers();
            ComboCurDay.SetСheckedState(GetFirstChild(ComboCurDay.SelectedNode).Text, true);
        }

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode == null)
            {
                if (n.Parent == null)
                {
                    return null;
                }
                if (n.Parent.PrevNode == null)
                {
                    return null;
                }
                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
            }
            return n.PrevNode;
        }

        private void SelectDouwnLevel(Infragistics.WebUI.UltraWebNavigator.Node node, bool selectFirst, CustomMultiCombo combo)
        {
            if (node.Nodes != null)
                if (node.Nodes.Count > 0)
                {
                    SelectDouwnLevel(node.Nodes[0], true, combo);
                }
                else
                {
                    if (selectFirst)
                    {
                        combo.SetСheckedState(node.Text, true);
                    }
                }
        }

        private string getMdxQuart(Node PrevNode)
        {
            if (PrevNode == null)
            {
                return "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[null]";
            }
            else
            {
                return SetCurDay.DataUniqeName[PrevNode.Text];
            }
        }


        int GetDijght(string yearCaption)
        {
            return int.Parse(yearCaption.Split(' ')[0]);
        }

        private void ChosenParam()
        {
            ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedValue];

            ChosenYear.Value = (GetDijght(ComboCurDay.SelectedNode.Parent.Text) - 2000).ToString();

            ChosenCustomers.Value = ComboCustomers.SelectedNode.Parent != null ?
                SetRegion.DataUniqeName[ComboCustomers.SelectedValue] :
                @"Except({[Строительство__Заказчики].[Строительство__Заказчики].[Заказчик].members},[Строительство__Заказчики].[Строительство__Заказчики].[(All)].[Все заказчики])";

            if (CheckBox1.Checked)
            {
                FilterGrid.Value = string.Format(@"crossjoin({0},
        Except([Строительство__Характеристика объектов].[Строительство__Характеристика объектов].members,
[Строительство__Характеристика объектов].[Строительство__Характеристика объектов].[(All)].[Все контракты]))", "{" + ChosenCustomers.Value + "}");
            }
            else
            {
                FilterGrid.Value = ChosenCustomers.Value;
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

        private object GetDeviation(object CurValue, object PrevValue)
        {
            try
            {
                if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
                {
                    return (decimal)CurValue - ((decimal)PrevValue);
                }
                return DBNull.Value;
            }
            catch { return DBNull.Value; }
        }

        private object GetSpeedDeviation(object CurValue, object PrevValue)
        {
            try
            {

                if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
                {
                    if ((decimal)PrevValue != 0)
                    {
                        return ((decimal)CurValue / ((decimal)PrevValue));
                    }
                    else
                    {
                        return DBNull.Value;
                    }
                }
                return DBNull.Value;
            }
            catch { return DBNull.Value; }
        }
        private static void fillStrRow(DataRow GridRow, string ParsRow)
        {
            //GridRow["Region"] = ParsRow[0];
            GridRow["Field"] = ParsRow;
            //GridRow["Org"] = GridRow["Yslyga"].ToString() +"|"+ ParsRow[3];
        }


        private object GetSum(object p, object p_2)
        {
            decimal val1 = 0;
            decimal val2 = 0;
            try
            {
                val1 = (decimal)p;
            }
            catch { }
            try
            {
                val2 = (decimal)p_2;
            }
            catch { }
            return val1 + val2;

        }
        private void SumCol(DataTable BaseTable, string val1, string val2, string val3)
        {
            foreach (DataRow Row in BaseTable.Rows)
            {
                Row[val3] = GetSum(Row[val1], Row[val2]);
            }
        }

        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }



            public int Compare_(DataRow x, DataRow y)
            {
                decimal X = (decimal)x["Value"];
                decimal Y = (decimal)y["Value"];
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
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }

            LR.Sort(new SortDataRow());



            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }


        bool Isemptytable(DataTable Table)
        {
            foreach (DataColumn col in Table.Columns)
            {
                if (col.ColumnName != "Field")
                {
                    foreach (DataRow row in Table.Rows)
                    {
                        if ((row[col] != DBNull.Value) && ((decimal)row[col] != 0))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;

        }

        DataRow GetLastRowDataTable(DataTable Table)
        {
            return Table.Rows[Table.Rows.Count - 1];
        }


        private void DataBindGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            CoolTable TableGrid = new CoolTable();

            ///Grid.DataSource = BaseTable;

            foreach (DataColumn col in BaseTable.Columns)
            {
                TableGrid.Columns.Add(col.ColumnName, col.DataType);
            }

            string PRevParent = "";

            foreach (DataRow Row in BaseTable.Rows)
            {
                string child = Row[0].ToString();
                if (CheckBox1.Checked)
                {
                    string[] b = Row[0].ToString().Split(';');
                    child = b[1];
                    string parent = b[0];

                    if (PRevParent != parent)
                    {
                        PRevParent = parent;
                        TableGrid.AddRow()[0] = parent;
                    }
                }

                TableGrid.Rows.Add(Row.ItemArray);
                GetLastRowDataTable(TableGrid)[0] = child; 
            }


            if (!CheckBox1.Checked)
            {
                for (int i = 1; i < 3; i++)
                {
                    TableGrid.Columns.RemoveAt(1);
                }
            }

            Grid.DataSource = TableGrid.Rows.Count> 0? TableGrid:null;

            Grid.DataBind();
        }

        protected void SetDefaultStyleHeader(ColumnHeader header)
        {
            GridItemStyle HeaderStyle = header.Style;

            HeaderStyle.Wrap = true;

            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        private ColumnHeader GenHeader(string Caption, int x, int y, int spanX, int SpanY)
        {
            ColumnHeader Header = new ColumnHeader();

            Header.Caption = Caption;
            Header.Style.Wrap = true;
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = spanX;
            Header.RowLayoutColumnInfo.SpanY = SpanY;
            SetDefaultStyleHeader(Header);

            return Header;
        }

        private void ConfigHeader(ColumnHeader ch, int x, int y, int spanX, int spanY, string Caption)
        {
            ch.RowLayoutColumnInfo.OriginX = x;
            ch.RowLayoutColumnInfo.OriginY = y;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.RowLayoutColumnInfo.SpanY = spanY;
            ch.Caption = Caption;
            SetDefaultStyleHeader(ch);
            if (ch.Column.Index != 0)
            {
                CRHelper.FormatNumberColumn(ch.Column, "N2");
            }
            else
            {
                ch.Column.CellStyle.Wrap = true;
            }
        }
        GridHeaderLayout HL;
        public void ConfHeader()
        {

            HL = new GridHeaderLayout(Grid);

            HL.AddCell("Наименование государственных заказчиков Адресной инвестиционной программы ЯНАО, государственных (муниципальных) заказчиков объектов программы, заказчиков Адресной программы");

            GridHeaderCell cell = null;
            if (CheckBox1.Checked)
            {
                cell = HL.AddCell("Сметная стоимость");
                cell.AddCell("в прогнозном уровне цен или предполагаемая (предельная) стоимость строительства", 2);
                cell.AddCell("% освоения", 2);
            }

            cell = HL.AddCell("Освоено капитальных вложений в текущих ценах с начала строительства");
            cell.AddCell("ВСЕГО", 2);
            cell = cell.AddCell("в том числе за счет средств:");
            cell.AddCell("окружного бюджета");
            cell.AddCell("местного бюджета");
            cell.AddCell("прочих источников");


            cell = HL.AddCell("Объем финансирования за отчетный период в текущих ценах с начала строительства");
            cell.AddCell("ВСЕГО", 2);
            cell = cell.AddCell("в том числе за счет средств:");
            cell.AddCell("окружного бюджета");
            cell.AddCell("местного бюджета");
            cell.AddCell("прочих источников");

            cell = HL.AddCell("% освоенных капитальных вложений от финансирования");
            cell.AddCell("ВСЕГО", 2);
            cell.AddCell("всего окружной бюджет", 2);

            HL.ApplyHeaderInfo();

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.Wrap = true;
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }
        private UltraGridCell GetPrevCell(UltraGridCell Cell)
        {
            return Cell.Row.PrevRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }

        void MergeCellsGrid(UltraWebGrid Grid, int col)
        {
            UltraGridRow MerdgRow = Grid.Rows[0];
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[col].Text == MerdgRow.Cells[col].Text)
                {

                }
                else
                {
                    MerdgRow.Cells[col].RowSpan = Row.Index - MerdgRow.Index;
                    MerdgRow = Row;
                }
            }
            MerdgRow.Cells[col].RowSpan = Grid.Rows.Count - MerdgRow.Index;
        }
        UltraGridCell GetNextVertCell(UltraGridCell cell)
        {
            return cell.Row.Cells[cell.Column.Index + 1];
        }

        private void FormatRowYsly(UltraWebGrid Grid, string p)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                UltraGridCell cell = Row.Cells.FromKey(p);
                if (GetNextVertCell(cell).Value == null)
                {
                    if (SetRegion.DataForCombo.ContainsKey(cell.Text))
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.BackColor = Color.FromArgb(200, 200, 200);
                        cell.ColSpan = 4;
                    }
                    else
                    {
                        cell.Style.Padding.Left = 10;
                        cell.Style.BackColor = Color.FromArgb(222, 222, 222);
                        cell.ColSpan = 4;
                    }


                }
                else
                {
                    cell.Style.Padding.Left = 20;
                }
            }
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            SetImageFromCell(Cell, ImageName,"images");
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName,string imageFolder)
        {
            string ImagePath = string.Format("~/{0}/",imageFolder) + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        string FormatstringHint(string h)
        {
            return h.Replace("квартал", "кварталу");
        }

        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {
            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));

            if (IndicatorCell.Value != null)
            {
                int IndexCol = ValueCell.Column.Index;
                decimal Value = decimal.Parse(IndicatorCell.Text.ToString().Replace("%", ""));
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 100 ? "Up" : "Down";

                    string TitleCaption = Value > 100 ? "роста" : "снижения";

                    try
                    {
                        IndicatorCell.Title =
                            string.Format("Темп {0} к {1}", TitleCaption,
                            FormatstringHint(GetPrevNode(ComboCurDay.SelectedNode).Text));

                        TitleCaption = Value > 100 ? "Прирост" : "Снижение";

                        GetNextCell(ValueCell).Title = string.Format("{0} к {1}", TitleCaption, FormatstringHint(GetPrevNode(ComboCurDay.SelectedNode).Text));
                    }
                    catch { }

                    string Color = "";
                    string revColor = "";
                    if ((Value > 100))
                    {
                        Color = "Red";
                        revColor = "Green";
                    }

                    if ((Value < 100))
                    {
                        Color = "Green";
                        revColor = "Red";
                    }

                    if (reverce)
                    {
                        Color = revColor;
                    }

                    if (!string.IsNullOrEmpty(Color))
                    {
                        SetImageFromCell(IndicatorCell, "arrow" + Color + UpOrdouwn + "BB.png");
                    }
                }
            }
        }

        void FormatTopCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorBottom = Color.Transparent;
        }

        void FormatCenterCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorBottom = Color.Transparent;
            cell.Style.BorderDetails.ColorTop = Color.Transparent;
        }

        void FormatBottomCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorTop = Color.Transparent;
        }

        string RangMax = "";


        void SetFormatCell(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                try
                {
                    Cell.Text = string.Format("{0:" + format + "}", decimal.Parse(Cell.Text.Replace("%", "")));
                }
                catch { }
            }
        }

        private bool IsReverceCol(string p)
        {
            return p.Contains("Начислено (предъявлено) жилищно-коммунальных платежей населению") || p.Contains("Процент неоплаченных коммунальных услуг") | p.Contains("Стоимость предоставленных населению услуг, рассчитанная по экономически обоснованным тарифам");
        }

        private void FormatValueCell(UltraGridCell ValueCell)
        {
            FormatTopCell(ValueCell);
            FormatCenterCell(GetNextCell(ValueCell));
            FormatBottomCell(GetNextCell(GetNextCell(ValueCell)));

            if (ValueCell.Value == null)
            {
                return;
            }
            if (ValueCell.Column.BaseColumnName == " Процент неоплаченных коммунальных услуг")
            {
                SetFormatCell(ValueCell, "P2");
            }
            else
            {
                SetFormatCell(ValueCell, "N2");
            }
            SetFormatCell(GetNextCell(ValueCell), "N2");
            SetFormatCell(GetNextCell(GetNextCell(ValueCell)), "P2");

            //SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, !IsReverceCol(ValueCell.Column.BaseColumnName));
        }


        private void FormatRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Field").Text[0] == ' ')
                {
                    Row.Cells.FromKey("Field").Style.Margin.Left = 10;
                }
                else
                {
                    Row.Cells.FromKey("Field").Style.Font.Bold = true;
                    if (CheckBox1.Checked)
                        Row.Cells.FromKey("Field").ColSpan = Grid.Columns.Count;
                }

                if (Row.Cells.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Value != null)
                {
                    

                    double val = (double)(decimal)Row.Cells.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО").Value ;
                    val *= 100;

                    SetImageFromCell(Row.Cells.FromKey("% освоенных капитальных вложений от финансирования;ВСЕГО"), GenGanga(val>100?99.99:val, "/EO_MOP_GAdge"+val.ToString(), "../../TemporaryImages", 100, 30), "TemporaryImages"); ;
                }
            }
            
            //% освоенных капитальных вложений от финансирования;ВСЕГО

             
        }

         
        void CustomizerOther()
        {
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

            Grid.DisplayLayout.NoDataMessage = "Нет данных";

            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
        }

        UltraGridColumn GetPrevGridCol(UltraGridColumn col)
        {
            return col.Band.Grid.Columns[col.Index - 1];
        }

        UltraGridColumn GetLastGridCol(UltraWebGrid Grid)
        {
            return Grid.Columns[Grid.Columns.Count-1];
        }

        private void ConfColumn()
        {
            int index = 0;
            foreach (UltraGridColumn col in Grid.Columns)
            {

                col.CellStyle.Wrap = true;
                if (index != 0)
                    if (col.Header.Caption.Contains("%"))
                    {
                        CRHelper.FormatNumberColumn(col, "P2");
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(col, "N2");
                    }
                index++;
            }
            CRHelper.FormatNumberColumn(GetLastGridCol(Grid) , "P2");
            CRHelper.FormatNumberColumn(GetPrevGridCol(GetLastGridCol(Grid)), "P2");

            
        }


        private void CustomizeGrid()
        {
            FormatRow();

            ConfHeader();

            ConfColumn();

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

            CustomizerSize.ContfigurationGrid(Grid);

            CustomizerOther();
        }



        private void GenerationGrid()
        {
            Grid.Bands.Clear();

            DataBindGrid();

            if (Grid.Rows.Count > 0)
            {
                CustomizeGrid();
            }
        }





        Dictionary<string, string> DictEdizm = new System.Collections.Generic.Dictionary<string, string>();
        Dictionary<string, string> DictEdizm_re = new System.Collections.Generic.Dictionary<string, string>();
        private EndExportEventHandler ExcelExporter_EndExport_;
        private void FillDictEdIzm()
        {
            DataTable table = DataBaseHelper.ExecQueryByID("DictEdizm", "field");
            foreach (DataRow Row in table.Rows)
            {
                DictEdizm.Add(Row[0].ToString(), "рублей за " + Row[1].ToString());
            }

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChosenParam();

            GenerationGrid();

            SetHeaderReport();

        }

        /// <summary>
        /// Генерит гадж картинку в темпори имайже, занчение окуругляется шоб дубляжа менбше было
        /// использует гаджет каторый уже есть на страничке,
        /// да и госпада разработчики(если вам взгрустулось глянуть мой код конеш. Остальным не читать!) из соседнего отдела, судя по тендециии моей пастоновки гаджи они хотят активно использовать
        /// так что этот метод разумно было бы в ядро закинуть :)
        /// </summary>
        /// <param name="value">Значение для гаджи</param>
        /// <param name="prefix">Префих ну чёт таипа ../../../../TemporaryImages/megaGadgz_mo_mo</param>
        /// <param name="width">высота</param>
        /// <param name="height">ширина</param>
        /// <returns> prefix+ value.tostring()+.png</returns>
        protected string GenGanga(double value, string prefix, string prefixPage, int width, int height)
        {    
            //крута! каментов больше чем кода!
            value = Math.Round(value);
            string path = prefix + value.ToString() + ".png";
            System.Double V1 = value;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)UltraGauge1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;

            Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement BrushElement =
 (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (V1 > 80)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (V1 < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }


            System.Drawing.Size size = new Size(width, height);
            UltraGauge1.SaveTo(Server.MapPath("~/TemporaryImages" + path), GaugeImageType.Png, size);
            
            return path;
        }





        private string getbig2()
        {

            string res = "  ";
            int IndexLastCol = Grid.Columns.Count - 1;
            for (int i = 2; i < Grid.Rows.Count; i += 3)
            {
                if ((Grid.Rows[i].Cells[IndexLastCol].Value != null) &&
                    (decimal.Parse(Grid.Rows[i].Cells[IndexLastCol].Value.ToString().Replace("%", "")) >= 2))
                {
                    res += string.Format("«{0}» (<b>{1}</b>), ", Grid.Rows[i].PrevRow.PrevRow.Cells[0].Text,
                        Grid.Rows[i].Cells[IndexLastCol].Text);
                }
            }
            return res.Remove(res.Length - 2);

        }

        private string getsmal2()
        {

            string res = "  ";
            int IndexLastCol = Grid.Columns.Count - 1;
            for (int i = 2; i < Grid.Rows.Count; i += 3)
            {

                if ((Grid.Rows[i].Cells[IndexLastCol].Value != null) &&
                    (decimal.Parse(Grid.Rows[i].Cells[IndexLastCol].Value.ToString().Replace("%", "")) <= -2))
                {
                    res += string.Format("«{0}» (<b>{1}</b>), ", Grid.Rows[i].PrevRow.PrevRow.Cells[0].Text, Grid.Rows[i].Cells[IndexLastCol].Text);
                }
            }

            return res.Remove(res.Length - 2);


        }

        private void SetHeaderReport()
        {
            PageSubTitle.Text = string.Format("Данные мониторинга строительства объектов Адресной инвестиционной программы Ямало-Ненецкого автономного округа, по состоянию на {0}",
                ComboCurDay.SelectedValue.ToLower());

            Hederglobal.Text = "Мониторинг финансирования объектов строительства";
            Page.Title = Hederglobal.Text;
        }

        #region экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
                ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

                ReportExcelExporter1.RowsAutoFitEnable = false;
                ReportExcelExporter1.HeaderCellHeight = 60;

                Workbook workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
                ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);

                //Grid.Columns.FromKey("Стоимость предоставлен-ных населению услуг, рассчитанная по экономически обоснованным тарифам,тыс руб"

                foreach (UltraGridColumn col in HL.Grid.Columns)
                {
                    col.Width = (int)(col.Width.Value * 2);
                }

                ReportExcelExporter1.Export(HL, sheet1, 3);

                for (int i = 3; i < Grid.Rows.Count; i++)
                {
                    //  if (sheet1.Rows[i].Cells[0].Value != null)
                    {

                        sheet1.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                        sheet1.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                        sheet1.Rows[i].Cells[0].CellFormat.Indent = 3;
                        //GetMarginFromGridField(sheet1.Rows[i].Cells[0].Value.ToString());
                    }
                }

                ReportExcelExporter1.WorksheetTitle = String.Empty;
                ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            }
            catch { }
        }

        int GetMarginFromGridField(string f)
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    if (row.Cells[0].Text == f)
                    {

                        return (int)row.Cells[0].Style.Padding.Left.Value / 20;
                    }
                }
            }
            return 0;
        }


        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }


        private void MergRegion(int indexFirstMerg, int i, Worksheet worksheet, int p)
        {
            if (indexFirstMerg != i)
            {
                worksheet.MergedCellsRegions.Add(indexFirstMerg, p, i, p);
            }

        }

        private void MergRegion_(int indexFirstMerg, int i, Worksheet worksheet, int p)
        {
            if (indexFirstMerg != i)
            {

                worksheet.MergedCellsRegions.Add(p+1, indexFirstMerg, p+1, i);
            }
        }

        private void ExcellMergerRow(Worksheet worksheet, int p)
        {
            int indexFirstMerg = 3;

            foreach (UltraGridRow r in Grid.Rows)
            {
                if (r.Cells[p].RowSpan > 1)
                {
                    try
                    {
                        MergRegion(r.Index + 5, r.Index + 5 + r.Cells[p].RowSpan - 1, worksheet, p);
                    }
                    catch { }
                }
                if (r.Cells[p].ColSpan > 1)
                {
                    try
                    {
                        MergRegion_(p, p + r.Cells[p].ColSpan - 1, worksheet, r.Index + 5);
                    }
                    catch { }
                }
            }
            return;
            for (int i = 3; i < Grid.Rows.Count + 3; i++)
            {
                try
                {
                    if (worksheet.Rows[i].Cells[p].Value.ToString() == worksheet.Rows[indexFirstMerg].Cells[p].Value.ToString())
                    {

                    }
                    else
                    {
                        MergRegion(indexFirstMerg, i - 2, worksheet, p);
                        indexFirstMerg = i;
                    }
                }
                catch { indexFirstMerg = i; }
            }

        }




        void sad(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExcellMergerRow(e.CurrentWorksheet, 0);
            //ExcellMergerRow(e.CurrentWorksheet, 1);
        }



        #endregion


        void PDFMergRow(UltraWebGrid Grid, UltraGridRow Firstrow, UltraGridRow LastRow, int col)
        {

            string text = Firstrow.Cells[col].Text;
            for (int i = Firstrow.Index; i < LastRow.Index + 1; i++)
            {
                Grid.Rows[i].Cells[col].Text = " ";
            }

            for (int i = Firstrow.Index + 1; i < LastRow.Index; i++)
            {
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorTop = Color.White;
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorBottom = Color.White;
            }
            Firstrow.Cells[col].Style.BorderDetails.ColorBottom = Color.White;

            if (!(string.IsNullOrEmpty(text)))
                if (text.Contains("|"))
                {
                    text = text.Split('|')[1];
                }
            Grid.Rows[(Firstrow.Index + LastRow.Index) / 2].Cells[col].Text = text;
        }

        void MergeCellsGridFormPDF(UltraWebGrid Grid, int col)
        {
            UltraGridRow MerdgRow = Grid.Rows[0];
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[col].RowSpan > 1)
                {
                    PDFMergRow(Grid, Row, Grid.Rows[Row.Index + Row.Cells[col].RowSpan - 1], col);
                }
            }
        }

        private void MergeCellsGridFormPDFCol(UltraWebGrid ultraWebGrid, UltraGridRow row)
        {
            //if (row.Cells[0].ColSpan > 1)
            {
                for (int i = 1; i < Grid.Columns.Count - 1; i++)
                {
                    row.Cells[i].Style.BackColor = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
                }
                row.Cells[Grid.Columns.Count - 1].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                row.Cells[Grid.Columns.Count - 1].Style.BackColor = row.Cells[0].Style.BackColor;
                row.Cells[0].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
            }
        }


        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                ReportPDFExporter1.PageTitle = Page.Title;
                ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

                Report report = new Report();
                ISection section1 = report.AddSection();
                
                int OnePercent = 1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = OnePercent * (100 / Grid.Columns.Count);
                }

                ReportPDFExporter1.HeaderCellHeight = 50;
                MergeCellsGridFormPDF(HL.Grid, 0);

                ReportPDFExporter1.Export(HL, section1);
            }
            catch { }
        }

        #endregion

        public EndExportEventHandler ExcelExporter_EndExport { get; set; }
    }
}