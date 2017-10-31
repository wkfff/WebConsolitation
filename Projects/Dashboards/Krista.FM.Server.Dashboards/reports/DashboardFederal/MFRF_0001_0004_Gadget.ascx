<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MFRF_0001_0004_Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.MFRF_0001_0004_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<div class="GadgetTopDate"><asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></div>
<asp:Panel ID="MainPanel" runat="server" Height="100%" Width="100%">
<igchart:UltraChart ID="ultraChart" runat="server" EmptyChartText="" ChartType="CylinderStackColumnChart3D" BackgroundImageFileName="" Version="7.3" BackgroundImageStyle="Centered" OnDataBinding="ultraChart_DataBinding" Height="300px" Width="480px" >
  <TitleTop Orientation="VerticalLeftFacing">
    <Margins Bottom="0" Left="0" Right="0" Top="0" />
  </TitleTop>
  <Legend SpanPercentage="28" Visible="True" Location="Bottom" Font="Microsoft Sans Serif, 6.5pt" >
      <Margins Bottom="0" Left="0" Right="0" Top="0" />
  </Legend>
  <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear" />
  <Data>
    <EmptyStyle Text="Пустое" />
  </Data>
  <TitleLeft Extent="33" HorizontalAlign="Center" Visible="True" Text="Тыс. руб." Location="Left">
    <Margins Bottom="0" Left="0" Right="0" Top="0" />
  </TitleLeft>
  <Axis>
    <Z Visible="True" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" HorizontalAlign="Far" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="DimGray">
        <SeriesLabels Orientation="Horizontal" HorizontalAlign="Far" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="DimGray" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </Z>
    <Y2 Visible="False" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Near" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray">
        <SeriesLabels Orientation="Horizontal" FormatString="" HorizontalAlign="Near" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </Y2>
    <X Visible="True" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Custom" ItemFormatString="" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center" FontColor="DimGray">
        <SeriesLabels Orientation="Horizontal" FormatString="" HorizontalAlign="Near" VerticalAlign="Near" Font="Verdana, 7pt" FontColor="DimGray" Flip="True" OrientationAngle="168" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </X>
    <Y TickmarkInterval="1" Visible="True" TickmarkStyle="Smart" LineThickness="1">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center" FontColor="DimGray">
        <SeriesLabels Orientation="Horizontal" FormatString="" HorizontalAlign="Far" VerticalAlign="Near" Font="Verdana, 7pt" FontColor="DimGray" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </Y>
    <X2 Visible="False" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Custom" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Center" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray" OrientationAngle="-25">
        <SeriesLabels Orientation="Horizontal" FormatString="" HorizontalAlign="Far" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </X2>
    <Z2 Visible="False" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart">
      <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" AlphaLevel="255" Thickness="1" />
      <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" AlphaLevel="255" Thickness="1" />
      <Labels Orientation="Horizontal" ItemFormatString="" HorizontalAlign="Near" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray">
        <SeriesLabels Orientation="Horizontal" HorizontalAlign="Near" VerticalAlign="Center" Font="Verdana, 7pt" FontColor="Gray" >
            <Layout Behavior="Auto">
            </Layout>
        </SeriesLabels>
          <Layout Behavior="Auto">
          </Layout>
      </Labels>
    </Z2>
      <PE ElementType="None" Fill="Cornsilk" />
  </Axis>
  <Tooltips FormatString="&lt;ITEM_LABEL&gt; (&lt;DATA_VALUE:#,##0&gt; тыс.руб.)" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"  />
    <Effects>
        <Effects>
            <igchartprop:GradientEffect>
            </igchartprop:GradientEffect>
        </Effects>
    </Effects>
            <TitleBottom Extent="33" Location="Bottom">
                <Margins Bottom="0" Left="0" Right="0" Top="0" />
            </TitleBottom>
            <Border Thickness="0" />
    <DeploymentScenario FilePath="./TemporaryImages" ImageURL="./TemporaryImages/Chart_MFRF0104Gadget_#SEQNUM(100).png" />
</igchart:UltraChart>
</asp:Panel>
