using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class FactRepository : IFactRepository
    {
        private readonly ILinqRepository<F_OIV_REG10Qual> factRepository;
        private readonly IRegion10MarksOivExtension extension;
        private readonly IMarksRepository marksRepository;
        private readonly ITerritoryRepository territoryRepository;
        private readonly IRepository<FX_OIV_StatusData> statusRepository;

        public FactRepository(
                                    ILinqRepository<F_OIV_REG10Qual> factRepository,
                                    IRegion10MarksOivExtension extension,
                                    IMarksRepository marksRepository,
                                    ITerritoryRepository territoryRepository,
                                    IRepository<FX_OIV_StatusData> statusRepository)
        {
            this.factRepository = factRepository;
            this.extension = extension;
            this.marksRepository = marksRepository;
            this.territoryRepository = territoryRepository;
            this.statusRepository = statusRepository;
        }

        public IDbContext DbContext
        {
            get { return factRepository.DbContext; }
        }

        public F_OIV_REG10Qual GetFactForMarkTerritory(int markId, int territoryId)
        {
            var fact = factRepository.FindAll()
                                 .FirstOrDefault(x => x.RefOIV.ID == markId 
                                                      && x.RefTerritory.ID == territoryId);
            
            if (fact == null)
            {
                fact = new F_OIV_REG10Qual
                           {
                               SourceID = extension.DataSourceOiv.ID,
                               RefOIV = marksRepository.FindOne(markId),
                               RefTerritory = territoryRepository.FindOne(territoryId),
                               RefStatusData = statusRepository.Get((int)OivStatus.OnEdit),
                               RefYear = extension.CurrentYear
                           };
            }

            return fact;
        }

        public IList<F_OIV_REG10Qual> GetMarksForOiv()
        {
            // Выбираем факты соответствующие показателям нашего ОИВ
            // До этого они должны быть проинициализированы в таблице фактов
            List<F_OIV_REG10Qual> facts = factRepository.FindAll()
                                                    .Where(x => x.RefYear == extension.CurrentYear 
                                                                && x.SourceID == extension.DataSourceOiv.ID
                                                                && x.RefTerritory == extension.RootTerritoryRf
                                                                && x.RefOIV.RefTypeMark.ID == (int)TypeMark.Gather
                                                                && (x.RefOIV.RefResponsOIV == extension.UserResponseOiv 
                                                                    || x.RefOIV.RefResponsOIV1 == extension.UserResponseOiv))
                                                    .OrderBy(x => x.RefOIV.Code)
                                                    .ToList();

            return facts;
        }

        public IList<F_OIV_REG10Qual> GetMarksForIMA()
        {
            // Выбираем факты соответствующие показателям нашего ОИВ
            // До этого они должны быть проинициализированы в таблице фактов
            List<F_OIV_REG10Qual> facts = factRepository.FindAll()
                                                    .Where(x => x.RefYear == extension.CurrentYear
                                                                && x.SourceID == extension.DataSourceOiv.ID
                                                                && x.RefTerritory == extension.RootTerritoryRf
                                                                && x.RefOIV.RefTypeMark.ID == (int)TypeMark.Gather)
                                                    .OrderBy(x => x.RefOIV.Code)
                                                    .ToList();

            return facts;
        }

        public IList<F_OIV_REG10Qual> GetMarks(D_Territory_RF territory)
        {
            // Выбираем факты соответствующие территории нашего ОМСУ
            // До этого они должны быть проинициализированы в таблице фактов
            List<F_OIV_REG10Qual> facts = factRepository.FindAll()
                                                    .Where(x => x.RefYear == extension.CurrentYear
                                                                && x.SourceID == extension.DataSourceOiv.ID
                                                                && x.RefTerritory == territory
                                                                && x.RefOIV.MO == true
                                                                && x.RefOIV.RefTypeMark.ID == (int)TypeMark.Gather)
                                                    .OrderBy(x => x.RefOIV.Code)
                                                    .ToList();

            return facts;
        }

        public IList<F_OIV_REG10Qual> GetTerritories(int markId)
        {
            // Выбираем факты соответствующие указанному показателю
            // До этого они должны быть проинициализированы в таблице фактов
            List<F_OIV_REG10Qual> facts = factRepository.FindAll()
                                                    .Where(x => x.RefYear == extension.CurrentYear
                                                                && x.SourceID == extension.DataSourceOiv.ID
                                                                && x.RefOIV.ID == markId)
                                                    .OrderBy(x => x.RefOIV.Code)
                                                    .ToList();

            return facts;
        }

        #region Implementation of IRepository<F_OMSU_Reg16>

        public F_OIV_REG10Qual Get(int id)
        {
            var result = factRepository.FindOne(id);
            if (result != null && result.SourceID == extension.DataSourceOiv.ID)
            {
                return result;
            }
            
            return null;
        }

        public IList<F_OIV_REG10Qual> GetAll()
        {
            return factRepository.FindAll().Where(x => x.SourceID == extension.DataSourceOiv.ID).ToList();
        }

        public void Save(F_OIV_REG10Qual entity)
        {
            if (entity.SourceID == extension.DataSourceOiv.ID)
            {
                factRepository.Save(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void Delete(F_OIV_REG10Qual entity)
        {
            if (entity.SourceID == extension.DataSourceOiv.ID)
            {
                factRepository.Delete(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID"); 
            }
        }

        #endregion
    }
}
