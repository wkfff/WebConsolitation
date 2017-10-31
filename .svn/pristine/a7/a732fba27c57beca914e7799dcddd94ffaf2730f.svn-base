using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;

namespace Krista.FM.Server.MessagesManager
{
    public interface IPermissionRepository
    {
        IQueryable<Permissions> GetPemissionsForObject(int objectId);

        IQueryable<Permissions> GetPermissionsForUser(int user, IList<int> memberships);
    }
}