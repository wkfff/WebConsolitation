using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Panel = Ext.Net.Panel;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class ProjectsView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            var tabPanel = new TabPanel
                               {
                                   ID = "ProjectsTabPanel",
                                   IDMode = IDMode.Explicit,
                                   Border = false,
                                   ActiveTabIndex = 0,
                                   EnableTabScroll = true,
                                   MonitorResize = true,
                                   LayoutOnTabChange = true,
                                   Closable = true
                               };

            tabPanel.Items.Add(new Panel
                                    {
                                        ID = "RunningProjectsTab",
                                        Title = "Раздел 1",
                                        AutoLoad =
                                            {
                                                Url = "/View/InvProjProjectsList", 
                                                Mode = LoadMode.IFrame,
                                                TriggerEvent = "show",
                                                Params =
                                                    {
                                                        new Parameter("refPart", ((int)InvProjPart.Part1).ToString(), ParameterMode.Value),
                                                        new Parameter("projStatus", ((int)InvProjStatus.Undefined).ToString(), ParameterMode.Value)
                                                    },
                                                ShowMask = true, 
                                                MaskMsg = "Загрузка..."
                                            }
                                    });

            tabPanel.Items.Add(new Panel
                                    {
                                        ID = "ProposedProjectsTab",
                                        Title = "Раздел 2",
                                        AutoLoad =
                                            {
                                                Url = "/View/InvProjProjectsList", 
                                                Mode = LoadMode.IFrame, 
                                                TriggerEvent = "show",
                                                Params =
                                                    {
                                                        new Parameter("refPart", ((int)InvProjPart.Part2).ToString(), ParameterMode.Value),
                                                        new Parameter("projStatus", ((int)InvProjStatus.Undefined).ToString(), ParameterMode.Value)
                                                    },
                                                ShowMask = true, 
                                                MaskMsg = "Загрузка..."
                                            }
                                    });

            tabPanel.Items.Add(new Panel
                                    {
                                        ID = "ExcludedProjectsTab",
                                        Title = "Исключенные проекты",
                                        AutoLoad =
                                            {
                                                Url = "/View/InvProjProjectsList", 
                                                Mode = LoadMode.IFrame, 
                                                TriggerEvent = "show",
                                                Params =
                                                    {
                                                        new Parameter("refPart", ((int)InvProjPart.Undefined).ToString(), ParameterMode.Value),
                                                        new Parameter("projStatus", ((int)InvProjStatus.Excluded).ToString(), ParameterMode.Value)
                                                    },
                                                ShowMask = true, 
                                                MaskMsg = "Загрузка..."
                                            }
                                    });

            return new List<Component> { new Viewport { Items = { new FitLayout { Items = { tabPanel } } } } };
        }
    }
}
