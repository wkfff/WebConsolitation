using System.Collections.Generic;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Messages.Presentation.Controls;
using Krista.FM.RIA.Extensions.Messages.Services;

namespace Krista.FM.RIA.Extensions.Messages.Presentation.Views
{
    public class MessagesView : View
    {
        private readonly IMessageService messageService;

        public MessagesView(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterEmbeddedScript<MessageGridControl>("MessageGridControl.js");
            ResourceManager.GetInstance(page).RegisterEmbeddedScript<MessageGridControl>("supercombobox.js");
            ResourceManager.GetInstance(page).RegisterEmbeddedStyle<MessageGridControl>("supercombobox.css");

            ResourceManager.GetInstance(page).RegisterIcon(Icon.Email);
            ResourceManager.GetInstance(page).RegisterIcon(Icon.EmailStar);

            var layout = new BorderLayout { ID = "borderLayoutMain" };

            var messageGridControl = new MessageGridControl(User);

            var panel = new Panel
            {
                ID = "mainPanel",
                Border = false,
                Layout = LayoutType.Fit.ToString(),
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            panel.Items.Add(messageGridControl.Build(page));

            layout.Center.Items.Add(panel);

            var viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);

            return new List<Component>
                {
                    viewport,
                    messageGridControl.CreateSendMsgWindow(page, "addNewMessageWindow", "/MessagesNav/SendMessage")
                };
        }
    }
}
