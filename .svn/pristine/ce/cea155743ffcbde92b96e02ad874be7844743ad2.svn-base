<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="DefaultDetail.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FO_0016_0002.Default" %>

<%@ Register Src="../../Components/GaugeIndicator.ascx" TagName="GaugeIndicator"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc2:CustomMultiCombo ID="ComboYear" runat="server"></uc2:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc2:CustomMultiCombo ID="ComboQuarter" runat="server"></uc2:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc2:CustomMultiCombo ID="ComboRegion" runat="server"></uc2:CustomMultiCombo>
            </td>            
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td runat="server" id="GaugesTD">
              
            </td>
        </tr>
    </table>
</asp:Content>