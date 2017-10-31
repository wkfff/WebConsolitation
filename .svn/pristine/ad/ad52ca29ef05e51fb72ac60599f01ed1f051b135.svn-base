<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.SB_002.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
    <%@ Register Src="../../Components/ChartBricks/PieChartBrick.ascx" TagName="PieChartBrick"
    TagPrefix="uc8" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <asp:Label ID="BroadCast" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br/><asp:Label
                    ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboBank" runat="server">
                </uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top; margin-top: -3px">
        <tr>
            <td valign="top" align="left">
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
                <td class="headerleft">
                </td>
                <td class="headerReport">
                    <asp:Label ID="gridElementCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                </td>
                <td class="headerright">
                </td>
       </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <uc5:UltraGridBrick ID="GridBrick" runat="server">
                            </uc5:UltraGridBrick>
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
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="middle"></td>
            <td valign="middle">
                <uc3:CustomMultiCombo ID="ComboGoal" runat="server">
                </uc3:CustomMultiCombo>
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
        <tr>
            <td class="topleft">
            </td>
            <td class="top" colspan="2">
            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="headerleft">
            </td>
            <td class="headerReport" width="35px">
                <div style="position:absolute;margin-top:-20px;"><asp:Image ID="GoalImage" runat="server" Height="35px" /></div>
            </td>
            <td class="headerReport">
                <asp:Label ID="mapElementCaption" runat="server" CssClass="ElementTitle"></asp:Label>
            </td>
            
            <td class="headerright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="overflow: visible;" colspan="2">
                <table runat="server">
                    <tr>
                        <td align="center" id="ChartTd">
                            <uc8:PieChartBrick ID="PieChartBrick" runat="server"></uc8:PieChartBrick>
                        </td>
                        <td valign="top">
                           <asp:Label ID="PriorityTotalLabel" runat="server" CssClass="PageSubTitle"></asp:Label>
                        </td>
                    </tr>
                </table>
                
                <table runat="server" id="HeraldTable">
                    <tr runat="server" id="HeraldTr">
                    </tr>
                </table>
                <DMWC:MapControl ID="DundasMap" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                    ImageUrl="../../TemporaryImages/map_fo_39_05#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/">
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
                </DMWC:MapControl>
            </td>
            <td class="right">
            </td>
        </tr>
        <tr>
            <td class="bottomleft">
            </td>
            <td class="bottom" colspan="2">
            </td>
            <td class="bottomright">
            </td>
        </tr>
    </table>
</asp:Content>
