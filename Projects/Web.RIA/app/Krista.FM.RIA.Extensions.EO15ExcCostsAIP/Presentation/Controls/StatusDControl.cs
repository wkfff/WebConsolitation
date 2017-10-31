using System.Web.Mvc;
using System.Web.UI;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;

[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField.js.PeriodField.js", "text/javascript")]

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls
{
    public class StatusDControl
    {
        public const string Scope = "EO15AIP.Control.StatusD";

        public static void AddStatusDButtons(GridPanel gp, Store store, ColumnBase statusDColumn, ViewPage page, BasePrincipal user, bool useFilter)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPControlStatusD", Resources.StatusDControl);

            statusDColumn.Renderer.Handler = "return " + Scope + ".rendererFn(record, {0});".FormatWith(gp.ID);
            statusDColumn.Align = Alignment.Center;

            var toolBar = gp.Toolbar();
            var isCoord = user.IsInRole(AIPRoles.Coordinator);
            var isClient = user.IsInRole(AIPRoles.MOClient) || user.IsInRole(AIPRoles.GovClient);

            gp.Toolbar().Add(new ToolbarSeparator());
            toolBar.Add(new Button
            {
                ID = "toEdit_{0}".FormatWith(gp.ID),
                ToolTip = @"Отправить на редактирование",
                Icon = Icon.PageEdit,
                Hidden = true,
                Listeners = { Click = { Handler = Scope + @".toEdit({0}, {1}, {2});".FormatWith(gp.ID, isCoord ? "true" : "false", isClient ? "true" : "false") } }
            });

            toolBar.Add(new Button
            {
                ID = "toReview_{0}".FormatWith(gp.ID),
                ToolTip = @"Отправить на рассмотрение",
                Hidden = true,
                Icon = isCoord ? Icon.PageBack : Icon.PageForward,
                Listeners = { Click = { Handler = Scope + @".toReview({0}, {1}, {2});".FormatWith(gp.ID, isCoord ? "true" : "false", isClient ? "true" : "false") } }
            });

            toolBar.Add(new Button
            {
                ID = "toAccept_{0}".FormatWith(gp.ID),
                ToolTip = @"Утвердить",
                Hidden = true,
                Icon = Icon.Tick,
                Listeners = { Click = { Handler = Scope + @".toAccept({0}, {1}, {2});".FormatWith(gp.ID, isCoord ? "true" : "false", isClient ? "true" : "false") } }
            });

            gp.Listeners.RowClick.AddAfter(Scope + @".rowClick({0}, rowIndex, {1}, {2});"
                .FormatWith(gp.ID, isCoord ? "true" : "false", isClient ? "true" : "false"));
            
            gp.Listeners.KeyPress.AddAfter(Scope + @".rowKeySelect({0}, e, {1}, {2});"
                .FormatWith(gp.ID, isCoord ? "true" : "false", isClient ? "true" : "false"));

            gp.Add(new Hidden { ID = "UrlIconStatusD1_{0}".FormatWith(gp.ID), Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) });
            gp.Add(new Hidden { ID = "UrlIconStatusD2_{0}".FormatWith(gp.ID), Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserMagnify) });
            gp.Add(new Hidden { ID = "UrlIconStatusD3_{0}".FormatWith(gp.ID), Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) });
            
            gp.Listeners.BeforeEdit.Handler = "return " + Scope + ".checkEdit(e, {0}, {1});".FormatWith(isCoord ? "true" : "false", isClient ? "true" : "false");

            if (useFilter)
            {
                CreateStateFilterButtons(gp, store);
            }
        }

        private static void CreateStateFilterButtons(GridPanel gp, Store store)
        {
            store.BaseParams.Add(new Parameter("filter", Scope + ".getStateFilter({0})".FormatWith(gp.ID), ParameterMode.Raw));

            gp.Toolbar().Add(new ToolbarSeparator());
            gp.Toolbar().Add(new DisplayField { Text = @"Фильтры: " });
            
            gp.Toolbar().Add(new Button
            {
                ID = "statusDFilter1_{0}".FormatWith(gp.ID),
                Icon = Icon.UserEdit,
                ToolTip = @"На редактировании",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = Scope + ".toggleFilter"
            });
            gp.Toolbar().Add(new Button
            {
                ID = "statusDFilter2_{0}".FormatWith(gp.ID),
                Icon = Icon.UserMagnify,
                ToolTip = @"На рассмотрении",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = Scope + ".toggleFilter"
            });
            gp.Toolbar().Add(new Button
            {
                ID = "statusDFilter3_{0}".FormatWith(gp.ID),
                Icon = Icon.Accept,
                ToolTip = @"Утверждено",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = Scope + ".toggleFilter"
            });
        }
    }
}
