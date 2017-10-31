using System.Linq;
using Krista.FM.Domain;

namespace Krista.FM.Server.MessagesManager
{
    public interface IMembershipsRepository
    {
        IQueryable<Memberships> GetGroupsForUser(int userId);

        IQueryable<Memberships> GetUsersForGroup(int groupId);
    }
}