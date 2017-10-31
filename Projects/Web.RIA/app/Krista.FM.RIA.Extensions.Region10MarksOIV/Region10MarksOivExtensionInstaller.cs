using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV
{
    public class Region10MarksOivExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public Region10MarksOivExtensionInstaller()
            : base(typeof(Region10MarksOivExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.Region10MarksOIV.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.Region10MarksOIV.Region10MarksOivExtensionInstaller, Krista.FM.RIA.Extensions.Region10MarksOIV"; }
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            container.RegisterType<IFactRepository, FactRepository>();
            container.RegisterType<IMarksDataInitializer, MarksDataInitializer>();
            container.RegisterType<IMarksCalculator, MarksCalculator>();
            container.RegisterType<IMarksRepository, MarksRepository>();
            container.RegisterType<ITerritoryRepository, TerritoryRepository>();
            container.RegisterType<IOivService, OivService>();
            container.RegisterType<IExportService, ExportService>();
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new Region10MarksOivExtension(
               Resolver.Get<IScheme>(),
               Resolver.Get<IRepository<FX_Date_Year>>(),
               Resolver.Get<ILinqRepository<D_Regions_Analysis>>(),
               Resolver.Get<ILinqRepository<D_Territory_RF>>(),
               Resolver.Get<ILinqRepository<DataSources>>(),
               Resolver.Get<ILinqRepository<D_OMSU_ResponsOIVUser>>(),
               Resolver.Get<ILinqRepository<D_OMSU_ResponsOIV>>());

            Resolver.RegisterInstance<IRegion10MarksOivExtension>(extension, LifetimeManagerType.Session);

            if (!extension.Initialize())
            {
                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Инициализация блока ОИВ",
                    Html = "Во время инициализации произошли ошибки. Подробности в логе сервера.",
                    HideDelay = 5000
                });

                clientExtensionService.AddClientExtension(message.ToScript());
            }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            Navigation navigation = base.ConfigureNavigation(parametersService);

            var extension = Resolver.Get<IRegion10MarksOivExtension>();
            var years = extension.Years;

            if (years != null)
            {
                // Формируем список имеющихся годов
                var yearListItems = new ListItemCollection<ListItem>();
                for (int i = years.Count - 1; i >= 0; i--)
                {
                    var year = years[i];
                    yearListItems.Add(new ListItem(year.ToString()));
                }

                var yearChooser = new ComboBox
                                      {
                                          ID = "cmbOivYearChooser",
                                          Icon = Icon.CalendarSelectDay,
                                          ToolTips = { new ToolTip { Html = "Выбрать актуальный год" } },
                                          Mode = DataLoadMode.Local,
                                          Editable = false,
                                          FieldLabel = "Актуальный год",
                                          Value = extension.CurrentYearVal
                                      };
                yearChooser.Items.AddRange(yearListItems);

                // При смене года нельзя чтобы в карточках были изменения
                yearChooser.Listeners.BeforeSelect.Handler =
                    @"
if (item.oldValue != record.data.value){
   var tabs = MdiTab.getTabs();
   for (var i = 0; i < tabs.length; i++){
     var tab = tabs[i];
     var tabName = tab.autoLoad.url.split('/')[2];
     switch (tabName){
       case 'Region10MarksOmsu':
         var w = tab.getBody();
         if (w.ViewPersistence.isDirty.call(w)){
            MdiTab.setActiveTab();            
            Ext.Msg.show({title:'Предупреждение',msg: 'Имеются несохраненные изменений!',buttons: Ext.Msg.OK,icon: Ext.MessageBox.ERROR,maxWidth: 1000});
            return false;
         }
         break;
       default:
     }
   }
   item.needExecuteDirectEventSelect = true;
}
else
{
  item.needExecuteDirectEventSelect = false;
}
return true;
";

                yearChooser.DirectEvents.Select.Url = "/Region10MarksOiv/SetCurrentYear/";
                yearChooser.DirectEvents.Select.IsUpload = false;
                yearChooser.DirectEvents.Select.CleanRequest = true;
                yearChooser.DirectEvents.Select.ExtraParams.Add(new Parameter("year", string.Format("{0}.getValue()", yearChooser.ID), ParameterMode.Raw));
                yearChooser.DirectEvents.Select.Failure = "Ext.Msg.show({title:'Ошибка',msg: response.responseText,buttons: Ext.Msg.OK,icon: Ext.MessageBox.ERROR,maxWidth: 1000});";
                yearChooser.DirectEvents.Select.Before = @"
if (item.needExecuteDirectEventSelect){
  return true;
}else{
return false;
}
";
                yearChooser.DirectEvents.Select.Success =
                    @"
var tabs = MdiTab.getTabs();
   for (var i = 0; i < tabs.length; i++){
     var tab = tabs[i];
     var w = tab.getBody();
     if (w.ViewPersistence != undefined){
       w.ViewPersistence.refresh.call(w);
     }
   }
";
                navigation.ButtomBar.Add(yearChooser);
            }

            navigationService.AddNavigation(navigation);
        }
    }
}
