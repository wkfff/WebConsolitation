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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.UFK_0014_0002
{
    public partial class Default : CustomReportPage
    {
        #region константы
        private const string periodAllMemberUN = "[Период].[День_ФО].[Данные всех периодов]";

        //Шаблон описания организации
        const string OgrEgrulDescrTemplate = "{0}<br/> ОГРН: {1}, ИНН: {2}, КПП: {3}, ОКАТО: {4}, ОКВЭД: {5}<br/>"
            + "{6}: {7},<br/>Регистрационный номер создания: {8}, дата постановки на учет в налоговом органе: "
            + "{9}, дата создания  ЮЛ: {10},<br/> Контактная информация: {11},<br/> Управляющая компания: {12},<br/>"
            + "ОГРН: {13}, ИНН: {14}, КПП: {15}, ОКАТО: {16},<br/> Адрес: {17},<br/> "
            + "Контактная информация: {18}";

        //Запасной шаблон    
        const string IpEgripDescrTemplate = "ФИО {0}<br/> ОГРНИП: {1}, ИНН: {2}, ОКВЭД: {3}, ОКАТО: {4},<br/>Документ, удостоверяющий личность: {5} "
            + "{6} выдан {7} {8}<br/>Контактная информация: {9}<br/>Адрес: {10}<br/>Дата постановки на учет в налоговом органе: {11}";
                
        #endregion
        
        #region Наборы данных страницы
        //для мастре-таблицы
        private DataTable masterdt = new DataTable();
        //для детали-диаграммы
        private DataTable chartdt = new DataTable();
        //для детали-таблицы
        private DataTable detailtabledt = new DataTable();

        /*
        private DataSet detailTableDataSet = new DataSet("TableDataSet");
        private DataTable detailTableGroupDT = new DataTable();
        private DataTable detailTableSubGroupDT = new DataTable();
        private DataTable detailTableArticleDT = new DataTable();
        private DataTable detailTableSubArticleDT = new DataTable();        
         */ 
        #endregion


        protected void Page_Init(object sender, EventArgs e)
       {
       }
       

        /// <summary>
        /// Подготовка к загрузке страницы
        /// </summary>
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            try
            {
				//Инициализируем размеры элементов
                int WidthDirty = ((int)Session["width_size"] - 68);
                int HalfWidthtDirty = (int)Math.Ceiling((double)(WidthDirty / 2));
                MasterTable.Width = HalfWidthtDirty;
                DetailTable.Width = HalfWidthtDirty;
                DetailChart.Width = HalfWidthtDirty;
                PayerInfoContainer.Width = HalfWidthtDirty.ToString();

                panelParameters.Width = WidthDirty;
                int treesWidth = (int)Math.Ceiling((double)(WidthDirty / 4));

                //Устанавливаем размеры параметров-деревьев. 
                //ОКВЭД делаем больше нормы, остальные чуть поменьше
                dtOKVD.Width = treesWidth + 150;
                dtRegions.Width = treesWidth - 50;
                dtBudgetLevel.Width = treesWidth - 50;                
                
                //в этом уже нет необходимости
                /*
                dtRegionsContainer.Style.Clear();
                dtRegionsContainer.Style.Add("width", treesWidth.ToString() + "px");                

                dtBudgetLevelContainer.Style.Clear();
                dtBudgetLevelContainer.Style.Add("width", treesWidth.ToString() + "px");

                dtOKVDContainer.Style.Clear();
                dtOKVDContainer.Style.Add("width", treesWidth.ToString() + "px");
                */ 
                                                
                int HeightDirty = ((int)Session["height_size"] - 240);
                MasterTable.Height = HeightDirty;
                DetailTable.Height = HeightDirty - 10;
                DetailChart.Height = HeightDirty;
                PayerInfoContainer.Height = (HeightDirty + 10).ToString();
                
                

            }
            catch
            {
                Page.Response.Redirect("../../Default.aspx");
            }

        }

        /// <summary>
        /// Начало загрузки страницы
        /// </summary>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            SubmitButton.Appearance.Style.BorderColor = System.Drawing.Color.Black;
            
            //Если грузимся впервые, установим значение параметров- по умолчанию
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


                if (UserParams.Region.Value != string.Empty)
                {
                    dtRegions.SetChecked(UserParams.Region.Value);
                }
                                
            }
            
            //Всегда скрываем панель параметров
            panelParameters.Expanded = false;
            //по умолчанию, прячем детали
            detailTabs.Visible = false;
            DetailTable.Visible = false;
            DetailChart.Visible = false;                
            
            
            //обновляем параметр и грузим таблицу
            UserParams.PeriodDayFO.Value = CRHelper.PeriodMemberUName(periodAllMemberUN, (DateTime)date.Value, detailmode.SelectedIndex + 1);            
            MasterTable.DataBind();
            
            TryToPaintSummary();
            
        }

        /// <summary>
        /// Информация по палтельщику
        /// </summary>
        private bool PayerDescrByTemplate(string PayerINN, string PayerKPP, bool FindInEgrul)
        {
            string qTemplate;
            string SQLText;
            if (FindInEgrul)
            {
                qTemplate = DataProvider.GetQueryText("OgrEgrulSQLTemplate");
                SQLText = string.Format(qTemplate, PayerINN, PayerKPP); ;
            }
            else
            {
                qTemplate = DataProvider.GetQueryText("IpEgripSQLTemplate");
                SQLText = string.Format(qTemplate, PayerINN); ;

            }
            DataTable payersDT = DataProvidersFactory.PrimaryMASDataProvider.GetWarehouseDataTable(SQLText);

            try
            {
                object[] vals;
                if (FindInEgrul)
                {
                    vals = payersDT.Rows[0].ItemArray;
                    chartdata.InnerHtml = string.Format(OgrEgrulDescrTemplate,
                        vals[0].ToString(), vals[1].ToString(), vals[2].ToString(), vals[3].ToString(),
                        vals[4].ToString(), vals[5].ToString(), vals[6].ToString(), vals[7].ToString(),
                        vals[8].ToString(), vals[9].ToString(), vals[10].ToString(), vals[11].ToString(),
                        vals[12].ToString(), vals[13].ToString(), vals[14].ToString(), vals[15].ToString(),
                        vals[16].ToString(), vals[17].ToString(), vals[18].ToString());

                }
                else
                {
                    vals = payersDT.Rows[0].ItemArray;

                    chartdata.InnerHtml = string.Format(IpEgripDescrTemplate,
                        vals[0].ToString(), vals[1].ToString(), vals[2].ToString(), vals[3].ToString(),
                        vals[4].ToString(), vals[5].ToString(), vals[6].ToString(), vals[7].ToString(),
                        vals[8].ToString(), vals[9].ToString(), vals[10].ToString(), vals[11].ToString());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        
        /// <summary>
        /// Получение данных мастер-таблицы
        /// </summary>
        protected void MasterTable_DataBinding(object sender, EventArgs e)
        {
        
            DataProvidersFactory.PrimaryMASDataProvider.WorkDir = Server.MapPath(".");
            #warning !!!!!! путаница с вариантами представлений тэйбла
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mastertable"), "Организация", masterdt);
            if (masterdt == null)
            {
                MasterTable.Clear();
                return;
            }
            masterdt.TableName = "organization";
            MasterTable.DataSource = masterdt;            
        }

        /// <summary>
        /// Раскрашиваем итоги
        /// </summary>
        private void TryToPaintSummary(Infragistics.WebUI.UltraWebGrid.UltraGridCell cll)
        {
            System.Drawing.Color sumColor = System.Drawing.Color.LightSteelBlue;
            if (cll.Value.ToString() == "Все")
            {
                cll.Row.Style.BackColor = sumColor;
                cll.Row.Style.Font.Bold = true;
            }
        }


        /// <summary>
        /// Раскрашиваем итоги
        /// </summary>
        private void TryToPaintSummary()
        {
            Infragistics.WebUI.UltraWebGrid.UltraGridCell cll;
            try
            {
                //смотрим на первую 
                cll = MasterTable.Rows[0].Cells[0];
                TryToPaintSummary(cll);
                
                //и последнюю
                cll = MasterTable.Rows[MasterTable.Rows.Count - 1].Cells[0];
                TryToPaintSummary(cll);
            }
            catch 
            {
            }
        }

        /// <summary>
        /// Подстройка формата мастер-таблицы
        /// </summary>
        protected void MasterTable_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                MasterTable.Columns[0].Width = 300;
                MasterTable.Columns[1].Width = 100;
                CRHelper.FormatNumberColumn(MasterTable.Columns[1], "N2");                                                              
            }
            catch
            {
                //Не будем в этом случае загромождать лог информацией об исключении, просто обозначаем
                CRHelper.SaveToErrorLog(CustomReportConst.errTableFormating);            
            }            
        }

        /// <summary>
        /// Рефреш детали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MasterTable_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            //выставляем параметр - организации           
            string pValue = "[Организации].[Сопоставимый].[Все]";
            if (e.Row.Cells[0].Value.ToString() != "Все")
            {
                pValue += string.Format(".[{0}]", e.Row.Cells[0].Value.ToString());
            }            
            UserParams.Organization.Value = pValue;
            
            detailTabs.Visible = true;
            DetailChart.Visible = true;
            DetailTable.Visible = true;            
            DetailTable.DataBind();
            DetailChart.DataBind();

            //Дастаем информацию по плательщику
            chartdata.InnerText = e.Row.Cells[0].Value.ToString();
            string PayerINN = e.Row.Cells[2].Value.ToString().Trim();
            string PayerKPP = e.Row.Cells[5].Value.ToString().Trim();
            if ((PayerINN != string.Empty) && (PayerINN != "0"))
            {            
                if (!PayerDescrByTemplate(PayerINN, PayerKPP, true)) PayerDescrByTemplate(PayerINN, "", false);            
            }
            
        }

        /// <summary>
        /// Обновление табличной детали
        /// </summary>
        protected void DetailTable_DataBinding(object sender, EventArgs e)
        {

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("detailtable"), "Статья", detailtabledt);
            DetailTable.DataSource = detailtabledt;        
            //detailtabledt.Columns[1]
        }

        /// <summary>
        /// Обновление детали диаграммы
        /// </summary>
        protected void DetailChart_DataBinding(object sender, EventArgs e)
        {
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("detailchart"), "series", chartdt);
            DetailChart.DataSource = chartdt;

        }
        
        /// <summary>
        /// Подстройка формата таблицы-детали
        /// </summary>
        protected void DetailTable_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                if (DetailTable.Columns[1].Header.Caption == "Код")
                {
                    DetailTable.Columns[1].Move(0);
                }

                CRHelper.FormatNumberColumn(DetailTable.Columns[2], "N2");
                DetailTable.Columns[1].Width = 200;
                DetailTable.Columns[1].CellStyle.Wrap = true;
            }
            catch
            {
            }
        }

        protected void detailTabs_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
        {

        }

    }
}
