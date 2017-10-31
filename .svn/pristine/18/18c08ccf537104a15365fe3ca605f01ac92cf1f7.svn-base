using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views;
using Krista.FM.RIA.Extensions.FO51PassportMO.Services;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Controllers
{
    public class FO51FormSborController : SchemeBoundController
    {
        private readonly IFO51Extension extension;
        private readonly IFactsCheckDefectsService factsCheckDefectsService;
        private readonly IControlService controlService;
        private readonly IFactsPassportMOSaveService factsPassportMOSaveService; 
        private readonly ILinqRepository<F_Marks_PassportMO> factsPassportRepository;
        private readonly IFactsPassportMOService factsService; 
        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;
        private readonly IProgressManager progressManager;
        private readonly IRepository<FX_State_PassportMO> stateRepository;

        /// <summary>
        /// Список месяцев.
        /// </summary>
        private readonly List<string> months = new List<string>(12)
        {
            " января", 
            " февраля", 
            " марта", 
            " апреля", 
            " мая", 
            " июня", 
            " июля", 
            " августа", 
            " сентября", 
            " октября", 
            " ноября", 
            " декабря"
        };
        
        public FO51FormSborController(
            IFO51Extension extension,
            IProgressManager progressManager,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            ILinqRepository<F_Marks_PassportMO> factsPassportRepository,
            IFactsPassportMOSaveService factsPassportMOSaveService,
            IRepository<FX_State_PassportMO> stateRepository,
            IFactsPassportMOService factsService,
            IFactsCheckDefectsService factsCheckDefectsService,
            IControlService controlService)
        {
            this.extension = extension;
            this.progressManager = progressManager;
            this.marksPassportRepository = marksPassportRepository;
            this.factsPassportRepository = factsPassportRepository;
            this.factsPassportMOSaveService = factsPassportMOSaveService;
            this.stateRepository = stateRepository;
            this.factsService = factsService;
            this.factsCheckDefectsService = factsCheckDefectsService;
            this.controlService = controlService;
        }

        /// <summary>
        /// Чтение данных по показателям.
        /// </summary>
        /// <param name="markId">Идентификатор корневого показателя.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <returns>Данные формы сбора.</returns>
        public AjaxStoreExtraResult Read(int markId, int periodId, int regionId)
        {
            try
            {
                var region = extension.GetActualRegion(periodId, regionId);
                if (region == null && regionId != FO51Extension.RegionsGO && 
                    regionId != FO51Extension.RegionsMR && regionId != FO51Extension.RegionsAll)
                {
                    throw new Exception("Регион не определен");
                }

                var year = periodId / 10000;
                var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
                if (source == null)
                {
                    throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору".FormatWith(year));
                }

                var isFictRegion = region != null && 
                    region.RefBridgeRegions != null && 
                    region.RefBridgeRegions.ID == FO51Extension.RegionFictID;
                var resultFacts = factsService.GetPassportMOFacts(
                    markId, 
                    periodId, 
                    region == null ? regionId : region.ID, 
                    source.ID, 
                    isFictRegion);

                return new AjaxStoreExtraResult(resultFacts, resultFacts.Count, String.Empty);
            }
            catch (Exception e)
            {
               Trace.TraceError("Ошибка чтения данных: {0}", e.Message);
               return new AjaxStoreExtraResult(new List<string>(), 0, "Ошибка чтения данных: {0}".FormatWith(e.Message));
            }
        }

        /// <summary>
        /// Чтение данных по показателям за пред. месяц от указанного.
        /// </summary>
        /// <param name="markId">Идентификатор корневого показателя.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <returns>Список данных по показателям за пред. месяц.</returns>
        public AjaxStoreExtraResult ReadCopy(int markId, int periodId, int regionId)
        {
            var year = periodId / 10000;
            var month = (periodId / 100) % 100;
            if (month > 1)
            {
                month--;
            }
            else
            {
                year--;
                month = 12;
            }

            return Read(markId, (year * 10000) + (month * 100), regionId);
        }

        /// <summary>
        /// Обработчик на открытие формы проверки данных.
        /// </summary>
        /// <returns>Представление с формой проверки данных.</returns>
        public ActionResult Book(int periodId, int regionId)
        {
            var region = extension.GetActualRegion(periodId, regionId);

            var view = new DefectsView(extension, periodId, region);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", view);
        }

        /// <summary>
        /// Обработчик на открытие формы контроля данных ОГВ.
        /// </summary>
        /// <returns>Представление с формой проверки данных.</returns>
        public ActionResult Control(int periodId, int regionId)
        {
            var region = extension.GetActualRegion(periodId, regionId);

            var view = new ControlForOGVView(extension, periodId, region);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", view);
        }

        [Transaction]
        public RestResult ChangeState(int periodId, int state, int regionId)
        {
            try
            {
                progressManager.SetCompleted("Проверка наличия источника данных...", 0.05);
                var year = periodId / 10000;
                var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
                if (source == null)
                {
                    throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору".FormatWith(year));
                }

                var sourceId = source.ID;

                progressManager.SetCompleted("Поиск района сопоставимого...", 0.1);

                // Изменилось состояние, поэтому 
                // по выбранному периоду и выбранному региону должна произойти смена состояния.
                var newState = stateRepository.Get(state);
                D_Regions_Analysis regionToUse = null;
                switch (extension.UserGroup)
                {
                    case FO51Extension.GroupOGV:
                        regionToUse = extension.GetActualRegion(periodId, regionId);
                        break;
                    case FO51Extension.GroupMo:
                        regionToUse = extension.GetActualRegion(periodId, extension.User.RefRegion.Value);
                        break;
                }

                if (regionToUse == null)
                {
                    throw new Exception("Отсутствует район по источнику на {0} год".FormatWith(periodId / 10000));
                }

                progressManager.SetCompleted("Контроль дефицит/профицит...", 0.2);

                // Контроль дефицит\профицит для МО при отправке на рассмотрение.
                if (state == FO51Extension.StateConsider && extension.UserGroup == FO51Extension.GroupMo)
                {
                    var errorMsg = CheckDeficit(periodId, regionToUse.ID, source);
                    if (errorMsg != null)
                    {
                        throw new Exception("По показателю '{0}' данные не прошли контроль на дефицит/профицит.".FormatWith(errorMsg));
                    }
                }
                
                // Если нужно отправить на рассмотрение, проверка на отклонения с отчетностью.
                if (state == 2 && extension.UserGroup == FO51Extension.GroupMo && 
                    (regionToUse.RefBridgeRegions == null || 
                    regionToUse.RefBridgeRegions.ID != FO51Extension.RegionFictID))
                {
                    progressManager.SetCompleted("Сверка с месячной отчетностью...", 0.2);
                    var result = factsCheckDefectsService.GetDefects(
                        periodId, 
                        regionToUse, 
                        true);
                    progressManager.SetCompleted(0.4);
                    if (!result.IsCorrect)
                    {
                        throw new Exception("Есть отклонения от показателей ежемесячной отчетности. Форма не может быть отправлена на рассмотрение");
                    }
                }

                progressManager.SetCompleted("Изменение состояния данных...", 0.4);

                factsService.SaveStateForFacts(periodId, state, regionToUse.ID, sourceId);

                progressManager.SetCompleted("Завершение выполнения операции...", 0.99);

                return new RestResult
                {
                    Success = true,
                    Message = "Состояние изменено",
                    Data = new List<int>()
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                Trace.TraceError("Ошибка изменения состояния данных: {0}", e.Message);
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public AjaxStoreResult CheckIfAllAccepted(int periodId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            var source = extension.DataSourcesFO51
                .FirstOrDefault(x => x.Year.Equals((periodId / 10000).ToString()));
            if (source == null)
            {
                result.SaveResponse.Message = FO51Extension.StateEdit.ToString();
                result.SaveResponse.Success = false;
                return result;
            }

            result.SaveResponse.Success = true;
            var regions = extension.GetRegions(periodId);
            foreach (var region in regions)
            {
                if (factsService.GetStateId(periodId, region.ID, source.ID) != FO51Extension.StateAccept)
                {
                    result.SaveResponse.Message = FO51Extension.StateEdit.ToString();
                    return result;
                }
            }

            result.SaveResponse.Message = FO51Extension.StateEdit.ToString();
            return result;
        }
        
        public ActionResult Defects(int periodId, int regionId)
        {
            try
            {
                var region = extension.GetActualRegion(periodId, regionId);
                if (region == null)
                {
                    throw new FO51PassportException("Отсутствует район по источнику на {0} год".FormatWith(periodId / 10000));
                }

                var data = factsCheckDefectsService.GetDefects(periodId, region, false);
                foreach (var defect in data.DefectsList)
                {
                    var name = defect.Name;
                    name = name.Replace(", в том числе:", String.Empty);
                    name = name.Replace(", в том числе", String.Empty);
                    var dot = name.IndexOf(". ");
                    if (dot > 0)
                    {
                        name = name.Remove(0, dot + 2);
                    }

                    defect.Name = name;
                }

                if (!data.IsCorrect)
                {
                    data.ErrorMsg += " При сверке данных выявлены отклонения показателей. Необходимо внести исправления";
                }

                return new AjaxStoreExtraResult(data.DefectsList, data.DefectsList.Count, data.ErrorMsg);
            }
            catch (FO51PassportException e)
            {
                return new AjaxStoreExtraResult(new List<object>(), 0, e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка получения отклонений показателей: {0}", e.Message);
                return new AjaxStoreExtraResult(new List<object>(), 0, "Server failed");
            }
        }

        public ActionResult ControlDefects(int periodId, int regionId)
        {
            try
            {
                var region = extension.GetActualRegion(periodId, regionId);
                if (region == null)
                {
                    throw new FO51PassportException("Отсутствует район по источнику на {0} год".FormatWith(periodId / 10000));
                }

                var data = controlService.GetDefects(periodId, region);
                
                return new AjaxStoreExtraResult(data, data.Count, String.Empty);
            }
            catch (FO51PassportException e)
            {
                return new AjaxStoreExtraResult(new List<object>(), 0, e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка получения данных для контроля показателей: {0}", e.Message);
                return new AjaxStoreExtraResult(new List<object>(), 0, "Server failed");
            }
        }

        public AjaxStoreResult GetState(int periodId, int regionId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            var source = extension.DataSourcesFO51
                .FirstOrDefault(x => x.Year.Equals((periodId / 10000).ToString()));

            var region = extension.GetActualRegion(periodId, regionId);
            
            result.SaveResponse.Success = source != null;
            result.SaveResponse.Message = (source == null)
                            ? "-1"
                            : factsService.GetStateId(periodId, region.ID, source.ID).ToString();
            return result;
        }
        
        public AjaxStoreResult ReportDataExists(int periodId, int regionId)
        {
            var regionToUse = extension.GetActualRegion(periodId, regionId);
            var checkResult = factsCheckDefectsService.ReportDataExists(periodId, regionToUse);
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = true;
            const string ResultTrue = "1";
            const string ResultFalse = "2";
            result.SaveResponse.Message = checkResult ? ResultTrue : ResultFalse;
            return result;
        }

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult SaveMesOtch(int periodId, int regionId, int parentMarkId)
        {
            var handledError = false;
            try
            {
                progressManager.SetCompleted("Подготовка к получению данных...", 0.05);
                var year = periodId / 10000;

                var region = extension.GetActualRegion(periodId, regionId);
                if (region == null)
                {
                    handledError = true;
                    throw new Exception("Отсутствует район по источнику на {0} год".FormatWith(year));
                }

                var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
                if (source == null)
                {
                    handledError = true;
                    throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору".FormatWith(year));
                }

                var parentMark = marksPassportRepository.FindOne(parentMarkId);
                if (parentMark != null)
                {
                    var periodForYear = (year * 10000) + 1;
                    var periodLastYear = periodForYear - 10000;
                    var month = (periodId / 100) % 100;
                    try
                    {
                        factsPassportMOSaveService.SaveMesOtchReCalc(
                            region,
                            source,
                            periodId,
                            year,
                            month,
                            periodForYear,
                            periodLastYear,
                            parentMark);
                    }
                    catch (Exception)
                    {
                        return new RestResult
                        {
                            Success = true,
                            Message = "Ежемесячный отчет об исполнении бюджета муниципального образования за выбранный период не заполнен.",
                            Data = null
                        };
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Данные месячной отчетности сохранены",
                    Data = null
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                Trace.TraceError("Ошибка сохранения данных месячной отчетности: {0}", e.Message);
                var msg = handledError ? e.Message : "Ошибка сохранения данных месячной отчетности: {0}".FormatWith(e.Message);
                return new RestResult { Success = true, Message = msg };
            }
        }
        
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Save(int periodId, int parentMarkId, int regionId, int? state)
        {
            var handledError = false;
            try
            {
                var year = periodId / 10000;

                var region = extension.GetActualRegion(periodId, regionId);
                if (region == null)
                {
                    handledError = true;
                    throw new Exception("Отсутствует район по источнику на {0} год".FormatWith(year));
                }

                var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
                if (source == null)
                {
                    handledError = true;
                    throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору".FormatWith(year));
                }

                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);
                var parentMark = marksPassportRepository.FindOne(parentMarkId);
                if (parentMark != null)
                {
                    var periodForYear = (year * 10000) + 1;
                    var periodLastYear = periodForYear - 10000;
                    var month = (periodId / 100) % 100;

                    // Есть измененные данные.
                    if (dataSet.ContainsKey("Updated"))
                    {
                        // Сохраняем их.
                        var updatedRecords = dataSet["Updated"];
                        factsPassportMOSaveService.SaveUpdatedRecordsAndCalc(
                            updatedRecords, 
                            region, 
                            source, 
                            periodId, 
                            year, 
                            month, 
                            periodForYear, 
                            periodLastYear, 
                            parentMark);
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Данные по разделу '{0}' сохранены".FormatWith(parentMark.Name),
                    Data = dataSet
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                Trace.TraceError("Ошибка сохранения формы сбора: {0}", e.Message);
                var msg = handledError ? e.Message : "Ошибка сохранения.";
                return new RestResult { Success = true, Message = msg };
            }
        }

        public ActionResult ShowPassportMo(int year)
        {
            var passportMOView = new EditPassportMOView(
                extension,
                Resolver.Get<ILinqRepository<D_Regions_Analysis>>(),
                Resolver.Get<ILinqRepository<D_Marks_PassportMO>>(),
                year);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", passportMOView);
        }

        public ActionResult ShowPassportOGV(int year)
        {
            var passportOGVView = new CheckPassportMOView(extension);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", passportOGVView);
        }

       private string CheckDeficit(int periodId, int regionId, DataSources source)
        {
            var month = (periodId / 100) % 100;
            var year = periodId / 10000;
            var yearPeriod = (year * 10000) + 1;
            var prevYearPeriod = ((year - 1) * 10000) + 1;
           
            var markIncome = marksPassportRepository.FindAll()
                            .FirstOrDefault(x => x.RefTypeMark.ID == 1 && x.SourceID == source.ID && x.ParentID == null && x.Name.Equals("Доходы"));
            ////Корневой показатель по Расходам не берем, т.к. для сравнения нужен конкретно показатель "ИТОГО РАСХОДОВ" (со второго уровня).
            var markCosts = marksPassportRepository.FindAll()
                            .FirstOrDefault(x => x.RefTypeMark.ID == 1 && x.SourceID == source.ID && x.Name.Equals("ИТОГО РАСХОДОВ"));
            var markSources = marksPassportRepository.FindAll()
                            .FirstOrDefault(x => x.RefTypeMark.ID == 1 && x.SourceID == source.ID && x.ParentID == null && x.Name.Equals("Источники финансирования дефицита бюджета"));

            var factsAll = factsPassportRepository.FindAll()
                 .Where(x =>
                        x.RefPasRegions.ID == regionId &&
                        x.SourceID == source.ID &&
                        (x.RefPasPeriod.ID == yearPeriod || 
                            x.RefPasPeriod.ID == prevYearPeriod || 
                            x.RefPasPeriod.ID == periodId || 
                            (x.RefVar.ID == month && x.RefPasPeriod.ID / 10000 == year)) &&
                        (x.RefPassportMO.ID == markIncome.ID || x.RefPassportMO.ID == markCosts.ID || x.RefPassportMO.ID == markSources.ID))
                 .ToList();

            var factIncome = GetRecordFactsByMark(factsAll, markIncome.ID, month, year, yearPeriod, prevYearPeriod, periodId);
            var factCosts = GetRecordFactsByMark(factsAll, markCosts.ID, month, year, yearPeriod, prevYearPeriod, periodId);
            var factSources = GetRecordFactsByMark(factsAll, markSources.ID, month, year, yearPeriod, prevYearPeriod, periodId);

            if (month == 1)
            {
                if ((factIncome.OrigPlan ?? 0) - (factCosts.OrigPlan ?? 0) + (factSources.OrigPlan ?? 0) != 0)
                {
                    return "План на год (первоначально утвержденный)";
                }

                if ((factIncome.FactLastYear ?? 0) - (factCosts.FactLastYear ?? 0) + (factSources.FactLastYear ?? 0) != 0)
                {
                    return "Исполнено за отчетный год (по годовому отчету)";
                }

                if ((factIncome.RefinPlan ?? 0) - (factCosts.RefinPlan ?? 0) + (factSources.RefinPlan ?? 0) != 0)
                {
                    return "Уточненный план на год (по годовому отчету)";
                }
            }

            if ((factIncome.FactPeriod ?? 0) - (factCosts.FactPeriod ?? 0) + (factSources.FactPeriod ?? 0) != 0)
            {
                return "Исполнено за отчетный месяц";
            }

            if ((factIncome.PlanPeriod ?? 0) - (factCosts.PlanPeriod ?? 0) + (factSources.PlanPeriod ?? 0) != 0)
            {
                return "Уточненный план на год (по месячному отчету)";
            }

            var scoreMoIncome = new[]
                                    {
                                        factIncome.ScoreMO1,
                                        factIncome.ScoreMO2,
                                        factIncome.ScoreMO3,
                                        factIncome.ScoreMO4,
                                        factIncome.ScoreMO5,
                                        factIncome.ScoreMO6,
                                        factIncome.ScoreMO7,
                                        factIncome.ScoreMO8,
                                        factIncome.ScoreMO9,
                                        factIncome.ScoreMO10,
                                        factIncome.ScoreMO11,
                                        factIncome.ScoreMO12,
                                    };
            var scoreMoCosts = new[]
                                    {
                                        factCosts.ScoreMO1,
                                        factCosts.ScoreMO2,
                                        factCosts.ScoreMO3,
                                        factCosts.ScoreMO4,
                                        factCosts.ScoreMO5,
                                        factCosts.ScoreMO6,
                                        factCosts.ScoreMO7,
                                        factCosts.ScoreMO8,
                                        factCosts.ScoreMO9,
                                        factCosts.ScoreMO10,
                                        factCosts.ScoreMO11,
                                        factCosts.ScoreMO12,
                                    };
            var scoreMoSources = new[]
                                    {
                                        factSources.ScoreMO1,
                                        factSources.ScoreMO2,
                                        factSources.ScoreMO3,
                                        factSources.ScoreMO4,
                                        factSources.ScoreMO5,
                                        factSources.ScoreMO6,
                                        factSources.ScoreMO7,
                                        factSources.ScoreMO8,
                                        factSources.ScoreMO9,
                                        factSources.ScoreMO10,
                                        factSources.ScoreMO11,
                                        factSources.ScoreMO12,
                                    };

            for (var indMonth = month; indMonth < 13; indMonth++)
            {
                if ((scoreMoIncome[indMonth - 1] ?? 0) - (scoreMoCosts[indMonth - 1] ?? 0) + (scoreMoSources[indMonth - 1] ?? 0) != 0)
                {
                    return "Оценка исполнения{0}".FormatWith(months[indMonth - 1]);
                }
            }

            return null;
        }

        private MarksPassportMOFact GetRecordFactsByMark(
            IEnumerable<F_Marks_PassportMO> factsAll, 
            int markId, 
            int month, 
            int year, 
            int periodYear, 
            int periodLastYear, 
            int periodId)
        {
            var model = new MarksPassportMOFact();

            // Получаем факты по показателю для региона пользователя.
            var facts = factsAll.Where(x => x.RefPassportMO.ID == markId);

            if (month == 1)
            {
                // Факты на текущий год.
                var factsOrig = facts.FirstOrDefault(x =>
                                            x.RefPasPeriod.ID == periodYear &&
                                            x.RefVar.Code == -1 &&
                                            x.OrigPlan != null);
                model.OrigPlan = factsOrig == null ? null : factsOrig.OrigPlan;

                // Факты на прошлый год.
                var factsLastYear = facts.FirstOrDefault(x =>
                                                x.RefPasPeriod.ID == periodLastYear &&
                                                x.RefVar.Code == -1 &&
                                                (x.FactLastYear != null || x.RefinPlan != 0));
                model.FactLastYear = factsLastYear == null ? null : factsLastYear.FactLastYear;
                model.RefinPlan = factsLastYear == null ? null : factsLastYear.RefinPlan;
            }

            // Факты на отчетный период.
            var factsYear = facts.FirstOrDefault(x => x.RefPasPeriod.ID == periodId && x.RefVar.Code == -1);
            model.FactPeriod = factsYear == null ? null : factsYear.FactPeriod;
            model.PlanPeriod = factsYear == null ? null : factsYear.PlanPeriod;

            // ОЦЕНКИ ИСПОЛНЕНИЯ ФЕВРАЛЯ, МАРТА, ...
            var scoreMO = new decimal?[13];
            for (var indMonth = month; indMonth <= 12; indMonth++)
            {
                var curPeriod = (year * 10000) + (indMonth * 100);

                // На данное поле накладывается соответствующий период 
                // (февраль, март…., какое присутствует в наименовании), 
                // а также накладывается соответствующий выбранному периоду вариант
                // из классификатора «Вариант.Паспорт МО (d.Variant.PassportMO)».
                var factsMonthVar = facts.FirstOrDefault(x =>
                                                x.RefPasPeriod.ID == curPeriod &&
                                                x.RefVar.Code == month);
                scoreMO[indMonth] = factsMonthVar == null ? null : factsMonthVar.ScoreMO;
            }

            model.ScoreMO1 = scoreMO[1];
            model.ScoreMO2 = scoreMO[2];
            model.ScoreMO3 = scoreMO[3];
            model.ScoreMO4 = scoreMO[4];
            model.ScoreMO5 = scoreMO[5];
            model.ScoreMO6 = scoreMO[6];
            model.ScoreMO7 = scoreMO[7];
            model.ScoreMO8 = scoreMO[8];
            model.ScoreMO9 = scoreMO[9];
            model.ScoreMO10 = scoreMO[10];
            model.ScoreMO11 = scoreMO[11];
            model.ScoreMO12 = scoreMO[12];

            return model;
        }

        private class FO51PassportException : Exception
        {
            public FO51PassportException(string messsage)
                : base(messsage)
            {
            }
        }
    }
}