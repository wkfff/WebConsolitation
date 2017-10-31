using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.Service2016Model;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Service2016
{
    public class Service2016InstitutionsInfoController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly Service2016InstitutionsInfoViewModel model = new Service2016InstitutionsInfoViewModel();

        public Service2016InstitutionsInfoController()
        {
            newRestService = Resolver.Get<INewRestService>();
        }
        
        public RestResult Read(int masterId)
        {
            return new RestResult
            {
                Success = true,
                Data = newRestService.GetItems<F_F_ServiceInstitutionsInfo>()
                        .Where(v => v.RefService.ID == masterId)
                        .Select(
                            v => new Service2016InstitutionsInfoViewModel
                            {
                                ID = v.ID,
                                RefStructure = v.RefStructure.ID,
                                RefStructureName = v.RefStructure.Name,
                                RefStructureInn = v.RefStructure.INN
                            })
            };
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data, int masterId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, masterId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var formData = JavaScriptDomainConverter<F_F_ServiceInstitutionsInfo>.DeserializeSingle(data);

                formData.RefService = newRestService.GetItem<D_Services_Service>(masterId);
                formData.RefStructure = newRestService.GetItem<D_Org_Structure>(formData.RefStructure.ID);

                string msg = "Запись обновлена";
                if (formData.ID < 0)
                {
                    formData.ID = 0;
                    msg = "Новая запись добавлена";
                }

                newRestService.Save(formData);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<F_F_ServiceInstitutionsInfo>()
                            .Where(v => v.ID == formData.ID)
                            .Select(
                                v => new Service2016InstitutionsInfoViewModel
                                {
                                    ID = v.ID,
                                    RefStructure = v.RefStructure.ID,
                                    RefStructureName = v.RefStructure.Name,
                                    RefStructureInn = v.RefStructure.INN
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id)
        {
            return newRestService.DeleteAction<F_F_ServiceInstitutionsInfo>(id);
        }

        public AjaxStoreResult GetStructures(int limit, int start, string query)
        {
            var data = newRestService.GetItems<D_Org_Structure>()
                .Where(
                    p => (p.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID) || p.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID) || p.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID)) &&
                         (p.INN.Contains(query) || p.KPP.Contains(query) || p.ShortName.Contains(query) || p.Name.Contains(query)));

            var result = data.Select(
                p => new
                {
                    p.ID,
                    p.INN,
                    p.Name
                });

            return new AjaxStoreResult(result.Skip(start).Take(limit), result.Count());
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "Учреждение \"{0}\" уже заведено<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => model.RefStructure))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.RefStructureName)));
            }

            var instInfo = newRestService.GetItems<F_F_ServiceInstitutionsInfo>().FirstOrDefault(
                x => !x.ID.Equals(record.GetValueToIntOrDefault(() => model.ID, -1))
                     && x.RefService.ID.Equals(masterId)
                     && x.RefStructure.ID.Equals(record.GetValueToIntOrDefault(() => model.RefStructure, -1)));

            if (instInfo != null)
            {
                message.Append(Msg2.FormatWith(instInfo.RefStructure.Name));
            }

            return message.ToString();
        }
    }
}
