using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controls
{
    public class BaseWindowControl : Control
    {
        public BaseWindowControl()
        {
            FormPanelComponents = new List<Component>();
        }

        public string GridName { get; set; }

        public string WindowName { get; set; }

        public string FormName { get; set; }

        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<Component> FormPanelComponents { get; set; }

        public int FormPanelLabelWidth { get; set; }

        public string ControllerName { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var baseWindow = new Window
            {
                ID = WindowName,
                Title = Title,
                Width = Width,
                Height = Height,
                Hidden = true,
                Icon = Icon.ApplicationFormEdit,
                Modal = true
            };

            baseWindow.Buttons.Add(CreateButtonSave());
            baseWindow.Buttons.Add(CreateButtonCancel());
            baseWindow.Items.Add(CreateFormPanel());
            return new List<Component> { baseWindow };
        }

        private FormPanel CreateFormPanel()
        {
            var formPanel1 = new FormPanel
            {
                ID = FormName,
                BodyCssClass = "x-window-mc",
                Border = false,
                CssClass = "x-window-mc",
                Padding = 10,
                Layout = "form",
                LabelWidth = FormPanelLabelWidth,
                Url = String.Format("/{0}/Create", ControllerName)
            };
            foreach (var formComponent in FormPanelComponents)
            {
                formPanel1.Items.Add(formComponent);
            }

            return formPanel1;
        }

        private Button CreateButtonSave()
        {
            var windowButtonSave = new Button
            {
                ID = "wndBtnSave",
                Text = "Сохранить",
                Icon = Icon.PageSave,
                Listeners =
                    {
                        Click =
                            {
                                Handler =
                                    "saveChanges({0}, {1}, {2}); ".FormatWith(
                                        GridName,
                                        WindowName,
                                        FormName)
                            }
                    }
            };

            return windowButtonSave;
        }

        private Button CreateButtonCancel()
        {
            var windowButtonCancel = new Button
            {
                ID = "wndBtnCancel",
                Text = "Отменить",
                Icon = Icon.Cancel,
                Listeners =
                {
                    Click =
                    {
                        Handler = "closeWindowCard({0}); ".FormatWith(WindowName)
                    }
                }
            };

            return windowButtonCancel;
        }
    }
}
