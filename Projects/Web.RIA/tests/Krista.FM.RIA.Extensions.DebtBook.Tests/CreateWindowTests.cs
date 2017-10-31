using System;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class CreateWindowTests
    {
        [Test]
        public void CreateWindowTest()
        {
            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            ViewPage page = new ViewPage();
            ResourceManager resourceManager = new ResourceManager();
            page.Controls.Add(resourceManager);

            Window win = GetWin();

            page.Controls.Add(win);

            string result = win.ToScript();

        }

        private static Window GetWin()
        {
            var win = new Window { 
                ID = "{0}BookWindow".FormatWith("gp"),
                Width = 800, 
                Height = 600,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true
            };
            win.AutoLoad.Url = "/Entity/Book?objectKey=7ef0edfd-9461-4333-8420-ccb102051826";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = "Загрузка справочника...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            win.Listeners.Update.Handler =
                "{0}.getBody().Extension.entityBook.onRowSelect = function(record){{btnOk.enable(); {0}.selectedBookRecord = record;}}"
                    .FormatWith(win.ID);

            var button = new Button { 
                ID = "btnOk", 
                Text = "OK", 
                Icon = Icon.Accept,
                Disabled = true
            };
            button.Listeners.Click.Handler = @"    
if ({0}.selectedBookRecord) {{
    alert({0}.selectedBookRecord.id);
    alert({0}.getBody().Extension.entityBook.getLookupValue());
/*    var grid = Ext.getCmp('GridPanel1');
    var r = grid.selModel.getSelected();
        
    r.beginEdit();
    r.set({0}.activeRefField, {0}.selectedRecord.id);
    r.set({0}.activeLookupField, {0}.getBody().Extension.entityBook.getLookupValue());
    r.endEdit();*/
    {0}.selectedBookRecord = undefined;
    btnOk.disable();
    {0}.hide();
}}
".FormatWith(win.ID);

            win.Buttons.Add(button);
            
            return win;
        }
    }
}
