using System;
using System.Data;
using System.Drawing;
using System.IO;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart; 
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Primitive = Infragistics.UltraChart.Core.Primitives.Primitive; 
using Text = Infragistics.UltraChart.Core.Primitives.Text;

namespace Krista.FM.Server.Dashboards.reports.iPad
{ 
    public partial class SEP_0001_0001_v : CustomReportPage
    {


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Сахалинская область";
            }
            lbInfo.Text = "<div align='justify'><p>Комплексная оценка социально-экономического развития муниципальных образований проводится за отчетный период (ежеквартально накопительным итогом и ежегодно) и осуществляется на основе показателей развития, сгруппированных по сферам:</p>";
            lbInfo.Text += "<ul><li>социальной;</li><li>экономической;</li><li>финансовой;</li></ul><p>а так же на основе формирования двойной рейтинговой шкалы с учетом оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления.</p>";
            lbInfo.Text += "<center><b>Расчет частных индикаторов уровня и динамики развития социальной, экономической и финансовой сфер</b></center>";
            lbInfo.Text += "<p>Большинство показателей развития соответствуют принципу «чем выше числовое значение, тем лучше ситуация». Для таких показателей индикаторы уровня развития социальной, экономической и финансовой сфер рассчитываются как отношение разности между абсолютным значением показателя развития муниципального образования за отчетный период и минимальным значением показателя развития среди муниципальных образований за отчетный период к разности между максимальным значением показателя развития среди муниципальных образований за отчетный период и минимальным значением показателя развития среди муниципальных образований за отчетный период.</p>";
            lbInfo.Text += "<p>Для показателей, которые являются «обратными», т.е. действует принцип «Чем выше числовое значение, тем хуже ситуация», индикаторы рассчитываются как отношение разности между максимальным значением показателя развития среди муниципальных образований за отчетный период и абсолютным значением показателя развития муниципального образования за отчетный период к разности между максимальным значением показателя развития среди муниципальных образований за отчетный период и минимальным значением показателя развития среди муниципальных образований за отчетный период.</p>";
            lbInfo.Text += "<p>Индикаторы динамики развития социальной, экономической и финансовой сфер рассчитывается соответствующим образом, только вместо значений показателей развития используется значение динамики развития. Значение динамики развития рассчитывается как отношение значения показателя развития муниципального образований за отчетный период текущего года к значению за аналогичный период предыдущий года.</p>";
            lbInfo.Text += "<center><b>Расчет интегральных показателей уровня и динамики развития социальной, экономической и финансовой сфер</b></center>";
            lbInfo.Text += "<p>Интегральный показатель уровня развития соответствующей сферы равен сумме произведений частных индикаторов уровня развития соответствующей сферы на весовой коэффициент показателя развития муниципального образования в интегральной оценке развития соответствующей сферы. Соответствующим образом рассчитываются интегральные показатели динамики развития соответствующих сфер.</p>";
            lbInfo.Text += "<center><b>Расчет сводных оценок уровня и динамики развития социальной, экономической и финансовой сфер</b></center>";
            lbInfo.Text += "<p>Для расчета показателей комплексной оценки устанавливаются весовые коэффициенты для каждой из оцениваемых сфер, а также для показателей уровня и динамики.</p>";
            lbInfo.Text += "<p>Показатель сводной оценки уровня и динамики развития определенной сферы равен сумме из первого произведения интегрального показателя уровня развития соответствующей сферы и весового коэффициента показателей уровня развития муниципальных образований в сводной оценке и из второго произведения интегрального показателя динамики развития соответствующей сферы и весового коэффициента показателей динамики развития муниципальных образований в сводной оценке.</p>";
            lbInfo.Text += "<p>Показатель сводной оценки уровня развития социальной, экономической и финансовой сфер рассчитывается как сумма произведений интегрального показателя уровня развития соответствующей сферы на весовой коэффициент показателей соответствующей сферы развития муниципальных образований в сводной оценке.</p>";
            lbInfo.Text += "<p>Показатель сводной оценки динамики развития социальной, экономической и финансовой сфер рассчитывается как сумма произведения интегрального показателя динамики развития соответствующей сферы на весовой коэффициент показателей соответствующей сферы развития муниципальных образований в сводной оценке.</p>";
            lbInfo.Text += "<center><b>Расчет итоговых сводных оценок социально-экономического развития муниципальных образований</b></center>";
            lbInfo.Text += "<p>Итоговая сводная оценка социально-экономического развития муниципальных образований состоит из двух слагаемых. Первое слагаемое равно произведению показателя сводной оценки уровня развития социальной, экономической и финансовой сфер и весового коэффициента показателей уровня развития муниципальных образований в сводной оценке. Второе слагаемое равно произведению показателя сводной оценки динамики развития социальной, экономической и финансовой сфер и весового коэффициента показателей динамики развития муниципальных образований в сводной оценке.</p>";
            lbInfo.Text += "<center><b>Расчет частных индикаторов оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления</b></center>";
            lbInfo.Text += "<p>Частные индикаторы оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления рассчитываются как отношение разности между абсолютным значением показателя оценки населением ситуации в сфере деятельности органов местного самоуправления за отчетный период и минимальным значением показателя оценки населением ситуации в сфере деятельности органов местного самоуправления среди муниципальных образований за отчетный период к разности между максимальным значением показателя оценки населением ситуации в сфере деятельности органов местного самоуправления среди муниципальных образований за отчетный период и минимальным значением показателя оценки населением ситуации в сфере деятельности органов местного самоуправления среди муниципальных образований за отчетный период.</p>";
            lbInfo.Text += "<center><b>Расчет интегральной оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления</b></center>";
            lbInfo.Text += "<p>Интегральная оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления равна сумме произведений частных индикаторов оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления и весового коэффициента показателя оценки населением ситуации в сфере деятельности органов местного самоуправления.</p>";
            lbInfo.Text += "<center><b>Итоги комплексной оценки социально-экономического развития муниципальных образований</b></center>";
            lbInfo.Text += "<p>Подведение итогов комплексной оценки социально-экономического развития муниципальных образований производится путем формирования рейтинга муниципальных образований на основе двух оценок:</p>";
            lbInfo.Text += "<ul><li>Итоговая сводная оценка социально-экономического развития муниципального образования R;</li><li>Интегральная оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления Rp.</li></ul><br></div>";
            InitializeTable1();
            lbInfo1.Text = "<div align='justify'><p>Итоги комплексной оценки социально-экономического развития муниципальных образований представляются в виде таблицы:</p></div>";
            InitializeTable2();
        }

        private void InitializeTable1()
        {
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);

            DataTable dt = new DataTable();
            dt.Columns.Add("Место в рейтинге по показателю комплексной оценки R", typeof(string));
            dt.Columns.Add("Шкала R", typeof(string));
            dt.Columns.Add("Комплексная оценка социально-экономического развития муниципального образования", typeof(string));
            dt.Columns.Add("Место в рейтинге по результатам оценки населением Rp", typeof(string));
            dt.Columns.Add("Шкала Rp", typeof(string));
            dt.Columns.Add("Оценка населением ситуации в ключевых сферах", typeof(string));

            object[] o = new object[dt.Columns.Count];
            o[0] = "1−6";
            o[1] = "А";
            o[2] = "Высокая";
            o[3] = "1−6";
            o[4] = "+";
            o[5] = "Высокая";
            dt.Rows.Add(o);

            o[0] = "7−13";
            o[1] = "B";
            o[2] = "Средняя";
            o[3] = "7−13";
            o[4] = "±";
            o[5] = "Средняя";
            dt.Rows.Add(o);

            o[0] = "14−19";
            o[1] = "C";
            o[2] = "Низкая ";
            o[3] = "14−19";
            o[4] = "−";
            o[5] = "Низкая ";
            dt.Rows.Add(o);

            UltraWebGrid1.DataSource = dt;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;


            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[5].CellStyle.Font.Size = 14;
            //e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[0].Width = 122;
            e.Layout.Bands[0].Columns[1].Width = 100;
            e.Layout.Bands[0].Columns[2].Width = 144;
            e.Layout.Bands[0].Columns[3].Width = 122;
            e.Layout.Bands[0].Columns[4].Width = 100;
            e.Layout.Bands[0].Columns[5].Width = 144;
        }

        private void InitializeTable2()
        {
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);

            DataTable dt = new DataTable();
            dt.Columns.Add("R\\Rp", typeof(string));
            dt.Columns.Add("-", typeof(string));
            dt.Columns.Add("±", typeof(string));
            dt.Columns.Add("+", typeof(string));

            object[] o = new object[dt.Columns.Count];
            o[0] = "A";
            o[1] = "МО (R;Rp)<br>. . .";
            o[2] = "МО (R;Rp)<br>. . .";
            o[3] = "МО (R;Rp)<br>. . .";
            dt.Rows.Add(o);

            o[0] = "B";
            o[1] = "МО (R;Rp)<br>. . .";
            o[2] = "МО (R;Rp)<br>. . .";
            o[3] = "МО (R;Rp)<br>. . .";
            dt.Rows.Add(o);

            o[0] = "C";
            o[1] = "МО (R;Rp)<br>. . .";
            o[2] = "МО (R;Rp)<br>. . .";
            o[3] = "МО (R;Rp)<br>. . .";
            dt.Rows.Add(o);

            UltraWebGrid2.DataSource = dt;
            UltraWebGrid2.DataBind();
        }

        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;


            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 14;
            //e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[0].Width = 183;
            e.Layout.Bands[0].Columns[1].Width = 183;
            e.Layout.Bands[0].Columns[2].Width = 183;
            e.Layout.Bands[0].Columns[3].Width = 183;

        }
    }
}