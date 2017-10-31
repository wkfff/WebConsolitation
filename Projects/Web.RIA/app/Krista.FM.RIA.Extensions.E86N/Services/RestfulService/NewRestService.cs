using System;
using System.Linq;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;

using NHibernate;

namespace Krista.FM.RIA.Extensions.E86N.Services.RestfulService
{
    public class NewRestService : INewRestService
    {
        private readonly ICacheRepositoryService cacheRepositoryService;

        public NewRestService()
        {
            cacheRepositoryService = Resolver.Get<ICacheRepositoryService>();
        }

        #region INewRestService Members
        
        public bool HaveTransaction
        {
            get
            {
                return Session.Transaction.IsActive;
            }
        }

        private static ISession Session
        {
            get
            {
                return NHibernateSession.Current;
            }
        }

        public virtual ILinqRepository<TDomain> GetRepository<TDomain>() where TDomain : DomainObject
        {
            return cacheRepositoryService.GetRepository<TDomain>();
        }

        public virtual TDomain GetItem<TDomain>(int id) where TDomain : DomainObject
        {
            return cacheRepositoryService.GetRepository<TDomain>().FindOne(id);
        }

        public TDomain GetItem<TDomain>(int? id) where TDomain : DomainObject
        {
            return id.HasValue  ? cacheRepositoryService.GetRepository<TDomain>().FindOne(id.Value) : null;
        }

        public virtual TDomain Load<TDomain>(int id) where TDomain : DomainObject
        {
            return cacheRepositoryService.GetRepository<TDomain>().Load(id);
        }

        public virtual IQueryable<TDomain> GetItems<TDomain>() where TDomain : DomainObject
        {
            return cacheRepositoryService.GetRepository<TDomain>().FindAll();
        }

        public virtual IQueryable<TDomain> GetItems<TDomain>(int parentId) where TDomain : DomainObject
        {
            return cacheRepositoryService.GetRepository<TDomain>().FindAll();
        }

        public virtual void Save<TDomain>(TDomain item) where TDomain : DomainObject
        {
            try
            {
                cacheRepositoryService.GetRepository<TDomain>().Save(item);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка сохранения записи: " + e.Message, e);
            }
        }

        public virtual void Delete<TDomain>(int id) where TDomain : DomainObject
        {
            try
            {
                cacheRepositoryService.GetRepository<TDomain>().Delete(GetItem<TDomain>(id));
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления записи: " + e.Message, e);
            }
        }

        public void Delete<TDomain>(TDomain entity) where TDomain : DomainObject
        {
            try
            {
                cacheRepositoryService.GetRepository<TDomain>().Delete(entity);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления записи: " + e.Message, e);
            }
        }

        public RestResult DeleteAction<TDomain>(int id) where TDomain : DomainObject
        {
            try
            {
                BeginTransaction();

                Delete<TDomain>(id);
                
                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (HaveTransaction)
                {
                    CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + e.ExpandException());

                if (HaveTransaction)
                {
                    RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        public RestResult DeleteDocDetailAction<TDomain>(int id, int docId) where TDomain : DomainObject
        {
            try
            {
                BeginTransaction();

                Delete<TDomain>(id);
                var logService = Resolver.Get<IChangeLogService>();
                logService.WriteDeleteDocDetail(GetItem<F_F_ParameterDoc>(docId));

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (HaveTransaction)
                {
                    CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteDocDetailAction: " + e.Message + " : " + e.ExpandException());

                if (HaveTransaction)
                {
                    RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public void CommitChanges()
        {
            try
            {
                Session.Flush();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка применения изменений(CommitChanges): " + e.Message, e);
            }
        }
        
        public void BeginTransaction()
        {
            try
            {
                Session.BeginTransaction();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка открытия транзакции(BeginTransaction): " + e.Message, e);
            }
        }

        public void CommitTransaction()
        {
            try
            {
                Session.Transaction.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка фиксирования транзакции(CommitTransaction): " + e.Message, e);
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                Session.Transaction.Rollback();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка отката транзакции(RollbackTransaction): " + e.Message, e);
            }
        }

        #endregion
    }
}