<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EO_0002_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPad.EO_0002_0002" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 1050; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible;">
        <table class="InformationText">
            <tr>
                <td>
                    <table>
                    <tr><td colspan='2'> <span class='DigitsValue'>Программа:</span>&nbsp;ДЦП «СОДЕЙСТВИЕ ЗАНЯТОСТИ НАСЕЛЕНИЯ
                                НА 2011 - 2013 ГОДЫ»</td></tr>
                        <tr>
                            <td>
                               
                                <span class='DigitsValue'>Основание для разработки:</span>&nbsp;Постановление
                                Правительства автономного округа от 9 октября 2010 года №246-п, Постановление Правительства
                                автономного округа от 14 апреля 2011 года №125-п<br />
                                <span class='DigitsValue'>Срок реализации:</span>&nbsp;2011-2013 годы<br />
                                <span class='DigitsValue'>Контроль за исполнением:</span>&nbsp;Департамент труда
                                и занятости населения ХМAО - Югры<br />
                                <span class='DigitsValue'>Цели программы:</span>&nbsp;Сдерживание роста безработицы
                                и снижение напряженности на рынке труда ХМАО – Югры<br />
                            </td>
                            <td>
                                <asp:Label ID="Label2" runat="server" CssClass="InformationText"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <uc1:iPadElementHeader ID="IPadElementHeader10" runat="server" Text="Финансирование программы по состоянию на {0:dd.MM.yyy}г., млн.руб."
                        Width="760px" />
                    <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart"
                        Version="8.2" Width="308px">
                        <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                    <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" Height="20px"
                        Visible="false" Width="200px" ForeColor="White">
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
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/1.png" />
                    </igGauge:UltraGauge>
                    <igtbl:UltraWebGrid ID="GRBSGridBrick" runat="server" Height="200px" Width="509px"
                        SkinID="UltraWebGrid">
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
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Задачи и мероприятия программы"
                        Width="760px" />
                    <span class='DigitsValue'>Задача 1:</span>&nbsp;Содействие занятости населения
                    <table>
                        <tr>
                            <td>
                                <igchart:UltraChart ID="UltraChart11" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td>
                                <igchart:UltraChart ID="UltraChart12" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td>
                                <igchart:UltraChart ID="UltraChart13" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td>
                                <igchart:UltraChart ID="UltraChart14" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                    <span class='DigitsValue'>Задача 2:</span>&nbsp;Социальная адаптация и трудовая
                    занятость молодежи
                    <table>
                        <tr>
                            <td>
                                <igchart:UltraChart ID="UltraChart21" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td>
                                <igchart:UltraChart ID="UltraChart22" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                    <span class='DigitsValue'>Задача 3:</span>&nbsp;Содействие трудоустройству инвалидов
                    <table>
                        <tr>
                            <td>
                                <igchart:UltraChart ID="UltraChart31" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                    <span class='DigitsValue'>Задача 4:</span>&nbsp;Повышение качества и конкурентоспособности
                    рабочей силы на рынке труда
                    <table>
                        <tr>
                            <td>
                                <igchart:UltraChart ID="UltraChart41" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
