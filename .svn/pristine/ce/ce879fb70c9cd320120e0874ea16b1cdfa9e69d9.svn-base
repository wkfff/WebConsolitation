using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;

namespace Krista.FM.Server.Dashboards.reports.MO_0002_0001
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam region { get { return (UserParams.CustomParam("region")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        private static int[] grid_rows_skip = { 0, 1, 2, 8, 11, 17, 20, 23, 25, 32, 36, 41, 48, 51, 62, 67, 72, 79, 82, 89, 92, 94, 95, 106, 109, 114, 118, 0};

        // Заголовок страницы
        private static String headerText = "Социально-экономическое положение по муниципальным образованиям {0}, на основании докладов глав администраций.";

        private static String[] grid_rows = {   "<b>Раздел I.Экономическое развитие</b>",
                                                "<b>Дорожное хозяйство</b>",
                                                "1. Доля отремонтированных автомобильных дорог общего пользования местного значения с твердым покрытием, в отношении которых произведен:",
                                                "&nbsp;капитальный ремонт",
                                                "&nbsp;текущий ремонт",
                                                "2. Доля автомобильных дорог местного значения с твердым покрытием, переданных на техническое обслуживание немуниципальным и (или) государственным предприятиям на основе долгосрочных договоров (свыше 3 лет)",
                                                "3. Доля протяженности автомобильных дорог общего пользования местного значения с твердым покрытием в общей протяженности автомобильных дорог общего пользования местного значения",
                                                "4. Доля населения, проживающего в населенных пунктах, не имеющих регулярного автобусного и (или) железнодорожного сообщения с административным центром городского округа (муниципального района), в общей численности населения городского округа (муниципального района)",
                                                "<b>Развитие малого и среднего предпринимательства</b>",
                                                "5. Число субъектов малого предпринимательства",
                                                "6. Доля среднесписочной численности работников (без внешних совместителей) малых предприятий в среднесписочной численности работников (без внешних совместителей) всех предприятий и организаций",
                                                "<b>Улучшение инвестиционной привлекательности</b>",
                                                "7.Площадь земельных участков, предоставленных для строительства - всего в том числе:",
                                                "&nbsp;для жилищного строительства, индивидуального жилищного строительства",
                                                "&nbsp;для комплексного освоения в целях жилищного строительства",
                                                "8. Доля площади земельных участков, являющихся объектами налогообложения земельным налогом, в общей площади территории городского округа (муниципального района)",
                                                "9. Средняя продолжительность периода с даты принятия решения о предоставлении земельного участка для строительства или подписания протокола о результатах торгов (конкурсов, аукционов) по предоставлению земельных участков до даты получения разрешения на строительство",
                                                "10. Площадь земельных участков, предоставленных для строительства, в отношении которых с даты принятия решения о предоставлении земельного участка или подписания протокола о результатах торгов (конкурсов, аукционов) не было получено разрешение на ввод в эксплуатацию:",
                                                "&nbsp;индивидуального жилищного строительства, - в течение 3 лет",
                                                "&nbsp;иных объектов капитального строительства - в течение 5 лет",
                                                "<b>Сельское хозяйство(для муниципальных районов)</b>",
                                                "11. Удельный вес прибыльных сельскохозяйственных организаций в общем их числе (для муниципальных районов)",
                                                "12. Доля фактически используемых сельскохозяйственных угодий в общей площади сельскохозяйственных угодий муниципального района",
                                                "<b>Раздел II. Доходы населения</b>",
                                                "13. Отношение среднемесячной номинальной начисленной заработной платы работников муниципальных учреждений к среднемесячной номинальной начисленной заработной плате работников крупных и средних предприятий и некоммерческих организаций городского округа муниципального района)",
                                                "14. Среднемесячная номинальная начисленная заработная плата работников:",
                                                "&nbsp;крупных и средних предприятий и некоммерческих организаций городского округа муниципального района)",
                                                "&nbsp;муниципальных детских дошкольных учреждений",
                                                "&nbsp;учителей муниципальных общеобразовательных учреждений",
                                                "&nbsp;прочих работающих в муниципальных общеобразовательных учреждениях административно-управленческого, учебно-вспомогательного, младшего обслуживающего персонала, а также педагогических работников, не осуществляющих учебный процесс)",
                                                "&nbsp;врачей муниципальных учреждений здравоохранения",
                                                "&nbsp;среднего медицинского персонала муниципальных учреждений здравоохранения",
                                                "<b>Раздел III. Здоровье</b>",
                                                "15. Удовлетворенность населения медицинской помощью",
                                                "16. Доля населения, охваченного профилактическими осмотрами",
                                                "17. Доля амбулаторных учреждений, имеющих медицинское оборудование в соответствии с табелем оснащения",
                                                "18. Доля муниципальных медицинских учреждений:",
                                                "&nbsp;применяющих медико-экономические стандарты оказания медицинской помощи",
                                                "&nbsp;переведенных на оплату медицинской помощи по результатам деятельности",
                                                "&nbsp;переведенных на новую систему оплаты труда, ориентированную на результат",
                                                "&nbsp;переведенных преимущественно на одноканальное финансирование через систему обязательного медицинского страхования",
                                                "19. Число случаев смерти лиц в возрасте до 65 лет:",
                                                "&nbsp;на дому — всего, в том числе",
                                                "&nbsp;от инфаркта миокарда,",
                                                "&nbsp;от инсульта",
                                                "&nbsp;в первые сутки в стационаре — всего, в том числе",
                                                "&nbsp;от инфаркта миокарда,",
                                                "&nbsp;от инсульта",
                                                "20. Число случаев смерти детей до 18 лет:",
                                                "&nbsp;на дому",
                                                "&nbsp;в первые сутки в стационаре",
                                                "21. Число работающих в муниципальных учреждениях здравоохранения в расчете на 10000 человек населения (на конец года) — всего, в том числе:",
                                                "&nbsp;число врачей в муниципальных учреждениях здравоохранения в расчете на 10000 человек населения (на конец года)",
                                                "&nbsp;из них участковых врачей и врачей общей практики",
                                                "&nbsp;число среднего медицинского персонала в муниципальных учреждениях здравоохранения в расчете на 10000 человек населения (на конец года)",
                                                "&nbsp;из них участковых медицинских сестер и медицинских сестер врачей общей практики",
                                                "22. Уровень госпитализации вмуниципальные учреждения здравоохранения",
                                                "23. Средняя продолжительность пребывания пациента на койке в круглосуточном стационаре муниципальных учреждений здравоохранения",
                                                "24. Среднегодовая занятость койки в муниципальных учреждениях здравоохранения",
                                                "25. Число коек в муниципальных учреждениях здравоохранения на 10000 человек населения",
                                                "26. Стоимость содержания одной койки в муниципальных учреждениях здравоохранения в сутки",
                                                "27. Средняя стоимость койко-дня в муниципальных стационарных медицинских учреждениях",
                                                "28. Объем медицинской помощи, предоставляемой муниципальными учреждениями здравоохранения, в расчете на одного жителя:",
                                                "&nbsp;стационарная медицинская помощь",
                                                "&nbsp;амбулаторная помощь",
                                                "&nbsp;дневные стационары всех типов",
                                                "&nbsp;скорая медицинская помощь",
                                                "29. Стоимость единицы объема оказанной медицинской помощи муниципальными учреждениями здравоохранения:",
                                                "&nbsp;стационарная медицинская помощь",
                                                "&nbsp;амбулаторная помощь",
                                                "&nbsp;дневные стационары всех типов",
                                                "&nbsp;скорая медицинская помощь",
                                                "<b>Раздел IV. Дошкольное и дополнительное образование детей</b>",
                                                "30. Удовлетворенность населения качеством дошкольного образования детей",
                                                "31. Удовлетворенность населения качеством дополнительного образования детей",
                                                "32. Доля детей в возрасте от 3 до 7 лет, получающих дошкольнуюобразовательную услугу и (или) услугу по их содержанию ворганизациях различной организационно-правовой формы и формы собственности, в общей численности детей от 3 до 7 лет",
                                                "33. Удельный вес детей в возрасте 5 - 18 лет, получающих услуги по дополнительному образованию в организациях различной организационно-правовой формы и формы собственности",
                                                "34. Доля детских дошкольных муниципальных учреждений в общем числе организаций, в том числе субъектов малого предпринимательства, оказывающих услуги по содержанию детей в таком учреждении, услуги по дошкольному образованию и получающих средства бюджета городского округа (муниципального района) на оказание таких услуг",
                                                "35. Доля детей в возрасте от 5 до 7 лет, получающих дошкольные образовательные услуги",
                                                "<b>Раздел V. Образование (общее)</b>",
                                                "36.Удовлетворенность населения качеством общего образования",
                                                "37.Удельный вес лиц, сдавшихединый государственный экзамен, в числе выпускников общеобразовательных муниципальных учреждений, участвовавших в едином государственном экзамене",
                                                "38.Доля муниципальных общеобразовательных учреждений, переведенных:",
                                                "&nbsp;на нормативное подушевое финансирование",
                                                "&nbsp;на новую систему оплаты труда, ориентированную на результат",
                                                "39.Доля муниципальных общеобразовательных учреждений с числом учащихся на 3-й ступени обучения (10 - 11 классы) менее 150 человек в городской местности и менее 84 человек в сельской местности в общем числе муниципальных общеобразовательных учреждений",
                                                "40. Численность учащихся, приходящихся на одного работающего в муниципальных общеобразовательных учреждениях всего, в том числе:",
                                                "&nbsp;на одного учителя",
                                                "&nbsp;на одного прочего работающегов муниципальных общеобразовательных учреждениях (административно-управленческого, учебно- вспомогательного, младшего обслуживающего персонала, а также педагогических работников, не осуществляющих учебный процесс)",
                                                "41.Средняя наполняемость классов в муниципальных общеобразовательных учреждениях:",
                                                "&nbsp;в городских поселениях",
                                                "&nbsp;в сельской местности",
                                                "<b>Раздел VI. Физическая культура и спорт</b>",
                                                "42. Удельный вес населения, систематически занимающегося физической культурой и спортом",
                                                "<b>Раздел VII. Жилищно-коммунальное хозяйство</b>",
                                                "43. Доля многоквартирных домов, в которых собственники помещений выбрали и реализуют один изспособов управления многоквартирными домами:",
                                                "&nbsp;непосредственное управление собственниками помещений в многоквартирном доме",
                                                "&nbsp;управление товариществом собственников жилья либо жилищным кооперативом или иным специализированным потребительским кооперативом",
                                                "&nbsp;управление муниципальным или государственным учреждением либо предприятием",
                                                "&nbsp;управление управляющей организацией другой организационно-правовой формы",
                                                "Управление хозяйственным обществом с долей участия в уставном капитале субъекта Российской Федерации и (или)городского округа (муниципального района) не более 25 процентов",
                                                "44. Доля организаций коммунального комплекса, осуществляющих производство товаров, оказание услуг по водо-, тепло-, газо-, электроснабжению, водоотведению, очистке сточных вод, утилизации (захоронению) твердых бытовых отходов и использующих объекты коммунальной инфраструктуры на праве частной собственности, по договору аренды или концессии, участие субъекта Российской Федерации и (или) городского округа (муниципального района) в уставном капитале которых составляет не более 25 процентов, в общем числе организаций коммунального комплекса, осуществляющих свою деятельность на территории городского округа (муниципального района)",
                                                "45. Доля организаций, осуществляющих управление многоквартирными домами и (или) оказание услуг по содержанию и ремонту общего имущества в многоквартирных домах, участие субъекта РФ и (или) городского округа (муниципального района) в уставном капитале которых составляет не более 25%, в общем числе организаций, осуществляющих данные виды деятельности на территории городского округа (муниципального района), кроме товариществ собственников жилья, жилищных, жилищно-строительных кооперативов и иных специализированных потребительских кооперативов",
                                                "46. Доля объема отпуска коммунальных ресурсов, счета за которые выставлены по показаниям приборов учета",
                                                "47. Уровень собираемости платежей за предоставленные жилищно-коммунальные услуги",
                                                "48. Процент подписанных паспортов готовности жилищного фондаи котельных (по состоянию на 15 ноября отчетного года)",
                                                "49. Отношение тарифов для промышленных потребителей к тарифам для населения:",
                                                "&nbsp;по водоснабжению",
                                                "&nbsp;по водоотведению",
                                                "<b>Раздел VIII. Доступность и качество жилья</b>",
                                                "50. Общая площадь жилых помещений, приходящаяся в среднем на одного жителя - всего в том числе",
                                                "&nbsp;введенная в действие за год",
                                                "51. Число жилых квартир в расчете на 1000 человек населения - всего в том числе",
                                                "&nbsp;введенных в действие за год",
                                                "52. Объем жилищного строительства, предусмотренный в соответствии с выданными разрешениями на строительство жилых зданий:",
                                                "&nbsp;общая площадь жилых помещений",
                                                "&nbsp;число жилых квартир",
                                                "53. Доля многоквартирных домов, расположенных на земельных участках, в отношении которых осуществлен государственный кадастровый учет",
                                                "<b>Раздел IX. Организация муниципального управления</b>",
                                                "55. Удовлетворенность населения деятельностью органов местного самоуправления городского округа (муниципального района), в том числе их информационной открытостью",
                                                "56. Доля муниципальных автономных учреждений от общего числа муниципальных учреждений (бюджетных и автономных) в городском округе (муниципальном районе)",
                                                "57. Доля собственных доходов местного бюджета (за исключением безвозмездных поступлений, поступлений налоговых доходов по дополнительным нормативам отчислений и доходов от платных услуг, оказываемых муниципальными бюджетными учреждениями) в общем объеме доходов бюджета муниципального образования",
                                                "58. Удельный вес населения, участвующего в культурно-досуговых мероприятиях, организованных органами местного самоуправления городских округови муниципальных районов",
                                                "59. Удовлетворенность населения качеством предоставляемых услуг в сфере культуры (качеством культурного обслуживания)",
                                                "60. Доля основных фондов организаций муниципальной формы собственности, находящихся в стадии банкротства, в общейстоимости основных фондов организаций муниципальной формы собственности (на конец года)",
                                                "61. Доля кредиторской задолженности по оплате труда (включая начисления на оплату труда) муниципальных бюджетных учреждений",
                                                "62. Доля объектов капитального строительства, по которым несоблюдены нормативные или плановые сроки ввода в эксплуатацию, в общем количестве объектов капитального строительства в том числе",
                                                "&nbsp;доля объектов капитального строительства муниципальной формы собственности, по которым не соблюдены нормативные или плановые сроки ввода в эксплуатацию, в общем количестве объектов капитального строительства муниципальной формы собственности",
                                                "64. Среднегодовая численность постоянного населения, тысяча человек",
                                                "65. Общий объем расходов бюджета муниципального образования — всего, в том числе:",
                                                "&nbsp;на бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;на образование (общее, дошкольное) из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;расходы на оплату труда и начисления на оплату труда",
                                                "&nbsp;на здравоохранение из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;расходы на оплату труда и начисления на оплату труда",
                                                "&nbsp;на культуру из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;расходы на оплату труда и начисления на оплату труда",
                                                "&nbsp;на физическую культуру и спорт из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;расходы на оплату труда и начисления на оплату труда",
                                                "&nbsp;на жилищно-коммунальное хозяйство из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;расходы на компенсацию разницы между экономически обоснованными тарифами и тарифами, установленными для населения",
                                                "&nbsp;расходы на покрытие убытков, возникших в связи сприменением регулируемых цен на жилищно-коммунальные услуги",
                                                "&nbsp;на содержаниеработников органов местного самоуправления из них:",
                                                "&nbsp;на развитие и поддержку малого предпринимательства из них:",
                                                "&nbsp;на транспорт из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств",
                                                "&nbsp;на дорожное хозяйство из них:",
                                                "&nbsp;бюджетные инвестиции на увеличение стоимости основных средств" };
        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размеров
                //web_grid1.Width = (int)((screen_width - 20));

                web_grid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
                //UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 180);

                UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
                UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
                UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
                UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

                UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
                UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                        <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
                UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
                //UltraGridExporter1.MultiHeader = true;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);

                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);

                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    last_year.Value = UserComboBox.getLastBlock(getLastDate());

                    LabelHeader.Text = String.Format(headerText, RegionSettingsHelper.Instance.RegionNameGenitive);

                    Collection<string> years = new Collection<string>();
                    years.Add((Convert.ToInt16(last_year.Value) - 1).ToString());
                    years.Add(last_year.Value);

                    ComboYear.Title = "Год";
                    ComboYear.Width = 100;
                    ComboYear.MultiSelect = false;
                    ComboYear.FillValues(years);
                    ComboYear.SetСheckedState(last_year.Value, true);

                    Collection<string> regions = new Collection<string>();
                    DataTable regionsDT = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("regions"), "regions", regionsDT);
                    for (int i = 0; i < regionsDT.Rows.Count; i++)
                        regions.Add(regionsDT.Rows[i].ItemArray[0].ToString());
                    ComboRegion.Title = "Муниципальное образование";
                    ComboRegion.Width = 500;
                    ComboRegion.MultiSelect = false;
                    ComboRegion.FillValues(regions);
                    ComboRegion.SetСheckedState("Город Рязань", true);

                }
                last_year.Value = ComboYear.SelectedValue;
                region.Value = ComboRegion.SelectedValue;
                web_grid1.DataBind();
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
        }

        // --------------------------------------------------------------------



        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_year"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
                //return null;
            }
        }


        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //DataTable grid1_table = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Область", grid1_table);
                //web_grid1.DataSource = grid1_table.DefaultView;


                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                int columnsCount = grid_set.Cells.Count / grid_set.Axes[1].Positions.Count;
                grid_table.Columns.Add("Показатели");
                grid_table.Columns.Add("Ед. изм.");
                grid_table.Columns.Add("Отчёт");
                grid_table.Columns.Add("Изменение");
                grid_table.Columns.Add("План 1");
                grid_table.Columns.Add("План 2");
                grid_table.Columns.Add("План 3");


                //grid_rows_skip
                int cellsCount = grid_set.Axes[1].Positions.Count;
                int j = 0;
                int skipCount = 0;
                for (int i = 0; i < grid_rows.Length; ++i)
                {
                    object[] values = new object[columnsCount + 1];
                    if (grid_rows_skip[skipCount] == i)
                    {
                        values[0] = grid_rows[i];
                        skipCount++;
                    }
                    else
                    {
                        values[0] = grid_rows[i];
                        values[1] = grid_set.Cells[0, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToLower();
                        double val1 = Convert.ToDouble(grid_set.Cells[1, grid_set.Axes[1].Positions[j].Ordinal].Value);
                        double val2 = Convert.ToDouble(grid_set.Cells[2, grid_set.Axes[1].Positions[j].Ordinal].Value);

                        values[2] = grid_set.Cells[1, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                        //values[2] = grid_set.Cells[2, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                        //values[1] = Math.Round(val1, 2).ToString();
                        if ((val1 == 0) | (val2 == 0))
                            values[3] = " "; 
                        else
                            values[3] = (Math.Round(val2 / val1 * 100, 2)).ToString() + " %"; 
                        for (int k = 3; k < columnsCount; ++k)
                        {
                            values[k + 1] = grid_set.Cells[k, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                        }
                        j++;
                    }
                    grid_table.Rows.Add(values);
                }
/*
                grid_set.Axes[1].Positions.Count

                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = new object[columnsCount + 1];
                    values[0] = grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption;
                    for (int i = 0; i < columnsCount; ++i)
                    {
                        values[i + 1] = grid_set.Cells[i, pos.Ordinal].FormattedValue.ToString();
                    }
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
 */
                web_grid1.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }        
        
        }


        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                e.Layout.HeaderStyleDefault.Font.Bold = true;
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 0;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.52) - 4;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth) * 0.08) - 4;
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.4 / (e.Layout.Bands[0].Columns.Count - 2)) - 4;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
                }
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                string checkedString = e.Row.Cells[0].Value.ToString();
                if (checkedString.Contains("&nbsp;"))

                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("&nbsp;", "");
                    //e.Row.Cells[0].Style.Padding.Left(30);
                }
                if (checkedString.Contains("<b>"))
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("<b>", "");
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("</b>", "");
                    //e.Row.Cells[0].Style.Padding.Left(30);
                }
                /*
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Index == 0) return;
                    if (e.Row.Cells[i].Value.ToString() == "") continue;
                    if (Convert.ToDouble(e.Row.PrevRow.Cells[i].Value) >= Convert.ToDouble(e.Row.Cells[i].Value))
                    {
                        if (Convert.ToDouble(e.Row.PrevRow.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[i].Value)) return;
                        e.Row.Cells[i].Style.CssClass = "ArrowDownRed";
                    }
                    else
                        e.Row.Cells[i].Style.CssClass = "ArrowUpGreen";
                }
                 */
            }
            catch
            {
                // ошибка инициализации
            }
        }



        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = LabelHeader.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = ComboYear.SelectedValue + " год";
            e.CurrentWorksheet.Rows[2].Cells[0].Value = ComboRegion.SelectedValue;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < web_grid1.Rows.Count; i = i + 1)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                //e.CurrentWorksheet.Rows[i].Cells[0].Value = e.CurrentWorksheet.Rows[i].Cells[0].Value.ToString().Replace("&nbsp;", "    ");
            }
            string formatString = "#,##0.00;[Red]-#,##0.00";
            for (int i = 1; i < web_grid1.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 70;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Columns[0].Width = 450 * 37;
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = web_grid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden)
            {
                offset++;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(web_grid1);
        }


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(web_grid1);
        }


        

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(LabelHeader.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(ComboRegion.SelectedValue + " - " + ComboYear.SelectedValue + " год");
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

        }


    }



}

