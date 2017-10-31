<%@ Page Language="C#" Title="Сводный отчет об исполнении долгосрочных муниципальных целевых программ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default1.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP0.EO_0005_MP_001.Default" %>

<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
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
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top; width: 1262px">
    <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true" /><asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Label"></asp:Label><br />
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
            <td>
            <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse">
        <tr>
            <td colspan="" rowspan="">
<uc3:CustomMultiCombo ID="Month" runat="server" />
            </td>
            <td>
<uc3:CustomMultiCombo ID="Zakaz" runat="server" />
            </td>
            <td id="tab">
            <uc3:CustomMultiCombo ID="region" runat="server" Visible="true" />
            </td>
            <td>
    <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td style="width: 31px">
                &nbsp;<igGauge:UltraGauge ID="Ga1" runat="server" BackColor="Transparent" Height="20px"
                    Visible="False" Width="200px" BorderColor="Transparent" ForeColor="White">
                    <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/1.png" />
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
            </td>
        </tr>
    </table>
    <br />
    
    <table style="width: 100%; border-collapse: collapse; background-color: white; margin-top: 10px; height: auto;" id="TABLE1">
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
                <asp:CheckBox ID="Mer" runat="server" AutoPostBack="True" Text="Детализация по мероприятиям" /><asp:CheckBox
                    ID="KOSGU" runat="server" AutoPostBack="True" Text="Детализация по КОСГУ" /><br />
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" OnDataBinding="G1_DataBinding"
                    OnInitializeLayout="UltraWebGrid1_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow"
                    SkinID="UltraWebGrid" Style="text-align: right" StyleSetName="Office2007Blue">
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                    <DisplayLayout AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" AllowUpdateDefault="Yes"
                        BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
                        RowHeightDefault="20px" SelectTypeRowDefault="Single" StationaryMargins="Header"
                        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
            <td class="bottomright">
            </td>
        </tr>
    </table>
</asp:Content>
