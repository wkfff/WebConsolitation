using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class NHibernateLinqRepository<T> : IRepository<T>, ILinqRepository<T>
    {
        private IDbContext dbContext;

        protected virtual ISession Session
        {
            get { return NHibernateSession.Current; }
        }

        #region IRepository Members

        public virtual T Get(int id)
        {
            return Session.Get<T>(id);
        }

        public T Load(int id)
        {
            return Session.Load<T>(id);
        }

        public virtual IList<T> GetAll()
        {
            return Session.CreateCriteria(typeof(T)).List<T>();
        }

        public virtual void Save(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void SaveAndEvict(T entity)
        {
            Save(entity);
            Session.Evict(entity);
        }

        public virtual void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public IQueryable<T> FindAll()
        {
            return Queryable.AsQueryable<T>(Session.Query<T>());
        }

        public IQueryable<T> FindAll(ILinqSpecification<T> specification)
        {
            return specification.SatisfyingElementsFrom(Session.Query<T>());
        }

        public T FindOne(int id)
        {
            return Session.Get<T>(id);
        }

        public T FindOne(ILinqSpecification<T> specification)
        {
            return specification.SatisfyingElementsFrom(Session.Query<T>()).SingleOrDefault();
        }

        #endregion

        public virtual IDbContext DbContext
        {
            get
            {
                return dbContext ?? (dbContext = new DbContext());
            }
        }
    }
}
