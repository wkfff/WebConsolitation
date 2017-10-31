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

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

/**
 *  «Социальное и экономическое развитие муниципального образования «Город Губкинский».
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0001
{
    public partial class _default : Dashboards.CustumReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Социальное и экономическое развитие муниципального образования «Город Губкинский»";
        // содержание изменяемых текстов отчётов
        private static String text1 = "Город Губкинский был основан 22 апреля 1986 года. В&nbsp;1996 году он получил статус города окружного значения. Город расположен в&nbsp;двухстах километрах от Полярного круга в&nbsp;северо-восточной части Западно-Сибирской низменности в&nbsp;лесотундровой зоне на левом берегу реки Пяку-Пур. В&nbsp;16&nbsp;км находится станция Пурпе и&nbsp;железная дорога Тюмень-Сургут-Новый Уренгой. Численность населения по состоянию на конец {0}&nbsp;г. составляет {1}&nbsp;человек. Плотность населения составляет {2:F2}&nbsp;человек на гектар.";
        private static String text2 = "В {0} году на территории муниципального образования работало {1}&nbsp;предприятий, учреждений и&nbsp;организаций всех форм собственности и&nbsp;различных видов деятельности. Объем продукции промышленного производства в&nbsp;действующих ценах за {2}&nbsp;год составил {3}&nbsp;млн.руб., в&nbsp;том числе продукция нефтедобывающей промышленности составила {4}&nbsp;млн.руб.";
        private static String text3 = "Объем платных услуг, реализованных населению в&nbsp;{0}&nbsp;г. составил {1:F1}&nbsp;млн.руб., {2}&nbsp;{3}&nbsp;г. В&nbsp;структуре стоимости платных услуг наибольший удельный вес занимают услуги ЖКХ и&nbsp;коммунальные <nobr>платежи - {4:p1}</nobr>, значительное место занимают транспортные <nobr>услуги - {5:p1}</nobr>, услуги <nobr>связи - {6:p1}</nobr>, услуги <nobr>образования - {7:p1}</nobr>. Общий объем розничного товарооборота за {8}&nbsp;г. составил {9:F1}&nbsp;млн.руб.{10}.";
        private static String text4 = "Среднесписочная численность работающих на предприятиях и&nbsp;в&nbsp;организациях муниципального образования в&nbsp;{0}&nbsp;году составила {1}&nbsp;человек, {2}, при этом среднемесячный доход рабочих и служащих по отраслям (на 1&nbsp;работающего, в&nbsp;среднем) в&nbsp;{3} году составил {4}&nbsp;рублей, {5}.";
        private static String text5 = "Общая заболеваемость населения в&nbsp;{0}&nbsp;году составила {1:F0} случая на 1000 человек населения, {2} годом на {3:F0} случаев на 1000 человек населения. Численность врачей составляет {4} человек, численность среднего медицинского персонала {5} человек. Мощность амбулаторно-поликлинического отделения составляет {6} посещений в&nbsp;смену, мощность стационарного отделения составляет {7} коек.";
        private static String text6 = "В {0} году объем инвестиций в&nbsp;основной капитал по предприятиям в&nbsp;целом, с&nbsp;учетом обособленных подразделений {1}&nbsp;млн.&nbsp;руб., {2}, в&nbsp;том числе инвестиции производственного назначения {3:F1}&nbsp;млн.&nbsp;руб., а непроизводственного назначения {4:F1}&nbsp;млн.&nbsp;руб. Источниками финансирования являлись: средства бюджета {5:F1}&nbsp;млн.&nbsp;руб., собственные средства предприятий {6:F1}&nbsp;млн.&nbsp;руб., привлеченные средства {7:F1}&nbsp;млн.&nbsp;руб.";
        private static String text7 = "На конец {0} года общая площадь жилого фонда составила {1:F1}&nbsp;тыс.&nbsp;м<sup>2</sup>. Обеспеченность населения жильем при нормативе {2:F1}&nbsp;м<sup>2</sup> общей площади на одного человека составляет {3:F1}&nbsp;м<sup>2</sup> или {4:p1}. Площадь ветхого и&nbsp;аварийного жилья (без балок, вагончиков, малопригодного жилья) составляет {5:F1}&nbsp;тыс.&nbsp;м<sup>2</sup>. На протяжении ряда лет в&nbsp;городе работает и&nbsp;совершенствуется система строительства жилья, основанная на привлечении средств по долевому участию юридических и&nbsp;физических лиц.";                

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);

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

                }

                // установка заголовка для страницы
                page_title.Text = page_title_caption;

                // установка ссылок на подробные отчёты
                HyperLink1.NavigateUrl = "~/reports/PMO_0001_0002/default.aspx";
                HyperLink2.NavigateUrl = "~/reports/PMO_0001_0003/default.aspx";
                HyperLink3.NavigateUrl = "~/reports/PMO_0001_0004/default.aspx";
                HyperLink4.NavigateUrl = "~/reports/PMO_0001_0005/default.aspx";
                HyperLink5.NavigateUrl = "~/reports/PMO_0001_0006/default.aspx";
                HyperLink6.NavigateUrl = "~/reports/PMO_0001_0007/default.aspx";
                HyperLink7.NavigateUrl = "~/reports/PMO_0001_0008/default.aspx";
                HyperLink8.NavigateUrl = "~/reports/PMO_0001_0009/default.aspx";
                HyperLink9.NavigateUrl = "~/reports/PMO_0001_00010/default.aspx";
                HyperLink10.NavigateUrl = "~/reports/PMO_0001_0011/default.aspx";

                // переменные для работы с данными
                CellSet cs1, cs2;
                String LastYear, LastYear2, PrevYear, PrevYear2, valueState, valueState2;
                float lastYearValue, prevYearValue, lastYearValue2, prevYearValue2, valueSize = 0,
                    ZKH, Transport, Connection, Education, 
                    Doc, Med, Pow, Bed,
                    Production, NonProduction, Budget, Self,
                    Debt, Norma, Real, Slum;

                // Территория и население
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("territoryLastYearPeople"));
                cs2 = PrimaryMASDataProvider.GetCellset(GetQueryText("territoryLastYearDensity"));
                // заполнение текста отчёта
                Label1.Text = String.Format(
                    text1,
                    core.UserComboBox.getLastBlock(cs1.Axes[1].Positions[0].Members[0].ToString()),
                    cs1.Cells[0].Value.ToString(),
                    float.Parse(cs2.Cells[0].Value.ToString()));

                // Социально-экономическое развитие
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("economIndex"));
                // заполнение текста отчёта
                Label2.Text = String.Format(
                    text2,
                    core.UserComboBox.getLastBlock(cs1.Axes[1].Positions[0].Members[1].ToString()),
                    cs1.Cells[0].Value.ToString(),
                    core.UserComboBox.getLastBlock(cs1.Axes[1].Positions[1].Members[1].ToString()),
                    cs1.Cells[1].Value.ToString(),
                    cs1.Cells[2].Value.ToString());

                // Торговля, платные услуги и общ. питание
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("tradeIndex"));
                LastYear = cs1.Axes[1].Positions[0].Members[1].ToString();
                LastYear = core.UserComboBox.getLastBlock(LastYear);
                PrevYear = cs1.Axes[1].Positions[1].Members[1].ToString();
                PrevYear = core.UserComboBox.getLastBlock(PrevYear);
                lastYearValue = float.Parse(cs1.Cells[0].Value.ToString()) / 1000;
                prevYearValue = float.Parse(cs1.Cells[1].Value.ToString()) / 1000;

                ZKH = float.Parse(cs1.Cells[2].Value.ToString()) / 100;
                Transport = float.Parse(cs1.Cells[3].Value.ToString()) / 100;
                Connection = float.Parse(cs1.Cells[4].Value.ToString()) / 100;
                Education = float.Parse(cs1.Cells[5].Value.ToString()) / 100;

                LastYear2 = cs1.Axes[1].Positions[6].Members[1].ToString();
                LastYear2 = core.UserComboBox.getLastBlock(LastYear2);
                PrevYear2 = cs1.Axes[1].Positions[7].Members[1].ToString();
                PrevYear2 = core.UserComboBox.getLastBlock(PrevYear2);
                lastYearValue2 = float.Parse(cs1.Cells[6].Value.ToString()) / 1000;
                prevYearValue2 = float.Parse(cs1.Cells[7].Value.ToString()) / 1000;

                // определение текстовой строки для подстановки в отчёт
                if (lastYearValue >= prevYearValue)
                {
                    if (lastYearValue == prevYearValue)
                    {
                        valueState = "не изменилось, по сравнению с";
                    }
                    else
                    {
                        valueState = "что на " + String.Format("{0:F1}", lastYearValue - prevYearValue) + "&nbsp;млн.руб. больше, чем в";
                    }
                }
                else
                {
                    valueState = "что на " + String.Format("{0:F1}", prevYearValue - lastYearValue) + "&nbsp;млн.руб. меньше, чем в";
                }

                // определение второй текстовой строки для подстановки в отчёт
                if (lastYearValue2 >= prevYearValue2)
                {
                    if (lastYearValue2 == prevYearValue2)
                    {
                        valueState2 = ", не изменилось по сравнению с " + PrevYear2 + " г";
                    }
                    else
                    {
                        valueSize = lastYearValue2 / prevYearValue2 - 1;
                        valueState2 = " с ростом в фактических ценах против " + PrevYear2 + "&nbsp;г. на&nbsp;" + String.Format("{0:p1}", valueSize);
                    }
                }
                else
                {
                    valueSize = 1 - lastYearValue2 / prevYearValue2;
                    valueState2 = " с уменьшением в фактических ценах против " + PrevYear2 + "&nbsp;г. на&nbsp;" + String.Format("{0:p1}", valueSize);
                }

                // заполнение текста отчёта
                Label3.Text = String.Format(
                    text3,
                    LastYear,
                    lastYearValue,
                    valueState,
                    PrevYear,
                    ZKH,
                    Transport,
                    Connection,
                    Education,
                    LastYear2,
                    lastYearValue2,
                    valueState2);

                // Труд и заработная плата
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("labourIndex"));
                LastYear = cs1.Axes[1].Positions[0].Members[1].ToString();
                LastYear = core.UserComboBox.getLastBlock(LastYear);
                PrevYear = cs1.Axes[1].Positions[1].Members[1].ToString();
                PrevYear = core.UserComboBox.getLastBlock(PrevYear);
                lastYearValue = float.Parse(cs1.Cells[0].Value.ToString());
                prevYearValue = float.Parse(cs1.Cells[1].Value.ToString());
                LastYear2 = cs1.Axes[1].Positions[2].Members[1].ToString();
                LastYear2 = core.UserComboBox.getLastBlock(LastYear2);
                PrevYear2 = cs1.Axes[1].Positions[3].Members[1].ToString();
                PrevYear2 = core.UserComboBox.getLastBlock(PrevYear2);
                lastYearValue2 = float.Parse(cs1.Cells[2].Value.ToString());
                prevYearValue2 = float.Parse(cs1.Cells[3].Value.ToString());

                // определение текстовой строки для подстановки в отчёт
                if (lastYearValue >= prevYearValue)
                {
                    if (lastYearValue == prevYearValue)
                    {
                        valueState = ", не изменилось по сравнению с " + PrevYear + " г";
                    }
                    else
                    {
                        valueSize = lastYearValue / prevYearValue - 1;
                        valueState = " прирост по сравнению с&nbsp;" + PrevYear + "&nbsp;г. составил&nbsp;" + String.Format("{0:p1}", valueSize);
                    }
                }
                else
                {
                    valueSize = 1 - lastYearValue / prevYearValue;
                    valueState = " убыль по сравнению с&nbsp;" + PrevYear + "&nbsp;г. составила&nbsp;" + String.Format("{0:p1}", valueSize);
                }

                // определение второй текстовой строки для подстановки в отчёт
                if (lastYearValue2 >= prevYearValue2)
                {
                    if (lastYearValue2 == prevYearValue2)
                    {
                        valueState2 = "не изменилось по сравнению с " + PrevYear2 + " г";
                    }
                    else
                    {
                        valueSize = lastYearValue2 / prevYearValue2 - 1;
                        valueState2 = "прирост по сравнению с&nbsp;" + PrevYear2 + "&nbsp;г. составил&nbsp;" + String.Format("{0:p1}", valueSize);
                    }
                }
                else
                {
                    valueSize = 1 - lastYearValue2 / prevYearValue2;
                    valueState2 = "убыль по сравнению с&nbsp;" + PrevYear2 + "&nbsp;г. составила&nbsp;" + String.Format("{0:p1}", valueSize);
                }

                // заполнение текста отчёта
                Label4.Text = String.Format(
                    text4,
                    LastYear,
                    lastYearValue,
                    valueState,
                    LastYear2,
                    lastYearValue2,
                    valueState2);


                // Мед.обслуживание и соц.обеспечение
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("medIndex"));
                LastYear = cs1.Axes[1].Positions[0].Members[0].ToString();
                LastYear = core.UserComboBox.getLastBlock(LastYear);
                PrevYear = cs1.Axes[1].Positions[0].Members[0].ToString();
                PrevYear = core.UserComboBox.getLastBlock(PrevYear);
                lastYearValue = float.Parse(cs1.Cells[0].Value.ToString());
                prevYearValue = float.Parse(cs1.Cells[1].Value.ToString());
                Doc = float.Parse(cs1.Cells[2].Value.ToString());
                Med = float.Parse(cs1.Cells[3].Value.ToString());
                Pow = float.Parse(cs1.Cells[4].Value.ToString());
                Bed = float.Parse(cs1.Cells[5].Value.ToString());

                // определение текстовой строки для подстановки в отчёт
                if (lastYearValue >= prevYearValue)
                {
                    if (lastYearValue == prevYearValue)
                    {
                        valueState = " не изменилась по сравнению с&nbsp;" + PrevYear;
                    }
                    else
                    {
                        valueSize = lastYearValue - prevYearValue;
                        valueState = " увеличилась по сравнению с&nbsp;" + PrevYear;
                    }
                }
                else
                {
                    valueSize = prevYearValue - lastYearValue;
                    valueState = " уменьшилась по сравнению с&nbsp;" + PrevYear;
                }

                // заполнение текста отчёта
                Label5.Text = String.Format(
                    text5,
                    LastYear,
                    lastYearValue,
                    valueState,
                    valueSize,
                    Doc,
                    Med,
                    Pow,
                    Bed);

                // Инвестиции и строительство
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("investIndex"));
                LastYear = cs1.Axes[1].Positions[0].Members[1].ToString();
                LastYear = core.UserComboBox.getLastBlock(LastYear);
                PrevYear = cs1.Axes[1].Positions[1].Members[1].ToString();
                PrevYear = core.UserComboBox.getLastBlock(PrevYear);
                lastYearValue = float.Parse(cs1.Cells[0].Value.ToString());
                prevYearValue = float.Parse(cs1.Cells[1].Value.ToString());
                Production = float.Parse(cs1.Cells[2].Value.ToString());
                NonProduction = float.Parse(cs1.Cells[3].Value.ToString());
                Budget = float.Parse(cs1.Cells[4].Value.ToString());
                Self = float.Parse(cs1.Cells[5].Value.ToString());
                Debt = float.Parse(cs1.Cells[6].Value.ToString());

                // определение текстовой строки для подстановки в отчёт
                if (lastYearValue >= prevYearValue)
                {
                    if (lastYearValue == prevYearValue)
                    {
                        valueState = " не изменилось по сравнению с " + PrevYear + " годом";
                    }
                    else
                    {
                        valueSize = lastYearValue / prevYearValue - 1;
                        valueState = " прирост инвестиций по сравнению с&nbsp;" + PrevYear + "&nbsp;годом составил&nbsp;" + String.Format("{0:p1}", valueSize);
                    }
                }
                else
                {
                    valueSize = 1 - lastYearValue / prevYearValue;
                    valueState = " прирост инвестиций по сравнению с&nbsp;" + PrevYear + "&nbsp;годом составила&nbsp;" + String.Format("{0:p1}", valueSize);
                }

                // заполнение текста отчёта
                Label6.Text = String.Format(
                    text6,
                    LastYear,
                    lastYearValue,
                    valueState,
                    Production,
                    NonProduction,
                    Budget,
                    Self,
                    Debt);

                // Жилищный фонд
                cs1 = PrimaryMASDataProvider.GetCellset(GetQueryText("housingLastYear"));
                LastYear = cs1.Axes[1].Positions[0].Members[1].ToString();
                LastYear = core.UserComboBox.getLastBlock(LastYear);
                lastYearValue = float.Parse(cs1.Cells[0].Value.ToString());
                Norma = float.Parse(cs1.Cells[1].Value.ToString());
                Real = float.Parse(cs1.Cells[2].Value.ToString());
                Slum = float.Parse(cs1.Cells[3].Value.ToString());

                // Вычисление отношений данных в процентах
                valueSize = Real / Norma;

                // заполнение текста отчёта
                Label7.Text = String.Format(
                    text7,
                    LastYear,
                    lastYearValue,
                    Norma,
                    Real,
                    valueSize,
                    Slum);
            }
            catch (Exception)
            {
                // неудачная загрузка ...
            }
        }

        protected void HeaderPR1_Load(object sender, EventArgs e)
        {

        }

    }

}

