<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.DashboardFederal_NAO" Title="Сравнение субъектов РФ" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc9" %>
<%@ Register Src="../../Components/GadgetContainer.ascx" TagName="GadgetContainer"
    TagPrefix="uc8" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc6" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="FK_0001_0003_Gadget.ascx" TagName="FK_0001_0003_Gadget" TagPrefix="uc5" %>
<%@ Register Src="FK_0001_0002_Gadget.ascx" TagName="FK_0001_0002_Gadget" TagPrefix="uc4" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div runat="server" id="ComprehensiveDiv" style="vertical-align: top">
        <table style="vertical-align: top;">
            <tr>
                <td valign="top" align="right">
                    <uc6:CustomMultiCombo ID="regionsCombo" runat="server" />
                </td>
                <td valign="top" align="left" style="width: 100%">
                    <uc9:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right">
                    <asp:HyperLink ID="WallLink" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
                    <asp:HyperLink ID="BlackStyleWallLink" runat="server" SkinID="HyperLink"></asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table>
                        <tr>
                            <td>
                                <asp:Image ID="imgHerald" runat="server" />
                            </td>
                            <td>
                                <asp:Label ID="TitleLabel" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br />
                                <asp:Label ID="SubTitleLabel" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td style="vertical-align: top; padding: 0px 0px 0px 0px; margin-top: -5px;">
                    <uc8:GadgetContainer ID="GadgetContainer2" runat="server" GadgetPath="~/reports/DashboardFederal_NAO/FK_0001_0002_Gadget.ascx"
                        HeaderVisible="false" />
                </td>
                <td width="3%">
                </td>
                <td style="vertical-align: top; padding: 0px 0px 0px 0px;">
                    <uc8:GadgetContainer ID="GadgetContainer3" runat="server" GadgetPath="~/reports/DashboardFederal_NAO/FK_0001_0003_Gadget.ascx"
                        HeaderVisible="false" />
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Label ID="RedArrowHintLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="YellowArrowHintLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="BestStarHintLabel" runat="server"></asp:Label>&nbsp;&nbsp;
                    <asp:Label ID="WorseStarHintLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="EqualbilityHintLabel" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <input id="screen_height" type="hidden" runat="server" />
        <input id="screen_width" type="hidden" runat="server" />
    </div>
</asp:Content>
