<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0001_wm1.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FK_0001_0001_wm1" %>

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
<body bgcolor="black">
    <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName=""  
         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
        Version="8.2" Width="310px" Height="336px" EnableViewState="False" JavaScriptEnabled="False">
        <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False"
            Font-Bold="False"></Tooltips>
        <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
        </ColorModel>
        <Effects>
            <Effects>
                <igchartprop:GradientEffect>
                </igchartprop:GradientEffect>
            </Effects>
        </Effects>
        <Axis>
            <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                    Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
                <StripLines Interval="1">
                </StripLines>
            </Z>
            <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                    HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
                <StripLines Interval="1">
                </StripLines>
            </Y2>
            <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                    HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
                <StripLines Interval="1">
                </StripLines>
            </X>
            <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                    HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto" Padding="0">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Y>
            <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                    HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
                <StripLines Interval="1">
                </StripLines>
            </X2>
            <PE ElementType="None" Fill="Cornsilk"></PE>
            <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                </MinorGridLines>
                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                </MajorGridLines>
                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                    Font="Verdana, 7pt" VerticalAlign="Center">
                    <Layout Behavior="Auto" Padding="0">
                    </Layout>
                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                        Font="Verdana, 7pt" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
                <StripLines Interval="1">
                </StripLines>
            </Z2>
        </Axis>
        <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_fk01_01_#SEQNUM(100).png" />
        <Legend>
            <Margins Bottom="0" Left="0" Right="0" Top="0" />
        </Legend>
        <Annotations Visible="False">
        </Annotations>
        <TitleLeft Extent="33" Location="Left" Visible="True">
            <Margins Bottom="0" Left="0" Right="0" Top="0" />
        </TitleLeft>
        <TitleRight Extent="33" Location="Right" Visible="True">
            <Margins Bottom="0" Left="0" Right="0" Top="0" />
        </TitleRight>
        <TitleTop Visible="False">
            <Margins Bottom="0" Left="0" Right="0" Top="0" />
        </TitleTop>
        <TitleBottom Visible="False" Extent="33" Location="Bottom">
            <Margins Bottom="0" Left="0" Right="0" Top="0" />
        </TitleBottom>
    </igchart:UltraChart>
    <div id="adminPanel" style="display: none;">
        <form id="form1" runat="server">
            <div>
            </div>
        </form>
    </div>
</body>
</html>
