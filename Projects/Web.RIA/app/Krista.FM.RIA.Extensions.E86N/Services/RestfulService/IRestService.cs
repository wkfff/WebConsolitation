using System.Collections.Generic;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Models;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface IRestService<TDomain, TViewModel>
        where TDomain : DomainObject
        where TViewModel : ViewModelBase
    {
        void                        SetRepository(ILinqRepository<TDomain> repository);
        ILinqRepository<TDomain>    GetRepository();

        TDomain                     GetItem(int id);
        IEnumerable<TDomain>        GetItems(int? id);

        TViewModel                  ConvertToView(TDomain item);
        IEnumerable<TViewModel>     ConvertToView(IEnumerable<TDomain> items);

        TDomain                     Save(TDomain item);
        void                        Delete(int id);

        TDomain                     DecodeJson(JsonObject json);
    }
}
