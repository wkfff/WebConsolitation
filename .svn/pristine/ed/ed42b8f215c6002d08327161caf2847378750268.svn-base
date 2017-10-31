using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.FinSourcePlanning.Constants;
using Krista.FM.Server.FinSourcePlanning.Services;
using Krista.FM.Server.Users;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using CreditsTypes = Krista.FM.ServerLibrary.FinSourcePlanning.CreditsTypes;

namespace Krista.FM.Server.FinSourcePlanning
{
	public class FinSourcePlanningFace : ServerSideObject, IFinSourcePlanningFace
    {
        /// <summary>
        /// Базовый год для расчета.
        /// </summary>
        public static int baseYear = 2009;

        private static FinSourcePlanningFace instance;
        private IScheme scheme;
	    private FinSourcesConstsHelper constsHelper;
	    private FinSourcePlaningPermissionsHelper permissionsHelper;

        public static FinSourcePlanningFace Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new FinSourcePlanningFace(null);
                }
                return instance;
            }
        }

        private FinSourcePlanningFace(ServerSideObject owner)
			: base (owner)
        {
        }

        public void Initialize(IScheme scheme)
        {
            this.scheme = scheme;
            constsHelper = new FinSourcesConstsHelper(scheme);
            permissionsHelper = new FinSourcePlaningPermissionsHelper(scheme);
        }

        public bool Initialized
        {
            get { return scheme != null; }
        }

        internal IScheme Scheme
        {
            get { return scheme; }
        }

        #region IFinSourcePlanningFace Members

        public IGuarantIncomeService GuarantIncomeService
        {
            get { return Services.GuarantIncomeService.Instance; }
        }

        public IGuarantIssuedService GuarantIssuedService
        {
            get { return Services.GuarantIssuedService.Instance; }
        }

        public IСreditIncomeService СreditIncomeService
        {
            get { return Services.СreditIncomeService.Instance; }
        }

        public IСreditIssuedService СreditIssuedService
        {
            get { return Services.СreditIssuedService.Instance; }
        }

        public ICapitalService CapitalService
        {
            get { return Services.CapitalService.Instance; }
        }

        public IIndicatorsService BKKUIndicators
        {
            get { return Services.BKKUIndicators.BKKUIndicatorsService.Instance; }
        }

        public IIndicatorsService DDEIndicators
        {
            get { return Services.DDEIndicators.DDEIndicatorsService.Instance; }
        }

        /// <summary>
        /// Базовый год для расчета.
        /// </summary>
        public int BaseYear
        {
            get { return baseYear; }
        }

        public int GetCreditClassifierRef(string detailObjectKey, string classifierKey, int sourceID,
            int refOKV, CreditsTypes creditType, int refTerrType, string programCode)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return constsHelper.GetCreditClassifierRef(detailObjectKey,
                    classifierKey, sourceID, refOKV, creditType, refTerrType, programCode, db);
            }
        }

        public int GetGuaranteeClassifierRef(string detailObjectKey, string classifierKey, bool isRegress, int sourceID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return constsHelper.GetGuaranteeClassifierRef(detailObjectKey, classifierKey, isRegress, sourceID, db);
            }
        }

        public int GetCapitalClassifierRef(string detailObjectKey, string classifierKey, int sourceID, int refOKV)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return constsHelper.GetCapitalClassifierRef(detailObjectKey, classifierKey, sourceID, refOKV, db);
            }
        }

        public Boolean CheckUIModuleVisible(string moduleKey)
        {
            return permissionsHelper.GetVisibleUIModule(moduleKey);
        }

        public void SetAllReferences(int sourceId, IDatabase db)
        {
            constsHelper.SetReferences(sourceId, db);
        }

        public void SetAllReferences(int sourceId)
        {
            constsHelper.SetReferences(sourceId, null);
        }

        public void ResreshConstsData()
        {
            constsHelper.FillConstsTable();
        }

        public void SetCurrencyRatesReferences()
        {
            CurrencyRateService.SetCurrencyRatesReferences(scheme);
        }

        public void TransfertDebtBookData(object pumpId, ref string errors)
        {
            var transfertService = new DataTransfertService();
            transfertService.TransfertData(scheme, pumpId);
        }

        #endregion
    }
}
