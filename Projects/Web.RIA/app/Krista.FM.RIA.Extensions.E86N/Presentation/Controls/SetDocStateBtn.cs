using System.Collections.Generic;
using System.Globalization;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public sealed class SetDocStateBtn : Control
    {
        private readonly ILinqRepository<FX_Org_SostD> statesRepository;

        public SetDocStateBtn(int docId)
        {
            statesRepository = Resolver.Get<ILinqRepository<FX_Org_SostD>>();
            DocId = docId;
        }

        public int DocId { get; set; }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var setStateBtn = new Button
            {
                ID = "setStateBtn",
                Text = @"Состояние документа",
                Icon = Icon.Reload
            };

            var menu = new Menu();

            menu.Add(GetMenuItem(FX_Org_SostD.CreatedStateID, Icon.UserAdd));

            menu.Add(GetMenuItem(FX_Org_SostD.UnderConsiderationStateID, Icon.UserCross));

            menu.Add(GetMenuItem(FX_Org_SostD.OnEditingStateID, Icon.UserEdit));

            menu.Add(GetMenuItem(FX_Org_SostD.ExportedStateID, Icon.UserGreen));

            menu.Add(GetMenuItem(FX_Org_SostD.FinishedStateID, Icon.UserHome));

            setStateBtn.Menu.Add(menu);

            return new List<Component> { setStateBtn };
        }

        private MenuItem GetMenuItem(int state, Icon icon)
        {
            var stateSt = state.ToString(CultureInfo.InvariantCulture);
            var stateName = statesRepository.FindOne(state).Name;
            
            return new MenuItem
                {
                    ID = string.Concat("state_", stateSt),
                    Text = stateName,
                    Icon = icon,
                    DirectEvents =
                        {
                            Click =
                                {
                                    Method = HttpMethod.POST,
                                    Url = UiBuilders.GetUrl<StateSysBaseController>("SetStateDoc"),
                                    CleanRequest = true,
                                    ExtraParams =
                                        {
                                            new Parameter("docId", DocId.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw),
                                            new Parameter("stateId", stateSt, ParameterMode.Raw)
                                        },
                                    Confirmation =
                                        {
                                            ConfirmRequest = true,
                                            Title = "Подтверждение смены состояния документа",
                                            Message = @"<p/>Документ будет переведен в состояние '{0}'
                                                                <p/>Вы действительно хотите продолжить?".FormatWith(stateName)
                                        }
                                }
                        }
                };
        }
    }
}
