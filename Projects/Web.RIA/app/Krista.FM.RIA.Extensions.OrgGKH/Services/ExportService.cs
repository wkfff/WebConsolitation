using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers;
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Models;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.OrgGKH.Services
{
    public class ExportService : IExportService
    {
        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;
        private readonly IList<D_Org_MarksGKH> marksAll;
        private readonly ILinqRepository<F_Org_GKH> planRepository;
        private readonly ILinqRepository<FX_Org_StatusD> statusRepository;

        public ExportService(
            IRepository<D_Org_MarksGKH> marksGkhRepository,
            ILinqRepository<F_Org_GKH> planRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            ILinqRepository<FX_Org_StatusD> statusRepository)
        {
            this.planRepository = planRepository;
            this.orgRepository = orgRepository;
            this.statusRepository = statusRepository;
            marksAll = marksGkhRepository.GetAll()
                .OrderBy(x => ((x.Code % 100 != 0 ? 1 : 2) * 10) + (((x.Code / 100) % 100 != 0 ? 1 : 2) * 100))
                .ToList();
         }

        /// <summary>
        /// Экспорт данных в Excel еженедельной формы сбора
        ///  </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="terr">Наименование территории</param>
        public Stream ExportWeek(int orgId, int periodId, int sourceId, string terr)
        {
            var template = new MemoryStream(Resources.Resource.TemplateExportWeek);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            try
            {
                if (sourceId == -1)
                {
                    throw new Exception("Источник не найден");
                }

                var data = GetData(periodId, orgId, sourceId);

                // определяем стили шрифтов
                HSSFFont fontBold;
                HSSFFont font;
                GetFonts(wb, out fontBold, out font);

                var statusRow = 1;
                var regionRow = 2;
                var orgRow = 3;
                var periodRow = 4;
                var headerRow = 6;

                var org = orgRepository.FindOne(orgId);
                var status = statusRepository.FindOne(data.First().Status);
                
                NPOIHelper.GetCellByXY(sheet, statusRow, 0).SetCellValue(status.Name);
                if (terr != null && terr.IsNotEmpty())
                {
                    NPOIHelper.CopyRow(wb, sheet, regionRow, regionRow + 1);
                    NPOIHelper.GetCellByXY(sheet, regionRow, 0).SetCellValue("Территория: {0}".FormatWith(terr));
                    regionRow++;
                    orgRow++;
                    periodRow++;
                    headerRow++;
                }

                NPOIHelper.GetCellByXY(sheet, regionRow, 0)
                    .SetCellValue("Муниципальное образование: {0}".FormatWith(org.RefRegionAn.Name));
                NPOIHelper.GetCellByXY(sheet, orgRow, 0)
                    .SetCellValue("Организация: {0}".FormatWith(org.NameOrg));

                var year = periodId / 10000;
                var month = (periodId / 100) % 100;
                var day = periodId % 100;
                var date = new DateTime(year, month, day);
                var startDate = date.AddDays(-4);
                var periodText = "{0}-{1}".FormatWith(
                    startDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), 
                    date.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
                NPOIHelper.GetCellByXY(sheet, periodRow, 0).SetCellValue(periodText);

                // вывод шапки таблицы
                var headerCell = NPOIHelper.GetCellByXY(sheet, headerRow, 0);
                var curRow = headerCell.RowIndex + 2;
                foreach (var fact in data)
                {
                    NPOIHelper.CopyRow(wb, sheet, curRow, curRow + 1);

                    NPOIHelper.SetCellValue(sheet, curRow, 0, fact.Code);
                    NPOIHelper.SetCellValue(sheet, curRow, 1, fact.MarkName);
                    NPOIHelper.SetCellValue(sheet, curRow, 2, fact.YearPlan);
                    NPOIHelper.SetCellValue(sheet, curRow, 3, fact.PerformedOP);

                    curRow++;
                }

                NPOIHelper.DeleteRows(sheet, curRow, curRow);
            }
            catch (Exception)
            {
            }

            // Сохраняем книгу
            return WriteToStream(wb);
        }

        /// <summary>
        /// Экспорт данных в Excel ежемесячной формы сбора
        ///  </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="terr">Наименование территории</param>
        public Stream ExportMonth(int orgId, int periodId, int sourceId, string terr)
        {
            var template = new MemoryStream(Resources.Resource.TemplateExportMonth);
            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            try
            {
                if (sourceId == -1)
                {
                    throw new Exception("Источник не найден");
                }

                var data = GetMonthlyData(periodId, orgId, sourceId);
                
                // определяем стили шрифтов
                HSSFFont fontBold;
                HSSFFont font;
                GetFonts(wb, out fontBold, out font);

                var statusRow = 1;
                var regionRow = 2;
                var orgRow = 3;
                var periodRow = 4;
                var headerRow = 6;

                var org = orgRepository.FindOne(orgId);
                var status = statusRepository.FindOne(data.First().Status);
                NPOIHelper.GetCellByXY(sheet, statusRow, 0).SetCellValue(status.Name);
                if (terr != null && terr.IsNotEmpty())
                {
                    NPOIHelper.CopyRow(wb, sheet, regionRow, regionRow + 1);
                    NPOIHelper.GetCellByXY(sheet, regionRow, 0).SetCellValue("Территория: {0}".FormatWith(terr));
                    regionRow++;
                    orgRow++;
                    periodRow++;
                    headerRow++;
                }

                NPOIHelper.GetCellByXY(sheet, regionRow, 0)
                    .SetCellValue("Муниципальное образование: {0}".FormatWith(org.RefRegionAn.Name));
                NPOIHelper.GetCellByXY(sheet, orgRow, 0)
                    .SetCellValue("Организация: {0}".FormatWith(org.NameOrg));
                var periodName = "{0} {1} года".FormatWith(
                    PeriodsController.Months[((periodId / 100) % 100) - 1], 
                    periodId / 10000);
                NPOIHelper.GetCellByXY(sheet, periodRow, 0).SetCellValue(periodName);
                
                // вывод шапки таблицы
                var headerCell = NPOIHelper.GetCellByXY(sheet, headerRow, 0);
                var curRow = headerCell.RowIndex + 2;
                foreach (var fact in data)
                {
                    NPOIHelper.CopyRow(wb, sheet, curRow, curRow + 1);

                    NPOIHelper.SetCellValue(sheet, curRow, 0, fact.Code);
                    NPOIHelper.SetCellValue(sheet, curRow, 1, fact.MarkName);
                    NPOIHelper.SetCellValue(sheet, curRow, 2, fact.Value);
                    NPOIHelper.SetCellValue(sheet, curRow, 3, fact.PlanO);
                    NPOIHelper.SetCellValue(sheet, curRow, 4, fact.Assigned);
                    NPOIHelper.SetCellValue(sheet, curRow, 5, fact.PlanS);
                    NPOIHelper.SetCellValue(sheet, curRow, 6, fact.Performed);
                    NPOIHelper.SetCellValue(sheet, curRow, 7, fact.PlanOOP);
                    NPOIHelper.SetCellValue(sheet, curRow, 8, fact.AssignedOP);
                    NPOIHelper.SetCellValue(sheet, curRow, 9, fact.PlanSOP);
                    NPOIHelper.SetCellValue(sheet, curRow, 10, fact.PerformedOP);

                    curRow++;
                }

                NPOIHelper.DeleteRows(sheet, curRow, curRow);
            }
            catch (Exception)
            {
            }

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

        private IEnumerable<MonthlyDataModel> GetMonthlyData(int periodId, int orgId, int sourceId)
        {
            var list = new List<MonthlyDataModel>();
            if (sourceId == -1)
            {
                return list;
            }

            var data = new List<MonthlyDataModel>();
                var tempId = -2;

                var orgPlans = planRepository.FindAll()
                    .Where(x => x.RefRegistrOrg.ID == orgId && x.SourceID == sourceId)
                    .ToList();

                // статус значения по показателю
                var forStatusPlan = orgPlans.FirstOrDefault(x => 
                    x.RefYearDayUNV.ID == periodId && 
                    x.RefMarksGKH.Code != 50000 && 
                    x.RefMarksGKH.Code != 60000);
                var statusId = forStatusPlan == null ? OrgGKHConsts.StateEdited : forStatusPlan.RefStatusD.ID;
                
                foreach (var mark in marksAll)
                {
                    var fact = new MonthlyDataModel
                                   {
                                       MarkId = mark.ID,
                                       MarkName = mark.Name,
                                       PrPlanY = mark.PrPlanY,
                                       PrPlanO = mark.PrPlanO,
                                       PrAssigned = mark.PrAssigned,
                                       PrPerformed = mark.PrPerformed,
                                       Code = "{0}.{1}.{2}"
                                        .FormatWith(mark.Code / 10000, (mark.Code / 100) % 100, mark.Code % 100),
                                       PrPlanS = mark.PrPlanS,
                                       PrPlanOOP = mark.PrPlanOOP,
                                       PrAssignedOP = mark.PrAssignedOP,
                                       PrPerformedOP = mark.PrPerformedOP,
                                       PrPlanSOP = mark.PrPlanSOP
                                   };
                    var planData = orgPlans.FirstOrDefault(x => 
                        x.RefMarksGKH.ID == mark.ID && 
                        x.RefYearDayUNV.ID == periodId);

                    fact.Value = FactValueService.GetPlanY(periodId, mark, planData, marksAll, orgPlans);

                    fact.PlanO = FactValueService.GetPlanO(periodId, mark, planData, marksAll, orgPlans);
                    fact.Assigned = FactValueService.GetAssigned(periodId, mark, planData, marksAll, orgPlans);
                    fact.PlanS = FactValueService.GetPlanS(periodId, mark, planData, marksAll, orgPlans);
                    fact.Performed = FactValueService.GetPerformed(periodId, mark, planData, marksAll, orgPlans);

                    fact.PlanOOP = FactValueService.GetPlanOOP(mark, planData, periodId, marksAll, orgPlans);
                    fact.AssignedOP = FactValueService.GetAssignedOP(mark, planData, periodId, marksAll, orgPlans);
                    fact.PlanSOP = FactValueService.GetPlanSOP(periodId, mark, planData, marksAll, orgPlans);
                    fact.PerformedOP = FactValueService.GetPerformedOP(periodId, mark, planData, marksAll, orgPlans);
                    if (planData == null)
                    {
                        fact.ID = tempId;
                        tempId--;
                    }

                    fact.Status = statusId;

                    data.Add(fact);
                }

                return data.OrderBy(x => x.Code).ToList();
        }

        private IEnumerable<WeeklyDataModel> GetData(int periodId, int orgId, int sourceId)
        {
            var minPeriodValue = (periodId / 10000) * 10000;
            var maxPeriodValue = (periodId / 100) * 100;

            var data = new List<WeeklyDataModel>();
            var marks = marksAll.Where(x => x.PrPerformedOP != "X");

            var orgPlans = planRepository.FindAll()
                .Where(x => x.RefRegistrOrg.ID == orgId && x.SourceID == sourceId)
                .ToList();

            // статус значения по показателю
            var forStatusPlan = orgPlans.FirstOrDefault(x => x.RefYearDayUNV.ID == periodId);
            var statusId = forStatusPlan == null ? OrgGKHConsts.StateEdited : forStatusPlan.RefStatusD.ID;
            
            var yearPlansForAllMarks = orgPlans.Where(x =>
                                                      x.RefYearDayUNV.ID > minPeriodValue &&
                                                      x.RefYearDayUNV.ID < maxPeriodValue &&
                                                      x.RefYearDayUNV.ID % 100 == 0);

            foreach (var mark in marks)
            {
                var plan = new WeeklyDataModel
                               {
                                   MarkName = mark.Name,
                                   MarkId = mark.ID,
                                   Code =
                                       "{0}.{1}.{2}".FormatWith(mark.Code / 10000, (mark.Code / 100) % 100, mark.Code % 100),
                                   PrPerformedOP = mark.PrPerformedOP
                               };

                var planData = orgPlans.FirstOrDefault(x =>
                                                       x.RefYearDayUNV.ID == periodId &&
                                                       x.RefMarksGKH.ID == mark.ID);

                if (planData == null)
                {
                    planData = new F_Org_GKH
                                   {
                                       RefMarksGKH = mark,
                                       RefRegistrOrg = orgRepository.FindOne(orgId),
                                       SourceID = sourceId
                                   };
                }

                plan.PerformedOP = mark.PrPerformedOP.Equals("AS")
                                       ? FactValueService.GetASValue(marksAll, orgPlans, mark.Code, fact => fact.PerformedOP, periodId)
                                       : planData.PerformedOP;

                planData.PerformedOP = plan.PerformedOP;
                planRepository.Save(planData);
                plan.ID = planData.ID;
                
                var yearPlanFact =
                    yearPlansForAllMarks.Where(x => x.RefMarksGKH.ID == mark.ID).OrderBy(x => x.RefYearDayUNV.ID).
                        LastOrDefault();

                plan.YearPlan = yearPlanFact == null ? null : yearPlanFact.Value;
                
                plan.Status = statusId;

                data.Add(plan);
            }

            return data.OrderBy(x => x.Code).ToList();
        }
    }
}
