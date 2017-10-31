using System;
using System.Collections.Generic;
using System.Linq;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Models;

namespace Krista.FM.RIA.Extensions.E86N.Services.RestfulService
{
    abstract public class RestService<TDomain, TViewModel> : 
        IRestService<TDomain, TViewModel>
        where TDomain : DomainObject
        where TViewModel : ViewModelBase
    {
        protected ILinqRepository<TDomain> Repository;

        protected RestService(ILinqRepository<TDomain> repository)
        {
            Repository = repository;
        }

        public void SetRepository(ILinqRepository<TDomain> repository)
        {
            Repository = repository;
        }

        public ILinqRepository<TDomain> GetRepository()
        {
            return Repository;
        }

        public TDomain GetItem(int id)
        {
            return Repository.FindOne(id);
        }

        public virtual IEnumerable<TDomain> GetItems(int? parentId)
        {
            return Repository.FindAll();
        }

        abstract public TViewModel ConvertToView(TDomain item);

        public IEnumerable<TViewModel> ConvertToView(IEnumerable<TDomain> items)
        {
            return items.Select(ConvertToView);
        }

        public TDomain Save(TDomain item)
        {
            try
            {
                Repository.Save(item);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception("RestService::Save: Ошибка сохранения записи: " + e.Message, e);
            }
        }

        public virtual void Delete(int id)
        {
            try
            {
                Repository.Delete(Repository.FindOne(id));
            }
            catch (Exception e)
            {
                throw new Exception("RestService::Delete: Ошибка удаления записи: " + e.Message, e);
            }
        }

        abstract public TDomain DecodeJson(JsonObject json);
    }
}
