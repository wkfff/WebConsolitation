namespace Krista.FM.Domain.Reporitory
{
    public interface IDbContext
    {
        void BeginTransaction();
        void CommitChanges();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
