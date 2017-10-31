using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Providers.DataAccess;
using Krista.FM.ServerLibrary;
using MSXML2;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	/// <summary>
	/// Запрос ставки рефинансирования от ЦБ.
	/// </summary>
	public class RequestRefinancingRateCommand : AbstractCommand
	{
		public RequestRefinancingRateCommand()
		{
			key = "RequestRefinancingRateCommand";
			caption = "Запросить ставки рефинансирования от ЦБР (требуется подключение к Интернет)";
			iconKey = "ButtonBlue";
		}

		public override void Run()
		{
			try
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Загрузка данных из ЦБ...";
				
				DataTable sourceTable = RequestValutaList();

				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Сохранение данных в базу...";

				IEntity entity = WorkplaceSingleton.Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_JournalCB_Key];

				using (IDataUpdater du = entity.GetDataUpdater())
				{
					DataTable dt = new DataTable();
					du.Fill(ref dt);

					foreach (DataRow row in sourceTable.Rows)
					{
						DataRow[] rows = dt.Select(String.Format("InputDate = '{0}'", row[3]));
						if (rows.Length == 0)
						{
							DataRow rateRow = dt.NewRow();
							rateRow["ID"] = entity.GetGeneratorNextValue;
                            rateRow["RowType"] = 0;
							rateRow["InputDate"] = row[3];
                            rateRow["PercentRate"] = row[1];
							rateRow["ReferencedData"] = row[2];
							dt.Rows.Add(rateRow);
						}
						else
						{
                            rows[0]["PercentRate"] = row[1];
						}
					}

					du.Update(ref dt);
				}

				((BaseClsUI) WorkplaceSingleton.Workplace.ActiveContent).Refresh();
			}
			catch (FinSourcePlanningException ex)
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
			}
		}


		internal static DataTable RequestValutaList()
		{
			string url = "http://www.cbr.ru/print.asp?file=/statistics/credit_statistics/refinancing_rates.htm";
			XMLHTTPClass request = new XMLHTTPClass();
			request.open("POST", url, false, String.Empty, String.Empty);
			request.setRequestHeader("Host", "www.cbr.ru");
			request.setRequestHeader("Content-Type", "text/html; charset=windows-1251");
			request.setRequestHeader("Content-Length", "0");
			request.setRequestHeader("Accept", "text/html; charset=windows-1251");
			request.send(String.Empty);

			if (request.status != 200)
			{
				throw new FinSourcePlanningException(String.Format("{0}. Код ошибки: {1}", request.statusText, request.status));
			}

			HtmlHelper ht = new HtmlHelper();
			DataSet ds = ht.GetTablesFromHtml(new System.IO.MemoryStream((byte[])request.responseBody), true);

			DataTable table = null;
			foreach (DataTable t in ds.Tables)
			{
				if (t.Columns.Count != 3)
					continue;
				if (t.Rows.Count < 2)
					continue;
				if (Convert.ToString(t.Rows[0][0]) == "Период действия" &&
					Convert.ToString(t.Rows[0][1]) == "%" &&
					Convert.ToString(t.Rows[0][2]) == "Нормативный документ")
				{
					table = t;
					break;
				}
			}

			if (table != null)
			{
				table.Rows[0].Delete();
				table.Columns.Add("StartDate", typeof(DateTime));
				table.Columns.Add("EndDate", typeof(DateTime));
				foreach (DataRow row in table.Rows)
				{
					string[] dates = Convert.ToString(row[0]).Split(new string[] { "–" }, System.StringSplitOptions.None);
					row[3] = DateTime.Parse(dates[0]);
					if (!String.IsNullOrEmpty(dates[1]))
						row[4] = DateTime.Parse(dates[1]); 
				}
			}

			return table;
		}
	}
}
