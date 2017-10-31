using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class ExportService : IExportService
    {
        private readonly IAppPrivilegeService requestsRepository;
        private readonly IFactsService factsRepository;
        private readonly ILinqRepository<T_Doc_ApplicationOrg> docsOrgRepository;
        private readonly ILinqRepository<T_Doc_ApplicationOGV> docsOGVRepository;
        private readonly IAppFromOGVService requestsFormOGVRepository;
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;
        private readonly ILinqRepository<F_Marks_Privilege> factsEstimateRepository;
        private readonly ICategoryTaxpayerService categoryRepository;
        private HSSFCellStyle style0;
        private HSSFCellStyle style1;
        private HSSFCellStyle style2;

        public ExportService(
            IAppPrivilegeService requestsRepository,
            IAppFromOGVService requestsFormOGVRepository,
            IFactsService factsRepository,
            ILinqRepository<T_Doc_ApplicationOrg> docsOrgRepository,
            ILinqRepository<T_Doc_ApplicationOGV> docsOGVRepository,
            IRepository<D_Marks_NormPrivilege> normRepository,
            ILinqRepository<F_Marks_Privilege> factsEstimateRepository,
            ICategoryTaxpayerService categoryRepository)
        {
            this.requestsRepository = requestsRepository;
            this.requestsFormOGVRepository = requestsFormOGVRepository;
            this.factsRepository = factsRepository;
            this.docsOrgRepository = docsOrgRepository;
            this.docsOGVRepository = docsOGVRepository;
            this.normRepository = normRepository;
            this.factsEstimateRepository = factsEstimateRepository;
            this.categoryRepository = categoryRepository;
        }

        public static string GetTextForPeriod(int periodId)
        {
            var period = periodId % 10000;
            var year = periodId / 10000;

            if (period == 1)
            {
                return "{0} год".FormatWith(year);
            }

            return period / 10 == 999
                       ? "{0} квартал {1} года".FormatWith(period % 10, year)
                       : "{0}.{1} {2} года".FormatWith(period % 100, period / 100, year);
        }

        /// <summary>
        /// Экспорт данных в Excel заявки от налогоплательщика.
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        public Stream ExportForTaxpayer(int applicationId)
        {
            var application = requestsRepository.Get(applicationId);
            var facts = factsRepository.GetFactsForOrganization(applicationId);
            
            var template = new MemoryStream(Resources.Resource.TemplateExportToTaxpayer);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            // определяем стили для вывода чисел
            // целое число
            style0 = wb.CreateCellStyle();
            style0.CloneStyleFrom(sheet.GetRow(13).GetCell(2).CellStyle);

            // один знак после запятой
            style1 = wb.CreateCellStyle();
            style1.CloneStyleFrom(sheet.GetRow(13).GetCell(3).CellStyle);

            // два знака после запятой
            style2 = wb.CreateCellStyle();
            style2.CloneStyleFrom(sheet.GetRow(13).GetCell(4).CellStyle);

            // вывод даты
            NPOIHelper.SetCellValue(sheet, 0, 5, application.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));

            // вывод номера заявки
            NPOIHelper.SetCellValue(sheet, 2, 5, application.ID.ToString());

            // вывод налогоплательщика 
            var title = "По налогоплательщику:";
            var text = "{0} {1}".FormatWith(title, application.RefOrgPrivilege.Name);
            NPOIHelper.SetCellValue(sheet, 6, 0, text);
            sheet.GetRow(6).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(6).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод района
            title = "Район:";
            text = "{0} {1}".FormatWith(title, application.RefOrgPrivilege.RefBridgeRegions.Name);
            NPOIHelper.SetCellValue(sheet, 7, 0, text);
            sheet.GetRow(7).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(7).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод категории
            title = "Категория:";
            text = "{0} {1}".FormatWith(title, application.RefOrgCategory.Name);
            sheet.GetRow(8).Height = (short)(sheet.GetRow(0).Height * ((text.Length / 90) + 1));

            NPOIHelper.SetCellValue(sheet, 8, 0, text);
            sheet.GetRow(8).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(8).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

           // вывод ответственных лиц
            title = "Ответственные лица:";
            text = "{0} {1}".FormatWith(title, application.Executor);
            NPOIHelper.SetCellValue(sheet, 9, 0, text);
            sheet.GetRow(9).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(9).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод шапки таблицы
            var year = application.RefYearDayUNV.ID / 10000;
            NPOIHelper.SetCellValue(sheet, 12, 2, "{0} (предшествующий факт)".FormatWith(year - 2));
            NPOIHelper.SetCellValue(sheet, 12, 3, "{0} (факт)".FormatWith(year - 1));
            NPOIHelper.SetCellValue(sheet, 12, 4, "{0} (оценка)".FormatWith(year));
            NPOIHelper.SetCellValue(sheet, 12, 5, "{0} (прогноз)".FormatWith(year + 1));

            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencyDecimalDigits = 2;
            format.CurrencyDecimalSeparator = " ";

            // вывод показателей
            var currentRowIndex = 13;
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefMarks.NumberString);
                var name = fact.RefMarks.Name.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, name);

                var style = GetStyle(fact.RefMarks.RefOKEI.Code);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.PreviousFact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Estimate, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.Forecast, style);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // вывод документов
            currentRowIndex += 4;
            WriteDocs(applicationId, currentRowIndex, wb, sheet);
            
            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel заявки от налогоплательщика. (ХМАО)
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        public Stream ExportForTaxpayerHMAO(int applicationId)
        {
            var application = requestsRepository.Get(applicationId);
            var facts = factsRepository.GetFactsForOrganizationHMAO(applicationId, application.RefTypeTax.ID);

            var template = new MemoryStream(Resources.Resource.TemplateExportToTaxpayerHMAO);
            
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            // вывод даты
            NPOIHelper.GetCellByName(wb, "Date").SetCellValue(application.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));

            // вывод номера заявки
            NPOIHelper.GetCellByName(wb, "Number").SetCellValue(application.ID.ToString());

            SetHeader(wb, application.RefYearDayUNV.ID, application.RefTypeTax.ID);

            // вывод налогоплательщика 
            SetCellValue(
                "Полное наименование организации:", 
                application.RefOrgPrivilege.Name, 
                "Org",
                wb,
                fontBold, 
                font);

            SetCellValue(
                "Основной вид деятельности (льготная категория):",
                application.RefOrgCategory.ShortName,
                "Category",
                wb,
                fontBold,
                font);

            SetCellValue(
                "ИНН налогоплательщика:",
                application.RefOrgPrivilege.Code.ToString(),
                "INN",
                wb,
                fontBold,
                font);

            SetCellValue(
                "КПП налогоплательщика:",
                application.RefOrgPrivilege.INN20,
                "KPP",
                wb,
                fontBold,
                font);

            SetCellValue(
                "Юридический адрес:",
                application.RefOrgPrivilege.LegalAddress,
                "LegalAddress",
                wb,
                fontBold,
                font);

            SetCellValue(
                "Местонахождение организации:",
                application.RefOrgPrivilege.Address,
                "Address",
                wb,
                fontBold,
                font);

            SetCellValue(
                "Количество обособленных подразделений на территории автономного округа:",
                application.RefOrgPrivilege.Unit.ToString(),
                "NUnits",
                wb,
                fontBold,
                font);

            SetCellValue(
                "Код ОКАТО муниципального образования в соответствии с Общероссийским классификатором объектов административно-территориального деления, на территории которого мобилизуются денежные средства от уплаты налога (сбора) в бюджетную систему Российской Федерации:",
                application.RefOrgPrivilege.OKATO.ToString(),
                "OKATO",
                wb,
                fontBold,
                font);

            SetCellValue(
                "Ответственное лицо - исполнитель (ФИО, контактный телефон):",
                application.Executor,
                "Executor",
                wb,
                fontBold,
                font);
            
            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencyDecimalDigits = 2;
            format.CurrencyDecimalSeparator = " ";

            var headerCell = NPOIHelper.GetCellByName(wb, "HeaderMark");

            var currentRowIndex = headerCell.RowIndex + 1;

            // определяем стили для вывода чисел
            GetStylesForNumbers(wb, sheet, currentRowIndex);
            
            // вывод шапки
            NPOIHelper.SetCellValue(sheet, currentRowIndex - 1, 2, GetTextForPeriod(application.RefYearDayUNV.ID));
            NPOIHelper.SetCellValue(sheet, currentRowIndex - 1, 3, GetTextForPeriod(application.RefYearDayUNV.ID - 10000));

            // вывод показателей
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefMarks.NumberString);
                var name = fact.RefMarks.Name.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, name);

                var style = GetStyle(fact.RefMarks.RefOKEI.Code);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.PreviousFact, style);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // вывод документов
            sheet = wb.GetSheetAt(1);
            currentRowIndex = NPOIHelper.GetCellByName(wb, "DocsTable").RowIndex + 1;
            WriteDocs(applicationId, currentRowIndex, wb, sheet);
            
            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel заявки от ОГВ.
        /// </summary>
        /// <param name="appFromOGVId">Идентификатор заявки от ОГВ</param>
        public Stream ExportForOGV(int appFromOGVId)
        {
            var template = new MemoryStream(Resources.Resource.TemplateExportToOGV);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            var fontBold = wb.CreateFont();
            fontBold.FontName = "Times New Roman";
            fontBold.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            fontBold.Underline = HSSFFont.U_SINGLE;
            fontBold.FontHeightInPoints = 12;

            var font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 12;

            // определяем стили для вывода чисел
            // целое число
            style0 = wb.CreateCellStyle();
            style0.CloneStyleFrom(sheet.GetRow(12).GetCell(2).CellStyle);

            // один знак после запятой
            style1 = wb.CreateCellStyle();
            style1.CloneStyleFrom(sheet.GetRow(12).GetCell(3).CellStyle);

            // два знака после запятой
            style2 = wb.CreateCellStyle();
            style2.CloneStyleFrom(sheet.GetRow(12).GetCell(4).CellStyle);
            
            var appFromOGV = requestsFormOGVRepository.FindOne(appFromOGVId);

            // вывод даты
            if (appFromOGV.RequestDate != null)
            {
                NPOIHelper.SetCellValue(
                    sheet, 
                    0, 
                    5,
                    ((DateTime)appFromOGV.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
            }

            // вывод номера заявки
            NPOIHelper.SetCellValue(sheet, 2, 5, appFromOGV.ID.ToString());

            // вывод налогоплательщика 
            var title = "Ответственный ОГВ:";
            var text = "{0} {1}".FormatWith(title, appFromOGV.RefOGV.Name);
            NPOIHelper.SetCellValue(sheet, 6, 0, text);
            sheet.GetRow(6).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(6).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод категории
            title = "Категория:";
            text = "{0} {1}".FormatWith(title, appFromOGV.RefOrgCategory.Name);
            NPOIHelper.SetCellValue(sheet, 7, 0, text);
            sheet.GetRow(7).Height = (short)(sheet.GetRow(0).Height * ((text.Length / 90) + 1));
            sheet.GetRow(7).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(7).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод ответственных лиц
            title = "Ответственные лица:";
            text = "{0} {1}".FormatWith(title, appFromOGV.Executor);
            NPOIHelper.SetCellValue(sheet, 8, 0, text);
            sheet.GetRow(8).GetCell(0).RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            sheet.GetRow(8).GetCell(0).RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);

            // вывод шапки таблицы
            var year = appFromOGV.RefYearDayUNV.ID / 10000;
            NPOIHelper.SetCellValue(sheet, 11, 2, "{0} (предшествующий факт)".FormatWith(year - 2));
            NPOIHelper.SetCellValue(sheet, 11, 3, "{0} (факт)".FormatWith(year - 1));
            NPOIHelper.SetCellValue(sheet, 11, 4, "{0} (оценка)".FormatWith(year));
            NPOIHelper.SetCellValue(sheet, 11, 5, "{0} (прогноз)".FormatWith(year + 1));

            // получили список значений показателей по включенным заявкам налогоплательщиков
            // отсортированный по показателям
            var data = factsRepository.GetFactsForOGV(appFromOGVId);

            var indicators = new List<IndicatorsModel>();

            // код показателя
            var curIndicatorFacts = new IndicatorsModel
                                        {
                                            Fact = 0,
                                            PreviousFact = 0,
                                            Estimate = 0,
                                            Forecast = 0,
                                            RefMarks = data.First().RefMarks.ID
                                        };
            foreach (var f in data)
            {
                // если перешли к фактам по другому показателю, сохраняем накопленные факты, создаем новые
                if (f.RefMarks.ID != curIndicatorFacts.RefMarks)
                {
                    indicators.Add(curIndicatorFacts);
                    curIndicatorFacts = new IndicatorsModel
                                            {
                                                Fact = 0,
                                                PreviousFact = 0,
                                                Estimate = 0,
                                                Forecast = 0
                                            };
                }

                curIndicatorFacts.Fact += f.Fact ?? 0;
                curIndicatorFacts.OKEI = f.RefMarks.RefOKEI.Code;
                curIndicatorFacts.PreviousFact += f.PreviousFact ?? 0;
                curIndicatorFacts.Estimate += f.Estimate ?? 0;
                curIndicatorFacts.Forecast += f.Forecast ?? 0;
                curIndicatorFacts.RefName = f.RefMarks.Name;
                curIndicatorFacts.RefMarks = f.RefMarks.ID;
                curIndicatorFacts.RefNumberString = f.RefMarks.NumberString;
            }

            indicators.Add(curIndicatorFacts);

            // вывод показателей
            var currentRowIndex = 12;
            foreach (var fact in indicators)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefNumberString);
                var name = fact.RefName.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, name);

                var style = GetStyle(fact.OKEI);
                
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.PreviousFact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Estimate, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.Forecast, style);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            var docs = docsOGVRepository.FindAll().Where(x => x.RefApplicOGV.ID == appFromOGVId);
            var reqAll = requestsRepository.FindAll().ToList();
            var requests = reqAll.Where(x => 
                x.RefOrgCategory.ID == appFromOGV.RefOrgCategory.ID && 
                x.RefYearDayUNV.ID == appFromOGV.RefYearDayUNV.ID && 
                x.RefStateOrg.ID > 1);
            var estRequests = requests.Count(x => 
                x.RefApplicOGV != null && 
                x.RefApplicOGV.ID == appFromOGV.RefOGV.ID);

            NPOIHelper.SetCellValue(
                sheet, 
                currentRowIndex + 1, 
                0, 
                "Общее количество заявок от налогоплательщиков в данной категории: {0}".FormatWith(requests.Count()));
            NPOIHelper.SetCellValue(
                sheet, 
                currentRowIndex + 2, 
                0, 
                "Количество заявок от налогоплательщиков в данной категории, отправленных на оценку: {0}".FormatWith(estRequests));

            // вывод документов
            currentRowIndex += 7;
            foreach (var doc in docs)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, doc.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, doc.Note);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, doc.DateDoc.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, doc.Executor);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);
            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // вывод заявок от налогоплательщиков
            currentRowIndex += 5;
            WriteReqList(wb, sheet, currentRowIndex, reqAll, requests);

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        public Stream ExportResultCategoryHMAO(int taxTypeId, int categoryId, string categoryName, int periodId)
        {
            var facts = factsRepository.GetFactsForOgvHMAO(categoryId, taxTypeId, periodId);
            var requests = requestsRepository.GetAppForOGVList(categoryId, periodId);
            var template = new MemoryStream(Resources.Resource.TemplateExportResultCategotyHMAO);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            SetHeader(wb, periodId, taxTypeId);
            SetCellValue("по категории:", categoryName, "Category", wb, fontBold, font);

            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencyDecimalDigits = 2;
            format.CurrencyDecimalSeparator = " ";

            var headerCell = NPOIHelper.GetCellByName(wb, "HeaderMark");
            var currentRowIndex = headerCell.RowIndex + 1;

            // определяем стили для вывода чисел
            GetStylesForNumbers(wb, sheet, currentRowIndex);

            // вывод шапки
            NPOIHelper.SetCellValue(sheet, currentRowIndex - 1, 2, GetTextForPeriod(periodId));
            NPOIHelper.SetCellValue(sheet, currentRowIndex - 1, 3, GetTextForPeriod(periodId - 10000));

            // вывод показателей
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefMarks.NumberString);
                var name = fact.RefMarks.Name.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, name);
                var style = GetStyle(fact.RefMarks.RefOKEI.Code);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.PreviousFact, style);
                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // вывод документов
            sheet = wb.GetSheetAt(1);
            currentRowIndex = NPOIHelper.GetCellByName(wb, "ReqListTable").RowIndex + 1;
            WriteReqList(wb, sheet, currentRowIndex, requests);

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями
        /// </summary>
        /// <param name="appFromOGVId">Идентификатор заявки от ОГВ</param>
        /// <param name="sourceId">Идентификатор источника данных</param>
        public Stream ExportResult(int appFromOGVId, int sourceId)
        {
            var template = new MemoryStream(Resources.Resource.TemplateExportResultData);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            // определяем стили для вывода чисел
            // целое число
            style0 = wb.CreateCellStyle();
            style0.CloneStyleFrom(sheet.GetRow(6).GetCell(2).CellStyle);

            // один знак после запятой
            style1 = wb.CreateCellStyle();
            style1.CloneStyleFrom(sheet.GetRow(6).GetCell(3).CellStyle);

            // два знака после запятой
            style2 = wb.CreateCellStyle();
            style2.CloneStyleFrom(sheet.GetRow(6).GetCell(4).CellStyle);

            var appFromOGV = requestsFormOGVRepository.FindOne(appFromOGVId);
            var normMin = normRepository.GetAll().Where(x => x.Year == appFromOGV.RefYearDayUNV.ID / 10000);
            var facts = factsEstimateRepository.FindAll().Where(x =>
                                                               x.RefCategory.ID == appFromOGV.RefOrgCategory.ID &&
                                                               x.RefYearDayUNV.ID == appFromOGV.RefYearDayUNV.ID &&
                                                               x.SourceID == sourceId &&
                                                               x.RefMarks.RefTypeMark.Code == 4).ToList();

            // вывод категории
            var text = "по категории: {0}".FormatWith(appFromOGV.RefOrgCategory.Name);
            sheet.GetRow(1).Height = (short)(sheet.GetRow(1).Height * ((text.Length / 90) + 1));
            NPOIHelper.SetCellValue(sheet, 1, 0, text);

            // вывод корректирующего коэффициента
            NPOIHelper.SetCellValue(sheet, 3, 1, appFromOGV.RefOrgCategory.CorrectIndex);

            // вывод шапки таблицы
            var year = appFromOGV.RefYearDayUNV.ID / 10000;
            NPOIHelper.SetCellValue(sheet, 5, 2, year - 2);
            NPOIHelper.SetCellValue(sheet, 5, 3, year - 1);
            NPOIHelper.SetCellValue(sheet, 5, 4, year);
            NPOIHelper.SetCellValue(sheet, 5, 5, year + 1);

            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencyDecimalDigits = 2;
            format.CurrencyDecimalSeparator = " ";

            // вывод показателей
            var currentRowIndex = 6;

            if (normMin != null && normMin.Count() > 0)
            {
                var norm = normMin.First();

                // уровень прожиточного минимума
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, norm.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, "руб.");
                var style = GetStyle(2);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, norm.PreviousFact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, norm.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, norm.Estimate, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, norm.Forecast, style);
                currentRowIndex++;
            }

            // вывод показателей
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                var name = fact.RefMarks.Name.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, name);

                switch (fact.RefMarks.RefOKEI.ID)
                {
                    case -1:
                        fact.RefMarks.RefOKEI.Designation = string.Empty;
                        break;
                    case 58:
                        fact.RefMarks.RefOKEI.Designation = "тыс. руб.";
                        break;
                }

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, fact.RefMarks.RefOKEI.Designation);

                var style = GetStyle(fact.RefMarks.RefOKEI.Code);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.PreviousFact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.Fact, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.Estimate, style);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.Forecast, style);

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями по типу налога и периоду
        /// </summary>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        /// <param name="periodId">Идентификатор периода</param>
        public Stream ExportResultTaxType(int taxTypeId, int periodId)
        {
            var categories = categoryRepository.GetByTax(taxTypeId);
            var facts = factsRepository.GetResultFormByTax(taxTypeId, periodId);
            var template = new MemoryStream(Resources.Resource.TemplateExportResultTaxHMAO);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            SetHeader(wb, periodId, taxTypeId);
            var headerCell = NPOIHelper.GetCellByName(wb, "HeaderMark");
            var currentRowIndex = headerCell.RowIndex;

            // определяем стили для вывода чисел
            GetStylesForNumbers(wb, sheet, currentRowIndex);

            // вывод шапки
            var colIndex = 2;
            var styleCategoryName = wb.CreateCellStyle();
            styleCategoryName.CloneStyleFrom(sheet.GetRow(currentRowIndex).GetCell(1).CellStyle);

            var colWidth = sheet.GetColumnWidth(2);
            var borderSize = headerCell.CellStyle.BorderTop;
            foreach (var category in categories)
            {
                var curCell = NPOIHelper.GetCellByXY(sheet, currentRowIndex, colIndex - 1);
                curCell.CellStyle.WrapText = true;
                curCell.CellStyle.BorderBottom = borderSize;
                curCell.CellStyle.BorderTop = borderSize;
                curCell.CellStyle.BorderLeft = borderSize;
                curCell.CellStyle.BorderRight = borderSize;
                curCell.CellStyle.SetFont(styleCategoryName.GetFont(wb));
                sheet.SetColumnWidth(colIndex, colWidth);
                NPOIHelper.SetCellValue(
                    sheet, 
                    currentRowIndex, 
                    colIndex, 
                    "{0} ({1})".FormatWith(category.ShortName, category.Name),
                    styleCategoryName);

                colIndex++;
            }

            currentRowIndex++;

            // вывод показателей
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                object value;
                fact.TryGetValue("RefNumberString", out value);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, value == null ? string.Empty : value.ToString());

                fact.TryGetValue("RefMarks", out value);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, value == null ? string.Empty : value.ToString());

                colIndex = 2;
                foreach (var category in categories)
                {
                    object okei;
                    fact.TryGetValue("OKEI", out okei);
                    var style = GetStyle(Convert.ToInt32(okei));
                    fact.TryGetValue("Fact{0}".FormatWith(category.ID), out value);
                    NPOIHelper.SetCellValue(
                        sheet, 
                        currentRowIndex, 
                        colIndex, 
                        value == null ? "0" : value.ToString(), 
                        style);
                    var curCell = NPOIHelper.GetCellByXY(sheet, currentRowIndex, colIndex);
                    curCell.CellStyle.BorderBottom = borderSize;
                    curCell.CellStyle.BorderTop = borderSize;
                    curCell.CellStyle.BorderLeft = borderSize;
                    curCell.CellStyle.BorderRight = borderSize;

                    colIndex++;
                }

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb); 
        }

        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями ОЦЕНКИ по определенной категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="categoryName">Наименование категории</param>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        /// <param name="periodId">Идентификатор периода</param>
        public Stream ExportEstimateCategoryHMAO(int categoryId, string categoryName, int taxTypeId, int periodId)
        {
            var facts = factsRepository.GetEstimateDataHMAO(periodId, categoryId, taxTypeId);
            var template = new MemoryStream(Resources.Resource.TemplateExportEstimateCategoryHMAO);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            // определяем стили шрифтов
            HSSFFont fontBold;
            HSSFFont font;
            GetFonts(wb, out fontBold, out font);

            SetHeader(wb, periodId, taxTypeId);
            SetCellValue("по категории:", categoryName, "Category", wb, fontBold, font);

            var headerCell = NPOIHelper.GetCellByName(wb, "HeaderFact");
            headerCell.SetCellValue(headerCell.StringCellValue.FormatWith(periodId / 10000));

            var currentRowIndex = headerCell.RowIndex + 1;

            // определяем стили для вывода чисел
            GetStylesForNumbers(wb, sheet, currentRowIndex);

            // вывод шапки
            NPOIHelper.SetCellValue(sheet, currentRowIndex - 1, 2, GetTextForPeriod(periodId));

            // вывод показателей
            foreach (var fact in facts)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, fact.RefNumberString);
                var name = fact.RefName.Replace("\r", string.Empty);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, name);
                var style = GetStyle(fact.OKEI);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, fact.Fact ?? 0, style);
                var curCell = NPOIHelper.GetCellByXY(sheet, currentRowIndex, 2);
                curCell.CellStyle.BorderTop = 1;
                curCell.CellStyle.BorderBottom = 1;
                curCell.CellStyle.BorderLeft = 1;
                curCell.CellStyle.BorderRight = 1;
                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            // Сохраняем книгу
            return WriteToStream(wb);
        }
        
        private static MemoryStream WriteToStream(HSSFWorkbook wb)
        {
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;

            var resultStream = new MemoryStream();
            wb.Write(resultStream);

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }

        /// <summary>
        /// Вывод списка заявок, входящих в заявку от ОГВ (Для Ярославля)
        /// </summary>
        /// <param name="wb">Книга Excel</param>
        /// <param name="sheet">Лист Excel</param>
        /// <param name="currentRowIndex">строка, с которой начинать вывод</param>
        /// <param name="reqAll">Список всех заявок</param>
        /// <param name="requests">Список заявок от налогопл, соответствующих заявке от ОГВ</param>
        private static void WriteReqList(
            HSSFWorkbook wb,
            HSSFSheet sheet,
            int currentRowIndex,
            IEnumerable<D_Application_Privilege> reqAll,
            IEnumerable<D_Application_Privilege> requests)
        {
            foreach (var request in requests)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, request.ID);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, request.RefOrgPrivilege.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, request.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));

                // считаем количество копий для заявки
                var copiesCnt =
                    reqAll.Count(
                        f =>
                        f.RowType == request.RowType &&
                        f.RefOrgPrivilege.ID == request.RefOrgPrivilege.ID &&
                        f.RefYearDayUNV.ID == request.RefYearDayUNV.ID) - 1;

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, copiesCnt);
                NPOIHelper.SetCellValue(
                    sheet,
                    currentRowIndex,
                    5,
                    (request.RefApplicOGV == null || request.RefApplicOGV.ID == -1) ? "-" : "+");

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);
        }

        /// <summary>
        /// Вывод списка заявок по категории в выбранном периоде (ХМАО)
        /// </summary>
        /// <param name="wb">Книга Excel</param>
        /// <param name="sheet">Лист Excel</param>
        /// <param name="currentRowIndex">номер строки, с которой начинать вывод</param>
        /// <param name="requests">Список заявок</param>
        private static void WriteReqList(
            HSSFWorkbook wb,
            HSSFSheet sheet,
            int currentRowIndex,
            IEnumerable<D_Application_Privilege> requests)
        {
            foreach (var request in requests)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, request.ID);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, request.RefOrgPrivilege.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, request.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, request.RefStateOrg.Name);
                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);
        }
        
        private static void GetFonts(HSSFWorkbook wb, out HSSFFont fontBold, out HSSFFont font)
        {
            fontBold = wb.CreateFont();
            fontBold.FontName = "Times New Roman";
            fontBold.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            fontBold.Underline = HSSFFont.U_SINGLE;
            fontBold.FontHeightInPoints = 12;

            font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 12;
        }

        private static void SetHeader(HSSFWorkbook wb, int periodId, int taxTypeId)
        {
            var header = NPOIHelper.GetCellByName(wb, "Header");
            
            var tax = taxTypeId == 4
                          ? "налогу на прибыль организаций"
                          : (taxTypeId == 9
                                 ? "налогу на имущество организаций"
                                 : "транспортному налогу");

            header.SetCellValue(header.StringCellValue.FormatWith(tax, GetTextForPeriod(periodId)));
        }

        private static void SetCellValue(
            string title,
            string value,
            string cellName,
            HSSFWorkbook wb,
            HSSFFont fontBold,
            HSSFFont font)
        {
            var text = "{0} {1}".FormatWith(title, value);
            var orgCell = NPOIHelper.GetCellByName(wb, cellName);
            orgCell.SetCellValue(text);
            orgCell.RichStringCellValue.ApplyFont(0, title.Length, fontBold);
            orgCell.RichStringCellValue.ApplyFont(title.Length + 1, text.Length, font);
        }

        private void GetStylesForNumbers(HSSFWorkbook wb, HSSFSheet sheet, int currentRowIndex)
        {
            // целое число
            style0 = wb.CreateCellStyle();
            style0.CloneStyleFrom(sheet.GetRow(currentRowIndex).GetCell(1).CellStyle);

            // один знак после запятой
            style1 = wb.CreateCellStyle();
            style1.CloneStyleFrom(sheet.GetRow(currentRowIndex).GetCell(2).CellStyle);

            // два знака после запятой
            style2 = wb.CreateCellStyle();
            style2.CloneStyleFrom(sheet.GetRow(currentRowIndex).GetCell(3).CellStyle);
        }

        private HSSFCellStyle GetStyle(int okei)
        {
            if (okei == 383 || okei == 384 || okei == 744)
            {
                return style1;
            }
            
            if (okei == 0)
            {
                return style2;
            }

            if (okei == 792)
            {
                return style0;
            }

            return style0;
        }

        private void WriteDocs(int applicationId, int currentRowIndex, HSSFWorkbook wb, HSSFSheet sheet)
        {
            var docs = docsOrgRepository.FindAll().Where(x => x.RefApplicOrg.ID == applicationId).ToList();

            foreach (var doc in docs)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, doc.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, doc.Note);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, doc.DateDoc.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, doc.Executor);
                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);
        }
    }
}
