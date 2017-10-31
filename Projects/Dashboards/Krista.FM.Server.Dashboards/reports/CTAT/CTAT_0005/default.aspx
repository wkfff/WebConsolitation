<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0005._default" %>
<%@ Register Src="../../../components/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc4" %>

<%@ Register Src="../../../components/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc3" %>

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

<%@ Register Src="../../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="../../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>


<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
   
                <td>
                <igtbl:UltraWebGrid ID = "TG" runat =server OnDataBinding="TG_DataBinding" SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                        HeaderClickActionDefault="SortMulti" Name="TG" RowHeightDefault="20px" RowSelectorsDefault="No"
                        SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                        TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                <td rowspan="2" style="vertical-align: top">
                    <DMWC:MapControl ID="map" runat="server" BackColor="White" BackGradientType="TopBottom"
                        BackSecondaryColor="249, 249, 240" BorderLineColor="DarkOrange" BorderLineWidth="2"
                        GridUnderContent="True" Height="363px" ImageUrl="../../../TemporaryImages/MapPic_#SEQ(300,3)"
                        RenderingImageUrl="../../../TemporaryImages/" ResourceKey="#MapControlResKey#MapControl1#"
                        Width="679px">
                        <Frame FrameStyle="Emboss" />
                        <NavigationPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                            BackShadowOffset="0" BorderColor="185, 206, 102" ButtonBorderColor="88, 95, 41"
                            Dock="Right" SymbolBorderColor="88, 95, 41" Visible="True">
                            <Location X="84.01573" Y="3.87477636" />
                            <Size Height="90" Width="90" />
                        </NavigationPanel>
                        <Viewport AutoSize="False" BackColor="" BorderColor="Gray">
                            <Location X="1.95434761" Y="3.87477636" />
                            <Size Height="90.17028" Width="95.19531" />
                        </Viewport>
                        <Parallels LabelColor="88, 95, 41" LineColor="Silver" Visible="False" />
                        <ShapeRules>
                            <DMWC:ShapeRule Category="2" ColorCount="1" ColoringMode="ColorRange" FromColor="RoyalBlue"
                                MiddleColor="RoyalBlue" Name="ShapeRule1" ToColor="RoyalBlue">
                                <CustomColors>
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                </CustomColors>
                            </DMWC:ShapeRule>
                        </ShapeRules>
                        <ZoomPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                            BackShadowOffset="0" BorderColor="185, 206, 102" ButtonBorderColor="88, 95, 41"
                            Dock="Right" SliderBarColor="88, 95, 41" SnapToTickMarks="True" SymbolBorderColor="48, 48, 48"
                            ThumbBorderColor="88, 95, 41" TickBorderColor="88, 95, 41" TickCount="20" Visible="True">
                            <Size Height="200" Width="40" />
                            <Location X="91.39036" Y="28.7366543" />
                        </ZoomPanel>
                        <Meridians LabelColor="88, 95, 41" LineColor="Silver" Visible="False" />
                        <ColorSwatchPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                            BackShadowOffset="0" BorderColor="185, 206, 102" DockedInsideViewport="False"
                            LabelColor="88, 95, 41" TitleColor="88, 95, 41" Visible="True">
                            <Location X="1.84202874" Y="72.85341" />
                            <Size Height="80" Width="180" />
                        </ColorSwatchPanel>
                        <ShapeFields>
                            <DMWC:Field Name="ID" Type="System.Double" />
                        </ShapeFields>
                        <DistanceScalePanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                            BackShadowOffset="0" BorderColor="185, 206, 102" Dock="Right" LabelColor="88, 95, 41">
                            <Location X="78.11603" Y="79.10078" />
                            <Size Height="55" Width="130" />
                        </DistanceScalePanel>
                        <Labels>
                            <DMWC:MapLabel Dock="Bottom" DockAlignment="Near" Name="MapLabelMeasures" Text=""
                                TextAlignment="BottomLeft">
                                <Size Height="30" Width="100" />
                                <Location X="1.95434761" Y="64.18365" />
                            </DMWC:MapLabel>
                        </Labels>
                    </DMWC:MapControl>
                </td>
            </tr>
            <tr>
    
                <td>
                    <igchart:UltraChart ID="TC" runat="server" BackgroundImageFileName=""  
                         ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        OnDataBinding="TC_DataBinding" Version="9.1">
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" />
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                        <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
                        </ColorModel>
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect>
                                </igchartprop:GradientEffect>
                            </Effects>
                        </Effects>
                        <Axis>
                            <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Z>
                            <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y2>
                            <X LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y>
                            <X2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </X2>
                            <PE ElementType="None" Fill="Cornsilk" />
                            <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Z2>
                        </Axis>
                    </igchart:UltraChart>
                </td>
            </tr>
            <tr>
    
                <td colspan="2">
                    <igtbl:UltraWebGrid ID="BG" runat="server" Height="200px" OnDataBinding="BG_DataBinding"
                        SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Width="325px">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="G"
                            RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                            Version="4.00" ViewType="OutlookGroupBy">
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
                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                                Width="325px">
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
                    </igtbl:UltraWebGrid></td>
            </tr>
            <tr>
                <td colspan="2">
                    <igchart:UltraChart ID="BC" runat="server" BackgroundImageFileName=""  
                         ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        OnDataBinding="BC_DataBinding" Version="9.1">
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" />
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                        <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
                        </ColorModel>
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect>
                                </igchartprop:GradientEffect>
                            </Effects>
                        </Effects>
                        <Axis>
                            <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Z>
                            <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y2>
                            <X LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y>
                            <X2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </X2>
                            <PE ElementType="None" Fill="Cornsilk" />
                            <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Z2>
                        </Axis>
                    </igchart:UltraChart>
                </td>
            </tr>
        </table>
            
    </div>
</asp:Content>