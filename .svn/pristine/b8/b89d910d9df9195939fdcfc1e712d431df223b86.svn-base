using System.Linq;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        [UnitOfWork]
        private void FixNsiOgs()
        {
            var ogsRepository = Resolver.Get<ILinqRepository<D_Org_NsiOGS>>();
            var founderRepository = Resolver.Get<ILinqRepository<D_Org_OrgYchr>>();
            
            var problems = ogsRepository.FindAll()
                .GroupBy(nsiOgs => nsiOgs.regNum)
                .Where(g => g.Count() > 1)
                .Select(g => new { g.Key, uniqueMaxId = g.Max(ogs => ogs.ID) })
                .ToDictionary(arg => arg.Key, arg => arg.uniqueMaxId);

            var founderProblems = founderRepository.FindAll()
                .Where(ychr => problems.Keys.Contains(ychr.RefNsiOgs.regNum))
                .Where(ychr => !problems.Values.Contains(ychr.RefNsiOgs.ID))
                .ToList();

            ogsRepository.DbContext.BeginTransaction();

            foreach (D_Org_OrgYchr founderProblem in founderProblems)
            {
                founderProblem.RefNsiOgs = ogsRepository.Load(problems[founderProblem.RefNsiOgs.regNum]);
                founderRepository.Save(founderProblem);
            }

            ogsRepository.FindAll()
                .Where(nsiOgs => problems.Keys.Contains(nsiOgs.regNum))
                .Where(nsiOgs => !problems.Values.Contains(nsiOgs.ID))
                .Each(ogsRepository.Delete);

            ogsRepository.DbContext.CommitChanges();
            ogsRepository.DbContext.CommitTransaction();
        }
    }
}
