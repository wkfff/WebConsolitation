using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;

namespace Krista.FM.RIA.Core.Gui
{
    /// <summary>
    /// Компонент для отображения многострочных сообщений об ошибках.
    /// </summary>
    public class ErrorWindow : Control
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Window window = new Window
            {
                Visible = true,
                Hidden = false,
                Modal = true,
                AutoWidth = true,
                AutoHeight = true,
                Title = Title,
                Maximizable = true,
                Resizable = true,
                Items =
                {
                    new HtmlEditor
                    {
                        ReadOnly = true,
                        Value = Text,
                        SubmitValue = false,
                        EnableAlignments = false,
                        EnableColors = false,
                        EnableFont = false,
                        EnableFontSize = false,
                        EnableFormat = false,
                        EnableLinks = false,
                        EnableLists = false,
                        EnableSourceEdit = false
                    }
                }
            };

            return new List<Component> { window };
        }
    }
}
