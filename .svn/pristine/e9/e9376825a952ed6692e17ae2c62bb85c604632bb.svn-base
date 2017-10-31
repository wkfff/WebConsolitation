using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public static class StateTaskFormHelper
    {
        public const int LastCol = 9;

        private const int FistCol = 1;

        private static HSSFCellStyle defaultStyleForData;

        private static HSSFCellStyle defaultStyleForHead;

        public static void ClearStyles()
        {
            defaultStyleForData = null;
            defaultStyleForHead = null;
        }

        public static void Part(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, int part)
        {
            var cellStyle = GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            font.FontHeightInPoints = 16;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;

            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Раздел {0}".FormatWith(part)).CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 3, currentRow, 5);
            currentRow += 2;
        }

        public static void MainHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, bool service)
        {
            var cellStyle = GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.FontHeightInPoints = 12;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;

            if (service)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 8, "Приложение 1");
                currentRow++;
            }

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "ГОСУДАРСТВЕННОЕ (МУНИЦИПАЛЬНОЕ) ЗАДАНИЕ").CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, LastCol);

            currentRow++;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "НА ОКАЗАНИЕ ГОСУДАРСТВЕННЫХ (МУНИЦИПАЛЬНЫХ) {0}".FormatWith(service ? "УСЛУГ" : "РАБОТ")).CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, LastCol);

            currentRow += 2;
        }

        public static void Indicators(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service, List<F_F_PNRZnach> indicators)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Показатели, характеризующие качество и (или) объем  государственной (муниципальной) услуги :");
            currentRow++;

            var qualityIndicators = indicators.Where(x => x.RefIndicators.RefCharacteristicType.ID == FX_FX_CharacteristicType.QualityIndex).ToList();
            var volumeIndicators = indicators.Where(x => x.RefIndicators.RefCharacteristicType.ID == FX_FX_CharacteristicType.VolumeIndex).ToList();

            if (qualityIndicators.Any())
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, "Показатели качества государственной (муниципальной) услуги:");
                currentRow++;
                IndicatorsHeader(workBook, sheet, ref currentRow, true, service.RefParametr.RefYearForm.ID);

                var counter = 1;
                foreach (var qualityIndicator in qualityIndicators)
                {
                    IndicatorRowFormat(workBook, sheet, currentRow, true, qualityIndicator, counter);

                    counter++;
                    currentRow++;
                }

                NpoiHelper.SetCellValue(sheet, currentRow, 0, "* N - значение показаетля в отчетный период, V - общий объем");
                currentRow++;
            }

            if (volumeIndicators.Any())
            {
                currentRow++;
                NpoiHelper.SetCellValue(sheet, currentRow, 0, "Плановый объем  государственной (муниципальной) услуги (в натуральных показателях):");
                currentRow++;
                IndicatorsHeader(workBook, sheet, ref currentRow, false, service.RefParametr.RefYearForm.ID);

                var counter = 1;
                foreach (var volumeIndicator in volumeIndicators)
                {
                    IndicatorRowFormat(workBook, sheet, currentRow, false, volumeIndicator, counter);

                    counter++;
                    currentRow++;
                }
            }

            currentRow++;
        }

        public static void SetDefaultStyle(HSSFWorkbook workBook, HSSFSheet sheet, int lastCol)
        {
            sheet.SetColumnWidth(0, 3600);
            sheet.SetColumnWidth(1, 4000);
            sheet.SetColumnWidth(2, 2600);
            sheet.SetColumnWidth(3, 2600);
            sheet.SetColumnWidth(3, 2600);
            sheet.SetColumnWidth(4, 2600);
            sheet.SetColumnWidth(5, 2600);
            sheet.SetColumnWidth(6, 2600);
            sheet.SetColumnWidth(7, 2600);
            sheet.SetColumnWidth(8, 2600);
            sheet.SetColumnWidth(9, 3600);

            var style = GetDefaultStyle(workBook);

            for (short i = 0; i <= lastCol; i++)
            {
                sheet.SetDefaultColumnStyle(i, style);
            }
        }

        public static void SphereOfActivity(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, string value)
        {
            var cellStyleAssistent = GetDefaultStyle(workBook);
            cellStyleAssistent.GetFont(workBook).FontHeightInPoints = 10;
            cellStyleAssistent.Alignment = HSSFCellStyle.ALIGN_CENTER;

            NpoiHelper.SetCellValue(sheet, currentRow, FistCol, value).CellStyle = GetDefaultStyleForHead(workBook);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, FistCol, LastCol);

            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, FistCol, "(наименование отраслевой направленности государственных (муниципальных) услуг)").CellStyle = cellStyleAssistent;
            NpoiHelper.SetMergedRegion(sheet, currentRow, FistCol, currentRow, LastCol);

            SetBorderTop(workBook, sheet, currentRow, FistCol, LastCol);

            currentRow += 2;
        }

        public static void Сustomer(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, string value)
        {
            var cellStyleAssistent = GetDefaultStyle(workBook);
            cellStyleAssistent.GetFont(workBook).FontHeightInPoints = 10;
            cellStyleAssistent.Alignment = HSSFCellStyle.ALIGN_CENTER;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Заказчик:");
            NpoiHelper.SetCellValue(sheet, currentRow, FistCol, value).CellStyle = GetDefaultStyleForHead(workBook);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, FistCol, LastCol);
            currentRow++;
            NpoiHelper.SetCellValue(sheet, currentRow, FistCol, "(наименование главного распорядителя бюджетных средств (учредителя))").CellStyle = cellStyleAssistent;
            NpoiHelper.SetMergedRegion(sheet, currentRow, FistCol, currentRow, LastCol);

            SetBorderTop(workBook, sheet, currentRow, FistCol, LastCol);

            currentRow++;
        }

        public static void Performer(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, string value)
        {
            var cellStyleAssistent = GetDefaultStyle(workBook);
            cellStyleAssistent.GetFont(workBook).FontHeightInPoints = 10;
            cellStyleAssistent.Alignment = HSSFCellStyle.ALIGN_CENTER;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Исполнитель:");
            NpoiHelper.SetCellValue(sheet, currentRow, FistCol, value).CellStyle = GetDefaultStyleForHead(workBook);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, FistCol, LastCol);
            currentRow++;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                FistCol,
                "(полное наименование получателя бюджетных средств государственного (муниципального) учреждения, юридический адрес)").CellStyle = cellStyleAssistent;
            NpoiHelper.SetMergedRegion(sheet, currentRow, FistCol, currentRow, LastCol);

            SetBorderTop(workBook, sheet, currentRow, FistCol, LastCol);

            currentRow += 2;
        }

        public static void Service(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            var work = service.RefVedPch.RefTipY.ID == D_Services_TipY.FX_FX_WORK;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Наименование государственной (муниципальной) {0}".FormatWith(work ? "работы" : "услуги"))
                .CellStyle = GetDefaultStyleForHead(workBook);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 2);

            NpoiHelper.SetCellValue(sheet, currentRow, 3, service.RefVedPch.Name).CellStyle = GetDefaultStyleForHead(workBook);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 3, LastCol);

            currentRow++;
            SetBorderTop(workBook, sheet, currentRow, 3, LastCol);

            currentRow += 2;
        }

        public static void Сonsumers(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service, IEnumerable<F_F_PotrYs> consumers)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Категории потребителей государственной (муниципальной) услуги:");
            currentRow++;

            switch (service.RefVedPch.RefPl.ID)
            {
                case D_Services_Platnost.Free:
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Потребители  бесплатных услуг");
                    currentRow++;
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Категория получателей бесплатных услуг");
                    NpoiHelper.SetCellValue(sheet, currentRow, 5, "Характеристика категорий получателей бесплатных услуг");
                    break;

                case D_Services_Platnost.Paid:
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Потребители  платных услуг");
                    currentRow++;
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Категория получателей частично платных услуг");
                    NpoiHelper.SetCellValue(sheet, currentRow, 5, "Характеристика категорий получателей частично платных услуг");
                    break;

                case D_Services_Platnost.PartiallyPaid:
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Потребители  частично платных услуг");
                    currentRow++;
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, "Категория получателей частично платных услуг");
                    NpoiHelper.SetCellValue(sheet, currentRow, 5, "Характеристика категорий получателей платных услуг");
                    break;
            }

            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 4);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 5, LastCol);

            SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 4);
            SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, LastCol);

            currentRow++;

            foreach (var consumer in consumers)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 1, consumer.RefCPotr.Name).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 5, consumer.RefCPotr.Name).CellStyle = GetDefaultStyleForData(workBook);

                NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 4);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 5, LastCol);

                SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 4);
                SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, LastCol);

                currentRow++;
            }

            currentRow += 2;
        }

        public static void Tariffs(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Предельные цены (тарифы) на оплату государственной (муниципальной) услуги в случаях, если законодательством");
            currentRow++;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Российской Федерации предусмотрено их оказание на платной основе, либо порядок их установления: ");
            currentRow++;

            var limits = service.Limits.ToList();
            var prices = service.Prices.ToList();

            TariffsHeader(workBook, sheet, ref currentRow);

            foreach (var limit in limits)
            {
                var festRow = currentRow;

                NpoiHelper.SetCellValue(sheet, currentRow, 0, limit.Name).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 2, limit.Price).CellStyle = GetDefaultStyleForData(workBook);

                foreach (var price in prices)
                {
                    NpoiHelper.SetCellValue(sheet, currentRow, 3, price.VidNPAGZ).CellStyle = GetDefaultStyleForData(workBook);
                    NpoiHelper.SetCellValue(sheet, currentRow, 5, price.NumNPA).CellStyle = GetDefaultStyleForData(workBook);
                    NpoiHelper.SetCellValue(sheet, currentRow, 6, price.DataNPAGZ).CellStyle = GetDefaultStyleForData(workBook);
                    NpoiHelper.SetCellValue(sheet, currentRow, 7, price.Name).CellStyle = GetDefaultStyleForData(workBook);
                    currentRow++;
                }

                TariffsRowFormat(workBook, sheet, festRow, currentRow - 1);
            }

            currentRow += 2;
        }

        public static void ReasonsForEarlyTermination(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Основания для досрочного прекращения исполнения государственного (муниципального) задания:");
            currentRow++;

            var stateTasksTerminations = service.RefParametr.StateTasksTerminations.ToList();

            ReasonsForEarlyTerminationHeader(workBook, sheet, ref currentRow);

            var counter = 1;
            foreach (var termination in stateTasksTerminations)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, counter).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 1, termination.EarlyTerminat).CellStyle = GetDefaultStyleForData(workBook);
                ReasonsForEarlyTerminationRowFormat(workBook, sheet, currentRow);

                currentRow++;
                counter++;
            }

            currentRow += 2;
        }

        public static void OrderOfService(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Порядок оказания государственной (муниципальной) услуги");
            currentRow++;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "нормативные правовые акты, регулирующие порядок оказания государственной (муниципальной) услуги");
            currentRow++;

            var data = service.RenderOrders.ToList();

            OrderOfServiceHeader(workBook, sheet, ref currentRow);

            var counter = 1;
            foreach (var item in data)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, counter).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 1, item.TypeNpa).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 4, item.NumberNpa).CellStyle = GetDefaultStyleForData(workBook);
                if (item.DateNpa != null)
                {
                    NpoiHelper.SetCellValue(sheet, currentRow, 6, item.DateNpa.Value.ToShortDateString()).CellStyle = GetDefaultStyleForData(workBook);
                }

                NpoiHelper.SetCellValue(sheet, currentRow, 8, item.RenderEnact).CellStyle = GetDefaultStyleForData(workBook);

                OrderOfServiceRowFormat(workBook, sheet, currentRow);

                counter++;
                currentRow++;
            }

            currentRow++;
        }

        public static void ControlOrder(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Порядок контроля за исполнением государственного (муниципального) задания");
            currentRow++;

            var stateTasksTerminations = service.RefParametr.StateTasksSupervisionProcedures.ToList();

            ControlOrderHeader(workBook, sheet, ref currentRow);

            foreach (var termination in stateTasksTerminations)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, termination.Form).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 3, termination.Rate).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 6, termination.Supervisor).CellStyle = GetDefaultStyleForData(workBook);
                ControlOrderRowFormat(workBook, sheet, currentRow);

                currentRow++;
            }

            currentRow += 2;
        }

        public static void FormOfReport(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Требования к отчетности об исполнении государственного (муниципального) задания");
            currentRow += 2;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Форма отчета об исполнении государственного (муниципального) задания:");
            currentRow++;

            var indicators = service.Indicators.ToList();

            FormOfReportHeader(workBook, sheet, ref currentRow);

            foreach (var indicator in indicators)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, indicator.RefIndicators.Name).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 2, indicator.RefIndicators.RefOKEI.Name).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 3, indicator.ComingYear).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 5, indicator.ActualValue).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 7, indicator.Protklp).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 9, indicator.SourceInfFact).CellStyle = GetDefaultStyleForData(workBook);
                FormOfReportRowFormat(workBook, sheet, currentRow);

                currentRow++;
            }

            currentRow += 2;
        }

        public static void DeadlineSubmissionReports(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Сроки представления отчетов об исполнении государственного (муниципального) задания:");
            currentRow++;

            var requestAccounts = service.RefParametr.StateTasksRequestAccounts.ToList();

            DeadlineSubmissionReportsHeader(workBook, sheet, ref currentRow);

            foreach (var requestAccount in requestAccounts)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, requestAccount.ReportForm).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 3, requestAccount.DeliveryTerm).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 6, requestAccount.OtherRequest).CellStyle = GetDefaultStyleForData(workBook);
                DeadlineSubmissionReportsRowFormat(workBook, sheet, currentRow);

                currentRow++;
            }

            currentRow += 2;
        }

        public static void CharacteristicsWorksReports(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, F_F_GosZadanie service)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, " Характеристика работ");
            currentRow++;

            var indicators = service.Indicators.ToList();
            var year = service.RefParametr.RefYearForm.ID;

            CharacteristicsWorksHeader(workBook, sheet, ref currentRow, year);

            foreach (var indicator in indicators)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, indicator.RefIndicators.Name).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 2, indicator.Info).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 4, indicator.ReportingYear).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 6, indicator.CurrentYear).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 7, indicator.ComingYear).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 8, indicator.FirstPlanYear).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetCellValue(sheet, currentRow, 9, indicator.SecondPlanYear).CellStyle = GetDefaultStyleForData(workBook);
                CharacteristicsWorksRowFormat(workBook, sheet, currentRow);

                currentRow++;
            }

            currentRow += 2;
        }

        public static HSSFCellStyle GetDefaultStyle(HSSFWorkbook workBook)
        {
            var cellStyle = workBook.CreateCellStyle();
            var font = workBook.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 11;
            cellStyle.SetFont(font);
            return cellStyle;
        }

        public static void SetBorderTop(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow, int fistCell, int lastCell)
        {
            for (var i = fistCell; i <= lastCell; i++)
            {
                var cell = NpoiHelper.GetCellByXy(sheet, currentRow, i) ?? sheet.GetRow(currentRow).CreateCell(i);
                var cellStyle = workBook.CreateCellStyle();
                cellStyle.CloneStyleFrom(cell.CellStyle);
                cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;
            }
        }

        public static void SetBorderBoth(HSSFWorkbook workBook, HSSFSheet sheet, int firstRow, int fistCol, int lastRow, int lastCol)
        {
            for (var i = fistCol; i <= lastCol; i++)
            {
                var cell = NpoiHelper.GetCellByXy(sheet, firstRow, i);
                var cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;

                cell = NpoiHelper.GetCellByXy(sheet, lastRow, i);
                cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;
            }

            for (var i = firstRow; i <= lastRow; i++)
            {
                var cell = NpoiHelper.GetCellByXy(sheet, i, fistCol);
                var cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;

                cell = NpoiHelper.GetCellByXy(sheet, i, lastCol);
                cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;
            }
        }

        public static HSSFCellStyle GetDefaultStyleForData(HSSFWorkbook workBook)
        {
            if (defaultStyleForData == null)
            {
                var cellStyle = workBook.CreateCellStyle();
                var font = workBook.CreateFont();
                font.FontName = "Times New Roman";
                font.FontHeightInPoints = 11;
                cellStyle.SetFont(font);
                cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                cellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
                cellStyle.WrapText = true;
                defaultStyleForData = cellStyle;
            }

            return defaultStyleForData;
        }

        private static HSSFCellStyle GetDefaultStyleForHead(HSSFWorkbook workBook)
        {
            if (defaultStyleForHead == null)
            {
                var cellStyle = workBook.CreateCellStyle();
                var font = workBook.CreateFont();
                font.FontName = "Times New Roman";
                font.FontHeightInPoints = 11;
                cellStyle.SetFont(font);
                cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                cellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_BOTTOM;
                cellStyle.WrapText = true;
                defaultStyleForHead = cellStyle;
            }

            return defaultStyleForHead;
        }
        
        private static void IndicatorsHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, bool isQuality, int year)
        {
            var cellStyle = GetDefaultStyleForData(workBook);

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "№  п/п").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Наименование показателя").CellStyle = cellStyle;
            if (isQuality)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 2, "Единица измерения").CellStyle = cellStyle;
            }

            NpoiHelper.SetCellValue(sheet, currentRow, 3, isQuality ? "Формула расчета" : "Единица измерения").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "Значение показателей {0} государственной (муниципальной) услуги".FormatWith(isQuality ? "качества" : "объема"))
                                    .CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "Источник информации о значении показателя").CellStyle = cellStyle;

            currentRow++;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "{0} год".FormatWith(year - 2)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "{0} год".FormatWith(year - 1)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "{0} год".FormatWith(year)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "{0} год".FormatWith(year + 1)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 8, "{0} год".FormatWith(year + 2)).CellStyle = cellStyle;
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "2").CellStyle = cellStyle;
            if (isQuality)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 2, "3").CellStyle = cellStyle;
            }

            NpoiHelper.SetCellValue(sheet, currentRow, 3, isQuality ? "4" : "3").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, isQuality ? "5" : "4").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 5, isQuality ? "6" : "5").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 6, isQuality ? "7" : "6").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 7, isQuality ? "8" : "7").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 8, isQuality ? "9" : "8").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 9, isQuality ? "10" : "9").CellStyle = cellStyle;

            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 0, currentRow - 1, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 1, currentRow - 1, isQuality ? 1 : 2);
            if (isQuality)
            {
                NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 2, currentRow - 1, 2);
            }

            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 3, currentRow - 1, 3);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 2, 4, 8);
            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 9, currentRow - 1, 9);

            if (!isQuality)
            {
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 2);
            }

            SetBorderBoth(workBook, sheet, currentRow - 2, 0, currentRow - 1, 0);
            SetBorderBoth(workBook, sheet, currentRow - 2, 1, currentRow - 1, !isQuality ? 2 : 1);
            if (isQuality)
            {
                SetBorderBoth(workBook, sheet, currentRow - 2, 2, currentRow - 1, 2);
            }

            SetBorderBoth(workBook, sheet, currentRow - 2, 3, currentRow - 1, 3);
            SetBorderBoth(workBook, sheet, currentRow - 2, 4, currentRow - 2, 8);
            SetBorderBoth(workBook, sheet, currentRow - 2, 9, currentRow - 1, 9);
            SetBorderBoth(workBook, sheet, currentRow - 1, 4, currentRow - 1, 4);
            SetBorderBoth(workBook, sheet, currentRow - 1, 5, currentRow - 1, 5);
            SetBorderBoth(workBook, sheet, currentRow - 1, 6, currentRow - 1, 6);
            SetBorderBoth(workBook, sheet, currentRow - 1, 7, currentRow - 1, 7);
            SetBorderBoth(workBook, sheet, currentRow - 1, 8, currentRow - 1, 8);
            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 0);
            SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, !isQuality ? 2 : 1);
            if (isQuality)
            {
                SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 2);
            }

            SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 3);
            SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 4);
            SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 6);
            SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 7);
            SetBorderBoth(workBook, sheet, currentRow, 8, currentRow, 8);
            SetBorderBoth(workBook, sheet, currentRow, 9, currentRow, 9);

            currentRow++;
        }

        private static void IndicatorRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow, bool isQuality, F_F_PNRZnach item, int counter)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, counter).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 1, item.RefIndicators.Name).CellStyle = GetDefaultStyleForData(workBook);
            if (isQuality)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 2, item.RefIndicators.RefOKEI.Name).CellStyle = GetDefaultStyleForData(workBook);
            }

            NpoiHelper.SetCellValue(sheet, currentRow, 3, isQuality ? item.Info : item.RefIndicators.RefOKEI.Name).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, item.ReportingYear).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 5, item.CurrentYear).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, item.ComingYear).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, item.FirstPlanYear).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 8, item.SecondPlanYear).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, item.Source).CellStyle = GetDefaultStyleForData(workBook);

            if (!isQuality)
            {
                NpoiHelper.GetCellByXy(sheet, currentRow, 2).CellStyle = GetDefaultStyleForData(workBook);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 2);
                SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 2);
            }

            for (var i = 0; i <= LastCol; i++)
            {
                if (isQuality)
                {
                    NpoiHelper.GetCellByXy(sheet, currentRow, i).CellStyle = GetDefaultStyleForData(workBook);
                    SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
                }
                else
                {
                    if (i != 1 && i != 2)
                    {
                        NpoiHelper.GetCellByXy(sheet, currentRow, i).CellStyle = GetDefaultStyleForData(workBook);
                        SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
                    }
                }
            }
        }

        private static void OrderOfServiceRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 3);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 4, 5);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 6, 7);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 8, 9);

            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 0);
            SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 3);
            SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 7);
            SetBorderBoth(workBook, sheet, currentRow, 8, currentRow, 9);
        }

        private static void OrderOfServiceHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "№  п/п").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Реквизиты нормативного правового акта, устанавливающего порядок оказания услуги").CellStyle = GetDefaultStyleForData(workBook);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Вид НПА").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "Номер").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "Дата утверждения").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 8, "Наименование").CellStyle = GetDefaultStyleForData(workBook);

            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "2").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "3").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "4").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 8, "5").CellStyle = GetDefaultStyleForData(workBook);

            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 0, currentRow - 1, 0);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 2, FistCol, LastCol);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 1, 3);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 4, 5);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 6, 7);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 8, 9);

            SetBorderBoth(workBook, sheet, currentRow - 2, 0, currentRow - 1, 0);
            SetBorderBoth(workBook, sheet, currentRow - 2, FistCol, currentRow - 2, LastCol);
            SetBorderBoth(workBook, sheet, currentRow - 1, 1, currentRow - 1, 3);
            SetBorderBoth(workBook, sheet, currentRow - 1, 4, currentRow - 1, 5);
            SetBorderBoth(workBook, sheet, currentRow - 1, 6, currentRow - 1, 7);
            SetBorderBoth(workBook, sheet, currentRow - 1, 8, currentRow - 1, 9);

            OrderOfServiceRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }

        private static void TariffsRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int firstRow, int currentRow)
        {
            NpoiHelper.SetMergedRegion(sheet, firstRow, 0, currentRow, 1);
            NpoiHelper.SetMergedRegion(sheet, firstRow, 2, currentRow, 2);
            SetBorderBoth(workBook, sheet, firstRow, 0, currentRow, 1);
            SetBorderBoth(workBook, sheet, firstRow, 2, currentRow, 2);

            for (var i = firstRow; i <= currentRow; i++)
            {
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, i, 3, 4);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, i, 5, 5);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, i, 6, 6);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, i, 7, 8);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, i, 9, 9);

                SetBorderBoth(workBook, sheet, i, 3, i, 4);
                SetBorderBoth(workBook, sheet, i, 5, i, 5);
                SetBorderBoth(workBook, sheet, i, 6, i, 6);
                SetBorderBoth(workBook, sheet, i, 7, i, 8);
                SetBorderBoth(workBook, sheet, i, 9, i, 9);
            }
        }

        private static void TariffsHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Платный элемент").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Тариф (цена), руб.").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Реквизиты нормативного правового акта, устанавливающего порядок определения цен").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "Примечание").CellStyle = GetDefaultStyleForData(workBook);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Вид НПА").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "Номер").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "Дата утверждения").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "Наименование").CellStyle = GetDefaultStyleForData(workBook);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "2").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "3").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "4").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "5").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "6").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "7").CellStyle = GetDefaultStyleForData(workBook);

            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 0, currentRow - 1, 1);
            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 2, currentRow - 1, 2);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 2, 3, 8);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 3, 4);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 5, 5);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 6, 6);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 7, 8);
            NpoiHelper.SetMergedRegion(sheet, currentRow - 2, 9, currentRow - 1, 9);

            SetBorderBoth(workBook, sheet, currentRow - 2, 0, currentRow - 1, 1);
            SetBorderBoth(workBook, sheet, currentRow - 2, 2, currentRow - 1, 2);
            SetBorderBoth(workBook, sheet, currentRow - 2, 3, currentRow - 2, 8);
            SetBorderBoth(workBook, sheet, currentRow - 1, 3, currentRow - 1, 4);
            SetBorderBoth(workBook, sheet, currentRow - 1, 5, currentRow - 1, 5);
            SetBorderBoth(workBook, sheet, currentRow - 1, 6, currentRow - 1, 6);
            SetBorderBoth(workBook, sheet, currentRow - 1, 7, currentRow - 1, 8);
            SetBorderBoth(workBook, sheet, currentRow - 2, 9, currentRow - 1, 9);

            TariffsRowFormat(workBook, sheet, currentRow, currentRow);

            currentRow++;
        }

        private static void ReasonsForEarlyTerminationRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, FistCol, LastCol);
            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 0);
            SetBorderBoth(workBook, sheet, currentRow, FistCol, currentRow, LastCol);
        }

        private static void ReasonsForEarlyTerminationHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "№  п/п").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Наименование основания для прекращения").CellStyle = GetDefaultStyleForData(workBook);
            ReasonsForEarlyTerminationRowFormat(workBook, sheet, currentRow);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "2").CellStyle = GetDefaultStyleForData(workBook);
            ReasonsForEarlyTerminationRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }

        private static void ControlOrderRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 2);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 3, 5);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 6, LastCol);
            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 2);
            SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, LastCol);
        }

        private static void ControlOrderHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Форма контроля").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Периодичность").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "Органы исполнительной власти, осуществляющие контроль за оказанием государственной (муниципальной) услуги").CellStyle =
                GetDefaultStyleForData(workBook);
            ControlOrderRowFormat(workBook, sheet, currentRow);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "2").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "3").CellStyle = GetDefaultStyleForData(workBook);
            ControlOrderRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }

        private static void FormOfReportRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 1);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 3, 4);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 5, 6);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 7, 8);

            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 1);
            SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 2);
            SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 4);
            SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, 6);
            SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 8);
            SetBorderBoth(workBook, sheet, currentRow, LastCol, currentRow, LastCol);
        }

        private static void FormOfReportHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Наименование показателя").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Единица измерения").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Значение, утвержденное в государственном (муниципальном) задании на отчетный финансовый год").CellStyle =
                GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "Фактическое значение за отчетный финансовый год").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "Причины отклонения от запланированных значений").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "Источник информации о фактическом значении показателя").CellStyle = GetDefaultStyleForData(workBook);

            FormOfReportRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }

        private static void DeadlineSubmissionReportsRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 2);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 3, 5);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 6, LastCol);

            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 2);
            SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, LastCol);
        }

        private static void DeadlineSubmissionReportsHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Форма отчёта").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "Сроки представления отчёта").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "Иные требования").CellStyle = GetDefaultStyleForData(workBook);
            DeadlineSubmissionReportsRowFormat(workBook, sheet, currentRow);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "2").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "3").CellStyle = GetDefaultStyleForData(workBook);
            DeadlineSubmissionReportsRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }

        private static void CharacteristicsWorksRowFormat(HSSFWorkbook workBook, HSSFSheet sheet, int currentRow)
        {
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 1);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 2, 3);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 4, 5);

            SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 1);
            SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 3);
            SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 6);
            SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 7);
            SetBorderBoth(workBook, sheet, currentRow, 8, currentRow, 8);
            SetBorderBoth(workBook, sheet, currentRow, 9, currentRow, 9);
        }

        private static void CharacteristicsWorksHeader(HSSFWorkbook workBook, HSSFSheet sheet, ref int currentRow, int year)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Наименование работ").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Содержание работ").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "Планируемый результат выполнения работ").CellStyle = GetDefaultStyleForData(workBook);
            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 4, "{0} год".FormatWith(year - 2)).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "{0} год".FormatWith(year - 1)).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "{0} год".FormatWith(year)).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 8, "{0} год".FormatWith(year + 1)).CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "{0} год".FormatWith(year + 2)).CellStyle = GetDefaultStyleForData(workBook);

            NpoiHelper.SetMergedRegion(sheet, currentRow - 1, 0, currentRow, 1);
            NpoiHelper.SetMergedRegion(sheet, currentRow - 1, 2, currentRow, 3);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow - 1, 4, LastCol);
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 4, 5);

            SetBorderBoth(workBook, sheet, currentRow - 1, 0, currentRow, 1);
            SetBorderBoth(workBook, sheet, currentRow - 1, 2, currentRow, 3);
            SetBorderBoth(workBook, sheet, currentRow - 1, 4, currentRow - 1, LastCol);
            SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 5);
            SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 6);
            SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 7);
            SetBorderBoth(workBook, sheet, currentRow, 8, currentRow, 8);
            SetBorderBoth(workBook, sheet, currentRow, 9, currentRow, 9);

            currentRow++;

            NpoiHelper.SetCellValue(sheet, currentRow, 0, "1").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "2").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "3").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "4").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "5").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 8, "6").CellStyle = GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 9, "7").CellStyle = GetDefaultStyleForData(workBook);
            CharacteristicsWorksRowFormat(workBook, sheet, currentRow);

            currentRow++;
        }
    }
}