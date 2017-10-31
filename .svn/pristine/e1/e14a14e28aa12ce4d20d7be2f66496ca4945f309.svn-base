using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Guarantee;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    class GuaranteeUI : DebtorBookUI
    {
        public GuaranteeUI(string key)
            : base(key)
        {}

        public override void  Initialize()
        {
 	         base.Initialize();
        }

        protected override DebtBookCls GetSubjectCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            return new SubjectGuarantee(entity);
        }

        protected override DebtBookCls GetRegionCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            return new RegionGuarantee(entity);
        }

        protected override DebtBookCls GetSettlementCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            return new SettlementGuarantee(entity);
        }
    }
}
