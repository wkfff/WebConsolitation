using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.Server.MessagesManager
{
    public class PermissionRepository : NHibernateLinqRepository<Permissions>, IPermissionRepository
    {
        public IQueryable<Permissions> GetPemissionsForObject(int objectId)
        {
            return FindAll().Where(x => x.RefObjects.ID == objectId
                                        || x.RefObjects.Name == "AllMessages");
        }

        public IQueryable<Permissions> GetPermissionsForUser(int user, IList<int> memberships)
        {
            return FindAll().Where(x => (x.RefGroups != null && memberships.Contains(x.RefGroups.ID))
                                   || (x.RefUsers != null && x.RefUsers.ID == user));
        }
    }
}
