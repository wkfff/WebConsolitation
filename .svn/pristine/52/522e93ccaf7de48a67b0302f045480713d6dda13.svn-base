using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class MarksRepository : IMarksRepository
    {
        private readonly ILinqRepository<D_OMSU_MarksOMSU> marksRepository;
        private readonly IMarksOmsuExtension extension;

        public MarksRepository(
                                ILinqRepository<D_OMSU_MarksOMSU> marksRepository, 
                                IMarksOmsuExtension extension)
        {
            this.marksRepository = marksRepository;
            this.extension = extension;
        }

        public IDbContext DbContext
        {
            get { return marksRepository.DbContext; }
        }

        public void Delete(D_OMSU_MarksOMSU target)
        {
            if (target.SourceID == extension.DataSourceOmsu.ID)
            {
                marksRepository.Delete(target);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void Save(D_OMSU_MarksOMSU entity)
        {
            if (entity.SourceID == extension.DataSourceOmsu.ID)
            {
                marksRepository.Save(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public void SaveAndEvict(D_OMSU_MarksOMSU entity)
        {
            if (entity.SourceID == extension.DataSourceOmsu.ID)
            {
                marksRepository.SaveAndEvict(entity);
            }
            else
            {
                throw new KeyNotFoundException("Неверное значение SourceID");
            }
        }

        public D_OMSU_MarksOMSU FindOne(int id)
        {
            var result = marksRepository.FindOne(id);
            if (result != null && result.SourceID == extension.DataSourceOmsu.ID)
            {
                return result;
            }

            return null;
        }

        public D_OMSU_MarksOMSU FindOne(ILinqSpecification<D_OMSU_MarksOMSU> specification)
        {
            var result = marksRepository.FindOne(specification);
            
            if (result != null && result.SourceID == extension.DataSourceOmsu.ID)
            {
                return result;
            }

            return null;
        }

        public IQueryable<D_OMSU_MarksOMSU> FindAll()
        {
            return marksRepository.FindAll().Where(x => x.SourceID == extension.DataSourceOmsu.ID);
        }

        public IQueryable<D_OMSU_MarksOMSU> FindAll(ILinqSpecification<D_OMSU_MarksOMSU> specification)
        {
            return marksRepository.FindAll(specification).Where(x => x.SourceID == extension.DataSourceOmsu.ID);
        }

        public int GetDatasourceYear()
        {
            return Convert.ToInt32(extension.DataSourceOmsu.Year);
        }
    }
}
