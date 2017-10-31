using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain.Services.FinSourceDebtorBook;

using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders.Note
{
    public class DebtBookNoteView : View
    {
        private IDebtBookExtension debtBookExtension;

        public DebtBookNoteView(IDebtBookExtension debtBookExtension)
        {
            this.debtBookExtension = debtBookExtension;
        }

        public override List<Component> Build(ViewPage page)
        {
            switch (debtBookExtension.UserRegionType)
            {
                case UserRegionType.Region:
                case UserRegionType.Town:
                    ////IUnityContainer container = Resolver.Get<IUnityContainer>();
                    ////var view = container.Resolve(typeof(DebtBookNoteFormView)) as View;
                    return (new DebtBookNoteFormView()).Build(page);
                case UserRegionType.Subject:
                    ////IUnityContainer container = Resolver.Get<IUnityContainer>();
                    ////var view = container.Resolve(typeof(DebtBookNoteFormView)) as View;
                    return (new DebtBookNoteGridView()).Build(page);
                default:
                    return new List<Component> { new DisplayField { Text = "Некорректный тип региона" } };
            }
        }
    }
}
