using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class NHibernateRepository<T> : IRepository<T>
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

        public virtual void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public virtual IList<T> FindAll(IDictionary<string, object> propertyValuePairs)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(T));
            foreach (string key in propertyValuePairs.Keys)
            {
                if (propertyValuePairs[key] != null)
                {
                    criteria.Add(Restrictions.Eq(key, propertyValuePairs[key]));
                }
                else
                {
                    criteria.Add(Restrictions.IsNull(key));
                }
            }
            return criteria.List<T>();
        }

        public virtual T FindOne(IDictionary<string, object> propertyValuePairs)
        {
            IList<T> foundList = FindAll(propertyValuePairs);
            if (foundList.Count > 1)
            {
                throw new NonUniqueResultException(foundList.Count);
            }
            if (foundList.Count == 1)
            {
                return foundList[0];
            }
            return default(T);
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
