using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class QueryTemplate
	{
		private readonly string templateID;
		private Dictionary<string, string> rules;


		public QueryTemplate(string template)
		{
			templateID = template;
			rules = new Dictionary<string, string>();
		}

		public void AddRule(string key, string value)
		{
			rules.Add(key, value);
		}

		public string ApplyRule()
		{
			string queryText = Query.GetQueryTemplate(templateID);
			
			Regex regex = new Regex(@"<#([\s\S]*?)#>");
			MatchCollection parameters = regex.Matches(queryText);

			foreach (Match match in parameters)
			{
				string paramName = match.Value.TrimStart('<').TrimStart('#').TrimEnd('>').TrimEnd('#');
				queryText = queryText.Replace(match.Value, rules[paramName]);
			}

			return queryText;
		}

	}
}
