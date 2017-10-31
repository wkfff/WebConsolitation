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
    public class OkvedyService : RestService<F_F_OKVEDY, PassportOkvedyViewModel>, IOkvedyService
    {
        public OkvedyService(ILinqRepository<F_F_OKVEDY> okvedyRepository)
            : base(okvedyRepository)
        {
        }

        public override IEnumerable<F_F_OKVEDY> GetItems(int? parentId)
        {
            return from p in GetRepository().FindAll()
                   where p.RefPassport.ID == parentId
                   select p;
        }

        public override PassportOkvedyViewModel ConvertToView(F_F_OKVEDY item)
        {
            throw new NotImplementedException();
        }

        public override F_F_OKVEDY DecodeJson(JsonObject json)
        {
            throw new NotImplementedException();
        }
    }
}
