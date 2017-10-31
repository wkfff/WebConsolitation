using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class MarksOmsuRepository : IMarksOmsuRepository
    {
        private readonly ILinqRepository<F_OMSU_Reg16> repository;
        private readonly IMarksOmsuExtension extension;
        private readonly IMarksRepository marksRepository;
        private readonly IRegionsRepository regionRepository;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;

        public MarksOmsuRepository(
                                    ILinqRepository<F_OMSU_Reg16> repository,
                                    IMarksOmsuExtension extension,
                                    IMarksRepository marksRepository,
                                    IRegionsRepository regionRepository,
                                    IRepository<FX_OMSU_StatusData> statusRepository)
        {
            this.repository = repository;
            this.extension = extension;
            this.marksRepository = marksRepository;
            this.regionRepository = regionRepository;
            this.statusRepository = statusRepository;
        }

        public IDbContext DbContext
        {
            get { return repository.DbContext; }
        }

        /// <summary>
        /// Возвращает значение показателя для заданного района.
        /// </summary>
        /// <param name="markId">Id показателя.</param>
        /// <param name="regionId">Id района.</param>
        public F_OMSU_Reg16 GetFactForMarkRegion(int markId, int regionId)
        {
            var fact = repository.FindAll()
                                 .FirstOrDefault(x => x.RefMarksOMSU.ID == markId && x.RefRegions.ID == regionId);
            
            if (fact == null)
            {
                fact = new F_OMSU_Reg16
                           {
                               SourceID = extension.DataSourceOmsu.ID,
                               RefMarksOMSU = marksRepository.FindOne(markId),
                               RefRegions = regionRepository.FindOne(regionId),
                               RefStatusData = statusRepository.Get((int)OMSUStatus.OnEdit),
                               RefYearDayUNV = extension.CurrentYearUNV
                           };
            }

            return fact;
        }

        /// <summary>
        /// Возвращает данные показателя для Ответственного ОИВ в разрезе районов.
        /// </summary>
        /// <param name="markId">Id показателя, данные по которому нужно вернуть.</param>
        public IList<F_OMSU_Reg16> GetForOIV(int markId)
        {
            var year = extension.CurrentYearUNV;
            var sourceId = extension.DataSourceOmsu.ID;
            
            // Выбираем факты соответствующие заданному показателю
            // До этого они должны быть проинициализированы в таблице фактов
            List<F_OMSU_Reg16> facts = repository.FindAll()
                .Where(x => x.RefMarksOMSU.ID == markId && x.RefYearDayUNV == year && x.SourceID == sourceId)
                .ToList();

            return facts;
        }

        public IList<F_OMSU_Reg16> GetForOIVPrevious(int markId)
        {
            var mark = marksRepository.FindOne(markId);

            if (extension.DataSourceOmsuPrevious == null)
            {
                return new List<F_OMSU_Reg16>();
            }

            FX_Date_YearDayUNV year = extension.PreviousYearUNV;
            int sourceId = extension.DataSourceOmsuPrevious.ID;
            
            // Выбираем факты соответствующие заданному показателю в прошлом году
            List<F_OMSU_Reg16> facts = repository.FindAll()
                .Where(x => x.RefMarksOMSU.RefMarksB == mark.RefMarksB 
                           && x.RefYearDayUNV == year 
                           && x.SourceID == sourceId)
                .ToList();

            return facts;
        }

        /// <summary>
        /// Возвращает данные по всем показателям для выбранного района (МО).
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        public IList<F_OMSU_Reg16> GetForMO(D_Regions_Analysis region)
        {
            return GetForMO(region, -1);
        }

        /// <summary>
        /// Возвращает данные по дочерним показателям указанного показателя для выбранного района (МО).
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        /// <param name="markId">Id родительского показателя.</param>
        public IList<F_OMSU_Reg16> GetForMO(D_Regions_Analysis region, int markId)
        {
            if (region == null)
            {
                return new List<F_OMSU_Reg16>();
            }

            var year = extension.CurrentYearUNV;
            var sourceId = extension.DataSourceOmsu.ID;

            // Выбираем факты соответствующие найденым показателям
            // Выбираем все показатели верхнего уровня или подчиненные markId
            List<F_OMSU_Reg16> facts;
            if (markId == -1)
            {
                facts = (from f in repository.FindAll()
                         where f.RefRegions == region
                               && f.RefYearDayUNV == year
                               && f.SourceID == sourceId
                               && (from m in marksRepository.FindAll()
                                   where m.RefTypeMark.ID == (int)TypeMark.Gather
                                         && m.ParentID == null
                                         && f.RefMarksOMSU == m
                                   select m).Any()
                         orderby f.RefMarksOMSU.Code
                         select f)
                         .ToList();
            }
            else
            {
                facts = (from f in repository.FindAll()
                         where f.RefRegions == region
                               && f.RefYearDayUNV == year
                               && f.SourceID == sourceId
                               && (from m in marksRepository.FindAll()
                                   where m.RefTypeMark.ID == (int)TypeMark.Gather
                                         && m.ParentID == markId
                                         && f.RefMarksOMSU == m
                                   select m).Any()
                         orderby f.RefMarksOMSU.Code
                         select f)
                         .ToList();
            }

            return facts;
        }

        public IList<F_OMSU_Reg16> GetForMOPrevious(D_Regions_Analysis region, int markId)
        {
            if (region == null)
            {
                return new List<F_OMSU_Reg16>();
            }

            if (extension.DataSourceOmsuPrevious == null)
            {
                return new List<F_OMSU_Reg16>();
            }

            var year = extension.PreviousYearUNV;
            var sourceId = extension.DataSourceOmsuPrevious.ID;

            // Выбираем факты соответствующие найденым показателям
            // Выбираем все показатели верхнего уровня
            List<F_OMSU_Reg16> facts;
            if (markId == -1)
            {
                facts = (from f in repository.FindAll()
                         where f.RefRegions.RefBridgeRegions == region.RefBridgeRegions
                               && f.RefYearDayUNV == year
                               && f.SourceID == sourceId
                               && (from m in marksRepository.FindAll()
                                   where m.RefTypeMark.ID == (int)TypeMark.Gather
                                         && m.ParentID == null
                                         && f.RefMarksOMSU.RefMarksB == m.RefMarksB
                                   select m).Any()
                         select f)
                         .ToList();
            }
            else
            {
                // Выбираем все показатели подчиненные markId
                facts = (from f in repository.FindAll()
                         where f.RefRegions.RefBridgeRegions == region.RefBridgeRegions
                               && f.RefYearDayUNV == year
                               && f.SourceID == sourceId
                               && (from m in marksRepository.FindAll()
                                   where m.RefTypeMark.ID == (int)TypeMark.Gather
                                         && m.ParentID == markId
                                         && f.RefMarksOMSU.RefMarksB == m.RefMarksB
                                   select m).Any()
                         select f)
                         .ToList();
            }

            return facts;
        }

        /// <summary>
        /// Возвращает данные по всем показателям для выбранного района (МО) без учета иерархии.
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        public IList<F_OMSU_Reg16> GetAllMarksForMO(D_Regions_Analysis region)
        {
            var year = extension.CurrentYearUNV;
            var sourceId = extension.DataSourceOmsu.ID;

            // Выбираем факты соответствующие найденым показателям
            // Выбираем все показатели верхнего уровня или подчиненные markId
            List<F_OMSU_Reg16> facts = (from f in repository.FindAll()
                                        where f.RefRegions == region
                                              && f.RefYearDayUNV == year
                                              && f.SourceID == sourceId
                                              && (from m in marksRepository.FindAll()
                                                  where m.RefTypeMark.ID == (int)TypeMark.Gather
                                                        && f.RefMarksOMSU == m
                                                  select m).Any()
                                        orderby f.RefMarksOMSU.Code
                                        select f)
                         .ToList();
            return facts;
        }

        public IList<F_OMSU_Reg16> GetAllMarksForMOPrevious(D_Regions_Analysis region)
        {
            if (extension.DataSourceOmsuPrevious == null)
            {
                return new List<F_OMSU_Reg16>();
            }

            var year = extension.PreviousYearUNV;
            var sourceId = extension.DataSourceOmsuPrevious.ID;

            // Выбираем факты соответствующие найденым показателям
            // Выбираем все показатели верхнего уровня или подчиненные markId
            List<F_OMSU_Reg16> facts = (from f in repository.FindAll()
                                        where f.RefRegions.RefBridgeRegions == region.RefBridgeRegions
                                              && f.RefYearDayUNV == year
                                              && f.SourceID == sourceId
                                              && (from m in marksRepository.FindAll()
                                                  where m.RefTypeMark.ID == (int)TypeMark.Gather
                                                        && f.RefMarksOMSU.RefMarksB == m.RefMarksB
                                                  select m).Any()
                                        select f)
                         .ToList();
            return facts;
        }

        public IEnumerable<F_OMSU_Reg16> GetCurrentYearFactsOfAllRegions(int markId)
        {
            return repository.FindAll()
                .Where(x => x.RefMarksOMSU.ID == markId && x.RefYearDayUNV == extension.CurrentYearUNV)
                .OrderBy(x => x.RefRegions.CodeLine);
        }

        #region Implementation of IRepository<F_OMSU_Reg16>

        public F_OMSU_Reg16 Get(int id)
        {
            var result = repository.FindOne(id);
            if (result != null && result.SourceID == extension.DataSourceOmsu.ID)
            {
                return result;
            }
            
            return null;
        }

        public IList<F_OMSU_Reg16> GetAll()
        {
            return repository.FindAll().Where(x => x.SourceID == extension.DataSourceOmsu.ID).ToList();
        }

        public void Save(F_OMSU_Reg16 entity)
        {
            if (entity.SourceID == extension.DataSourceOmsu.ID)
            {
                repository.Save(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void Delete(F_OMSU_Reg16 entity)
        {
            if (entity.SourceID == extension.DataSourceOmsu.ID)
            {
                repository.Delete(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID"); 
            }
        }

        #endregion
    }
}
