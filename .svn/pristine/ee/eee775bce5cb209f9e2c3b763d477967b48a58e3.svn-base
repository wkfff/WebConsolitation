using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailDocView : View
    {
        private readonly int objectId;
        private readonly bool canEdit;
        private readonly bool canDeleteDoc;
        private readonly D_ExcCosts_CObject curObject;

        public DetailDocView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
        {
            this.objectId = objectId;
            curObject = constrRepository.GetOne(objectId);
            canEdit = User.IsInRole(AIPRoles.Coordinator) ||
                ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
                    && extension.Client != null && extension.Client.ID == curObject.RefClients.ID);
            canDeleteDoc = !((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
                            && extension.Client != null && extension.Client.ID == curObject.RefClients.ID);
        }

        public override List<Component> Build(ViewPage page)
        {
            var fileListPanel = DetailFileListControl.Get(canEdit, canDeleteDoc, objectId);
            fileListPanel.AutoScroll = true;
            fileListPanel.Enabled = canEdit;
            fileListPanel.Height = 170;
            fileListPanel.Title = @"Электронные версии документов";
            fileListPanel.Collapsible = false;
            fileListPanel.AddRefreshButton();
            fileListPanel.AddSaveButton();
            fileListPanel.Frame = true;
            fileListPanel.AddColumnsWrapStylesToPage(page);
            page.Controls.Add(fileListPanel.GetFileListStore());
            return new List<Component> { fileListPanel, fileListPanel.GetFileUploadWindow() };
        }
    }
}
