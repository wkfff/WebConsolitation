using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class MarksRepository : IMarksRepository
    {
        private readonly ILinqRepository<D_OIV_Mark> marksRepository;
        private readonly IRegion10MarksOivExtension extension;

        public MarksRepository(
                                ILinqRepository<D_OIV_Mark> marksRepository, 
                                IRegion10MarksOivExtension extension)
        {
            this.marksRepository = marksRepository;
            this.extension = extension;
        }

        public IDbContext DbContext
        {
            get { return marksRepository.DbContext; }
        }

        public void Delete(D_OIV_Mark target)
        {
            if (target.SourceID == extension.DataSourceOiv.ID)
            {
                marksRepository.Delete(target);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void Save(D_OIV_Mark entity)
        {
            if (entity.SourceID == extension.DataSourceOiv.ID)
            {
                marksRepository.Save(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void SaveAndEvict(D_OIV_Mark entity)
        {
            if (entity.SourceID == extension.DataSourceOiv.ID)
            {
                marksRepository.SaveAndEvict(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public D_OIV_Mark FindOne(int id)
        {
            var result = marksRepository.FindOne(id);
            if (result != null && result.SourceID == extension.DataSourceOiv.ID)
            {
                return result;
            }

            return null;
        }

        public D_OIV_Mark FindOne(ILinqSpecification<D_OIV_Mark> specification)
        {
            var result = marksRepository.FindOne(specification);
            
            if (result != null && result.SourceID == extension.DataSourceOiv.ID)
            {
                return result;
            }

            return null;
        }

        public IQueryable<D_OIV_Mark> FindAll()
        {
            return marksRepository.FindAll().Where(x => x.SourceID == extension.DataSourceOiv.ID);
        }

        public IQueryable<D_OIV_Mark> FindAll(ILinqSpecification<D_OIV_Mark> specification)
        {
            return marksRepository.FindAll(specification).Where(x => x.SourceID == extension.DataSourceOiv.ID);
        }

        public int GetDatasourceYear()
        {
            return Convert.ToInt32(extension.DataSourceOiv.Year);
        }
    }
}
