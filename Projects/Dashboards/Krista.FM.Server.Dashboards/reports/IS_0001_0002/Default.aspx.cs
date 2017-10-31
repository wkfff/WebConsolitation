using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Drawing;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.Shared;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using System.Text.RegularExpressions;
using Infragistics.UltraChart.Shared.Events;
using Krista.FM.Common;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Data.Series;

namespace Krista.FM.Server.Dashboards.reports.IS_0001_0002
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private CustomParam squareID;
        private CustomParam squareRegNumber;

        private static string[] squareNames;
        private static string[] squareIDs;
        private static string[] squareRegNumbers;

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            db = GetDataBase();

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid1.Width = Unit.Parse("725px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid1.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid1.Width = Unit.Parse("1200px");
            }

            HeaderTable.Width = String.Format("{0}px", (int)UltraWebGrid1.Width.Value);

            #region Грид

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            
            #endregion

            squareID = UserParams.CustomParam("square_id");

            squareRegNumber = UserParams.CustomParam("square_reg_number", true);

            if (!String.IsNullOrEmpty(squareRegNumber.Value))
                squareID.Value = GetSquareIDByRegNumber(squareRegNumber.Value);

            Page.Title = PageTitle.Text = String.Format("Паспорт инвестиционной площадки");
                        
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if (!Page.IsPostBack)
                {
                    ComboSquare.Title = "Регистрационный номер";
                    ComboSquare.ParentSelect = true;
                    ComboSquare.MultiSelect = false;
                    ComboSquare.Width = 650;
                    FillSquaresDictionary(ComboSquare);

                    if (ComboSquare.GetRootNodesCount() == 0)
                    {
                        db.Dispose();
                        throw new Exception("Не найдено данных для постоения отчета");
                    }
                }

                squareID.Value = GetSquareIDByName(ComboSquare.SelectedValue);

                Page.Title = PageTitle.Text = String.Format("Паспорт инвестиционной площадки: {0}", ComboSquare.SelectedValue);

                UltraWebGrid1.DataBind();
            }
            finally
            {
                db.Dispose();
            }

        }

        #region Грид

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string[] param = {
                                 "Регистрационный номер",
                                 "Муниципальное образование",
                                 "Местоположение",
                                 "Кадастровый номер",
                                 "Площадь, кв. км.",
                                 "Категория земель",
                                 "Собственник (пользователь) земельного участка:",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Организация",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ФИО руководителя",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ФИО ответственного лица",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;E-mail ответственного лица",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Телефон ответственного лица",
                                 "Разрешенное использование земельного участка (в соответствии с Правилами землепользования и застройки муниципального образования)",
                                 "Обременения (фактическое использование земельного участка)",
                                 "Наличие градостроительного плана земельного участка. Обеспеченность территории, в которую входит земельный участок, " +
                                        "документами территориального планирования, правилами землепользования и застройки (градостроительные регламенты), " +
                                        "докуметацией по планировке территории, инженерным изысканиям (в том числе топографические съемки различных масштабов)",
                                 "Ограничения использования земельного участка (санитарно-защитные зоны, охранные зоны и др.)",
                                 "Параметры разрешенного строительства объекта капитального строительства",
                                 "Наличие на земельном участве водоемов, зеленых насаждений (деревья, кустарники, особо ценные породы), " +
                                        "степень заболоченности и заселенности, особенности рельефа территории участка",
                                 "Наличие (удаленность от земельного участка) объектов транспортной инфраструктуры, в т.ч.:",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;автомобильные дороги с твердым покрытием (асфальтобетон, бетон), муниципальный транспорт (краткая характеристика);",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;железнодорожная магистраль, станция, тупик, ветка, подкрановые пути, краткая характеристика (в том числе электрифицированные, неэлектрифицированные);",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;водный транспортный путь, пристань, причальная стенка и др. (краткая характеристика);",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;аэропорт (грузовые и пассажирские перевозки), краткая характеристика",
                                 "Наличие (удаленность от земельного участка) сетей инженерно-технического обеспечения и объектов инженерной инфраструктуры, в т.ч.:",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;объекты водоснабжения (артезианские скважины, насосные станции, водонапорные башни, магистральные сети), " +
                                        "тип, мощность объектов водоснабжения, возможность и условия подключения;",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;канализация (тип: бытовая, ливневая, канализационная насосная станция, очистные сооружения мощность, возможность и условия подключения);",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;объекты газоснабжения (магистральные сети, распределительные устройства), тип, мощность, возможность и условия подключения;",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;объекты электроснабжения (электрические линии, подстанции), тип и мощность, возможность и условия подключения;",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;объекты теплоснабжения (центральные тепловые подстанции, сети), тип и мощность, возможность и условия подключения;",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;полигон для размещения бытовых, промышленных и производственных отходов, тип, мощность, возможность и условия дополнительного размещения отходов;",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;телефонизация площадки",
                                 "Технические условия подключения объекта капитального строительства к сетям инженерно-технического обеспечения",
                                 "Информация о плате за подключение объекта капитального строительства к сетям инженерно-технического обеспечения",
                                 "Расстояние от земельного участка до жилых массивов, водоемов, природоохранных и санитарно-защитных зон",
                                 "Перечень и  характеристика зданий, сооружений и других объектов, находящихся на земельном участке",
                                 "Наличие (удаленность от земельного участка) природных, лесных ресурсов, месторождений полезных ископаемых, их характеристика",
                                 "Близость земельного участка к населенным пунктам, количество жителей, уровень занятости населения, демографическая ситуация, " +
                                        "основные градообразующие отрасли экономики",
                                 "Близость земельного участка к объектам среднего специального и высшего образования, наличие кадрового потенциала территории и источники его формирования",
                                 "Близость земельного участка к объектам:",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;здравоохранения (поликлиники, больницы общего и специализированного профиля, здравпункты, аптеки, фельдшерско-акушерские пункты и т.д.)",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;социальной сферы (детские сады, школы, места проведения досуга населения и т.д.)",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;сферы услуг (магазины, кафе, столовые, бытовое обслуживание населения и т.д.)",
                                 "Близость земельного участка к объектам гостинично-деловой сферы (гостиницы, бизнес-центры, офисы компаний и т.д.)"
                             };
            string query = DataProvider.GetQueryText("IS_0001_0002_grid");
            DataTable dtSquare = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            DataRow row = dtSquare.Rows[0];

            HyperLink.Text = "Показать на карте";
            HyperLink.NavigateUrl = String.Format("http://maps.google.com?t=h&z=16&output=embed&q=loc:{0},{1}",
                row["CoordinatesLat"].ToString().Replace(',', '.'), row["CoordinatesLng"].ToString().Replace(',', '.'));

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Наименование поля", typeof(string));
            dtGrid.Columns.Add("Значение", typeof(string));
            for (int i = 0; i < param.Length; ++i)
            {
                DataRow newRow = dtGrid.NewRow();
                newRow["Наименование поля"] = param[i];
                newRow["Значение"] = row[i];
                dtGrid.Rows.Add(newRow);
            }

            (sender as UltraWebGrid).DataSource = dtGrid;
        }

        private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;
            
            UltraGridBand band = grid.Bands[0];
            band.Columns[0].Width = Unit.Parse("400px");
            band.Columns[1].Width = Unit.Parse(String.Format("{0}px", (int)grid.Width.Value - 401));
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].CellStyle.Font.Bold = true;
            band.Columns[1].CellStyle.Wrap = true;
            band.HeaderLayout.Clear();

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            //e.Layout.RowSelectorStyleDefault.Width = Unit.Parse("1px");
        }

        private void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].GetText() == "Группировочное")
            {
                e.Row.Cells[0].ColSpan = 2;
            }
        }

        #endregion

        private string GetSquareIDByName(string squareName)
        {
            return squareIDs[Array.IndexOf(squareNames, squareName)];
        }

        private string GetSquareNameByID(string squareID)
        {
            return squareNames[Array.IndexOf(squareIDs, squareID)];
        }

        private string GetSquareIDByRegNumber(string squareRegNumber)
        {
            string query = DataProvider.GetQueryText("IS_0001_0002_id");
            return Convert.ToString(db.ExecQuery(query, QueryResultTypes.Scalar));
        }

        private void FillSquaresDictionary(CustomMultiCombo combo)
        {
            string query = DataProvider.GetQueryText("IS_0001_0002_square");
            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            squareIDs = new string[dt.Rows.Count];
            squareNames = new string[dt.Rows.Count];
            squareRegNumbers = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                DataRow row = dt.Rows[i];
                string id = row["id"].ToString();
                string regNumber = row["RegNumber"].ToString();
                string territory = row["Territory"].ToString();

                squareIDs[i] = id;
                squareNames[i] = String.Format("{0} ({1})", regNumber, territory);
                squareRegNumbers[i] = regNumber;

                dict.Add(String.Format("{0} ({1})", regNumber, territory), 0);
            }
            combo.FillDictionaryValues(dict);
            if (!String.IsNullOrEmpty(squareID.Value))
                combo.SetСheckedState(GetSquareNameByID(squareID.Value), true);
        }



        private static IDatabase GetDataBase()
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                return scheme.SchemeDWH.DB;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

    }
}
