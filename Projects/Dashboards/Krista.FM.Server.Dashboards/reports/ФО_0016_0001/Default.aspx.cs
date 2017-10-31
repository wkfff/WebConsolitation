using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.WebDataInput;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Microsoft.AnalysisServices.AdomdClient;
using Line=Infragistics.UltraChart.Core.Primitives.Line;
using Text=Infragistics.UltraChart.Core.Primitives.Text;

namespace Krista.FM.Server.Dashboards.reports.UFK_0016_0001
{
    public partial class Default : CustomReportPage
    {

        #region Наборы данных страницы
        
        //общий датасет для таблицы
        private DataSet tableDataSet = new DataSet("TableDataSet");       
        private DataTable masterdt = new DataTable();
        //для диаграммы
        private DataTable chartdt = new DataTable();
        //нормативное значение
        private double normValue = 0;

        #endregion
        
        //Семафор для управления коллизиями обработчиков смены активной ячейки и строки
        private bool AllowChangeRowHandling = true;
    
        /// <summary>
        /// Переключить тип детали
        /// </summary>
        private void SwitchDetails(bool ShowRegions)
        {
            RowDetailPanel.Visible = !ShowRegions;
            ColumnDetailPanel.Visible = ShowRegions;        
        }
    
        /// <summary>
        /// Предзагузка страницы
        /// </summary>
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
           
            //расчет размеров элемента
            try
            {
                //на сколько пикселов деталь больше мастера
                int DetailGreaterMaster = 50;

                chart.Data.ZeroAligned = true;
                chart.Axis.X.Labels.SeriesLabels.Visible = false;

                //Инициализируем размеры элементов
                double widthDirty = ((int)Session["width_size"] - 50);
                MasterTable.Width = (int)Math.Ceiling(widthDirty);
                RowDetailPanel.Width = (int)Math.Ceiling(widthDirty);
                ColumnDetailPanel.Width = (int)Math.Ceiling(widthDirty);
                chart.Width = (int)Math.Ceiling(widthDirty * 0.8);
                

                int HalfHeightDirty = (int)Math.Ceiling((double)(((int)Session["height_size"] - 300) /2));
                MasterTable.Height = HalfHeightDirty - DetailGreaterMaster;
                chart.Height = HalfHeightDirty + DetailGreaterMaster;
                RowDetailPanel.Height = HalfHeightDirty + DetailGreaterMaster;
                ColumnDetailPanel.Height = HalfHeightDirty + DetailGreaterMaster;
                
                
                //Инициализируем размеры гэйджев
                int gaugeWidth = (int)Math.Ceiling((widthDirty - 10) / 8);
                int gaugeHeigh = (int)Math.Ceiling(((double)(ColumnDetailPanel.Height.Value - 50) / 2));
                                                                                
                GaugeBK1.Width = gaugeWidth;
                GaugeBK1.Height = gaugeHeigh;
                GaugeBK2.Width = gaugeWidth;
                GaugeBK2.Height = gaugeHeigh;
                GaugeBK3_obl.Width = gaugeWidth;
                GaugeBK3_obl.Height = gaugeHeigh;
                GaugeBK3_mest.Width = gaugeWidth;
                GaugeBK3_mest.Height = gaugeHeigh;
                GaugeBK4.Width = gaugeWidth;
                GaugeBK4.Height = gaugeHeigh;
                GaugeBK5.Width = gaugeWidth;
                GaugeBK5.Height = gaugeHeigh;
                GaugeBK6_a.Width = gaugeWidth;
                GaugeBK6_a.Height = gaugeHeigh;
                GaugeBK6_b.Width = gaugeWidth;
                GaugeBK6_b.Height = gaugeHeigh;
                GaugeBK6_v.Width = gaugeWidth;
                GaugeBK6_v.Height = gaugeHeigh;
                GaugeBK7.Width = gaugeWidth;
                GaugeBK7.Height = gaugeHeigh;
                GaugeBK8.Width = gaugeWidth;
                GaugeBK8.Height = gaugeHeigh;
                GaugeBK9.Width = gaugeWidth;
                GaugeBK9.Height = gaugeHeigh;
                GaugeBK10.Width = gaugeWidth;
                GaugeBK10.Height = gaugeHeigh;
                GaugeBK11.Width = gaugeWidth;
                GaugeBK11.Height = gaugeHeigh;                                

            }
            catch
            {
            }
                        
        }

        /// <summary>
        /// Загрузка страницы
        /// </summary>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            SubmitButton.Appearance.Style.ForeColor = Color.Black;
            SubmitButton.Appearance.Style.BorderColor = Color.Gray;
            
            if (!Page.IsPostBack)
            {
                RowDetailPanel.Visible = false;
                ColumnDetailPanel.Visible = false;

                ComboYear.SelectedIndex = 10;
                ComboQuarter.SelectedIndex = 1;
                
                string initPeriodValue = "[Период].[Год Квартал Месяц].[Данные всех периодов].[2008].[Полугодие 1].[Квартал 2]";
                UserParams.PeriodYQM_Quarter.Value = initPeriodValue;
                MasterTable.DataBind();

            }

            //заголовок отчета
            string curTitle = "Результаты мониторинга БК и КУ в разрезе районов";
            if (ComboQuarter.SelectedIndex < 4)
            {
                curTitle += string.Format(" за {0} квартал {1} года", ComboQuarter.SelectedIndex + 1, ComboYear.SelectedRow.Cells[0].Value);
            }
            else
            {
                curTitle += string.Format(" за {0} год", ComboYear.SelectedRow.Cells[0].Value);
            }
            PageTitle.InnerText = curTitle;
        }        
        
        /// <summary>
        /// Минимальное и максимальное значение в колонке таблицы
        /// </summary>
        private void GetMinMaxValueFromMasterTable(int ColumnNum, out double max, out double min)
        {
            max = -100000000;
            min = 100000000;
            double curValue;
            foreach (UltraGridRow row in MasterTable.Rows)
	        {
	            if ((row.Cells[ColumnNum].Value != null) &&  (double.TryParse(row.Cells[ColumnNum].Value.ToString(), out curValue)))
	            {
                    max = Math.Max(curValue, max);
                    min = Math.Min(curValue, min);
	            }
	        }	        
        }

        /// <summary>
        /// Установка граничных значений шкалы гэйджа
        /// </summary>
        private void SetGaugeScaleBounds(UltraGauge gauge, int masterTableColumnNum, int masterTableRowNum)
        {
            double minValue;
            double maxValue;        
            GetMinMaxValueFromMasterTable(masterTableColumnNum, out maxValue, out minValue);

            double checkValue = (double)((LinearGauge)gauge.Gauges[0]).Scales[0].Markers[1].Value;
            
            maxValue = Math.Max(maxValue, checkValue + 0.5);
            minValue = Math.Min(minValue, checkValue - 0.5);


            ((LinearGauge)gauge.Gauges[0]).Scales[0].Axis.SetStartValue(minValue);
            ((LinearGauge)gauge.Gauges[0]).Scales[0].Axis.SetEndValue(maxValue);

            double tickInterval = (maxValue - minValue) / 3;
            ((LinearGauge)gauge.Gauges[0]).Scales[0].Axis.SetTickmarkInterval(tickInterval);

            double actualValue;
            if (MasterTable.Rows[masterTableRowNum].Cells[masterTableColumnNum].Value != null)
            {
                ((LinearGauge)gauge.Gauges[0]).Scales[0].Markers[0].Visible = true;
                actualValue = double.Parse(MasterTable.Rows[masterTableRowNum].Cells[masterTableColumnNum].Value.ToString());
            }
            else
            {
                ((LinearGauge)gauge.Gauges[0]).Scales[0].Markers[0].Visible = false;
                return;
            }

            //  есть нарушение
            bool krime = (MasterTable.Rows[masterTableRowNum].Cells[masterTableColumnNum + 1].Value != null) ? 
                          Convert.ToInt32(MasterTable.Rows[masterTableRowNum].Cells[masterTableColumnNum + 1].Value) == 1 :
                          false; 

            ((LinearGauge)gauge.Gauges[0]).Scales[0].Markers[0].Value = actualValue;

            gauge.ToolTip = actualValue.ToString(); 
            
            BrushElement be = ((LinearGauge)gauge.Gauges[0]).Scales[0].Markers[0].BrushElement;
            if (krime)
            {
                ((SimpleGradientBrushElement)be).StartColor = Color.Pink;
                ((SimpleGradientBrushElement)be).EndColor = Color.Red;
            }
            else
	        {
                ((SimpleGradientBrushElement)be).StartColor = Color.LightGreen;
                ((SimpleGradientBrushElement)be).EndColor = Color.Green;                
	        }                                                  
        }
        
        /// <summary>
        /// запрос описания индикатора
        /// </summary>
        private void FetchIndicatorDescr(string IndName)
        {
            #warning вынести в настройку
            string MDX = "SELECT {[Показатели].[БККУ_Сопоставимый].[Данные всех источников].[" + IndName + "]} " 
                + "Dimension Properties [Показатели].[БККУ_Сопоставимый].[Наименование], "
                + "[Показатели].[БККУ_Сопоставимый].[Содержание], "
                + "[Показатели].[БККУ_Сопоставимый].[Формула], "
                + "[Показатели].[БККУ_Сопоставимый].[Условия проверки], "
                + "[Показатели].[БККУ_Сопоставимый].[Пороговое значение] on columns "
                + "FROM [ФО_БККУ_Показатели]";
                
            CellSet cls = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(MDX);
            if (cls != null)
            {
                try
                {
                    descrTD1.InnerText = cls.Axes[0].Positions[0].Members[0].MemberProperties[0].Value == null ? ""
                                                : cls.Axes[0].Positions[0].Members[0].MemberProperties[0].Value.ToString();
                    descrTD2.InnerText = cls.Axes[0].Positions[0].Members[0].MemberProperties[1].Value == null ? ""
                                                : cls.Axes[0].Positions[0].Members[0].MemberProperties[1].Value.ToString();
                    descrTD3.InnerText = cls.Axes[0].Positions[0].Members[0].MemberProperties[2].Value == null ? ""
                                                : cls.Axes[0].Positions[0].Members[0].MemberProperties[2].Value.ToString();
                    string descr4 = cls.Axes[0].Positions[0].Members[0].MemberProperties[3].Value == null ? ""
                        : cls.Axes[0].Positions[0].Members[0].MemberProperties[3].Value.ToString();
                    string descr5 = cls.Axes[0].Positions[0].Members[0].MemberProperties[4].Value == null ? ""
                        : cls.Axes[0].Positions[0].Members[0].MemberProperties[4].Value.ToString();
                    descrTD4.InnerText = descr4 + " " + descr5;
                    
                    // заодно получим и нормативное значение
                    string separator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                    normValue = double.Parse(descr5.Replace(".", separator));
                }
                catch
                {
                }
            }
            
        }

        /// <summary>
        /// смена активной ячейки мастер-таблицы
        /// </summary>
        protected void MasterTable_ActiveCellChange(object sender, CellEventArgs e)
        {
           if (e.Cell == null) return;
                      
           try
           {
               SwitchDetails(!(e.Cell.Column.Index > 0));
               if (e.Cell.Column.Index > 0)
               {
                   SetUpIndicatorInfo(e.Cell.Column.Header.Key);
               }
               else
               {
                   SetUpGauges(e.Cell.Row);
               }                                    
           }
           finally
           {
               AllowChangeRowHandling = false;
           }           
        }

        /// <summary>
        /// Настройка одного индикатора
        /// </summary>
        private void SetUpIndicatorInfo(string indicatorName)
        {
            string tmpl = "[Показатели].[БККУ_Сопоставимый].[Данные всех источников].[{0}]";
            Session["indicators_bkku"] = string.Format(tmpl, indicatorName);

            chartTitleLabel.Text = string.Format("Значение индикатора {0}", indicatorName);
            descrTitleLabel.Text = string.Format("Описание индикатора {0}", indicatorName);
            FetchIndicatorDescr(indicatorName);

            RenderElement(chart, "chart", chartdt);
        }

        /// <summary>
        /// Настройка набора индикаторов
        /// </summary>
        private void SetUpGauges(UltraGridRow row)
        {
            regionDetailLabel.Text = row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString();
            TotalVioletionCountLabel.Text = row.Cells[3].Value == null ? "" : row.Cells[3].Value.ToString();
            GroupMO.Text = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();

            //значения и шкалы гэйджев
            SetGaugeScaleBounds(GaugeBK1, 5, row.Index);
            SetGaugeScaleBounds(GaugeBK2, 7, row.Index);
            SetGaugeScaleBounds(GaugeBK3_obl, 9, row.Index);
            SetGaugeScaleBounds(GaugeBK3_mest, 11, row.Index);
            SetGaugeScaleBounds(GaugeBK4, 13, row.Index);
            SetGaugeScaleBounds(GaugeBK5, 15, row.Index);
            SetGaugeScaleBounds(GaugeBK6_a, 17, row.Index);
            SetGaugeScaleBounds(GaugeBK6_b, 19, row.Index);
            SetGaugeScaleBounds(GaugeBK6_v, 21, row.Index);
            SetGaugeScaleBounds(GaugeBK7, 23, row.Index);
            SetGaugeScaleBounds(GaugeBK8, 25, row.Index);
            SetGaugeScaleBounds(GaugeBK9, 27, row.Index);
            SetGaugeScaleBounds(GaugeBK10, 29, row.Index);
            SetGaugeScaleBounds(GaugeBK11, 31, row.Index);
        }

        /// <summary>
        /// Получение данных мастер таблицы
        /// </summary>
        protected void MasterTable_DataBinding(object sender, EventArgs e)
        {            
            DataProvidersFactory.PrimaryMASDataProvider.WorkDir = Server.MapPath(".");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mastertable"), "Район", masterdt);
            if (masterdt == null)
            {
                MasterTable.Clear();
                return;
            }
            masterdt.TableName = "districts";
            tableDataSet.Tables.Add(masterdt);            
            MasterTable.DataSource = tableDataSet.Tables["districts"].DefaultView;            
        }

        /// <summary>
        /// Формат мастер-таблицы
        /// </summary>
        protected void MasterTable_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
                return;
            try
            {
                for (int i = 1; i <= 32; i++)
                {
                    if (i % 2 == 0)
                    {
                        MasterTable.Columns[i].Hidden = true;
                    }
                    if (i == 1 || i == 9 || i == 11 ||
                        i == 17 || i == 19 || i == 21)
                    {
                        MasterTable.Columns[i].Width = 62;
                    }
                    else
                    {
                        MasterTable.Columns[i].Width = 59;
                    }
                    if (i == 1 || i == 2)
                    {
                        CRHelper.FormatNumberColumn(MasterTable.Columns[i], "N0");
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(MasterTable.Columns[i], "N2");
                    }
                }

                MasterTable.Columns[0].Header.Caption = "Район";
                MasterTable.Columns[0].Header.Key = "Район";

                MasterTable.Columns[1].Header.Caption = "Группа МО";//<br/>Количество нарушений требований БК<br/>=0";
                MasterTable.Columns[1].Header.Key = "Группа МО";

                MasterTable.Columns[3].Header.Caption = "ОКН";//<br/>Количество нарушений условий КУ<br/>=0";
                MasterTable.Columns[3].Header.Key = "ОКН";

                MasterTable.Columns[5].Header.Caption = "БК 1";//<br/>Предельный объем муниципального долга<br/><=1";
                MasterTable.Columns[5].Header.Key = "БК 1";

                MasterTable.Columns[7].Header.Caption = "БК 2";//<br/>Предельный размер дефицита местного бюджета<br/><=0";
                MasterTable.Columns[7].Header.Key = "БК 2";

                MasterTable.Columns[9].Header.Caption = "БК 3_обл";//<br/>Отношение текущих расходов местного бюджета к объему доходов местного бюджета<br/><=1";
                MasterTable.Columns[9].Header.Key = "БК 3_обл";

                MasterTable.Columns[11].Header.Caption = "БК 3_мест";//<br/>Отношение текущих расходов местного бюджета к объему доходов местного бюджета<br/><=1";
                MasterTable.Columns[11].Header.Key = "БК 3_мест";

                MasterTable.Columns[13].Header.Caption = "БК 4";//<br/>Предельный объем расходов на обслуживание муниципального долга<br/><=0";
                MasterTable.Columns[13].Header.Key = "БК 4";

                MasterTable.Columns[15].Header.Caption = "БК 5";//<br/>Показатель условий оплаты труда муниципальных служащих, финансируемых за счет средств местного бюджета<br/><=1";
                MasterTable.Columns[15].Header.Key = "БК 5";

                MasterTable.Columns[17].Header.Caption = "БК 6_а";//<br/>>=1";
                MasterTable.Columns[17].Header.Key = "БК 6_а";

                MasterTable.Columns[19].Header.Caption = "БК 6_б";//<br/>Кредиторская задолженность по выплате заработной платы<br/><=1";
                MasterTable.Columns[19].Header.Key = "БК 6_б";

                MasterTable.Columns[21].Header.Caption = "БК 6_в";//<br/>Кредиторская задолженность по начислениям на оплату труда<br/><=1";
                MasterTable.Columns[21].Header.Key = "БК 6_в";

                MasterTable.Columns[23].Header.Caption = "БК 7";//<br/>Кредиторская задолженность по оплате коммунальных услуг бюджетными учреждениями<br/><=1";
                MasterTable.Columns[23].Header.Key = "БК 7";

                MasterTable.Columns[25].Header.Caption = "БК 8";//<br/>Установленный уровень оплаты населением жилищно-коммунальных услуг<br/>>=1";
                MasterTable.Columns[25].Header.Key = "БК 8";

                MasterTable.Columns[27].Header.Caption = "БК 9";//<br/>Фактический уровень оплаты населением жилищно-коммунальных услуг<br/>>=1";
                MasterTable.Columns[27].Header.Key = "БК 9";

                MasterTable.Columns[29].Header.Caption = "БК 10";//<br/>Максимально допустимая доля собственных расходов граждан на оплату жилья и коммунальных услуг в совокупном доходе семьи<br/>>=0";
                MasterTable.Columns[29].Header.Key = "БК 10";

                MasterTable.Columns[31].Header.Caption = "БК 11";//<br/>Отношение тарифа на электроэнергию для промышленности к тарифу для населения<br/><=1";
                MasterTable.Columns[31].Header.Key = "БК 11";
            }
            catch
            {
            }
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        protected void SubmitButton_Click(object sender, ButtonEventArgs e)
        {
            RowDetailPanel.Visible = false;
            ColumnDetailPanel.Visible = false;

        
            string periodUN = string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);
            
            switch (ComboQuarter.SelectedIndex)
	        {
	            case 0:
                case 1:
	                periodUN += string.Format(".[Полугодие 1].[Квартал {0}]", ComboQuarter.SelectedIndex + 1);
	                break;
	                
	            case 2:
	            case 3:
	                periodUN += string.Format(".[Полугодие 2].[Квартал {0}]", ComboQuarter.SelectedIndex + 1);
	                break;	                		
	        }
           
            UserParams.PeriodYQM_Quarter.Value = periodUN;
            MasterTable.DataBind();
        }

        /// <summary>
        /// Обновление детали
        /// </summary>
        protected void MasterTable_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (AllowChangeRowHandling)
                {
                    SetUpGauges(e.Row);
                    SwitchDetails(true);
                }
            }
            finally
            {
                AllowChangeRowHandling = true;
            }                           
        }

        // выводим нормативное значение
        protected void chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            // для нулевого не выводим
            if (normValue == 0)
            {
                return;
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];        

            int textWidht = 200;
            int textHeight = 10;
            int lineLength = 250;

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Нормативное значение: {0}", normValue));
            e.SceneGraph.Add(text);

            Line line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normValue));
            e.SceneGraph.Add(line);
        }

        // красим ячейки
        protected void MasterTable_InitializeRow(object sender, RowEventArgs e)
        {
            switch (Convert.ToInt32(e.Row.Cells[1].Value))
            {
                case 1:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.LightGreen;
                        break;
                    }
                case 2:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.LightYellow;
                        break;
                    }
                case 3:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.Pink;
                        break;
                    }
            }

            // красим красным, если кол-во нарушений == 1
            for (int i = 1; i <= 31; i = i + 2)
            {
                if (Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                {
                    e.Row.Cells[i].Style.BackgroundImage = "~/images/BallRed.gif";
                }
            }
        }

        // меняем вывод ошибки, когда нет данных
        protected void chart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.Font = new Font("Microsoft Sans Serif, 7.8pt", 10);
            e.Text = "Нет данных";
            e.LabelStyle.VerticalAlign = StringAlignment.Near;
        }


    }
}