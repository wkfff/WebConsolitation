using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastRepositiory<T> : ILinqRepository<T>
    {
        protected readonly ILinqRepository<T> Repository;

        public ForecastRepositiory(ILinqRepository<T> repository)
        {
            this.Repository = repository;
        }

        public IDbContext DbContext
        {
            get { return Repository.DbContext; }
        }

        public IQueryable<T> FindAll(ILinqSpecification<T> specification)
        {
            return Repository.FindAll(specification);
        }
        
        public T FindOne(ILinqSpecification<T> specification)
        {
            return Repository.FindOne(specification);
        }

        public IQueryable<T> FindAll()
        {
            return Repository.FindAll();
        }

        public void Save(T entity)
        {
            Repository.Save(entity);
        }

        public void SaveAndEvict(T entity)
        {
            Repository.SaveAndEvict(entity);
        }

        public T FindOne(int id)
        {
            return Repository.FindOne(id);
        }

        public void Delete(T entity)
        {
            Repository.Delete(entity);
        }
    }
}
