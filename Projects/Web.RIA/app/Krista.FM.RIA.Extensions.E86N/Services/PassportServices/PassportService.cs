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
    public class PassportService : RestService<F_Org_Passport, PassportViewModel>, IPassportService
    {
        private readonly IOkvedyService _okvedyService;
        private readonly IFilialService _filialService;

        public PassportService(
                ILinqRepository<F_Org_Passport> passportRepository,
                IOkvedyService okvedyService,
                IFilialService filialService)
            : base(passportRepository)
        {
            this._okvedyService = okvedyService;
            this._filialService = filialService;
        }

        public override IEnumerable<F_Org_Passport> GetItems(int? parentId)
        {
            return from p in GetRepository().FindAll()
                   where p.RefParametr.ID == parentId
                   select p;
        }

        public override void Delete(int id)
        {
            var okvedys = _okvedyService.GetItems(id);
            foreach (F_F_OKVEDY okvedy in okvedys)
            {
                _okvedyService.Delete(okvedy.ID);
            }

            var filials = _filialService.GetItems(id);
            foreach (F_F_Filial filial in filials)
            {
                _filialService.Delete(filial.ID);
            }

            base.Delete(id);
        }

        public override PassportViewModel ConvertToView(F_Org_Passport item)
        {
            throw new NotImplementedException();
        }

        public override F_Org_Passport DecodeJson(JsonObject json)
        {
            throw new NotImplementedException();
        }
    }
}
