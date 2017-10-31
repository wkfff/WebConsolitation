using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public sealed class ReportsService : NewRestService, IReportService
    {
        private readonly IAuthService auth;

        /// <summary>
        /// Код региона из справочника ППО 
        /// </summary>
        private readonly string regionCodePPO;

        public ReportsService()
        {
            auth = Resolver.Get<IAuthService>();
            var scheme = Resolver.Get<IScheme>();
            regionCodePPO = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Replace(" ", string.Empty) + "000";
        }

        public HSSFWorkbook InitializeWorkBookWithoutTemplate(List<string> headers, List<ReportsHelper.Column> columns, List<DocumentsRegisterViewModel> data)
        {
            var workBook = new HSSFWorkbook();
            workBook.CreateSheet("Реестр документов");
            var currentRow = 0;
            var headersCellStyle = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;
            foreach (var header in headers)
            {
                workBook.GetSheetAt(0).CreateRow(currentRow).CreateCell(0);
                workBook.GetSheetAt(0).GetRow(currentRow).Height = 500;
                workBook.GetSheetAt(0).GetRow(currentRow).GetCell(0).CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                workBook.GetSheetAt(0).GetRow(currentRow).GetCell(0).CellStyle = headersCellStyle;
                workBook.GetSheetAt(0).GetRow(currentRow).GetCell(0).SetCellValue(header);
                workBook.GetSheetAt(0).AddMergedRegion(
                    new Region
                        {
                            RowFrom = currentRow, 
                            RowTo = currentRow, 
                            ColumnFrom = 0, 
                            ColumnTo = columns.Count - 1
                        });
                currentRow++;
            }

            if (columns.Count != 0)
            {
                workBook.GetSheetAt(0).CreateRow(currentRow);
                var i = 0;
                var row = currentRow;
                columns.Each(
                    x =>
                    {
                        workBook.GetSheetAt(0).GetRow(row).CreateCell(i);
                        workBook.GetSheetAt(0).GetRow(row).GetCell(i).CellStyle = new ReportsHelper.HSSFCellStyleForColumns(workBook).CellStyle;
                        workBook.GetSheetAt(0).GetRow(row).Height = 500;
                        workBook.GetSheetAt(0).GetRow(row).GetCell(i).SetCellValue(x.ID);
                        workBook.GetSheetAt(0).SetColumnWidth(i, x.Width * 30);
                        i++;
                    });
                currentRow++;
            }

            var styleForData = new ReportsHelper.HSSFCellStyleForData(workBook).CellStyle;
            data.ForEach(
                x =>
                {
                    workBook.GetSheetAt(0).CreateRow(currentRow);
                    var currentColumn = 0;
                    columns.ForEach(
                        y =>
                        {
                            workBook.GetSheetAt(0).GetRow(currentRow).CreateCell(currentColumn);
                            workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).CellStyle = styleForData;

                            switch (y.ID)
                            {
                                case "Название":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructureName);
                                    break;
                                case "ГРБС":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructureGrbs);
                                    break;
                                case "ППО":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructurePpo);
                                    break;
                                case "Документ":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.Type);
                                    break;
                                case "Состояние":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.State);
                                    break;
                                case "Примечание":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.Note);
                                    break;
                                case "Год формирования":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.Year);
                                    break;
                                case "ИНН":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructureInn);
                                    break;
                                case "КПП":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructureKpp);
                                    break;
                                case "Тип учреждения":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(x.StructureType);
                                    break;
                                case "Дата закрытия учреждения":
                                    workBook.GetSheetAt(0).GetRow(currentRow).GetCell(currentColumn).SetCellValue(
                                        x.StructureCloseDate.HasValue ? x.StructureCloseDate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
                                    break;
                            }

                            currentColumn++;
                        });
                    currentRow++;
                });

            return workBook;
        }

        public HSSFWorkbook GetDocReport(FX_FX_PartDoc partDoc, DateTime reportDate, bool isPPO, string tmpFileName, string docYear)
        {
            return new DocReport(partDoc, reportDate, isPPO, docYear).GetDocReport(tmpFileName);
        }

        public HSSFWorkbook GetStateTaskForm(int docId)
        {
            return new StateTaskForm(docId).GetStateTaskForm();
        }

        public HSSFWorkbook GetStateTaskForm2016(int docId)
        {
            return new StateTaskForm2016(docId).GetStateTaskForm();
        }

        public HSSFWorkbook GetAnalReport(DateTime reportDate)
        {
            var workBook = new HSSFWorkbook();
            HSSFSheet sheet = workBook.CreateSheet("Отчет");

            // учреждения у которых не найден паспорт или отсутствует нужная информация в паспорте
            var badInstitutions = new List<D_Org_Structure>();

            // отбираем все учреждения на дату отчета
            var institutions = GetItems<D_Org_Structure>().Where(x => (x.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID)
                                                                        || x.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID)
                                                                        || x.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID))
                                                                    && (x.CloseDate == null || x.CloseDate > reportDate)).ToList();

            // отбираем последние паспорта на дату отчета для отобранных ранее учреждений
            var passports = new List<F_Org_Passport>();

            foreach (var structure in institutions)
            {
                var passportsList = structure.Documents.Where(x => x.RefPartDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID)).ToList();
                if (passportsList.Any())
                {
                    var docId = passportsList.Max(p => p.ID);
                    var passport = GetItems<F_Org_Passport>().First(pas => pas.RefParametr.ID.Equals(docId));
                    if (passport.RefVid != null)
                    {
                        passports.Add(passport);        
                    }
                    else
                    {
                        // если не указан вид учреждения
                        badInstitutions.Add(structure);
                    }
                }
                else
                {
                    badInstitutions.Add(structure);
                }
            }

            var analReportData = new AnalReportData(this, sheet, workBook, reportDate);

            // заполняем ГУ
            analReportData.FillGu(passports.Where(x => x.RefParametr.RefUchr.RefOrgPPO.Code.Equals(regionCodePPO)));

            // заполняем МУ
            analReportData.FillMu(passports.Where(x => !x.RefParametr.RefUchr.RefOrgPPO.Code.Equals(regionCodePPO)));

            analReportData.FillBadInstitutions(badInstitutions);

            // заполняем формулы, устанавливаем стили и рисуем шапку
            analReportData.FinishReport();

            return workBook;
        }

        public HSSFWorkbook GetNewAnalReport(DateTime reportDate, bool isPPO)
        {
            var workBook = new HSSFWorkbook();
            HSSFSheet sheet = isPPO ? workBook.CreateSheet("по ППО") : workBook.CreateSheet("по ГРБС");

            var profile = !auth.IsAdmin() && !auth.IsSpectator() ? auth.Profile : null;

            var cachedFounder = GetItems<D_Org_Structure>()
                .Join(
                    GetItems<D_Org_OrgYchr>(),
                    structure => new
                    {
                        inn = structure.INN,
                        kpp = structure.KPP,
                    },
                    ychr => new
                    {
                        ychr.RefNsiOgs.inn,
                        ychr.RefNsiOgs.kpp,
                    },
                    (structure, ychr) => structure.ID)
                .ToList();

            var cachedFounderByName = GetItems<D_Org_Structure>()
                .Join(
                    GetItems<D_Org_OrgYchr>(),
                    structure => structure.Name,
                    ychr => ychr.Name,
                    (structure, ychr) => structure.ID)
                .ToList();

            var institutionsFounders = cachedFounder.Union(cachedFounderByName).ToList();

            var typesInstitutionList = new List<int>
                                        {
                                            FX_Org_TipYch.BudgetaryID,
                                            FX_Org_TipYch.AutonomousID,
                                            FX_Org_TipYch.GovernmentID
                                        };
            
            List<ReportItem> reportItems;
            List<D_Org_Structure> institutions;

            if (!isPPO)
            {
                /*    ГРБС    */

                // id главного ППО
                var regionId = GetItems<D_Org_PPO>().Single(ppo => ppo.Code.Equals(regionCodePPO)).ID;

                var id = (auth.IsAdmin() || auth.IsSpectator() || profile == null) ? regionId : profile.RefUchr.RefOrgPPO.ID;

                reportItems = ((auth.IsAdmin() || auth.IsSpectator() || profile == null)
                                   ? GetItems<D_Org_GRBS>().ToList()
                                   : GetItems<D_Org_GRBS>().ToList().Where(
                                       x => auth.IsPpoUser()
                                                ? x.RefOrgPPO.ID == profile.RefUchr.RefOrgPPO.ID
                                                : x.ID == profile.RefUchr.RefOrgGRBS.ID))
                    .Where(x => x.RefOrgPPO.ID == id).Select(
                        x => new ReportItem
                        {
                            ID = x.ID,
                            Name = x.Name
                        }).ToList();

                var grbsIds = reportItems.Select(x => x.ID).ToList();

                institutions = GetItems<D_Org_Structure>().Where(
                    x => grbsIds.Contains(x.RefOrgGRBS.ID)
                         && (x.CloseDate == null || x.CloseDate > reportDate)
                         && typesInstitutionList.Contains(x.RefTipYc.ID)
                         && !institutionsFounders.Contains(x.ID)).ToList();
            }
            else
            {
                /*      ППО      */
                var orgFilter = new List<int>();
                /* Если отчет строится для всей области, то необходимо обьединять элеметы в Мун.Округа, 
                 * иначе этого делать не надо, но данные отчета подвергаются дополнительной фильтрации по окато.*/
                if (auth.IsAdmin() || auth.IsSpectator() || profile == null || (profile.RefUchr.RefOrgPPO.Code == regionCodePPO))
                {
                    ////Обьединяються в округа на основании справочника ППО, он имеет 4 уровня.
                    var root = GetItems<D_Org_PPO>().Single(t => t.Code == regionCodePPO);
                    reportItems = new List<ReportItem> { new ReportItem { ID = root.ID, Name = root.Name, Bold = true } };
                    orgFilter.Add(root.ID);
                    var level2 = GetItems<D_Org_PPO>().Where(t => t.ParentID == root.ID).Select(t => t.ID).ToList();
                    var level3 = GetItems<D_Org_PPO>().Where(t => t.ParentID.HasValue && level2.Contains(t.ParentID.Value)).OrderBy(t => t.Code).ToList();
                    foreach (var aria in level3)
                    {
                        var level4 = GetItems<D_Org_PPO>().Where(t => t.ParentID == aria.ID);
                        var pposId = level4.Select(t => t.ID).ToList();
                        pposId.Add(aria.ID);
                        orgFilter.Add(aria.ID);
                        orgFilter.AddRange(level4.Select(t => t.ID));
                        /* Если 3 символ в коде ОКАТО 4, то это городской округ, у него нет подконтрольных элементов, 
                         * поэтому он считается 1 элементом а не группой, но при этом участвует в нумерации и суммировании как и группы.*/
                        reportItems.Add(
                            new ReportPPOAriaItem
                            {
                                ID = aria.ID,
                                Code = aria.Code,
                                Name = aria.Code[2] == '4' ? aria.Name : string.Concat(aria.Name, @" с поселениями"),
                                PposID = pposId,
                                Bold = true
                            });
                        if (aria.Code[2] != '4')
                        {
                            reportItems.Add(
                                new ReportItem
                                {
                                    ID = aria.ID,
                                    Name = aria.Name,
                                    Bold = false
                                });
                        }

                        reportItems.AddRange(
                            level4.Select(
                                ppo => new ReportItem
                                {
                                    ID = ppo.ID,
                                    Name = ppo.Name,
                                    Bold = false
                                }));
                    }
                }
                else
                {
                    var clientCode = profile.RefUchr.RefOrgPPO.Code;
                    var code = clientCode.TrimEnd('0');

                    var ppos = GetItems<D_Org_PPO>().Select(x => new { x.ID, x.Code, x.Name, parent = true }).OrderBy(x => x.Code).ToList();
                    ppos = ppos.Where(x => x.Code.StartsWith(code) || x.Code.Equals(clientCode))
                        .Select(x => new { x.ID, x.Code, x.Name, parent = x.Code.Equals(clientCode) })
                        .OrderBy(x => x.Code).ToList();
                    reportItems = ppos.Select(
                        x => new ReportItem
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Bold = false
                        }).ToList();
                    orgFilter = ppos.Select(x => x.ID).ToList();
                }

                institutions = GetItems<D_Org_Structure>().Where(
                    x => orgFilter.Contains(x.RefOrgPPO.ID)
                         && (x.CloseDate == null || x.CloseDate > reportDate)
                         && typesInstitutionList.Contains(x.RefTipYc.ID)
                         && !institutionsFounders.Contains(x.ID)).ToList();
            }

            var analReportData = new NewAnalReportData(sheet, workBook, reportDate, isPPO);

            // заполняем отчет
            analReportData.FillReport(reportItems, institutions);

            return workBook;
        }

        public HSSFWorkbook GetMonitoringPlacementInfoReport(DateTime reportDate, string docYear)
        {
            return MonitoringPlacementInfoReport.GetReport(reportDate, docYear);
        }

        /************************************************************************************************************************************************************************************************/

        private class AnalReportData
        {
            private const string TotalCodeRow = "Total";
            private const string KazRow = "Kaz";
            private const string BudgRow = "Budg";
            private const string AvtRow = "Avt";
            private const string NoVid = "NoVid";

            private const string PPCol = "PP";
            private const string NameCol = "Name";
            private const string TotalCol = "Total";
            private const string TotalPerCol = "TotalPer";
            private const string TotalGuCol = "TotalGu";
            private const string TotalGuPerCol = "TotalGuPer";
            private const string TotalMuCol = "TotalMu";
            private const string TotalMuPerCol = "TotalMuPer";

            private const int DeltaRow = 5;
            private const string Percents = "Per";

            private readonly List<string> cols;

            private readonly List<string> rows;

            private readonly HSSFWorkbook workBook;

            private readonly HSSFSheet sheet;

            private readonly ReportsService reportsService;

            private readonly DateTime reportDate;

            public AnalReportData(ReportsService reportsService, HSSFSheet sheet, HSSFWorkbook workBook, DateTime reportDate)
            {
                cols = new List<string>();
                rows = new List<string>();
                this.workBook = workBook;
                this.sheet = sheet;
                this.reportsService = reportsService;
                this.reportDate = reportDate;
                InitReport();
            }

            public void FillGu(IEnumerable<F_Org_Passport> passportsGu)
            {
               passportsGu.Each(x =>
                                { 
                                    switch (x.RefParametr.RefUchr.RefTipYc.ID)
                                    {
                                        case FX_Org_TipYch.AutonomousID:
                                            IncCellValByCode(AvtRow, TotalGuCol);
                                            break;
                                        case FX_Org_TipYch.BudgetaryID:
                                            IncCellValByCode(BudgRow, TotalGuCol);
                                            break;
                                        case FX_Org_TipYch.GovernmentID:
                                            IncCellValByCode(KazRow, TotalGuCol);
                                        break;
                                    }

                                    IncCellValByCode(x.RefVid.Code.Substring(0, 2), TotalGuCol);
                                });
            }

            public void FillMu(IEnumerable<F_Org_Passport> passports)
            {
                var sortPassports = passports.ToList();

                sortPassports.Sort((x, y) => string.Compare(x.RefParametr.RefUchr.RefOrgPPO.Code, y.RefParametr.RefUchr.RefOrgPPO.Code, StringComparison.Ordinal));

                sortPassports.Each(x =>
                                 {
                                    var colCode = Crutches(x.RefParametr.RefUchr.RefOrgPPO.Code.Substring(0, 5));
                                    switch (x.RefParametr.RefUchr.RefTipYc.ID)
                                    {
                                        case FX_Org_TipYch.AutonomousID:
                                            IncCellValByCode(AvtRow, colCode);
                                            IncCellValByCode(AvtRow, TotalMuCol);
                                            break;
                                        case FX_Org_TipYch.BudgetaryID:
                                            IncCellValByCode(BudgRow, colCode);
                                            IncCellValByCode(BudgRow, TotalMuCol);
                                            break;
                                        case FX_Org_TipYch.GovernmentID:
                                            IncCellValByCode(KazRow, colCode);
                                            IncCellValByCode(KazRow, TotalMuCol);
                                            break;
                                    }
                                    
                                     IncCellValByCode(x.RefVid.Code.Substring(0, 2), colCode);
                                     IncCellValByCode(x.RefVid.Code.Substring(0, 2), TotalMuCol);

                                     if (cols.IndexOf(colCode + Percents) == -1)
                                     {
                                         SetCellByCode(x.RefVid.Code.Substring(0, 2), colCode + Percents, string.Empty);    
                                     }
                });
            }

            public void FinishReport()
            {
                FillTotal();
                FillOtheData();
                SetHeader();
                SetStyles();
            }

            public void FillBadInstitutions(IEnumerable<D_Org_Structure> badInstitutions)
            {
                var instList = badInstitutions.ToList();

                if (instList.Any())
                {
                    HSSFSheet sheetLocal = workBook.CreateSheet("Нет сферы");

                    var styleForData = new ReportsHelper.HSSFCellStyleForData(workBook).CellStyle;
                    var styleBoldText = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;
                    var styleHeader = new ReportsHelper.HSSFCellStyleForColumns(workBook).CellStyle;

                    NpoiHelper.SetCellValue(
                        sheetLocal, 0, 0, "Учреждения, у которых не  указан вид учреждения(по состоянию на \"{0}\")".FormatWith(reportDate.Date.ToString("dd.MM.yyyy"))).CellStyle = styleBoldText;
                    NpoiHelper.SetAlignCenterSelection(workBook, sheetLocal, 0, 0, 3);

                    NpoiHelper.SetCellValue(sheetLocal, 1, 0, "ППО").CellStyle = styleHeader;
                    NpoiHelper.SetCellValue(sheetLocal, 1, 1, "ГРБС").CellStyle = styleHeader;
                    NpoiHelper.SetCellValue(sheetLocal, 1, 2, "Полное наименование учреждения").CellStyle = styleHeader;
                    NpoiHelper.SetCellValue(sheetLocal, 1, 3, "ИНН").CellStyle = styleHeader;

                    var row = 2;
                    instList.Each(x =>
                                         {
                                             if (x.RefOrgPPO == null)
                                             {
                                                 throw new Exception("Не указан ППО у учреждения '{0}'({1})".FormatWith(x.Name, x.INN));
                                             }

                                             // заполняем последнюю строчку в отчете
                                             var colCode = Crutches(x.RefOrgPPO.Code.Substring(0, 5));
                                             var total = x.RefOrgPPO.Code.Equals("78000000000") ? TotalGuCol : TotalMuCol;
                                             switch (x.RefTipYc.ID)               
                                             {
                                                 case FX_Org_TipYch.AutonomousID:
                                                     if (total != TotalGuCol)
                                                     {
                                                         IncCellValByCode(AvtRow, colCode);
                                                     }

                                                     IncCellValByCode(AvtRow, total);
                                                     break;
                                                 case FX_Org_TipYch.BudgetaryID:
                                                     if (total != TotalGuCol)
                                                     {
                                                         IncCellValByCode(BudgRow, colCode);
                                                     }

                                                     IncCellValByCode(BudgRow, total);
                                                     break;
                                                 case FX_Org_TipYch.GovernmentID:
                                                     if (total != TotalGuCol)
                                                     {
                                                         IncCellValByCode(KazRow, colCode);
                                                     }

                                                     IncCellValByCode(KazRow, total);
                                                     break;
                                             }

                                             if (total != TotalGuCol)
                                             {
                                                 IncCellValByCode(NoVid, colCode);
                                             }

                                             IncCellValByCode(NoVid, total);

                                             if ((total != TotalGuCol) && cols.IndexOf(colCode + Percents) == -1)
                                             {
                                                 SetCellByCode(NoVid, colCode + Percents, string.Empty);
                                             }

                                             // заполняем вторую страницу
                                             NpoiHelper.SetCellValue(sheetLocal, row, 0, x.RefOrgPPO == null ? string.Empty : x.RefOrgPPO.Name).CellStyle = styleForData;
                                             NpoiHelper.SetCellValue(sheetLocal, row, 1, x.RefOrgGRBS == null ? string.Empty : x.RefOrgGRBS.Name).CellStyle = styleForData;
                                             NpoiHelper.SetCellValue(sheetLocal, row, 2, x.Name).CellStyle = styleForData;
                                             NpoiHelper.SetCellValue(sheetLocal, row, 3, x.INN).CellStyle = styleForData;
                                             ++row;
                                         });

                    sheetLocal.SetColumnWidth(0, 10000);
                    sheetLocal.SetColumnWidth(1, 10000);
                    sheetLocal.SetColumnWidth(2, 10000);
                    sheetLocal.SetColumnWidth(3, 5000);
                }
            }

            private void FillTotal()
            {
                int firstRow = GetRowByCode(KazRow) + 1;
                int lastRow = GetRowByCode(AvtRow) + 1;

                cols.Where(x => !x.Equals(PPCol) && !x.Equals(NameCol)).Each(x =>
                          {
                              if (x.EndsWith(Percents))
                              {
                                  SetCellByCode(TotalCodeRow, x, 100);
                              }
                              else
                              {
                                  string col = NpoiHelper.ConvertToLetter(GetColByCode(x));

                                  string formula = "SUM({0}{1}:{0}{2})".FormatWith(col, firstRow, lastRow);

                                  SetCellFormulaByCode(TotalCodeRow, x, formula);   
                              }
                          });

                rows.Where(x => !x.Equals(TotalCodeRow) && !x.StartsWith("Sep")).Each(x =>
                                                                             {
                                                                                 string col1 = NpoiHelper.ConvertToLetter(GetColByCode(TotalGuCol));
                                                                                 string col2 = NpoiHelper.ConvertToLetter(GetColByCode(TotalMuCol));
                                                                                 int row = GetRowByCode(x) + 1;

                                                                                 string formula = "SUM({0}{1},{2}{1})".FormatWith(col1, row, col2);

                                                                                 SetCellFormulaByCode(x, TotalCol, formula);
                                                                             });
            }

            private void FillOtheData()
            {
                SetCellByCode(NoVid, PPCol, rows.Count - 2);
                SetCellByCode(NoVid, NameCol, "Учреждения, которые не имеют сферы деятельности");

                cols.Where(c => !c.Equals(PPCol) && !c.Equals(NameCol) && !c.Equals(TotalCol))
                    .Each(c => rows.Where(r => !r.Equals(TotalCodeRow) && !r.StartsWith("Sep"))
                                   .Each(r =>
                                         {
                                             if (c.EndsWith(Percents))
                                             {
                                                 int rowTotal = GetRowByCode(TotalCodeRow) + 1;
                                                 string colTotal = NpoiHelper.ConvertToLetter(GetColByCode(c.Replace(Percents, string.Empty)));
                                                 int rowCur = GetRowByCode(r) + 1;

                                                 string formula = "{0}{1}/${0}${2}*100".FormatWith(colTotal, rowCur, rowTotal);

                                                 SetCellFormulaByCode(r, c, formula); 
                                             }
                                             else
                                             {
                                                 if (GetCellValueByCode(r, c).IsNullOrEmpty())
                                                 {
                                                     SetCellByCode(r, c, 0);
                                                 }
                                             }       
                                         }));
            }

            private void SetStyles()
            {
                sheet.SetColumnWidth(GetColByCode(PPCol), 1000);
                sheet.SetColumnWidth(GetColByCode(NameCol), 10000);

                var styleForData = new ReportsHelper.HSSFCellStyleForData(workBook).CellStyle;
                var styleBoldText = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;
                var styleHeader = new ReportsHelper.HSSFCellStyleForColumns(workBook).CellStyle;
                
                var numberCell = workBook.CreateCellStyle();
                numberCell.CloneStyleFrom(styleForData);
                numberCell.DataFormat = NpoiHelper.DataFormatFloat;
                
                NpoiHelper.GetCellByXy(sheet, 0, 0).CellStyle = styleBoldText;
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, 0, 0, 20);

                NpoiHelper.GetCellByXy(sheet, 2, GetColByCode(PPCol)).CellStyle = styleBoldText;
                NpoiHelper.SetMergedRegion(sheet, 2, GetColByCode(PPCol), 3, GetColByCode(PPCol));
                NpoiHelper.SetBorderBoth(workBook, sheet, 2, GetColByCode(PPCol), 3, GetColByCode(PPCol));

                NpoiHelper.GetCellByXy(sheet, 2, GetColByCode(NameCol)).CellStyle = styleBoldText;
                NpoiHelper.SetMergedRegion(sheet, 2, GetColByCode(NameCol), 3, GetColByCode(NameCol));
                NpoiHelper.SetBorderBoth(workBook, sheet, 2, GetColByCode(NameCol), 3, GetColByCode(NameCol));

                cols.Where(c => !c.Equals(PPCol) && !c.Equals(NameCol) && !c.EndsWith(Percents))
                    .Each(c =>
                          {
                              NpoiHelper.GetCellByXy(sheet, 2, GetColByCode(c)).CellStyle = styleBoldText;
                              NpoiHelper.SetAlignCenterSelection(workBook, sheet, 2, GetColByCode(c), GetColByCode(c + Percents));
                              NpoiHelper.SetBorderBoth(workBook, sheet, 2, GetColByCode(c), 2, GetColByCode(c + Percents));
                              NpoiHelper.GetCellByXy(sheet, 3, GetColByCode(c)).CellStyle = styleHeader;
                              NpoiHelper.GetCellByXy(sheet, 3, GetColByCode(c + Percents)).CellStyle = styleHeader;
                          });

                cols.Each(c => NpoiHelper.GetCellByXy(sheet, 4, GetColByCode(c)).CellStyle = styleHeader);

                cols.Each(c => rows.Each(r =>
                                   {
                                       GetCellByCode(r, c).CellStyle = c.EndsWith(Percents) ? numberCell : styleForData;
                                   }));
            }

            private void SetHeader()
            {
                NpoiHelper.SetCellValue(
                    sheet, 0, 0, "Состав бюджетной сферы Ярославской области ( по данным регионального сервиса \"Web-консолидация 86-н\"на дату {0} )".FormatWith(reportDate.Date.ToString("dd.MM.yyyy")));

                NpoiHelper.SetCellValue(sheet, 2, GetColByCode(PPCol), "№ п/п");
                NpoiHelper.SetCellValue(sheet, 2, GetColByCode(TotalCol), "Всего");
                NpoiHelper.SetCellValue(sheet, 2, GetColByCode(TotalGuCol), "ГУ, всего");
                NpoiHelper.SetCellValue(sheet, 2, GetColByCode(TotalMuCol), "МУ, всего");

                cols.Where(c => !c.Equals(PPCol) && !c.Equals(NameCol) && !c.StartsWith(TotalCol) && !c.EndsWith(Percents))
                    .Each(c => NpoiHelper.SetCellValue(sheet, 2, GetColByCode(c), reportsService.GetItems<D_Org_PPO>().First(x => x.Code.Equals(c + "000000")).Name));

                cols.Where(c => !c.Equals(PPCol) && !c.Equals(NameCol)).Each(c => NpoiHelper.SetCellValue(sheet, 3, GetColByCode(c), c.EndsWith(Percents) ? "%" : "шт"));

                int counter = 1;
                cols.Where(c => !c.Equals(PPCol)).Each(c => NpoiHelper.SetCellValue(sheet, 4, GetColByCode(c), counter++));
            }

            private void IncCellValByCode(string rowCode, string colCode)
            {
                int val = 0;
                try
                {
                    val = Convert.ToInt32(NpoiHelper.GetCellStringValue(sheet, GetRowByCode(rowCode), GetColByCode(colCode)));
                }
                catch
                {
                    // ignored
                }

                NpoiHelper.SetCellValue(sheet, GetRowByCode(rowCode), GetColByCode(colCode), ++val);
            }

            private void SetCellByCode(string rowCode, string colCode, object value)
            {
                NpoiHelper.SetCellValue(sheet, GetRowByCode(rowCode), GetColByCode(colCode), value);
            }

            private void SetCellFormulaByCode(string rowCode, string colCode, string formula)
            {
                NpoiHelper.SetCellFormula(sheet, GetRowByCode(rowCode), GetColByCode(colCode), formula);
            }

            private string GetCellValueByCode(string rowCode, string colCode)
            {
               return NpoiHelper.GetCellStringValue(sheet, GetRowByCode(rowCode), GetColByCode(colCode));
            }

            private HSSFCell GetCellByCode(string rowCode, string colCode)
            {
                return NpoiHelper.GetCellByXy(sheet, GetRowByCode(rowCode), GetColByCode(colCode));
            }

            private void InitReport()
            {
                SetCellByCode(TotalCodeRow, PPCol, 1);
                SetCellByCode(TotalCodeRow, NameCol, "Всего");
                SetCellByCode(TotalCodeRow, TotalCol, string.Empty);
                SetCellByCode(TotalCodeRow, TotalPerCol, string.Empty);
                SetCellByCode(TotalCodeRow, TotalGuCol, string.Empty);
                SetCellByCode(TotalCodeRow, TotalGuPerCol, string.Empty);
                SetCellByCode(TotalCodeRow, TotalMuCol, string.Empty);
                SetCellByCode(TotalCodeRow, TotalMuPerCol, string.Empty);

                SetCellByCode("Sep1", NameCol, "в т.ч. по типам");

                SetCellByCode(KazRow, PPCol, 2);
                SetCellByCode(KazRow, NameCol, "Казенные учреждения");
                SetCellByCode(KazRow, TotalCol, string.Empty);
                SetCellByCode(KazRow, TotalPerCol, string.Empty);
                SetCellByCode(KazRow, TotalGuCol, string.Empty);
                SetCellByCode(KazRow, TotalGuPerCol, string.Empty);
                SetCellByCode(KazRow, TotalMuCol, string.Empty);
                SetCellByCode(KazRow, TotalMuPerCol, string.Empty);

                SetCellByCode(BudgRow, PPCol, 3);
                SetCellByCode(BudgRow, NameCol, "Бюджетные учреждения");
                SetCellByCode(BudgRow, TotalCol, string.Empty);
                SetCellByCode(BudgRow, TotalPerCol, string.Empty);
                SetCellByCode(BudgRow, TotalGuCol, string.Empty);
                SetCellByCode(BudgRow, TotalGuPerCol, string.Empty);
                SetCellByCode(BudgRow, TotalMuCol, string.Empty);
                SetCellByCode(BudgRow, TotalMuPerCol, string.Empty);

                SetCellByCode(AvtRow, PPCol, 4);
                SetCellByCode(AvtRow, NameCol, "Автономные учреждения");
                SetCellByCode(AvtRow, TotalCol, string.Empty);
                SetCellByCode(AvtRow, TotalPerCol, string.Empty);
                SetCellByCode(AvtRow, TotalGuCol, string.Empty);
                SetCellByCode(AvtRow, TotalGuPerCol, string.Empty);
                SetCellByCode(AvtRow, TotalMuCol, string.Empty);
                SetCellByCode(AvtRow, TotalMuPerCol, string.Empty);

                SetCellByCode("Sep2", NameCol, "в т.ч. по отраслям");

                var vidOrg = reportsService.GetItems<D_Org_VidOrg>().Where(x => x.Code.EndsWith("00000")).ToList();
                vidOrg.Sort((x, y) => string.Compare(x.Code, y.Code, StringComparison.Ordinal));
                vidOrg.Each(x =>
                            {
                                SetCellByCode(x.Code.Substring(0, 2), PPCol, rows.Count - 1);
                                SetCellByCode(x.Code.Substring(0, 2), NameCol, x.Name);
                            });
            }

            private int AddColByCode(string code)
            {
                cols.Add(code);

                return cols.IndexOf(code);
            }

            private int AddRowByCode(string code)
            {
                rows.Add(code);

                return rows.IndexOf(code);
            }

            private int GetColByCode(string code)
            {
                return cols.IndexOf(code) == -1 ? AddColByCode(code) : cols.IndexOf(code);
            }

            private int GetRowByCode(string code)
            {
                return rows.IndexOf(code) == -1 ? AddRowByCode(code) + DeltaRow : rows.IndexOf(code) + DeltaRow;
            }

            private string Crutches(string code)
            {
                // а что делать!? не легка наша жизнь(((
                switch (code)
                {
                    case "78410":
                        return "78237";
                    case "78417":
                        return "78243";
                    case "78420":
                        return "78246";
                }

                return code;
            }
        }

        private class NewAnalReportData
        {
            private const int PPCol = 0;
            private const int NameCol = 1;
            private const int TotalCol = 2;
            private const int TotalBuAu = 3;
            private const int TotalKu = 4;
            
            private const int DeltaRow = 4;

            private readonly HSSFWorkbook workBook;

            private readonly HSSFSheet sheet;

            private readonly DateTime reportDate;

            private readonly bool isPpo;

            private int summAuBu;
            private int summKu;

            public NewAnalReportData(HSSFSheet sheet, HSSFWorkbook workBook, DateTime reportDate, bool isPpo)
            {
                this.workBook = workBook;
                this.sheet = sheet;
                this.reportDate = reportDate;
                this.isPpo = isPpo;
            }

            public void FillReport(IEnumerable<ReportItem> reportItems, List<D_Org_Structure> institutions)
            {
                var curRow = DeltaRow;

                var comment = new HSSFComment(new NoteRecord(), new TextObjectRecord()) { Visible = false };

                foreach (var item in reportItems)
                {
                    // номер по порядку
                    var cellPP = NpoiHelper.SetCellValue(sheet, curRow, PPCol, curRow - DeltaRow + 1);
                    if (item.Bold)
                    {
                        cellPP.CellComment = comment;
                    }

                    // наименование ГРБС(ППО)
                    NpoiHelper.SetCellValue(sheet, curRow, NameCol, item.Name);

                    if (isPpo)
                    {
                        var ppoitem = item;

                        int insts;

                        var ariaItem = ppoitem as ReportPPOAriaItem;
                        if (ariaItem != null)
                        {
                            insts = institutions.Count(x => ariaItem.PposID.Contains(x.RefOrgPPO.ID)
                                && (x.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID) || x.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID)));
                        }
                            else
                        {
                            insts = institutions.Count(x => x.RefOrgPPO.ID.Equals(ppoitem.ID)
                                && (x.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID) || x.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID)));
                        }

                        if (item.Bold)
                        {
                            summAuBu += insts;
                        }
                        
                        // количество АУ/БУ учреждений
                        NpoiHelper.SetCellValue(sheet, curRow, TotalBuAu, insts);

                        var ppoAriaItem = ppoitem as ReportPPOAriaItem;
                        insts = ppoAriaItem != null ? institutions.Count(x => ppoAriaItem.PposID.Contains(x.RefOrgPPO.ID) && x.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID)) : institutions.Count(x => x.RefOrgPPO.ID.Equals(ppoitem.ID) && x.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID));

                        if (item.Bold)
                        {
                            summKu += insts;
                        }

                        // количество КУ учреждений
                        NpoiHelper.SetCellValue(sheet, curRow, TotalKu, insts);
                    }
                    else
                    {
                        ReportItem ppoitem = item;
                        var insts = institutions.Count(x => x.RefOrgGRBS.ID.Equals(ppoitem.ID)
                                && (x.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID) || x.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID)));

                        // количество АУ/БУ учреждений
                        NpoiHelper.SetCellValue(sheet, curRow, TotalBuAu, insts);

                        insts = institutions.Count(x => x.RefOrgGRBS.ID.Equals(ppoitem.ID) && x.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID));

                        // количество КУ учреждений
                        NpoiHelper.SetCellValue(sheet, curRow, TotalKu, insts);
                    }

                    ++curRow;
                }

                FillTotal();

                SetHeader();

                SetStyles();
            }

            private void FillTotal()
            {
                int lastRow = sheet.LastRowNum + 1;

                if (!isPpo)
                {
                    string col = NpoiHelper.ConvertToLetter(TotalKu);
                    string formula = "SUM({0}{1}:{0}{2})".FormatWith(col, DeltaRow + 1, lastRow);
                    NpoiHelper.SetCellFormula(sheet, lastRow, TotalKu, formula);

                    col = NpoiHelper.ConvertToLetter(TotalBuAu);
                    formula = "SUM({0}{1}:{0}{2})".FormatWith(col, DeltaRow + 1, lastRow);
                    NpoiHelper.SetCellFormula(sheet, lastRow, TotalBuAu, formula);
                }
                else
                {
                    NpoiHelper.SetCellValue(sheet, lastRow, TotalBuAu, summAuBu);
                    NpoiHelper.SetCellValue(sheet, lastRow, TotalKu, summKu);    
                }

                ++lastRow;
                
                for (var row = DeltaRow; row < lastRow; row++)
                {
                    var formula = "SUM({0}{1}:{2}{1})".FormatWith(NpoiHelper.ConvertToLetter(TotalBuAu), row + 1, NpoiHelper.ConvertToLetter(TotalKu));
                    NpoiHelper.SetCellFormula(sheet, row, TotalCol, formula);
                }
            }

            private void SetStyles()
            {
                var lastRow = sheet.LastRowNum;

                sheet.SetColumnWidth(NameCol, 10000);

                var styleForData = new ReportsHelper.HSSFCellStyleForData(workBook).CellStyle;
                var styleForDataBold = workBook.CreateCellStyle();
                styleForDataBold.CloneStyleFrom(styleForData);
                var boldFont = workBook.CreateFont();
                boldFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                styleForDataBold.SetFont(boldFont);

                var styleBoldText = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;
                var styleHeader = new ReportsHelper.HSSFCellStyleForColumns(workBook).CellStyle;
                var styleHeaderRotate = workBook.CreateCellStyle();
                styleHeaderRotate.CloneStyleFrom(styleHeader);
                styleHeaderRotate.Rotation = 90;

                NpoiHelper.GetCellByXy(sheet, 0, 0).CellStyle = styleBoldText;
                NpoiHelper.GetCellByXy(sheet, 1, 0).CellStyle = styleBoldText;
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, 0, 0, 4);
                NpoiHelper.SetAlignCenterSelection(workBook, sheet, 1, 0, 4);

                sheet.GetRow(2).Height = 2000;

                for (var col = 0; col <= 4; col++)
                {
                    NpoiHelper.GetCellByXy(sheet, 2, col).CellStyle = col > 1 ? styleHeaderRotate : styleHeader;

                    NpoiHelper.GetCellByXy(sheet, 3, col).CellStyle = styleHeader;
                }

                for (var row = DeltaRow; row <= lastRow; row++)
                {
                    var bold = NpoiHelper.GetCellByXy(sheet, row, 0).CellComment != null;
                    
                    for (var col = 0; col <= 4; col++)
                    {
                        NpoiHelper.GetCellByXy(sheet, row, col).CellStyle = bold ? styleForDataBold : styleForData;
                    }
                }
            }

            private void SetHeader()
            {
                NpoiHelper.SetCellValue(
                    sheet, 0, PPCol, "Структура сети за {0} год )".FormatWith(reportDate.Date.ToString("yyyy")));
                NpoiHelper.SetCellValue(
                    sheet, 1, PPCol, "(по состоянию на {0})".FormatWith(reportDate.Date.ToString("dd.MM.yyyy")));

                NpoiHelper.SetCellValue(sheet, 2, PPCol, "№ п/п");
                NpoiHelper.SetCellValue(sheet, 2, NameCol, isPpo ? "ППО" : "ГРБС");
                NpoiHelper.SetCellValue(sheet, 2, TotalCol, "Всего учреждений ");
                NpoiHelper.SetCellValue(sheet, 2, TotalBuAu, "В том числе БУ/АУ");
                NpoiHelper.SetCellValue(sheet, 2, TotalKu, "В том числе КУ");

                for (var col = 0; col <= 4; col++)
                {
                    NpoiHelper.SetCellValue(sheet, 3, col, col + 1);
                }

                NpoiHelper.SetCellValue(sheet, sheet.LastRowNum, NameCol, "Всего");    
            }
        }
    }
}