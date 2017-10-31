using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Credits;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    class OrganizationCreditUI : DebtorBookUI
    {
        public OrganizationCreditUI(string key)
            : base(key)
        {
        }

        public override void  Initialize()
        {
 	         base.Initialize();
        }

        protected override DebtBookCls GetSubjectCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            return new SubjectOrganizationCredit(entity);
        }

        protected override DebtBookCls GetRegionCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            return new RegionOrganizationCredit(entity);
        }

        protected override DebtBookCls GetSettlementCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            return new SettlementOrganizationCredit(entity);
        }
    }
}
