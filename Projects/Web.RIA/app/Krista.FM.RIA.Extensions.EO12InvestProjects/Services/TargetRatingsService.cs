using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public class TargetRatingsService : ITargetRatingsService
    {
        private readonly ILinqRepository<F_InvProject_Data> factRepository;
        private readonly IProjectService projectService;
        private readonly IAdditionalDataService additionalService;

        private const string SeparatorForEncodeYearQuarter = ".";

        public TargetRatingsService(
                                     ILinqRepository<F_InvProject_Data> factRepository,
                                     IProjectService projectService,
                                     IAdditionalDataService additionalService)
        {
            this.factRepository = factRepository;
            this.projectService = projectService;
            this.additionalService = additionalService;
        }

        public IList<object> GetQuarterList(int refProjId)
        {
            var project = projectService.GetProject(refProjId);

            int beginYear = project.RefBeginDate.GetYear();
            int endYear = project.RefEndDate.GetYear();
            var result = new List<object>();

            // Добавляем "предыдущий год"
            if (beginYear > 0)
            {
                result.Add(new
                {
                    ID = EncodeYearQuarter(beginYear - 1, 0),
                    Value = String.Format("Предыдущий год ({0})", beginYear - 1)
                });
            }

            for (int year = beginYear; year <= endYear; year++)
            {
                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    result.Add(new
                    {
                        ID = EncodeYearQuarter(year, quarter),
                        Value = String.Format("{0} квартал {1} года", quarter, year)
                    });
                }
            }

            return result;
        }
        
        public DataTable GetRatingsTable(int refProjId, string yearQuarter)
        {
            int year = DecodeYearFromYearQuarter(yearQuarter);
            int quarter = DecodeQuarterFromYearQuarter(yearQuarter);
            
            FX_Date_YearDayUNV refYearQuarter;
            if (quarter > 0)
            {
                refYearQuarter = additionalService.GetRefYearQuarter(year, quarter);
            }
            else if (quarter == 0)
            {
                refYearQuarter = additionalService.GetRefYear(year);
            }
            else
            {
                throw new ArgumentException("Неверное значение квартала");
            }

            DataTable table = new DataTable();

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("IndicatorId", typeof(int));
            table.Columns.Add("Indicator", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("Note", typeof(string));
            table.Columns.Add("Value", typeof(string));
            table.Columns.Add("SumValue", typeof(decimal));

            // Берём из таблицы фактов все показатели по проекту
            var data = from f in factRepository.FindAll()
                       where f.RefReestr.ID == refProjId
                             && f.RefIndicator.RefTypeI.ID == (int)InvProjInvestType.TargetRatings
                       orderby f.RefIndicator.Code
                       select f;

            // Формируем таблицу
            DataRow row = null;
            int oldIndicatorId = 0;
            foreach (var factRow in data)
            {
                // Если новый индикатор - то нужно начинать формировать новую строку в таблицу
                if (oldIndicatorId != factRow.RefIndicator.ID)
                {
                    // Добавляем ранее сформированную строку
                    if (oldIndicatorId != 0)
                    {
                        table.Rows.Add(row);
                    }

                    row = table.NewRow();
                    row["ID"] = factRow.RefIndicator.ID;
                    row["IndicatorId"] = factRow.RefIndicator.ID;
                    row["Indicator"] = factRow.RefIndicator.Name;
                    row["Unit"] = factRow.RefIndicator.RefOKEI.Designation;
                    row["Note"] = factRow.RefIndicator.Note;
                    row["Value"] = String.Empty;
                    row["SumValue"] = 0;

                    oldIndicatorId = factRow.RefIndicator.ID;
                }

                if (factRow.RefDate == refYearQuarter)
                {
                    row["Value"] = factRow.Value.ToString();
                }

                // Суммируем всё кроме "предыдущего года"
                if (factRow.RefDate.IsQuarter())
                {
                    row["SumValue"] = (decimal)row["SumValue"] + factRow.Value;
                }
            }

            if (row != null)
            {
                table.Rows.Add(row);
            }

            return table;
        }

        public void CreateFactData(int refProjId, string yearQuarter, int refIndicatorId, decimal value)
        {
            // Находим проект
            var projectEntyty = projectService.GetProject(refProjId);

            // Находими индикатор
            var indicatorEntity = additionalService.GetIndicator(refIndicatorId);

            // Находим ссылку на Квартал/Год
            var year = DecodeYearFromYearQuarter(yearQuarter);
            var quarter = DecodeQuarterFromYearQuarter(yearQuarter);
            FX_Date_YearDayUNV dateEntity;
            if (quarter > 0)
            {
                dateEntity = additionalService.GetRefYearQuarter(year, quarter);
            }
            else if (quarter == 0)
            {
                dateEntity = additionalService.GetRefYear(year);
            }
            else
            {
                throw new ArgumentException("Неверное значение квартала");
            }
            
            // Формируем строку для вставки
            var factEntity = new F_InvProject_Data
                {
                    RefReestr = projectEntyty,
                    RefIndicator = indicatorEntity,
                    RefDate = dateEntity,
                    Value = value
                };

             // Сохраняем
             factRepository.Save(factEntity);
             factRepository.DbContext.CommitChanges();

             // Страхуемся от дубликатов из параллельных сессий
             ProtectionFromDuplicatesInFactData(factEntity);
        }

        public void UpdateFactData(int refProjId, string yearQuarter, int refIndicatorId, decimal? value)
        {
            // Находим проект
            var projectEntyty = projectService.GetProject(refProjId);

            // Находими индикатор
            var indicatorEntity = additionalService.GetIndicator(refIndicatorId);

            // Находим ссылку на Квартал/Год
            int year = DecodeYearFromYearQuarter(yearQuarter);
            int quarter = DecodeQuarterFromYearQuarter(yearQuarter);
            FX_Date_YearDayUNV dateEntity;
            if (quarter > 0)
            {
                dateEntity = additionalService.GetRefYearQuarter(year, quarter);
            }
            else if (quarter == 0)
            {
                dateEntity = additionalService.GetRefYear(year);
            }
            else
            {
                throw new ArgumentException("Неверное значение квартала");
            }
            
            // Находим данные по указанным дате и показателю
            var facts = from p in factRepository.FindAll()
                        where p.RefIndicator == indicatorEntity
                              && p.RefDate == dateEntity
                              && p.RefReestr == projectEntyty
                        select p;

            if (facts.Count() > 1)
            {
                // Если дубликаты - ошибка
                throw new DuplicateNameException(String.Format("При изменении данных обнаружен дубликат по году {0} кварталу {1} для показателя {2}", year, quarter, indicatorEntity.Name));
            }

            F_InvProject_Data factEntityOld = facts.FirstOrDefault();

            if (factEntityOld == null && value == null)
            {
                // Данных не было и не надо
            }
            else if (factEntityOld == null && value != null)
            {
                // Данные нужно создать
                var factEntityNew = new F_InvProject_Data
                    {
                        RefReestr = projectEntyty,
                        RefIndicator = indicatorEntity,
                        RefDate = dateEntity,
                        Value = (decimal)value
                    };

                // Сохраняем
                factRepository.Save(factEntityNew);
                factRepository.DbContext.CommitChanges();

                // Страхуемся от дубликатов от параллельных сессий
                ProtectionFromDuplicatesInFactData(factEntityNew);
            }
            else if (factEntityOld != null && value == null)
            {
                // Данные нужно удалить
                factRepository.Delete(factEntityOld);
                factRepository.DbContext.CommitChanges();
            }
            else if (factEntityOld != null && value != null)
            {
                // Данные нужно изменить при необходимости
                if (factEntityOld.Value != value)
                {
                    factEntityOld.Value = (decimal)value;
                    factRepository.Save(factEntityOld);
                    factRepository.DbContext.CommitChanges();
                }
            }
            else
            {
                throw new Exception("Недостижимая ветка кода!");
            }
        }

        public void DeleteFactData(int refProjId, string yearQuarter, int refIndicatorId)
        {
            FX_Date_YearDayUNV dateEntity;

            // Находим ссылку на Квартал/Год
            try
            {
                int year = DecodeYearFromYearQuarter(yearQuarter);
                int quarter = DecodeQuarterFromYearQuarter(yearQuarter);
                if (quarter > 0)
                {
                    dateEntity = additionalService.GetRefYearQuarter(year, quarter);
                }
                else if (quarter == 0)
                {
                    dateEntity = additionalService.GetRefYear(year);
                }
                else
                {
                    throw new ArgumentException("Неверное значение квартала");
                }
            }
            catch (KeyNotFoundException)
            {
                return;
            }
            
            // Находим строки для удаления
            var rows = from p in factRepository.FindAll()
                       where p.RefReestr.ID == refProjId
                             && p.RefIndicator.ID == refIndicatorId
                             && p.RefDate == dateEntity
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

        public string EncodeYearQuarter(int year, int quarter)
        {
            return String.Format("{0}{1}{2}", year, SeparatorForEncodeYearQuarter, quarter);
        }

        public int DecodeYearFromYearQuarter(string yearQuarter)
        {
            int separatorPosition = yearQuarter.IndexOf(SeparatorForEncodeYearQuarter);
            int year = Convert.ToInt32(yearQuarter.Substring(0, separatorPosition));
            return year;
        }

        public int DecodeQuarterFromYearQuarter(string yearQuarter)
        {
            int separatorPosition = yearQuarter.IndexOf(SeparatorForEncodeYearQuarter);
            int quarter = Convert.ToInt32(yearQuarter.Substring(separatorPosition + 1));
            return quarter;
        }

        public void ProtectionFromDuplicatesInFactData(F_InvProject_Data entity)
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
