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
    public class TargetRatingService : ITargetRatingService
    {
        private readonly ILinqRepository<D_ExcCosts_GoalMark> rateRepository;
        private readonly ILinqRepository<F_ExcCosts_MarkPrg> factRepository;
        private readonly ILinqRepository<FX_ExcCosts_TypeMark> rateTypeRepository;
        private readonly IAdditionalService additionalService;
        private readonly IDatasourceService datasourceService;

        public TargetRatingService(
                              ILinqRepository<D_ExcCosts_GoalMark> rateRepository,
                              ILinqRepository<F_ExcCosts_MarkPrg> factRepository,
                              ILinqRepository<FX_ExcCosts_TypeMark> rateTypeRepository,
                              IAdditionalService additionalService,
                              IDatasourceService datasourceService)
        {
            this.rateRepository = rateRepository;
            this.factRepository = factRepository;
            this.rateTypeRepository = rateTypeRepository;
            this.additionalService = additionalService;
            this.datasourceService = datasourceService;
        }

        public DataTable GetRatingsListTable(D_ExcCosts_ListPrg program)
        {
            DataTable table = new DataTable();

            IList<int> years = program.GetYearsWithPreviousAndFollowing();
            var prevYear = years.Min();
            var followYear = years.Max();

            int sourceId = datasourceService.GetDefaultDatasourceId();
            
            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("TaskName", typeof(string));
            table.Columns.Add("TaskId", typeof(int));
            table.Columns.Add("RateName", typeof(string));
            table.Columns.Add("RateTypeName", typeof(string));
            table.Columns.Add("RateTypeId", typeof(int));
            table.Columns.Add("UnitName", typeof(string));
            table.Columns.Add("UnitId", typeof(int));
            foreach (int year in years)
            {
                table.Columns.Add(string.Format("Year{0}", year), typeof(decimal));
            }

            // выбираем индикаторы
            var rates = from r in rateRepository.FindAll()
                        where r.RefTask.RefGoal.RefProg == program
                        select r;

            // Формируем строки таблицы для каждого индикатора
            foreach (D_ExcCosts_GoalMark rate in rates)
            {
                DataRow row = table.NewRow();
                row["ID"] = rate.ID;
                row["TaskName"] = rate.RefTask.Name;
                row["TaskId"] = rate.RefTask.ID;
                row["RateName"] = rate.Name;
                row["RateTypeName"] = rate.RefTypeMark.Name;
                row["RateTypeId"] = rate.RefTypeMark.ID;
                row["UnitName"] = rate.RefOKEI.Designation;
                row["UnitId"] = rate.RefOKEI.ID;

                // Выбираем всё из таблицы фактов по данному проекту только для нужного типа индикаторов, и упорядочиваем по индикаторам и годам
                var factdata = from f in factRepository.FindAll()
                              where f.RefMark == rate
                                   && f.SourceID == sourceId
                              select f;
                
                foreach (F_ExcCosts_MarkPrg factRow in factdata)
                {
                    int year = factRow.RefUNV.GetYear();
                    string columnName = string.Format("Year{0}", year);

                    //// if - для  страховки от наличия неудаленных данных в таблице фактов - такой колонки в таблице нет, а даные для неё есть...
                    if (table.Columns.Contains(columnName))
                    {
                        decimal? data;
                        if (year == prevYear)
                        {
                            data = factRow.BaseValue;
                        }
                        else if (year == followYear)
                        {
                            data = factRow.GoalValue;
                        }
                        else
                        {
                            data = factRow.Target;
                        }

                        if (data == null)
                        {
                            row[columnName] = DBNull.Value;
                        }
                        else
                        {
                            row[columnName] = data;
                        }
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public IList GetAllRateTypeListForLookup()
        {
            var data = from p in this.rateTypeRepository.FindAll()
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name
                       };

            return data.ToList();
        }

        public D_ExcCosts_GoalMark GetRate(int id)
        {
            var entity = this.rateRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Показатель не найден");
            }

            return entity;
        }

        public void CreateRateWithFactData(int programId, D_ExcCosts_Tasks task, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit, Dictionary<int, decimal?> yearsValues)
        {
            if (task.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Задача не соответствует указанной программе");
            }

            int sourceId = datasourceService.GetDefaultDatasourceId();

            D_ExcCosts_GoalMark rate = CreateRate(task, rateName, rateType, unit);

            var prevYear = yearsValues.Min(f => f.Key);
            var followYear = yearsValues.Max(f => f.Key);
            foreach (KeyValuePair<int, decimal?> yearValue in yearsValues)
            {
                if (yearValue.Value != null)
                {
                    var factEntity = new F_ExcCosts_MarkPrg
                    {
                        SourceID = sourceId,
                        TaskID = -1,
                        GoalValue = null,
                        BaseValue = null,
                        Target = null,
                        Fact = null,
                        RefMark = rate,
                        RefUNV = additionalService.GetRefYear(yearValue.Key)
                    };

                    SetFactValue(ref factEntity, yearValue.Key, yearValue.Value, prevYear, followYear);
                    factRepository.Save(factEntity);
                    factRepository.DbContext.CommitChanges();

                    // Страхуемся от дубликатов из параллельных сессий
                    ProtectionFromDuplicatesInFactData(factEntity);
                }
            }
        }

        public void UpdateRateWithFactData(int programId, int rateId, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit, D_ExcCosts_Tasks task, Dictionary<int, decimal?> yearsValues)
        {
            var rate = GetRate(rateId);

            if (rate.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Показатель не соответствует указанной программе");
            }

            // Обновляем показатель при необходимости
            if (rate.Name != rateName
                || rate.RefOKEI.ID != unit.ID
                || rate.RefTypeMark.ID != rateType.ID
                || rate.RefTask.ID != task.ID)
            {
                UpdateRate(ref rate, rateName, rateType, unit, task);
            }

            int sourceId = datasourceService.GetDefaultDatasourceId();

            // Обновляем факты
            // Проходим по всем годам
            var yearList = rate.RefTask.RefGoal.RefProg.GetYearsWithPreviousAndFollowing();
            var prevYear = yearList.Min();
            var followYear = yearList.Max();
            foreach (int year in yearList)
            {
                var yearEntity = additionalService.GetRefYear(year);

                // Находим данные по указанным году и показателю
                var facts = from p in factRepository.FindAll()
                            where p.RefUNV == yearEntity
                                  && p.RefMark == rate
                                  && p.SourceID == sourceId
                            select p;

                if (facts.Count() > 1)
                {
                    // Если дубликаты - ошибка
                    throw new DuplicateNameException(String.Format("При изменении данных обнаружен дубликат по году {0} для показателя {1}", year, rate.Name));
                }

                F_ExcCosts_MarkPrg factEntityOld = facts.FirstOrDefault();

                decimal? data = yearsValues[year];

                // первый и последний год - факты сохранять на BaseValue и GoalValue соответственно
                if (factEntityOld == null && data == null)
                {
                    // Данных не было и не надо
                    continue;
                }
                else if (factEntityOld == null && (data != null))
                {
                    // Данные нужно создать
                    var factEntityNew = new F_ExcCosts_MarkPrg
                    {
                        SourceID = sourceId,
                        TaskID = -1,
                        GoalValue = null,
                        BaseValue = null,
                        Target = null,
                        Fact = null,
                        RefMark = rate,
                        RefUNV = additionalService.GetRefYear(year)
                    };
                    SetFactValue(ref factEntityNew, year, data, prevYear, followYear);

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
                    // Данные нужно изменить
                    SetFactValue(ref factEntityOld, year, data, prevYear, followYear);
                    
                    factRepository.Save(factEntityOld);
                    factRepository.DbContext.CommitChanges();
                }
                else
                {
                    throw new Exception("Недостижимая ветка кода!");
                }
            }
        }

        public void DeleteRateWithFactData(int programId, int rateId)
        {
            int sourceId = datasourceService.GetDefaultDatasourceId();

            // Находим показатель
            D_ExcCosts_GoalMark rate;
            try
            {
                rate = GetRate(rateId);
            }
            catch (KeyNotFoundException)
            {
                return;
            }
            
            if (rate.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Показатель не соответствует указанной программе");
            }

            // Находим строки для удаления в таблице фактов
            var rows = from p in factRepository.FindAll()
                       where p.RefMark == rate
                             && p.SourceID == sourceId
                       select p;
            foreach (var row in rows)
            {
                factRepository.Delete(row);
            }
            
            factRepository.DbContext.CommitChanges();

            rateRepository.Delete(rate);
            rateRepository.DbContext.CommitChanges();
        }

        public FX_ExcCosts_TypeMark GetRateType(int id)
        {
            var entity = rateTypeRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Тип показателя не найдена");
            }

            return entity;
        }

        public DataTable GetReportTable(D_ExcCosts_ListPrg program, int year)
        {
            DataTable table = new DataTable();

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("RateName", typeof(string));
            table.Columns.Add("RateUnits", typeof(string));
            table.Columns.Add("RateTypeName", typeof(string));

            table.Columns.Add("RateBaseValue", typeof(decimal));
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

            table.Columns.Add("RateNote", typeof(string));
            
            int sourceIdPlan = datasourceService.GetDefaultDatasourceId();
            int sourceIdFact = datasourceService.GetFactDatasourceId();

            var rateList = from t in rateRepository.FindAll()
                                   where t.RefTask.RefGoal.RefProg == program
                                   orderby t.RefTask.Name
                                   select new
                                   {
                                       ID = t.ID,
                                       RateName = t.Name,
                                       RateTypeName = t.RefTypeMark.Name,
                                       RateNote = t.Note,
                                       RateUnitDesignation = t.RefOKEI.Designation
                                   };
         
            DataRow row = null;
            
            // Маска для выделения из выборки данных за все месяца нужного года
            var monthes = new List<int>();
            for (int month = 1; month <= 12; month++)
            {
                monthes.Add((year * 10000) + (month * 100));
            }

            foreach (var rate in rateList)
            {
                row = table.NewRow();
                row["ID"] = rate.ID;
                row["RateName"] = rate.RateName;
                row["RateUnits"] = rate.RateUnitDesignation;
                row["RateTypeName"] = rate.RateTypeName;

                row["RateNote"] = rate.RateNote;
                
                int previousYear = program.RefBegDate.GetYear() - 1;
                var rateBaseValue = factRepository.FindAll()
                                                .Where(f => f.SourceID == sourceIdPlan
                                                            && f.RefMark.ID == rate.ID
                                                            && f.RefUNV == additionalService.GetRefYear(previousYear))
                                                .Select(f => f.BaseValue)
                                                .FirstOrDefault();
                if (rateBaseValue == null)
                {
                    row["RateBaseValue"] = DBNull.Value;
                }
                else
                {
                    row["RateBaseValue"] = rateBaseValue;
                }

                int followingYear = program.RefEndDate.GetYear() + 1;
                var planYear = factRepository.FindAll()
                                            .Where(f => f.SourceID == sourceIdPlan
                                                        && f.RefMark.ID == rate.ID
                                                        && f.RefUNV == additionalService.GetRefYear(followingYear))
                                            .Select(f => f.GoalValue)
                                            .FirstOrDefault();
                if (planYear == null)
                {
                    row["PlanYear"] = DBNull.Value;
                }
                else
                {
                    row["PlanYear"] = planYear;
                }

                // Факты все по данному sourceId, мероприятию и источнику финансирования
                var factAll = factRepository.FindAll()
                                            .Where(f => f.SourceID == sourceIdFact
                                                        && f.RefMark.ID == rate.ID)
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
                bool canSummarize = !rate.RateUnitDesignation.StartsWith("%");
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

                    if (canSummarize)
                    {
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
                }

                row["Quarter1"] = quarter1;
                row["Quarter2"] = quarter2;
                row["Quarter3"] = quarter3;
                row["Quarter4"] = quarter4;

                row["HalfYear1"] = halfYear1;
                row["HalfYear2"] = halfYear2;
                row["FactYear"] = factYear;

                table.Rows.Add(row);
            }

            return table;
        }

        public void SaveReportFactData(int programId, int rateId, int year, Dictionary<int, decimal?> monthFactList)
        {
            var userCredentials = (BasePrincipal)System.Web.HttpContext.Current.User;

            var rateEntity = GetRate(rateId);

            if (rateEntity.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Показатель не соответствует указанной программе");
            }

            if (rateEntity.RefTask.RefGoal.RefProg.RefCreators.Login != userCredentials.DbUser.Name)
            {
                throw new SecurityException(String.Format("Запрещено редактировать показатель: {0}", rateEntity.Name));
            }

            int sourceIdFact = datasourceService.GetFactDatasourceId();
            
            foreach (var monthFact in monthFactList)
            {
                FX_Date_YearDayUNV monthEntity = additionalService.GetRefYearMonth(year, monthFact.Key);

                var factList = factRepository.FindAll()
                                             .Where(f => f.SourceID == sourceIdFact
                                                        && f.RefUNV == monthEntity
                                                        && f.RefMark == rateEntity)
                                             .ToList();

                F_ExcCosts_MarkPrg factEntity = null;
                if (factList.Count > 1)
                {
                    Trace.TraceError(
                                     "Ошибка в таблице фактов \"Показатели\" - дубликаты были удалены, по году={0}, месяцу={1}, id_показателя={2}",
                                      year,
                                      monthFact.Key,
                                      rateEntity.ID);

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
                    factEntity = new F_ExcCosts_MarkPrg
                    {
                        SourceID = sourceIdFact,
                        TaskID = -1,
                        GoalValue = null,
                        BaseValue = null,
                        Target = null,
                        Fact = monthFact.Value,
                        RefMark = rateEntity,
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

        public void DeleteAllRate(int taskId)
        {
            var rates = from f in rateRepository.FindAll()
                          where f.RefTask.ID == taskId
                          select f;
            foreach (var entity in rates)
            {
                rateRepository.Delete(entity);
            }

            try
            {
                rateRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления показателей", e);
            } 
        }

        private void SetFactValue(ref F_ExcCosts_MarkPrg factEntity, int year, decimal? data, int prevYear, int followYear)
        {
            if (year == prevYear)
            {
                factEntity.BaseValue = data;
            }
            else if (year == followYear)
            {
                factEntity.GoalValue = data;
            }
            else
            {
                factEntity.Target = data;
            }
        }

        private void ProtectionFromDuplicatesInFactData(F_ExcCosts_MarkPrg entity)
        {
            var dummy = from p in factRepository.FindAll()
                        where p.RefUNV == entity.RefUNV
                              && p.RefMark == entity.RefMark
                              && p.SourceID == entity.SourceID
                        select new { p.ID };
            int i = dummy.Count();
            if (dummy.Count() > 1)
            {
                factRepository.Delete(entity);
                factRepository.DbContext.CommitChanges();
                throw new DuplicateNameException("Создаваемые данные по показателю/году уже занесены.");
            }
        }
        
        /// <summary>
        /// Создание нового показателя
        /// </summary>
        private D_ExcCosts_GoalMark CreateRate(D_ExcCosts_Tasks task, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit)
        {
            D_ExcCosts_GoalMark rate = new D_ExcCosts_GoalMark
                                           {
                                               Name = rateName,
                                               Note = null,
                                               ReasonDec = null,
                                               Suggestion = null,
                                               RefOKEI = unit,
                                               RefTypeMark = rateType,
                                               RefTask = task
                                           };
            rateRepository.Save(rate);
            return rate;
        }

        /// <summary>
        /// Изменение существующего показателя
        /// </summary>
        private void UpdateRate(ref D_ExcCosts_GoalMark rate, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit, D_ExcCosts_Tasks task)
        {
            if (rate.ID == 0)
            {
                throw new Exception("update несуществующего в БД показателя");
            }

            rate.Name = rateName;
            rate.RefOKEI = unit;
            rate.RefTypeMark = rateType;
            rate.RefTask = task;
            
            rateRepository.Save(rate);
            rateRepository.DbContext.CommitChanges();
        }
    }
}
