<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports.Master" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.ST_001_YANAO._default" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>
<%@ Register Src="../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>    

<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register src="../../Components/ReportExcelExporter.ascx" tagname="ReportExcelExporter" tagprefix="uc4" %>
<%@ Register src="../../Components/ReportPDFExporter.ascx" tagname="ReportPDFExporter" tagprefix="uc5" %>
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

<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                    &nbsp;
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Анализ средних розничных цен на нефтепродукты в разрезе муниципальных образований</asp:Label><br />
                    <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle">Данные ежегодного и ежемесячного мониторинга качества атмосферного воздуха в городах ХМАО-Югры (по состоянию на выбранную дату)</asp:Label>
                    <br />
                </td>
                <td style="margin-left: 40px">
                    <uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                    <uc5:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboCurDay" runat="server" Title="Выберите дату" 
                        Width="150" />
                </td>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="ComboCustomers" runat="server" Title="Товар" 
                        Visible="true" Width="150" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                </td>
                
                <td align="right" style="width: 100%" valign="top">
                    <div style="visibility:hidden">
             <iggauge:ultragauge ID="UltraGauge1" runat="server" BackColor="Transparent" BorderColor="Transparent"
                                ForeColor="ActiveCaptionText" Height="20px"
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
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                            </iggauge:ultragauge></div></td>
             <td style="width: 100%" align="right" valign="top">
                <uc2:GridSearch ID="GridSearch1" runat="server" />
            </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td colspan="2">
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
                                            <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" 
                                                Text="Раскрытие пообъектно" />
                                            <igtbl:UltraWebGrid
                                    ID="Grid" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                                                <Bands>
                                                    <igtbl:UltraGridBand>
                                                        <AddNewRow View="NotSet" Visible="NotSet">
                                                        </AddNewRow>
                                                    </igtbl:UltraGridBand>
                                                </Bands>
                                                <DisplayLayout Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy" SortCaseSensitiveDefault="False" SortingAlgorithmDefault="NotSet" HeaderClickActionDefault="NotSet">
                                                    <GroupByBox>
                                                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                                                        </BoxStyle>
                                                    </GroupByBox>
                                                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                                                    </GroupByRowStyleDefault>
                                                    <ActivationObject BorderColor="" BorderWidth="">
                                                    </ActivationObject>
                                                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                                    </FooterStyleDefault>
                                                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                                                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                                                        <Padding Left="3px" />
                                                    </RowStyleDefault>
                                                    <FilterOptionsDefault>
                                                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                                                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                Font-Size="11px">
                                                            <Padding Left="2px" />
                                                        </FilterOperandDropDownStyle>
                                                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                        </FilterHighlightRowStyle>
                                                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                Font-Size="11px" Height="300px" Width="200px">
                                                            <Padding Left="2px" />
                                                        </FilterDropDownStyle>
                                                    </FilterOptionsDefault>
                                                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                                    </HeaderStyleDefault>
                                                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                                    </EditCellStyleDefault>
                                                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                                                    </FrameStyle>
                                                    <Pager MinimumPagesForDisplay="2">
                                                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                                        </PagerStyle>
                                                    </Pager>
                                                    <AddNewBox Hidden="False">
                                                        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
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
                </td>
            </tr>
            </table>
            
        </div> 


 </asp:Content>


