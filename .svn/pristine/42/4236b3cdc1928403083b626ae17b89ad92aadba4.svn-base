<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.EO_0001_0002.Default" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
				
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                <uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table style="vertical-align: top;">
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboPeriod" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboCustomers" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboTerritory" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
	<table>
		<tr>
			<td><asp:Label ID="DynamicText" runat="server"></asp:Label></td>
		</tr>
	</table>
    <table style="vertical-align: top;">
        <tr>
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
                            OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                            OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                SelectTypeRowDefault="Extended">
                                <GroupByBox>
                                    <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                    </BoxStyle>
                                </GroupByBox>
                                <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                </GroupByRowStyleDefault>
                                <ActivationObject BorderWidth="" BorderColor="">
                                </ActivationObject>
                                <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </FooterStyleDefault>
                                <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                    Font-Names="Microsoft Sans Serif" BackColor="Window">
                                    <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                    <Padding Left="3px"></Padding>
                                </RowStyleDefault>
                                <FilterOptionsDefault>
                                    <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                        Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                        CustomRules="overflow:auto;">
                                        <Padding Left="2px"></Padding>
                                    </FilterOperandDropDownStyle>
                                    <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                    </FilterHighlightRowStyle>
                                    <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                        Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                        Height="300px" CustomRules="overflow:auto;">
                                        <Padding Left="2px"></Padding>
                                    </FilterDropDownStyle>
                                </FilterOptionsDefault>
                                <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </HeaderStyleDefault>
                                <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                </EditCellStyleDefault>
                                <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                    Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                </FrameStyle>
                                <Pager MinimumPagesForDisplay="2">
                                    <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                        </BorderDetails>
                                    </PagerStyle>
                                </Pager>
                                <AddNewBox Hidden="False">
                                    <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                        </BorderDetails>
                                    </BoxStyle>
                                </AddNewBox>
                            </DisplayLayout>
                        </igtbl:UltraWebGrid>
                    </td>
                    <td class="right">
                    </td>
                </tr>
                <tr>
                    <td class="bottomleft">
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
                    <td class="bottom">
                    </td>
                    <td class="bottomright">
                    </td>
                </tr>
            </table>
        </tr>
    </table>
</asp:Content>
