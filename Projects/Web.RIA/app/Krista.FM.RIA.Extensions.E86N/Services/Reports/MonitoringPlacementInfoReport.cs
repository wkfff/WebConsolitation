﻿using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    /// <summary>
    /// Отчет «Мониторинг размещения сведений»
    /// </summary>
    public class MonitoringPlacementInfoReport
    {
        private readonly IAuthService auth;
        private readonly ICommonDataService commonDataService;
        
        /// <summary>
        /// Код региона из справочника ППО 
        /// </summary>
        private readonly string regionCodePPO;

        /// <summary>
        /// Код региона 2е цифры
        /// </summary>справочника ППО 
        private readonly string regionCode;

        /// <summary>
        /// ID всех учреждений-учредителей
        /// </summary>
        private readonly List<int> institutionsFounders;

        /// <summary>
        /// Профиль текущего пользователя если он есть
        /// </summary>
        private readonly D_Org_UserProfile profile;
        
        /// <summary>
        /// год формирования документов для отчета
        /// </summary>
        private readonly int year;

        /// <summary>
        /// Дата отчета
        /// </summary>
        private readonly DateTime reportDate;
        
        /// <summary>
        /// Строки данных отчета
        /// </summary>
        private List<MonitoringPlacementInfoReportItem> reportItems;
        /// <summary>
        /// Экспортированный документ с максимальным ID
        /// </summary>
        private F_F_ParameterDoc maxExported;

        /// <summary>
        /// Максимальный ID экспортированного документа
        /// </summary>
        private int maxIdDoc;
        /// <summary>
        /// Типы документов отсортированные по ID
        /// </summary>
        private List<FX_FX_PartDoc> partDocs;

        private MonitoringPlacementInfoReport(DateTime reportDate, string docYear)
        {
            this.reportDate = reportDate;
            year = Convert.ToInt32(docYear);

            auth = Resolver.Get<IAuthService>();
            commonDataService = Resolver.Get<ICommonDataService>();
            var scheme = Resolver.Get<IScheme>();
            regionCodePPO = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Replace(" ", string.Empty) + "000";
            regionCode = regionCodePPO.Remove(2);
            
            profile = auth.Profile;

            
            // Получаем перечень ID учреждений, у которых есть учредители
            var cachedFounder =
                commonDataService.GetItems<D_Org_Structure>()
                    .Join(
                        commonDataService.GetItems<D_Org_OrgYchr>(),
                        structure => new { inn = structure.INN, kpp = structure.KPP },
                        ychr => new { ychr.RefNsiOgs.inn, ychr.RefNsiOgs.kpp },
                        (structure, ychr) => structure.ID)
                    .ToList();

            // Получаем перечень ID учреждений, у которых имя совпадает с учредителем
            var cachedFounderByName =
                commonDataService.GetItems<D_Org_Structure>()
                    .Join(
                            commonDataService.GetItems<D_Org_OrgYchr>(),
                            structure => structure.Name,
                            ychr => ychr.Name,
                            (structure, ychr) => structure.ID)
                    .ToList();

            institutionsFounders = cachedFounder.Union(cachedFounderByName).ToList();

            partDocs = commonDataService.GetItems<FX_FX_PartDoc>().OrderBy(x => x.ID).ToList();
        }
        
        public static HSSFWorkbook GetReport(DateTime reportDate, string docYear)
        {
            var workBook = new HSSFWorkbook();
            workBook.CreateSheet("Мониторинг размещения сведений");

            return new MonitoringPlacementInfoReport(reportDate, docYear).DoReport(workBook,docYear);
        }

        private HSSFWorkbook DoReport(HSSFWorkbook workBook,string docYear)
        {
            GetInstitutionData();

            GetDocData(docYear);
            
            FillingSheet(workBook, reportItems);

            return workBook;
        }

        /// <summary>
        /// Проверка галочек у КНМ и ГЗ
        /// </summary>
        private bool CheckDoc(F_F_ParameterDoc doc)
        {
            switch (doc.RefPartDoc.ID)
            {
                case FX_FX_PartDoc.StateTaskDocTypeID:
                    {
                        return commonDataService.GetItems<T_F_ExtHeader>().Any(x => x.RefParametr.ID == doc.ID && x.NotBring);
                    }

                case FX_FX_PartDoc.InfAboutControlActionsDocTypeID:
                    {
                        return commonDataService.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == doc.ID && x.NotInspectionActivity);
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Определяем должен ли быть документ
        /// </summary>
        private bool DoDocsByTypes(int typeInst, int typeDoc)
        {
            var typesDocs = commonDataService.GetPartDocList(typeInst).Select(x => x.ID).ToList();

            return typesDocs.Contains(typeDoc);
        }

        /// <summary>
        /// Заполнение информации по документам
        /// </summary>
        private void GetDocData(string docYear)
        {
            DateTime dateForCompare = DateTime.Parse("01.01." + year);
            reportItems.Each(
                x =>
                {
                    // все документы учреждения за год формирования
                    var docs = commonDataService.GetItems<F_F_ParameterDoc>().Where(d => d.RefUchr.ID.Equals(x.Id) 
                                                                                        &&(!d.RefPartDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID)
                                                                                        && (d.RefPartDoc.ID.Equals(FX_FX_PartDoc.ResultsOfActivityDocTypeID) || d.RefPartDoc.ID.Equals(FX_FX_PartDoc.InfAboutControlActionsDocTypeID))
                                                                                        && (d.RefYearForm.ID < year)
                                                                                        )
                                                                                        || 
                                                                                        (!d.RefPartDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID)
                                                                                        && !(d.RefPartDoc.ID.Equals(FX_FX_PartDoc.ResultsOfActivityDocTypeID) || d.RefPartDoc.ID.Equals(FX_FX_PartDoc.InfAboutControlActionsDocTypeID))
                                                                                        
                                                                                        //&& d.RefYearForm.ID.Equals(year)
                                                                                        
                                                                                         )).ToList();

                    // все паспорта беруться без ограничения на год формирования
                    var passports = commonDataService.GetItems<F_F_ParameterDoc>().Where(d => d.RefUchr.ID.Equals(x.Id)
                                                                                        && d.RefPartDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID)
                                                                                        && (d.OpeningDate == null || d.OpeningDate <= reportDate)
                                                                                        && (d.CloseDate == null || d.CloseDate > reportDate)).OrderBy(p => p.ID).ToList();

                    partDocs.Each(
                        p =>
                        {
                            if (DoDocsByTypes(x.Type, p.ID))
                            {
                                var docsTyped = p.ID.Equals(FX_FX_PartDoc.PassportDocTypeID) 
                                        ? passports 
                                         : docs.Where(d => d.RefPartDoc.ID.Equals(p.ID)).OrderBy(d => d.ID).ToList();

                                // если есть документы текущего типа
                                if (docsTyped.Any())
                                {
                                    // проверяем документы в состоянии экспортирован
                                    var exported = docsTyped.Where(e => e.RefSost.ID.Equals(FX_Org_SostD.ExportedStateID)).ToList();
//                                    if (exported.Any())
//                                    {
//                                        maxIdDoc = exported.Max(e => e.ID);
//                                        maxExported = exported.First(e => e.ID == maxIdDoc);
//                                    }
                                    if (exported.Any())
                                    {
                                        // експортированных документов может быть несколько, берм последний
                                        var maxIdDoc = exported.Max(e => e.ID);

                                        // берем записи из журнала по последнему экспортированному документу
                                        var logRec = commonDataService.GetItems<F_F_ChangeLog>()
                                            .Where(c => c.DocId.Equals(maxIdDoc) && c.RefChangeType.ID.Equals(FX_FX_ChangeLogActionType.OnExportedState))
                                            .OrderBy(l => l.ID).ToList();

                                        var state = GetStateByDocState(FX_Org_SostD.ExportedStateID);
                                        x.Docs.Add(new MonitoringPlacementInfoReportDocItem
                                        {
                                            TypeDoc = p.ID,
                                            Date = logRec.Any() ? logRec.Last().Data : (DateTime?)null,
                                            State = state.Name
                                        });
                                    }
                                    else
                                    {
                                        // документ
                                        var doc = docsTyped.Last();

                                        if (CheckDoc(doc))
                                        {
                                            x.Docs.Add(new MonitoringPlacementInfoReportDocItem
                                            {
                                                TypeDoc = p.ID,
                                                Date = null,
                                                State = "Не формируется"
                                            });
                                        }
                                        else
                                        {
                                            var state = GetStateByDocState(doc.RefSost.ID);
                                            
                                            // берем записи из журнала по последнему документу
                                            var logRec = commonDataService.GetItems<F_F_ChangeLog>()
                                                .Where(c => c.DocId.Equals(doc.ID) && c.RefChangeType.ID.Equals(state.ID))
                                                .OrderBy(l => l.ID).ToList();
                                            
                                            x.Docs.Add(new MonitoringPlacementInfoReportDocItem
                                            {
                                                TypeDoc = p.ID,
                                                Date = logRec.Any() ? logRec.Last().Data : (DateTime?)null,
                                                State = state.Name
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    x.Docs.Add(new MonitoringPlacementInfoReportDocItem
                                    {
                                        TypeDoc = p.ID,
                                        Date = null,
                                        State = "Отсутствует"
                                    });
                                }
                            }
                            else
                            {
                                x.Docs.Add(new MonitoringPlacementInfoReportDocItem
                                {
                                    TypeDoc = p.ID,
                                    Date = null,
                                    State = "Не формируется"
                                });
                            }
                        });
                });
        }

        /// <summary>
        /// Состояние из журнала по состоянию документа
        /// </summary>
        private FX_FX_ChangeLogActionType GetStateByDocState(int docState)
        {
            int state = -1;
            switch (docState)
            {
                case FX_Org_SostD.CreatedStateID:
                    state = FX_FX_ChangeLogActionType.AddDocument;
                    break;
                case FX_Org_SostD.UnderConsiderationStateID:
                    state = FX_FX_ChangeLogActionType.OnUnderConsiderationState;
                    break;
                case FX_Org_SostD.OnEditingStateID:
                    state = FX_FX_ChangeLogActionType.OnEditingState;
                    break;
                case FX_Org_SostD.FinishedStateID:
                    state = FX_FX_ChangeLogActionType.OnFinishedState;
                    break;
                case FX_Org_SostD.ExportedStateID:
                    state = FX_FX_ChangeLogActionType.OnExportedState;
                    break;
            }

            return commonDataService.Load<FX_FX_ChangeLogActionType>(state);
        }

        /// <summary>
        /// Формируем учреждения для отчета
        /// </summary>
        private void GetInstitutionData()
        {
            var typesInstitutionList = new List<int>
                                       {
                                           FX_Org_TipYch.BudgetaryID,
                                           FX_Org_TipYch.AutonomousID,
                                           FX_Org_TipYch.GovernmentID
                                       };
            
            // если отчет строит ФО(МО, ГО)
            if (auth.IsPpoUser() && profile.RefUchr.RefOrgPPO.Code != regionCodePPO)
            {
                /*      ППО      */
                var orgFilter = new List<int>();

                var clientCode = profile.RefUchr.RefOrgPPO;

                /* Если 3 символ в коде ОКАТО 4, то это городской округ*/
                if (clientCode.Code[2] == '4')
                {
                    // это ГО
                    orgFilter.Add(clientCode.ID);
                }
                else
                {
                    // это МО
                    FillPpoFilter(clientCode.ID, orgFilter);
                }

                reportItems =
                    commonDataService.GetItems<D_Org_Structure>()
                    .Where(
                       x =>
                       orgFilter.Contains(x.RefOrgPPO.ID)
                       && (!x.OpenDate.HasValue || x.OpenDate.Value.Year <= year)
                       && (!x.CloseDate.HasValue || x.CloseDate > reportDate)
                       && !institutionsFounders.Contains(x.ID))
                    .ToList()
                    .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                    .Select(x => new MonitoringPlacementInfoReportItem
                                    {
                                        NamePpo = x.RefOrgPPO.Name,
                                        NameGrbs = x.RefOrgGRBS.Name,
                                        Id = x.ID,
                                        Type = GetTypeOfStructureByYear(x),
                                        Inn = x.INN,
                                        Name = x.Name,
                                        Docs = new List<MonitoringPlacementInfoReportDocItem>()
                                    }).ToList();
            }
            else
            {
                // если отчет строит или админ или Депфин области
                reportItems =
                commonDataService.GetItems<D_Org_Structure>()
                   .Where(
                       x =>
                       x.RefOrgPPO.Code.StartsWith(regionCode)
                       && (!x.OpenDate.HasValue || x.OpenDate.Value.Year <= year)
                       && (!x.CloseDate.HasValue || x.CloseDate > reportDate)
                       && !institutionsFounders.Contains(x.ID))
                   .ToList()
                   .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                   .Select(x => new MonitoringPlacementInfoReportItem
                   {
                       NamePpo = x.RefOrgPPO.Name,
                       NameGrbs = x.RefOrgGRBS.Name,
                       Id = x.ID,
                       Type = GetTypeOfStructureByYear(x),
                       Inn = x.INN,
                       Name = x.Name,
                       Docs = new List<MonitoringPlacementInfoReportDocItem>()
                   }).ToList();
            }
        }

        // заполнение ППО для МО
        private void FillPpoFilter(int clientCodeID, List<int> orgFilter)
        {
            orgFilter.Add(clientCodeID);
            var child = commonDataService.GetItems<D_Org_PPO>().Where(x => x.ParentID == clientCodeID).ToList();
            child.Each(x => FillPpoFilter(x.ID, orgFilter));
        }
        
        /// <summary>
        /// Тип учреждения из истории(по году формирования)
        /// </summary>
        private int GetTypeOfStructureByYear(D_Org_Structure structure)
        {
            var historyType = year.Equals(reportDate.Year) 
                                ? structure.TypeHistories.SingleOrDefault(x => x.DateStart <= reportDate && reportDate <= x.DateEnd) 
                                : structure.TypeHistories.SingleOrDefault(x => x.DateStart.Year <= year && year <= x.DateEnd.Year);

            return historyType != null ? historyType.RefTypeStructure.ID : structure.RefTipYc.ID;
        }
        
        /// <summary>
        /// Формирование заголовка
        /// </summary>
        private void FillHeaderSheet(HSSFWorkbook workBook)
        {
            var sheet = workBook.GetSheetAt(0);

            var styleHeaderText = new ReportsHelper.HSSFCellStyleBoldText(workBook).CellStyle;

            NpoiHelper.SetCellValue(sheet, 0, 0, "Мониторинг размещения сведений").CellStyle = styleHeaderText;
            
            NpoiHelper.SetCellValue(sheet, 2, 0, "за {0} год".FormatWith(year)).CellStyle = styleHeaderText;

            var styleHeader = new ReportsHelper.HSSFCellStyleForColumns(workBook).CellStyle;
            
            NpoiHelper.SetCellValue(sheet, 5, 0, "ППО").CellStyle = styleHeader;
            NpoiHelper.SetCellValue(sheet, 8, 0, "1").CellStyle = styleHeader;

            NpoiHelper.SetCellValue(sheet, 5, 1, "ГРБС").CellStyle = styleHeader;
            NpoiHelper.SetCellValue(sheet, 8, 1, "2").CellStyle = styleHeader;

            NpoiHelper.SetCellValue(sheet, 5, 2, "ИНН учреждения").CellStyle = styleHeader;
            NpoiHelper.SetCellValue(sheet, 8, 2, "3").CellStyle = styleHeader;

            NpoiHelper.SetCellValue(sheet, 5, 3, "Наименование учреждения").CellStyle = styleHeader;
            NpoiHelper.SetCellValue(sheet, 8, 3, "4").CellStyle = styleHeader;

            sheet.SetColumnWidth(0, 8000);
            sheet.SetColumnWidth(1, 6000);
            sheet.SetColumnWidth(2, 3500);
            sheet.SetColumnWidth(3, 8000);
            
            NpoiHelper.SetMergedRegion(sheet, 5, 0, 7, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, 5, 0, 7, 0);

            NpoiHelper.SetMergedRegion(sheet, 5, 1, 7, 1);
            NpoiHelper.SetBorderBoth(workBook, sheet, 5, 1, 7, 1);

            NpoiHelper.SetMergedRegion(sheet, 5, 2, 7, 2);
            NpoiHelper.SetBorderBoth(workBook, sheet, 5, 2, 7, 2);

            NpoiHelper.SetMergedRegion(sheet, 5, 3, 7, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, 5, 3, 7, 3);

            NpoiHelper.SetCellValue(sheet, 5, 4, "Перечень документов");
            
            var col = 4;
            partDocs.Each(
                (x, c) =>
                    {
                        NpoiHelper.SetCellValue(sheet, 6, col, x.Name).CellStyle = styleHeader;
                        NpoiHelper.SetMergedRegion(sheet, 6, col, 6, col + 1);
                        NpoiHelper.SetBorderBoth(workBook, sheet, 6, col, 6, col + 1);

                        sheet.SetColumnWidth(col, 4000);
                        sheet.SetColumnWidth(col + 1, 4000);

                        NpoiHelper.SetCellValue(sheet, 7, col, "состояние").CellStyle = styleHeader;
                        NpoiHelper.SetCellValue(sheet, 7, col + 1, "дата").CellStyle = styleHeader;
                        NpoiHelper.SetCellValue(sheet, 8, col, c + 5).CellStyle = styleHeader;
                        NpoiHelper.GetCellByXy(sheet, 8, col + 1).CellStyle = styleHeader;
                        
                        col += 2;
                    });

            sheet.GetRow(6).Height = 500;

            var lastCol = col - 1;

            NpoiHelper.SetAlignCenterSelection(workBook, sheet, 0, 0, lastCol);

            NpoiHelper.SetAlignCenterSelection(workBook, sheet, 2, 0, lastCol);

            NpoiHelper.SetAlignCenterSelection(workBook, sheet, 5, 4, lastCol);
            NpoiHelper.SetBorderBoth(workBook, sheet, 5, 4, 5, lastCol);
        }

        /// <summary>
        /// Формирование страницы
        /// </summary>
        private void FillingSheet(HSSFWorkbook workBook, IList<MonitoringPlacementInfoReportItem> reportData)
        {
            FillHeaderSheet(workBook);
            
            var sheet = workBook.GetSheetAt(0);

            var styleCell = new ReportsHelper.HSSFCellStyleForData(workBook).CellStyle;

            var curRow = 9;
            reportData.Each(
                x =>
                    {
                        NpoiHelper.SetCellValue(sheet, curRow, 0, x.NamePpo).CellStyle = styleCell;
                        NpoiHelper.SetCellValue(sheet, curRow, 1, x.NameGrbs).CellStyle = styleCell;
                        NpoiHelper.SetCellValue(sheet, curRow, 2, x.Inn).CellStyle = styleCell;
                        NpoiHelper.SetCellValue(sheet, curRow, 3, x.Name).CellStyle = styleCell;

                        var curCol = 4;
                        var row = curRow;
                        partDocs.Each(
                            p =>
                                {
                                    var doc = x.Docs.Find(d => d.TypeDoc.Equals(p.ID));
                                    NpoiHelper.SetCellValue(sheet, row, curCol, doc.State).CellStyle = styleCell;
                                    NpoiHelper.SetCellValue(sheet, row, curCol + 1, doc.Date.HasValue ? doc.Date.Value.ToString("dd.MM.yy") : null).CellStyle = styleCell;
                                    curCol += 2;
                                });

                        curRow++;
                    });
        }
    }
}