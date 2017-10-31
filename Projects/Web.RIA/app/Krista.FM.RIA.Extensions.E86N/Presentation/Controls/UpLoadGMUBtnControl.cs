using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Progress;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class UpLoadGmuBtnControl : Control
    {
        public const string SucHndler = @"Ext.Msg.show({
                                             title:'Выполнено',
                                             msg: result.extraParams.msg + ';<p/>' + result.extraParams.responseText,
                                             buttons: Ext.Msg.OK,
                                             icon: Ext.MessageBox.INFO,
                                             maxWidth: 1000
                                             });";

        public const string FailHndler = @"Ext.Msg.show({
                                             title:'Ошибка',
                                             msg: result.extraParams.msg + ';<p/>' + result.extraParams.responseText,
                                             buttons: Ext.Msg.OK,
                                             icon: Ext.MessageBox.ERROR,
                                             maxWidth: 1000
                                            });";

        private const string ID = "btnUpLoadGMU";

        public UpLoadGmuBtnControl()
        {
            Id = ID;
            SuccessHandler = SucHndler;
            FailureHandler = FailHndler;
        }

        public UpLoadGmuBtnControl(string uploadController, int? docid)
        {
            Id = ID;
            UploadController = uploadController;
            DocId = docid;
        }

        public string UploadController { private get; set; }

        public string SuccessHandler { private get; set; }

        public string FailureHandler { private get; set; }

        public int? DocId { get; set; }

        public static Window CreateUploadGmuWindow(string successHandler, string failureHandler, string uploadController, string getDocIdsHandler)
        {
            var wnd = new Window
            {
                ID = "wndUploadGMU",
                Title = @"Параметры выгрузки",
                Resizable = false,
                Icon = Icon.DiskUpload,
                Width = 400,
                AutoHeight = true,
                Hidden = true,
                Modal = true,
            };

            var form = new FormPanel
            {
                ID = "formUploadGMUParams",
                Border = false,
                MonitorValid = true,
                Padding = 6,
                DefaultAnchor = "95%",
                LabelWidth = 50,
                BodyStyle = "background:transparent;"
            };

            form.Listeners.ClientValidation.Handler = "#{btnUploadGMU}.setDisabled(!valid);";

            form.Items.Add(
                new TextField
                {
                    ID = "txtfldName",
                    FieldLabel = @"Логин",
                    AllowBlank = false
                });

            form.Items.Add(
                new TextField
                {
                    ID = "txtfldPassword",
                    FieldLabel = @"Пароль",
                    InputType = Ext.Net.InputType.Password,
                    AllowBlank = false
                });

            wnd.Items.Add(form);

            var btnUploadGmu = new Button
            {
                ID = "btnUploadGMU",
                Icon = Icon.DiskUpload,
                Text = @"Выгрузить"
            };

            btnUploadGmu.DirectEvents.Click.Url = uploadController;
            btnUploadGmu.DirectEvents.Click.CleanRequest = true;
            btnUploadGmu.DirectEvents.Click.ExtraParams.Add(
                new Parameter("Name", "txtfldName.getValue()", ParameterMode.Raw));
            btnUploadGmu.DirectEvents.Click.ExtraParams.Add(
                new Parameter("Pass", "txtfldPassword.getValue()", ParameterMode.Raw));
            btnUploadGmu.DirectEvents.Click.ExtraParams.Add(new Parameter("docs", getDocIdsHandler, ParameterMode.Raw));

            btnUploadGmu.DirectEvents.Click.Failure = failureHandler;

            btnUploadGmu.DirectEvents.Click.Success = successHandler;

            btnUploadGmu.DirectEvents.Click.ExtraParams.Add(new ProgressConfig("Экспорт утвержденных документов..."));
            btnUploadGmu.DirectEvents.Click.Timeout = 3600000;

            wnd.Buttons.Add(btnUploadGmu);
            return wnd;
        }

        public override List<Component> Build(ViewPage page)
        {
            var wnd = CreateUploadGmuWindow(SuccessHandler, FailureHandler, UploadController, DocId.ToString());

            page.Controls.Add(wnd);
            
            var btn = new Button
                            {
                                ID = Id,
                                Icon = Icon.DiskUpload,
                                ToolTip = @"Выгрузка XML на сайт ГМУ"
                            };

            btn.Listeners.Click.Handler = "#{wndUploadGMU}.show()";

            return new List<Component> { btn };
        }
    }
}
