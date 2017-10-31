<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomCalendar.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.CustomCalendar" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDateChooser.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>

<script id="Infragistics" type="text/javascript">
<!--

function webCalendar_ValueChanged(oCalendar, oDate, oEvent){
	EnableSubmitButton();
}
// -->
</script>
<igmisc:WebPanel ID="webPanel" runat="server" EnableAppStyling="True"
    StyleSetName="Office2007Blue" Width="204px" Expanded="False"  ExpandEffect="None" Font-Size="11px" ExpandOnClick="true" EnableViewState="false">
    <Template>
        <igsch:WebCalendar ID="webCalendar" runat="server" EnableAppStyling="True" StyleSetName="Office2007Blue"
            Width="202px">
            <Layout HideOtherMonthDays="True">
                <CalendarStyle Width="202px">
                </CalendarStyle>
                <FooterStyle CustomRules="display:none" />
                <DayStyle Font-Names="Verdana" ForeColor="#1572B4" />
                <DropDownStyle Font-Names="Verdana" ForeColor="#1572B4">
                </DropDownStyle>
            </Layout>
            <ClientSideEvents ValueChanged="webCalendar_ValueChanged" />
        </igsch:WebCalendar>
    </Template>
    <Header>
        <ExpandedAppearance>
            <Styles Font-Size="12px">
                <Padding Bottom="5px" Left="3px" Top="3px" />
            </Styles>
        </ExpandedAppearance>
    </Header>
</igmisc:WebPanel>
