using System;
using System.Collections.Generic;
using System.Linq;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.PassportServices
{
    public class FilialService : RestService<F_F_Filial, FilialViewModel>, IFilialService
    {
        public FilialService(ILinqRepository<F_F_Filial> repository) : base(repository)
        {
        }

        public override IEnumerable<F_F_Filial> GetItems(int? parentId)
        {
            return from p in GetRepository().FindAll()
                   where p.RefPassport.ID == parentId
                   select p;
        }

        public override FilialViewModel ConvertToView(F_F_Filial item)
        {
            throw new NotImplementedException();
        }

        public override F_F_Filial DecodeJson(JsonObject json)
        {
            throw new NotImplementedException();
        }
    }
}
