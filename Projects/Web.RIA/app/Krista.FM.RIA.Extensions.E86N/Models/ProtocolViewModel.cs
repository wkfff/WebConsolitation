using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using bus.gov.ru;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Models.Abstract;
using Krista.FM.RIA.Extensions.E86N.Services.Params;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    public class ProtocolViewModel : AbstractModelBase<ProtocolViewModel, DomainObject>
    {
        [Description("Результат операции")]
        public string ProtocolResult { get; set; }

        [Description("Тип")]
        public string Code { get; set; }
        
        [Description("Наименование")]
        public string Name { get; set; }

        [Description("Описание")]
        public string Description { get; set; }

        public override string ValidateData()
        {
            throw new NotImplementedException();
        }

        public override ProtocolViewModel GetModelByDomain(DomainObject domain)
        {
            throw new NotImplementedException();
        }

        public override DomainObject GetDomainByModel()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProtocolViewModel> GetModelData(NameValueCollection paramsList)
        {
            var protocol = Resolver.Get<IParamsMap>().GetParam<IDataPumpProtocolProvider>("Protocol");
            var protocolResult = protocol.Confirmation.body.result;

            return protocol.Confirmation.body.violation
                        .Select((x, i) => new ProtocolViewModel
                                        {
                                            ID = i,
                                            ProtocolResult = protocolResult,
                                            Code = x.code,
                                            Name = x.name,
                                            Description = x.description
                                        }).AsQueryable();
        }
    }
}
