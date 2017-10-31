using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class RegionsRepository : IRegionsRepository
    {
        private readonly ILinqRepository<D_Regions_Analysis> regionsRepository;
        private readonly IMarksOmsuExtension extension;

        public RegionsRepository(
                                ILinqRepository<D_Regions_Analysis> regionsRepository, 
                                IMarksOmsuExtension extension)
        {
            this.regionsRepository = regionsRepository;
            this.extension = extension;
        }

        public IDbContext DbContext
        {
            get { return regionsRepository.DbContext; }
        }

        public void Delete(D_Regions_Analysis target)
        {
            if (target.SourceID == extension.DataSourceRegion.ID)
            {
                regionsRepository.Delete(target);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void Save(D_Regions_Analysis entity)
        {
            if (entity.SourceID == extension.DataSourceRegion.ID)
            {
                regionsRepository.Save(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void SaveAndEvict(D_Regions_Analysis entity)
        {
            if (entity.SourceID == extension.DataSourceRegion.ID)
            {
                regionsRepository.SaveAndEvict(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public D_Regions_Analysis FindOne(int id)
        {
            var result = regionsRepository.FindOne(id);
            if (result != null && result.SourceID == extension.DataSourceRegion.ID)
            {
                return result;
            }

            return null;
        }

        public D_Regions_Analysis FindOne(ILinqSpecification<D_Regions_Analysis> specification)
        {
            var result = regionsRepository.FindOne(specification);
            
            if (result != null && result.SourceID == extension.DataSourceRegion.ID)
            {
                return result;
            }

            return null;
        }

        public IQueryable<D_Regions_Analysis> FindAll()
        {
            return regionsRepository.FindAll().Where(x => x.SourceID == extension.DataSourceRegion.ID);
        }

        public IQueryable<D_Regions_Analysis> FindAll(ILinqSpecification<D_Regions_Analysis> specification)
        {
            return regionsRepository.FindAll(specification).Where(x => x.SourceID == extension.DataSourceRegion.ID);
        }

        public int GetDatasourceYear()
        {
            return Convert.ToInt32(extension.DataSourceRegion.Year);
        }
    }
}
