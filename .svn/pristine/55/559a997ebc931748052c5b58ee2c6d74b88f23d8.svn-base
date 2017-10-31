<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FK_0001_0001_Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FK_0001_0001_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<div class="GadgetTopDate" style="width: 100%"><asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></div>
<asp:Panel ID="MainPanel" runat="server" Height="100%" Width="100%">
    <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
          Version="8.2" OnDataBinding="UltraChart_DataBinding">
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
            <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
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
            <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True">
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
        <Border Color="Transparent" />
        <DeploymentScenario FilePath="./TemporaryImages" ImageURL="./TemporaryImages/Chart_FK0101Gadget_#SEQNUM(100).png" />
    </igchart:UltraChart>
 <div style="text-align: right;">
    <asp:HyperLink ID="HyperLink1" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
    <asp:HyperLink ID="HyperLink2" runat="server" SkinID="HyperLink" ></asp:HyperLink>
 </div>   
</asp:Panel>