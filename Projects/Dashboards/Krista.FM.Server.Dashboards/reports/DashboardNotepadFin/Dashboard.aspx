<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Dashboard.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.DashboardNotepadFin.DashboardNotepadFin"
    Title="Сравнение субъектов РФ" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc9" %>
<%@ Register Src="../../Components/GadgetContainer.ascx" TagName="GadgetContainer"
    TagPrefix="uc8" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc7" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc6" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="vertical-align: top">
        <table style="vertical-align: top;">
            <tr>
                <td rowspan="2" valign="top">
                    <uc7:PopupInformer ID="PopupInformer1" runat="server" />
                </td>
                <td rowspan="2">
                    <asp:Image ID="imgHerald" runat="server" Visible="false" />
                </td>
                <td colspan="3">
                    <asp:Label ID="TitleLabel" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
                    <asp:Label ID="SubTitleLabel" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label></td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc6:CustomMultiCombo ID="rzprCombo" runat="server" />
                </td>
                <td>
                    <uc9:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                 <td>
                    <asp:CheckBox ID="BudgetLevel" runat="server" Text="Консолидированный бюджет" AutoPostBack="true"
                        Checked="false" Style="font-family: Verdana; font-size: 10pt;" /></td>
            </tr>
        </table>
        <table>
            <tr>
                <td valign="top">
                    <uc8:GadgetContainer ID="GadgetContainer4" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0011_Gadget.ascx" />
                </td>
                <td valign="top">
                    <uc8:GadgetContainer ID="GadgetContainer5" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0001_Gadget.ascx" />
                </td>
            </tr>
            <tr>
                <td colspan="2" valign="top">
                    <uc8:GadgetContainer ID="GadgetContainer1" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0004_Gadget.ascx" />
                </td>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <uc8:GadgetContainer ID="GadgetContainer7" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0007_Gadget.ascx" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <uc8:GadgetContainer ID="GadgetContainer3" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0003_Gadget.ascx" />
                </td>
                <td valign="top" rowspan="2">
                    <uc8:GadgetContainer ID="GadgetContainer6" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0006_Gadget.ascx" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <uc8:GadgetContainer ID="GadgetContainer2" runat="server" GadgetPath="~/reports/DashboardNotepadFin/FO_0002_0002_Gadget.ascx" />
                </td>
                <td valign="top">
                </td>
            </tr>
        </table>
        <input id="screen_height" type="hidden" runat="server" />
        <input id="screen_width" type="hidden" runat="server" />
    </div>

    <script type="text/javascript">
        function uncheck(checkBoxId)
        { 
	        var checkbox = document.getElementById(checkBoxId);
	        checkbox.checked = !checkbox.checked;
        } 
    </script>

</asp:Content>
