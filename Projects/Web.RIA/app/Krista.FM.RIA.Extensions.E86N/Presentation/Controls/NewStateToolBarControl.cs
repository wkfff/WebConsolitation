using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.RIA.Extensions.E86N.Services;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class NewStateToolBarControl : Control
    {
        public const string Scope = GlobalConsts.ScopeStateToolBar;
        private readonly IStateSystemService stateSystemService;

        public NewStateToolBarControl(int docId)
        {
            DocId = docId;
            stateSystemService = Resolver.Get<IStateSystemService>();
        }

        /// <summary>
        ///  Идентификатор документа
        /// </summary>
        public int DocId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            ResourceManager resmngr = ResourceManager.GetInstance(page);
            resmngr.RegisterClientScriptBlock("StateToolBarControl.js", Resource.StateToolBarControl);

            var tb = new Toolbar { ID = "StateToolBarNew" };

            int? schemId = stateSystemService.GetSchemStateTransitionsID(stateSystemService.GetTypeDocID(DocId));

            D_State_SchemTransitions schemTransitions = schemId.HasValue
                                                            ? stateSystemService.GetSchemStateTransitions(schemId.Value)
                                                            : null;

            if (schemTransitions != null)
            {
                // инициализация при построении интерфейса
                resmngr.DirectEvents.DocumentReady.Url = schemTransitions.InitAction.IsNullOrEmpty()
                                                             ? "/StateSysBase/InitialView"
                                                             : schemTransitions.InitAction;
                resmngr.DirectEvents.DocumentReady.CleanRequest = true;
                resmngr.DirectEvents.DocumentReady.ExtraParams.Add(new Parameter("docId", DocId.ToString(CultureInfo.InvariantCulture)));

                var btnGroup = new ButtonGroup { ID = "StatesButtonGroup", Layout = "toolbar" };

                IQueryable<D_State_Transitions> transitions = stateSystemService.GetTransitions(schemTransitions.ID);

                foreach (D_State_Transitions transition in transitions)
                {
                    switch (transition.TransitionClass)
                    {
                        case "Base":
                            btnGroup.Add(
                                new Button
                                    {
                                        ID = GlobalConsts.BtnTransition + transition.ID,
                                        Disabled = true,
                                        Icon = (Icon)Enum.Parse(typeof(Icon), transition.Ico),
                                        ToolTip = transition.Note,
                                        Text = transition.Name,
                                        DirectEvents =
                                            {
                                                Click =
                                                    {
                                                        Url = transition.Action,
                                                        CleanRequest = true,
                                                        ExtraParams =
                                                            {
                                                                new Parameter(
                                                                    "docId",
                                                                    DocId.ToString(CultureInfo.InvariantCulture),
                                                                    ParameterMode.Value),

                                                                new Parameter(
                                                                    "transitionID",
                                                                    transition.ID.ToString(CultureInfo.InvariantCulture),
                                                                    ParameterMode.Value)
                                                            }
                                                    }
                                            }
                                    });
                            break;

                        case "Export":
                            var btnExport = new UpLoadGmuBtnControl(transition.Action + "?transitionID={0}".FormatWith(transition.ID), DocId)
                                                {
                                                    Id = GlobalConsts.BtnTransition + transition.ID,
                                                    FailureHandler = @"#{wndUploadGMU}.hide();" + UpLoadGmuBtnControl.FailHndler,
                                                    SuccessHandler = "#{wndUploadGMU}.hide();" + UpLoadGmuBtnControl.SucHndler
                                                                     + "{0}.closeDoc({1})".FormatWith(Scope, DocId)
                                                }.Build(page)[0] as Button;

                            if (btnExport != null)
                            {
                                btnExport.Disabled = true;
                                btnExport.Text = transition.Name;
                                btnExport.ToolTip = transition.Note;
                                btnExport.Icon = (Icon)Enum.Parse(typeof(Icon), transition.Ico);

                                btnGroup.Add(btnExport);
                            }

                            break;

                        case "WithDialog":
                            btnGroup.Add(
                                new Button
                                    {
                                        ID = GlobalConsts.BtnTransition + transition.ID,
                                        Disabled = true,
                                        Icon = (Icon)Enum.Parse(typeof(Icon), transition.Ico),
                                        ToolTip = transition.Note,
                                        Text = transition.Name,
                                        DirectEvents =
                                            {
                                                Click =
                                                    {
                                                        Url = transition.Action,
                                                        CleanRequest = true,
                                                        ExtraParams =
                                                            {
                                                                new Parameter("docId", DocId.ToString(CultureInfo.InvariantCulture), ParameterMode.Value),
                                                                new Parameter("transitionID", transition.ID.ToString(CultureInfo.InvariantCulture), ParameterMode.Value)
                                                            },
                                                        Before = Scope + ".showNoteWin({0})".FormatWith(DocId)
                                                    }
                                            }
                                    });
                            break;
                    }
                }

                tb.Add(btnGroup);
            }

            return new List<Component> { tb };
        }

        public Toolbar BuildComponent(ViewPage page)
        {
            return Build(page)[0] as Toolbar;
        }
    }
}