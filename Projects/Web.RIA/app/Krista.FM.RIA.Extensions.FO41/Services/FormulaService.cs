using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class FormulaService
    {
        private readonly ILinqRepository<D_Formula_DataPrivileg> repository;

        public FormulaService(ILinqRepository<D_Formula_DataPrivileg> repository)
        {
            this.repository = repository;
        }

        public IDbContext DbContext
        {
            get { return repository.DbContext; }
        }

        /// <summary>
        /// Возвращает формулы по показателю для четырех периодов
        /// </summary>
        /// <param name="indicId">Id показателя</param>
        /// <param name="namePeriod">Наименование периода (Prev, Fact, Estimate, Forecast)</param>
        public string GetForIndicator(int indicId, string namePeriod)
        {
            var fls = repository.FindAll()
                .Where(f => f.RefMarksPrivilege.ID == indicId && f.NamePeriod.Equals(namePeriod));
            return fls.Count() > 0 ? fls.First().Formula : string.Empty;
        }
    }
}
