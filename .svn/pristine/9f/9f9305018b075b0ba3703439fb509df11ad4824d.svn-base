<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UFK_0008_0001_H.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.UFK_0008_0001_H" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>  

    <table style="left: 0px; position: absolute; top: 0px; border-collapse: collapse;">      
            <tr><td style="width: 315px; height: 360px; border-right: lime 0px solid; border-top: lime 0px solid; border-left: lime 0px solid; border-bottom: lime 0px solid; position: relative; background-color: black; left: 0px; top: 0px; padding-left: 3px; border-collapse: collapse;" align="left" valign="top" colspan="" rowspan="">
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="292px" OnDataBinding="Grid_DataBinding"
                    OnInitializeLayout="Grid_InitializeLayout" OnInitializeRow="Grid_InitializeRow"
                    SkinID="UltraWebGrid" Width="475px">
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                        HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" RowHeightDefault="20px"
                        RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="292px"
                            Width="475px">
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
                <asp:TextBox ID="TextBox1" runat="server" CssClass="ServeText" ReadOnly="True" SkinID="TextBoxServeText"
                    Width="470px"></asp:TextBox><br />
                <asp:TextBox ID="TextBox2" runat="server" SkinID="TextBoxServeText" Width="470px" CssClass="ServeText" ReadOnly="True"></asp:TextBox><br />
                <asp:TextBox ID="TextBox3" runat="server" SkinID="TextBoxServeText" Width="470px" CssClass="ServeText" ReadOnly="True"></asp:TextBox><br />
                <asp:TextBox ID="TextBox4" runat="server" SkinID="TextBoxServeText" Width="470px" CssClass="ServeText" ReadOnly="True"></asp:TextBox><br />
                <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    Version="8.2" Width="470px" BackColor="Black" SkinID="UltraWebColumnChart">
<Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False"></Tooltips>

<ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="30" LineColor="209, 209, 209">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="209, 209, 209" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="209, 209, 209" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" OrientationAngle="45" Visible="False">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Custom" FontColor="209, 209, 209" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center" OrientationAngle="45">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart" Extent="60" LineColor="209, 209, 209">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="209, 209, 209" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt; млн." FontColor="209, 209, 209" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="209, 209, 209" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>
                    <Legend AlphaLevel="50" BackgroundColor="DimGray" BorderColor="Transparent" FontColor="209, 209, 209"
                        Location="Top" Visible="True">
                        <Margins Bottom="40" Left="15" Top="0" Right="0" />
                    </Legend>
                    <TitleTop Font="Arial, 12.75pt, style=Bold" FontColor="209, 209, 209" VerticalAlign="Far">
                        <Margins Bottom="-55" Left="0" Right="0" Top="0" />
                    </TitleTop>
                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_mfrf01_04_#SEQNUM(100).png" />
</igchart:UltraChart>

                <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName=""  
                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    Version="8.2" Width="470px" SkinID="UltraWebColumnChart" Height="255px">
<Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False"></Tooltips>

<ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>
                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_mfrf01_04_#SEQNUM(100).png" />
</igchart:UltraChart>

                <igchart:UltraChart ID="UltraChart3" runat="server" BackgroundImageFileName=""  
                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    Version="8.2" Width="470px" SkinID="UltraWebColumnChart" Height="255px">
<Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False"></Tooltips>

<ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>
                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_mfrf01_04_#SEQNUM(100).png" />
</igchart:UltraChart>
            </td></tr>
                 <tr><td style="width: 315px; border-right: 0px solid; border-top: 0px solid; background-image: url(../../../images/servePane.gif); border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 25px; padding-left: 5px; left: 0px; margin-left: 0px; position: static; top: 408px;" align="left" valign="top" colspan="" rowspan="">
                
                     <asp:Table ID="Table1" runat="server">
                         <asp:TableRow runat="server">
                             <asp:TableCell runat="server"><asp:Label ID="Label7" runat="server" Font-Names="Arial" Font-Size="10px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label><br />
                <asp:Label ID="Label6" runat="server" Font-Names="Arial" Font-Size="10px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label></asp:TableCell>
                         </asp:TableRow>
                     </asp:Table>
                 </td></tr> 

        </table>
    </div>
    </form>
</body>
</html>
