using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.SMO.FinSourcePlaning;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.SMO
{
	public class SmoFinSourcePlanningFace : SmoServerSideObject<IFinSourcePlanningFace>, IFinSourcePlanningFace
	{
		public SmoFinSourcePlanningFace(IFinSourcePlanningFace serverObject)
			: base(serverObject)
        {
        }

		#region IFinSourcePlanningFace Members

		public IIndicatorsService BKKUIndicators
		{
			get { return serverControl.BKKUIndicators; }
		}

		public IIndicatorsService DDEIndicators
		{
			get { return serverControl.DDEIndicators; }
		}

		public IGuarantIncomeService GuarantIncomeService
		{
			get { return serverControl.GuarantIncomeService; }
		}

		public IGuarantIssuedService GuarantIssuedService
		{
			get { return new SmoGuarantyService(serverControl.GuarantIssuedService); }
		}

		public IСreditIncomeService СreditIncomeService
		{
			get { return new SmoСreditIncomeService(serverControl.СreditIncomeService); }
		}

		public IСreditIssuedService СreditIssuedService
		{
			get { return serverControl.СreditIssuedService; }
		}

		public ICapitalService CapitalService
		{
			get { return new SmoCapitalService(serverControl.CapitalService); }
		}

		public int BaseYear
		{
			get { return serverControl.BaseYear; }
		}

        public int GetCreditClassifierRef(string detailObjectKey,
            string classifierKey, int sourceID, int refOKV, CreditsTypes creditType, int refTerrType, string programCode)
        {
            return serverControl.GetCreditClassifierRef(detailObjectKey,
                classifierKey, sourceID, refOKV, creditType, refTerrType, programCode);
        }

        public int GetGuaranteeClassifierRef(string detailObjectKey, string classifierKey, bool isRegress, int sourceID)
        {
            return serverControl.GetGuaranteeClassifierRef(detailObjectKey, classifierKey, isRegress, sourceID);
        }

        public int GetCapitalClassifierRef(string detailObjectKey, string classifierKey, int sourceID, int refOKV)
        {
            return serverControl.GetCapitalClassifierRef(detailObjectKey, classifierKey, sourceID, refOKV);
        }

        public Boolean CheckUIModuleVisible(string moduleKey)
        {
            return serverControl.CheckUIModuleVisible(moduleKey);
        }

        public void SetAllReferences(int sourceId, IDatabase db)
	    {
            serverControl.SetAllReferences(sourceId, db);
	    }

        public void SetAllReferences(int sourceId)
        {
            serverControl.SetAllReferences(sourceId);
        }

        public void ResreshConstsData()
        {
            serverControl.ResreshConstsData();
        }

        public void SetCurrencyRatesReferences()
        {
            serverControl.SetCurrencyRatesReferences();
        }

        public void TransfertDebtBookData(object pumpId, ref string errors)
        {
            serverControl.TransfertDebtBookData(pumpId, ref errors);
        }

		#endregion
	}
}
