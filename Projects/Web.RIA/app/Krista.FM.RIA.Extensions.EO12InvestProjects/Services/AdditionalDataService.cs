using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public class AdditionalDataService : IAdditionalDataService
    {
        private readonly ILinqRepository<FX_Date_YearDayUNV> yearDayUnvRepository;
        private readonly ILinqRepository<D_Territory_RF> territoryRfRepository;
        private readonly ILinqRepository<FX_InvProject_Status> invProjStatusRepository;
        private readonly ILinqRepository<FX_InvProject_Part> invProjPartRepository;
        private readonly ILinqRepository<D_InvProject_Indic> indicatorsRepository;
        private readonly ILinqRepository<D_OK_OKVED> okvedRepository;

        public AdditionalDataService(
                                       ILinqRepository<FX_Date_YearDayUNV> yearDayUnvRepository,
                                       ILinqRepository<D_Territory_RF> territoryRfRepository,
                                       ILinqRepository<FX_InvProject_Status> invProjStatusRepository,
                                       ILinqRepository<FX_InvProject_Part> invProjPartRepository,
                                       ILinqRepository<D_OK_OKVED> okvedRepository,
                                       ILinqRepository<D_InvProject_Indic> indicatorsRepository)
        {
            this.yearDayUnvRepository = yearDayUnvRepository;
            this.territoryRfRepository = territoryRfRepository;
            this.invProjStatusRepository = invProjStatusRepository;
            this.invProjPartRepository = invProjPartRepository;
            this.okvedRepository = okvedRepository;
            this.indicatorsRepository = indicatorsRepository;
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

        public FX_Date_YearDayUNV GetRefYearQuarter(int year, int quarter)
        {
            var id = (year * 10000) + 9990 + quarter;  ////формат id-шников для квартальных записей: YYYY999N
            var entity = yearDayUnvRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0} и кварталом {1} не найдена.", year, quarter));
            }

            return entity;
        }

        public FX_Date_YearDayUNV GetRefYearDayUnvUndefined()
        {
            const int UNDEFINED = -1;
            var entity = yearDayUnvRepository.FindOne(UNDEFINED);
            if (entity == null)
            {
                throw new KeyNotFoundException("Запись с неуказанным значением даты не найдена.");
            }

            return entity;
        }

        public D_Territory_RF GetRefTerritory(int id)
        {
            var result = territoryRfRepository.FindOne(id);
            return result;
        }

        public FX_InvProject_Status GetRefStatus(int id)
        {
            var result = invProjStatusRepository.FindOne(id);
            return result;
        }

        public FX_InvProject_Part GetRefPart(int id)
        {
            var result = invProjPartRepository.FindOne(id);
            return result;
        }

        public D_OK_OKVED GetRefOKVED(int id)
        {
            var result = okvedRepository.FindOne(id);
            return result;
        }

        public IList GetIndicatorList(int refTypeI)
        {
            var data = (
                from p in indicatorsRepository.FindAll()
                where p.RefTypeI.ID == refTypeI
                select new 
                        {
                            ID = p.ID,
                            Name = p.Name,
                            Note = p.Note,
                            Unit = p.RefOKEI.Designation
                        }).ToList();
            return data;
        }

        public D_InvProject_Indic GetIndicator(int id)
        {
            var entity = indicatorsRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Показатель не найден.");
            }

            return entity;
        }
    }
}
