using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Название:	График размещения государственных и муниципальных заказов
	/// Описание:	Мониторинг размещения заказов по данным общероссийского сайта для размещения информации о размещении заказов
	/// Тип:		Оригинальный
	/// БД:			Реляционная: fmserv\mssql2005
	/// </summary>
    public partial class OOS_0001_0002 : CustomReportPage
    {
    	public readonly string TEMPORARY_URL_PREFIX = "../../..";
		public readonly string REPORT_ID = "OOS_0001_0002";

		private const int CALENDAR_MONTH_DISPERSION = 3;

		// параметры запросов
		private CustomParam customParamDate;
		private CustomParam customParamTerritory;

		private int paramTerritoryId;
		private string paramTerritory;
		private DateTime paramDate;

		private Dictionary<int, int> Cube2Relational_CastTerra =
			new Dictionary<int, int>
				{
					{82, 59514}
				};

		private Dictionary<int, string[]> TypePurch =
			new Dictionary<int, string[]>
 						{
 							{0, new []{"", ""}}, //Значение не указано
							{1, new []{"конкурс", "ShadowContest"}}, //Открытый конкурс
							{2, new []{"аукцион", "ShadowTender"}}, //Открытый аукцион
							{3, new []{"аукцион", "ShadowTender"}}, //Открытый аукцион в электронной форме
							{4, new []{"котировка", "ShadowQuote"}}, //Запрос котировок
							{5, new []{"отбор", "ShadowChoice"}}, //Предварительный отбор и запрос котировок при чрезвычайных ситуациях
							{6, new []{"", ""}}, //Единственный поставщик
							{7, new []{"", ""}}, //Размещение заказа на товарных биржах
							{8, new []{"интерес", "ShadowInterest"}}, //Сообщение о заинтересованности в проведении конкурса
 						};

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			customParamDate = UserParams.CustomParam("param_date");
			customParamTerritory = UserParams.CustomParam("param_territory");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);
			
			// значения по умолчанию
			paramDate = DateTime.Now.Date;
			paramTerritory = UserParams.StateArea.Value;
			paramTerritory = "Ямало-Ненецкий автономный округ";
			paramTerritoryId = Int32.Parse(CustomParams.GetSubjectIdByName(paramTerritory));

			if (!Cube2Relational_CastTerra.ContainsKey(paramTerritoryId))
				throw new Exception(
					String.Format(
						"В словаре Cube2Relational_CastTerra не найдено сопоставление территории с реляционной базой: {0}({1})",
						paramTerritory, paramTerritoryId));

			// параметры
			customParamTerritory.Value = Convert.ToString(Cube2Relational_CastTerra[paramTerritoryId]);
			
			using (IDatabase database = GetDataBase())
			{
				GenerateReport(database);
			}

        }
		
		private void GenerateReport(IDatabase database)
		{
			List<string> monthsParam = new List<string>();
			for (int monthIndex = -CALENDAR_MONTH_DISPERSION; monthIndex < CALENDAR_MONTH_DISPERSION + 1; monthIndex++)
			{
				monthsParam.Add(String.Format("{0:yyyyMM}00", paramDate.AddMonths(monthIndex)));
			}
			customParamDate.Value = String.Join(",", monthsParam.ToArray());
			DataTable dtData = (DataTable)database.ExecQuery(DataProvider.GetQueryText("OOS_0001_0002_data"), QueryResultTypes.DataTable);
			DataTable dtDataBrief = (DataTable)database.ExecQuery(DataProvider.GetQueryText("OOS_0001_0002_data_brief"), QueryResultTypes.DataTable);
			dtDataBrief.PrimaryKey = new[] { dtDataBrief.Columns[0] };
			if (dtData.Rows.Count == 0)
				return;

			StringBuilder dataIncludes = new StringBuilder();
			DataDay day = new DataDay();
			day.Items = new List<DataItem>();
			for (int rowIndex = 0; rowIndex < dtData.Rows.Count; rowIndex++)
			{
				DataRow row = dtData.Rows[rowIndex];

				int typePurchID = CRHelper.DBValueConvertToInt32OrZero(row["TypePurchID"]);
				DataItem item = 
					new DataItem
				        {
							MethodID = typePurchID,
							Method = TypePurch[typePurchID][0].ToUpperFirstSymbol(),
				            BudgetLevel = Convert.ToString(row["BudgetLevel"]),
				            Terra = Convert.ToString(row["Terra"]),
							Customer = Convert.ToString(row["Customer"]).ToUpperFirstSymbol(),
				            Purchase = Convert.ToString(row["Purchase"]).Trim().TrimEnd('.').ToUpperFirstSymbol(),
				            Price = FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(row["Price"]), 3),
				            Link = Convert.ToString(row["Link"]),
				            RefPublicDate = Convert.ToString(row["RefPublicDate"]),
				            RefGiveDate = Convert.ToString(row["RefGiveDate"]),
				            RefConsiderDate = Convert.ToString(row["RefConsiderDate"]),
				            RefMatchDate = Convert.ToString(row["RefMatchDate"]),
				            RefResultDate = Convert.ToString(row["RefResultDate"])
				        };
				day.Items.Add(item);
				
				if ((rowIndex == dtData.Rows.Count-1) || 
					((rowIndex < dtData.Rows.Count-1) && (dtData.Rows[rowIndex][0].ToString() != dtData.Rows[rowIndex+1][0].ToString())))
				{
					DataRow rowBrief = dtDataBrief.Rows.Find(dtData.Rows[rowIndex][0].ToString());
					day.GenerationDate = paramDate.ToString("yyyyMMdd");
					day.PublicCount = CRHelper.DBValueConvertToInt32OrZero(rowBrief["PublicCount"]);
					day.PublicPrice = FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(rowBrief["PublicPrice"]), 0);
					day.GiveCount = CRHelper.DBValueConvertToInt32OrZero(rowBrief["GiveCount"]);
					day.GivePrice = FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(rowBrief["GivePrice"]), 0);
					day.ConsiderCount = CRHelper.DBValueConvertToInt32OrZero(rowBrief["ConsiderCount"]);
					day.ConsiderPrice = FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(rowBrief["ConsiderPrice"]), 0);
					day.ResultCount = CRHelper.DBValueConvertToInt32OrZero(rowBrief["ResultCount"]);
					day.ResultPrice = FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(rowBrief["ResultPrice"]), 0);

					day.PrepareArray();
					string currentDay = dtData.Rows[rowIndex][0].ToString();
					string filename = String.Format("{0}db_{1}.xml", HttpContext.Current.Server.MapPath("~/TemporaryImages/"), currentDay);
					File.WriteAllText(filename, JSONHelper.Serialize(day), Encoding.Default);
					dataIncludes.AppendFormat("<include id=\"db_{0}\" src=\"../../../TemporaryImages/db_{0}.xml\"></include>", currentDay);
					
					day = new DataDay();
					day.Items = new List<DataItem>();
				}

			}
			Includes.InnerHtml = dataIncludes.ToString();
		}

		private static string FormatCurrency(decimal currency, int digits)
		{
			string format = String.Format("N{0}", digits);
			if (currency == 0)
				return String.Empty;
			return
				currency < 1000000
				? String.Format("{0} тыс. руб", (currency / 1000m).ToString(format))
				: String.Format("{0} млн. руб", (currency / 1000000m).ToString(format));
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

	public class JSONHelper
	{
		public static string Serialize<T>(T obj)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
			MemoryStream ms = new MemoryStream();
			serializer.WriteObject(ms, obj);
			string retVal = Encoding.Default.GetString(ms.ToArray());
			ms.Dispose();
			return retVal;
		}

		public static T Deserialize<T>(string json)
		{
			T obj = Activator.CreateInstance<T>();
			MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
			obj = (T)serializer.ReadObject(ms);
			ms.Close();
			ms.Dispose();
			return obj;
		}
	}

	[DataContract]
	public class DataDay
	{
		[DataMember]
		public string GenerationDate { set; get; }

		[DataMember]
		public int PublicCount { set; get; }
		[DataMember]
		public string PublicPrice { set; get; }
		
		[DataMember]
		public int GiveCount { set; get; }
		[DataMember]
		public string GivePrice { set; get; }

		[DataMember]
		public int ConsiderCount { set; get; }
		[DataMember]
		public string ConsiderPrice { set; get; }

		[DataMember]
		public int ResultCount { set; get; }
		[DataMember]
		public string ResultPrice { set; get; }

		[IgnoreDataMember]
		public List<DataItem> Items { set; get; }

		[DataMember]
		public DataItem[] Array { set; get; }

		public void PrepareArray()
		{
			Array = Items.ToArray();
		}
	}

	[DataContract]
	public class DataItem
	{
		[DataMember]
		public string Method { set; get; }

		[DataMember]
		public int MethodID { set; get; }

		[DataMember]
		public string BudgetLevel { set; get; }

		[DataMember]
		public string Terra { set; get; }

		[DataMember]
		public string Customer { set; get; }

		[DataMember]
		public string Purchase { set; get; }

		[DataMember]
		public string Price { set; get; }

		[DataMember]
		public string Link { set; get; }

		[DataMember]
		public string RefPublicDate { set; get; }

		[DataMember]
		public string RefGiveDate { set; get; }

		[DataMember]
		public string RefConsiderDate { set; get; }

		[DataMember]
		public string RefMatchDate { set; get; }

		[DataMember]
		public string RefResultDate { set; get; }
	}

}