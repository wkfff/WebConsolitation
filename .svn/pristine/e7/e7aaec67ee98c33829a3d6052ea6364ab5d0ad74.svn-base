<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0001._0028._02._default" %>

<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <table>
        <tr>
            <td style="width: 100%">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Мониторинг ситуации на рынке труда по области (картограммы)</asp:Label></td>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    <table style="vertical-align: top">
        <tr>
            <td colspan="2" valign="top">
                <uc2:CustomMultiCombo ID="Year" runat="server" Title="Период" />
            </td>
            <td valign="top">
                <uc3:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td align="right" style="width: 100%" valign="top">
                &nbsp;
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse; background-color: white">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="headerleft">
            </td>
            <td class="headerReport">
                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle"></asp:Label></td>
            <td class="headerright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="vertical-align: top">
    <DMWC:MapControl ID="Mapo" runat="server" BackColor="Transparent" BackSecondaryColor="Transparent"
        BorderLineColor="Black" BorderLineWidth="1" Height="800px" ImageType="Bmp" ImageUrl="../../../../TemporaryImages/MapPic_#SEQ(300,3)"
        ResourceKey="#MapControlResKey#MapControl1#" Width="1024px">
<NavigationPanel>
<Location X="0" Y="0"></Location>

<Size Width="90" Height="90"></Size>
</NavigationPanel>

<Viewport>
<Location X="0.0977517143" Y="0.125156447"></Location>

<Size Width="99.80469" Height="99.75"></Size>
</Viewport>

<ZoomPanel>
<Location X="0" Y="0"></Location>

<Size Width="40" Height="200"></Size>
</ZoomPanel>

<ColorSwatchPanel Visible="True">
<Size Width="180" Height="80"></Size>

<Location X="0.0977517143" Y="89.98749"></Location>
</ColorSwatchPanel>

<DistanceScalePanel>
<Location X="0" Y="0"></Location>

<Size Width="130" Height="55"></Size>
</DistanceScalePanel>
</DMWC:MapControl>
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
    <table style="width: 100%; border-collapse: collapse; background-color: white">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="headerleft">
            </td>
            <td class="headerReport">
                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle">Коэффициент напряженности на рынке труда (человек на одну вакансию)</asp:Label></td>
            <td class="headerright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="vertical-align: top">
    <DMWC:MapControl ID="Mapa" runat="server" BackColor="Transparent" BackSecondaryColor="Transparent"
        BorderLineColor="Black" BorderLineWidth="1" Height="800px" ImageType="Bmp" ImageUrl="../../../../TemporaryImages/MapPic_#SEQ(300,3)"
        ResourceKey="#MapControlResKey#MapControl1#" Width="1024px">
<NavigationPanel>
<Location X="0" Y="0"></Location>

<Size Width="90" Height="90"></Size>
</NavigationPanel>

<Viewport>
<Location X="0.0977517143" Y="0.125156447"></Location>

<Size Width="99.80469" Height="99.75"></Size>
</Viewport>

<ZoomPanel>
<Location X="0" Y="0"></Location>

<Size Width="40" Height="200"></Size>
</ZoomPanel>

<ColorSwatchPanel Visible="True">
<Size Width="180" Height="80"></Size>

<Location X="0.0977517143" Y="89.98749"></Location>
</ColorSwatchPanel>

<DistanceScalePanel>
<Location X="0" Y="0"></Location>

<Size Width="130" Height="55"></Size>
</DistanceScalePanel>
</DMWC:MapControl>
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
