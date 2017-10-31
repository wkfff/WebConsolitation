<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Dashboard.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.DashboardFederal" Title="Сравнение субъектов РФ" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc9" %>
<%@ Register Src="../../Components/GadgetContainer.ascx" TagName="GadgetContainer" TagPrefix="uc8" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc7" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc6" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="MFRF_0001_0004_Gadget.ascx" TagName="MFRF_0001_0004_Gadget" TagPrefix="uc1" %>
<%@ Register Src="FK_0001_0001_Gadget.ascx" TagName="FK_0001_0001_Gadget" TagPrefix="uc2" %>
<%@ Register Src="FK_0001_0003_Gadget.ascx" TagName="FK_0001_0003_Gadget" TagPrefix="uc5" %>
<%@ Register Src="FK_0001_0002_Gadget.ascx" TagName="FK_0001_0002_Gadget" TagPrefix="uc4" %>
<%@ Register Src="FK_0001_0004_Gadget.ascx" TagName="FK_0001_0004_Gadget" TagPrefix="uc3" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="vertical-align: top">
        <table style="vertical-align: top;">
            <tr>
                <td rowspan="2" valign="top">
                    <uc7:PopupInformer ID="PopupInformer1" runat="server" />
                </td>
                <td rowspan="2">
                    <asp:Image ID="imgHerald" runat="server" />
                </td>
                <td colspan="3">
                    <asp:Label ID="TitleLabel" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
                    <asp:Label ID="SubTitleLabel" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label></td>
            </tr>
            <tr>
                <td valign="top" align="left">
                    <uc6:CustomMultiCombo ID="regionsCombo" runat="server" />
                </td>
                <td valign="top" align="left">
                    <uc9:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td valign="top" align="right" width="100%">
                    <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td style="vertical-align: top; padding: 0px 0px 0px 0px; margin-top: -5px;">
                    <uc8:GadgetContainer ID="GadgetContainer2" runat="server" GadgetPath="~/reports/dashboardfederal/FK_0001_0002_Gadget.ascx" />
                    <uc8:GadgetContainer ID="GadgetContainer3" runat="server" GadgetPath="~/reports/dashboardfederal/FK_0001_0003_Gadget.ascx" />
                </td>
                <td style="vertical-align: top; padding: 0px 0px 0px 0px;">
                    <uc8:GadgetContainer ID="GadgetContainer1" runat="server" GadgetPath="~/reports/dashboardfederal/FK_0001_0001_Gadget.ascx" />
                    <uc8:GadgetContainer ID="GadgetContainer5" runat="server" GadgetPath="~/reports/dashboardfederal/MFRF_0001_0004_Gadget.ascx" />
                </td>
                <td style="vertical-align: top; padding: 0px 0px 0px 0px;">
                    <uc8:GadgetContainer ID="GadgetContainer4" runat="server" GadgetPath="~/reports/dashboardfederal/FK_0001_0013_Gadget.ascx" />
                    <uc8:GadgetContainer ID="GadgetContainer7" runat="server" GadgetPath="~/reports/dashboardfederal/MFRF_0001_0001_Gadget.ascx" />
                </td>
            </tr>
        </table>
        <input id="screen_height" type="hidden" runat="server" />
        <input id="screen_width" type="hidden" runat="server" />
    </div>
    <div id="TweetButtonPlaceHolder" runat="server"></div>
</asp:Content>
