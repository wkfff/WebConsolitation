using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.ORG_0004_0001
{
	public partial class Default : CustomReportPage
	{

		private const int rowsPerPage = 1000;
		private int currentPage;
		private int pagesCount;
		private int rowsCount;

		private GridHeaderLayout headerLayout;
		private Collection<string> comboDebtsDictionary ;
		private Dictionary<string, int> comboRegionsDictionary;
		private Dictionary<string, string> filterDictionary;

		IDatabase database;

		public Default()
		{
			comboDebtsDictionary = 
				new Collection<string>
					{
						"Задолженность перед ФНС",
						"Задолженность по взносам в ПФ и ФОМС"
					};

			comboRegionsDictionary =
				new Dictionary<string, int>
					{
						{"Крупнейшие налогоплательщики НСО", 0},
						{"Баганский район", 0},
						{"Барабинский район", 0},
						{"Болотнинский район", 0},
						{"Венгеровский район", 0},
						{"Доволенский район", 0},
						{"Здвинский район", 0},
						{"Искитимский район", 0},
						{"Карасукский район", 0},
						{"Каргатский район", 0},
						{"Колыванский район", 0},
						{"Коченевский район", 0},
						{"Кочковский район", 0},
						{"Краснозерский район", 0},
						{"Куйбышевский район", 0},
						{"Купинский район", 0},
						{"Кыштовский район", 0},
						{"Маслянинский район", 0},
						{"Мошковский район", 0},
						{"Новосибирский район", 0},
						{"Ордынский район", 0},
						{"Северный район", 0},
						{"Сузунский район", 0},
						{"Татарский район", 0},
						{"Тогучинский район", 0},
						{"Убинский район", 0},
						{"Усть-Таркский район", 0},
						{"Чановский район", 0},
						{"Черепановский район", 0},
						{"Чистоозерный район", 0},
						{"Чулымский район", 0},
						{"г. Бердск", 0},
						{"г. Новосибирск", 0},
						{"Дзержинский район г. Новосибирска", 0},
						{"Заельцовский район г. Новосибирска", 0},
						{"Кировский район г. Новосибирска", 0},
						{"Ленинский район г. Новосибирска", 0},
						{"Октябрьский район г. Новосибирска", 0},
						{"Центральный район г. Новосибирска", 0},
						{"Железнодорожный район г. Новосибирска", 0},
						{"Калининский район г. Новосибирска", 0},
						{"Советский район г. Новосибирска", 0},
						{"Первомайский район г. Новосибирска", 0}
					};

			filterDictionary =
				new Dictionary<string, string>
					{
						{"Крупнейшие налогоплательщики НСО", "5460"},
						{"Баганский район", "5417,5474"},
						{"Барабинский район", "5451,5485"},
						{"Болотнинский район", "5413,5475"},
						{"Венгеровский район", "5419,5487"},
						{"Доволенский район", "5420,5456"},
						{"Здвинский район", "5421,5485"},
						{"Искитимский район", "5446,5483"},
						{"Карасукский район", "5422,5474"},
						{"Каргатский район", "5423,5464"},
						{"Колыванский район", "5424,5475"},
						{"Коченевский район", "5425,5464"},
						{"Кочковский район", "5426,5456"},
						{"Краснозерский район", "5427,5456"},
						{"Куйбышевский район", "5452,5485"},
						{"Купинский район", "5429,5474"},
						{"Кыштовский район", "5430,5487"},
						{"Маслянинский район", "5431,5483"},
						{"Мошковский район", "5432,5475"},
						{"Новосибирский район", "5406,5475"},
						{"Ордынский район", "5434,5456"},
						{"Северный район", "5435,5485"},
						{"Сузунский район", "5436,5483"},
						{"Татарский район", "5453,5487"},
						{"Тогучинский район", "5438,5475"},
						{"Убинский район", "5439,5464"},
						{"Усть-Таркский район", "5416,5487"},
						{"Чановский район", "5415,5487"},
						{"Черепановский район", "5440,5483"},
						{"Чистоозерный район", "5441,5487"},
						{"Чулымский район", "5442,5464"},
						{"г. Бердск", "5445"},
						{"г. Новосибирск", "5411"},
						{"Дзержинский район г. Новосибирска", "5401"},
						{"Заельцовский район г. Новосибирска", "5402"},
						{"Кировский район г. Новосибирска", "5403"},
						{"Ленинский район г. Новосибирска", "5404"},
						{"Октябрьский район г. Новосибирска", "5405"},
						{"Центральный район г. Новосибирска", "5406"},
						{"Железнодорожный район г. Новосибирска", "5407"},
						{"Калининский район г. Новосибирска", "5410"},
						{"Советский район г. Новосибирска", "5408,5473"},
						{"Первомайский район г. Новосибирска", "5409,5473"}
					};

		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

			UltraWebGrid1.Height = CustomReportConst.minScreenHeight - 300;
			UltraWebGrid1.Width = CustomReportConst.minScreenWidth;
			UltraWebGrid1.Grid.DisplayLayout.NoDataMessage = "Организация не найдена";
			UltraWebGrid1.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
			
			CrossLink.Text = " Прирост&nbsp;недоимки&nbsp;по&nbsp;видам&nbsp;деятельности";
			CrossLink1.Text = " Прирост&nbsp;недоимки&nbsp;по&nbsp;МР&nbsp;и&nbsp;ГО";

			CrossLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultOKVD.aspx";
			CrossLink1.NavigateUrl = "~/reports/FNS_0001_0001/DefaultRegions.aspx";

			Page.Title = "Организации-должники";
			PageTitle.Text = Page.Title;

			Label1.Text = "ИНН";
			Label2.Text = "КПП";
			Label3.Text = "Наименование";

			innText.Width = 230;
			kppText.Width = 200;
			nameText.Width = 625;

			innText.MaxLength = 12;
			kppText.MaxLength = 9;

			linkFirst.Text = "Первая";
			linkFirst.InitLink();
			linkPrev.Text = "Предыдущая";
			linkPrev.InitLink();
			linkNext.Text = "Следующая";
			linkNext.InitLink();
			linkLast.Text = "Последняя";
			linkLast.InitLink();
			
			
		}
		
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (!Page.IsPostBack)
			{
				ComboRegions.Title = "По ИФНС";
				ComboRegions.Width = 300;
				ComboRegions.MultiSelect = true;
				ComboRegions.ParentSelect = true;
				ComboRegions.FillDictionaryValues(comboRegionsDictionary);

				ComboDebts.Title = "Характер задолженности";
				ComboDebts.Width = 500;
				ComboDebts.MultiSelect = false;
				ComboDebts.ParentSelect = false;
				ComboDebts.FillValues(comboDebtsDictionary);

			}
			
			// получим номер текущей страницы
			
			currentPage = 0;
			if (Page.IsPostBack)
			{
				string target = Request.Form.Get("__EVENTTARGET").Replace("$", "_");

				// кнопки навигации
				if (linkFirst.ClientID.Equals(target)
					|| linkPrev.ClientID.Equals(target)
					|| linkNext.ClientID.Equals(target)
					|| linkLast.ClientID.Equals(target))
				{
					if (!Int32.TryParse(Request.Form.Get("__EVENTARGUMENT"), out currentPage))
					{
						currentPage = 0;
					}	
					PageNumber.Value = currentPage.ToString();
				}

				// кнопки экспорта
				else if (ReportExcelExporter1.ExcelExportButton.ClientID.Equals(target)
					|| ReportPDFExporter1.PdfExportButton.ClientID.Equals(target))
				{
					if (!Int32.TryParse(PageNumber.Value, out currentPage))
					{
						currentPage = 0;
					}
				}
				// иное
				else
				{
					currentPage = 0;
					PageNumber.Value = currentPage.ToString();
				}

			}

			// сбиндим данные

			database = GetDataBase();
			UltraWebGrid1.Grid.DataSource = GetTable();
			UltraWebGrid1.Grid.Bands.Clear();
			UltraWebGrid1.DataBind();

			// сформируем зависимые от данных элементы страницы

			pagesCount = Convert.ToInt32(Math.Floor((double) (rowsCount - 1)/rowsPerPage))+1;

			// тектовая инфа
			Label4.Text = String.Format(
				"Всего найдено {0:N0} организаций, страница {1} из {2}",
				rowsCount,
				currentPage + 1,
				pagesCount
				);
			
			// первая
			if (currentPage > 0)
			{
				linkFirst.Visible = true;
				linkFirst.NavigateUrl = String.Format("javascript:__doPostBack('{0}','{1}')", linkFirst.ClientID, 0);
			}
			else
			{
				linkFirst.Visible = false;
			}
				
			// предыдущая
			if (currentPage > 0)
			{
				linkPrev.Visible = true;
				linkPrev.NavigateUrl = String.Format("javascript:__doPostBack('{0}','{1}')", linkPrev.ClientID, currentPage - 1);
			}
			else
			{
				linkPrev.Visible = false;
			}

			// следующая
			if ((currentPage+1)*rowsPerPage < rowsCount )
			{
				linkNext.Visible = true;
				linkNext.NavigateUrl = String.Format("javascript:__doPostBack('{0}','{1}')", linkNext.ClientID, currentPage + 1);
			}
			else
			{
				linkNext.Visible = false;
			}

			// последняя
			if ((currentPage + 1) * rowsPerPage < rowsCount)
			{
				linkLast.Visible = true;
				linkLast.NavigateUrl = String.Format("javascript:__doPostBack('{0}','{1}')", linkLast.ClientID, pagesCount-1);
			}
			else
			{
				linkLast.Visible = false;
			}

			
			

		}

		/// <summary>
		/// настроим фильтр
		/// </summary>
		private string GetFilter()
		{
			List<string> where = new List<string>();

			if (!innText.Text.Equals(String.Empty))
			{
				where.Add(String.Format("inn like '%{0}%'", innText.Text.Replace(@"'", @"''")));
			}
			if (!kppText.Text.Equals(String.Empty))
			{
				where.Add(String.Format("kpp like '%{0}%'", kppText.Text.Replace(@"'", @"''")));
			}
			if (!nameText.Text.Equals(String.Empty))
			{
				where.Add(String.Format("name like '%{0}%'", nameText.Text.Replace(@"'", @"''")));
			}

			if (ComboRegions.SelectedValues.Count > 0)
			{
				List<string> ifnsWhere = new List<string>();
				foreach (string value in ComboRegions.SelectedValues)
				{
					string[] ifnsCodes = filterDictionary[value].Split(',');
					foreach (string str in ifnsCodes)
					{
						ifnsWhere.Add(String.Format("kpp like '{0}%'", str));
					}
				}
				where.Add(String.Format("({0})", String.Join(" or ", ifnsWhere.ToArray())));
			}

			switch (ComboDebts.SelectedIndex)
			{
				case 0:
					where.Add("DataKind = 'ФНС'");
					break;
				case 1:
					where.Add("DataKind = 'ПФ и ФОМС'");
					break;
			}

			return
				where.Count == 0
				? String.Empty
				: String.Format("WHERE {0}", String.Join(" and ", where.ToArray()));
		}

		private DataTable GetTable()
		{
			string filter = GetFilter();
			// улала, MSSQL нумерует записи с 1
			int startRow = currentPage*rowsPerPage + 1;

			string queryText =
				String.Format(
				"WITH OrderedOrders AS" +
				"(" +
					"SELECT INN AS inn, KPP AS kpp, Name AS name," +
					"ROW_NUMBER() OVER (ORDER BY name ASC) AS 'RowNumber'" +
					"FROM d_Org_FNSDebtor " +
					"{0}" +
				") " +
				"SELECT inn, kpp, name " +
				"FROM OrderedOrders " +
				"WHERE RowNumber BETWEEN {1} AND {2};",
				filter,
				startRow,
				startRow + rowsPerPage - 1);

			string queryCountText = String.Format("select count (*) from d_Org_FNSDebtor {0}", filter);
			string queryDateText = "select b.pumpdate from PUMPHISTORY b where b.id in (select a.pumpid from d_org_fnsdebtor a)";
			try
			{
				DataTable dtDate = (DataTable)database.ExecQuery(queryDateText, QueryResultTypes.DataTable);
				DateTime date = (DateTime)(dtDate.Rows[0][0]);
				PageSubTitle.Text = String.Format("Информация приводится по состоянию на {0:01.MM.yyyy} года", date);

				DataTable dt = (DataTable)database.ExecQuery(queryText, QueryResultTypes.DataTable);
				rowsCount = (int)(database.ExecQuery(queryCountText, QueryResultTypes.Scalar));
				if (dt.Rows.Count > 0)
				{
					return dt;
				}
			}
			finally
			{
				database.Dispose();
			}
			return null;
		}

		private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
		{
			e.Layout.GroupByBox.Hidden = true;
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
			e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
			e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(730);

			e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
			e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;

			e.Layout.Bands[0].Columns[0].Format = "#000000000";
			e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.Bands[0].Columns[1].Format = "#00000000";
			e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

			headerLayout = new GridHeaderLayout(UltraWebGrid1.Grid);
			headerLayout.AddCell("ИНН", "Индивидуальный номер налогоплательщика");
			headerLayout.AddCell("КПП", "Код причины постановки на учет");
			headerLayout.AddCell("Наименование организации", "Наименование организации");
			headerLayout.ApplyHeaderInfo();
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
		
		#region экспорт

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

			ReportExcelExporter1.HeaderCellHeight = 30;
			ReportExcelExporter1.GridColumnWidthScale = 1.1;
			ReportExcelExporter1.Export(headerLayout, 3);
		}

		private void PdfExportButton_Click(Object sender, EventArgs e)
		{
			ReportPDFExporter1.PageTitle = PageTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();

			ReportPDFExporter1.HeaderCellHeight = 50;
			ReportPDFExporter1.Export(headerLayout, section1);


		}

		#endregion

	}

	public static class Extensions
	{

		public static void InitLink(this HyperLink link)
		{
			link.Style.Add("font-family", "Verdana");
			link.Style.Add("font-size", "12px");
		}
	}
}
