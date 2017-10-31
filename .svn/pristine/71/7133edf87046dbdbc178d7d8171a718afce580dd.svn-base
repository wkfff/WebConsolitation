using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class TerritoryRepository : ITerritoryRepository
    {
        private readonly ILinqRepository<D_Territory_RF> territoryRepository;
        
        public TerritoryRepository(ILinqRepository<D_Territory_RF> territoryRepository)
        {
            this.territoryRepository = territoryRepository;
        }

        public IDbContext DbContext
        {
            get { return territoryRepository.DbContext; }
        }

        public void Delete(D_Territory_RF target)
        {
            throw new NotImplementedException();
        }

        public void Save(D_Territory_RF entity)
        {
            throw new NotImplementedException();
        }

        public void SaveAndEvict(D_Territory_RF entity)
        {
            throw new NotImplementedException();
        }

        public D_Territory_RF FindOne(int id)
        {
            var result = territoryRepository.FindOne(id);
            return result;
        }

        public D_Territory_RF FindOne(ILinqSpecification<D_Territory_RF> specification)
        {
            var result = territoryRepository.FindOne(specification);
            return result;
        }

        public IQueryable<D_Territory_RF> FindAll()
        {
            return territoryRepository.FindAll();
        }

        public IQueryable<D_Territory_RF> FindAll(ILinqSpecification<D_Territory_RF> specification)
        {
            return territoryRepository.FindAll(specification);
        }
    }
}
