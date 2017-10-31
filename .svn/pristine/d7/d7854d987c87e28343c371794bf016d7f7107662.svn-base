using System;
using System.Collections.Generic;
using System.Linq;

using bus.gov.ru;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public sealed class CommonDataService : NewRestService, ICommonDataService
    {
        public readonly IAuthService Auth;

        public CommonDataService()
        {
            Auth = Resolver.Get<IAuthService>();
        }      

        public IEnumerable<FX_Fin_YearForm> GetYearFormList()
        {
            return GetItems<FX_Fin_YearForm>();
        }

        public IQueryable<FX_FX_PartDoc> GetPartDocList(int typeInstitution)
        {
            var data = GetItems<FX_FX_PartDoc>();
            if (!Auth.IsAdmin())
            {
                data = data.Where(doc => doc.ID != 4);
            }

            switch (typeInstitution)
            {
                case FX_Org_TipYch.AutonomousID:
                    {
                        return data.Where(x =>
                            (x.ID != FX_FX_PartDoc.NoValueDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.SmetaDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503130Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503121Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503127Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503137Type));
                    }

                case FX_Org_TipYch.BudgetaryID:
                    {
                        return data.Where(x =>
                            (x.ID != FX_FX_PartDoc.NoValueDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.SmetaDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503130Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503121Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503127Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503137Type));
                    }

                case FX_Org_TipYch.GovernmentID:
                    {
                        return data.Where(x =>
                            (x.ID != FX_FX_PartDoc.NoValueDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.PfhdDocTypeID) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503730Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503721Type) &&
                            (x.ID != FX_FX_PartDoc.AnnualBalanceF0503737Type));
                    }
            }

            return data.Where(x => (x.ID != FX_FX_PartDoc.NoValueDocTypeID));
        }

        public IQueryable<FX_FX_PartDoc> GetPartDocList()
        {
            return GetPartDocList(-1);
        }

        public IQueryable<FX_FX_PartDoc> GetPartDocList(int masterId, DateTime formationDate)
        {
            var master = masterId;
            if (master == -1)
            {
                master = Auth.Profile.RefUchr.ID;
            }

            var type = GetTypeOfInstitution(GetItems<D_Org_Structure>().First(x => x.ID == master), formationDate);

            return GetPartDocList(type.ID);
        }

        public IEnumerable<D_Org_TipFil> GetTipFilList()
        {
            return GetItems<D_Org_TipFil>();
        }

        public IEnumerable<D_OKVED_OKVED> GetOkvedList()
        {
            return GetItems<D_OKVED_OKVED>();
        }

        public IEnumerable<D_OKVED_OKVED> GetOkvedList(string query, DateTime? dateBegin, DateTime? dateEnd)
        {
            var data = GetItems<D_OKVED_OKVED>().Where(x => x.Name.Contains(query) || x.Code.Contains(query)).ToList()
                                                .FindAll(x => x.IsEntryDates(dateBegin, dateEnd, "OpenDate", "CloseDate"));

            return data;
        }

        public IEnumerable<D_Org_PrOKVED> GetPrOkvedList()
        {
            return GetItems<D_Org_PrOKVED>();
        }

        public IEnumerable<D_Doc_TypeDoc> GetTypeDocList()
        {
            return GetItems<D_Doc_TypeDoc>();
        }

        public IEnumerable<D_Org_Category> GetOrgCategoryList()
        {
            return GetItems<D_Org_Category>();
        }

        public IEnumerable<FX_Fin_FinPeriod> GetFinPeriodList()
        {
            return GetItems<FX_Fin_FinPeriod>();
        }

        public IEnumerable<FX_Org_SostD> GetSostDataList()
        {
            return GetItems<FX_Org_SostD>();
        }

        public IEnumerable<FX_Org_TipYch> GetTipUchList()
        {
            return GetItems<FX_Org_TipYch>();
        }

        public IEnumerable<D_Org_GRBS> GetGRBSList()
        {
            return GetItems<D_Org_GRBS>();
        }

        public IEnumerable<D_Org_PPO> GetPPOList()
        {
            return GetItems<D_Org_PPO>();
        }

        /// <summary>
        /// Определение типа учреждения по дате
        /// </summary>
        public FX_Org_TipYch GetTypeOfInstitution(D_Org_Structure structure, DateTime formationDate)
        {
            try
            {
                var historyType = structure.TypeHistories.SingleOrDefault(
                x => x.DateStart <= formationDate && formationDate <= x.DateEnd);
                return historyType != null ? historyType.RefTypeStructure : structure.RefTipYc;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("GetTypeOfInstitution: Ошибка при определении типа учреждения {0} на дату {1}".FormatWith(structure.INN, formationDate));
            }
        }

        /// <summary>
        /// Определение типа учреждения по эгземпляру документа
        /// </summary>
        public FX_Org_TipYch GetTypeOfInstitution(F_F_ParameterDoc item)
        {
            if (item == null)
            {
                return null;
            }

            var year = item.RefYearForm.ID;
            var historyType = item.RefUchr.TypeHistories
                .Where(x => x.DateStart.Year <= year 
                        && year <= x.DateEnd.Year
                        && x.DateStart <= item.OpeningDate)
                .OrderByDescending(x => x.DateEnd)
                .FirstOrDefault();

            if (historyType != null)
            {
                return historyType.RefTypeStructure;
            }

            return item.RefUchr.RefTipYc;
        }

        /// <summary>
        /// Определение типа учреждения по идентификатору документа
        /// </summary>
        public FX_Org_TipYch GetTypeOfInstitution(int docId)
        {
            return GetTypeOfInstitution(GetItem<F_F_ParameterDoc>(docId));
        }
    }
}
