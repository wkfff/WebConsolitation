using System;
using System.Collections.Generic;
using System.Data;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class FO1601Gadget : GadgetControlBase
	{
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			DataTable dataTable = new DataTable();
			string query = DataProvider.GetQueryText("FO_0016_0001");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dataTable);

			Grid.Rows[0].Cells[0].Value = dataTable.Rows[0][0];
			Grid.Rows[0].Cells[1].Value = dataTable.Rows[0][1];
			Grid.Rows[0].Cells[2].Value = dataTable.Rows[0][2];

			Grid.Rows[1].Cells[0].Value = TrimTextList(Convert.ToString(dataTable.Rows[0][3]));
			Grid.Rows[1].Cells[1].Value = TrimTextList(Convert.ToString(dataTable.Rows[0][4]));
			Grid.Rows[1].Cells[2].Value = TrimTextList(Convert.ToString(dataTable.Rows[0][5]));
		}

		/// <summary>
		/// Обрезает текстовый список до 10 элементов.
		/// </summary>
		private static string TrimTextList(string text)
		{
			List<string> list = new List<string>();
			string[] parts = text.Split(new char[] {','});
			int count = 1;
			foreach (string part in parts)
			{
				list.Add(part.Trim());
				if (count >= 8)
				{
					list.Add("...");
					break;
				}
				count++;
			}
			text = String.Join("\n", list.ToArray());
			return text;
		}

		protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
		{
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Индикаторы БККУ по МО"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/ФО_0016_0001/Default.aspx"; }
		}

		#endregion
	}
}