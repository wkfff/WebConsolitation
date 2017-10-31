<%@ Page Language="C#" Title="Оценка экономического развития" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="defaultMO.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0002._0003.Default" %>

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
            <td align=right>
               &nbsp;<asp:HyperLink id="HyperLink3" runat="server" NavigateUrl="defaultMO.doc" SkinID="HyperLink" ToolTip="Справка"><img src="../../../../images/getHelp.gif" align="absmiddle" title="Аналитический материал" alt="Аналитический материал" border="0" /></asp:HyperLink>&nbsp;
            </td>                                             
            <td style="width: 100%">
                <asp:Label ID="Label1" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="16pt" Text="Уровень результативности деятельности органов местного самоуправления." SkinID="PageTitle"></asp:Label>
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
               &nbsp;<asp:HyperLink id="HyperLink1" runat="server" SkinID="HyperLink" Visible="False">Муниципальные&nbsp;районы</asp:HyperLink> 
            </td>                        
            <td>
               &nbsp;<asp:HyperLink id="HyperLink2" runat="server" NavigateUrl="default1.aspx" SkinID="HyperLink" Visible="False">Городские&nbsp;округа</asp:HyperLink> 
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
        OnInitializeLayout="web_grid1_InitializeLayout" Width="100%" EnableAppStyling="True" StyleSetName="Office2007Blue" OnInitializeRow="web_grid1_InitializeRow" OnActiveCellChange="web_grid1_ActiveCellChange" OnSortColumn="web_grid1_SortColumn">
        <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout BorderCollapseDefault="Separate" Name="webxgrid1" RowHeightDefault="20px" StationaryMarginsOutlookGroupBy="True"
                TableLayout="Fixed" Version="4.00" SelectTypeCellDefault="Single" SelectTypeColDefault="Single" AllowSortingDefault="OnClient" HeaderClickActionDefault="SortMulti" ScrollBarView="Horizontal">
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
            Transform3D-Perspective="40" Transform3D-Scale="85" Transform3D-XRotation="120"
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
                <X Extent="150" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
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
                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
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
        
        <br/>
        
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
        <div style="padding-left: 3px">
        <DMWC:MapControl ID="DundasMap" runat="server" BackColor="Transparent" BackSecondaryColor="Transparent"
            Height="800px" ResourceKey="#MapControlResKey#MapControl1#" Width="1024px" ImageUrl="../../../../TemporaryImages/MapPic_#SEQ(300,3)" BorderLineColor="Black" BorderLineWidth="1">
            <NavigationPanel>
                <Location X="0" Y="0" />
                <Size Height="90" Width="90" />
            </NavigationPanel>
            <Viewport>
                <Location X="0.0977517143" Y="0.125156447" />
                <Size Height="99.75" Width="99.80469" />
            </Viewport>
            <ZoomPanel>
                <Size Height="200" Width="40" />
                <Location X="0" Y="0" />
            </ZoomPanel>
            <ColorSwatchPanel Visible="True">
                <Location X="0.0977517143" Y="89.98749" />
                <Size Height="80" Width="180" />
            </ColorSwatchPanel>
            <DistanceScalePanel>
                <Location X="0" Y="0" />
                <Size Height="55" Width="130" />
            </DistanceScalePanel>
        </DMWC:MapControl>
        </div>
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
<br/>    

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
<asp:Label ID="Label3" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Small" Text="Информация"></asp:Label>
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
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Доходы населения</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Отношение среднемесячной номинальной начисленной заработной платы муниципальных учреждений к среднемесячной номинальной начисленной заработной платы работников крупных и средних предприятий и некоммерческих организаций максимально в Кадомском, Пителинском и Сасовском районах и составляет соответственно  148%, 128,7% и 99% соответственно, а минимально в Касимовском, Путятинском и Пронском районах и  составляет 41%, 42,6% и 44,1% соответственно. В то же время среднемесячная номинальная начисленная заработная плата работников крупных и средних предприятий и некоммерческих организаций муниципального района максимальна в Пронском, Путятинском и Касимовском районах и составляет 15020 рублей, 14525 рублей и 14167 рублей соответственно, а в  Чучковском, Кадомском и Сасовском районах составляет минимальное значение 4090 рублей, 5289 рублей и 7158 рублей соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Среднемесячная номинальная начисленная заработная плата учителей муниципальных общеобразовательных учреждений и детских дошкольных учреждений в Старожиловском, Путятинском и Кадомском районе составляет 9480 рублей, 9246 рублей и 8533 рублей соответственно, что является  максимальное значение по муниципальным районам, а  в Шацком, Пронском и Рыбновском районах является минимальной и составляет 4121 рубль, 4703 рубля и 5013  рублей соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Доходы населения» лучшим муниципальным районом признан Клепиковский район, худшим в данной сфере признан Шацкий район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Здоровье</b><br/>
 <br/> 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Михайловском, Старожиловском и Чучковском районах все население охвачено профилактическими осмотрами,  в то же время в Ухоловском, Клепиковском и Рязанском районах  уровень охвата населения профилактическими осмотрами минимален, по сравнению с остальными районами области.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Только в восьми муниципальных районах области 100 процентов муниципальных медицинских учреждений применяют медико-экономические стандарты оказания медицинской помощи – Новодеревенский, Ермишинский, Клепиковский, Путятинский, Ряжский, Сапожковский, Скопинский и Ухоловский районы.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Средняя продолжительность пребывания пациента на койке в круглосуточном стационаре муниципальных учреждений здравоохранения максимальна в Касимовском, Скопинском и Сасовском районах и составляет соответственно 21, 19 и 17,5 дней, минимальна  в Пронском, Путятинском и Кораблинском районах и составляет соответственно 9, 10 и 10,2 дней.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Здоровье» лучшим муниципальным районом признан Ряжский район, худшим в данной сфере признан Шиловский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Дошкольное и дополнительное образование детей</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля детей в возрасте от 3 до 7 лет, получающих дошкольную образовательную услугу и (или) услугу по их содержанию в организациях различной организационно – правовой  формы и формы собственности, в общей численности детей от 3 до 7 лет в Пителинском, Ермишинском и Пронском районах составляет 92, 88 и 85 процентов соответственно, что является максимальным значением по муниципальным районам, а в Путятинском, Сасовском и Сараевском всего соответственно 34, 34,6 и  36,6 процента – минимальное значение.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Удельный вес детей в возрасте 5 - 18 лет, получающих услуги по дополнительному образованию в организациях различной организационно – правовой  формы в Скопинском, Ряжском и Милославском районах составляет максимальное значение из всех муниципальных районах и составляет 97, 94 и 92 процента соответственно, а в Сараевском, Сасовском и Михайловском районах данный показатель составляет минимальное значение  15, 30 и 35 процентов соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Новодеревенском, Ермишинском, Кадомском, Касимовском, Михайловском, Ряжском, Рязанском, Сапожковском, Скопинском и Ухоловском районах доля детских дошкольных муниципальных учреждений от общего числа организаций оказывающих услуги по содержанию детей в таком учреждении и получающих средства бюджета городского округа на оказание таких услуг составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Дошкольное и дополнительное образование детей» лучшим муниципальным районом признан Пителинский район, худшим в данной сфере признан Сараевский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Образование (общее)</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В только в Кораблинском и Ермишенском районах 100 процентов лиц от числа выпускников образовательных муниципальных учреждений, сдали единый государственный экзамен, это является максимальным значением по муниципальным районам, в тоже время в Сапожковском районе данный показатель составляет всего 23,6  процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Численность учащихся, приходящихся на одного учителя  в муниципальных общеобразовательных учреждениях максимальна в Кораблинском и Ряжском районах и составляет соответственно 8,8 и 8,5 учеников на одного учителя, а минимальна в Скопинском районе 3,9 ученика на одного учителя.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Средняя наполняемость классов в муниципальных общеобразовательных учреждениях в максимальна в Пронском и Рязанском  районах и составила 14 и 11 человек соответственно, минимальная наполняемость в Кадомском районе 3,3  человека.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Образование (общее)» лучшим муниципальным районом признан Ухоловский район, худшим в данной сфере признан Сапожковский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Физическая культура и спорт</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Максимальный охват населения систематическими занятиями физической культурой и спортом составляет в Путятинском районе – 44 процента от всего населения, а минимальный в Рязанском районе – всего 0,06  процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Физическая культура и спорт» лучшим муниципальным районом признан Путятинский район, худшим в данной сфере признан Рязанский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Жилищно-коммунальное хозяйство</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Касимовском муниципальном районе, единственном среди остальных муниципальных районов,  доля объема отпуска коммунальных ресурсов, счета за которые выставлены по показаниям приборов учета, составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Ряжском, Старожиловском, Михайловском и Сараевском районах уровень собираемости платежей за предоставленные жилищно-коммунальные услуги приближается к 100 процентам.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Только в Рязанском, Михайловском и Пителинском районах процент подписанных паспортов готовности жилищного фонда и котельных (по состоянию на 15 ноября отчетного года) не составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Отношение тарифов для промышленных потребителей к тарифам для населения по водоснабжению максимальна в Ряжском, Кадомском и Пителинском районах – 236, 205 и 186 процентов соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Отношение тарифов для промышленных потребителей к тарифам для населения по водоотведению максимальна в Ряжском, Скопинском и Михайловском районах – 273, 201 и 152 процента соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Жилищно-коммунальное хозяйство» лучшим муниципальным районом признан Путятинский район, худшим в данной сфере признан Спасский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Доступность и качество жилья</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Путятинском, Клепиковском и Скопинском районах общая площадь жилых помещений, приходящаяся в среднем на одного жителя в сравнении с остальными муниципальными районами максимальна и составляет соответственно 33,4 , 33,4 и 32,8 квадратных метра, минимальное значение по данному показателю в Ухоловском районе 20,8 квадратных метров. В то же время общая площадь жилых помещений, приходящаяся в среднем на одного жителя, введенная в действие за год в Рыбновском районе больше чем в остальных городских округах и составляет 0,55 квадратного метра на 1 жителя.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля многоквартирных домов, расположенных на земельных участках, в отношении которых осуществлен государственный кадастровый учет максимальна в Ряжском и Сапожковском районах и составляет  45 и 38 процента соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Доступность и качество жилья» лучшим муниципальным районом признан Рязанский район, худшим в данной сфере признан Ухоловский район.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Организация местного самоуправления</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля собственных доходов местного бюджета (за исключением безвозмездных  поступлений, поступлений налоговых доходов по дополнительным нормативам отчислений и доходов от платных услуг, оказываемых муниципальными бюджетными учреждениями) в общем объеме доходов бюджета муниципального образования максимальна в Милославском, Пронском и Рыбновском районах и составляет 69,7 , 40,16 и 39,2 процентов соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Охват населения для участия в культурно-досуговых мероприятиях, организованных органами местного самоуправления муниципальных районов,  максимален Ермишенском, Сараевском и Кадомском районах.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки муниципальных районов в сфере «Организация местного самоуправления» лучшим муниципальным районом признан Рязанский район, худшим в данной сфере признан Спасский муниципальный район.<br/>
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


</asp:Content>
