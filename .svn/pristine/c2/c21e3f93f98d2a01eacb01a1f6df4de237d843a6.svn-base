using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.PfhdService
{
    public class PfhdService : NewRestService, IPfhdService
    {
        public IEnumerable<F_Fin_finActPlan> GetItems(int? parentId)
        {
            if (parentId == null)
            {
                throw new InvalidDataException("PfhdService::GetItems: Не указан родительский документ");
            }

            // Нужно проверить, существуют ли все три записи и создать недостающие.
            var rows = from p in GetRepository<F_Fin_finActPlan>().FindAll()
                       where p.RefParametr.ID == parentId
                       select p;

            if (!rows.Any(x => x.NumberStr == 0))
            {
                FillNew(parentId.Value, 0);
            }

            if (!rows.Any(x => x.NumberStr == 1))
            {
                FillNew(parentId.Value, 1);
            }

            if (!rows.Any(x => x.NumberStr == 2))
            {
                FillNew(parentId.Value, 2);
            }

            return from p in GetRepository<F_Fin_finActPlan>().FindAll()
                   where p.RefParametr.ID == parentId
                   select p;
        }

        public void Delete(int id)
        {
            int parameterDoc = GetItem<F_Fin_finActPlan>(id).RefParametr.ID;
            DeleteAll(GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == parameterDoc));
            DeleteAll(GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == parameterDoc));
            DeleteAll(GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == parameterDoc));
            Delete<F_Fin_finActPlan>(id);
        }

        private void DeleteAll<T>(IEnumerable<T> unknown) where T : DomainObject
        {
            foreach (var item in unknown)
            {
                Delete<T>(item.ID);
            }
        }

        private void FillNew(int parentId, int numStr)
        {
            try
            {
                var finActPlan =
                    new F_Fin_finActPlan
                        {
                            SourceID = 0, 
                            TaskID = 0, 
                            RefParametr = Load<F_F_ParameterDoc>(parentId), 
                            totnonfinAssets = 0, 
                            realAssets = 0, 
                            highValPersAssets = 0, 
                            finAssets = 0, 
                            income = 0, 
                            expense = 0, 
                            finCircum = 0, 
                            kreditExpir = 0, 
                            planInpayments = 0, 
                            stateTaskGrant = 0, 
                            actionGrant = 0, 
                            budgetaryFunds = 0, 
                            paidServices = 0, 
                            planPayments = 0, 
                            labourRemuneration = 0, 
                            telephoneServices = 0, 
                            freightServices = 0, 
                            publicServeces = 0, 
                            rental = 0, 
                            maintenanceCosts = 0, 
                            mainFunds = 0, 
                            fictitiousAssets = 0, 
                            tangibleAssets = 0, 
                            publish = 0, 
                            NumberStr = numStr
                        };
                Save(finActPlan);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("PfhdService::FillNew: Ошибка заполнения нового Плана ФХД: " + e.Message, e);
            }
        }
    }
}
