<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.IP_0001_0003.Default" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table id="HeaderTable" runat="server" style="vertical-align: top; border-collapse: collapse"
        width="100%">
        <tr>
            <td valign="top">
                <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="default.html"
                    Visible="true"></uc3:PopupInformer>
            </td>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboTerr" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top" style="width: 100%;">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse; margin-top: 10px;">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="overflow: visible; vertical-align: top">
                <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" SkinID="UltraWebGrid">
                </igtbl:UltraWebGrid>
            </td>
            <td class="right">
            </td>
        </tr>
        <tr>
            <td class="bottomleft">
            </td>
            <td class="bottom">
            </td>
            <td class="bottomright">
            </td>
        </tr>
    </table>
</asp:Content>
