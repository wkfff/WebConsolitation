<%@ Page Language="C#" Title="Оценка экономического развития" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="defaultCity.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0002._0004.dafault1" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>


<%@ Register Src="~/Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc5" %>
<%@ Register Src="~/Components/UltraGridExporter.ascx" TagName="UltraGridExporter" TagPrefix="uc4" %>
<%@ Register Src="~/Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"    TagPrefix="uc3" %>


<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table>
        <tr>
            <td style="width: 100%">
                <asp:Label ID="Label1" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="16pt" Text="Оценка эффективности расходования бюджетных средств" SkinID="PageTitle"></asp:Label>
            </td>
            <td>            
               &nbsp;<uc4:UltraGridExporter id="UltraGridExporter1" runat="server"></uc4:UltraGridExporter>                        
            </td>                                                                          
        </tr>
    </table>

    
   
    <table>
        <tr>
            <td valign="top" style="padding-top: 2px">
                <uc3:custommulticombo id="ComboYear" runat="server"></uc3:custommulticombo>
            </td>
            <td>
                <uc5:refreshbutton id="RefreshButton1" runat="server"></uc5:refreshbutton>
            </td>
            <td>
               &nbsp;<asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="defaultMO.aspx" SkinID="HyperLink" Visible="False">Муниципальные районы</asp:HyperLink> 
            </td>                        
            <td>
               &nbsp;<asp:HyperLink id="HyperLink2" runat="server" SkinID="HyperLink" Visible="False">Городские округа</asp:HyperLink> 
            </td>          
        </tr>
     </table>

    <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
            <td style="overflow: visible;">
<igtbl:UltraWebGrid ID="web_grid1" runat="server" OnDataBinding="web_grid1_DataBinding"
        OnInitializeLayout="web_grid1_InitializeLayout" Width="100%" EnableAppStyling="True" StyleSetName="Office2007Blue" OnInitializeRow="web_grid1_InitializeRow" OnClick="web_grid1_Click">
        <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout BorderCollapseDefault="Separate" Name="webxgrid1" RowHeightDefault="20px" StationaryMarginsOutlookGroupBy="True"
                TableLayout="Fixed" Version="4.00" SelectTypeCellDefault="Single" SelectTypeColDefault="Single" AllowSortingDefault="OnClient" HeaderClickActionDefault="SortMulti" ScrollBarView="Horizontal" NoDataMessage="Данных не найдено">
                <GroupByBox Hidden="True">
                    <BoxStyle BackColor="ActiveBorder" BorderColor="White">
                    </BoxStyle>
                </GroupByBox>
                <GroupByRowStyleDefault BackColor="Control" BorderColor="White">
                </GroupByRowStyleDefault>
                <ActivationObject BorderColor="" BorderWidth="">
                </ActivationObject>
                <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </FooterStyleDefault>
                <RowStyleDefault BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                    Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                    <BorderDetails ColorLeft="White" ColorTop="White" />
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
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px"/>
                </HeaderStyleDefault>
                <EditCellStyleDefault BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                    Font-Names="Arial" Font-Size="8.25pt">
                    <BorderDetails ColorLeft="White" ColorTop="White" />
                    <Padding Left="3px" />
                </EditCellStyleDefault>
                <FrameStyle BackColor="Transparent" BorderStyle="None" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt"
                    Width="100%">
                </FrameStyle>
                <Pager MinimumPagesForDisplay="2">
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </PagerStyle>
                </Pager>
                <AddNewBox Hidden="False">
                    <BoxStyle BackColor="White" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </BoxStyle>
                </AddNewBox> 
            <RowAlternateStyleDefault BackColor="#F1F1F2">
            </RowAlternateStyleDefault>
            <SelectedRowStyleDefault BackColor="#EFF1B4">
            </SelectedRowStyleDefault>           
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
     
        
    <asp:Label ID="Label2" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
        Font-Size="X-Small" Text="Для просмотра диаграмм (карт) по показателям выберите любое поле столбца с показателем в таблице, по которому необходимо построить диаграмму (карту). Диаграмма (карта) будет обновлена спустя 1-5 секунд."></asp:Label><br />
    <br />
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%" TriggerControlIDs="web_grid1">
        <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
            <tr>
                <td class="topleft">
                </td>
                <td class="top">
                </td>
                <td class="topright">
                </td>
            </tr>
            <tr>
                <td class="headerleft">
                </td>
                <td class="headerReport">
<asp:Label ID="chart1_caption" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Small" Text="заголовок таблицы"></asp:Label>
                </td>
                <td class="headerright">
                </td>
            </tr>
            <tr>
                <td class="left">
                </td>
                <td style="overflow: visible;">
                    <table>
                        <tr>
                            <td>
<igchart:UltraChart
            ID="Chart1" runat="server" BackgroundImageFileName=""   
            ChartType="CylinderColumnChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
            Height="400px" OnDataBinding="Chart1_DataBinding" OnInvalidDataReceived="InvalidDataReceived"
            Transform3D-Perspective="20" Transform3D-Scale="85" Transform3D-XRotation="120"
            Transform3D-YRotation="0" Version="9.1" Width="100%">
            <Tooltips Display="Never" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                Font-Strikeout="False" Font-Underline="False" FormatString="&lt;DATA_VALUE:###,##0.##&gt;" />
            <Data SwapRowsAndColumns="True" ZeroAligned="True">
            </Data>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
            </ColorModel>
            <Effects>
                <Effects>
                    <igchartprop:GradientEffect>
                    </igchartprop:GradientEffect>
                </Effects>
            </Effects>
            <Axis>
                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
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
                <X Extent="0" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Margin>
                        <Near Value="2" />
                    </Margin>
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 9.75pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="UseCollection">
                                <BehaviorCollection>
                                    <igchartprop:FontScalingAxisLabelLayoutBehavior MaximumSize="10" MinimumSize="10">
                                    </igchartprop:FontScalingAxisLabelLayoutBehavior>
                                </BehaviorCollection>
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X>
                <Y Extent="30" LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
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
            <Legend Location="Left" SpanPercentage="10"></Legend>
        </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
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
        
        
    </igmisc:WebAsyncRefreshPanel>                                                            
    <asp:PlaceHolder ID="ContactInformationPlaceHolder" runat="server"></asp:PlaceHolder>    
</asp:Content>
