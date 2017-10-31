using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0004_wm1_H : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int monthNum =  CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            CRHelper.SetNextMonth(ref yearNum, ref monthNum);

            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label1.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            LabelTitle1.Text = string.Format("Динамика по налогу на прибыль за {0}-{1} год по {2} и темп роста к аналогичному периоду прошлого года:",
                                   UserParams.PeriodLastYear.Value,
                                   UserParams.PeriodYear.Value,
                                   UserParams.ShortStateArea.Value);

            LabelTitle2.Text = string.Format("Динамика по НДФЛ за {0}-{1} год по {2} и темп роста к аналогичному периоду прошлого года:",
                       UserParams.PeriodLastYear.Value,
                       UserParams.PeriodYear.Value,
                       UserParams.ShortStateArea.Value);


            LabelTitle3.Text = string.Format("Динамика общей суммы доходов за {0}-{1} год по {2} и темп роста к аналогичному периоду прошлого года:",
                       UserParams.PeriodLastYear.Value,
                       UserParams.PeriodYear.Value,
                       UserParams.ShortStateArea.Value);

            LabelComments1.Text = "За месяц млн.руб. = поступления за месяц в бюджет субъекта";
            LabelComments2.Text = "Назнач. млн.руб. = план поступлений на год";
            LabelComments3.Text = "Исполн. млн.руб. = поступления нарастающим итогом с начала года";
            LabelComments4.Text = "Исп. % = процент выполнения назначений (плана)";
            LabelComments5.Text = "Темп роста % = темп роста исполнения нарастающим итогом к аналогичному периоду предыдущего года";

            FillData1();
            FillData2();
            FillData3();
        }

        private void FormatDt()
        {
            List<int> inserts = new List<int>();

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                DataRow row = dt.Rows[j];

                if (row[0].ToString() == "Январь")
                {
                    inserts.Add(j + inserts.Count);
                }

                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    if (i < 5 && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                    else
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                    }
                }
            }

            for (int i = 0; i < inserts.Count; i++)
            {
                DataRow r = dt.NewRow();
                r[0] = dt.Rows[inserts[i]].ItemArray[1].ToString();
                dt.Rows.InsertAt(r, inserts[i]);
            }
        }

        protected void FillData1()
        {
            dt = new DataTable();
            UserParams.KDGroup.Value = "[КД].[Сопоставимый].[Все коды доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ].[Налог на прибыль организаций]";
            string query = DataProvider.GetQueryText("FK_0001_0004_H");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);
            
            FormatDt();

            SetDataTable(gridTable1, dt);
        }


        protected void FillData2()
        {
            dt = new DataTable();
            UserParams.KDGroup.Value = "[КД].[Сопоставимый].[Все коды доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ].[Налог на доходы физических лиц]";
            string query = DataProvider.GetQueryText("FK_0001_0004_H");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            FormatDt();

            SetDataTable(gridTable2, dt);
        }

        protected void FillData3()
        {
            dt = new DataTable();
            UserParams.KDGroup.Value = "[КД].[Сопоставимый].[Все коды доходов].[Доходы - всего в том числе:]";
            string query = DataProvider.GetQueryText("FK_0001_0004_H");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            FormatDt();

            SetDataTable(gridTable3, dt);
        }

        private void SetDataTable(Table gridTable,  DataTable dtGrid)
        {
            TableRow rowHeader = new TableRow();

            int fontSize = CRHelper.fontFK0004H;
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            CRHelper.AddCaptionCell(rowHeader, "Месяц", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "За месяц млн.руб.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Назнач. млн.руб.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Исполн. млн.руб.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Исп. %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Темп роста %", fontSize, captionColor);
            gridTable.Rows.Add(rowHeader);

            for (int i = 0; i < dtGrid.Rows.Count; i++) 
            {
                TableRow row = new TableRow();
                Color firtsCellColor = fontColor;
                if (dtGrid.Rows[i][2].ToString() == string.Empty)
                {
                    firtsCellColor = mainColumnColor;
                }

                CRHelper.AddDataCellL(row, string.Format("{0}", dtGrid.Rows[i][0]), fontSize, firtsCellColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][2]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][3]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][4]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][5]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][6]), fontSize, mainColumnColor);

                gridTable.Rows.Add(row);
            }
            for (int i = 0; i < gridTable.Rows.Count - 1; i++)
            {
                if (dtGrid.Rows[i][2].ToString() == string.Empty)
                {
                    CRHelper.SetCellHBorderNone(gridTable.Rows[i + 1], false);
                    CRHelper.SetCellHBorderNone(gridTable.Rows[i + 2], true);
                }
            }
        }
    }
}
