using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.Server.MessagesManager
{
    public interface IMessageRepository : ILinqRepository<Message>
    {
        IQueryable<Message> GetObsoleteMessages();

        IQueryable<Message> GetUserMessages(int userId);
    }
}