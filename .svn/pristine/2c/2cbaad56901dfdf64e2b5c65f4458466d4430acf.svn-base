using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class ExportService : IExportService
    {
        private readonly IExportRepository exportRepository;
        private readonly IFormRepository formRepository;
        private readonly IProtocolService protocolService;
        private readonly IJobTitleService jobTitleService;

        public ExportService(
            IExportRepository exportRepository, 
            IFormRepository formRepository,
            IJobTitleService jobTitleService,
            IProtocolService protocolService)
        {
            this.exportRepository = exportRepository;
            this.formRepository = formRepository;
            this.jobTitleService = jobTitleService;
            this.protocolService = protocolService;
        }

        /// <summary>
        /// Экспорт данных в Excel с незавершенным вводом данных.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        public Stream ExportUnactedRegions(int taskId)
        {
            var taskInfo = exportRepository.GetTaskInfo(taskId);
            var regionlist = exportRepository.GetIncompleteRegions(taskId);

            Stream template = new MemoryStream(Resource.TemplateExportUnactedRegions);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            if (taskInfo != null)
            {
                NPOIHelper.SetCellValue(sheet, 6, 2, GetPeriodText(taskInfo.EndDate));
                NPOIHelper.SetCellValue(sheet, 7, 0, GetCurrentDateText());

                var currentRowIndex = 9;
                var counter = 1;

                if (regionlist.Count > 2)
                {
                    for (var i = 0; i < regionlist.Count - 2; i++)
                    {
                        NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                    }
                }

                foreach (var region in regionlist)
                {
                    var protocolInfo = protocolService.GetRegionProtocol(region.ID);

                    NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, counter++);
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, region.RefRegion.CodeLine);
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, region.Name);

                    if (protocolInfo.Count > 0)
                    {
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, protocolInfo.First().Commentary);
                    }

                    currentRowIndex++;
                }
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel по форме сбора.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        public Stream ExportFormCollection(int taskId)
        {
            var taskInfo = exportRepository.GetTaskInfo(taskId);
            Stream template = new MemoryStream(Resource.TemplateExportFormCollection);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            if (taskInfo != null)
            {
                NPOIHelper.SetCellValue(sheet, 4, 3, taskInfo.RefSubject.Name);
                NPOIHelper.SetCellValue(sheet, 6, 3, GetPeriodText(taskInfo.EndDate));
                var list = formRepository.GetFormData(taskId);

                if (list != null)
                {
                    var currentRowIndex = 11;

                    foreach (var fact in list)
                    {
                        if (fact.Code == 30)
                        {
                            currentRowIndex++;
                        }

                        if (fact.OrgType1 != null)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, fact.OrgType1);
                        }

                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, fact.OrgType2);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, fact.OrgType3);

                        currentRowIndex++;
                    }

                    var jobInfo = jobTitleService.GetTaskExecuters(taskInfo.ID);

                    if (jobInfo != null)
                    {
                        NPOIHelper.SetCellValue(sheet, 28, 3, jobInfo.Name);
                        NPOIHelper.SetCellValue(sheet, 29, 3, jobInfo.Phone);
                    }

                    var protocolInfo = protocolService.GetTaskProtocol(taskInfo.ID);

                    if (protocolInfo != null && protocolInfo.Count > 0)
                    {
                        NPOIHelper.SetCellValue(sheet, 30, 3, protocolInfo.First().ChangeDate.ToShortDateString());
                    }

                    sheet.ForceFormulaRecalculation = true;
                }
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel отчета по МО.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        public Stream ExportMOReport(int taskId)
        {
            var taskInfo = exportRepository.GetTaskInfo(taskId);
            var dataList = exportRepository.GetSubjectTrihedrData(taskId);
            Stream template = new MemoryStream(Resource.TemplateExportSubjectTrihedrData);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            if (taskInfo != null)
            {
                NPOIHelper.SetCellValue(sheet, 4, 1, GetPeriodText(taskInfo.EndDate));
                NPOIHelper.SetCellValue(sheet, 5, 0, GetCurrentDateText());

                if (dataList != null)
                {
                    var currentRowIndex = 11;

                    var regions = from l in dataList
                                  group l by new { l.RegionId, l.RegionName, l.RegionCode, l.RegionTypeName }
                                  into g
                                  orderby g.Key.RegionCode
                                  select g;

                    var totalCount = dataList.Count() + regions.Count() + 2;

                    for (var i = 0; i < totalCount; i++)
                    {
                        NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
                    }

                    const int ColumnCount = 24;

                    var font = wb.CreateFont();
                    font.FontName = "Arial";
                    font.FontHeightInPoints = 8;
                    font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

                    var cellStyle = wb.CreateCellStyle();
                    cellStyle.BorderTop = 1;
                    cellStyle.BorderLeft = 1;
                    cellStyle.BorderRight = 1;
                    cellStyle.BorderBottom = 1;
                    cellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    cellStyle.SetFont(font);
                    cellStyle.GetFont(wb).Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

                    var cellSummaryStyle = wb.CreateCellStyle();
                    cellSummaryStyle.BorderTop = 1;
                    cellSummaryStyle.BorderLeft = 1;
                    cellSummaryStyle.BorderRight = 1;
                    cellSummaryStyle.BorderBottom = 1;
                    cellSummaryStyle.Alignment = HSSFCellStyle.ALIGN_RIGHT;
                    cellSummaryStyle.SetFont(font);
                    cellSummaryStyle.GetFont(wb).Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

                    foreach (var region in regions)
                    {
                        var regionId = region.Key.RegionId;
                        var regionData = from d in dataList where d.RegionId == regionId select d;
                        sheet.AddMergedRegion(new Region(currentRowIndex, 0, currentRowIndex, ColumnCount));
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 00, String.Format("{0} {1} {2}", region.Key.RegionCode, region.Key.RegionName, region.Key.RegionTypeName));
                        SetRowStyle(sheet, currentRowIndex, cellStyle, ColumnCount);
                        currentRowIndex++;

                        foreach (var fact in regionData)
                        {
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 00, String.Format("  {0}", fact.OrgName));

                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 01, fact.PrincipalCountOffBudget);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 03, fact.WorkerCountOffBudget);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 05, fact.PrincipalCountJoined);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 08, fact.WorkerCountJoined);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 11, fact.PrincipalCountMinSalary);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 14, fact.WorkerCountMinSalary);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 17, fact.PrincipalCountAvgSalary);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 20, fact.WorkerCountAvgSalary);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 23, fact.MinSalary);
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 24, fact.AvgSalary);

                            var calcRow = fact;

                            if (fact.OrgType == 1)
                            {
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 02, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 04, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 06, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 09, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 12, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 15, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 18, 100.0);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 21, 100.0);

                                var bigOrgData = from d in regionData where d.OrgType == 2 select d;
                                var avgOrgData = from d in regionData where d.OrgType == 3 select d;

                                var bigOrgRow = bigOrgData.FirstOrDefault() ?? new SubjectTrihedrDataModel();
                                var avgOrgRow = avgOrgData.FirstOrDefault() ?? new SubjectTrihedrDataModel();

                                calcRow = new SubjectTrihedrDataModel
                                              {
                                                  PrincipalCountOffBudget =
                                                      GetDecimal(bigOrgRow.PrincipalCountOffBudget) +
                                                      GetDecimal(avgOrgRow.PrincipalCountOffBudget),
                                                  WorkerCountOffBudget =
                                                      GetDecimal(bigOrgRow.WorkerCountOffBudget) +
                                                      GetDecimal(avgOrgRow.WorkerCountOffBudget),
                                                  PrincipalCountJoined =
                                                      GetDecimal(bigOrgRow.PrincipalCountJoined) +
                                                      GetDecimal(avgOrgRow.PrincipalCountJoined),
                                                  WorkerCountJoined =
                                                      GetDecimal(bigOrgRow.WorkerCountJoined) +
                                                      GetDecimal(avgOrgRow.WorkerCountJoined),
                                                  PrincipalCountMinSalary =
                                                      GetDecimal(bigOrgRow.PrincipalCountMinSalary) +
                                                      GetDecimal(avgOrgRow.PrincipalCountMinSalary),
                                                  WorkerCountMinSalary =
                                                      GetDecimal(bigOrgRow.WorkerCountMinSalary) +
                                                      GetDecimal(avgOrgRow.WorkerCountMinSalary),
                                                  PrincipalCountAvgSalary =
                                                      GetDecimal(bigOrgRow.PrincipalCountAvgSalary) +
                                                      GetDecimal(avgOrgRow.PrincipalCountAvgSalary),
                                                  WorkerCountAvgSalary =
                                                      GetDecimal(bigOrgRow.WorkerCountAvgSalary) +
                                                      GetDecimal(avgOrgRow.WorkerCountAvgSalary)
                                              };

                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 01, calcRow.PrincipalCountOffBudget);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 03, calcRow.WorkerCountOffBudget);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 05, calcRow.PrincipalCountJoined);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 08, calcRow.WorkerCountJoined);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 11, calcRow.PrincipalCountMinSalary);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 14, calcRow.WorkerCountMinSalary);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 17, calcRow.PrincipalCountAvgSalary);
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 20, calcRow.WorkerCountAvgSalary);
                            }
                            else
                            {
                                var curOrgType = 2;
                                var cmpOrgType = 3;

                                if (fact.OrgType == 3)
                                {
                                    curOrgType = 3;
                                    cmpOrgType = 2;
                                }

                                var curOrgData =
                                    (from d in regionData where d.OrgType == curOrgType select d).FirstOrDefault();
                                var cmpOrgData =
                                    (from d in regionData where d.OrgType == cmpOrgType select d).FirstOrDefault();

                                if (curOrgData == null)
                                {
                                    curOrgData = new SubjectTrihedrDataModel();
                                }

                                if (cmpOrgData == null)
                                {
                                    cmpOrgData = new SubjectTrihedrDataModel();
                                }

                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 02, GetPart(curOrgData.PrincipalCountOffBudget, cmpOrgData.PrincipalCountOffBudget));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 04, GetPart(curOrgData.WorkerCountOffBudget, cmpOrgData.WorkerCountOffBudget));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 06, GetPart(curOrgData.PrincipalCountJoined, cmpOrgData.PrincipalCountJoined));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 09, GetPart(curOrgData.WorkerCountJoined, cmpOrgData.WorkerCountJoined));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 12, GetPart(curOrgData.PrincipalCountMinSalary, cmpOrgData.PrincipalCountMinSalary));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 15, GetPart(curOrgData.WorkerCountMinSalary, cmpOrgData.WorkerCountMinSalary));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 18, GetPart(curOrgData.PrincipalCountAvgSalary, cmpOrgData.PrincipalCountAvgSalary));
                                NPOIHelper.SetCellValue(sheet, currentRowIndex, 21, GetPart(curOrgData.WorkerCountAvgSalary, cmpOrgData.WorkerCountAvgSalary));
                            }

                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 07, GetPercent(calcRow.PrincipalCountJoined, calcRow.PrincipalCountOffBudget));
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 10, GetPercent(calcRow.WorkerCountJoined, calcRow.WorkerCountOffBudget));
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 13, GetPercent(calcRow.PrincipalCountMinSalary, calcRow.PrincipalCountJoined));
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 16, GetPercent(calcRow.WorkerCountMinSalary, calcRow.WorkerCountJoined)); 
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 19, GetPercent(calcRow.PrincipalCountAvgSalary, calcRow.PrincipalCountJoined));
                            NPOIHelper.SetCellValue(sheet, currentRowIndex, 22, GetPercent(calcRow.WorkerCountAvgSalary, calcRow.WorkerCountJoined));

                            currentRowIndex++;
                        }
                    }

                    sheet.AddMergedRegion(new Region(currentRowIndex, 0, currentRowIndex, ColumnCount));
                    NPOIHelper.SetCellValue(sheet, currentRowIndex, 00, "Всего по АТЕ");
                    SetRowStyle(sheet, currentRowIndex, cellStyle, ColumnCount);

                    for (var i = 1; i < 4; i++)
                    {
                        var rowIndex = currentRowIndex + i;
                        var orgData = from o in dataList where o.OrgType == i select o;
                        SetRowStyle(sheet, rowIndex, cellSummaryStyle, ColumnCount);
                        SetCellStyle(sheet, rowIndex, 0, cellStyle);

                        if (orgData.Count() > 0)
                        {
                            NPOIHelper.SetCellValue(sheet, rowIndex, 00, orgData.FirstOrDefault().OrgName);
                            
                            if (i == 1)
                            {
                                orgData = from o in dataList where o.OrgType != i select o;
                            }

                            var summaryRow = GetTrihedrSum(orgData);

                            NPOIHelper.SetCellValue(sheet, rowIndex, 01, summaryRow.PrincipalCountOffBudget);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 03, summaryRow.WorkerCountOffBudget);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 05, summaryRow.PrincipalCountJoined);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 08, summaryRow.WorkerCountJoined);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 11, summaryRow.PrincipalCountMinSalary);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 14, summaryRow.WorkerCountMinSalary);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 17, summaryRow.PrincipalCountAvgSalary);
                            NPOIHelper.SetCellValue(sheet, rowIndex, 20, summaryRow.WorkerCountAvgSalary);

                            NPOIHelper.SetCellValue(sheet, rowIndex, 07, GetPercent(summaryRow.PrincipalCountJoined, summaryRow.PrincipalCountOffBudget));
                            NPOIHelper.SetCellValue(sheet, rowIndex, 10, GetPercent(summaryRow.WorkerCountJoined, summaryRow.WorkerCountOffBudget));
                            NPOIHelper.SetCellValue(sheet, rowIndex, 13, GetPercent(summaryRow.PrincipalCountMinSalary, summaryRow.PrincipalCountJoined));
                            NPOIHelper.SetCellValue(sheet, rowIndex, 16, GetPercent(summaryRow.WorkerCountMinSalary, summaryRow.WorkerCountJoined));
                            NPOIHelper.SetCellValue(sheet, rowIndex, 19, GetPercent(summaryRow.PrincipalCountAvgSalary, summaryRow.PrincipalCountJoined));
                            NPOIHelper.SetCellValue(sheet, rowIndex, 22, GetPercent(summaryRow.WorkerCountAvgSalary, summaryRow.WorkerCountJoined));

                            if (i == 1)
                            {
                                NPOIHelper.SetCellValue(sheet, rowIndex, 02, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 04, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 06, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 09, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 12, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 15, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 18, 100.0);
                                NPOIHelper.SetCellValue(sheet, rowIndex, 21, 100.0);
                            }
                            else
                            {
                                var curOrgType = i;
                                var cmpOrgType = 3 - i + 2;

                                var curOrgData = GetTrihedrSum(from o in dataList where o.OrgType == curOrgType select o);
                                var cmpOrgData = GetTrihedrSum(from o in dataList where o.OrgType == cmpOrgType select o);

                                NPOIHelper.SetCellValue(sheet, rowIndex, 02, GetPart(curOrgData.PrincipalCountOffBudget, cmpOrgData.PrincipalCountOffBudget));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 04, GetPart(curOrgData.WorkerCountOffBudget, cmpOrgData.WorkerCountOffBudget));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 06, GetPart(curOrgData.PrincipalCountJoined, cmpOrgData.PrincipalCountJoined));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 09, GetPart(curOrgData.WorkerCountJoined, cmpOrgData.WorkerCountJoined));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 12, GetPart(curOrgData.PrincipalCountMinSalary, cmpOrgData.PrincipalCountMinSalary));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 15, GetPart(curOrgData.WorkerCountMinSalary, cmpOrgData.WorkerCountMinSalary));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 18, GetPart(curOrgData.PrincipalCountAvgSalary, cmpOrgData.PrincipalCountAvgSalary));
                                NPOIHelper.SetCellValue(sheet, rowIndex, 21, GetPart(curOrgData.WorkerCountAvgSalary, cmpOrgData.WorkerCountAvgSalary));                                
                            }
                        }

                        for (var j = 1; j < 100; j++)
                        {
                            var cellSrc = NPOIHelper.GetCellByXY(sheet, 12, j);
                            var cellDst = NPOIHelper.GetCellByXY(sheet, rowIndex, j);

                            if (cellDst != null && cellSrc != null)
                            {
                                var formatStr = cellSrc.CellStyle.GetDataFormatString();
                                var format = HSSFDataFormat.GetBuiltinFormat(formatStr);

                                if (format < 0)
                                {
                                    format = wb.CreateDataFormat().GetFormat(formatStr);
                                }

                                var newStyle = wb.CreateCellStyle();
                                newStyle.CloneStyleFrom(cellSummaryStyle);
                                newStyle.DataFormat = format;
                                cellDst.CellStyle = newStyle;
                            }
                        }
                    }
                }
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        public SubjectTrihedrDataModel GetTrihedrSum(IEnumerable<SubjectTrihedrDataModel> orgData)
        {
            var summaryRow = new SubjectTrihedrDataModel()
            {
                PrincipalCountOffBudget = 0,
                WorkerCountOffBudget = 0,
                PrincipalCountJoined = 0,
                WorkerCountJoined = 0,
                PrincipalCountMinSalary = 0,
                WorkerCountMinSalary = 0,
                PrincipalCountAvgSalary = 0,
                WorkerCountAvgSalary = 0,
            };

            foreach (var regionData in orgData)
            {
                summaryRow.PrincipalCountOffBudget += GetDecimal(regionData.PrincipalCountOffBudget);
                summaryRow.WorkerCountOffBudget += GetDecimal(regionData.WorkerCountOffBudget);
                summaryRow.PrincipalCountJoined += GetDecimal(regionData.PrincipalCountJoined);
                summaryRow.WorkerCountJoined += GetDecimal(regionData.WorkerCountJoined);
                summaryRow.PrincipalCountMinSalary += GetDecimal(regionData.PrincipalCountMinSalary);
                summaryRow.WorkerCountMinSalary += GetDecimal(regionData.WorkerCountMinSalary);
                summaryRow.PrincipalCountAvgSalary += GetDecimal(regionData.PrincipalCountAvgSalary);
                summaryRow.WorkerCountAvgSalary += GetDecimal(regionData.WorkerCountAvgSalary);
            }

            return summaryRow;
        }

        public decimal GetDecimal(object value)
        {
            return value != null ? Convert.ToDecimal(value) : 0;
        }

        public decimal? GetPart(decimal? value1, decimal? value2)
        {
            if (value1 != null)
            {
                if (value2 != null)
                {
                    return value1 + value2 > 0 ? 100 * value1 / (value1 + value2) : null;
                }
                
                return 100;
            }

            return null;
        }

        public decimal? GetPercent(decimal? value1, decimal? value2)
        {
            if (value1 != null && value2 != null)
            {
                return value2 != 0 ? 100 * value1 / value2 : null;
            }

            return null;
        }

        /// <summary>
        /// Экспорт данных в Excel отчета по исполнителям.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        public Stream ExportExecuters(int taskId)
        {
            var taskInfo = exportRepository.GetTaskInfo(taskId);
            var regionList = exportRepository.GetCompleteRegions(taskId);
            var taskList = exportRepository.GetChildTasks(taskId);

            Stream template = new MemoryStream(Resource.TemplateExportExecuters);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            NPOIHelper.SetCellValue(sheet, 5, 0, GetPeriodText(taskInfo.EndDate));
            NPOIHelper.SetCellValue(sheet, 6, 0, GetCurrentDateText());

            var currentRowIndex = 8;
            var counter = 1;

            if (regionList.Count < 1)
            {
                NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex + 1);
            }

            for (var i = 0; i < regionList.Count - 2; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);
            }

            foreach (var region in regionList)
            {
                var jobInfo = jobTitleService.GetRegionExecuters(region.ID);

                NPOIHelper.SetCellValue(sheet, currentRowIndex, 0, counter++);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 1, region.RefRegion.CodeLine);
                NPOIHelper.SetCellValue(sheet, currentRowIndex, 2, region.Name);

                if (jobInfo.Count > 0)
                {
                    var jobRow = jobInfo.Where(f => taskList.Contains(f.RefReport.RefTask)).FirstOrDefault();

                    if (jobRow != null)
                    {
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 3, jobRow.Name);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 4, jobRow.Office);
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, 5, jobRow.Phone);
                    }
                }

                currentRowIndex++;
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        public string GetDocumentName(int taskId, string reportName, bool subjectReport)
        {
            var taskInfo = exportRepository.GetTaskInfo(taskId);
            var documentNameTemplate = "{0}_Sogl_№_{1}";

            // отчет по субъекту - название района не присоединять
            if (!subjectReport)
            {
                documentNameTemplate += "_{2}";
            }

            return String.Format(
                documentNameTemplate,
                reportName,
                GetPeriodText(taskInfo.EndDate),
                taskInfo.RefSubject.RefRegion.Name);
        }

        private static string GetCurrentDateText()
        {
            return String.Format(" Дата выборки - {0}", DateTime.Now.ToShortDateString());
        }

        private static string GetPeriodText(DateTime dateValue)
        {
            return String.Format("{0} квартал {1} года", dateValue.Month / 3, dateValue.Year);
        }

        private static void SetRowStyle(HSSFSheet sheet, int rowIndex, HSSFCellStyle cellStyle, int columnCount)
        {
            var row = sheet.GetRow(rowIndex);

            for (var k = 0; k < columnCount; k++)
            {
                var cell = row.GetCell(k);
                cell.CellStyle = cellStyle;
            }
        }

        private static void SetCellStyle(HSSFSheet sheet, int rowIndex, int cellIndex, HSSFCellStyle cellStyle)
        {
            var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, cellIndex);

            if (cell != null)
            {
                cell.CellStyle = cellStyle;
            }
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
    }
}
