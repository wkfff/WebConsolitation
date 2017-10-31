using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Presentation.Views
{
    public sealed class AccountsManagerView : View
    {
        public const string AccountsID = "Accounts";

        public const string AccountsStoreID = "AccountsStore";

        private readonly IAuthService auth;

        public AccountsManagerView(IAuthService auth)
        {
            this.auth = auth;
        }

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            RestActions restActions = ResourceManager.GetInstance(Page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.PUT;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            Store userStore = GetStore(AccountsStoreID);
            Page.Controls.Add(userStore);

            var view = new Viewport
                {
                    ID = "viewportMain", 
                    Items =
                        {
                            new BorderLayout
                                {
                                    Center = { Items = { CreateUsersGridPanel(userStore.ID) } }
                                }
                        }
                };

            return new List<Component> { view };
        }

        private IEnumerable<Component> CreateUsersGridPanel(string storeId)
        {
            var table = new GridPanel
                {
                    ID = AccountsID, 
                    StoreID = storeId, 
                    Border = false, 
                    TrackMouseOver = true, 
                    AutoScroll = true, 
                    Layout = LayoutType.Fit.ToString(), 
                    LoadMask = { ShowMask = true, Msg = "Загрузка..." }, 
                    ClearEditorFilter = false, 
                    AutoExpandColumn = FiedsId.Login, 
                    StripeRows = true, 
                    ClicksToEdit = 1, 
                    View = { new LockingGridView { ForceFit = false } }, 
                    Plugins =
                        {
                            new GridFilters
                                {
                                    ID = string.Concat(AccountsID, "Filters"), 
                                    Local = false, 
                                    Filters =
                                        {
                                            new StringFilter { DataIndex = FiedsId.Login }, 
                                            new StringFilter { DataIndex = FiedsId.OrgName }, 
                                            new DateFilter { DataIndex = FiedsId.LastLogin }, 
                                        }, 
                                }, 
                        }, 
                };

            var selMod = new RowSelectionModel { ID = "RowSelectionModel", SingleSelect = false, };
            table.SelectionModel.Add(selMod);

            table.AddRefreshButton();

            table.AddSaveButton();

            table.Toolbar().Add(
                new Button
                    {
                        ID = "btnCreateAgents", 
                        Icon = Icon.UserMagnify, 
                        Text = @"Создать представителей", 
                        ToolTip = @"Создать аккаунты представителей по организациям", 
                        DirectEvents =
                            {
                                Click =
                                    {
                                        Method = HttpMethod.POST, 
                                        Url = "/Accounts/CreateAgents", 
                                        CleanRequest = true, 
                                        Timeout = 60000, 
                                        Before = "Ext.Msg.wait('Загружается...', 'Загрузка');", 
                                        Success =
                                            @"
            
                  Ext.MessageBox.hide();
                  Ext.net.Notification.show({iconCls : 'icon-information',
                                                 html : result.extraParams.msg,
                                              title : 'Уведомление', hideDelay : 2500});", 
                                        Failure =
                                            @"
                  
                    if (result.extraParams != undefined && result.extraParams.responseText!=undefined){
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
                                      icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });}", 
                                    }, 
                            }, 
                    });
            table.Toolbar().Add(
                new Button
                    {
                        ID = "btnResetAccountsPassord", 
                        Icon = Icon.GroupKey, 
                        Text = @"Сбросить пароли", 
                        ToolTip = @"Сбросить пароли", 
                        DirectEvents =
                            {
                                Click =
                                    {
                                        Method = HttpMethod.PUT, 
                                        Url = "/Accounts/ResetAccountsPassord", 
                                        Confirmation =
                                            {
                                                ConfirmRequest = true, 
                                                Title = "Подтверждение сброса паролей", 
                                                Message =
                                                    @"<p/>Система автоматически создаст для каждого пользователя новый пароль.
<p/>Новые пароли, после завершения операции, можно будет получить у администратора.
<p/>Сброс паролей для всех пользователей - это <b>необратимое изменение</b>, аутентификация пользователя по старому паролю будет невозможна.
<p/>Вы действительно хотите продолжить?", 
                                            }, 
                                        CleanRequest = true, 
                                        Timeout = 60000, 
                                        Before = "Ext.Msg.wait('Загружается...', 'Загрузка');", 
                                        Success =
                                            @"
            
                  Ext.MessageBox.hide();
                  Ext.net.Notification.show({iconCls : 'icon-information',
                                                 html : result.extraParams.msg,
                                              title : 'Уведомление', hideDelay : 2500});", 
                                        Failure =
                                            @"
                  
                    if (result.extraParams != undefined && result.extraParams.responseText!=undefined){
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
                                      icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });}", 
                                    }, 
                            }, 
                    });

            table.Toolbar().Add(
                new Button
                    {
                        ID = "btnAllowPwdAuth", 
                        Icon = Icon.KeyStart, 
                        Text = @"Разрешить аутентификацию логин/пароль", 
                        ToolTip = @"Активировать пользователей", 
                        DirectEvents =
                            {
                                Click =
                                    {
                                        Method = HttpMethod.PUT, 
                                        Url = "/Accounts/ActivateAccounts", 
                                        ExtraParams =
                                            {
                                                new ProgressConfig("Активация пользователей..."), 
                                            }, 
                                        CleanRequest = true, 
                                        Timeout = 60000, 
                                    }
                            }
                    });

            ColumnModel columnModel = table.ColumnModel;

            // var primary = 
            // new List<ReferencerInfo.FieldInfo>();
            // var secondary = 
            // new List<ReferencerInfo.FieldInfo>
            // {
            // new ReferencerInfo.FieldInfo {Caption = "Наименование", Name = "Name"},
            // //new ReferencerInfo.FieldInfo {Caption = "Регистрационный номер", Name = "OGRN"},
            // };

            // columnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger)
            // .SetHidden(true);
            // columnModel.AddColumn(FiedsId.OrgName, "Учереждение", DataAttributeTypes.dtString)
            // .SetWidth(80)
            // .AddLookupForColumn(
            // primary,
            // secondary,
            // "/Entity/DataWithCustomSearch?objectKey={0}".FormatWith(D_Org_Structure.Key),
            // Page);

            // columnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger)
            // .SetHidden(true).SetEditable(false);
            columnModel.AddColumn(FiedsId.Login, "Логин", DataAttributeTypes.dtString)
                .SetEditable(false).SetLocked().SetWidth(90);
            columnModel.AddColumn(FiedsId.IsGRBS, "G", DataAttributeTypes.dtBoolean)
                .SetEditableBoolean().SetWidth(20).SetEditable(
                    auth.IsGrbsUser() || auth.IsPpoUser() ||
                    auth.IsAdmin());
            columnModel.AddColumn(FiedsId.IsPPO, "P", DataAttributeTypes.dtBoolean)
                .SetEditableBoolean().SetWidth(20).SetEditable(auth.IsPpoUser() || auth.IsAdmin());
            columnModel.AddColumn(FiedsId.LastLogin, "Дата последней регистрации", DataAttributeTypes.dtDateTime)
                .SetWidth(100);
            columnModel.AddColumn(FiedsId.OrgName, "Учереждение", DataAttributeTypes.dtString)
                .SetEditable(false).SetWidth(600);
            columnModel.AddColumn(FiedsId.OrgID, "OrgID", DataAttributeTypes.dtString)
                .SetEditable(false).SetHidden(true);
            columnModel.AddColumn(FiedsId.Email, FiedsId.Email, DataAttributeTypes.dtString)
                .SetEditable(false);

            return new List<Component> { table };
        }

        private Store GetStore(string storeId)
        {
            var store = new Store
                {
                    ID = storeId, 
                    Restful = true, 
                    AutoLoad = true, 
                    ShowWarningOnFailure = true, 
                    SkipIdForNewRecords = false, 
                    WarningOnDirty = false, 
                    DirtyWarningTitle = @"Несохраненные изменения", 
                    DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?", 
                    RefreshAfterSaving = RefreshAfterSavingMode.None
                };

            store
                .SetRestController("Accounts")
                .SetJsonReader()
                .AddField("ID")
                .AddField(FiedsId.Login)
                .AddField(FiedsId.OrgID)
                .AddField(FiedsId.OrgName)
                .AddField(FiedsId.Email)
                .AddField(FiedsId.IsGRBS)
                .AddField(FiedsId.IsPPO)
                .AddField(FiedsId.LastLogin);

            store.Listeners.LoadException.Handler =
                "Ext.Msg.alert('Ошибка при загрузке списка учетных записей', response.responseText);";
            store.Listeners.SaveException.Handler =
                "Ext.Msg.alert('Ошибка при сохранение', response.responseText);";
            store.Listeners.Save.Handler =
                "Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение.', hideDelay  : 2500});";
            store.Listeners.Exception.Handler =
                @"if (response.raw != undefined && response.raw.message != undefined)
                {
                    Ext.Msg.alert('Ошибка при сохранении', response.raw.message);
                }else{
                    Ext.Msg.alert('Ошибка', response.responseText);
                }";

            return store;
        }

        #region Nested type: FiedsId

        /// <summary>
        ///   описатель идентификаторов полей
        /// </summary>
        private static class FiedsId
        {
            public const string Login = "Login";

            public const string OrgID = "OrgId";

            public const string OrgName = "OrgName";

            public const string Email = "Email";

            public const string IsGRBS = "IsGrbs";

            public const string IsPPO = "IsPpo";

            public const string LastLogin = "LastLogin";
        }

        #endregion
    }
}
