<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FW_0003_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FW_0003_0001" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: black;">
    <form id="form1" runat="server">
     <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div id="mainDiv" runat="server" style="position: absolute; width: 767px; top: 0px;
        left: 0px; overflow: hidden; z-index: 2; background-position: left top; background-repeat: no-repeat">
        <table style="border-collapse: collapse">
            <tr>
                <td>
                    <table style="width: 765px; margin-bottom: -20px">
                        <tr>
                            <td class="InformationText" style="width: 100%;">
                                <asp:Label ID="lbDescription" runat="server" Text="Label"></asp:Label>
                                <table style="width: 100%; border-collapse: collapse;" class="DigitsValueLarge">
                                    <tr>
                                        <td style="width: 30%">
                                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="width: 30%">
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="width: 30%">
                                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <igtbl:UltraWebGrid ID="WebGrid1" runat="server" Height="200px" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never"
                            SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                            TableLayout="Fixed" Version="4.00" BorderCollapseDefault="Separate" GridLinesDefault="None">
                            <GroupByBox Hidden="True">
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
                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                Font-Bold="False" Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right"
                                Wrap="True">
                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
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
                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid"
                                BorderWidth="1px" Font-Bold="True" Font-Names="Arial" Font-Size="18px" ForeColor="White"
                                HorizontalAlign="Center" Wrap="True">
                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px" Width="315px">
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
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never"
                            SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                            TableLayout="Fixed" Version="4.00" BorderCollapseDefault="Separate" GridLinesDefault="None">
                            <GroupByBox Hidden="True">
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
                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                Font-Bold="False" Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right"
                                Wrap="True">
                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
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
                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid"
                                BorderWidth="1px" Font-Bold="True" Font-Names="Arial" Font-Size="18px" ForeColor="White"
                                HorizontalAlign="Center" Wrap="True">
                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px" Width="315px">
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
            </tr>
            <tr>
                <td>
                    <div style="margin-left: -40px; margin-top: -40px">
                        <DMWC:MapControl ID="DundasMap1" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                            ImageUrl="../../../TemporaryImages/map_fw_03_01_01#SEQ(300,3)" RenderingImageUrl="../../../TemporaryImages/"
                            RenderType="ImageTag">
                            <NavigationPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="90" Width="90"></Size>
                            </NavigationPanel>
                            <Viewport>
                                <Location X="0" Y="0"></Location>
                                <Size Height="100" Width="100"></Size>
                            </Viewport>
                            <ZoomPanel>
                                <Size Height="200" Width="40"></Size>
                                <Location X="0" Y="0"></Location>
                            </ZoomPanel>
                            <ColorSwatchPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="60" Width="350"></Size>
                            </ColorSwatchPanel>
                            <DistanceScalePanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="55" Width="130"></Size>
                            </DistanceScalePanel>
                        </DMWC:MapControl>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                 <uc3:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Среднее значение взвешенной оценки по группам показателей"
                                        Width="100%" />
                    <igchart:UltraChart ID="UltraChart1" runat="server" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                         Version="9.1" SkinID="UltraWebColumnChart">                        
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FW0301_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
            <tr>
                <td>
                 <uc3:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Среднее значение взвешенной оценки по группам показателей"
                                        Width="100%" />
                    <igchart:UltraChart ID="UltraChart2" runat="server" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                         Version="9.1" SkinID="UltraWebColumnChart">                        
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FW0301_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
