<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default_FFSSR.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MFRF_0001_0004_FFSSR.Default" Title="Федеральный фонд софинансирования социальных расходов"  %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc4" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>

<%@ Register Src="../../components/PeriodParameter.ascx" TagName="PeriodParameter" TagPrefix="uc1" %>
<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebTab.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>
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

<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                            <table>
                                            <tr>
                                            <td>
                                            <font style="font-size: 16px; font-family: Verdana">
                                                <asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Федеральный фонд софинансирования социальных расходов"></asp:Label>
                                                </font>
                                            </td>
                  
                                                <td colspan="1" valign=top style="width: 60px" align=center>
                                                <a href="Default_FF.aspx" style="font-size: 10px; font-family: Verdana">Фонды</a></td>
                                                <td colspan="1" valign=top style="width: 60px" align=center>
                                                <a href="Default_FFK.aspx" style="font-size: 10px; font-family: Verdana">ФФК</a></td>
                                                <td colspan="1" valign=top style="width: 60px" align=center>
                                                <a href="Default_FFPR.aspx" style="font-size: 10px; font-family: Verdana">ФФПР</a></td>
                                            </tr>
                                            </table>
                                            </td>
                                        </tr>
                                        <tr>
                                        <td  rowspan="1" valign="top">
                                        <table><tr>
                                        <td valign="top">
                                            <asp:Label ID="Label2" runat="server" CssClass="ParamChooseTitle" Text="Год"></asp:Label><br />
                                            <igcmbo:WebCombo ID="ComboYear" runat="server" BackColor="White" BorderColor="Silver"
                                                BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                                                Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                                                StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="97px">
                                                <Columns>
                                                    <igtbl:UltraGridColumn Width="30px">
                                                        <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                                                        <header caption="Уровень детализации" key="mode"></header>
                                                        <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                                            font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                                                    </igtbl:UltraGridColumn>
                                                </Columns>
                                                <ExpandEffects ShadowColor="LightGray" />
                                                <DropDownLayout ColHeadersVisible="No" DropdownHeight="260px" DropdownWidth="80px"
                                                    GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                                                    XmlLoadOnDemandType="Synchronous">
                                                    <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                                                    </RowAlternateStyle>
                                                    <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                        Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                                                        <BorderDetails StyleLeft="None" />
                                                    </RowSelectorStyle>
                                                    <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                        Font-Size="X-Small" Height="260px" Width="80px">
                                                    </FrameStyle>
                                                    <RowStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px"
                                                        Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px">
                                                        <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                                                    </RowStyle>
                                                    <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid"
                                                        BorderWidth="1px" />
                                                </DropDownLayout>
                                                <Rows>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="1998"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="1999"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2000"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2001"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2002"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2003"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2004"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2005"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2006"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2007"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2008"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2009"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2010"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>                               
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2011"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2012"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2013"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2014"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2015"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2016"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2017"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2018"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2019"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                    <igtbl:UltraGridRow Height="">
                                                        <cells>
<igtbl:UltraGridCell Text="2020"></igtbl:UltraGridCell>
</cells>
                                                    </igtbl:UltraGridRow>
                                                </Rows>
                                            </igcmbo:WebCombo>
                                            </td>                                                
                                            
                                            <td colspan="1" valign="top"><br/>
                                                &nbsp;<uc4:RefreshButton ID="RefreshButton1" runat="server" />
                                            </td>
                                        
                                        </tr></table>
                                            <igtbl:UltraWebGrid ID="UltraWebGridFFSSR" runat="server" EnableAppStyling="True"
                                                    Height="200px" OnActiveRowChange="UltraWebGridFFSSR_ActiveRowChange" OnDataBinding="UltraWebGridFFSSR_DataBinding"
                                                    OnInitializeLayout="UltraWebGridFFSSR_InitializeLayout" StyleSetName="Office2007Blue"
                                                    Width="325px" SkinID="UltraWebGrid">
                                                    <Bands>
                                                        <igtbl:UltraGridBand>
                                                            <AddNewRow View="NotSet" Visible="NotSet">
                                                            </AddNewRow>
                                                        </igtbl:UltraGridBand>
                                                    </Bands>
                                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                                        HeaderClickActionDefault="SortMulti" Name="UltraWebGridFFSSR" RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
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
                                          <td valign="top">
                                                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel7" runat="server" TriggerControlIDs="UltraWebGridFFSSR">
                                                <nobr><asp:Label id="lbFO" runat="server" Text="Label" Font-Bold="True" CssClass="ElementTitle"></asp:Label></nobr>
                                                    <igchart:UltraChart ID="UltraChartFFSSR2" runat="server" BackgroundImageFileName=""
                                                           ChartType="StackColumnChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                        Height="365px" OnDataBinding="UltraChartFFSSR2_DataBinding" Version="7.3">
                                                        <Tooltips FormatString="&lt;SERIES_LABEL&gt;: &lt;DATA_VALUE:N2&gt; руб." HotTrackingFillColor="Yellow"
                                                            HotTrackingOutlineColor="Black" Font-Names="Verdana" Font-Size="Small" />
                                                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_mfrf01_04_#SEQNUM(100).png" />
                                                        <ColorModel AlphaLevel="150">
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
                                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
                                                            <Y2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                    Visible="False" />
                                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                    Visible="True" />
                                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                                        <Layout Behavior="Auto">
                                                                        </Layout>
                                                                    </SeriesLabels>
                                                                </Labels>
                                                            </Y2>
                                                            <X Extent="50" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                    Visible="False" />
                                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                    Visible="True" />
                                                                <Labels Flip="True" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                                    ItemFormatString="&lt;ITEM_LABEL&gt;" Orientation="VerticalLeftFacing" OrientationAngle="210"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                    <SeriesLabels Flip="True" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                                                        Orientation="Custom" OrientationAngle="210" VerticalAlign="Center">
                                                                        <Layout Behavior="Auto">
                                                                        </Layout>
                                                                    </SeriesLabels>
                                                                </Labels>
                                                            </X>
                                                            <Y LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
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
                                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
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
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                        VerticalAlign="Center">
                                                                        <Layout Behavior="Auto">
                                                                        </Layout>
                                                                    </SeriesLabels>
                                                                </Labels>
                                                            </Z2>
                                                        </Axis>
                                                        <Legend Font="Verdana, 7.25pt" Location="Bottom" SpanPercentage="27" Visible="True">
                                                        </Legend>
                                                    </igchart:UltraChart>
                                                    <nobr><asp:Label id="lbSybject" runat="server" Text="Label" Font-Bold="True" CssClass="ElementTitle"></asp:Label></nobr><igchart:UltraChart ID="UltraChartFFSSR1" runat="server" BackgroundImageFileName=""
                                                           ChartType="StackColumnChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                        Height="325px" OnDataBinding="UltraChartFFSSR1_DataBinding" Version="7.3">
                                                        <Tooltips FormatString="&lt;DATA_VALUE:N2&gt; руб." HotTrackingFillColor="Yellow"
                                                            HotTrackingOutlineColor="White" Font-Names="Verdana" Font-Size="Small" />
                                                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_mfrf01_04_#SEQNUM(100).png" />
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
                                                            <Y2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                    Visible="False" />
                                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                    Visible="True" />
                                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                                        <Layout Behavior="Auto">
                                                                        </Layout>
                                                                    </SeriesLabels>
                                                                </Labels>
                                                            </Y2>
                                                            <X Extent="20" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
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
                                                            <Y LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True">
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
                                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
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
                                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                        VerticalAlign="Center">
                                                                        <Layout Behavior="Auto">
                                                                        </Layout>
                                                                    </SeriesLabels>
                                                                </Labels>
                                                            </Z2>
                                                        </Axis>
                                                        <Legend Font="Verdana, 7.25pt" Location="Bottom" SpanPercentage="40" Visible="True">
                                                        </Legend>
                                                    </igchart:UltraChart>
                                                    </igmisc:WebAsyncRefreshPanel>
                                            </td>     
                                        </tr>
                                    </table>
    
    </div>
    </asp:Content>
