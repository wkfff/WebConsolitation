<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FK_0004_0006.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc9" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Assembly="Infragistics35.Web.v11.2, Version=11.2.20112.1019, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.Web.UI.GridControls" TagPrefix="ig" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html"
                    Visible="false" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc9:CustomCalendar ID="CustomCalendar1" runat="server"></uc9:CustomCalendar>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
            <td style="vertical-align: top;">
                <asp:Label ID="lbInfo" runat="server" CssClass="InformationText"></asp:Label>
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
    <table style="width: 100%">
        <tr>
            <td>
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
                        <td class="headerReport" style="overflow: visible;">
                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle">Исполнение федеральных целевых программ</asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td valign="top" align="right" style="padding-top: 4px">
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td valign="top" align="left" style="padding-top: 4px">
                            <asp:CheckBox ID="ShowAll" runat="server" Text="Показать подпрограммы и мероприятия" AutoPostBack="true"
                                Checked="false" CssClass="InformationText" />
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <asp:ScriptManager ID="ScriptManager1" runat="server">
                            </asp:ScriptManager>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <!-- child controls -->
                                    <ig:WebDataGrid ID="WebDataGrid1" runat="server" StyleSetName="Cfo" DefaultColumnWidth="100px"
                                        Width="800px" AutoGenerateColumns="False">
                                        <Behaviors>
                                            <ig:Activation>
                                            </ig:Activation>
                                        </Behaviors>
                                    </ig:WebDataGrid>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ShowAll" EventName="CheckedChanged"></asp:AsyncPostBackTrigger>
                                </Triggers>
                            </asp:UpdatePanel>
                            <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" Height="20px"
                                Visible="False" Width="200px" ForeColor="White">
                                <Gauges>
                                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                                        <Scales>
                                            <igGaugeProp:LinearGaugeScale>
                                                <MajorTickmarks EndExtent="35" StartExtent="22">
                                                    <StrokeElement Color="Transparent">
                                                    </StrokeElement>
                                                </MajorTickmarks>
                                                <Markers>
                                                    <igGaugeProp:LinearGaugeBarMarker BulbSpan="10" InnerExtent="20" OuterExtent="80"
                                                        SegmentSpan="99" ValueString="40">
                                                        <Background>
                                                            <BrushElements>
                                                                <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" StartColor="64, 64, 64" />
                                                            </BrushElements>
                                                        </Background>
                                                        <BrushElements>
                                                            <igGaugeProp:MultiStopLinearGradientBrushElement Angle="90">
                                                                <ColorStops>
                                                                    <igGaugeProp:ColorStop Color="253, 119, 119" />
                                                                    <igGaugeProp:ColorStop Color="239, 87, 87" Stop="0.417241365" />
                                                                    <igGaugeProp:ColorStop Color="224, 0, 0" Stop="0.42889908" />
                                                                    <igGaugeProp:ColorStop Color="199, 0, 0" Stop="1" />
                                                                </ColorStops>
                                                            </igGaugeProp:MultiStopLinearGradientBrushElement>
                                                        </BrushElements>
                                                    </igGaugeProp:LinearGaugeBarMarker>
                                                </Markers>
                                                <Ranges>
                                                    <igGaugeProp:LinearGaugeRange EndValueString="100" InnerExtent="20" OuterExtent="80"
                                                        StartValueString="0">
                                                        <BrushElements>
                                                            <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" StartColor="64, 64, 64" />
                                                        </BrushElements>
                                                    </igGaugeProp:LinearGaugeRange>
                                                </Ranges>
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                                </BrushElements>
                                                <StrokeElement Color="Transparent" Thickness="0">
                                                </StrokeElement>
                                                <Axes>
                                                    <igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="12.5" />
                                                </Axes>
                                            </igGaugeProp:LinearGaugeScale>
                                        </Scales>
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                        </BrushElements>
                                        <StrokeElement Color="White" Thickness="0">
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                            </BrushElements>
                                        </StrokeElement>
                                    </igGaugeProp:LinearGauge>
                                </Gauges>
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/1.png" />
                            </igGauge:UltraGauge>
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
</asp:Content>
