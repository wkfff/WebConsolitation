using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class BebtBookStructureServiceGridView : BebtBookGridView
    {
        public BebtBookStructureServiceGridView(IDebtBookExtension extension, IVariantProtocolService protocolService) 
            : base(extension, protocolService)
        {
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var components = base.Build(page);

            GridPanel gridPanel = components.OfType<GridPanel>().First();

            // Удаляем с тулбара лишние кнопки
            var button = gridPanel.TopBar[0].Items.OfType<Button>().First(
                x => x.ID == "{0}AddNewRowButton".FormatWith(gridPanel.ID));
            button.Hidden = true;

            button = gridPanel.TopBar[0].Items.OfType<Button>().First(
                x => x.ID == "{0}DeleteRowButton".FormatWith(gridPanel.ID));
            button.Hidden = true;

            return components;
        }
    }
}
