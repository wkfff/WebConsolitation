using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class ExportService : IExportService
    {
        private readonly IMarksOmsuRepository repository;
        private readonly IRegionsRepository regionsRepository;
        private readonly IMarksOmsuExtension extension;
        private readonly IMarksRepository marksRepository;
        private readonly IIneffExpencesService ineffExpencesService;

        public ExportService(
            IMarksOmsuRepository repository,
            IRegionsRepository regionsRepository,
            IMarksOmsuExtension extension,
            IMarksRepository marksRepository,
            IIneffExpencesService ineffExpencesService)
        {
            this.repository = repository;
            this.regionsRepository = regionsRepository;
            this.extension = extension;
            this.marksRepository = marksRepository;
            this.ineffExpencesService = ineffExpencesService;
        }

        /// <summary>
        /// Экспорт данных в Excel для ОМСУ.
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="regionId">Id района.</param>
        public Stream ExportForOmsu(int sourceId, int regionId)
        {
            var list = repository.GetAllMarksForMO(regionsRepository.FindOne(regionId));

            Stream template = new MemoryStream(Resources.Resource.TemplateExportForOmsu);

            HSSFWorkbook wb = new HSSFWorkbook(template);

            var sheet = wb.GetSheetAt(0);

            int year = extension.CurrentYear;
            NPOIHelper.SetCellValue(sheet, 1, 4, year - 1);
            NPOIHelper.SetCellValue(sheet, 1, 5, year);
            NPOIHelper.SetCellValue(sheet, 1, 6, year + 1);
            NPOIHelper.SetCellValue(sheet, 1, 7, year + 2);
            NPOIHelper.SetCellValue(sheet, 1, 8, year + 3);

            var cellRanges = new List<CellRangeAddress>();

            int currentRowIndex = 2;
            var currentRowRangeStart = 1;
            var previousRefCompRepID = 0;
            foreach (F_OMSU_Reg16 fact in list)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                if (previousRefCompRepID != fact.RefMarksOMSU.RefCompRep.ID)
                {
                    // Сменилась группа показателей, начнем новую группировку строк.
                    previousRefCompRepID = fact.RefMarksOMSU.RefCompRep.ID;
                    cellRanges.Add(new CellRangeAddress(currentRowRangeStart, currentRowIndex - 1, 0, 0));
                    currentRowRangeStart = currentRowIndex;
                }

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefMarksOMSU.RefCompRep.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, fact.RefMarksOMSU.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.RefMarksOMSU.RefOKEI.Symbol);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.RefMarksOMSU.CodeRepDouble);

                HSSFCellStyle cellStyle = GetCellStyle(wb, fact.RefMarksOMSU);
                
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.PriorValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.CurrentValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 6, fact.Prognoz1, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 7, fact.Prognoz2, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 8, fact.Prognoz3, cellStyle);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 9, fact.Note);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Применим группировку в первом столбце
            foreach (var cellRange in cellRanges)
            {
                sheet.AddMergedRegion(cellRange);
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel для ОИВ.
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="markId">Id показателя.</param>
        public Stream ExportForOiv(int sourceId, int markId)
        {
            var list = repository.GetForOIV(markId);

            Stream template = new MemoryStream(Resources.Resource.TemplateExportForOiv);

            HSSFWorkbook wb = new HSSFWorkbook(template);

            var sheet = wb.GetSheetAt(0);

            int year = extension.CurrentYear;
            NPOIHelper.SetCellValue(sheet, 1, 2, year - 1);
            NPOIHelper.SetCellValue(sheet, 1, 3, year);
            NPOIHelper.SetCellValue(sheet, 1, 4, year + 1);
            NPOIHelper.SetCellValue(sheet, 1, 5, year + 2);
            NPOIHelper.SetCellValue(sheet, 1, 6, year + 3);

            var mark = marksRepository.FindOne(markId);
            NPOIHelper.SetCellValue(sheet, 0, 1, mark.Name);
            NPOIHelper.SetCellValue(sheet, 1, 1, mark.RefOKEI.Name);

            int currentRowIndex = 4;
            foreach (F_OMSU_Reg16 fact in list.OrderBy(x => x.RefRegions.CodeLine))
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefRegions.Name);

                HSSFCellStyle cellStyle = GetCellStyle(wb, fact.RefMarksOMSU);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.PriorValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.CurrentValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Prognoz1, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.Prognoz2, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 6, fact.Prognoz3, cellStyle);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 7, fact.Note);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных и/ф "Форма ввода показателей для оценки эффективности деятельности органов местного самоуправления городских округов и муниципальных районов автономного округа."
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="markId">Id показателя.</param>
        public Stream ExportOivInputData(int sourceId, int markId)
        {
            Stream template = new MemoryStream(Resources.Resource.TemplateExportOivInputData);
            var workbook = new HSSFWorkbook(template);
            var sheet = workbook.GetSheetAt(0);
            
            var mark = marksRepository.FindOne(markId);
            NPOIHelper.SetCellValue(sheet, 0, 1, mark.Name);
            NPOIHelper.SetCellValue(sheet, 1, 1, mark.RefOKEI.Name);
            int year = extension.CurrentYear;
            NPOIHelper.SetCellValue(sheet, 3, 2, year - 1);
            NPOIHelper.SetCellValue(sheet, 3, 3, year);
            
            var list = repository.GetForOIV(markId);

            int currentRowIndex = 4;
            foreach (F_OMSU_Reg16 fact in list.OrderBy(x => x.RefRegions.CodeLine))
            {
                NPOIHelper.CopyRow(workbook, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefRegions.Name);
                HSSFCellStyle cellStyle = GetCellStyle(workbook, fact.RefMarksOMSU);
                
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.PriorValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.CurrentValue, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Note);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            return WriteToStream(workbook);
        }

        public Stream ExportIneffExpenceFacts(int markId, string itfHeader)
        {
            const int FirstDataRowIndex = 5;
            const int FirstSourceFactsColumnIndex = 3;
            const int ReservedSourceColumnsCount = 105;

            Stream template = new MemoryStream(Resources.Resource.TemplateExportIneffExpences);
            var workbook = new HSSFWorkbook(template);
            var sheet = workbook.GetSheetAt(0);

            // Фиксированные данные
            int year = extension.CurrentYear;
            NPOIHelper.SetCellValue(sheet, 4, 1, year - 1);
            NPOIHelper.SetCellValue(sheet, 4, 2, year);
            var mark = marksRepository.FindOne(markId);
            NPOIHelper.SetCellValue(sheet, 0, 0, itfHeader);
            NPOIHelper.SetCellValue(sheet, 1, 1, mark.Name);
            NPOIHelper.SetCellValue(sheet, 2, 1, mark.CalcMark);
            NPOIHelper.SetCellValue(sheet, 3, 1, mark.RefOKEI.Name);

            var calculationPlan = ineffExpencesService.GetMarkCalculationPlan(markId);

            // Заголовки колонок исходных фактов
            var currentColumnIndex = FirstSourceFactsColumnIndex;
            var sourceFactsStyles = new Dictionary<D_OMSU_MarksOMSU, HSSFCellStyle>();
            foreach (var markWithLevel in calculationPlan)
            {
                NPOIHelper.SetCellValue(sheet, 1, currentColumnIndex, markWithLevel.Key.Name);
                string sign = (markWithLevel.Key.RefTypeMark.ID != (int)TypeMark.Calculated)
                              || markWithLevel.Key.Formula.IsNullOrEmpty()
                                  ? markWithLevel.Key.Symbol
                                  : markWithLevel.Key.CalcMark;
                NPOIHelper.SetCellValue(sheet, 2, currentColumnIndex, sign);
                NPOIHelper.SetCellValue(sheet, 3, currentColumnIndex, markWithLevel.Key.RefOKEI.Name);
                NPOIHelper.SetCellValue(sheet, 4, currentColumnIndex, year - 1);
                NPOIHelper.SetCellValue(sheet, 4, currentColumnIndex + 1, year);

                // Подготовим стили ячеек, которые будут потом использованы для отображения значений.
                sourceFactsStyles.Add(markWithLevel.Key, GetCellStyle(workbook, markWithLevel.Key));

                currentColumnIndex += 2;
            }

            // Строки
            var cellStyleNormal = GetCellStyle(workbook, mark);
            var cellStyleRed = GetCellStyle(workbook, mark, true);
            var dataTable = ineffExpencesService.GetTargetFactsViewModel(markId, true);
            int currentRowIndex = FirstDataRowIndex;
            foreach (DataRow currentDataRow in dataTable.Rows)
            {
                NPOIHelper.CopyRow(workbook, sheet, currentRowIndex, currentRowIndex + 1);

                // Основные колонки
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, currentDataRow.Field<string>("NameMO"));
                var priorApproved = Math.Round(currentDataRow.Field<decimal?>("PriorApproved") ?? 0, mark.Capacity ?? 0);
                var currentApproved = Math.Round(currentDataRow.Field<decimal?>("CurrentApproved") ?? 0, mark.Capacity ?? 0);
                
                var hasWarnPrior = currentDataRow.Field<string>("hasWarnPrior") != string.Empty;
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, priorApproved, hasWarnPrior ? cellStyleRed : cellStyleNormal);
                var hasWarnCurrent = currentDataRow.Field<string>("hasWarnCurrent") != string.Empty;
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, currentApproved, hasWarnCurrent ? cellStyleRed : cellStyleNormal);

                // Переменные колонки исходных фактов
                var currentColumnIndex2 = FirstSourceFactsColumnIndex;
                foreach (var markWithLevel in calculationPlan)
                {
                    var priorCalc2 = Math.Round(currentDataRow.Field<decimal?>("Prior" + markWithLevel.Key.ID) ?? 0, markWithLevel.Key.Capacity ?? 0);
                    var currentCalc2 = Math.Round(currentDataRow.Field<decimal?>("Current" + markWithLevel.Key.ID) ?? 0, markWithLevel.Key.Capacity ?? 0);
                    HSSFCellStyle cellStyle;
                    sourceFactsStyles.TryGetValue(markWithLevel.Key, out cellStyle);
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex2, priorCalc2, cellStyle);
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex2 + 1, currentCalc2, cellStyle);
                    currentColumnIndex2 += 2;
                }
                
                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Скрываем неиспользованные колонки исходных фактов
            for (int currentColumnIndex3 = FirstSourceFactsColumnIndex + (calculationPlan.Count * 2);
                currentColumnIndex3 < ReservedSourceColumnsCount;
                currentColumnIndex3++)
            {
                sheet.SetColumnHidden(currentColumnIndex3, true);
            }

            return WriteToStream(workbook);            
        }

        private static HSSFCellStyle GetCellStyle(HSSFWorkbook wb, D_OMSU_MarksOMSU mark, bool markRed = false)
        {
            HSSFFont font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 10;
            if (markRed)
            {
                font.Color = HSSFColor.RED.index;
            }

            HSSFCellStyle cellStyle = wb.CreateCellStyle();
            cellStyle.BorderTop = 1;
            cellStyle.BorderLeft = 1;
            cellStyle.BorderRight = 1;
            cellStyle.BorderBottom = 1;
            cellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            cellStyle.SetFont(font);
            if (mark.Grouping || mark.Formula.IsNotNullOrEmpty())
            {
                cellStyle.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
                cellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            }

            string scale = ".";
            if (mark.Capacity == null)
            {
                scale = String.Empty;
            }
            else if (mark.Capacity == 0)
            {
                scale = String.Empty;
            }
            else
            {
                scale = scale.PadRight((int)mark.Capacity + 1, '0');
            }

            switch (mark.RefOKEI.Symbol)
            {
                case "ДА/НЕТ":
                    cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("\"Да\";\"Да\";\"Нет\"");
                    break;
                default:
                    cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("# ##0{0}".FormatWith(scale));
                    break;
            }

            return cellStyle;
        }

        private static MemoryStream WriteToStream(HSSFWorkbook wb)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;

            MemoryStream resultStream = new MemoryStream();
            wb.Write(resultStream);

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
