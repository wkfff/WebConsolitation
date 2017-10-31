using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    /// <summary>
    /// Отчет по состояниям документов
    /// </summary>
    public class DocReport
    {
        private const string Header = "Показатели состояния работы по занесению {0} на региональный сервис 'Web-консолидация 86н' {1}";
        private const string Header1 = "(по состоянию на {0})";
        private const string Header2 = "Отсутствуют мероприятия";
        private const string Header3 = "Отсутствуют мероприятия(Документ утверждён)";
        private const string Header4 = "Не доводится ГЗ";
        private const string Header5 = "Не доводится ГЗ  (Документ утверждён)";
        private const string Header6 = "Учреждений казенных (уникально)";
        private const string Header7 = "Всего незавершенных документов(гр.5+гр.6+гр.7+гр.8+гр.15)";

        private readonly IAuthService auth;
        private readonly INewRestService newRestService;

        /// <summary>
        /// Код региона из справочника ППО 
        /// </summary>
        private readonly string regionCodePPO;

        /// <summary>
        /// Идентификаторы типов учреждений(FX_Org_TipYch) для отчета
        /// </summary>
        private readonly List<int> typesInstitutionList;

        /// <summary>
        /// ID всех учреждений-учредителей
        /// </summary>
        private readonly List<int> institutionsFounders;

        /// <summary>
        /// Профиль текущего пользователя если он есть
        /// </summary>
        private readonly D_Org_UserProfile profile;

        /// <summary>
        /// Учреждения которые не создали документ
        /// </summary>
        private readonly List<D_Org_Structure> notCreatedInstitutions;

        /// <summary>
        /// Тип документа по которому строим отчет
        /// </summary>
        private readonly FX_FX_PartDoc partDoc;

        /// <summary>
        /// год формирования документов для отчета
        /// </summary>
        private readonly int year;

        /// <summary>
        /// Дата отчета
        /// </summary>
        private readonly DateTime reportDate;

        /// <summary>
        /// По кому формируем отчет по ГРБС или по ППО
        /// </summary>
        private readonly bool isGrbs;

        /// <summary>
        /// Открытые учреждения, попадающие в отчет
        /// </summary>
        private List<D_Org_Structure> institutions;

        /// <summary>
        /// Закрытые учреждения, попадающие в отчет
        /// </summary>
        private List<D_Org_Structure> closeInstitutions;

        /// <summary>
        /// Строки данных отчета
        /// </summary>
        private List<ReportItem> reportItems;
        
        private bool oldStyle = true;

        /// <summary>
        /// Формат ячеек данных
        /// </summary>
        private HSSFCellStyle cellStyle;

        public DocReport(FX_FX_PartDoc partDoc, DateTime reportDate, bool isPPO, string docYear)
        {
            this.partDoc = partDoc;
            isGrbs = !isPPO;
            this.reportDate = reportDate;
            year = Convert.ToInt32(docYear);

            auth = Resolver.Get<IAuthService>();
            newRestService = Resolver.Get<INewRestService>();
            var scheme = Resolver.Get<IScheme>();
            regionCodePPO = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Replace(" ", string.Empty) + "000";

            notCreatedInstitutions = new List<D_Org_Structure>();

            typesInstitutionList = GetTypesInstitutionByTypeDoc();

            profile = !auth.IsAdmin() && !auth.IsSpectator() ? auth.Profile : null;

            // Получаем перечень ID учреждений, у которых есть учредители
            var cachedFounder =
                newRestService.GetItems<D_Org_Structure>()
                    .Join(
                        newRestService.GetItems<D_Org_OrgYchr>(),
                        structure => new { inn = structure.INN, kpp = structure.KPP },
                        ychr => new { ychr.RefNsiOgs.inn, ychr.RefNsiOgs.kpp },
                        (structure, ychr) => structure.ID)
                    .ToList();

            // Получаем перечень ID учреждений, у которых имя совпадает с учредителем
            var cachedFounderByName =
                newRestService.GetItems<D_Org_Structure>()
                    .Join(
                            newRestService.GetItems<D_Org_OrgYchr>(),
                            structure => structure.Name,
                            ychr => ychr.Name,
                            (structure, ychr) => structure.ID)
                    .ToList();

            institutionsFounders = cachedFounder.Union(cachedFounderByName).ToList();
        }
        
        public HSSFWorkbook GetDocReport(string tmpFileName)
        {
            HSSFWorkbook workBook;

            using (var fileStream = new FileStream(
                tmpFileName,
                FileMode.Open,
                FileAccess.Read))
            {
                workBook = new HSSFWorkbook(fileStream);
            }
            
            if (isGrbs)
            {
                GetGrbsData();
            }
            else
            {
                GetPpoData();
            }

            // Работаем сначала с открытыми учреждениями
            var institutionsIDList = institutions.Select(i => i.ID).ToList();

            // Выбираем ВСЕ документы учреждений, направленных на отчет 
            var documents = partDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID)
                                ? newRestService.GetItems<F_F_ParameterDoc>()
                                      .Where(
                                          x => x.RefPartDoc.ID.Equals(partDoc.ID)
                                               && institutionsIDList.Contains(x.RefUchr.ID)).ToList()
                                : newRestService.GetItems<F_F_ParameterDoc>()
                                      .Where(
                                          x => x.RefPartDoc.ID.Equals(partDoc.ID)
                                               && institutionsIDList.Contains(x.RefUchr.ID)
                                               && x.RefYearForm.ID.Equals(year)).ToList();

            var documents1 = documents;

            // Разбиваем учреждения по ГРБСам или по ППО, а также сопоставляем каждому учреждению его документы
            var docs = institutions
                .ToLookup(
                    x => isGrbs ? x.RefOrgGRBS.ID : x.RefOrgPPO.ID,
                    x => new
                    {
                        org = x.ID,
                        docs = GetDocs(x, documents1)
                    });

            // Оформляем отчет по полученным данным
            var reportData = new List<Docs>();
            reportItems.ForEach(
                x =>
                    {
                        var item = x as ReportPPOAriaItem;
                        if (item != null)
                        {
                            var aria = new Docs { Name = item.Name, Numerable = true };
                            item.PposID.ForEach(
                                t =>
                                {
                                    aria.InstitutionCount += docs[t].Select(doc => doc.org).Distinct().Count();
                                    aria.Count += docs[t].Sum(d => d.docs.Count);
                                    aria.NotCreated += docs[t].Sum(d => d.docs.NotCreated);
                                    aria.Created += docs[t].Sum(d => d.docs.Created);
                                    aria.UnderConsideration += docs[t].Sum(d => d.docs.UnderConsideration);
                                    aria.OnEditing += docs[t].Sum(d => d.docs.OnEditing);
                                    aria.Finished += docs[t].Sum(d => d.docs.Finished);
                                    aria.TotalIncomplete += docs[t].Sum(d => d.docs.TotalIncomplete);
                                    aria.Exported += docs[t].Sum(d => d.docs.Exported);
                                    aria.NumberOfPreparation += docs[t].Sum(d => d.docs.NumberOfPreparation);
                                    aria.PresenceOfCompleted += docs[t].Sum(d => d.docs.PresenceOfCompleted);
                                    aria.NotInspectionActivity += docs[t].Sum(d => d.docs.NotInspectionActivity);
                                    aria.NotInspectionActivityFinished += docs[t].Sum(d => d.docs.NotInspectionActivityFinished);
                                    aria.NotBring += docs[t].Sum(d => d.docs.NotBring);
                                    aria.NotBringFinished += docs[t].Sum(d => d.docs.NotBringFinished);
                                    aria.GovernmentCountNotCreatedDocs += docs[t].Sum(d => d.docs.GovernmentCountNotCreatedDocs);
                                });
                            reportData.Add(aria);
                        }
                        else
                        {
                            reportData.Add(
                                new Docs
                                {
                                    Root = x.Bold,
                                    Name = x.Name,
                                    Numerable = oldStyle,
                                    InstitutionCount = docs[x.ID].Select(doc => doc.org).Distinct().Count(),
                                    Count = docs[x.ID].Sum(d => d.docs.Count),
                                    NotCreated = docs[x.ID].Sum(d => d.docs.NotCreated),
                                    Created = docs[x.ID].Sum(d => d.docs.Created),
                                    UnderConsideration = docs[x.ID].Sum(d => d.docs.UnderConsideration),
                                    OnEditing = docs[x.ID].Sum(d => d.docs.OnEditing),
                                    Finished = docs[x.ID].Sum(d => d.docs.Finished),
                                    TotalIncomplete = docs[x.ID].Sum(d => d.docs.TotalIncomplete),
                                    Exported = docs[x.ID].Sum(d => d.docs.Exported),
                                    NumberOfPreparation = docs[x.ID].Sum(d => d.docs.NumberOfPreparation),
                                    PresenceOfCompleted = docs[x.ID].Sum(d => d.docs.PresenceOfCompleted),
                                    NotInspectionActivity = docs[x.ID].Sum(d => d.docs.NotInspectionActivity),
                                    NotInspectionActivityFinished = docs[x.ID].Sum(d => d.docs.NotInspectionActivityFinished),
                                    NotBring = docs[x.ID].Sum(d => d.docs.NotBring),
                                    NotBringFinished = docs[x.ID].Sum(d => d.docs.NotBringFinished),
                                    GovernmentCountNotCreatedDocs = docs[x.ID].Sum(d => d.docs.GovernmentCountNotCreatedDocs)
                                });
                        }
                    });

            reportData = reportData.Where(x => x.InstitutionCount > 0).ToList();

            reportData.Each(x => x.PartOfIncomplete = Math.Round(x.Count == 0 ? 0.0 : (double)x.TotalIncomplete / x.Count * 100d, 1, MidpointRounding.AwayFromZero));

            GetDataCellFormat(workBook);

            // Формируем страницу 1
            FillingFirstSheet(workBook, reportData);

            // Формируем страницу 2
            FillingSecondSheet(workBook);

            // Формируем страницу 3(закрытые учреждения)
            FillingThirdSheet(workBook, GetDataCloseInst());

            return workBook;
        }

        /// <summary>
        ///  Обработка закрытых учреждений
        /// </summary>
        private List<Docs> GetDataCloseInst()
        {
             var institutionsIDList = closeInstitutions.Select(i => i.ID).ToList();
             var documents = partDoc.ID == FX_FX_PartDoc.PassportDocTypeID
                             ? newRestService.GetItems<F_F_ParameterDoc>()
                                   .Where(
                                       x => x.RefPartDoc.ID == partDoc.ID
                                            && institutionsIDList.Contains(x.RefUchr.ID)).ToList()
                             : newRestService.GetItems<F_F_ParameterDoc>()
                                   .Where(
                                       x => x.RefPartDoc.ID == partDoc.ID
                                            && institutionsIDList.Contains(x.RefUchr.ID)
                                            && x.RefYearForm.ID == year).ToList();

             var docs = closeInstitutions
                 .ToLookup(
                     x => isGrbs ? x.RefOrgGRBS.ID : x.RefOrgPPO.ID,
                     x => new
                     {
                         org = x.ID,
                         docs = GetDocs(x, documents, true)
                     });
             var reportDataCloseInst = new List<Docs>();
             reportItems.ForEach(
                 x =>
                 {
                     if (!((x is ReportPPOAriaItem) && ((ReportPPOAriaItem)x).Code[2] != '4'))
                     {
                         reportDataCloseInst.Add(
                             new Docs
                             {
                                 Name = x.Name,
                                 InstitutionCount = docs[x.ID].Select(doc => doc.org).Distinct().Count(),
                                 Count = docs[x.ID].Sum(d => d.docs.Count),
                                 NotCreated = docs[x.ID].Sum(d => d.docs.NotCreated),
                                 Created = docs[x.ID].Sum(d => d.docs.Created),
                                 UnderConsideration = docs[x.ID].Sum(d => d.docs.UnderConsideration),
                                 OnEditing = docs[x.ID].Sum(d => d.docs.OnEditing),
                                 Finished = docs[x.ID].Sum(d => d.docs.Finished),
                                 TotalIncomplete = docs[x.ID].Sum(d => d.docs.TotalIncomplete),
                                 Exported = docs[x.ID].Sum(d => d.docs.Exported),
                                 NumberOfPreparation = docs[x.ID].Sum(d => d.docs.NumberOfPreparation),
                                 PresenceOfCompleted = docs[x.ID].Sum(d => d.docs.PresenceOfCompleted),
                                 NotInspectionActivity = docs[x.ID].Sum(d => d.docs.NotInspectionActivity),
                                 NotInspectionActivityFinished = docs[x.ID].Sum(d => d.docs.NotInspectionActivityFinished),
                                 GovernmentCountNotCreatedDocs = docs[x.ID].Sum(d => d.docs.GovernmentCountNotCreatedDocs)
                             });
                     }
                 });
             return reportDataCloseInst.Where(x => x.InstitutionCount > 0).ToList();
        }

        private void GetPpoData()
        {
            /*      ППО      */
            var orgFilter = new List<int>();
            /* Если отчет строится для всей области, то необходимо обьединять элеметы в Мун.Округа, 
                 * иначе этого делать не надо, но данные отчета подвергаются дополнительной фильтрации по окато.*/
            if (auth.IsAdmin() || auth.IsSpectator() || profile == null || profile.RefUchr.RefOrgPPO.Code == regionCodePPO)
            {
                ////Обьединяються в округа на основании справочника ППО, он имеет 4 уровня.
                oldStyle = false;
                var root = newRestService.GetItems<D_Org_PPO>().Single(t => t.Code == regionCodePPO);
                
                // здесь Bold используется для идентификации корня ППО(региона)
                reportItems = new List<ReportItem> { new ReportItem { ID = root.ID, Name = root.Name, Bold = true } };
                orgFilter.Add(root.ID);
                var level2 = newRestService.GetItems<D_Org_PPO>().Where(t => t.ParentID == root.ID).Select(t => t.ID).ToList();
                var level3 = newRestService.GetItems<D_Org_PPO>().Where(t => t.ParentID.HasValue && level2.Contains(t.ParentID.Value)).OrderBy(t => t.Code).ToList();
                foreach (var aria in level3)
                {
                    var level4 = newRestService.GetItems<D_Org_PPO>().Where(t => t.ParentID == aria.ID);
                    var pposId = level4.Select(t => t.ID).ToList();
                    pposId.Add(aria.ID);
                    orgFilter.Add(aria.ID);
                    orgFilter.AddRange(level4.Select(t => t.ID));
                    
                    /* Если 3 символ в коде ОКАТО 4, то это городской округ, у него нет подконтрольных элементов, 
                         * поэтому он считается 1 элементом а не группой, но при этом участвует в нумерации и суммировании как и группы.*/
                    reportItems.Add(new ReportPPOAriaItem
                                        {
                                            ID = aria.ID,
                                            Code = aria.Code,
                                            Name = aria.Code[2] == '4' ? aria.Name : string.Concat(aria.Name, @" с поселениями"), PposID = pposId
                                        });

                    if (aria.Code[2] != '4')
                    {
                        reportItems.Add(new ReportItem { ID = aria.ID, Name = aria.Name });
                    }

                    reportItems.AddRange(level4.Select(ppo => new ReportItem { ID = ppo.ID, Name = ppo.Name }));
                }
            }
            else
            {
                var clientCode = profile.RefUchr.RefOrgPPO.Code;
                var code = clientCode.TrimEnd('0');

                var ppos = newRestService.GetItems<D_Org_PPO>().Select(x => new { x.ID, x.Code, x.Name, parent = true }).OrderBy(x => x.Code).ToList();
                ppos = ppos.Where(x => x.Code.StartsWith(code) || x.Code.Equals(clientCode))
                            .Select(x => new { x.ID, x.Code, x.Name, parent = x.Code.Equals(clientCode) })
                                .OrderBy(x => x.Code).ToList();
                reportItems = ppos.Select(x => new ReportItem { ID = x.ID, Name = x.Name }).ToList();
                orgFilter = ppos.Select(x => x.ID).ToList();
            }

            institutions =
                newRestService.GetItems<D_Org_Structure>()
                    .Where(x => orgFilter.Contains(x.RefOrgPPO.ID) 
                            && (!x.OpenDate.HasValue || x.OpenDate.Value.Year <= year)
                            && (!x.CloseDate.HasValue || x.CloseDate > reportDate) 
                            && !institutionsFounders.Contains(x.ID))
                    .ToList()
                    .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                    .ToList();

            closeInstitutions =
                newRestService.GetItems<D_Org_Structure>()
                    .Where(x => orgFilter.Contains(x.RefOrgPPO.ID) && (x.CloseDate.HasValue && x.CloseDate <= reportDate) && !institutionsFounders.Contains(x.ID))
                    .ToList()
                    .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                    .ToList();
        }

        private void GetGrbsData()
        {
            /*    ГРБС    */

            // id главного ППО
            var regionId = newRestService.GetItems<D_Org_PPO>().Single(ppo => ppo.Code.Equals(ConfigurationManager.AppSettings["ClientLocationOKATOCode"] + "000000000")).ID;

            var id = (auth.IsAdmin() || auth.IsSpectator() || profile == null) ? regionId : profile.RefUchr.RefOrgPPO.ID;

            // ГРБСы, по которым будет составляться отчет
            reportItems =
                ((auth.IsAdmin() || auth.IsSpectator() || profile == null)
                     ? newRestService.GetItems<D_Org_GRBS>().ToList()
                     : newRestService.GetItems<D_Org_GRBS>().ToList().Where(x => auth.IsPpoUser() ? x.RefOrgPPO.ID == profile.RefUchr.RefOrgPPO.ID : x.ID == profile.RefUchr.RefOrgGRBS.ID))
                    .Where(x => x.RefOrgPPO.ID == id).Select(x => new ReportItem { ID = x.ID, Name = x.Name }).ToList();

            var grbsIds = reportItems.Select(x => x.ID).ToList();

            // Выбираем учреждения, у которых ГРБС нужный нам для отчета,
            // год создания учреждения меньше или равен году формирования документов,
            // дата закрытия учреждения или пуста или больше даты отчета
            // и не явлется учредителем
            institutions =
                newRestService.GetItems<D_Org_Structure>()
                    .Where(x => grbsIds.Contains(x.RefOrgGRBS.ID)
                            && (!x.OpenDate.HasValue || x.OpenDate.Value.Year <= year)
                            && (x.CloseDate == null || x.CloseDate > reportDate)
                            && !institutionsFounders.Contains(x.ID))
                    .ToList()
                    .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                    .ToList();

            // Выбираем ЗАКРЫТЫЕ учреждения, у которых ГРБС нужный нам для отчета
            closeInstitutions =
                newRestService.GetItems<D_Org_Structure>()
                    .Where(x => grbsIds.Contains(x.RefOrgGRBS.ID) && (x.CloseDate.HasValue && x.CloseDate <= reportDate) && !institutionsFounders.Contains(x.ID))
                    .ToList()
                    .Where(x => typesInstitutionList.Contains(GetTypeOfStructureByYear(x)))
                    .ToList();
        }

        private bool AddСriterion(F_F_ParameterDoc doc)
        {
            switch (doc.RefPartDoc.ID)
            {
                case FX_FX_PartDoc.StateTaskDocTypeID:
                    {
                        return !newRestService.GetItems<T_F_ExtHeader>().Any(x => x.RefParametr.ID == doc.ID && x.NotBring);
                    }

                case FX_FX_PartDoc.InfAboutControlActionsDocTypeID:
                    {
                        return !newRestService.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == doc.ID && x.NotInspectionActivity);
                    }

                default:
                    {
                        return true;
                    }
            }
        }

        private Docs GetDocs(D_Org_Structure inst, IEnumerable<F_F_ParameterDoc> documents, bool isCloseInst = false)
        {
            // Отбираем документы только данного учреждения
            var docsList = documents.Where(y => y.RefUchr.ID == inst.ID).ToList();
            var result = new Docs();

            if (docsList.Any())
            {
                // если есть документы
                result.NotCreated = 0;
                var notCurrentYear = reportDate.Year != DateTime.Now.Year;
                
                result.Created = docsList.Count(
                    d => d.RefSost.ID == FX_Org_SostD.CreatedStateID
                            && (d.OpeningDate <= reportDate.AddDays(-5) || d.OpeningDate == null) && AddСriterion(d));

                result.UnderConsideration = docsList.Count(
                    d => d.RefSost.ID == FX_Org_SostD.UnderConsiderationStateID
                            && (d.OpeningDate <= reportDate.AddDays(-5) || d.OpeningDate == null) && AddСriterion(d));

                result.OnEditing = docsList.Count(
                    d => d.RefSost.ID == FX_Org_SostD.OnEditingStateID
                            && (d.OpeningDate <= reportDate.AddDays(-5) || d.OpeningDate == null) && AddСriterion(d));

                result.Finished = docsList.Count(
                    d => d.RefSost.ID == FX_Org_SostD.FinishedStateID
                            && (notCurrentYear || d.OpeningDate <= reportDate || d.OpeningDate == null) && AddСriterion(d));

                result.Exported = docsList.Count(
                    d => d.RefSost.ID == FX_Org_SostD.ExportedStateID
                            && (notCurrentYear || d.OpeningDate <= reportDate || d.OpeningDate == null) && AddСriterion(d));
                
                result.NumberOfPreparation = docsList.Count(d => d.RefSost.ID != FX_Org_SostD.ExportedStateID
                                                                 && d.RefSost.ID != FX_Org_SostD.FinishedStateID
                                                                 && d.OpeningDate >= reportDate.AddDays(-5));

                result.PresenceOfCompleted = Convert.ToInt16(docsList.Any(d => d.RefSost.ID == FX_Org_SostD.ExportedStateID
                                                                               || d.RefSost.ID == FX_Org_SostD.FinishedStateID));

                if (docsList.First().RefPartDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID)
                {
                    result.NotInspectionActivity = docsList.Count(d => d.RefSost.ID != FX_Org_SostD.FinishedStateID
                                                                    && (d.OpeningDate <= reportDate.AddDays(-5) || d.OpeningDate == null)
                                                                    && !AddСriterion(d));

                    result.NotInspectionActivityFinished = docsList.Count(d => d.RefSost.ID == FX_Org_SostD.FinishedStateID && !AddСriterion(d));
                }

                if (docsList.First().RefPartDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                {
                    result.NotBring = docsList.Count(d => d.RefSost.ID != FX_Org_SostD.FinishedStateID
                                                                    && (d.OpeningDate <= reportDate.AddDays(-5) || d.OpeningDate == null)
                                                                    && !AddСriterion(d));

                    result.NotBringFinished = docsList.Count(d => d.RefSost.ID == FX_Org_SostD.FinishedStateID && !AddСriterion(d));
                }
            }
            else
            {
                // если нет документов
                if (!isCloseInst)
                {
                    result.NotCreated = 1;
                    notCreatedInstitutions.Add(inst);
                }
            }

            result.GovernmentCountNotCreatedDocs = inst.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID) ? 1 : 0;

            result.TotalIncomplete = result.NotCreated + result.Created + result.UnderConsideration + result.OnEditing + result.NotBring;
            result.Count = result.TotalIncomplete + result.Finished + result.Exported + result.NotInspectionActivity + result.NotInspectionActivityFinished + result.NotBring + result.NotBringFinished;

            return result;
        }

        /// <summary>
        /// Определяет типы учреждений по типу документа
        /// </summary>
        /// <returns> список idшников типов учреждений </returns>
        private List<int> GetTypesInstitutionByTypeDoc()
        {
            switch (partDoc.ID)
            {
                case FX_FX_PartDoc.SmetaDocTypeID:
                    {
                        return new List<int> { FX_Org_TipYch.GovernmentID };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    {
                        return new List<int> { FX_Org_TipYch.GovernmentID };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    {
                        return new List<int> { FX_Org_TipYch.GovernmentID };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    {
                        return new List<int> { FX_Org_TipYch.GovernmentID };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    {
                        return new List<int> { FX_Org_TipYch.GovernmentID };
                    }

                case FX_FX_PartDoc.StateTaskDocTypeID:
                    {
                        return new List<int>
                                   {
                                       FX_Org_TipYch.BudgetaryID,
                                       FX_Org_TipYch.AutonomousID,
                                       FX_Org_TipYch.GovernmentID
                                   };
                    }

                case FX_FX_PartDoc.PfhdDocTypeID:
                    {
                        return new List<int>
                                   {
                                       FX_Org_TipYch.BudgetaryID,
                                       FX_Org_TipYch.AutonomousID
                                   };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    {
                        return new List<int>
                                   {
                                       FX_Org_TipYch.BudgetaryID,
                                       FX_Org_TipYch.AutonomousID
                                   };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    {
                        return new List<int>
                                   {
                                       FX_Org_TipYch.BudgetaryID,
                                       FX_Org_TipYch.AutonomousID
                                   };
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    {
                        return new List<int>
                                   {
                                       FX_Org_TipYch.BudgetaryID,
                                       FX_Org_TipYch.AutonomousID
                                   };
                    }
            }

            return newRestService.GetItems<FX_Org_TipYch>().Where(x => x.ID != 0).Select(x => x.ID).ToList();
        }

        /// <summary>
        /// Тип учреждения из истории(по году формирования)
        /// </summary>
        private int GetTypeOfStructureByYear(D_Org_Structure structure)
        {
            F_Org_TypeHistory historyType;
            if (year.Equals(reportDate.Year))
            {
                historyType = structure.TypeHistories.SingleOrDefault(x => x.DateStart <= reportDate && reportDate <= x.DateEnd);
            }
            else
            {
                historyType = structure.TypeHistories.SingleOrDefault(x => x.DateStart.Year <= year && year <= x.DateEnd.Year);
            }
                
            return historyType != null ? historyType.RefTypeStructure.ID : structure.RefTipYc.ID;
        }

        /// <summary>
        /// Создаем стиль оформления ячеек данных
        /// </summary>
        private void GetDataCellFormat(HSSFWorkbook workBook)
        {
            var sheet = workBook.GetSheetAt(0);

            cellStyle = workBook.CreateCellStyle();
            cellStyle.CloneStyleFrom(NpoiHelper.GetCellByXy(sheet, 3, 0).CellStyle);
            cellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
        }

        /// <summary>
        /// Формирование заголовка для первой страницы
        /// </summary>
        private void FillHeaderFirstSheet(HSSFWorkbook workBook)
        {
            var sheet = workBook.GetSheetAt(0);

            // добавление дополнительных столбцов в шаблон для КМ
            if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID)
            {
                NpoiHelper.SetCellValue(sheet, 2, 14, Header2).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 14, "15").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 15, Header3).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 15, "16").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
            }

            // добавление дополнительных столбцов в шаблон для ГЗ
            if (partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
            {
                NpoiHelper.SetCellValue(sheet, 2, 9, Header7).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 9, "10").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 14, Header4).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 14, "15").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 15, Header5).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 15, "16").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 16, Header6).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 13).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 16, "17").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 13).CellStyle;
            }

            workBook.SetSheetName(0, isGrbs ? "по ГРБС" : "по ППО");
            
            var header = Header.FormatWith(
                GetTypeDoc(partDoc.ID),
                partDoc.ID != FX_FX_PartDoc.PassportDocTypeID ? "за {0} год".FormatWith(year) : string.Empty);

            NpoiHelper.SetCellValue(sheet, 0, 0, header);

            NpoiHelper.SetCellValue(sheet, 1, 0, Header1.FormatWith(string.Format("{0:dd.MM.yyyy}", reportDate)));

            NpoiHelper.SetCellValue(sheet, 2, 1, isGrbs ? "ГРБС" : "ППО");
        }

        /// <summary>
        /// Формирование первой страницы
        /// </summary>
        private void FillingFirstSheet(HSSFWorkbook workBook, ICollection<Docs> reportData)
        {
            FillHeaderFirstSheet(workBook);

            const int FirstRow = 4;

            var curRow = 0;
            var numerable = 1;
            var summArray = new List<int>();
            var rootRow = 0;

            var sheet = workBook.GetSheetAt(0);
            
            for (var position = 0; position < reportData.Count; position++)
            {
                curRow = FirstRow + position;
                var reportRow = reportData.ElementAt(position);

                // фиксируем строку с корнем ППО, для дальнейшего ее суппирования
                if (reportRow.Root)
                {
                    rootRow = curRow + 1;
                }

                if (reportRow.Numerable)
                {
                    summArray.Add(curRow + 1);
                    NpoiHelper.SetCellValue(sheet, curRow, 0, numerable++).CellStyle = cellStyle;
                }
                else
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 0, string.Empty).CellStyle = cellStyle;
                }

                NpoiHelper.SetCellValue(sheet, curRow, 1, reportRow.Name).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 2, reportRow.InstitutionCount).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 3, reportRow.Count).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 4, reportRow.NotCreated).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 5, reportRow.Created).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 6, reportRow.UnderConsideration).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 7, reportRow.OnEditing).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 8, reportRow.Finished).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 9, reportRow.TotalIncomplete).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 10, reportRow.Exported).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 11, reportRow.PartOfIncomplete).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 12, reportRow.NumberOfPreparation).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 13, reportRow.PresenceOfCompleted).CellStyle = cellStyle;
                if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID)
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 14, reportRow.NotInspectionActivity).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 15, reportRow.NotInspectionActivityFinished).CellStyle = cellStyle;
                }

                if (partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 14, reportRow.NotBring).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 15, reportRow.NotBringFinished).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 16, reportRow.GovernmentCountNotCreatedDocs).CellStyle = cellStyle;
                }
            }

            if (reportData.Any())
            {
                var numberCell = workBook.CreateCellStyle();
                numberCell.CloneStyleFrom(cellStyle);
                numberCell.DataFormat = NpoiHelper.DataFormatFloat;
                curRow++;
                if (!oldStyle)
                {
                    var formulaBuilder = new StringBuilder("SUM(");
                    summArray.ForEach(i => formulaBuilder.Append("{0}").Append(i).Append(","));
                    formulaBuilder.Append(")");
                    var formula = formulaBuilder.ToString();
                    NpoiHelper.SetCellValue(sheet, curRow, 1, "Итого по МР с поселениями и ГО").CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 2, string.Format(formula, "C")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 3, string.Format(formula, "D")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 4, string.Format(formula, "E")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 5, string.Format(formula, "F")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 6, string.Format(formula, "G")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 7, string.Format(formula, "H")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 8, string.Format(formula, "I")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 9, string.Format(formula, "J")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 10, string.Format(formula, "K")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 11, string.Format("J{0}/D{0}*100", curRow + 1)).CellStyle = numberCell;
                    NpoiHelper.SetCellFormula(sheet, curRow, 12, string.Format(formula, "M")).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 13, string.Format(formula, "N")).CellStyle = cellStyle;
                    if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID || partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                    {
                        NpoiHelper.SetCellFormula(sheet, curRow, 14, string.Format(formula, "O")).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 15, string.Format(formula, "P")).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 16, string.Format(formula, "Q")).CellStyle = cellStyle;
                    }
                    
                    curRow++;
                    NpoiHelper.SetCellValue(sheet, curRow, 1, "Всего").CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 2, "SUM(C{0},C{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 3, "SUM(D{0},D{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 4, "SUM(E{0},E{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 5, "SUM(F{0},F{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 6, "SUM(G{0},G{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 7, "SUM(H{0},H{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 8, "SUM(I{0},I{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 9, "SUM(J{0},J{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 10, "SUM(K{0},K{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 11, "J{0}/D{0}*100".FormatWith(curRow + 1)).CellStyle = numberCell;
                    NpoiHelper.SetCellFormula(sheet, curRow, 12, "SUM(M{0},M{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 13, "SUM(N{0},N{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID || partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                    {
                        NpoiHelper.SetCellFormula(sheet, curRow, 14, "SUM(O{0},O{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 15, "SUM(P{0},P{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 16, "SUM(Q{0},Q{1})".FormatWith(curRow, rootRow)).CellStyle = cellStyle;
                    }
                }
                else
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 1, "Всего").CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 2, "SUM(C{0}:C{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 3, "SUM(D{0}:D{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 4, "SUM(E{0}:E{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 5, "SUM(F{0}:F{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 6, "SUM(G{0}:G{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 7, "SUM(H{0}:H{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 8, "SUM(I{0}:I{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 9, "SUM(J{0}:J{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 10, "SUM(K{0}:K{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 11, "J{0}/D{0}*100".FormatWith(curRow + 1)).CellStyle = numberCell;
                    NpoiHelper.SetCellFormula(sheet, curRow, 12, "SUM(M{0}:M{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 13, "SUM(N{0}:N{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID || partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                    {
                        NpoiHelper.SetCellFormula(sheet, curRow, 14, "SUM(O{0}:O{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 15, "SUM(P{0}:P{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                        NpoiHelper.SetCellFormula(sheet, curRow, 16, "SUM(Q{0}:Q{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    }
                }
            }
        }

        /// <summary>
        /// Формирование 2й страницы
        /// </summary>
        private void FillingSecondSheet(HSSFWorkbook workBook)
        {
            const string CloseInstHeader = "Учреждения, которые не создали документ \"{0}\" {1}";
            const int FirstRow = 3;
            
            var sheet = workBook.GetSheetAt(1);
            
            var header = CloseInstHeader.FormatWith(
                                        partDoc.Name,
                                        partDoc.ID != FX_FX_PartDoc.PassportDocTypeID ? "за {0} год".FormatWith(year) : string.Empty);

            NpoiHelper.SetCellValue(sheet, 0, 0, header);
            NpoiHelper.SetCellValue(sheet, 1, 0, Header1.FormatWith(string.Format("{0:dd.MM.yyyy}", reportDate)));
            
            var curRow = FirstRow;
            foreach (var notCreatedInstitution in notCreatedInstitutions)
            {
                if (notCreatedInstitution.RefOrgPPO == null)
                {
                    throw new Exception("У учреждения '{0}({1})' не проставлен ППО".FormatWith(notCreatedInstitution.Name, notCreatedInstitution.INN));
                }

                NpoiHelper.SetCellValue(sheet, curRow, 0, notCreatedInstitution.RefOrgPPO.Name).CellStyle = cellStyle;

                if (notCreatedInstitution.RefOrgGRBS == null)
                {
                    throw new Exception("У учреждения '{0}({1})' не проставлен ГРБС".FormatWith(notCreatedInstitution.Name, notCreatedInstitution.INN));
                }

                NpoiHelper.SetCellValue(sheet, curRow, 1, notCreatedInstitution.RefOrgGRBS.Name).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 2, notCreatedInstitution.Name).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 3, notCreatedInstitution.INN).CellStyle = cellStyle;

                curRow++;
            }
        }

        /// <summary>
        /// Формирование 3й страницы с закрытыми учреждениями
        /// </summary>
        private void FillingThirdSheet(HSSFWorkbook workBook, ICollection<Docs> reportDataCloseInst)
        {
            const int FirstRow = 4;
            
            var sheet = workBook.GetSheetAt(2);
            
            FillHeaderThirdSheet(sheet);

            var curRow = 0;

            for (var position = 0; position < reportDataCloseInst.Count; position++)
            {
                curRow = FirstRow + position;
                var reportRow = reportDataCloseInst.ElementAt(position);
                NpoiHelper.SetCellValue(sheet, curRow, 0, position + 1).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 1, reportRow.Name).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 2, reportRow.InstitutionCount).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 3, reportRow.Count - reportRow.NotCreated).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 4, reportRow.Created).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 5, reportRow.UnderConsideration).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 6, reportRow.OnEditing).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 7, reportRow.Finished).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 8, reportRow.Exported).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, curRow, 9, reportRow.PresenceOfCompleted).CellStyle = cellStyle;
                if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID)
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 10, reportRow.NotInspectionActivity).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 11, reportRow.NotInspectionActivityFinished).CellStyle = cellStyle;
                }

                if (partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                {
                    NpoiHelper.SetCellValue(sheet, curRow, 10, reportRow.NotBring).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 11, reportRow.NotBringFinished).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, curRow, 12, reportRow.GovernmentCountNotCreatedDocs).CellStyle = cellStyle;
                }
            }

            if (reportDataCloseInst.Any())
            {
                curRow++;
                NpoiHelper.SetCellValue(sheet, curRow, 1, "Всего").CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 2, "SUM(C{0}:C{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 3, "SUM(D{0}:D{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 4, "SUM(E{0}:E{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 5, "SUM(F{0}:F{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 6, "SUM(G{0}:G{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 7, "SUM(H{0}:H{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 8, "SUM(I{0}:I{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                NpoiHelper.SetCellFormula(sheet, curRow, 9, "SUM(J{0}:J{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID || partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
                {
                    NpoiHelper.SetCellFormula(sheet, curRow, 10, "SUM(K{0}:K{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 11, "SUM(L{0}:L{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                    NpoiHelper.SetCellFormula(sheet, curRow, 12, "SUM(M{0}:M{1})".FormatWith(FirstRow + 1, curRow)).CellStyle = cellStyle;
                }
            }
        }

        /// <summary>
        /// Формирование шапки 3й страницы
        /// </summary>
        private void FillHeaderThirdSheet(HSSFSheet sheet)
        {
            if (partDoc.ID == FX_FX_PartDoc.InfAboutControlActionsDocTypeID)
            {
                NpoiHelper.SetCellValue(sheet, 2, 10, Header2).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 9).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 10, "10").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 9).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 11, Header3).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 10).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 11, "11").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 10).CellStyle;
            }

            if (partDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID)
            {
                NpoiHelper.SetCellValue(sheet, 2, 10, Header4).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 9).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 10, "11").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 9).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 11, Header5).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 10).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 11, "12").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 10).CellStyle;
                NpoiHelper.SetCellValue(sheet, 2, 12, Header6).CellStyle = NpoiHelper.GetCellByXy(sheet, 2, 10).CellStyle;
                NpoiHelper.SetCellValue(sheet, 3, 12, "13").CellStyle = NpoiHelper.GetCellByXy(sheet, 3, 10).CellStyle;
            }

            var header = Header.FormatWith(GetTypeDoc(partDoc.ID), partDoc.ID != FX_FX_PartDoc.PassportDocTypeID ? "за {0} год".FormatWith(year) : string.Empty);

            NpoiHelper.SetCellValue(sheet, 0, 0, header);

            NpoiHelper.SetCellValue(sheet, 1, 0, string.Concat(Header1.FormatWith(string.Format("{0:dd.MM.yyyy}", reportDate)), " Для закрытых учреждений"));

            NpoiHelper.SetCellValue(sheet, 2, 1, isGrbs ? "ГРБС" : "ППО");
        }

        private string GetTypeDoc(int typeDoc)
        {
            var result = string.Empty;
            switch (typeDoc)
            {
                case FX_FX_PartDoc.PassportDocTypeID:
                    result = "паспорта учреждения";
                    break;
                case FX_FX_PartDoc.StateTaskDocTypeID:
                    result = "государственного (муниципального) задания";
                    break;
                case FX_FX_PartDoc.PfhdDocTypeID:
                    result = "плана ФХД";
                    break;
                case FX_FX_PartDoc.SmetaDocTypeID:
                    result = "информации о бюджетной смете";
                    break;
                case FX_FX_PartDoc.ResultsOfActivityDocTypeID:
                    result = "информации о результатах деятельности и об использовании имущества";
                    break;
                case FX_FX_PartDoc.InfAboutControlActionsDocTypeID:
                    result = "информации о контрольных мероприятиях";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    result = "отчета по приносящей доход деятельности (ф. 0503137)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    result = "баланса (ф. 0503730)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    result = "отчета о результатах (ф. 0503721)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    result = "отчета об исполнении ПФХД (ф. 0503737)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    result = "баланса (ф. 0503130)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    result = "отчета о результатах (ф. 0503121)";
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    result = "отчета об исполнении бюджета (ф. 0503127)";
                    break;
            }

            return result;
        }
    }
}