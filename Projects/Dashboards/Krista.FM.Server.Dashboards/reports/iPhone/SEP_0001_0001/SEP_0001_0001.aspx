<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SEP_0001_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPad.SEP_0001_0001" %>


<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"> 
    <title>Untitled Page</title>
    <style type="text/css"> 
		.TextWhite
		{
			font-size: 14px;
			color: white;
			font-family: Arial;
		}
		.TextXXLarge
		{
			font-size: 22px;
			font-weight: bold;			
		}
		.TextXLarge
		{
			font-size: 18px;
			font-weight: bold;
		}
		.TextLarge
		{
			font-size: 16px;
		}
		.TextNormal
		{
			font-size: 14px;
		}
		.ShadowGreen
		{			
			text-shadow:0px 0px 2px black, 
						0px 0px 2px black, 
						1px 0px 3px rgb(50, 255, 50), 
						-1px 0px 3px rgb(50, 255, 50), 
						0px 1px 3px rgb(50, 255, 50), 
						0px -1px 3px rgb(50, 255, 50), 
						0px 0px 5px rgb(0, 255, 0),
						0px 0px 7px rgb(0, 200, 0);
		}
		.ShadowRed
		{
			text-shadow:0px 0px 2px black, 
						0px 0px 2px black, 
						1px 0px 3px rgb(255, 50, 50), 
						-1px 0px 3px rgb(255, 50, 50), 
						0px 1px 3px rgb(255, 50, 50), 
						0px -1px 3px rgb(255, 50, 50), 
						0px 0px 5px rgb(255, 0, 0),
						0px 0px 7px rgb(200, 0, 0);
		}
		.ShadowYellow
		{
			text-shadow:0px 0px 2px black, 
						0px 0px 2px black, 
						1px 0px 3px rgb(255, 255, 50), 
						-1px 0px 3px rgb(255, 255, 50), 
						0px 1px 3px rgb(255, 255, 50), 
						0px -1px 3px rgb(255, 255, 50), 
						0px 0px 5px rgb(255,255, 0),
						0px 0px 7px rgb(200, 255, 0);
		}
	</style>
</head> 
<body style="background-color: black;">
    <form id="form1" runat="server">
    
    <div style="position: absolute; width: 768px; height: 1050; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible;">
        <table> 
            <tr>
                <td> 
                    <div id="HeraldImageContainer" runat="server" style="float: left; margin-left: 32px" />
                </td>
                <td>
                    <asp:Label ID="lbInfo" runat="server" SkinID="InformationText"></asp:Label>
                </td> 
            </tr>
        </table> 
       
       <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr> 
                    <td align="left" valign="top">
                         <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Распределение муниципальных образований по итогам комплексной оценки социально-экономического развития"
                            Width="100%" />
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
                                                  <igGaugeProp:LinearGaugeBarMarker BulbSpan="10" InnerExtent="20" 
                                                      OuterExtent="80" PrecisionString="0" SegmentSpan="99" ValueString="40">
                                                      <Background>
                                                          <BrushElements>
                                                              <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                                  StartColor="64, 64, 64" />
                                                          </BrushElements>
                                                      </Background>
                                                      <StrokeElement>
                                                          <BrushElements>
                                                              <igGaugeProp:SimpleGradientBrushElement EndColor="Transparent" 
                                                                  GradientStyle="Horizontal" StartColor="64, 64, 64" />
                                                          </BrushElements>
                                                      </StrokeElement>
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
                                                  <igGaugeProp:LinearGaugeRange EndValueString="100" InnerExtent="20" 
                                                      OuterExtent="80" StartValueString="0">
                                                      <BrushElements>
                                                          <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                              StartColor="64, 64, 64" />
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
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/1.png" />
                    </igGauge:UltraGauge>
                    <asp:Table ID="detailTable" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid"
                        BorderWidth="1px" GridLines="Both" Width="740px">
                    </asp:Table>
                 
                    </td> 
                </tr>
            </table> 
        
            <table style=" margin-left:150px">
            <tr>
                <td>
                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText" 
                       CssClass="TextXXLarge"></asp:Label>
                </td> 
            </tr>
        </table>
            <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr> 
                    <td align="left" valign="top">
                       
                            <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Сводная оценка социально-экономического развития"
                            Width="100%" />
                             <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" 
                            Width="325px" SkinID="UltraWebGrid">
<DisplayLayout Name="UltraWebGrid1" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" 
                                AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" 
                                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" 
                                HeaderClickActionDefault="SortMulti" RowHeightDefault="20px" 
                                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" 
                                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" 
                                TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
        BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
        Font-Size="8.25pt" Height="200px" Width="325px">
    </FrameStyle>
    <Pager MinimumPagesForDisplay="2">
        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
        </PagerStyle>
    </Pager>
    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
    </EditCellStyleDefault>
    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </FooterStyleDefault>
    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
        HorizontalAlign="Left">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </HeaderStyleDefault>
    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
            BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                WidthTop="1px" />
        </BoxStyle> 
    </AddNewBox>
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
    <FilterOptionsDefault>
        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
            BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
            Width="200px">
            <Padding Left="2px" />
        </FilterDropDownStyle>
        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
        </FilterHighlightRowStyle>
        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
            BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
            <Padding Left="2px" />
        </FilterOperandDropDownStyle>
    </FilterOptionsDefault>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow> 
</igtbl:UltraGridBand>
</Bands>
                        </igtbl:UltraWebGrid>
                        
                    </td>
                </tr>
            </table>
            <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td align="left" valign="top">
                       
                            <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Интегральная оценка населением ситуации в ключевых сферах<br>деятельности органов местного самоуправления"
                            Width="100%" />
                             <igtbl:UltraWebGrid ID="UltraWebGrid3" runat="server" Height="200px" 
                            Width="325px" SkinID="UltraWebGrid">
<DisplayLayout Name="UltraWebGrid1" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" 
                                AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" 
                                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" 
                                HeaderClickActionDefault="SortMulti" RowHeightDefault="20px" 
                                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" 
                                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" 
                                TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
        BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
        Font-Size="8.25pt" Height="200px" Width="325px">
    </FrameStyle>
    <Pager MinimumPagesForDisplay="2">
        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
        </PagerStyle>
    </Pager>
    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
    </EditCellStyleDefault>
    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </FooterStyleDefault>
    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
        HorizontalAlign="Left">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </HeaderStyleDefault>
    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
            BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                WidthTop="1px" />
        </BoxStyle>
    </AddNewBox>
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
    <FilterOptionsDefault>
        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
            BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
            Width="200px">
            <Padding Left="2px" />
        </FilterDropDownStyle>
        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
        </FilterHighlightRowStyle>
        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
            BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
            <Padding Left="2px" />
        </FilterOperandDropDownStyle>
    </FilterOptionsDefault>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>
                        </igtbl:UltraWebGrid>
                          
                    </td>
                </tr>
            </table>

             <table style="width: 768px; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td align="left" valign="top" style="float: center; " width="740px" colspan="2">
                       
            <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Исходные показатели по сферам"
                            Width="740px" />
                            </td>
                            </tr>
                            <tr>
                            <td valign="top" align="left" >
                            <div style="float: center; " id="detalizationIconDiv" runat="server">
                            </div>
                                            
                            </td>
                            <td ><asp:Label ID="lbUrl" runat="server" SkinID="InformationText" Width="550px"></asp:Label></td>
                            </tr>
                            
                            </table>
        <div style="margin-top: 100px">
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </div>
    </div>
    </form>
</body>
</html>
