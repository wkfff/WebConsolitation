using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Consolidated
{
    public class ExportService : IExportService
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_CD_CollectTask> collectTaskRepository;
        private readonly ILinqRepository<T_Org_CPrice> factRepository;
        
        public ExportService(
                             ILinqRepository<D_CD_Task> taskRepository, 
                             ILinqRepository<T_Org_CPrice> factRepository, 
                             ILinqRepository<D_CD_CollectTask> collectTaskRepository)
        {
            this.taskRepository = taskRepository;
            this.factRepository = factRepository;
            this.collectTaskRepository = collectTaskRepository;
        }

        public Stream GetReportGas(int taskId)
        {
            Stream template = new MemoryStream(Resource.TemplateGasolineCons);

            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            var task = taskRepository.FindOne(taskId);

            NPOIHelper.SetCellValue(sheet, 3, 0, "по состоянию на {0} (еженедельно)".FormatWith(task.EndDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU"))));

            var currentCollectTask = task.RefCollectTask;
            var prevWeekCollectTask = GetPrevWeekCollectTask(currentCollectTask, GoodType.Gasoline);
            var prevMonthCollectTask = GetPrevMonthCollectTask(currentCollectTask, GoodType.Gasoline);
            var beginYearCollectTask = GetBeginYearCollectTask(currentCollectTask, GoodType.Gasoline);

            var currentFacts = GetFacts(currentCollectTask, GoodType.Gasoline);
            var prevWeekFacts = GetFacts(prevWeekCollectTask, GoodType.Gasoline);
            var prevMonthFacts = GetFacts(prevMonthCollectTask, GoodType.Gasoline);
            var beginYearFacts = GetFacts(beginYearCollectTask, GoodType.Gasoline);

            // Рисуем шапку таблицы на кол-во товаров
            IList<D_Org_Good> goods = GetGoods(task.RefCollectTask, GoodType.Gasoline);
            int currentColumnIndex = 8;
            for (int i = 0; i < (goods.Count - 1) * 7; i++)
            {
                NPOIHelper.CopyColumn(sheet, currentColumnIndex - 7, currentColumnIndex);    
                currentColumnIndex ++;
            }

            DateTime? currentDate = currentCollectTask != null ? currentCollectTask.EndPeriod : (DateTime?)null;
            DateTime? prevWeekDate = prevWeekCollectTask != null ? prevWeekCollectTask.EndPeriod : (DateTime?)null;
            DateTime? prevMonthDate = prevMonthCollectTask != null ? prevMonthCollectTask.EndPeriod : (DateTime?)null;
            DateTime? beginYearDate = beginYearCollectTask != null ? beginYearCollectTask.EndPeriod : (DateTime?)null;
            currentColumnIndex = 1;
            foreach (D_Org_Good good in goods)
            {
                NPOIHelper.SetCellValue(sheet, 6, currentColumnIndex + 3, good.Name);

                if (beginYearDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex, ((DateTime)beginYearDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (prevMonthDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 1, ((DateTime)prevMonthDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (prevWeekDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 2, ((DateTime)prevWeekDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (currentDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 3, ((DateTime)currentDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                currentColumnIndex += 7; 
            }
            
            var subjects = GetSubjects(task.RefCollectTask, GoodType.Gasoline);

            var font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 7;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            var cellStyle = wb.CreateCellStyle();
            cellStyle.BorderTop = 1;
            cellStyle.BorderLeft = 1;
            cellStyle.BorderRight = 1;
            cellStyle.BorderBottom = 1;
            cellStyle.SetFont(font);
            cellStyle.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
            cellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;

            int firstRowIndex = 9;
            int currentRowIndex = firstRowIndex;
            foreach (var subject in subjects)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, subject.Name);
                NPOIHelper.GetCellByXY(sheet, currentRowIndex, 0).CellStyle = cellStyle;
                
                currentColumnIndex = 1;
                for (int j = 0; j < goods.Count; j++)
                {
                    for (int i = 4; i <= 6; i++)
                    {
                        var formula = NPOIHelper.GetCellFormula(sheet, currentRowIndex, currentColumnIndex + i);
                        formula = NPOIHelper.ShiftFormulaRow(formula, currentRowIndex - firstRowIndex, row => row >= 0);
                        NPOIHelper.SetCellFormula(sheet, currentRowIndex, currentColumnIndex + i, formula);
                    }

                    currentColumnIndex += 7;
                }

                currentRowIndex++;

                var orgs = GetOrganizations(task.RefCollectTask, subject, GoodType.Gasoline);
                foreach (D_Org_RegistrOrg organization in orgs)
                {
                    NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                    NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, organization.NameOrg);
                    
                    currentColumnIndex = 1;
                    foreach (D_Org_Good good in goods)
                    {
                        var beginYearFact = beginYearFacts.FirstOrDefault(x => x.RefReport.RefTask.RefSubject == subject
                                                                            && x.RefRegistrOrg == organization
                                                                            && x.RefGood == good);
                        if (beginYearFact != null)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, beginYearFact.Price);
                        }

                        var prevMonthFact = prevMonthFacts.FirstOrDefault(x => x.RefReport.RefTask.RefSubject == subject
                                                                            && x.RefRegistrOrg == organization
                                                                            && x.RefGood == good);
                        if (prevMonthFact != null)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 1, prevMonthFact.Price);
                        }

                        var prevWeekFact = prevWeekFacts.FirstOrDefault(x => x.RefReport.RefTask.RefSubject == subject
                                                                          && x.RefRegistrOrg == organization
                                                                          && x.RefGood == good);
                        if (prevWeekFact != null)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 2, prevWeekFact.Price);
                        }

                        var currentFact = currentFacts.FirstOrDefault(x => x.RefReport.RefTask.RefSubject == subject
                                                                            && x.RefRegistrOrg == organization
                                                                            && x.RefGood == good);
                        if (currentFact != null)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 3, currentFact.Price);
                        }

                        for (int i = 4; i <= 6; i++)
                        {
                            var formula = NPOIHelper.GetCellFormula(sheet, currentRowIndex, currentColumnIndex + i);
                            formula = NPOIHelper.ShiftFormulaRow(formula, currentRowIndex - firstRowIndex, row => row >= 0);
                            NPOIHelper.SetCellFormula(sheet, currentRowIndex, currentColumnIndex + i, formula);    
                        }
                        
                        currentColumnIndex += 7;
                    }

                    currentRowIndex++;
                }
            }

            NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, "Средняя цена");
            NPOIHelper.GetCellByXY(sheet, currentRowIndex, 0).CellStyle = cellStyle;

            currentColumnIndex = 1;
            foreach (D_Org_Good good in goods)
            {
                var data = currentFacts.Where(x => x.RefGood == good).Select(x => x.Price).ToList();
                decimal? currentAvg = data.Any() ? data.Average() : (decimal?)null;

                data = prevWeekFacts.Where(x => x.RefGood == good).Select(x => x.Price).ToList();
                decimal? prevWeekAvg = data.Any() ? data.Average() : (decimal?)null;

                data = prevMonthFacts.Where(x => x.RefGood == good).Select(x => x.Price).ToList();
                decimal? prevMonthAvg = data.Any() ? data.Average() : (decimal?)null;

                data = beginYearFacts.Where(x => x.RefGood == good).Select(x => x.Price).ToList();
                decimal? beginYearAvg = data.Any() ? data.Average() : (decimal?)null;

                NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, beginYearAvg);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 1, prevMonthAvg);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 2, prevWeekAvg);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 3, currentAvg);

                for (int i = 4; i <= 6; i++)
                {
                    var formula = NPOIHelper.GetCellFormula(sheet, currentRowIndex - 1, currentColumnIndex + i);
                    formula = NPOIHelper.ShiftFormulaRow(formula, 1, row => row >= 0);
                    NPOIHelper.SetCellFormula(sheet, currentRowIndex, currentColumnIndex + i, formula);
                }

                currentColumnIndex += 7;
            }

            currentRowIndex++;
            
            return WriteToStream(wb);
        }

        public Stream GetReportFood(int taskId)
        {
            Stream template = new MemoryStream(Resource.TemplateFoodCons);

            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            var task = taskRepository.FindOne(taskId);

            var currentCollectTask = task.RefCollectTask;
            var prevWeekCollectTask = GetPrevWeekCollectTask(currentCollectTask, GoodType.Food);
            var prevMonthCollectTask = GetPrevMonthCollectTask(currentCollectTask, GoodType.Food);
            var beginYearCollectTask = GetBeginYearCollectTask(currentCollectTask, GoodType.Food);

            var currentFacts = GetFacts(currentCollectTask, GoodType.Food);
            var prevWeekFacts = GetFacts(prevWeekCollectTask, GoodType.Food);
            var prevMonthFacts = GetFacts(prevMonthCollectTask, GoodType.Food);
            var beginYearFacts = GetFacts(beginYearCollectTask, GoodType.Food);
            
            var str = String.Format(
                                    "за период с {0} по {1}",
                                    beginYearCollectTask != null ? beginYearCollectTask.EndPeriod.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")) : String.Empty,
                                    currentCollectTask.EndPeriod.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
            NPOIHelper.SetCellValue(sheet, 4, 0, str);
           
            // Рисуем шапку таблицы по количеству субьектов
            var subjects = GetSubjects(currentCollectTask, GoodType.Food);
            int currentColumnIndex = 9;
            for (int i = 0; i < subjects.Count * 7; i++)
            {
                NPOIHelper.CopyColumn(sheet, currentColumnIndex - 7, currentColumnIndex);
                currentColumnIndex++;
            }

            DateTime? currentDate = currentCollectTask != null ? currentCollectTask.EndPeriod : (DateTime?)null;
            DateTime? prevWeekDate = prevWeekCollectTask != null ? prevWeekCollectTask.EndPeriod : (DateTime?)null;
            DateTime? prevMonthDate = prevMonthCollectTask != null ? prevMonthCollectTask.EndPeriod : (DateTime?)null;
            DateTime? beginYearDate = beginYearCollectTask != null ? beginYearCollectTask.EndPeriod : (DateTime?)null;
            currentColumnIndex = 2;
            for (int i = -1; i < subjects.Count; i++)
            {
                if (i >= 0)
                {
                    NPOIHelper.SetCellValue(sheet, 6, currentColumnIndex + 3, subjects[i].Name);
                }

                if (beginYearDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex, ((DateTime)beginYearDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (prevMonthDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 1, ((DateTime)prevMonthDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (prevWeekDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 2, ((DateTime)prevWeekDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                if (currentDate != null)
                {
                    NPOIHelper.SetCellValue(sheet, 8, currentColumnIndex + 3, ((DateTime)currentDate).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));
                }

                currentColumnIndex += 7;
            }

            // Проходим по товарам (строки)
            IList<D_Org_Good> goods = GetGoods(task.RefCollectTask, GoodType.Food);
            int startRowIndex = 9;
            int currentRowIndex = startRowIndex;
            foreach (var good in goods)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, good.Name);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, good.RefUnits.Symbol);
                
                // Проходим по субьектам (столбцы группами)
                var beginYearPrices = new List<decimal>();
                var prevMonthPrices = new List<decimal>();
                var prevWeekPrices = new List<decimal>();
                var currentPrices = new List<decimal>();

                currentColumnIndex = 9;
                foreach (D_CD_Subjects subject in subjects)
                {
                    var beginYearOrgFacts = beginYearFacts.Where(x => x.RefReport.RefTask.RefSubject == subject
                                                                   && x.RefGood == good);
                    if (beginYearOrgFacts.Any())
                    {
                        var beginYearFactPrice = beginYearOrgFacts.Average(x => x.Price);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, beginYearFactPrice);
                        beginYearPrices.Add(beginYearFactPrice);
                    }

                    var prevMonthOrgFacts = prevMonthFacts.Where(x => x.RefReport.RefTask.RefSubject == subject
                                                                   && x.RefGood == good);
                    if (prevMonthOrgFacts.Any())
                    {
                        var prevMonthFactPrice = prevMonthOrgFacts.Average(x => x.Price);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 1, prevMonthFactPrice);
                        prevMonthPrices.Add(prevMonthFactPrice);
                    }

                    var prevWeekOrgFacts = prevWeekFacts.Where(x => x.RefReport.RefTask.RefSubject == subject
                                                                 && x.RefGood == good);
                    if (prevWeekOrgFacts.Any())
                    {
                        var prevWeekFactPrice = prevWeekOrgFacts.Average(x => x.Price);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 2, prevWeekFactPrice);
                        prevWeekPrices.Add(prevWeekFactPrice);
                    }

                    var currentOrgFact = currentFacts.Where(x => x.RefReport.RefTask.RefSubject == subject
                                                              && x.RefGood == good);
                    if (currentOrgFact.Any())
                    {
                        var currentFactPrice = currentOrgFact.Average(x => x.Price);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 3, currentFactPrice);
                        currentPrices.Add(currentFactPrice);
                    }

                    for (int i = 4; i <= 6; i++)
                    {
                        var formula = NPOIHelper.GetCellFormula(sheet, currentRowIndex, currentColumnIndex + i);
                        formula = NPOIHelper.ShiftFormulaRow(formula, 1, row => row >= 0);
                        NPOIHelper.SetCellFormula(sheet, currentRowIndex + 1, currentColumnIndex + i, formula);
                    }

                    currentColumnIndex += 7;
                }

                // Средние значения всех субьектов
                currentColumnIndex = 2;
                if (beginYearPrices.Any())
                {
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, beginYearPrices.Average());
                }

                if (prevMonthPrices.Any())
                {
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 1, prevMonthPrices.Average());
                }
                
                if (prevWeekPrices.Any())
                {
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 2, prevWeekPrices.Average());
                }

                if (currentPrices.Any())
                {
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex + 3, currentPrices.Average());
                }

                for (int i = 4; i <= 6; i++)
                {
                    var formula = NPOIHelper.GetCellFormula(sheet, currentRowIndex, currentColumnIndex + i);
                    formula = NPOIHelper.ShiftFormulaRow(formula, 1, row => row >= 0);
                    NPOIHelper.SetCellFormula(sheet, currentRowIndex + 1, currentColumnIndex + i, formula);
                }
                
                currentRowIndex++;
            }

            // Удаляем строку-шаблон
            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);
            
            // Итоговые формулы по субьектам
            var font = wb.CreateFont();
            font.FontName = "Times New Roman";
            font.FontHeightInPoints = 7;
            font.Boldweight = HSSFFont.BOLDWEIGHT_NORMAL;

            var cellStyle = wb.CreateCellStyle();
            cellStyle.BorderTop = 1;
            cellStyle.BorderLeft = 0;
            cellStyle.BorderRight = 0;
            cellStyle.BorderBottom = 0;
            cellStyle.SetFont(font);
            cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("# ##0.00%");

            for (int i = 0; i < subjects.Count; i++)
            {
                int columnInd = 9 + (i * 7) + 4;
                sheet.GetRow(currentRowIndex).CreateCell(columnInd);
                var cell = NPOIHelper.GetCellByXY(sheet, currentRowIndex, columnInd);
                cell.CellStyle = cellStyle;
                NPOIHelper.SetCellFormula(sheet, currentRowIndex, columnInd, NPOIHelper.ShiftFormulaColumn("max(A{0}:A{1})".FormatWith(startRowIndex + 1, currentRowIndex), columnInd, row => row >= 0));
            }
            
            currentRowIndex++;
            sheet.GetRow(currentRowIndex).CreateCell(0);
            NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, "Ф.И.О. исполнителя: _____________________(тел:_________________)");
            
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
        /// Перечень товаров, фигурирующих в отчете на данную дату
        /// </summary>
        private IList<D_Org_Good> GetGoods(D_CD_CollectTask colectTask, GoodType goodType)
        {
            var goods = factRepository.FindAll()
                                      .Where(x => x.RefReport.RefTask.RefCollectTask == colectTask
                                               && x.RefReport.RefTask.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName))
                                      .Select(x => x.RefGood)
                                      .Distinct()
                                      .ToList()
                                      .OrderBy(y => y.Code)
                                      .ToList();

            return goods;
        }

        /// <summary>
        /// Субьекты, на которые есть назначенные задачи
        /// </summary>
        private IList<D_CD_Subjects> GetSubjects(D_CD_CollectTask collectTask, GoodType goodType)
        {
            var subjects = factRepository.FindAll()
                                         .Where(x => x.RefReport.RefTask.RefCollectTask == collectTask
                                                  && x.RefReport.RefTask.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName))
                                         .Select(x => x.RefReport.RefTask.RefSubject)
                                         .Distinct()
                                         .ToList();
            return subjects;
        }

        /// <summary>
        /// Организации, которые участвуют в сборе, в разрезе данного субьекта 
        /// </summary>
        private IList<D_Org_RegistrOrg> GetOrganizations(D_CD_CollectTask collectTask, D_CD_Subjects subject, GoodType goodType)
        {
             var organizations = factRepository.FindAll()
                                     .Where(x => x.RefReport.RefTask.RefCollectTask == collectTask
                                              && x.RefReport.RefTask.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName)
                                              && x.RefReport.RefTask.RefSubject == subject)
                                     .Select(x => x.RefRegistrOrg)
                                     .Distinct()
                                     .ToList()
                                     .OrderBy(y => y.Code)
                                     .ToList();

             return organizations.ToList();
        }

        /// <summary>
        /// Получение всех данных всех отчетов по конкретной задаче сбора
        /// </summary>
        private IList<T_Org_CPrice> GetFacts(D_CD_CollectTask collectTask, GoodType goodType, bool approvedOnly = true)
        {
            var data = factRepository.FindAll()
                                     .Where(x => x.RefReport.RefTask.RefCollectTask == collectTask
                                                && x.RefReport.RefTask.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName));
            if (approvedOnly)
            {
                data = data.Where(x => x.RefReport.RefTask.RefStatus.ID == (int)TaskViewModel.TaskStatus.Accepted);
            }
            
            var result = data.ToList();
            return result;
        }

        /// <summary>
        /// Находит задачу сбора на предыдущую неделю
        /// </summary>
        private D_CD_CollectTask GetPrevWeekCollectTask(D_CD_CollectTask currentCollectTask, GoodType goodType)
        {
            var prevWeekCollectTask = collectTaskRepository.FindAll()
                                                           .Where(x => x.ID != currentCollectTask.ID
                                                                    && x.EndPeriod < currentCollectTask.EndPeriod
                                                                    && x.RefSubject == currentCollectTask.RefSubject
                                                                    && taskRepository.FindAll().Any(t => t.RefCollectTask == x
                                                                                                         && t.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName)))
                                                           .OrderByDescending(x => x.EndPeriod)
                                                           .FirstOrDefault();
            return prevWeekCollectTask;
        }

        /// <summary>
        /// Находит задачу сбора за предыдущий месяц
        /// </summary>
        private D_CD_CollectTask GetPrevMonthCollectTask(D_CD_CollectTask currentCollectTask, GoodType goodType)
        {
            var currentDate = currentCollectTask.EndPeriod;
            var prevMonthColectTask = collectTaskRepository.FindAll()
                                                           .Where(x => x.RefSubject == currentCollectTask.RefSubject
                                                                    && x.EndPeriod <= currentDate.AddMonths(-1)
                                                                    && x.EndPeriod > currentDate.AddMonths(-2)
                                                                    && taskRepository.FindAll().Any(t => t.RefCollectTask == x
                                                                                                         && t.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName)))
                                                           .OrderByDescending(x => x.EndPeriod)
                                                           .FirstOrDefault();
            return prevMonthColectTask;
        }

        /// <summary>
        /// Находит задачу сбора на начало года
        /// </summary>
        private D_CD_CollectTask GetBeginYearCollectTask(D_CD_CollectTask currentCollectTask, GoodType goodType)
        {
            var currentDate = currentCollectTask.EndPeriod;
            var beginYearColectTask = collectTaskRepository.FindAll()
                                                           .Where(x => x.RefSubject == currentCollectTask.RefSubject
                                                                    && x.EndPeriod.Year == currentDate.Year
                                                                    && taskRepository.FindAll().Any(t => t.RefCollectTask == x
                                                                                                         && t.RefTemplate.Class == (goodType == GoodType.Gasoline ? Gasoline.Form.FormClassName : Food.Form.FormClassName)))
                                                           .OrderBy(x => x.EndPeriod)
                                                           .FirstOrDefault();
            return beginYearColectTask;
        }
    }
}
