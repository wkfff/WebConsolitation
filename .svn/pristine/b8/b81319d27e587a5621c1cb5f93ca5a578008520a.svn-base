<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0042_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0042_0001" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="iPadElementHeader" Src="~/Components/iPadElementHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 767px; height: 950px; top: 0px; left: 0px;
        overflow: hidden; z-index: 2;">
        <table style="width: 765; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>            
                <td align="left" valign="top">
                    <div runat="server" id="HeraldImageContainer" style="float: left; margin-left: 32px">
                    </div>
                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText"></asp:Label>
                </td>
                <td>
                    <a href='webcommand?showPinchReport=FO_0042_0003'>
                        <img src='../../../images/detail.png'/></a>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table style="border-collapse: collapse">
                        <tr>
                            <td valign="top">
                                <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Среднее значение взвешенной оценки по группам показателей"
                                    Width="430px" />
                                <div style="height: 7px; clear: both">
                                </div>
                                <asp:Table ID="detailTable" runat="server" BackColor="Black" BorderColor="#323232"
                                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both" Width="430px">
                                </asp:Table>
                            </td>
                            <td valign="top" align="center" wigth="300px" style="width: 300px">
                                <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Среднее значение взвешенной оценки по группам показателей"
                                    Width="323px" />
                                <asp:Label ID="lbAvgValue" runat="server" SkinID="DigitsValueLarge"></asp:Label>
                                <div style="margin-left: 3px; margin-top: 1px">
                                    <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="Transparent" Height="257px"
                                        Width="120px">
                                        <Gauges>
                                            <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                                                <Scales>
                                                    <igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="20">
                                                        <MajorTickmarks StartWidth="2" EndWidth="2" StartExtent="40" EndExtent="60">
                                                            <StrokeElement Color="DimGray">
                                                            </StrokeElement>
                                                            <BrushElements>
                                                                <igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
                                                            </BrushElements>
                                                        </MajorTickmarks>
                                                        <MinorTickmarks Frequency="0.2" StartExtent="45" EndExtent="55">
                                                            <StrokeElement Color="Black">
                                                            </StrokeElement>
                                                        </MinorTickmarks>
                                                        <Labels ZPosition="AboveMarkers" Extent="25" Font="Trebuchet MS, 9pt">
                                                            <Shadow Depth="2">
                                                                <BrushElements>
                                                                    <igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
                                                                </BrushElements>
                                                            </Shadow>
                                                            <BrushElements>
                                                                <igGaugeProp:SolidFillBrushElement Color="Black"></igGaugeProp:SolidFillBrushElement>
                                                            </BrushElements>
                                                        </Labels>
                                                        <Markers>
                                                            <igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" StartExtent="-20" BulbSpan="30"
                                                                ValueString="58" PrecisionString="0">
                                                                <BrushElements>
                                                                    <igGaugeProp:SimpleGradientBrushElement GradientStyle="Horizontal" StartColor="200, 255, 0, 0"
                                                                        EndColor="200, 178, 34, 34"></igGaugeProp:SimpleGradientBrushElement>
                                                                </BrushElements>
                                                            </igGaugeProp:LinearGaugeBarMarker>
                                                        </Markers>
                                                        <Axes>
                                                            <igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="20"></igGaugeProp:NumericAxis>
                                                        </Axes>
                                                    </igGaugeProp:LinearGaugeScale>
                                                    <igGaugeProp:LinearGaugeScale InnerExtent="5" OuterExtent="95" EndExtent="98" StartExtent="60">
                                                        <BrushElements>
                                                            <igGaugeProp:SimpleGradientBrushElement StartColor="150, 255, 255, 255" EndColor="50, 255, 255, 255">
                                                            </igGaugeProp:SimpleGradientBrushElement>
                                                        </BrushElements>
                                                    </igGaugeProp:LinearGaugeScale>
                                                </Scales>
                                                <BrushElements>
                                                    <igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="239, 243, 246"
                                                        EndColor="182, 215, 230"></igGaugeProp:SimpleGradientBrushElement>
                                                </BrushElements>
                                                <StrokeElement Thickness="5">
                                                    <BrushElements>
                                                        <igGaugeProp:SimpleGradientBrushElement StartColor="182, 215, 230" EndColor="147, 194, 217">
                                                        </igGaugeProp:SimpleGradientBrushElement>
                                                    </BrushElements>
                                                </StrokeElement>
                                            </igGaugeProp:LinearGauge>
                                        </Gauges>
                                    </igGauge:UltraGauge>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <div style="height: 7px; clear: both">
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" valign="top" colspan="2">
                    <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Максимальные итоговые оценки по 5 ГРБС автономного округа&nbsp;<img src='../../../images/StarYellow.png'>"
                        Width="756px" />
                    <igtbl:UltraWebGrid ID="BestGRBSGrid" runat="server" Height="200px" Width="509px"
                        OnDataBinding="GRBSGrid_DataBinding" OnInitializeLayout="GRBSGrid_InitializeLayout"
                        SkinID="UltraWebGrid" OnInitializeRow="GRBSGrid_InitializeRow">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                            StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                            StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
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
                                Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
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
            </tr>
            <tr>
                <td align="left" valign="top" colspan="2">
                    <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Минимальные итоговые оценки по 5 ГРБС автономного округа&nbsp;<img src='../../../images/StarGray.png'>"
                        Width="756px" />
                    <igtbl:UltraWebGrid ID="WorseGRBSGrid" runat="server" Height="200px" Width="509px"
                        OnDataBinding="GRBSGrid_DataBinding" OnInitializeLayout="GRBSGrid_InitializeLayout"
                        SkinID="UltraWebGrid" OnInitializeRow="GRBSGrid_InitializeRow">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                            StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                            StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
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
                                Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
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
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
