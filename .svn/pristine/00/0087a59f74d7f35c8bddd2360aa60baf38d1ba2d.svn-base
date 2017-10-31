using System;
using System.Data;
using System.Drawing;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.UFK_0014_0001
{
    public partial class _Default : CustomReportPage
    {        
        #region Константы страницы
        //UN всех районов
        const string regionAllMemberUN = "[Районы].[Сопоставимый].[Все районы]";        
        //имя таблицы районов
        const string districtsTableName = "districts";
        //имя таблицы поселений        
        const string settlementsTableName = "settlements";

        #endregion
    
        #region Наборы данных страницы
        //иерархический датасет для мастер-таблицы
        private DataSet tableDataSet = new DataSet("TableDataSet");
        //Первый уровень иерархии мастер-таблицы: Районы
        private DataTable masterdt = new DataTable();
        //Втопрой уровеньиерархии мастер-таблицы: Поселения
        private DataTable detaldt = new DataTable();
        //Набор данных для диаграммы
        private DataTable chartdt = new DataTable();
        #endregion
                        
        /// <summary>
        /// Видимость диаграммы
        /// </summary>
        private void ShowHideChart(bool IsShow)
        {
            NoChartData.Visible = ! IsShow;
            chart.Visible = IsShow;        
        }
        
        /// <summary>
        /// Подготовка загрузки страницы
        /// </summary>
        /// <param name="sender"></param>
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
             
            try
            {
				//Инициализируем размеры элементов
				double halfWidth = ((int)Session["width_size"]- 68) / 2;
                MainTable.Width = (int)Math.Ceiling(halfWidth);
                chart.Width = (int)Math.Ceiling(halfWidth);

                double heightDirty = ((int)Session["height_size"] - 300);
                MainTable.Height = (int)Math.Ceiling(heightDirty);
                chart.Height = (int)Math.Ceiling(heightDirty);
                
                chartLabel.Width = chart.Width;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Начало загрузки страницы
        /// </summary>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            ShowHideChart(false);
            SubmitButton.Appearance.Style.BorderColor = Color.Black;

            //Если грузимся впервые, установим значение параметра- по умолчанию
            if (!Page.IsPostBack)
            {
                DateTime lastDay;
                int periodDetailMode;

                #warning нерешенная проблема с 32 янв.  Пока инициализируем период не из базы, а константой
                if (UserParams.PeriodDayFO.Value != string.Empty)
                {
                    lastDay = CRHelper.PeriodDayFoDate(UserParams.PeriodDayFO.Value);
                    periodDetailMode = CRHelper.PeriodDayFoDetailLevel(UserParams.PeriodDayFO.Value);
                }
                else
                {
                    lastDay = new DateTime(2007, 12, 4);
                    periodDetailMode = 4;
                }
                date.Value = lastDay;
                detailmode.SelectedIndex = periodDetailMode;

            }

            string pValue = CRHelper.PeriodMemberUName("[Период].[День_ФО].[Данные всех периодов]", (DateTime)date.Value, detailmode.SelectedIndex + 1);

            //Обновляем таблицу только, если либо параметр периода был изменен, либо загружаемся в первый раз

            if (!Page.IsPostBack || !UserParams.PeriodDayFO.ValueIs(pValue))
            {
                UserParams.PeriodDayFO.Value = pValue;
                MainTable.DataBind();
            }

            


        }

        /// <summary>
        /// Выборка данных мастер таблицы
        /// </summary>
        protected void MainTable_DataBinding(object sender, EventArgs e)
        {                    
            DataProvidersFactory.PrimaryMASDataProvider.WorkDir = Server.MapPath(".");
                        
            //Берем первый уровень иерархии в таблице
            #warning !!!!!! путаница с вариантами представлений тэйбла
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mastertable"), "Район", masterdt);
            if (masterdt == null)
            {
                MainTable.Clear();
                return;
            }
            masterdt.TableName = districtsTableName;
            
            //Берем второй уровень
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(DataProvider.GetQueryText("detailtable"), detaldt);
            if (detaldt == null) return;
            detaldt.TableName = settlementsTableName;
            

            //Добавляем в датасет и пытаемся установить связь
            tableDataSet.Tables.Add(masterdt);
            tableDataSet.Tables.Add(detaldt);
            try
            {
                tableDataSet.Relations.Add("RP", tableDataSet.Tables[districtsTableName].Columns[0],
                    tableDataSet.Tables[settlementsTableName].Columns[0]);
            }
            catch
            {
            }

            MainTable.DataSource = tableDataSet.Tables[districtsTableName].DefaultView;

            //настраиваем подпись
            string pdescr = CRHelper.PeriodDescr((DateTime)date.Value, detailmode.SelectedIndex + 1);
            gridLabel.Text = string.Format("Поступления на {0} в тысячах рублей", pdescr);                        
        }
        
        /// <summary>
        /// Подстройка формата представления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MainTable_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                //Форматирование чиловых колонок районов
                for (int i = 1; i < 9; i++)
                {
                    CRHelper.FormatNumberColumn(MainTable.Bands[0].Columns[i], "N2");
                }

                //Форматирование чиловых колонок поселений
                for (int i = 2; i < 10; i++)
                {
                    CRHelper.FormatNumberColumn(MainTable.Bands[1].Columns[i], "N2");
                }                           
                MainTable.Bands[1].Columns[0].Hidden = true;
                
                //Детальная таблица (поселения) была получена через рекордсет,
                //поэтому ее заголовки нуждаются в настройке.
                MainTable.Bands[1].Columns[1].Header.Caption = "Поселение";                
                MainTable.Bands[1].Columns[2].Header.Caption = "Все уровни";
                MainTable.Bands[1].Columns[3].Header.Caption = "Конс.бюджет субъекта";
                MainTable.Bands[1].Columns[4].Header.Caption = "Бюджет субъекта";
                MainTable.Bands[1].Columns[5].Header.Caption = "Конс.бюджет МО";
                MainTable.Bands[1].Columns[6].Header.Caption = "Бюджет ГО";
                MainTable.Bands[1].Columns[7].Header.Caption = "Конс.бюджет МР";
                MainTable.Bands[1].Columns[8].Header.Caption = "Бюджет поселения";
                MainTable.Bands[1].Columns[9].Header.Caption = "Бюджет района";
            }
            catch
            {
                //Не будем в этом случае загромождать лог информацией об исключении, просто обозначаем
                CRHelper.SaveToErrorLog(CustomReportConst.errTableFormating);
            }
        }
                
        /// <summary>
        /// Изменение активной строки мастер-таблицы. 
        /// </summary>
        protected void MainTable_ActiveRowChange(object sender, RowEventArgs e)
        {                        
            ShowHideChart(true);            
            string newRegion = GetNewRegionUN(e.Row);
            chartLabel.Text = GetNewRegionLabel(e.Row);
            UserParams.Region.Value = newRegion;
            chart.Data.UseRowLabelsColumn = true;
            RenderElement(chart, "chart", chartdt);

        }

        /// <summary>
        /// Название текущего района/поселения
        /// </summary>
        private string GetNewRegionLabel(UltraGridRow row)
        {
            string resTemplate = "Структура по плательщикам по {0} \"{1}\"";
            if (RowBelongRegions(row))
            {
                return string.Format(resTemplate, "поселению", row.Cells[1].Text);
            }
            else
            {
                return string.Format(resTemplate, "району", row.Cells[0].Text);
            }
        
        }
        
        /// <summary>
        /// UN региона по строке таблицы
        /// </summary>
        private string GetNewRegionUN(UltraGridRow row)
        {
            if (RowBelongRegions(row))
            {
                return string.Format("{0}.[{1}].[{2}]", regionAllMemberUN, row.Cells[0].Text, row.Cells[1].Text);
            }
            else
            {
                string memDescr = row.Cells[0].Text;
                if (memDescr == "Все районы") return regionAllMemberUN;
                return string.Format("{0}.[{1}]", regionAllMemberUN, memDescr);
            }
        }

        /// <summary>
        /// Строка таблицы принадлежит первому уровню иерархии (районы)
        /// </summary>
        private static bool RowBelongRegions(UltraGridRow row)
        {
            return row.Band.BaseTableName == settlementsTableName;
        }


    }
}
