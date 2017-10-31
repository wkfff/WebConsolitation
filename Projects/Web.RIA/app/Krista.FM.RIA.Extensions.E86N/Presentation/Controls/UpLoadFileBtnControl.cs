using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Progress;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class UpLoadFileBtnControl : Control
    {
        public UpLoadFileBtnControl()
        {
            Params = new Dictionary<string, string>();
            Id = "btnUpLoad";
            Icon = Icon.DiskUpload;
            Name = "Импорт";
            Upload = true;
            SuccessHandler = @"
              Ext.MessageBox.hide();
              Ext.net.Notification.show({iconCls : 'icon-information',
                                             html : result.extraParams.msg,
                                          title : 'Уведомление', hideDelay : 2500});
              if (result.extraParams != undefined && result.extraParams.warning != undefined) {
                  Ext.Msg.show({title : 'Протокол',
                                value : result.extraParams.warning,
                                multiline : true,
                                minWidth : 200,
                                defaultTextHeight : 600, 
                                height : 600, 
                                width : 600,
                                modal : true,
                                icon : Ext.Msg.PROMPT,
                                buttons : Ext.Msg.OK });
              }";
        }

        public string UploadController { get; set; }

        public string Name { get; set; }

        public bool Upload { get; set; }

        public string SuccessHandler { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public Icon Icon { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var uploadFileForm = new FormPanel
            {
                ID = Id,
                FormID = Id + "ID",
                Frame = false,
                MonitorValid = true,
                BodyStyle = "background-color:transparent;",
                Border = false,
                FileUpload = Upload,
                Layout = LayoutType.Fit.ToString()
            };

            Component importFile;

            if (Upload)
            {
                importFile = new FileUploadField
                {
                    ID = Id + "FileUploadField",
                    ButtonOnly = true,
                    ButtonText = Name,
                    Icon = Icon
                };
            }
            else
            {
                importFile = new Button
                {
                    ID = Id + "FileDownloadBtn",
                    Icon = Icon,
                    ToolTip = Name,
                    Text = Name
                };
            }

            GetComponentDirectEvent(importFile, Upload).Timeout = 6000000;
            GetComponentDirectEvent(importFile, Upload).Method = HttpMethod.POST;
            GetComponentDirectEvent(importFile, Upload).CleanRequest = true;
            GetComponentDirectEvent(importFile, Upload).Url = UploadController;
            GetComponentDirectEvent(importFile, Upload).IsUpload = true;
            GetComponentDirectEvent(importFile, Upload).FormID = uploadFileForm.FormID;
            if (Upload)
            {
                GetComponentDirectEvent(importFile, Upload).ExtraParams.Add(new Parameter("fileName", importFile.ID + ".getValue()", ParameterMode.Raw));
                GetComponentDirectEvent(importFile, Upload).ExtraParams.Add(new ProgressConfig("Загрузка файла на сервер"));
                GetComponentDirectEvent(importFile, Upload).Before = "Ext.Msg.wait('Файл загружается...', 'Импорт');";
            }

            foreach (var param in Params)
            {
                GetComponentDirectEvent(importFile, Upload).ExtraParams.Add(new Parameter(param.Key, param.Value, ParameterMode.Raw));
            }

            GetComponentDirectEvent(importFile, Upload).Failure = @"
                  
                if (result.extraParams != undefined && result.extraParams.responseText != undefined){
                    Ext.Msg.show({title:'Ошибка',
                                  msg:result.extraParams.responseText,
                                  minWidth:200,
                                  modal:true,
                                  icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
                }else{
                    Ext.Msg.show({title:'Ошибка',
                                  msg:result.responseText,
                                  minWidth:200,
                                  modal:true,
                                  icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });}";

            GetComponentDirectEvent(importFile, Upload).Success = SuccessHandler;

            uploadFileForm.Items.Add(importFile);

            return new List<Component> { uploadFileForm };
        }

        private static ComponentDirectEvent GetComponentDirectEvent(Component cmp, bool upload)
        {
            if (upload)
            {
                return ((FileUploadField)cmp).DirectEvents.FileSelected;
            }

            return ((Button)cmp).DirectEvents.Click;
        }
    }
}
