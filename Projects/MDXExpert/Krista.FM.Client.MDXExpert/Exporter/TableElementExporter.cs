using System;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    class TableElementExporter: ElementExporter
    {
        public TableElementExporter(CustomReportElement element)
            : base(element)
        {
        }

        public override Excel.Worksheet ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperties)
        {
            if (sheet == null)
                return null;

            string statusBarText = sheet.Application.StatusBar
                + "������������ �������������� ������� \"" + this.Element.Title + "\". ";

            sheet.Application.StatusBar = !this.Element.PadingModeEnable ? statusBarText :
                statusBarText + String.Format("�������� 1 �� {0}.", this.Element.TablePager.PageCount);

            //�������� ����� ������� �� ������� ���������
            int currentPageNumber = this.Element.TablePager.CurrentPageNumber;
            this.Element.TablePager.CurrentPageNumber = 1;

            try
            {
                //��������� ��������
                this.CustomElement.MainForm.Operation.StartOperation();
                this.CustomElement.MainForm.Operation.Text = "������� �������";

                this.Element.ExpertGrid.Exporter.ToExcelWorksheet(sheet, this.GetStartTable(sheet),
                    isPrintVersion, isSeparateProperties);

                //���� ������� �������, � ��������� �� ���������� ��������, ����� ���������� �� 
                //������������� � ��������������
                if (this.Element.PadingModeEnable)
                {
                    for (int i = 2; i <= this.Element.TablePager.PageCount; i++)
                    {
                        sheet.Application.StatusBar = statusBarText
                            + String.Format("�������� {0} �� {1}.", i, this.Element.TablePager.PageCount);
                        this.Element.TablePager.CurrentPageNumber = i;
                        Application.DoEvents();

                        //��������� ��������
                        this.CustomElement.MainForm.Operation.StartOperation();
                        this.CustomElement.MainForm.Operation.Text = "������� �������";

                        this.Element.ExpertGrid.Exporter.ToExcelWorksheet(sheet, isPrintVersion, isSeparateProperties);
                    }
                }

                //����������� � ��������� ����� ��������� � ����� �����
                base.MapElementCaption(sheet, isPrintVersion);
                base.MapTableComment(sheet, isPrintVersion);

                int elementLastRowIndex = 1;
                Excel.Name excelName = ExcelUtils.GetExcelName(sheet, "ExpertGrid");
                if (excelName != null)
                {
                    Excel.Range gridRange = excelName.RefersToRange;
                    if (gridRange != null)
                    {
                        elementLastRowIndex = gridRange.Row + gridRange.Rows.Count;
                    }
                    excelName.Delete();
                }

                if (this.CustomElement.Comment.Visible && (this.CustomElement.Comment.Place == CommentPlace.Bottom))
                {
                    elementLastRowIndex++;
                }
                ExcelUtils.MarkExcelName((sheet.Cells[elementLastRowIndex, 1] as Excel.Range),
                                         Consts.elementLastRow, false);


                //����� �������� �������� �� �� �������, ��� � �� ��� ������
                this.Element.TablePager.CurrentPageNumber = currentPageNumber;
                sheet.Application.StatusBar = false;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.CustomElement.MainForm.Operation.StopOperation();
            }

            return sheet;
        }


        public override Excel.Worksheet ToExcel(Excel.Workbook workbook, bool isPrintVersion, bool isSeparateProperties, int beforeSheetIndex)
        {
            if (workbook == null)
                return null;

            /*string statusBarText = workbook.Application.StatusBar;
                + "������������ �������������� ������� \"" + this.Element.Title + "\". ";

            workbook.Application.StatusBar = !this.Element.PadingModeEnable ? statusBarText :
                statusBarText + String.Format("�������� 1 �� {0}.", this.Element.TablePager.PageCount);*/

            //��������� ���� � ������� ����� ��������������
            Excel.Worksheet tableSheet = (Excel.Worksheet)workbook.Sheets.Add(workbook.Sheets[beforeSheetIndex],
                Type.Missing, Type.Missing, Type.Missing);
            tableSheet.Name = ExcelUtils.GetSheetName(workbook, this.Element.Title);

            return this.ToExcelWorksheet(tableSheet, isPrintVersion, isSeparateProperties);
        }

        private Point GetStartTable(Excel.Worksheet sheet)
        {
            int rowIndex = GetStartRowIndex(sheet);
            int columnIndex = 1;

            if (this.Element.Caption.Visible)
                rowIndex++;

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Top)
                rowIndex++;

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Left)
                columnIndex++;

            return new Point(rowIndex, columnIndex);
        }

        public TableReportElement Element
        {
            get
            {
                return (TableReportElement)base.CustomElement;
            }
        }
    }
}
