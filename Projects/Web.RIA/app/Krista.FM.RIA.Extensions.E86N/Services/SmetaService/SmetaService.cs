using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Services.SmetaService
{
    public class SmetaService : RestService<F_Fin_Smeta, SmetaViewModel>, ISmetaService
    {
        private readonly ILinqRepository<F_F_ParameterDoc> parDocRepository;
        private readonly ILinqRepository<D_KOSGY_KOSGY> kosguRepository;
        private readonly ILinqRepository<D_Fin_RazdPodr> razdPodrRepository;
        private readonly ILinqRepository<D_Fin_VidRash> vidRashRepository;
        private readonly ILinqRepository<D_Fin_nsiBudget> budgetRepository;
       
        public SmetaService(
                ILinqRepository<F_Fin_Smeta> smetaRepository,
                ILinqRepository<F_F_ParameterDoc> paramDocRepository,
                ILinqRepository<D_Fin_nsiBudget> budgetRepository,
                ILinqRepository<D_KOSGY_KOSGY> kosguRepository,
                ILinqRepository<D_Fin_RazdPodr> razdPodrRepository,
                ILinqRepository<D_Fin_VidRash> vidRashRepository)
            : base(smetaRepository)
        {
            parDocRepository = paramDocRepository;

            this.budgetRepository = budgetRepository;
            this.kosguRepository = kosguRepository;
            this.razdPodrRepository = razdPodrRepository;
            this.vidRashRepository = vidRashRepository;
        }

        public override IEnumerable<F_Fin_Smeta> GetItems(int? parentId)
        {
            return from p in GetRepository().FindAll()
                   where p.RefParametr.ID == parentId
                   select p;
        }

        public override SmetaViewModel ConvertToView(F_Fin_Smeta item)
        {
            return SmetaHelpers.ConvertToViewModel(item);
        }

        public override F_Fin_Smeta DecodeJson(JsonObject json)
        {
            try
            {
                var refParameter = JsonUtils.ParseRepositoryId(parDocRepository, json, "RefParameterID", "Не указан родительский документ");

                var refBudget = JsonUtils.ParseRepositoryId(budgetRepository, json, "RefBudgetID", "Не указан бюджет");
                var refKosgy = JsonUtils.ParseRepositoryId(kosguRepository, json, "RefKosgyID", "Не указан КОСГУ");
                var refRazdPodr = JsonUtils.ParseRepositoryId(razdPodrRepository, json, "RefRazdPodrID", "Не указан раздел/подраздел");
                var refVidRash = JsonUtils.ParseRepositoryId(vidRashRepository, json, "RefVidRashID", "Не указан вид расходов");

                var result =
                    new F_Fin_Smeta
                        {
                            SourceID = 0,
                            TaskID = 0,
                            RefBudget = refBudget,
                            RefKbkBudget = null,
                            RefKosgy = refKosgy,
                            RefParametr = refParameter,
                            RefRazdPodr = refRazdPodr,
                            RefVidRash = refVidRash,
                            Funds = JsonUtils.GetFieldOrDefault(json, "Funds", (Decimal)0),
                            FundsOneYear = JsonUtils.GetFieldOrDefault(json, "FundsOneYear", (Decimal)0),
                            FundsTwoYear = JsonUtils.GetFieldOrDefault(json, "FundsTwoYear", (Decimal)0),
                            CelStatya = JsonUtils.GetFieldOrDefault(json, "CelStatya", ""),
                            Event = JsonUtils.GetFieldOrDefault(json, "Event", "")
                        };
                var id = Convert.ToInt32(json["ID"]);
                if (id > 0)
                {
                    result.ID = id;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidDataException("SmetaService::DecodeJson: Ошибка преобразования полученных сервером данных в смету: " + e.Message, e);
            }
        }
    }
}
