<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GaugeIndicator.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.GaugeIndicator" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<table runat="server" id="gaugeTable">
    <tr>
        <td colspan="2">
            <asp:Label ID="gaugeTitle" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <igGauge:UltraGauge ID="ultraGauge" runat="server" BackColor="Transparent" Height="250px"
                Width="100px">
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="5">
<MinorTickmarks EndExtent="50" Frequency="0.2" StartExtent="40">
<StrokeElement Color="Black"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidExtent="87" EndExtent="87" ValueString="62" StartExtent="77" StartWidth="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="135, 189, 214" StartColor="185, 217, 231"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="106, 154, 172"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>

<Shadow Depth="3"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 0, 0, 0"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</igGaugeProp:LinearGaugeNeedle>
<igGaugeProp:LinearGaugeBarMarker ValueString="54" PrecisionString="0" OuterExtent="72" InnerExtent="56"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Brown" StartColor="255, 99, 67" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="3" EndExtent="53" StartExtent="38" StartWidth="3"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="DimGray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="20"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="20" FormatString="&lt;DATA_VALUE:N2&gt;" Font="Trebuchet MS, 9pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Black"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
<igGaugeProp:LinearGaugeScale EndExtent="98" StartExtent="60" OuterExtent="96" InnerExtent="4"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="50, 255, 255, 255" StartColor="150, 255, 255, 255"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:SimpleGradientBrushElement EndColor="182, 215, 230" StartColor="239, 243, 246"></igGaugeProp:SimpleGradientBrushElement>
</brushelements>
                        <strokeelement thickness="5"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="147, 194, 217" StartColor="182, 215, 230"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</strokeelement>
                    </igGaugeProp:LinearGauge>
                </Gauges>
            </igGauge:UltraGauge>
        </td>
        <td align="left" valign="top" style="font-family: Verdana; font-size: 10pt;">
            <b>Численное значение:&nbsp;</b><asp:Label ID="indFactValue" runat="server" CssClass="ElementSubTitle"
                Text=""></asp:Label><br />
            <b>Ранг по индикатору среди районов:&nbsp;</b><asp:Label ID="indRankValue" runat="server"
                CssClass="ElementSubTitle" Text=""></asp:Label><br />
            <b>Минимальное значение по всем районам:&nbsp;</b><asp:Label ID="indMinValue" runat="server"
                CssClass="ElementSubTitle" Text=""></asp:Label><br />
            <b>Максимальное значение по всем районам:&nbsp;</b><asp:Label ID="indMaxValue" runat="server"
                CssClass="ElementSubTitle" Text=""></asp:Label><br />
            <b>Индикатор:&nbsp;</b><asp:Label ID="indNameLabel" runat="server" CssClass="ElementSubTitle"
                Text=""></asp:Label><br />
            <b>Содержание индикатора:&nbsp;</b><asp:Label ID="indContentLabel" runat="server"
                CssClass="ElementSubTitle" Text=""></asp:Label>
            <br />
            <b>Формула:&nbsp;</b><asp:Label ID="indFormulaLabel" runat="server" CssClass="ElementSubTitle"
                Text=""></asp:Label><br />
            <b>Нормативное значение:&nbsp;</b><asp:Label ID="indNormValueLabel" runat="server" CssClass="ElementSubTitle" Text=""></asp:Label>
        </td>
    </tr>
</table>
