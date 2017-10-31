using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Capital;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    public class CapitalUI : DebtorBookUI
    {
        public CapitalUI(string key)
            : base(key)
        {}

        public override void Initialize()
        {
            base.Initialize();

            // тут всякая хрень по настройке интерфейса будет
        }

        protected override DebtBookCls GetSubjectCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            return new SubjectCapital(entity);
        }

        protected override DebtBookCls GetRegionCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            return new RegionCapital(entity);
        }

        protected override DebtBookCls GetSettlementCls()
        {
            IEntity entity = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            return new SettlementCapital(entity);
        }
    }
}
