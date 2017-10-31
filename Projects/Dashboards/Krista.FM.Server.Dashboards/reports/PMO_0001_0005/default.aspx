<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0005._default" %>

<%@ Register Src="../../components/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc4" %>

<%@ Register Src="../../components/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc4:HeaderPR ID="HeaderPR1" runat="server" />
        
        <table width="100%">
        
        <caption style="text-align:left">
            <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Verdana" Font-Size="Large"></asp:Label>
        </caption> 
               
        <tr><td style="width:10%">  
         
            <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2"
                Text="Обновить данные">
                <Appearance>
                    <style backcolor="White" bordercolor="Black" borderstyle="Solid" borderwidth="1px"></style>
                </Appearance>
                <PressedAppearance>
                    <style font-bold="False" font-italic="False" font-overline="False" font-strikeout="False"
                        font-underline="False"></style>
                </PressedAppearance>
                <DisabledAppearance>
                    <style bordercolor="Control"></style>
                </DisabledAppearance>
                <ClientSideEvents Click="SubmitButton_Click" />
                <HoverAppearance>
                    <style font-bold="False" font-italic="False" font-overline="False" font-strikeout="False"
                        font-underline="False"></style>
                </HoverAppearance>
                <FocusAppearance>
                    <style font-bold="False" font-italic="False" font-overline="False" font-strikeout="False"
                        font-underline="False"></style>
                </FocusAppearance>
            </igtxt:WebImageButton>
    
        </td><td style="width:90%" colspan=2>
        
            <igmisc:WebPanel ID="WebPanel1" runat="server" EnableAppStyling="True" Expanded="False"
                Font-Bold="True" Height="100%" StyleSetName="Office2007Blue" Width="100%">
                <Header Text="Панель параметров">
                </Header>
                <Template>
                    <uc3:UserComboBox ID="user_combo" runat="server" StuffID="Территория" />
                </Template>
            </igmisc:WebPanel>

        </td></tr><tr><td colspan=2>
        
            <asp:Label ID="grid_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" Width="90%"></asp:Label>
            
        </td><td colspan=1 style="vertical-align:top; width:51%">
        
            <asp:Label ID="chart_pie_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" Width="90%"></asp:Label>
            
        </td></tr><tr><td style="vertical-align:top" colspan=2>                    
            
            <igtbl:UltraWebGrid ID="grid" runat="server" EnableAppStyling="True" Height="100%"
                StyleSetName="Office2007Blue" Width="100%" OnDataBinding="grid_DataBinding" OnInitializeLayout="grid_InitializeLayout" OnActiveRowChange="grid_ActiveRowChange" OnDblClick="grid_DblClick"><Bands>
                <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
                </igtbl:UltraGridBand>
                </Bands>

                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="grid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended" NoDataMessage="данных не найдено">
                <GroupByBox Prompt="Перетащите сюда колонку для группировки" Hidden="True">
                <BoxStyle BorderColor="Window" BackColor="ActiveBorder"></BoxStyle>
                </GroupByBox>

                <GroupByRowStyleDefault BorderColor="Window" BackColor="Control"></GroupByRowStyleDefault>

                <ActivationObject BorderWidth="" BorderColor=""></ActivationObject>

                <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </FooterStyleDefault>

                <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window">
                <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>

                <Padding Left="3px"></Padding>
                </RowStyleDefault>

                <FilterOptionsDefault>
                <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                <Padding Left="2px"></Padding>
                </FilterOperandDropDownStyle>

                <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55"></FilterHighlightRowStyle>

                <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                <Padding Left="2px"></Padding>
                </FilterDropDownStyle>
                </FilterOptionsDefault>

                <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </HeaderStyleDefault>

                <EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

                <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="None" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="100%" Height="100%"></FrameStyle>

                <Pager MinimumPagesForDisplay="2">
                <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </PagerStyle>
                </Pager>

                <AddNewBox Hidden="False">
                <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </BoxStyle>
                </AddNewBox>
                </DisplayLayout>
            </igtbl:UltraWebGrid>
        
        </td><td style="vertical-align:top" colspan=1>                   
        
            <igchart:UltraChart ID="chart_pie" runat="server" BackgroundImageFileName="" BorderColor="InactiveCaption"
                BorderWidth="0px" EmptyChartText="данных не найдено"
                Version="8.2" ChartType="PieChart" OnDataBinding="chart_pie_DataBinding" OnInvalidDataReceived="chart_pie_InvalidDataReceived">

                <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink" Scaling="Random"></ColorModel>

                <Effects><Effects>
                <igchartprop:GradientEffect></igchartprop:GradientEffect>
                </Effects>
                </Effects>

                <Axis>
                <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" Visible="False">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </Z>

                <Y2 LineThickness="1" TickmarkInterval="5" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" Visible="False" ItemFormatString="">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </Y2>

                <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </X>

                <Y LineThickness="1" TickmarkInterval="5" Visible="True" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </Y>

                <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" Visible="False" ItemFormatString="">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </X2>

                <PE ElementType="None" Fill="Cornsilk"></PE>

                <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" Visible="False">
                <Layout Behavior="Auto"></Layout>

                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                <Layout Behavior="Auto"></Layout>
                </SeriesLabels>
                </Labels>
                </Z2>
                </Axis>
                <Legend Visible="True"></Legend>
                <Border Color="InactiveCaption" Thickness="0" />
                <PieChart OthersCategoryPercent="2" OthersCategoryText="Прочие">
                </PieChart>
            </igchart:UltraChart>
                
        </td></tr><tr><td colspan=2 style="width:50%">
        
            <asp:Label ID="chart_avg_count_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" Width="90%"></asp:Label>                        
            
        </td><td style="width:50%">
            
            <asp:Label ID="chart_avg_wage_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" Width="90%"></asp:Label>            
            
        </td></tr><tr><td colspan=2>                            

            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="100%" Width="100%" RefreshTargetIDs="chart_avg_count_title,WebAsyncRefreshPanel2" TriggerControlIDs="grid">
            <igchart:UltraChart ID="chart_avg_count" runat="server" BackgroundImageFileName=""
                BorderColor="InactiveCaption" BorderWidth="0px" ChartType="SplineAreaChart" EmptyChartText="данных не найдено"
                OnDataBinding="chart_avg_count_DataBinding" Version="8.2" SplineAreaChart-LineDrawStyle="Solid" OnInit="chart_avg_count_Init" OnInvalidDataReceived="chart_avg_count_InvalidDataReceived">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" />
                <Border Color="InactiveCaption" Thickness="0" />
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
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
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
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" TickmarkIntervalType="Weeks" Extent="40">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="40">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y>
                    <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False" TickmarkIntervalType="Weeks">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
            </igmisc:WebAsyncRefreshPanel>
        
        </td><td>                    
        
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Height="100%" Width="100%" RefreshTargetIDs="chart_avg_wage_title">            
            <igchart:UltraChart ID="chart_avg_wage" runat="server" BackgroundImageFileName=""
                BorderColor="InactiveCaption" BorderWidth="0px" ChartType="SplineChart" EmptyChartText="данных не найдено"
                OnDataBinding="chart_avg_wage_DataBinding" Version="8.2" OnInvalidDataReceived="chart_avg_wage_InvalidDataReceived">
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
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
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
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="40">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="40">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                Orientation="Horizontal" VerticalAlign="Center">
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
                <Border Color="InactiveCaption" Thickness="0" />
                <SplineChart>
                    <ChartText>
                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                            Row="-2" VerticalAlign="Far" Visible="True">
                        </igchartprop:ChartTextAppearance>
                    </ChartText>
                </SplineChart>
                <Legend Location="Top" SpanPercentage="15" Visible="True"></Legend>
            </igchart:UltraChart>
            </igmisc:WebAsyncRefreshPanel>
        
        </td></tr></table>
        
    </div>
    </form>
</body>
</html>
