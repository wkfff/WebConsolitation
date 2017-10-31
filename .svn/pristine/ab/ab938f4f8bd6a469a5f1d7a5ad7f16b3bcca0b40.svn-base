<%@ Page Language="C#" AutoEventWireup="true" Codebehind="MFRF_0001_0005_wm1_V.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.MFRF_0001_0005_wm1_V" %>

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
    <asp:Label ID="LabelTitle" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
    <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName=""  
         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
        SkinID="UltraWebColumnChart" Version="8.2" Width="311px" OnFillSceneGraph="UltraChart_FillSceneGraph">
        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
            Font-Underline="False" />
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
                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Z>
            <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                    Visible="False" />
                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                    Visible="True" />
                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                    Orientation="Horizontal" VerticalAlign="Center">
                    <Layout Behavior="Auto">
                    </Layout>
                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Y2>
            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                    Visible="False" />
                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                    Visible="True" />
                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                    <Layout Behavior="Auto">
                    </Layout>
                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </X>
            <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True">
                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                    Visible="False" />
                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                    Visible="True" />
                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
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
            <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                    Visible="False" />
                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                    Visible="True" />
                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                    <Layout Behavior="Auto">
                    </Layout>
                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                        VerticalAlign="Center">
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
                    Orientation="Horizontal" VerticalAlign="Center">
                    <Layout Behavior="Auto">
                    </Layout>
                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Z2>
        </Axis>
        <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_v_#SEQNUM(100).png" />
    </igchart:UltraChart>
    <table>
        <tr>
            <td style="width: 220px; border-right: 0px solid; border-top: 0px solid; border-left: 0px solid;
                border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse;
                background-image: url(../../../images/servePane.gif);" align="left" valign="top"
                colspan="2">
                <asp:Label ID="Label1" runat="server" SkinID="ServeText" Text="Label"></asp:Label><br />
                <asp:Label ID="Label2" runat="server" SkinID="ServeText" Text="Label"></asp:Label></td>
        </tr>
    </table>
    <div id="adminPanel" style="display: none;">
        <form id="form1" runat="server">
            <div>
            </div>
        </form>
    </div>
</body>
</html>
