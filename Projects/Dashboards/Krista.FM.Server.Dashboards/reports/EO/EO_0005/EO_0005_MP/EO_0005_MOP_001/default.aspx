<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP.EO_0005_MP_001._default.E01._default" %>

<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top; width: 1262px">
    <asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Отчет о выполнении окружных программ в муниципальном образовании {0}." Width="100%"></asp:Label></td>
            <td>
            <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" Visible="true" />
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse">
        <tr>
            <td style="height: 41px" >
<uc3:CustomMultiCombo
ID="Year" runat="server" />
            </td>
            <td colspan="1" rowspan="1" style="height: 41px">
<uc3:CustomMultiCombo ID="Month" runat="server" Title="Месяц" Visible="true" />
            </td>
            <td style="height: 41px">
<uc3:CustomMultiCombo ID="Zakaz" runat="server" Title="Наименование заказчика" />
            </td>
            <td id="tab" style="height: 41px">
            </td>
            <td style="height: 41px">
    <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td style="height: 41px">
                            <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="Transparent" BorderColor="Transparent"
                                ForeColor="ActiveCaptionText" Height="20px" OnDataBinding="UltraGauge1_DataBinding"
                                Width="250px">
                                <Gauges>
                                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                                        <scales>
<igGaugeProp:LinearGaugeScale>
<MajorTickmarks StartExtent="22" EndExtent="35">
<StrokeElement Color="Transparent"></StrokeElement>
</MajorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="99" OuterExtent="80" InnerExtent="20" BulbSpan="10" ValueString="50">
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
                                <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                            </igGauge:UltraGauge>
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse">
        <tr>
            <td>
    
    <table style="width: 100%; border-collapse: collapse; background-color: white" id="TABLE1">
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
            <td style="background-color: white">
                <asp:CheckBox ID="DetailModule" runat="server" AutoPostBack="True"
                    Text="Детализация по мероприятиям" />
                <asp:CheckBox ID="DetailKOSGY" runat="server" AutoPostBack="True" 
                    Text="Детализация по КОСГУ" /><igtbl:UltraWebGrid ID="G" runat="server" EnableAppStyling="True" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="G_DataBinding" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow" style="text-align: right">
    <Bands>
        <igtbl:UltraGridBand>
            <AddNewRow View="NotSet" Visible="NotSet">
            </AddNewRow>
        </igtbl:UltraGridBand>
    </Bands>
    <DisplayLayout AllowDeleteDefault="Yes"
        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" Name="G" RowHeightDefault="20px" SelectTypeRowDefault="Single" StationaryMargins="Header"
        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy" HeaderClickActionDefault="SortMulti" CellClickActionDefault="NotSet">
        <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
        </FrameStyle>
        <Pager MinimumPagesForDisplay="2">
            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </PagerStyle>
        </Pager>
        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
        </EditCellStyleDefault>
        <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </FooterStyleDefault>
        <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </HeaderStyleDefault>
        <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
            <Padding Left="3px" />
            <BorderDetails ColorLeft="Window" ColorTop="Window" />
        </RowStyleDefault>
        <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
        </GroupByRowStyleDefault>
        <GroupByBox>
            <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
            </BoxStyle>
        </GroupByBox>
        <AddNewBox Hidden="False">
            <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </BoxStyle>
        </AddNewBox>
        <ActivationObject BorderColor="" BorderWidth="">
        </ActivationObject>
        <FilterOptionsDefault FilterUIType="HeaderIcons">
            <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px" Height="300px" Width="200px">
                <Padding Left="2px" />
            </FilterDropDownStyle>
            <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
            </FilterHighlightRowStyle>
            <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px">
                <Padding Left="2px" />
            </FilterOperandDropDownStyle>
        </FilterOptionsDefault>
    </DisplayLayout>
</igtbl:UltraWebGrid></td>
            <td class="right">
            </td>
        </tr>
        <tr>
            <td class="bottomleft">
            </td>
            <td class="bottom">
            </td>
            <td <%--style="font-style:italic"--%> class="bottomright">
            </td>
        </tr>
    </table>
            </td>
        </tr>
    </table>
</asp:Content>

