using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    /// Представление для отображения протокола
    /// </summary>
    public class ProtocolView : View
    {
        private readonly ProtocolViewModel model = new ProtocolViewModel();

        public override List<Component> Build(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterIcon(Icon.Exclamation);
            ResourceManager.GetInstance(page).RegisterIcon(Icon.Information);
            ResourceManager.GetInstance(page).RegisterIcon(Icon.Error);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            Store store = StoreExtensions.StoreUrlCreateDefault(
                 "ProtocolStore",
                 true,
                 UiBuilders.GetUrl<DocCommonController>("ReadAction"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocCommonController>("DeleteAction"));

            store.SetBaseParams("modelType", model.GetType().AssemblyQualifiedName, ParameterMode.Value);
            store.AddFieldsByClass(model);

            store.Listeners.Load.Handler = @"if (records[0].data.ProtocolResult === 'failure') 
                                             {
                                                window.ProtocolStatusBar.setIcon('icon-exclamation');
                                                window.ProtocolStatusBar.setText('В протоколе присутствуют ошибки');
                                             };";

            page.Controls.Add(store);
            
            GridPanel gp = UiBuilders.CreateGridPanel("ProtocolGrid", store);
            gp.Header = false;
            gp.AddRefreshButton();

            gp.ColumnModel.Columns.Add(new ImageCommandColumn
            {
                ColumnID = "Icon",
                Align = Alignment.Center,
                Width = 10,
                Commands = { new ImageCommand() },
                PrepareCommand =
                    {
                        Handler = @"if (record.get('Code')==='error') 
                                    {
                                        command.iconCls = 'icon-exclamation';
                                        command.qtext = 'Ошибка';
                                    }; 
                                    
                                    if (record.get('Code')==='info') 
                                    {
                                        command.iconCls = 'icon-information';
                                        command.qtext = 'Информация';
                                    }; 
                                            
                                    if (record.get('Code')==='warning') 
                                    {
                                       command.iconCls = 'icon-error';
                                       command.qtext = 'Предупреждение';
                                    };"
                    }
            });

            gp.ColumnModel.AddColumn(() => model.Code, true).SetWidth(50).SetHidden(true);
            gp.ColumnModel.AddColumn(() => model.Name, true).SetWidth(100);
            gp.ColumnModel.AddColumn(() => model.Description, true);

            var gridFilters = new GridFilters
            {
                Local = true
            };

            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => model.Code) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => model.Name) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => model.Description) });

            gp.Plugins.Add(gridFilters);

            gp.BottomBar.Add(new StatusBar
                                 {
                                     ID = "ProtocolStatusBar",
                                     Icon = Icon.Accept,
                                     Text = @"Протокол не содержит ошибок"
                                 });

            gp.AddColumnsWrapStylesToPage(page);

            return new List<Component>
                       {
                           new Viewport
                               {
                                   Layout = LayoutType.Fit.ToString(),
                                   Items = { gp }
                               }
                       };
        }
    }
}
