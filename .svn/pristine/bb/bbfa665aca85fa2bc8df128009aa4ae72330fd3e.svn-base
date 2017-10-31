using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.Server.MessagesManager
{
    public class ObjectRepository : NHibernateLinqRepository<Objects>, IObjectRepository
    {
        public Objects GetObjectsByObjectKey(string objectKey)
        {
            return FindAll().FirstOrDefault(x => x.ObjectKey == objectKey);
        }
    }
}
