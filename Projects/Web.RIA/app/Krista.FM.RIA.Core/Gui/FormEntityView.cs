using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class FormEntityView : Control
    {
        private const string EntityFormId = "entityForm";

        public FormEntityView()
        {
            Params = new Dictionary<string, string>();
            StoreListeners = new Dictionary<string, string>();
        }

        public string Title { get; set; }

        public IEntity Entity { get; set; }
        
        public IPresentation Presentation { get; set; }
        
        public Dictionary<string, string> Params { get; private set; }
        
        public Dictionary<string, string> StoreListeners { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store ds = BuildStore();
            page.Controls.Add(ds);

            Panel mainPanel = new Panel 
            { 
                ID = "mainPanel", 
                Border = false,
                AutoWidth = false,
                AnchorHorizontal = "93%",
                AutoScroll = true
            };
            mainPanel.Items.Add(CreateEntityForm());
            mainPanel.TopBar.Add(CreateToolbar());

            Ext.Net.ResourceManager.GetInstance(page)
                .RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");

            Viewport viewport = new Viewport 
            { 
                ID = "viewportMain",
                Layout = "fit"
            };
            viewport.Items.Add(mainPanel);

            return new List<Component> { viewport };
        }

        private static Toolbar CreateToolbar()
        {
            Button btnSave = new Button
            {
                Text = "Сохранить",
                Icon = Icon.Disk
            };
            btnSave.Listeners.Click.Handler = @"alert('Save');";

            Toolbar toolbar = new Toolbar();
            toolbar.Items.Add(btnSave);

            return toolbar;
        }

        private FormPanel CreateEntityForm()
        {
            FormPanel entityForm = new FormPanel
                                       {
                                           ID = EntityFormId,
                                           Border = false,
                                           Url = "/",
                                           Layout = "form",
                                           LabelWidth = 230,
                                           Padding = 5,
                                           TrackResetOnLoad = true
                                       };

            foreach (IDataAttribute attribute in Entity.Attributes.Values)
            {
                if (attribute.Class == DataAttributeClassTypes.Typed)
                {
                    entityForm.Items.AddFormField(attribute, null);
                }
                else
                {
                    // Добавляем скрытое поле в форму
                    entityForm.Items.AddHiddenFormField(attribute);
                }
            }

            return entityForm;
        }

        private Store BuildStore()
        {
            Store ds = new Store 
            { 
                ID = "{0}Store".FormatWith(Id), 
                AutoLoad = true, 
                RemoteSort = true 
            };

            ds.Proxy.Add(new HttpProxy
            {
                Url = "/Entity/DataPaging?objectKey={0}"
                    .FormatWith(Entity.ObjectKey),
                Method = HttpMethod.POST
            });

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/Entity/Save?objectKey={0}"
                    .FormatWith(Entity.ObjectKey),
                Method = HttpMethod.POST
            });

            // Настраиваем Reader
            JsonReader reader = new JsonReader { IDProperty = "ID", TotalProperty = "total", Root = "data" };
            foreach (IDataAttribute attribute in Entity.Attributes.Values)
            {
                reader.Fields.AddRecordField(attribute);
            }

            ds.Reader.Add(reader);

            // Параметры
            ds.BaseParams.Add(new Parameter("limit", "25", ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("dir", "ASC"));
            ds.BaseParams.Add(new Parameter("sort", "ID"));
            
            ds.DirectEventConfig.Method = HttpMethod.POST;
            ds.DirectEventConfig.CleanRequest = true;

            // Устанавливаем обработчики событий из конфигурации
            foreach (KeyValuePair<string, string> storeListener in StoreListeners)
            {
                ds.Listeners.AddListerer(storeListener.Key, storeListener.Value);
            }

            ds.Listeners.BeforeLoad.AddBefore(
                "{0}.el.mask('Загрузка данных...', 'x-mask-loading');"
                .FormatWith(EntityFormId));
            
            ds.Listeners.LoadException.AddBefore(
                "{0}.el.unmask();atert('LoadException');"
                .FormatWith(EntityFormId));
            
            ds.Listeners.BeforeSave.AddBefore(
                "{0}.el.mask('Сохранение данных...', 'x-mask-loading');"
                .FormatWith(EntityFormId));

            ds.Listeners.Save.AddAfter("ViewPersistence.onFormSaved();");
            
            ds.Listeners.Load.AddBefore(@"
var isNew = false;
if (records.length > 0) {{
    {0}.getForm().loadRecord(records[0]);
}} else {{
    isNew = true;
    {0}.getForm().reset();
    ViewPersistence.isChanged = true;
}}
{0}.el.unmask();".FormatWith(EntityFormId));
            
            return ds;
        }
    }
}
