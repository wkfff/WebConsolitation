using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ILinqRepository<D_ExcCosts_Finances> financeRepository;
        private readonly ILinqRepository<F_ExcCosts_Finance> factRepository;
        private readonly IAdditionalService additionalService;
        private readonly IDatasourceService datasourceService;

        public FinanceService(
                              ILinqRepository<D_ExcCosts_Finances> financeRepository, 
                              ILinqRepository<F_ExcCosts_Finance> factRepository,
                              IAdditionalService additionalService,
                              IDatasourceService datasourceService)
        {
            this.financeRepository = financeRepository;
            this.factRepository = factRepository;
            this.additionalService = additionalService;
            this.datasourceService = datasourceService;
        }

        public DataTable GetFinanceListTable(D_ExcCosts_ListPrg program)
        {
            DataTable table = new DataTable();

            IList<int> years = program.GetYears();

            int sourceId = datasourceService.GetDefaultDatasourceId();

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("ActionName", typeof(string));
            table.Columns.Add("ActionId", typeof(int));
            table.Columns.Add("ActionIdOld", typeof(int));
            table.Columns.Add("FinSourceName", typeof(string));
            table.Columns.Add("FinSourceId", typeof(int));
            table.Columns.Add("FinSourceIdOld", typeof(int));
            table.Columns.Add("Total", typeof(decimal));
            foreach (int year in years)
            {
                table.Columns.Add(string.Format("Year{0}", year), typeof(decimal));
            }

            // Выбираем всё из таблицы фактов по данному проекту только для нужного типа индикаторов, и упорядочиваем по индикаторам и годам
            var data = from f in factRepository.FindAll()
                       where f.RefEvent.RefTask.RefGoal.RefProg == program
                             && f.SourceID == sourceId
                       orderby f.RefEvent.ID, f.RefFin.ID, f.RefUNV.ID
                       select f;

            // Формируем транспонированную таблицу
            DataRow row = null;
            int oldActionId = 0;
            int oldRefFinId = 0;
            foreach (F_ExcCosts_Finance factRow in data)
            {
                // Если новая пара индикаторов - то нужно начинать формировать новую строку в таблицу
                if (oldActionId != factRow.RefEvent.ID || oldRefFinId != factRow.RefFin.ID)
                {
                    // Добавляем ранее сформированную строку
                    if (oldActionId != 0 || oldRefFinId != 0)
                    {
                        table.Rows.Add(row);
                    }

                    row = table.NewRow();
                    row["ID"] = factRow.ID;
                    row["ActionName"] = factRow.RefEvent.Name;
                    row["ActionId"] = factRow.RefEvent.ID;
                    row["ActionIdOld"] = factRow.RefEvent.ID;
                    row["FinSourceName"] = factRow.RefFin.Name;
                    row["FinSourceId"] = factRow.RefFin.ID;
                    row["FinSourceIdOld"] = factRow.RefFin.ID;
                    row["Total"] = 0;

                    oldActionId = factRow.RefEvent.ID;
                    oldRefFinId = factRow.RefFin.ID;
                }

                string columnName = string.Format("Year{0}", factRow.RefUNV.GetYear());
                //// if - для  страховки от наличия неудаленных данных в таблице фактов - такой колонки в таблице нет, а даные для неё есть...
                if (table.Columns.Contains(columnName))
                {
                    if (factRow.Target == null)
                    {
                        row[columnName] = DBNull.Value;
                    }
                    else
                    {
                        row[columnName] = factRow.Target;
                    }
                }

                row["Total"] = (decimal)row["Total"] + (factRow.Target ?? 0);
            }

            if (row != null)
            {
                table.Rows.Add(row);
            }

            return table;
        }

        public IList GetAllFinSourcesListForLookup()
        {
            var data = from p in this.financeRepository.FindAll()
                       select new { p.ID, p.Name };
            return data.ToList();
        }

        public DataTable GetReportTable(D_ExcCosts_ListPrg program, int year)
        {
            DataTable table = new DataTable();

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("TargetName", typeof(string));
            table.Columns.Add("TaskName", typeof(string));
            table.Columns.Add("ActionName", typeof(string));
            table.Columns.Add("ActionOwnerName", typeof(string));
            table.Columns.Add("FinsourceName", typeof(string));

            table.Columns.Add("Month1", typeof(decimal));
            table.Columns.Add("Month2", typeof(decimal));
            table.Columns.Add("Month3", typeof(decimal));
            table.Columns.Add("Month4", typeof(decimal));
            table.Columns.Add("Month5", typeof(decimal));
            table.Columns.Add("Month6", typeof(decimal));
            table.Columns.Add("Month7", typeof(decimal));
            table.Columns.Add("Month8", typeof(decimal));
            table.Columns.Add("Month9", typeof(decimal));
            table.Columns.Add("Month10", typeof(decimal));
            table.Columns.Add("Month11", typeof(decimal));
            table.Columns.Add("Month12", typeof(decimal));

            table.Columns.Add("Quarter1", typeof(decimal));
            table.Columns.Add("Quarter2", typeof(decimal));
            table.Columns.Add("Quarter3", typeof(decimal));
            table.Columns.Add("Quarter4", typeof(decimal));

            table.Columns.Add("HalfYear1", typeof(decimal));
            table.Columns.Add("HalfYear2", typeof(decimal));

            table.Columns.Add("FactYear", typeof(decimal));
            table.Columns.Add("PlanYear", typeof(decimal));
            table.Columns.Add("RatioYear", typeof(decimal));
            
            table.Columns.Add("ActionEffect", typeof(string));
            table.Columns.Add("ActionFailReason", typeof(string));
            
            int sourceIdPlan = datasourceService.GetDefaultDatasourceId();
            int sourceIdFact = datasourceService.GetFactDatasourceId();

            var financesPlanList = from t in factRepository.FindAll()
                          where t.RefEvent.RefTask.RefGoal.RefProg == program
                                && t.RefUNV == additionalService.GetRefYear(year)
                                && t.SourceID == sourceIdPlan
                          orderby t.RefEvent.RefTask.RefGoal.Name, t.RefEvent.RefTask.Name, t.RefEvent.Name, t.RefFin.Name
                          select new
                                {
                                     ID = t.ID,
                                     TargetName = t.RefEvent.RefTask.RefGoal.Name,
                                     TaskName = t.RefEvent.RefTask.Name,
                                     ActionName = t.RefEvent.Name,
                                     ActionId = t.RefEvent.ID,
                                     ActionOwnerName = t.RefEvent.RefCreators.Name,
                                     ActionEffect = t.RefEvent.ResultEv,
                                     ActionFailReason = t.RefEvent.ReasonFail,
                                     FinsourceName = t.RefFin.Name,
                                     FinsourceId = t.RefFin.ID,
                                     PlanYear = t.Target
                                 };
            
            //// TODO: отображение строк в зависимости от владельца

            DataRow row = null;
            
            var monthes = new List<object>();
            for (int month = 1; month <= 12; month++)
            {
                monthes.Add((year * 10000) + (month * 100));
            }

            foreach (var plan in financesPlanList)
            {
                row = table.NewRow();
                row["ID"] = plan.ID;
                row["TargetName"] = plan.TargetName;
                row["TaskName"] = plan.TaskName;
                row["ActionName"] = plan.ActionName;
                row["ActionOwnerName"] = plan.ActionOwnerName;
                row["FinsourceName"] = plan.FinsourceName;

                row["ActionEffect"] = plan.ActionEffect;
                row["ActionFailReason"] = plan.ActionFailReason;

                // Факты все по данному sourceId, мероприятию и источнику финансирования
                var factAll = factRepository.FindAll()
                                            .Where(f => f.SourceID == sourceIdFact 
                                                        && f.RefEvent.ID == plan.ActionId 
                                                        && f.RefFin.ID == plan.FinsourceId)
                                            .ToList();
                
                // Отрубаем нужный год и только данные за месяца
                var facts = from f in factAll
                            join m in monthes on f.RefUNV.ID equals m
                            select f;

                // Расписываем введенные факты по месяцам и суммируем
                decimal quarter1 = 0;
                decimal quarter2 = 0;
                decimal quarter3 = 0;
                decimal quarter4 = 0;
                decimal halfYear1 = 0;
                decimal halfYear2 = 0;
                decimal factYear = 0;
                foreach (var fact in facts)
                {
                    int month = fact.RefUNV.GetMonth();
                    var columnName = String.Format("Month{0}", month);

                    if (fact.Fact == null)
                    {
                        row[columnName] = DBNull.Value;
                    }
                    else
                    {
                        row[columnName] = fact.Fact;
                    }

                    switch (month)
                    {
                        case 1:
                        case 2:
                        case 3:
                            quarter1 += fact.Fact ?? 0;
                            halfYear1 += fact.Fact ?? 0;
                            factYear += fact.Fact ?? 0;
                            break;
                        case 4:
                        case 5:
                        case 6:
                            quarter2 += fact.Fact ?? 0;
                            halfYear1 += fact.Fact ?? 0;
                            factYear += fact.Fact ?? 0;
                            break;
                        case 7:
                        case 8:
                        case 9:
                            quarter3 += fact.Fact ?? 0;
                            halfYear2 += fact.Fact ?? 0;
                            factYear += fact.Fact ?? 0;
                            break;
                        case 10:
                        case 11:
                        case 12:
                            quarter4 += fact.Fact ?? 0;
                            halfYear2 += fact.Fact ?? 0;
                            factYear += fact.Fact ?? 0;
                            break;
                    }
                }

                row["Quarter1"] = quarter1;
                row["Quarter2"] = quarter2;
                row["Quarter3"] = quarter3;
                row["Quarter4"] = quarter4;

                row["HalfYear1"] = halfYear1;
                row["HalfYear2"] = halfYear2;
                row["FactYear"] = factYear;

                if (plan.PlanYear == null)
                {
                    row["PlanYear"] = DBNull.Value;
                    row["RatioYear"] = DBNull.Value;
                }
                else
                {
                    row["PlanYear"] = plan.PlanYear;

                    if (plan.PlanYear == 0)
                    {
                        row["RatioYear"] = DBNull.Value;
                    }
                    else
                    {
                        row["RatioYear"] = factYear * 100 / plan.PlanYear;
                    }
                }
                
                table.Rows.Add(row);
            }
            
            return table;
        }
        
        public D_ExcCosts_Finances GetFinSource(int id)
        {
            var entity = financeRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Источник финансирования не найден.");
            }

            return entity;
        }

        public void CreateFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource, Dictionary<int, decimal?> yearsValues)
        {
            if (action.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие не соответствует указанной программе");
            }

            int sourceId = datasourceService.GetDefaultDatasourceId();
            
            foreach (KeyValuePair<int, decimal?> yearValue in yearsValues)
            {
                if (yearValue.Value != null)
                {
                    var factEntity = new F_ExcCosts_Finance
                                     {
                                         SourceID = sourceId,
                                         TaskID = -1,
                                         Target = yearValue.Value,
                                         Fact = null,
                                         RefEvent = action,
                                         RefFin = finSource,
                                         RefUNV = additionalService.GetRefYear(yearValue.Key)
                                     };
                    factRepository.Save(factEntity);
                    factRepository.DbContext.CommitChanges();

                    // Страхуемся от дубликатов из параллельных сессий
                    ProtectionFromDuplicatesInFactData(factEntity);
                }
            }
        }

        public void UpdateFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource, Dictionary<int, decimal?> yearsValues)
        {
            if (action.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие не соответствует указанной программе");
            }

            int sourceId = datasourceService.GetDefaultDatasourceId();
            
            // Проходим по всем годам
            foreach (int year in action.RefTask.RefGoal.RefProg.GetYears())
            {
                var yearEntity = additionalService.GetRefYear(year);

                // Находим данные по указанным году и показателю
                var facts = from p in factRepository.FindAll()
                            where p.RefUNV == yearEntity
                                  && p.RefEvent == action
                                  && p.RefFin == finSource
                                  && p.SourceID == sourceId
                            select p;

                if (facts.Count() > 1)
                {
                    // Если дубликаты - ошибка
                    throw new DuplicateNameException(String.Format("При изменении данных обнаружен дубликат по году {0} для показателей {1} - {2}", year, action.Name, finSource.Name));
                }

                F_ExcCosts_Finance factEntityOld = facts.FirstOrDefault();
                
                decimal? data = yearsValues[year];

                if (factEntityOld == null && data == null)
                {
                    // Данных не было и не надо
                    continue;
                }
                else if (factEntityOld == null && (data != null))
                {
                    // Данные нужно создать
                    var factEntityNew = new F_ExcCosts_Finance
                    {
                        SourceID = sourceId,
                        TaskID = -1,
                        Target = data,
                        Fact = null,
                        RefEvent = action,
                        RefFin = finSource,
                        RefUNV = additionalService.GetRefYear(year)
                    };

                    // Сохраняем
                    factRepository.Save(factEntityNew);
                    factRepository.DbContext.CommitChanges();

                    // Страхуемся от дубликатов от параллельных сессий
                    ProtectionFromDuplicatesInFactData(factEntityNew);
                }
                else if (factEntityOld != null && data == null)
                {
                    // Данные нужно удалить
                    factRepository.Delete(factEntityOld);
                    factRepository.DbContext.CommitChanges();
                }
                else if (factEntityOld != null && (data != null))
                {
                    // Данные нужно изменить при необходимости
                    if (factEntityOld.Target != data)
                    {
                        factEntityOld.Target = data;
                        factRepository.Save(factEntityOld);
                        factRepository.DbContext.CommitChanges();
                    }
                }
                else
                {
                    throw new Exception("Недостижимая ветка кода!");
                }
            }
        }

        public void DeleteFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource)
        {
            if (action.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие не соответствует указанной программе");
            }

            int sourceId = datasourceService.GetDefaultDatasourceId();
            
            // Находим строки для удаления
            var rows = from p in factRepository.FindAll()
                       where p.RefEvent == action
                             && p.RefFin == finSource
                             && p.SourceID == sourceId
                       select p;
            foreach (var row in rows)
            {
                factRepository.Delete(row);
            }

            factRepository.DbContext.CommitChanges();
        }

        public void SaveReportFactData(int programId, int financePlanId, Dictionary<int, decimal?> monthFactList)
        {
            var userCredentials = (BasePrincipal)System.Web.HttpContext.Current.User;

            var planEntity = factRepository.FindOne(financePlanId);
            if (planEntity == null)
            {
                throw new KeyNotFoundException("Запись о плановом мероприятии/источнике_финансирования в таблице фактов отсутствует!");
            }

            if (planEntity.RefEvent.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие не соответствует указанной программе");
            }

            if (planEntity.RefEvent.RefCreators.Login != userCredentials.DbUser.Name 
                 && planEntity.RefEvent.RefTask.RefGoal.RefProg.RefCreators.Login != userCredentials.DbUser.Name)
            {
                throw new SecurityException(String.Format("Запрещено редактировать данное мероприятие: {0}", planEntity.RefEvent.Name));
            }

            int sourceIdFact = datasourceService.GetFactDatasourceId();
            int year = planEntity.RefUNV.GetYear();

            foreach (var monthFact in monthFactList)
            {
                FX_Date_YearDayUNV monthEntity = additionalService.GetRefYearMonth(year, monthFact.Key);

                var factList = factRepository.FindAll()
                                             .Where(f => f.SourceID == sourceIdFact
                                                        && f.RefUNV == monthEntity
                                                        && f.RefEvent == planEntity.RefEvent
                                                        && f.RefFin == planEntity.RefFin)
                                             .ToList();
                
                F_ExcCosts_Finance factEntity = null;
                if (factList.Count > 1)
                {
                    Trace.TraceError(
                                     "Ошибка в таблице фактов \"Финансирование\" - дубликаты были удалены, по году={0}, месяцу={1}, id_мероприятия={2}, id_источника_финансирования={3}",
                                      year, 
                                      monthFact.Key, 
                                      planEntity.RefEvent.ID, 
                                      planEntity.RefFin.ID);

                    // Дубликаты - удаляем все (ругаться бесполезно - застопорим работу пользователя)
                    foreach (var entity in factList)
                    {
                        factRepository.Delete(entity);
                    }
                }
                else if (factList.Count == 1)
                {
                    factEntity = factList.First();
                    factEntity.Fact = monthFact.Value;
                }

                if (factEntity == null && monthFact.Value != null)
                {
                    factEntity = new F_ExcCosts_Finance
                                     {
                                         SourceID = sourceIdFact,
                                         TaskID = -1,
                                         Target = null,
                                         Fact = monthFact.Value,
                                         RefEvent = planEntity.RefEvent,
                                         RefFin = planEntity.RefFin,
                                         RefUNV = monthEntity
                                     };
                }

                // Если нужно создавать/изменять, то выполняем изменения в БД
                if (factEntity != null)
                {
                    factRepository.Save(factEntity);
                    factRepository.DbContext.CommitChanges();

                    // Страхуемся от дубликатов от параллельных сессий
                    ProtectionFromDuplicatesInFactData(factEntity);
                }
            }
        }

        ////public void DeleteAllFacts(int programId)
        ////{
        ////    var targets = from f in targetRepository.FindAll()
        ////                 where f.RefProg.ID == programId
        ////                select f;
        ////    foreach (var entity in targets)
        ////    {
        ////        targetRepository.Delete(entity);
        ////        taskService.DeleteAllTask(entity.ID);
        ////    }

        ////    if (targets.Count() > 0)
        ////    {
        ////        targetRepository.DbContext.CommitChanges();
        ////    }
        ////}

        ////private void CheckEntity(D_ExcCosts_Goals entity)
        ////{
        ////    if (String.IsNullOrEmpty(entity.Name))
        ////    {
        ////        throw new ArgumentNullException("Поле Name не может быть пустым");
        ////    }
        ////}
         
        private void ProtectionFromDuplicatesInFactData(F_ExcCosts_Finance entity)
        {
            var dummy = from p in factRepository.FindAll()
                        where p.RefUNV == entity.RefUNV
                              && p.RefEvent == entity.RefEvent
                              && p.RefFin == entity.RefFin
                              && p.SourceID == entity.SourceID
                        select new { p.ID };
            int i = dummy.Count();
            if (dummy.Count() > 1)
            {
                factRepository.Delete(entity);
                factRepository.DbContext.CommitChanges();
                throw new DuplicateNameException("Создаваемые данные по показателям/году уже занесены.");
            }
        }
    }
}
