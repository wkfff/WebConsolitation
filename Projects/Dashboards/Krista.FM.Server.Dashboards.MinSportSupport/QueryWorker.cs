using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
	public class QueryWorker
	{
		public static readonly string queriesPath = HttpContext.Current.Server.MapPath("~/reports/MinSport/");

		private string name;
		public Dictionary<string, QueryTemplateWorker> Templates { set; get; }


		public QueryWorker(string name)
		{
			this.name = name;
			Templates = new Dictionary<string, QueryTemplateWorker>();
		}

		/// <summary>
		/// Выполнить запрос из текущей папки запросов
		/// </summary>
		/// <returns></returns>
		public DataTable GetDataTable()
		{
			return GetBaseDataTable(String.Empty);
		}

		/// <summary>
		/// Выполнить запрос из общей папки запросов
		/// </summary>
		/// <returns></returns>
		public DataTable GetCommonDataTable()
		{
			return GetBaseDataTable(queriesPath);
		}

		/// <summary>
		/// Получить запрос из указанной папки запросов
		/// </summary>
		/// <param name="queryPath"></param>
		/// <returns></returns>
		private DataTable GetBaseDataTable(string queryPath)
		{
			string query = DataProvider.GetQueryText(name, queryPath);

			// применить шаблоны
			query = ImplantTemplates(query);
			
			DataTable dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtGrid);
			return dtGrid;
		}
		
		/// <summary>
		/// Применить шаблоны к запросу
		/// </summary>
		/// <param name="queryText">Первоначальный текст запроса</param>
		private string ImplantTemplates(string queryText)
		{
			Regex regex = new Regex(@"<#([\s\S]*?)#>");
			MatchCollection templateCollection = regex.Matches(queryText);

			foreach (Match match in templateCollection)
			{
				string templateName = match.Value.TrimStart('<').TrimStart('#').TrimEnd('>').TrimEnd('#');
				queryText = queryText.Replace(match.Value, Templates[templateName].ApplyRule());
			}

			return queryText;
		}

		/// <summary>
		/// Получить шаблон запроса
		/// </summary>
		/// <param name="QueryID"></param>
		/// <returns></returns>
		public static string GetQueryTemplate(string QueryID)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(queriesPath);

			foreach (FileInfo f in dirInfo.GetFiles(CustomReportConst.QueryFileMasc))
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(f.FullName);

				XmlNode xmlNode = xmlDoc.SelectSingleNode(string.Format("//query[@id='{0}']", QueryID));
				if (xmlNode != null)
				{
					return xmlNode.LastChild.InnerText;
				}
			}
			throw new Exception("Не удалось найти текст запроса " + QueryID);
		}

	}
}
