<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Import Namespace="Krista.FM.Domain.Services.FinSourceDebtorBook" %>
<%@ Import Namespace="Krista.FM.RIA.Core" %>
<%@ Import Namespace="Krista.FM.RIA.Extensions.DebtBook" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Параметры отчета</title>
    <script src="<%# ResourceRegister.Script("/Content/js/Extension.View.js")%>" type="text/javascript"></script>
    <script src="<%# ResourceRegister.Script("/Content/js/DebtBook.ReportParams.js")%>" type="text/javascript"></script>
    <script runat="server">
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.Header.DataBind();
            
            IDebtBookExtension extension = (IDebtBookExtension) Model;
            
            string report;
            // При первом вызове Request.Params["report"] происходит исключение,
            // но при повторном вызове все срабатывает...
            try
            {
                report = Request.Params["report"];
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                report = Request.Params["report"];
            }
            reportType.Text = report;
            userRegion.Text = Convert.ToString(extension.UserRegionId);
            LP_userRegion.Text = extension.UserRegionName;
            LP_userRegion.Disabled = extension.UserRegionType != UserRegionType.Subject;
        }
    </script>
</head>
<body>

    <ext:ResourceManager ID="ResourceManager1" runat="server"/>

    <ext:FormPanel ID="ReportParamsForm" runat="server" Method="GET" Url="/" 
        Padding="5" Border="false">
        <Items>
            <ext:TextField ID="reportType" runat="server" Hidden="true"/>
            <ext:TextField ID="userRegion" runat="server" Hidden="true"/>
            <ext:TriggerField ID="LP_userRegion" runat="server" FieldLabel="Район" AllowBlank="false" Width="250">
                <Triggers>
                    <ext:FieldTrigger Icon="Ellipsis" Qtip="Щелкните для выбора значения из справочника"/>
                </Triggers>
                <Listeners>
                    <TriggerClick Handler="triggerClick(item, '383f887a-3ebb-4dba-8abb-560b5777436f');" />
                </Listeners>
            </ext:TriggerField>

            <ext:TextField ID="userId" runat="server" Hidden="true"/>
            <ext:TriggerField ID="LP_userId" runat="server" FieldLabel="Подпись" AllowBlank="false" Width="250">
                <Triggers>
                    <ext:FieldTrigger Icon="Ellipsis" Qtip="Щелкните для выбора значения из справочника"/>
                </Triggers>
                <Listeners>
                    <TriggerClick Handler="triggerClick(item, '4d192956-aced-4718-a87c-b2e5519c022a');" />
                </Listeners>
            </ext:TriggerField>
        </Items>
    </ext:FormPanel>

    <ext:Window 
        ID="BookWindow" 
        runat="server" 
        Icon="ApplicationFormEdit" 
        Width="480" 
        Height="360" 
        Hidden="true" 
        Modal="true"
        Constrain="true">
        <AutoLoad 
            Url="/Entity/Show?objectKey=7ef0edfd-9461-4333-8420-ccb102051826" 
            Mode="IFrame" 
            TriggerEvent="show" 
            ReloadOnEvent="true" 
            ShowMask="true" 
            MaskMsg="Загрузка справочника...">
            <Params>
                <ext:Parameter Name="id" Value="" Mode="Value" />
                <ext:Parameter Name="sourceId" Value="" Mode="Value" />
            </Params>
        </AutoLoad>
        <Listeners>
            <Update Handler="BookWindow.getBody().Extension.entityBook.onRowSelect = Extension.DebtBookReportParams.onBookRowSelect;" />
        </Listeners>
        <Buttons>
            <ext:Button runat="server" ID="btnOk" Text="OK" Icon="Accept" Disabled="true">
                <Listeners>
                    <Click Fn="bookClose" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
</body>
</html>
