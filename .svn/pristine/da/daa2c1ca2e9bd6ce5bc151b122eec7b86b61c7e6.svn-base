using System.Collections.Generic;
using System.Globalization;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    using System.Text;

    public sealed class VersioningControl : Control
    {
        private readonly string readOnlyDocHandler;

        public VersioningControl(int recId, string copyController)
        {
            Scope = "E86n.Control.StateToolBar";
            RecId = recId;
            CopyController = copyController;
        }

        /// <summary>
        /// Панель версионного управления документом
        /// </summary>
        /// <param name="recId"> идентификатор документа  </param>
        /// <param name="copyController"> наименование контроллера где находится метод копирования документа </param>
        /// <param name="readOnlyDocHandler"> JS-хэндлер закрытия документа </param>
        public VersioningControl(int recId, string copyController, string readOnlyDocHandler)
        {
            Scope = "E86n.Control.StateToolBar";
            RecId = recId;
            CopyController = copyController;
            this.readOnlyDocHandler = readOnlyDocHandler;
        }

        public string CopyController { get; set; }

        public int RecId { get; set; }

        public string Scope { get; set; }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var auth = Resolver.Get<IAuthService>();

            var btnGroup = new ButtonGroup
                               {
                                   Layout = "toolbar",
                               };

            var script = new StringBuilder();

            // инициализация интерфейса по статусу(открыт/закрыт) документа
            script.AppendLine(Scope + ".checkDocumentClosure({0}, {1});".FormatWith(RecId.ToString(CultureInfo.InvariantCulture), auth.IsAdmin().ToString().ToLower()));

            // установка хэндлера закрытия документа, чтобы система состояний(версий) знала как закрыть документ
            if (this.readOnlyDocHandler.IsNotNullOrEmpty())
            {
                script.AppendLine(Scope + ".ReadOnlyDocHandler = {0};".FormatWith(this.readOnlyDocHandler));
            }

            btnGroup.Listeners.BeforeRender.Handler = script.ToString();

            var btnStateClose = new Button
            {
                ID = "btnStateClose",
                EnableToggle = true,
                ToggleGroup = @"Group",
                Disabled = true,
                Icon = Icon.Lock,
                ToolTip = @"Закрыть",
                DirectEvents =
                    {
                        Click =
                        {
                            Url = "/Documents/CloseDocument",
                            CleanRequest = true, 
                            ExtraParams = { new Parameter("recId", RecId.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw) },
                            Success = "btnStateClose.setDisabled(true); btnStateOpen.setDisabled(false); window.location.reload();",
                            Before = @"if (!confirm('Вы подтверждаете закрытие?'))
                                            { 
                                                window.btnStateOpen.toggle(true);
                                                window.btnStateClose.toggle(false);
                                                return false;
                                            }"
                        },
                    }
            };

            var btnStateOpen = new Button
            {
                ID = "btnStateOpen",
                EnableToggle = true,
                ToggleGroup = @"Group",
                Disabled = true,
                Icon = Icon.LockOpen,
                ToolTip = @"Открыть",
                DirectEvents =
                {
                    Click =
                    {
                        Url = "/Documents/OpenDocument",
                        CleanRequest = true,
                        ExtraParams = { new Parameter("recId", RecId.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw) },
                        Success = "btnStateOpen.setDisabled(true); btnStateClose.setDisabled(false); window.location.reload();",
                        Before = @"if (!confirm('Выполнение этой операции может привести к появлению нескольких открытых документов. Вы подтверждаете открытие?'))
                                        { 
                                            window.btnStateOpen.toggle(false);
                                            window.btnStateClose.toggle(true);     
                                            return false;
                                        }"
                    },
                }
            };

            btnGroup.Add(btnStateClose);
            btnGroup.Add(btnStateOpen);

            var btnCopy = new Button
            {
                ID = "btnCopy",
                Disabled = true,
                Icon = Icon.ApplicationFormEdit,
                ToolTip = @"Скопировать данные из предыдущего документа",
                Text = @"Скопировать данные",
                DirectEvents =
                {
                    BeforeRender =
                    {
                        Url = "/{0}/CheckIfCanDocumentCopy".FormatWith(CopyController),
                        CleanRequest = true,
                        ExtraParams = { new Parameter("recId", RecId.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw) },
                        Success = "if ({0}) {{btnCopy.setDisabled(true)}} else {{btnCopy.setDisabled(false)}}".FormatWith(auth.IsPpoUser().ToString().ToLower()),
                        Failure = "btnCopy.setDisabled(true)",
                    },

                    Click =
                    {
                        Url = "/{0}/CopyContent".FormatWith(CopyController),
                        CleanRequest = true,
                        ExtraParams = { new Parameter("recId", RecId.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw) },
                        Success = "window.location.reload();",
                        Before = @"if (!confirm('Все введенные данные изменятся на данные из предыдущей версии документа. Вы подтверждаете копирование?')) return false;"
                    },
                },
            };

            btnGroup.Add(btnCopy);

            return new List<Component> { btnGroup };
        }
    }
}