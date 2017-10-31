using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Infragistics.Excel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message
{
	public class ValidationMessagesReporter
	{
		private List<ValidationResultVisualizator> visualizators;

		private String filename;

		public ValidationMessagesReporter(List<ValidationResultVisualizator> visualizators)
		{
			this.visualizators = visualizators;
		}

		/// <summary>
		/// Имя файла для записи отчета по умолчанию
		/// </summary>
		public string FileName
		{
			get { return filename; }
			set { filename = value; }
		}

		public void GetValidationReport(IWin32Window parent)
		{
			DataTable dt = new DataTable();
			DataColumn clmn = dt.Columns.Add("Message");
			clmn.Caption = "Сообщение об ошибке";
			foreach (ValidationResultVisualizator visualizator in visualizators)
			{
				if (!string.IsNullOrEmpty(visualizator.ValidationMessage.Summary))
				{
					DataRow row = dt.NewRow();
					row[0] = visualizator.ValidationMessage.Summary;
					dt.Rows.Add(row);
				}
			}
			dt.AcceptChanges();
			UltraGrid grid = new UltraGrid();
			grid.Parent = (Control) parent;
			grid.Visible = false;
			grid.DataSource = dt;

			UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();
			Workbook wb = new Workbook();
			excelExpoter.Export(grid, wb);
			if (ExportImportHelper.GetFileName(filename, ExportImportHelper.fileExtensions.xls, true, ref filename))
				wb.Save(filename);
		}
	}
}