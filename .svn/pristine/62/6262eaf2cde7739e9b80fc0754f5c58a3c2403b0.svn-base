using System;
using System.Collections.Generic;
using System.IO;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class ExportService : IExportService
    {
        private readonly IFactRepository factRepository;
        private readonly IRegion10MarksOivExtension extension;
        private readonly ITerritoryRepository territoryRepository;

        public ExportService(
                              IFactRepository factRepository, 
                              IRegion10MarksOivExtension extension, 
                              ITerritoryRepository territoryRepository)
        {
            this.factRepository = factRepository;
            this.extension = extension;
            this.territoryRepository = territoryRepository;
        }

        public Stream ExportForOiv(int sourceId, int territoryId, bool isIMA)
        {
            IList<F_OIV_REG10Qual> list;
            if (isIMA)
            {
                list = factRepository.GetMarksForIMA();
            }
            else
            {
                list = factRepository.GetMarksForOiv();
            }

            return CreateOivReport(list, extension.CurrentYearVal);
        }

        public Stream ExportForOmsu(int sourceId, int territoryId)
        {
            IList<F_OIV_REG10Qual> list;

            list = factRepository.GetMarks(territoryRepository.FindOne(territoryId));

            return CreateOivReport(list, extension.CurrentYearVal);
        }

        public Stream ExportForOmsuCompare(int sourceId, int? markId)
        {
            IList<F_OIV_REG10Qual> list;
            if (markId == null)
            {
                list = new List<F_OIV_REG10Qual>();
            }
            else
            {
                list = factRepository.GetTerritories((int)markId);
            }

            return CreateOmsuCompareReport(list, extension.CurrentYearVal);
        }

        private static Stream CreateOivReport(IList<F_OIV_REG10Qual> factList, int year)
        {
            Stream template = new MemoryStream(Resources.Resource.TemplateExportForOiv);

            HSSFWorkbook wb = new HSSFWorkbook(template);

            var sheet = wb.GetSheetAt(0);

            NPOIHelper.SetCellValue(sheet, 1, 4, year);
            NPOIHelper.SetCellValue(sheet, 1, 5, year + 1);
            NPOIHelper.SetCellValue(sheet, 1, 6, year + 2);
            NPOIHelper.SetCellValue(sheet, 1, 7, year + 3);

            int currentRowIndex = 2;
            foreach (F_OIV_REG10Qual fact in factList)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefOIV.RefOIV.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, fact.RefOIV.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.RefOIV.RefUnits.Symbol);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.RefOIV.CodeRep);

                HSSFCellStyle cellStyle = GetCellStyle(wb, fact.RefOIV);
                int correction = GetValueTypeCorrection(fact.RefOIV);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Fact / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.Forecast / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 6, fact.Forecast2 / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 7, fact.Forecast3 / correction, cellStyle);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 8, fact.Note);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb);
        }
        
        private static Stream CreateOmsuCompareReport(IList<F_OIV_REG10Qual> factList, int year)
        {
            Stream template = new MemoryStream(Resources.Resource.TemplateExportForOmsuCompare);

            HSSFWorkbook wb = new HSSFWorkbook(template);

            var sheet = wb.GetSheetAt(0);

            NPOIHelper.SetCellValue(sheet, 1, 1, year);
            NPOIHelper.SetCellValue(sheet, 1, 2, year + 1);
            NPOIHelper.SetCellValue(sheet, 1, 3, year + 2);
            NPOIHelper.SetCellValue(sheet, 1, 4, year + 3);

            int currentRowIndex = 2;
            foreach (F_OIV_REG10Qual fact in factList)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefTerritory.Name);
                
                HSSFCellStyle cellStyle = GetCellStyle(wb, fact.RefOIV);
                int correction = GetValueTypeCorrection(fact.RefOIV);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, fact.Fact / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.Forecast / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.Forecast2 / correction, cellStyle);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Forecast3 / correction, cellStyle);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        private static HSSFCellStyle GetCellStyle(HSSFWorkbook wb, D_OIV_Mark mark)
        {
            HSSFFont font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 10;

            HSSFCellStyle cellStyle = wb.CreateCellStyle();
            cellStyle.BorderTop = 1;
            cellStyle.BorderLeft = 1;
            cellStyle.BorderRight = 1;
            cellStyle.BorderBottom = 1;
            cellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            cellStyle.SetFont(font);
            if (mark.Formula.IsNotNullOrEmpty())
            {
                cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
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

            switch (mark.RefUnits.Symbol)
            {
                case "ДА/НЕТ":
                    cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("\"Да\";\"Да\";\"Нет\"");
                    break;
                case "ПРОЦ":
                    cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("# ##0{0}%".FormatWith(scale));
                    break;
                default:
                    cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("# ##0{0}".FormatWith(scale));
                    break;
            }

            return cellStyle;
        }

        private static int GetValueTypeCorrection(D_OIV_Mark mark)
        {
            int correction = 1;
            if (mark.RefUnits.Symbol == "ПРОЦ")
            {
                correction = 100;
            }

            return correction;
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
