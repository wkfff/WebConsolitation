<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FO.FO_0002._001._default" %>
<%@ Register Src="../../../../Components/GadgetContainer.ascx" TagName="GadgetContainer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../Components/GaugeIndicator.ascx" TagName="GaugeIndicator"
    TagPrefix="uc6" %>
<%@ Register Src="../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc7" %>

<%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc2" %>
<%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
    <%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%">
                <uc2:PopupInformer ID="PopupInformer1" runat="server" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>
                &nbsp;
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label></td>
            <td>
                <uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server" OnLoad="ComboYear_Load" PanelHeaderTitle="Период"
                    ParentSelect="false" ShowSelectedValue="false" />
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboLevel" runat="server" Title="Уровень бюджета" />
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top">
                <div style="visibility: hidden">
                    <igGauge:UltraGauge ID="Ga1" runat="server" BackColor="Transparent" Height="20px"
                        Style="visibility: hidden" Width="100px">
                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/1.png" />
                        <Gauges>
                            <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                                <scales>
<igGaugeProp:LinearGaugeScale>
<MajorTickmarks StartExtent="22" EndExtent="35">
<StrokeElement Color="Transparent"></StrokeElement>
</MajorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="99" OuterExtent="80" InnerExtent="20" BulbSpan="10" ValueString="40">
<Background><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</Background>
<BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="253, 119, 119"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="239, 87, 87" Stop="0.417241365"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="224, 0, 0" Stop="0.42889908"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="199, 0, 0" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>
<Ranges>
<igGaugeProp:LinearGaugeRange InnerExtent="20" OuterExtent="80" StartValueString="0" EndValueString="100"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>

<StrokeElement Color="Transparent"></StrokeElement>
<Axes>
<igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="12.5"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:LinearGaugeScale>
</scales>
                                <strokeelement color="White" thickness="0"></strokeelement>
                            </igGaugeProp:LinearGauge>
                        </Gauges>
                    </igGauge:UltraGauge>
                    <igGauge:UltraGauge ID="Ga2" runat="server" BackColor="Transparent" Height="20px"
                        Style="visibility: hidden" Width="100px">
                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/2.png" />
                        <Gauges>
                            <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                                <scales>
<igGaugeProp:LinearGaugeScale EndExtent="98" StartExtent="2">
<StrokeElement Color="Transparent"></StrokeElement>
</igGaugeProp:LinearGaugeScale>
<igGaugeProp:LinearGaugeScale>
<MajorTickmarks StartExtent="22" EndExtent="35">
<StrokeElement Color="Transparent"></StrokeElement>
</MajorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="99" OuterExtent="80" InnerExtent="20" BulbSpan="10" ValueString="40">
<Background><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</Background>
<BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="253, 119, 119"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="239, 87, 87" Stop="0.417241365"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="224, 0, 0" Stop="0.42889908"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="199, 0, 0" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>
<Ranges>
<igGaugeProp:LinearGaugeRange InnerExtent="20" OuterExtent="80" StartValueString="0" EndValueString="100"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>

<StrokeElement Color="Transparent"></StrokeElement>
<Axes>
<igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="12.5"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:LinearGaugeScale>
</scales>
                                <strokeelement thickness="0"></strokeelement>
                            </igGaugeProp:LinearGauge>
                        </Gauges>
                    </igGauge:UltraGauge>
                    &nbsp;
                </div>
            </td>
        </tr>
    </table>
</asp:Content>