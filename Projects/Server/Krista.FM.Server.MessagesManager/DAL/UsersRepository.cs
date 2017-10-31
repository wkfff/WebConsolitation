using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.Server.MessagesManager
{
    public class MembershipsRepository : NHibernateLinqRepository<Memberships>, IMembershipsRepository
    {
        public IQueryable<Memberships> GetGroupsForUser(int userId)
        {
            return FindAll().Where(x => x.RefUsers.ID == userId);
        }

        public IQueryable<Memberships> GetUsersForGroup(int groupId)
        {
            return FindAll().Where(x => x.RefGroups.ID == groupId);
        }
    }
}
