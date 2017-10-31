using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public class InvestPlanService : IInvestPlanService
    {
        private readonly ILinqRepository<F_InvProject_Data> factRepository;
        private readonly IProjectService projectService;
        private readonly IAdditionalDataService additionalService;
        
        public InvestPlanService(
                                  ILinqRepository<F_InvProject_Data> factRepository,
                                  IProjectService projectService,
                                  IAdditionalDataService additionalService)
        {
            this.factRepository = factRepository;
            this.projectService = projectService;
            this.additionalService = additionalService;
        }

        public DataTable GetInvestsTable(int refProjId, InvProjInvestType projInvestType)
        {
            DataTable table = new DataTable();
            
            IList<int> yearsColumns = GetYearsColumns(refProjId);

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Indicator", typeof(string));
            table.Columns.Add("IndicatorId", typeof(int));
            table.Columns.Add("Unit", typeof(string));
            foreach (int year in yearsColumns)
            {
                table.Columns.Add(string.Format("Year{0}", year), typeof(decimal));
            }

            // Выбираем всё из таблицы фактов по данному проекту только для нужного типа индикаторов, и упорядочиваем по индикаторам и годам
            var data = from f in factRepository.FindAll()
                        where f.RefReestr.ID == refProjId
                                && f.RefIndicator.RefTypeI.ID == (int)projInvestType
                        orderby f.RefIndicator.ID, f.RefDate.ID
                        select f;

            // Формируем транспонированную таблицу
            DataRow row = null;
            int oldIndicatorId = 0;
            foreach (F_InvProject_Data fact in data)
            {
                // Если новый индикатор - то нужно начинать формировать новую строку в таблицу
                if (oldIndicatorId != fact.RefIndicator.ID)
                {
                    // Добавляем ранее сформированную строку
                    if (oldIndicatorId != 0)
                    {
                        table.Rows.Add(row);
                    }

                    row = table.NewRow();
                    row["ID"] = fact.RefIndicator.ID;
                    row["Indicator"] = fact.RefIndicator.Name;
                    row["IndicatorId"] = fact.RefIndicator.ID;
                    row["Unit"] = fact.RefIndicator.RefOKEI.Designation;
                    oldIndicatorId = fact.RefIndicator.ID;
                }

                string columnName = string.Format("Year{0}", fact.RefDate.GetYear());
                if (table.Columns.Contains(columnName))
                {
                    //// if - для  страховки от наличия неудаленных данных в таблице фактов - такой колонки в таблице нет, а даные для неё есть...
                    row[columnName] = fact.Value;
                }
            }

            if (row != null)
            {
                table.Rows.Add(row);
            }
            
            return table;
        }
        
        public void CreateFactData(int refProjId, int refIndicatorId, List<int> years, Dictionary<string, string> rowData)
        {
            // Находим проект
            var projectEntyty = projectService.GetProject(refProjId);

            // Находими индикатор
            var indicatorEntity = additionalService.GetIndicator(refIndicatorId);
            
            // Проходим по всем годам
            foreach (int year in years)
            {
                string dataStr = rowData[String.Format("Year{0}", year)];
                if (dataStr == String.Empty)
                {
                    continue;
                }

                decimal data = ConvertToDecimal(dataStr);

                var yearEntity = additionalService.GetRefYear(year);

                // Если данные заполнены, тогда формируем строку для вставки
                var factEntity = new F_InvProject_Data
                                     {
                                         RefReestr = projectEntyty,
                                         RefIndicator = indicatorEntity,
                                         RefDate = yearEntity,
                                         Value = data
                                     };

                // Сохраняем
                factRepository.Save(factEntity);
                factRepository.DbContext.CommitChanges();

                // Страхуемся от дубликатов из параллельных сессий
                ProtectionFromDuplicatesInFactData(factEntity);
            }
        }

        public void UpdateFactData(int refProjId, int refIndicatorId, List<int> years, Dictionary<string, string> rowData)
        {
            // Находим проект
            var projectEntyty = projectService.GetProject(refProjId);

            // Находими индикатор
            var indicatorEntity = additionalService.GetIndicator(refIndicatorId);
            
            // Проходим по всем годам
            foreach (int year in years)
            {
                var yearEntity = additionalService.GetRefYear(year);

                // Находим данные по указанным году и показателю
                var facts = from p in factRepository.FindAll()
                            where p.RefIndicator == indicatorEntity
                                  && p.RefDate == yearEntity
                                  && p.RefReestr == projectEntyty
                            select p;
                
                if (facts.Count() > 1)
                {
                    // Если дубликаты - ошибка
                    throw new DuplicateNameException(String.Format("При изменении данных обнаружен дубликат по году {0} для показателя {1}", year, indicatorEntity.Name));
                }

                F_InvProject_Data factEntityOld = facts.FirstOrDefault();
                decimal data;
                
                string dataStr = rowData[String.Format("Year{0}", year)];
                
                if (factEntityOld == null && String.IsNullOrEmpty(dataStr))
                {
                     // Данных не было и не надо
                     continue;
                }
                else if (factEntityOld == null && (!String.IsNullOrEmpty(dataStr)))
                {
                    // Данные нужно создать
                    data = ConvertToDecimal(dataStr);

                    var factEntityNew = new F_InvProject_Data
                                            {
                                                RefReestr = projectEntyty,
                                                RefIndicator = indicatorEntity,
                                                RefDate = yearEntity,
                                                Value = data
                                            };

                    // Сохраняем
                    factRepository.Save(factEntityNew);
                    factRepository.DbContext.CommitChanges();

                    // Страхуемся от дубликатов от параллельных сессий
                    ProtectionFromDuplicatesInFactData(factEntityNew);
                }
                else if (factEntityOld != null && String.IsNullOrEmpty(dataStr))
                {
                    // Данные нужно удалить
                    factRepository.Delete(factEntityOld);
                    factRepository.DbContext.CommitChanges();
                }
                else if (factEntityOld != null && (!String.IsNullOrEmpty(dataStr)))
                {
                    // Данные нужно изменить при необходимости
                    data = ConvertToDecimal(dataStr);
                    if (factEntityOld.Value != data)
                    {
                        factEntityOld.Value = data;
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

        public void DeleteFactData(int refProjId, int refIndicatorId)
        {
            // Находим строки для удаления
            var rows = from p in factRepository.FindAll()
                       where p.RefReestr.ID == refProjId
                             && p.RefIndicator.ID == refIndicatorId
                       select p;
            foreach (var row in rows)
            {
                factRepository.Delete(row);
            }

            if (rows.Count() > 0)
            {
                factRepository.DbContext.CommitChanges();
            }
        }

        /// <summary>
        /// Формирует список колонок по годам, начиная с года начала и заканчивая годом окончания реализации проекта
        /// </summary>
        /// <param name="projId">id проекта</param>
        public IList<int> GetYearsColumns(int projId)
        {
            var project = projectService.GetProject(projId);
            var result = new List<int>();
            int beginYear = project.RefBeginDate.GetYear();
            int endYear = project.RefEndDate.GetYear();

            for (int i = beginYear; i <= endYear; i++)
            {
                result.Add(i);
            }

            return result;
        }

        private static decimal ConvertToDecimal(string value)
        {
            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";
            string val = value.Replace(".", ",");
            decimal result = Decimal.Parse(val, ci);
            return result;
        }

        private void ProtectionFromDuplicatesInFactData(F_InvProject_Data entity)
        {
            var dummy = from p in factRepository.FindAll()
                        where p.RefIndicator == entity.RefIndicator
                              && p.RefDate == entity.RefDate
                              && p.RefReestr == entity.RefReestr
                        select new { p.ID };
            int i = dummy.Count();
            if (dummy.Count() > 1)
            {
                factRepository.Delete(entity);
                factRepository.DbContext.CommitChanges();
                throw new DuplicateNameException("Создаваемые данные по показателю/году уже занесены.");
            }
        }
    }
}
