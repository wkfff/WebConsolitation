using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class ReportNavigation : Navigation
    {
        private readonly IAuthService auth;

        public ReportNavigation()
        {
            auth = Resolver.Get<IAuthService>();
            Id = "acE86NExtensionReportNavigation";
            Title = "Отчеты";
            Icon = Icon.Book;
            Group = "Отчеты";
            DashboardIcon = "/Krista.FM.RIA.Extensions.E86N/Presentation/Content/Images/task-72.png/extention.axd";
            DefaultItemId = string.Empty;
        }

        public override List<Component> Build(ViewPage page)
        {
            var panel = new MenuPanel
                            {
                                ID = "acE86NExtensionReportNavigation",
                                Title = @"Отчеты",
                                Icon = Icon.ReportStart,
                                Border = false
                            };

            var reportControl = new ReportControl(true).Build(page);
            panel.Menu.Items.Add(reportControl[0]);

            var analReportControl = new AnalReportControl(true).Build(page);
            panel.Menu.Items.Add(analReportControl[0]);

            var newAnalReportControl = new NewAnalReportControl(true).Build(page);
            panel.Menu.Items.Add(newAnalReportControl[0]);

            // отчет доступен только ФО и админам
            if (auth.IsPpoUser() || auth.IsKristaRu())
            {
                panel.Menu.Items.Add(new MonitoringPlacementInfoReportControl().BuildComponent(page));
            }
            
            return new List<Component> { panel };
        }
    }
}