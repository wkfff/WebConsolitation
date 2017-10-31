using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Wizards;
using Krista.FM.Client.Components;
using MSXML2;
using Infragistics.Win;
using Krista.FM.Client.Common;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Wizards
{
	public partial class LoadRateExchangeWizard : Form
	{
		public LoadRateExchangeWizard()
		{
			InitializeComponent();

			ultraDateTimeEditor1.DateTime = new DateTime(DateTime.Now.Year, 1, 1);
			ultraDateTimeEditor2.DateTime = DateTime.Now;

			grid.ugData.DisplayLayout.GroupByBox.Hidden = true;
			grid.utmMain.Visible = false;
			grid.AllowAddNewRecords = false;

			InfragisticComponentsCustomize.CustomizeUltraGridParams(grid.ugData);
		}
        
		private void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
		{
			if (e.CurrentPage == wizardDateRangePage)
			{
				if (dataTable1.Rows.Count == 0)
				{
					// Загрузить список валют
					IXMLDOMDocument doc = null;
					try
					{
						doc = RateExchangeLoader.RequestValutaList();
					}
					catch (FinSourcePlanningException ex)
					{
						MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Step = 0;
						return;
					}

					foreach (IXMLDOMNode node in doc.selectNodes("/Valuta/Item"))
					{
						DataRow row = dataTable1.NewRow();
						row[0] = node.attributes[0].text;
						row[1] = node.selectSingleNode("Name").text;
						row[2] = node.selectSingleNode("Nominal").text;
						if (node.attributes[0].text == "R01235" ||
						    node.attributes[0].text == "R01239")
							row[3] = true;
						else
							row[3] = false;
					    //row[4] = 0;
						dataTable1.Rows.Add(row);
					}
					grid.DataSource = dataSet1;

					UltraGridBand band = grid.ugData.DisplayLayout.Bands[0];

					UltraGridColumn clmn;

					grid.ugData.DisplayLayout.Bands[0].Columns[0].Hidden = true;
				    grid.AllowAddNewRecords = false;
				    grid.AllowClearTable = false;
				    grid.AllowDeleteRows = false;
				    grid.StateRowEnable = false;

				    band.Columns["ColumnCheck"].CellActivation = Activation.AllowEdit;
                    // убираем колонку с состоянием записи
                    if (band.Columns.Exists(UltraGridEx.StateColumnName))
                        band.Columns.Remove(UltraGridEx.StateColumnName);

					clmn = band.Columns[1];
					clmn.CellActivation = Activation.ActivateOnly;
					clmn.AllowRowFiltering = DefaultableBoolean.True;
					clmn.AllowRowSummaries = AllowRowSummaries.False;
					clmn.Width = 320;

					clmn = band.Columns[2];
					clmn.CellActivation = Activation.ActivateOnly;
					clmn.AllowRowFiltering = DefaultableBoolean.True;
					clmn.AllowRowSummaries = AllowRowSummaries.False;
					clmn.Width = 96;

				}
				e.Step = 1;
			}

			if (e.CurrentPage == wizardValutaPage)
			{
				e.Step = 1;
				wizard.WizardButtons = WizardForm.TWizardsButtons.None;
				// Загрузить курсы валют
				LoadRateExchange();
                
			}
		}

		private void wizard_Back(object sender, WizardForm.EventNextArgs e)
		{
			if (e.CurrentPage == wizardFinalPage1)
			{
				e.Step = 2;
			}
		}

		private void LoadRateExchange()
		{
			lbProgress.Text = "Инициализация...";
			new RateExchangeLoader(this);
		}

		private void wizard_Cancel(object sender, WizardForm.EventWizardCancelArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void wizard_Finish(object sender, WizardForm.EventNextArgs e)
		{
			Close();
		}

		private void wizard_WizardClosed(object sender, EventArgs e)
		{
			Close();
		}

        internal void SetFinalMessage(string warnings)
        {
            wizardFinalPage1.Description2 = string.Format("Курсы валют были успешно загружены. {0}{0} {1} ", Environment.NewLine, warnings);
        }
	}

	internal class RateExchangeLoader
	{
		private Thread thread;
		
		private int currentProgressValue = 0;

		private LoadRateExchangeWizard wizardForm;

		private SetStringDelegate SetTextDelegate;
	    private SetStringDelegate SetWarningDelegate;
		private SetIntDelegate SetProgressValueDelegate;
		private SetIntDelegate StepNextDelegate;

		public RateExchangeLoader(LoadRateExchangeWizard wizard)
		{
			this.wizardForm = wizard;

			SetTextDelegate = new SetStringDelegate(SetText);
			SetProgressValueDelegate = new SetIntDelegate(SetProgressValue);
			StepNextDelegate = new SetIntDelegate(StepNext);
            SetWarningDelegate = new SetStringDelegate(SetWarningMessage);
			
			thread = new Thread(Run);
			thread.Start();
		}

		private void SetText(string text)
		{
			wizardForm.lbProgress.Text = text;
		}

		private void SetProgressValue(int val)
		{
			wizardForm.progressBar.Value = val;
		}

		private void StepNext(int val)
		{
			wizardForm.wizard.StepNext(val);
			wizardForm.wizard.WizardButtons = WizardForm.TWizardsButtons.AllFinishEnabled;
		}

        private void SetWarningMessage(string warning)
        {
            wizardForm.SetFinalMessage(warning);
        }

		public void Run()
		{
			DataRow[] rows = wizardForm.dataTable1.Select("ColumnCheck = true");

			wizardForm.progressBar.Minimum = 0;
			wizardForm.progressBar.Maximum = rows.Length * 2;
			wizardForm.Invoke(SetProgressValueDelegate, new object[] { currentProgressValue });

			DataTable rateTable = new DataTable();
			rateTable.Columns.Add("RateDate", typeof (DateTime));
			rateTable.Columns.Add("Code", typeof(string));
			rateTable.Columns.Add("Value", typeof(decimal));
			rateTable.Columns.Add("Nominal", typeof(int));

			rateTable.Columns.Add("RefOKV", typeof(int));
            rateTable.Columns.Add("CurrencyCode", typeof(int));
		    StringBuilder sb = new StringBuilder();
			// Загружаем данные из ЦБ
			foreach (DataRow row in rows)
			{
				if (Convert.ToBoolean(row[3]))
				{
					wizardForm.Invoke(SetTextDelegate, new object[] {String.Format("Загрузка курса валюты {0}...", row[1])});
					
					IXMLDOMDocument doc = RequestDocument(
						Convert.ToString(row[0]), 
						wizardForm.ultraDateTimeEditor1.DateTime,
						wizardForm.ultraDateTimeEditor2.DateTime);

					wizardForm.Invoke(SetTextDelegate, new object[] { "Обработка загруженных данных..." });

					DataTable dtOKVCurrency = new DataTable();
					IEntity entity = WorkplaceSingleton.Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_OKV_Currency_Key];
					using (IDataUpdater du = entity.GetDataUpdater())
					{
						du.Fill(ref dtOKVCurrency);
					}
					string valutaCode = ValutaCodes.ResourceManager.GetString(doc.selectSingleNode("ValCurs").attributes[0].text);
                    if (string.IsNullOrEmpty(valutaCode))
                    {
                        sb.AppendLine(string.Format("Курс валюты '{0}' не опубликован на сайте", row[1]));
                    }
                    else
                    {
                        DataRow[] OKVCurrencyRows = dtOKVCurrency.Select(String.Format("Code = {0}", valutaCode));
                        if (OKVCurrencyRows.Length > 0)
                        {
                            foreach (IXMLDOMNode node in doc.selectNodes("/ValCurs/Record"))
                            {
                                DataRow rateRow = rateTable.NewRow();
                                rateRow[0] = Convert.ToDateTime(node.attributes[0].text);
                                rateRow[1] = node.attributes[1].text;
                                rateRow[2] = Convert.ToDecimal(node.selectSingleNode("Value").text);
                                rateRow[3] = Convert.ToInt32(node.selectSingleNode("Nominal").text);
                                rateRow[4] = OKVCurrencyRows[0]["ID"];
                                rateRow[5] = OKVCurrencyRows[0]["Code"];
                                rateTable.Rows.Add(rateRow);
                            }
                        }
                    }
				    wizardForm.Invoke(SetProgressValueDelegate, new object[] { ++currentProgressValue });
				}
			}
		    wizardForm.Invoke(SetWarningDelegate, new object[] {sb.ToString()});
			// Сохраняем в базу данных
			SaveInDatabase(rateTable);

			wizardForm.Invoke(StepNextDelegate, new object[] {1});
		}

		private void SaveInDatabase(DataTable rateTable)
		{
			wizardForm.progressBar.Maximum = wizardForm.progressBar.Maximum + rateTable.Rows.Count;
			wizardForm.Invoke(SetTextDelegate, new object[] { "Сохранение данных в базу..." });

			IEntity entity = WorkplaceSingleton.Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];

			using (IDataUpdater du = entity.GetDataUpdater())
			{
				DataTable dt = new DataTable();
				du.Fill(ref dt);

				foreach (DataRow row in rateTable.Rows)
				{
					DataRow[] rows = dt.Select(String.Format("RefOKV = {0} and DateFixing = '{1}'", row[4], row[0]));
					if (rows.Length == 0)
					{
						DataRow rateRow = dt.NewRow();
						rateRow["ID"] = entity.GetGeneratorNextValue;
						rateRow["DateFixing"] = row[0];
						rateRow["EXCHANGERATE"] = row[2];
						rateRow["Nominal"] = row[3];
						rateRow["RefOKV"] = row[4];
                        rateRow["CurrencyCode"] = row[5];
                        rateRow["ISPROGNOZEXCHRATE"] = 0;
                        rateRow["PumpID"] = -1;
                        rateRow["RowType"] = 0;
						dt.Rows.Add(rateRow);
					}
					wizardForm.Invoke(SetProgressValueDelegate, new object[] { currentProgressValue++ });
				}

				du.Update(ref dt);
			}
		}

		internal static IXMLDOMDocument RequestDocument(string valutaID, DateTime fromDate, DateTime toDate)
		{
			string url = String.Format(
				"http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={0:d2}/{1:d2}/{2}&date_req2={3:d2}/{4:d2}/{5}&VAL_NM_RQ={6}",
				fromDate.Day, fromDate.Month, fromDate.Year,
				toDate.Day, toDate.Month, toDate.Year,
				valutaID);
			XMLHTTPClass request = new XMLHTTPClass();
			request.open("POST", url, false, String.Empty, String.Empty);
			request.setRequestHeader("Host", "www.cbr.ru");
			request.setRequestHeader("Content-Type", "text/xml;");
			request.setRequestHeader("Content-Length", "0");
			request.setRequestHeader("Accept", "text/xml;");
			request.send(String.Empty);

			if (request.status != 200)
			{
				throw new FinSourcePlanningException(String.Format("{0}. Код ошибки: {1}", request.statusText, request.status));
			}

			return (IXMLDOMDocument)request.responseXML;
		}

		internal static IXMLDOMDocument RequestValutaList()
		{
			string url = "http://www.cbr.ru/scripts/XML_val.asp?d=0";
			XMLHTTPClass request = new XMLHTTPClass();
			request.open("POST", url, false, String.Empty, String.Empty);
			request.setRequestHeader("Host", "www.cbr.ru");
			request.setRequestHeader("Content-Type", "text/xml;");
			request.setRequestHeader("Content-Length", "0");
			request.setRequestHeader("Accept", "text/xml;");
			request.send(String.Empty);

			if (request.status != 200)
			{
				throw new FinSourcePlanningException(String.Format("{0}. Код ошибки: {1}", request.statusText, request.status));
			}

			return (IXMLDOMDocument)request.responseXML;
		}
	}
}