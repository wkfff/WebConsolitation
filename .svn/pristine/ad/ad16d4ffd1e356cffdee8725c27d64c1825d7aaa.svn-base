using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Models;
using Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Views;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Controllers
{
    public class MeasuresController : SchemeBoundController
    {
        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IEO10Extension extension;

        /// <summary>
        /// Репозиторий задач/мероприятий
        /// </summary>
        private readonly IRepository<D_MissivePRF_GoalArrang> goalArrangRepository;

        /// <summary>
        /// Репозиторий источников
        /// </summary>
        private readonly IRepository<DataSources> sourceRepository;

        /// <summary>
        /// Репозиторий отчетов
        /// </summary>
        private readonly IRepository<D_MissivePRF_Relationships> relationshipsRepository;

        /// <summary>
        /// Репозиторий периодов
        /// </summary>
        private readonly IRepository<FX_Date_YearDayUNV> periodsRepository;

        /// <summary>
        /// Мастер-деталь: связь мероприятия и исполнителя
        /// </summary>
        private readonly IRepository<T_MissivePRF_Respons> masterDetalRepository;

        public MeasuresController(
            IEO10Extension extension,
            IRepository<D_MissivePRF_GoalArrang> goalArrangRepository,
            IRepository<DataSources> sourceRepository,
            IRepository<D_MissivePRF_Relationships> relationshipsRepository,
            IRepository<FX_Date_YearDayUNV> periodsRepository,
            IRepository<T_MissivePRF_Respons> masterDetalRepository)
        {
            this.extension = extension;
            this.goalArrangRepository = goalArrangRepository;
            this.sourceRepository = sourceRepository;
            this.relationshipsRepository = relationshipsRepository;
            this.periodsRepository = periodsRepository;
            this.masterDetalRepository = masterDetalRepository;
        }

        /// <summary>
        /// Чтение данных по мероприятиям
        /// </summary>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <returns>Список мероприятий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public AjaxStoreResult Read(int sourceId)
        {
            try
            {
                // Получение списка мероприятий для исполнителя (extension.Executer) на выбранный источник
                var data = masterDetalRepository.GetAll().Where(md =>
                                                                md.RefRespons != null &&
                                                                md.RefRespons.ID == extension.Executer.ID &&
                                                                md.RefGoal.SourceID == sourceId);

                // Результирующий список мероприятий
                var dataList = new List<MeasureModel>();
                if (data != null) 
                {
                    foreach (var md in data)
                    {
                        var goalArrang = md.RefGoal;

                        // формирование объекта мероприятия согласно модели
                        var measure = new MeasureModel
                                          {
                                              ID = goalArrang.ID,
                                              MeasureId = goalArrang.ID,
                                              MeasureName = goalArrang.Name,
                                              SpaceDisch = goalArrang.SpaceDisch,
                                              TaskId = goalArrang.ParentID
                                          };

                        // Если указана задача, к которой принадлежит мероприятие, подтягиваем ее название
                        if (measure.TaskId != null)
                        {
                            measure.TaskName = goalArrangRepository.Get((int)measure.TaskId).Name;
                        }

                        // получаем список отчетов по мероприятию (должен быть один) - берем первый или значение по умолчанию
                        var report = relationshipsRepository.GetAll()
                            .Where(r => r.RefGoalArrang.ID == measure.MeasureId).FirstOrDefault();

                        // если отчет существует
                        if (report != null)
                        {
                            // устанавливаем значение идентификатора отчета для мероприятия
                            measure.RelationshipsId = report.ID;

                            // идентификатор периода (по дате формирования отчета)
                            var periodId = report.RefPeriod.ID;
                            measure.RelationshipsPeriodId = periodId;

                            // даты периода
                            var date = new DateTime(periodId / 10000, (periodId / 100) % 100, periodId % 100);
                            measure.RelationshipsDate = date;

                            // текст отчета
                            measure.RelationshipsReport = report.Report;
                        }

                        dataList.Add(measure);
                    }
                }

                // сортируем список мероприятий по задаче
                dataList.OrderBy(x => x.TaskId);

                return new AjaxStoreResult 
                {  
                    ResponseFormat = StoreResponseFormat.Load, 
                    Data = dataList,
                    Total = dataList.Count
                };
            }
            catch (Exception)
            {
                return new AjaxStoreResult
                {
                    ResponseFormat = StoreResponseFormat.Load,
                    Data = new List<string>(),
                    Total = 0
                };
            }
        }

        /// <summary>
        /// Сохранение измений по отчетам мероприятий
        /// </summary>
        /// <returns>Результат сохранения</returns>
        [Transaction]
        public ActionResult Save()
        {
            try
            {
                // получаем данные из интерфейса
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                // для измененных записей
                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        // если отчет существовал - изменяем его, иначе - создаем новый
                        var report = new D_MissivePRF_Relationships { RefExecut = extension.Executer };
                        var reportId = record["RelationshipsId"];
                        if (reportId != null && Convert.ToInt32(reportId) > 0)
                        {
                            report.ID = Convert.ToInt32(reportId);
                        }

                        // проставляем идентификатор мероприятия
                        var measureId = Convert.ToInt32(record["MeasureId"]);
                        var measure = goalArrangRepository.Get(measureId);
                        if (measure != null)
                        {
                            report.RefGoalArrang = measure;

                            // проставляем дату
                            var date = Convert.ToDateTime(record["RelationshipsDate"]);
                            var periodID = (date.Year * 10000) + (date.Month * 100) + date.Day;
                            var period = periodsRepository.Get(periodID) ??
                                         periodsRepository.Get((DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100) + DateTime.Today.Day);

                            report.RefPeriod = period;

                            // проставляем текст отчета
                            var text = record["RelationshipsReport"].ToString();
                            if (text.Length > 4096)
                            {
                                text = text.Substring(0, 4096);
                            }

                            report.Report = text;
                            report.RowType = 0;

                            // сохраняем изменения
                            relationshipsRepository.Save(report);
                        }
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Показатели обновлены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Получение списка годов, на которые есть классификатор Послание ПРФ. Задачи Мероприятия (d.MissivePRF.GoalArrang)
        /// </summary>
        /// <returns>Список годов</returns>
        public AjaxStoreResult Years()
        {
            try
            {
                // Источники: "ЭО\0001"
                var sources =
                    from s in sourceRepository.GetAll()
                    where s.SupplierCode == "ЭО" && s.DataCode == 1
                    orderby s.Year
                    select new
                               {
                                   SourceID = s.ID,
                                   s.Year
                               };

                sources = sources.Distinct().ToList();

                return new AjaxStoreResult { Data = sources, ResponseFormat = StoreResponseFormat.Load };
            }
            catch (Exception)
            {
                return new AjaxStoreResult { Data = new List<string>(), ResponseFormat = StoreResponseFormat.Load };
            }
        }

        public ActionResult Expand(int measureId)
        {
            return new AjaxResult
            {
                Script = MeasuresReportsControl.CreateGridScript(measureId)
            };
        }

        public ActionResult ReadReports(int measureId)
        {
            var reports =
                relationshipsRepository.GetAll().Where(x => x.RefGoalArrang.ID == measureId).ToList().OrderBy(
                    x => x.RefPeriod.ID);
            var reportObjects = (from r in reports
                                 let period = r.RefPeriod.ID
                                 let month = (period / 100) % 100
                                 select new
                                            {
                                                r.ID,
                                                Date =
                                     "{0}.{1}{2}.{3}".FormatWith(period / 10000, month < 10 ? "0" : string.Empty, month, period % 100),
                                                r.Report
                                            }).ToList();
            return new AjaxStoreResult
                       {
                           ResponseFormat = StoreResponseFormat.Load,
                           Data = reportObjects,
                           Total = reportObjects.Count
                       };
        }

        [HttpPost]
        [Transaction]
        public RestResult SaveReports(string data)
        {
            var returnResult = new List<Dictionary<string, object>>();
            try
            {
                var dataHandler = new StoreDataHandler(data);
                var dataSet = dataHandler.ObjectData<Dictionary<string, object>>();
                foreach (var updated in dataSet.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var report = relationshipsRepository.Get(id);
                    if (report != null)
                    {
                        report.Report = updated["Report"].ToString();
                        relationshipsRepository.Save(report);
                        returnResult.Add(updated);
                    }
                }
            }
            catch (Exception)
            {
            }

            return new RestResult
            {
                Success = true,
                Message = "Сохранено",
                Data = returnResult
            };
        }

                /// <summary>
        /// Обработчик на открытие формы для добавления нового отчета
        /// </summary>
        /// <returns>Представление с формой добавления ого отчета</returns>
        public ActionResult Book(int measureId)
        {
            var reportForm = new ReportFormView(measureId);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", reportForm);
        }

        /// <summary>
        /// Сохранение отчета
        /// </summary>
        /// <param name="measureId">Идентификатор исполнителя</param>
        /// <param name="values">Реквизиты отчета</param>
        /// <returns>Результат сохранения отчета</returns>
        [HttpPost]
        [Transaction]
        public AjaxStoreResult SaveReport(int measureId, FormCollection values)
        {
            var result = new AjaxStoreResult();
            try
            {
                var date = Convert.ToDateTime(values["RefPeriod"]);
                var periodId = (date.Year * 10000) + (date.Month * 100) + date.Day;
                var reportText = values["Report"];

                var report = new D_MissivePRF_Relationships
                                 {
                                     RefPeriod = periodsRepository.Get(periodId),
                                     RowType = 0,
                                     Report = reportText,
                                     RefExecut = extension.Executer,
                                     RefGoalArrang = goalArrangRepository.Get(measureId)
                                 };

                relationshipsRepository.Save(report);

                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = report.ID.ToString();
                result.Data = values;

                return result;
            }
            catch (Exception e)
            {
                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = false;
                result.SaveResponse.Message = e.Message;

                return result;
            }
        }
    }
}
