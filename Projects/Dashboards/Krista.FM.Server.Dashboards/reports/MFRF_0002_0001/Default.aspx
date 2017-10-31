<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MFRF_0002_0001.Default" Title="Мониторинг соблюдения требований БК и КУ"  %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc3" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>

<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

    <asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
            <td>
               <asp:Label ID="Label1" runat="server" SkinID="PageTitle" Text="Мониторинг соблюдения требований БК и КУ"></asp:Label>                 
            </td>
            </tr>
            <tr>
            <td><table><tr>            
            <td valign="top" style="font-family: Verdana; font-size: small">
                <asp:Label ID="Label2" runat="server" CssClass="ParamChooseTitle" Text="Год"></asp:Label></td>
                <td valign="top" style="font-family: Verdana; font-size: small">
                    <asp:Label ID="Label3" runat="server" CssClass="ParamChooseTitle" Text="Квартал"></asp:Label></td>            
            </tr>
            <tr>            
            <td valign="top"><igcmbo:WebCombo ID="ComboYear" runat="server" BackColor="White" BorderColor="Silver"
                    BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                    Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                    StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="80px" SelectedIndex="9">
                <Columns>
                    <igtbl:UltraGridColumn Width="50px">
                        <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                        <header caption="Уровень детализации" key="mode"></header>
                        <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                            font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                    </igtbl:UltraGridColumn>
                </Columns>
                <ExpandEffects ShadowColor="LightGray" />
                <DropDownLayout ColHeadersVisible="No" DropdownHeight="110px" DropdownWidth="100px"
                        GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                        XmlLoadOnDemandType="Synchronous">
                    <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                    </RowAlternateStyle>
                    <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                        <BorderDetails StyleLeft="None" />
                    </RowSelectorStyle>
                    <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Size="X-Small" Height="110px" Width="100px">
                    </FrameStyle>
                    <RowStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px"
                            Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px">
                        <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                    </RowStyle>
                    <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid"
                            BorderWidth="1px" />
                </DropDownLayout>
                <ClientSideEvents AfterSelectChange="ComboYear_AfterSelectChange" />
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
                </Rows>
            </igcmbo:WebCombo>
            </td>
                 <td valign="top">
                <igcmbo:WebCombo ID="ComboQuarter" runat="server" BackColor="White" BorderColor="Silver"
                    BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                    Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                    StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="80px">
                    <Columns>
                        <igtbl:UltraGridColumn Width="50px">
                            <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                            <header caption="Уровень детализации" key="mode"></header>
                            <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                        </igtbl:UltraGridColumn>
                    </Columns>
                    <ExpandEffects ShadowColor="LightGray" />
                    <DropDownLayout ColHeadersVisible="No" DropdownHeight="110px" DropdownWidth="100px"
                        GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                        XmlLoadOnDemandType="Synchronous">
                        <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                        </RowAlternateStyle>
                        <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                            <BorderDetails StyleLeft="None" />
                        </RowSelectorStyle>
                        <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Size="X-Small" Height="110px" Width="100px">
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
<igtbl:UltraGridCell Text="1"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="2"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="3"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="4"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="За год"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                    </Rows>
                    <ClientSideEvents AfterSelectChange="ComboQuarter_AfterSelectChange" />
                </igcmbo:WebCombo></td>
                    <td valign="top">
                    <igmisc:WebPanel ID="WebPanel" runat="server" EnableAppStyling="True" StyleSetName="Office2007Blue" Expanded="False">
                        <Template>
                            <uc1:DimensionTree ID="Indicator" runat="server" CubeName="МФ РФ_Индикаторы БК и КУ"
                                DefaultMember="[МФ РФ].[Сопоставимый индикаторы БККУ].[Все индикаторы].[Установленный уровень оплаты населением жилищно-коммунальных услуг]"
                                HierarchyName="[МФ РФ].[Сопоставимый индикаторы БККУ]" ProviderKind="Secondary"
                                Width="200" MultipleChoice="false" EnableTheming="true" Height="200" />
                        </Template>
                    </igmisc:WebPanel></td>
                     <td valign="top">
                         <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                     </td>
                
            </tr>
            </table></td>
            </tr>
            <tr>            
            <td valign="top">
                <table>
                    <tr>
                        <td valign="top" style="height: 707px">
                            <table>
                                <tr>
                                     <td valign="top" align="left">
                    <igtbl:UltraWebGrid ID="IndicatorsGrid" runat="server" Height="200px" Width="325px" OnActiveRowChange="IndicatorsGrid_ActiveRowChange" OnDataBinding="IndicatorsGrid_DataBinding" OnInitializeLayout="IndicatorsGrid_InitializeLayout" EnableAppStyling="True" StyleSetName="Office2007Blue" OnInitializeRow="IndicatorsGrid_InitializeRow" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            HeaderClickActionDefault="SortMulti" Name="IndicatorsGrid" RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
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
            </tr>
            <tr>           
                            <td valign="top" style="font-family: Verdana; font-size: small">
                                <asp:Label ID="lbInfo" runat="server"></asp:Label></td>
                        </tr>
            </table></td>
            <td valign="top">
                <table><tr>
                  <td valign="top" align="left">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" TriggerControlIDs="IndicatorsGrid">
                    <igchart:UltraChart ID="DynamicChart" runat="server" BackgroundImageFileName=""  
                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        Version="7.3" OnDataBinding="DynamicChart_DataBinding" OnFillSceneGraph="DynamicChart_FillSceneGraph">
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
                        <Legend Location="Bottom"></Legend>
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                       <igchart:UltraChart ID="SubjectsFOChart" runat="server" BackgroundImageFileName=""
                              EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                           OnDataBinding="SubjectsFOChart_DataBinding" OnFillSceneGraph="SubjectsFOChart_FillSceneGraph"
                           Version="7.3">
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
                                   <Labels Flip="True" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                       ItemFormatString="&lt;ITEM_LABEL&gt;" Orientation="Custom" OrientationAngle="210"
                                       VerticalAlign="Center">
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
                           <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                           <TitleTop ReverseText="True">
                           </TitleTop>
                       </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel></td>
                </tr>
                </table></td>
            </tr></table></td>
            </tr>
        </table>
    </div>
    </asp:Content>
