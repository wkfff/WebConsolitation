using System.Collections.Generic;
using NHibernate;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public abstract class BusinessBaseType<TEntity>
       where TEntity : class, new()
    {
        protected BusinessBaseType()
        {
        }

        public TEntity Load(object pk)
        {
            using (ISession session = NHibernateSession.Current)
            {
                return (TEntity)session.Load(typeof(TEntity), pk);
            }
        }

        public void Delete(object pk)
        {
            using (ISession session = NHibernateSession.Current)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(pk);
                    transaction.Commit();
                }
            }
        }

        public void Save(object obj)
        {
            using (ISession session = NHibernateSession.Current)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(obj);
                    transaction.Commit();
                }
            }
        }

        public List<TEntity> ToList()
        {
            List<TEntity> resultList = new List<TEntity>();

            using (ISession session = NHibernateSession.Current)
            {
                var objects = session
                    .CreateCriteria(typeof(TEntity))
                    .List();

                foreach (object obj in objects)
                {
                    resultList.Add((TEntity)obj);
                }
            }

            return resultList;
        }
    }
}
