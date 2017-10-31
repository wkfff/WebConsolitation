using System;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    public class ReportExporter
    {
        private MainForm _report;

        public ReportExporter(MainForm report)
        {
            this.Report = report;
        }

        /// <summary>
        /// Экспорт всех элементов отчета в Excel. Если путь к книге не указан, создасть новую.
        /// </summary>
        /// <param name="bookPath"></param>
        public void ToExcel(string bookPath, bool isPrintVersion, bool isUnionExport, bool isSeparateProperties)
        {
            Excel.Workbook excelBook = ExcelUtils.GetExcelBookForExport(bookPath);
            if (excelBook == null)
                return;

            try
            {
                Common.ExcelUtils.SetExcelAplFocus(excelBook.Application);

                excelBook.Application.Interactive = false;
                excelBook.Application.ScreenUpdating = false;

                //Если экспортируем все элементы отчета на один лист, то создаем этот общий лист
                Excel.Worksheet commonSheet = null;
                if (isUnionExport)
                {
                    commonSheet = (Excel.Worksheet)excelBook.Sheets.Add(excelBook.Sheets[1],
                        Type.Missing, Type.Missing, Type.Missing);

                    string reportName =
                        CommonUtils.SeparateFileName(this.Report.ReportFileName == string.Empty
                                                         ? Consts.newReportName
                                                         :
                                                             this.Report.ReportFileName, false);
                    commonSheet.Name = ExcelUtils.GetSheetName(excelBook, reportName);
                }

                for (int i = 0; i < this.Report.ControlPanes.Count; i++)
                {
                    CustomReportElement reportElement = (this.Report.ControlPanes[i].Control as CustomReportElement);
                    excelBook.Application.StatusBar = String.Format("Идет экспорт {0} из {1} элементов. ",
                        i + 1, this.Report.ControlPanes.Count);
                    if (isUnionExport)
                    {
                        reportElement.Exporter.ToExcelWorksheet(commonSheet, isPrintVersion, isSeparateProperties);
                    }
                    else
                    {
                        reportElement.Exporter.ToExcel(excelBook, isPrintVersion, isSeparateProperties, i + 1);
                    }
                }
                if (excelBook.Sheets.Count > 0)
                    (excelBook.Sheets[1] as Excel.Worksheet).Activate();

                excelBook.Application.Interactive = true;
                excelBook.Application.ScreenUpdating = true;

                excelBook.Application.StatusBar = false;
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e, ErrorFormButtons.WithoutTerminate);
            }
        }

        /// <summary>
        /// Экспорт активного элемента отчета в Excel. Если путь к книге не указан, создасть новую.
        /// </summary>
        /// <param name="bookPath"></param>
        public void ActiveElementToExcel(string bookPath, bool isPrintVersion, bool isSeparateProperties)
        {
            Excel.Workbook excelBook = ExcelUtils.GetExcelBookForExport(bookPath);
            if (excelBook == null)
            {
                return;
            }
            try
            {
                Common.ExcelUtils.SetExcelAplFocus(excelBook.Application);

                if (Report.ActiveElement != null)
                    Report.ActiveElement.Exporter.ToExcel(excelBook, isPrintVersion, isSeparateProperties, 1);

                excelBook.Application.StatusBar = false;
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e, ErrorFormButtons.WithoutTerminate);
            }
        }

        public MainForm Report
        {
            get { return _report; }
            set { _report = value; }
        }
    }
}
