using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.IO;

using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core.DataProviders
{
	/// <summary>
	/// Предназначен для выполнения переименований измерений 
	/// в тексте запроса из формата 2000 в формат 2005.
	/// </summary>
	public sealed class MdxQueryRenameService
	{
		private Dictionary<string, string> renameList;

		private MdxQueryRenameService()
		{
			string fileName = ConfigurationManager.AppSettings["MDXQueryRenameList"];

			if (!String.IsNullOrEmpty(fileName))
				Init(fileName);
		}

		/// <summary>
		/// Инициализация сервиса.
		/// </summary>
		/// <param name="configurationFile">Путь к конфигурационному файлу с настройками переименований.</param>
		public void Init(string configurationFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(CRHelper.BasePath, configurationFile));
			renameList = new Dictionary<string, string>();
			foreach (XmlNode xmlNode in doc.SelectNodes("/replaces/replace"))
			{
				string oldString = xmlNode.Attributes["old"].Value;
				string newString = xmlNode.Attributes["new"].Value;

				string[] oldParts = oldString.Split(new char[] {'.'});
				string[] newParts = newString.Split(new char[] { '.' });

				if (oldParts.GetLength(0) == 1)
					oldString = String.Format("[{0}]", oldParts[0]);
				else
					oldString = String.Format("[{0}].[{1}]", oldParts[0], oldParts[1]);

				renameList.Add(
					oldString,
					String.Format("[{0}].[{1}]", newParts[0], newParts[1]));
			} 
		}

		public string Rename(string queryString)
		{
			if (renameList == null)
				return queryString;

			foreach (KeyValuePair<string, string> renameItem in renameList)
			{
				queryString	= queryString.Replace(renameItem.Key, renameItem.Value);
			}

			return queryString;
		}
	}
}
