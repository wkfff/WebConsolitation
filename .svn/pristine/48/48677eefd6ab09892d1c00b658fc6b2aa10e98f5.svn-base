using System;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core.MemberDigests
{
    /// <summary>
    /// Класс для управления сборниками атрибутов элементов измерений
    /// </summary>
    public class MemberDigestHelper
    {
        /// <summary>
        /// Одиночка в контексте отдельной сессии
        /// </summary>
        public static MemberDigestHelper Instance
        {
            get
            {
                if (HttpContext.Current.Session["MemberDigestHelper"] == null)
                {
                    HttpContext.Current.Session["MemberDigestHelper"] = new MemberDigestHelper();
                }
                // CRHelper.SaveToErrorLog(String.Format("{0} - {1}", HttpContext.Current.Session.SessionID, HttpContext.Current.Session["MemberDigestHelper"].GetHashCode()));
                return (MemberDigestHelper)HttpContext.Current.Session["MemberDigestHelper"];
            }
        }

        #region Список местных бюджетов

        private MemberAttributesDigest localBudgetDigest;

        /// <summary>
        /// Список местных бюджетов
        /// </summary>
        public MemberAttributesDigest LocalBudgetDigest
        {
            get
            {
                if (localBudgetDigest == null)
                {
                    FillLocalBudgetDigest();
                }
                return localBudgetDigest;
            }
        }

        private void FillLocalBudgetDigest()
        {
            CustomParam.CustomParamFactory("regions_level").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("regions_dimension").Value = RegionSettingsHelper.Instance.RegionDimension;
            CustomParam.CustomParamFactory("own_subject_budget_name").Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            bool useAuthorizedList = Convert.ToBoolean(RegionSettingsHelper.Instance.AuthorizedLocalBudgetList);

            string queryName = "LocalBudgetDigest";
            if (useAuthorizedList)
            {
                string login = HttpContext.Current.Session[CustomReportConst.currentUserKeyName].ToString();
                string moName = CustomParams.GetMONameByLogin(login);
                if (moName != String.Empty)
                {
                    CustomParam.CustomParamFactory("authorizedMO").Value = moName;
                    queryName = "AuthorizedLocalBudgetDigest";
                }
            }

            localBudgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, queryName, CRHelper.BasePath);
        }

        #endregion

        #region Список местных бюджетов с климатическими зонами

        private MemberAttributesDigest localBudgetWithClimaticZonesDigest;

        /// <summary>
        /// Список местных бюджетов с климатическими зонами
        /// </summary>
        public MemberAttributesDigest LocalBudgetWithClimaticZonesDigest
        {
            get
            {
                if (localBudgetWithClimaticZonesDigest == null)
                {
                    FillLocalBudgetWithClimaticZonesDigest();
                }
                return localBudgetWithClimaticZonesDigest;
            }
        }

        private void FillLocalBudgetWithClimaticZonesDigest()
        {
            CustomParam.CustomParamFactory("regions_level").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("regions_dimension").Value = RegionSettingsHelper.Instance.RegionDimension;
            CustomParam.CustomParamFactory("own_subject_budget_name").Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            bool useAuthorizedList = Convert.ToBoolean(RegionSettingsHelper.Instance.AuthorizedLocalBudgetList);

            string queryName = "LocalBudgetWithClimaticZonesDigest";
            if (useAuthorizedList)
            {
                string login = HttpContext.Current.Session[CustomReportConst.currentUserKeyName].ToString();
                string moName = CustomParams.GetMONameByLogin(login);
                if (moName != String.Empty)
                {
                    CustomParam.CustomParamFactory("authorizedMO").Value = moName;
                    queryName = "AuthorizedLocalBudgetDigest";
                }
            }

            localBudgetWithClimaticZonesDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, queryName, CRHelper.BasePath);
        }

        #endregion
    }
}
