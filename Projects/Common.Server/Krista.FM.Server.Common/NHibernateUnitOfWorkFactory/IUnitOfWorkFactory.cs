using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
