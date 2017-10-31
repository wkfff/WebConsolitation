using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface ICommonDataService : INewRestService
    {
        IEnumerable<FX_Fin_YearForm> GetYearFormList();

        IQueryable<FX_FX_PartDoc> GetPartDocList();

        IQueryable<FX_FX_PartDoc> GetPartDocList(int masterId, DateTime formationDate);

        IQueryable<FX_FX_PartDoc> GetPartDocList(int typeInstitution);

        IEnumerable<D_Org_TipFil> GetTipFilList();

        IEnumerable<D_OKVED_OKVED> GetOkvedList();

        IEnumerable<D_OKVED_OKVED> GetOkvedList(string query, DateTime? dateBegin, DateTime? dateEnd);

        IEnumerable<D_Org_PrOKVED> GetPrOkvedList();

        IEnumerable<D_Doc_TypeDoc> GetTypeDocList();

        IEnumerable<D_Org_Category> GetOrgCategoryList();

        IEnumerable<FX_Fin_FinPeriod> GetFinPeriodList();

        IEnumerable<FX_Org_SostD> GetSostDataList();

        IEnumerable<FX_Org_TipYch> GetTipUchList();

        IEnumerable<D_Org_GRBS> GetGRBSList();

        IEnumerable<D_Org_PPO> GetPPOList();

        /// <summary>
        /// Определение типа учреждения по дате
        /// </summary>
        FX_Org_TipYch GetTypeOfInstitution(D_Org_Structure structure, DateTime formationDate);

        /// <summary>
        /// Определение типа учреждения по эгземпляру документа
        /// </summary>
        FX_Org_TipYch GetTypeOfInstitution(F_F_ParameterDoc item);

        /// <summary>
        /// Определение типа учреждения по идентификатору документа
        /// </summary>
        FX_Org_TipYch GetTypeOfInstitution(int docId);
    }
}
