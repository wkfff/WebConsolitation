<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Krista.FM.RIA.Core" %>

<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<%@ Register src="~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/Task/TaskTab.ascx" tagname="TaskTab" tagprefix="uc" %>
<%@ Register src="~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/Task/DocumentsTab.ascx" tagname="DocumentsTab" tagprefix="uc" %>
<%@ Register src="~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/Task/UploadWindow.ascx" tagname="UploadWindow" tagprefix="uc" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; windows-1251" />
    <link href="<%# ResourceRegister.Style("/Krista.FM.RIA.Extensions.Tasks/Presentation/Content/css/Task.Show.css/extention.axd") %>" type="text/css" rel="stylesheet" />
    <script src="<%# ResourceRegister.Script("/Krista.FM.RIA.Extensions.Tasks/Presentation/js/Task.Show.js/extention.axd") %>" type="text/javascript"></script>
    <script runat="server">
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            Page.Header.DataBind();
        }
    </script>
</head>
<body>
    <ext:ResourceManager runat="server" />
    
    <ext:Store ID="dsUsers" runat="server" AutoLoad="true">
        <Proxy>
            <ext:HttpProxy Url="/User/DataPaging/" />
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="ID" Root="data" TotalProperty="totalCount">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="NAME" />
                    <ext:RecordField Name="FIRSTNAME" />
                    <ext:RecordField Name="LASTNAME" />
                    <ext:RecordField Name="PATRONYMIC" />
                    <ext:RecordField Name="JOBTITLE" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    
    <ext:Store ID="dsTask" runat="server" ShowWarningOnFailure="true">
        <Proxy>
            <ext:HttpProxy Url="/Task/Get/"/>
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="TaskID" Root="data" TotalProperty="totalCount">
                <Fields>
                    <ext:RecordField Name="TaskID"/>
                    <ext:RecordField Name="Headline"/>
                    <ext:RecordField Name="Job"/>
                    <ext:RecordField Name="Description"/>
                    <ext:RecordField Name="FromDate"/>
                    <ext:RecordField Name="ToDate"/>
                    <ext:RecordField Name="Owner"/>
                    <ext:RecordField Name="OwnerName"/>
                    <ext:RecordField Name="Doer"/>
                    <ext:RecordField Name="DoerName"/>
                    <ext:RecordField Name="Curator"/>
                    <ext:RecordField Name="CuratorName"/>
                    <ext:RecordField Name="State"/>
                    <ext:RecordField Name="Editable"/>
                    <ext:RecordField Name="Actions" IsComplex="true"/>
                    <ext:RecordField Name="CashedAction"/>
                    <ext:RecordField Name="CanDoAction"/>
                    <ext:RecordField Name="LockedUserName"/>
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="start" Value="0" Mode="Raw" />
            <ext:Parameter Name="id" Value="#{txtFilter}.getValue()" Mode="Raw" />
        </BaseParams>
        <Listeners>
            <BeforeLoad Handler="#{TaskPanel}.el.mask('Загрузка задачи...', 'x-mask-loading');" />
            <LoadException Handler="#{TaskPanel}.el.unmask();" />
            <Load Fn="taskLoaded" />
        </Listeners>
    </ext:Store>
    
    <ext:Store ID="dsDocuments" runat="server" ShowWarningOnFailure="true" AutoLoad="false">
        <Proxy>
            <ext:HttpProxy Url="/Task/GetDocuments/" />
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="ID" Root="data.Rows" TotalProperty="totalCount">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="NAME" />
                    <ext:RecordField Name="SOURCEFILENAME" />
                    <ext:RecordField Name="DOCUMENTTYPENAME" />
                    <ext:RecordField Name="DESCRIPTION" />
                    <ext:RecordField Name="VERSION" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="id" Value="#{txtFilter}.getValue()" Mode="Raw" />
        </BaseParams>
        <Listeners>
            <BeforeLoad Handler="return !this.loaded;" />
            <Load Handler="this.loaded = true;" />
        </Listeners>
    </ext:Store>

    <ext:Viewport ID="ViewPort1" runat="server" EnableTheming="true">
        <Content>
            <ext:FitLayout ID="FitLayout1" runat="server">
                <Items>
                    <ext:Panel ID="ToolbarsPanel" runat="server" Border="false">
                        <TopBar>
                            <ext:Toolbar ID="taskToolbar" runat="server">
                                <Items>
                                    <ext:Button ID="btnApplay" runat="server" Text="Применить" Icon="Tick" >
                                        <Listeners>
                                            <Click Fn="onApplayClick"/>
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnCancel" runat="server" Text="Отменить" Icon="Cross" >
                                        <Listeners>
                                            <Click Fn="onCancelClick"/>
                                        </Listeners>
                                    </ext:Button>
                                    <ext:ToolbarSeparator/>
                                    <ext:Button ID="btnPhantom" runat="server" Icon="CogGo"/>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Content>
                            <ext:Hidden ID="txtFilter" runat="server" Text='<%# this.ViewData["id"] %>' AutoDataBind="true" />
                            <ext:FitLayout ID="FitLayout2" runat="server">
                                <Items>
                                    <ext:TabPanel ID="TaskPanel" runat="server" Border="false" LayoutOnTabChange="true">
                                        <Items>
                                            <ext:Panel ID="tabGeneralDetails" runat="server" Title="Задача" BodyStyle="padding:6px;">
                                                <Content>
                                                    <uc:TaskTab ID="taskTabControl" runat="server" />
                                                </Content>
                                            </ext:Panel>
                                            <ext:Panel ID="tabDocuments" runat="server" Title="Документы" BodyStyle="background-color:#F0F0F0;">
                                                <Content>
                                                    <uc:DocumentsTab ID="documentsTabControl" runat="server"/>
                                                </Content>
                                                <Listeners>
                                                    <Activate Handler="#{dsDocuments}.reload();" />
                                                </Listeners>
                                            </ext:Panel>
                                        </Items>
                                    </ext:TabPanel>
                                </Items>
                            </ext:FitLayout>
                        </Content>
                    </ext:Panel>
                </Items>
            </ext:FitLayout>
        </Content>
    </ext:Viewport>
    
    <uc:UploadWindow ID="UploadWindowControl" runat="server" />
</body>
</html>
