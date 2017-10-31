using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class AdditionalService : IAdditionalService
    {
        private readonly ILinqRepository<D_ExcCosts_Creators> creatorsRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> yearDayUnvRepository;
        private readonly ILinqRepository<FX_ExcCosts_TpPrg> programTypeRepository;
        private readonly ILinqRepository<D_Units_OKEI> unitsRepository;
        private readonly ILinqRepository<D_Territory_RF> territoryRepository; 
        
        public AdditionalService(
                                  ILinqRepository<D_ExcCosts_Creators> creatorsRepository,
                                  ILinqRepository<FX_Date_YearDayUNV> yearDayUnvRepository,
                                  ILinqRepository<FX_ExcCosts_TpPrg> programTypeRepository,
                                  ILinqRepository<D_Units_OKEI> unitsRepository, 
                                  ILinqRepository<D_Territory_RF> territoryRepository)
        {
            this.creatorsRepository = creatorsRepository;
            this.yearDayUnvRepository = yearDayUnvRepository;
            this.programTypeRepository = programTypeRepository;
            this.unitsRepository = unitsRepository;
            this.territoryRepository = territoryRepository;
        }

        public D_ExcCosts_Creators GetCreator(string login)
        {
            var data = (from t in creatorsRepository.FindAll()
                        where t.Login == login
                        select t).ToList();

            if (data.Count == 0)
            {
                throw new KeyNotFoundException(String.Format("Заказчик с логином {0} не найден.", login));
            }
            else if (data.Count > 1)
            {
                throw new DuplicateNameException(String.Format("Для логина {0} обнаружены дубликаты среди заказчиков.", login));
            }
            else
            {
                return data.First();
            }
        }

        public D_ExcCosts_Creators GetCreator(int id)
        {
            var entity = creatorsRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Заказчик не найден!");
            }

            return entity;
        }

        public FX_Date_YearDayUNV GetRefYear(int year)
        {
            int id = (year * 10000) + 1; ////формат id-шников для годовых записей: YYYY0001
            var result = yearDayUnvRepository.FindOne(id);
            if (result == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0} не найдена.", year));
            }

            return result;
        }

        public FX_Date_YearDayUNV GetRefYearMonth(int year, int month) 
        {
            int id = (year * 10000) + (month * 100) + 00; ////формат id-шников для месяца: YYYYMM00
            var result = yearDayUnvRepository.FindOne(id);
            if (result == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0} и месяцем {1} не найдена.", year, month));
            }

            return result;
        }

        public FX_ExcCosts_TpPrg GetRefTypeProg(int id)
        {
            var entity = programTypeRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Тип программы не найден");
            }

            return entity;
        }

        public IList GetAllOwnersList()
        {
            var data = from p in this.creatorsRepository.FindAll()
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name,
                       };

            return data.ToList();
        }

        public IList GetAllUnitListForLookup()
        {
            var data = from p in this.unitsRepository.FindAll()
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name,
                           Designation = p.Designation
                       };

            return data.ToList();
        }

        public D_Units_OKEI GetUnit(int id)
        {
            var entity = unitsRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Единица измерения не найдена");
            }

            return entity;
        }

        public FX_Date_YearDayUNV GetRefYearMonthDay(int year, int month, int day)
        {
            int id = (year * 10000) + (month * 100) + day; ////формат id-шников для дат: YYYYMMDD
            var result = yearDayUnvRepository.FindOne(id);
            if (result == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0}, месяцем {1}, днем {2} не найдена.", year, month, day));
            }

            return result;
        }

        public D_Territory_RF GetRefTerritory(int id)
        {
            var entity = territoryRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Территория не найдена");
            }

            return entity;
        }
    }
}
