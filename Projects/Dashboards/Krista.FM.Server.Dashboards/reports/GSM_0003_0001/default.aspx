<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.GSM_0003_0001.Default" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" colspan="1" style="width: 85%;">
                <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="default.html"
                    Visible="true"></uc3:PopupInformer>
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>
                <br />
                <asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="1" style="width: 15%;">
                <uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                <uc5:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                &nbsp;<br />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboDate" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboFuel" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
            <td style="overflow: visible;">
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="325px"
                    SkinID="UltraWebGrid" />
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
    <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
            <td>
                <asp:Label ID="LabelChart1" runat="server" CssClass="ElementTitle" Text="заголовок диаграммы"></asp:Label>
                <igchart:UltraChart ID="UltraChart1" runat="server" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    Version="11.1">
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                </igchart:UltraChart>
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
    <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
            <td>
                <asp:Label ID="LabelMap" runat="server" Text="заголовок карты" CssClass="ElementTitle"></asp:Label>
                <dmwc:mapcontrol id="DundasMap" runat="server" backcolor="White" resourcekey="#MapControlResKey#DundasMap1#"
                    imageurl="../../TemporaryImages/map_org_0001_0007#SEQ(300,3)" renderingimageurl="../../TemporaryImages/">
                    <NavigationPanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="90" Width="90"></Size>
                    </NavigationPanel>
                    <Viewport>
                        <Location X="0" Y="0"></Location>
                        <Size Height="100" Width="100"></Size>
                    </Viewport>
                    <ZoomPanel>
                        <Size Height="200" Width="40"></Size>
                        <Location X="0" Y="0"></Location>
                    </ZoomPanel>
                    <ColorSwatchPanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="60" Width="350"></Size>
                    </ColorSwatchPanel>
                    <DistanceScalePanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="55" Width="130"></Size>
                    </DistanceScalePanel>
                </dmwc:mapcontrol>
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
