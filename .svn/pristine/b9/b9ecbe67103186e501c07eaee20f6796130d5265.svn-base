<%@ Page Language="C#" Title="Оценка экономического развития" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="DefaultCity.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0002._0002_.Default" %>

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
                <asp:Label ID="LabelHeader" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="16pt" Text="Заголовок" SkinID="PageTitle"></asp:Label>
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
               &nbsp;<asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="default.aspx" Font-Names="Arial" Font-Size="Small" SkinID="HyperLink" Visible="False">Муниципальные районы</asp:HyperLink> 
            </td>                        
            <td>
               &nbsp;<asp:HyperLink id="HyperLink2" runat="server" Font-Names="Arial" Font-Size="Small" SkinID="HyperLink" Visible="False">Городские округа</asp:HyperLink> 
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
                TableLayout="Fixed" Version="4.00" ScrollBar="Never" SelectTypeCellDefault="Single" SelectTypeColDefault="Single" AllowSortingDefault="OnClient" HeaderClickActionDefault="SortMulti">
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
<igchart:UltraChart ID="Chart1" runat="server" BackgroundImageFileName=""  
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
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Эффективная деятельность органов государственной власти  регионального и местного уровня имеет огромное значение для  экономического развития, как региона в целом, так и отдельных городских и районных образований.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Влияние этой деятельности на уровень экономического развития заключается в создании условий для привлечения инвестиций, расширение промышленного, сельскохозяйственного и других видов производства, развития предпринимательской деятельности, в частности малого и среднего бизнеса.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Для достижения поставленных целей необходим рост финансовых возможностей бюджетов, создание развитой инфраструктуры, в том числе транспортной, снижение безработицы и административных барьеров.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В 2008 году, несмотря на кризисные явления, в Рязанской области наблюдалась положительная динамика практически по всем основным показателям, характеризующим экономическое развитие региона.<br/>  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Одним из самых важных показателей уровня развития региона является валовой региональный продукт. Его объем в 2008 году составил 145,7 млрд. руб. Это на 19,6 млрд. руб. больше уровня 2007 года.<br/> 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатель объема инвестиций в основной капитал (за исключением бюджетных средств) в расчете на одного человека в последние годы характеризовался устойчивой динамикой ежегодного роста. За 2008 год прирост составил 30,6 %.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Этому способствовала активная инвестиционная политика Правительства Рязанской области, направленная на привлечение крупных инвесторов на территорию региона, строительство современных предприятий, освоение месторождений полезных ископаемых и др.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Влияние экономического кризиса сказалось на индексе промышленного производства, который составил всего 99,6%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Продукция сельского хозяйства выросла на 18,2% и составила 28,8 млрд. руб.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Оборот розничной торговли возрос на 20,7% и составил 82,8 млрд. рублей.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Реальные располагаемые денежные доходы населения к предыдущему году составили 121,1%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Среднемесячная номинальная начисленная заработная плата составила 12687,4 руб. и возросла по отношению к 2007 году на 29,5%.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>1. Экономическое развитие.</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>1.1. Дорожное хозяйство и транспорт.</b><br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатель доли отремонтированных автомобильных дорог общего пользования местного значения с твердым покрытием, в отношении которых произведен капитальный и текущий ремонт, является обязательным для заполнения только для городских округов Рязанской области.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В 2008 году капитальный ремонт произведен в 2 городских округах – городе Рязани и городе Скопине.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Текущий ремонт произведен во всех 4 городских округах.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городских округах Рязанской области в 2008 году отсутствуют автомобильные дороги местного значения с твердым покрытием, переданные на техническое обслуживание немуниципальным и (или) государственным предприятиям на основе долгосрочных договоров (свыше 3 лет).<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля автомобильных дорог общего пользования местного значения с твердым покрытием в общей протяженности автомобильных в 2008 году выросла в 7 муниципальных образованиях, снизилась в 1 (Рязанский район) и осталась без изменения в 21, в том числе в 4  районах она уже в 2007 году составляла 100%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В 24 районах и городе Рязани имеются населенные пункты, не обеспеченные регулярным транспортным сообщением с административными районными центрами. Доля населения, проживающего в населенных пунктах, не имеющих регулярного автобусного и (или) железнодорожного сообщения с административным центром городского округа (муниципального района), в общей численности населения городского округа (муниципального района) уменьшилась в 2008 году в 9 районах (Захаровском, Кадомском, Касимовском, Клепиковском, Новодеревенском, Рязанском, Сасовском, Скопинском и Спасском), возросла в 1 районе (Шацком), в 14 районах и городе Рязани осталась без изменения.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Расходы бюджетных средств  на развитие транспорта в 2008 году были произведены в 17 (59%) муниципальных образованиях.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Расходы на развитие дорожного хозяйства  произведены в 25 (86%) муниципальных образованиях.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Таким образом, учитывая важность развития транспортной инфраструктуры области, развитие дорожного хозяйства, главам всех муниципальных образований области необходимо усилить работу в данной сфере, для чего:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- произвести инвентаризацию дорог, находящихся в границах муниципального образования и принять на баланс бесхозяйные дороги;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- обеспечить нормальное состояние автомобильных дорог, проводить текущий и капитальный ремонт;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- развивать маршрутную сеть и сокращать количество населенных пунктов, не имеющих транспортного сообщения с административным центром;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- в бюджетах муниципальных образований предусмотреть финансирование дальнейшего развития транспорта и дорожного хозяйства.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>1.2. Развитие малого и среднего предпринимательства.</b><br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатели, характеризующие развитие малого и среднего предпринимательства, являются обязательными для заполнения только для городских округов Рязанской области.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В среднем по Рязанской области количество субъектов малого предпринимательства на 10 тысяч человек населения  в 2008 году составляет 70 единиц.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городах Скопин и Сасово данный показатель выше среднеобластного, а в городе Касимов – ниже. По городу Рязани данные не представлены.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля среднесписочной численности работников малых предприятий в общей среднесписочной численности работников Рязанской области в 2008 году составляет 22%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городе Скопин данный показатель выше среднеобластного, а в городах Касимов и Сасово – ниже. По городу Рязани данные так же не представлены.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Бюджетные финансовые средства на развитие и поддержку малого предпринимательства в 2008 году были направлены только в городе Рязани в размере 0,6 млн. руб.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Развитие предпринимательской деятельности является одной из приоритетных задач в Рязанской области. Для ее осуществления необходимо провести следующее:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- принять муниципальную программу развития малого и среднего предпринимательства, включив в нее мероприятия по финансовой поддержке;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- активизировать работу общественных координационных советов по вопросам предпринимательства при главах районных администраций;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- обеспечить информирование жителей области о действующей областной программе «Развитие малого  и среднего предпринимательства в Рязанской области в 2009 году» и проводимых конкурсах по предоставлению субсидий из бюджетов различных уровней на поддержку субъектов малого и среднего предпринимательства.<br/>
<br/> 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>1.3. Улучшение инвестиционной привлекательности.</b><br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатели, характеризующие улучшение инвестиционной привлекательности, являются обязательными для заполнения только для городских округов Рязанской области.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городских округах Рязанской области 2008 году было выделено земельных участков, предоставленных для строительства 98,5 га, что меньше чем в 2007 году на 15 га за счет уменьшения площадей в городе Рязани.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Для жилищного и индивидуального жилищного строительства в 2008 году выделено 28,2 га, что меньше чем в 2007 году на 13,5 га. Снижение произошло во всех городских округах.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Среди районных муниципальных образований, представивших сведения в данном разделе, следует отметить Скопинский район, где под строительство было выделено 239,1 га, которое практически полностью будет использовано под производственное строительство.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Площади для комплексного освоения в целях жилищного строительства в 2008 году не предоставлялись.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля площади земельных участков, являющихся объектами  налогообложения земельным налогом, в общей площади территории городского округа, по городским округам очень сильно отличается. В городе Рязани она составляет 17,8%, в городе Скопин – 37,7%,а в городе Сасово – 81%. По городу Касимов данные не представлены.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Среди районных муниципальных образований, представивших сведения в данном разделе, наибольшее значение данного показателя показано в Спасском районе – 94%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Средняя по области продолжительность периода с даты принятия решения о предоставлении земельного участка для строительства или подписания протокола о результатах торгов (конкурсов, аукционов) по предоставлению земельных участков до даты получения разрешения на строительство в 2008 году составляет 4,2 месяца.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Данный показатель по всем городским муниципальным образованиям в 2008 году по сравнению с 2007 годом снижен. Однако в городе Скопин он ниже среднеобластного в 2 раза (60 дней), в городе Сасово практически на среднем уровне (130 дней), в городе Касимов несколько выше среднеобластного (154 дня), а в городе Рязань выше среднего по области в 3,7 раза.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Площадь земельных участков, предоставленных для строительства жилых объектов в городских округах, которые не введены в течении 3 лет, в 2008 году снижена в городе Рязань на 1 га и городе Сасово на 0,2 га. Однако в городе Рязань размер этих площадей составляет 29,1 га.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Еще более значительные площади в городских округах занимают прочие объекты, на ввод которых в течение 5 лет не получено разрешение. В городе Рязани такие объекты занимают площадь 72,2 га, в городе  Касимов 26,3 га, в городе Сасово – 11,6 га (по городу Скопин данные не представлены).<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатели площади земельных участков, выделяемых для строительства, и сроков оформления разрешений на строительство являются очень важными для характеристики деятельность органов местного самоуправления по созданию условий для привлечения инвестиций и соответственно развития экономики муниципального образования.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Показатели доли площади земельных участков, являющихся объектами  налогообложения земельным налогом, в общей площади территории городского округа и площади земли под не введенными в срок объектами влияют на доходную часть бюджетов муниципальных образований, формируемую за счет земельного налога и налога на имущество.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Для обеспечения инвестиционной привлекательности городских и районных округов главам администраций необходимо:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- систематически проводить анализ показателей, характеризующих инвестиционную привлекательность;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- увеличивать площади, выделяемые под строительство;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- увеличивать объемы жилищного строительства, создавая условия для развития комплексной застройки территорий;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- принимать меры к увеличению площадей земельных участков, являющихся объектами налогообложения земельным налогом;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- добиваться своевременного ввода объектов в эксплуатацию,  снижению размера площадей под незавершенными строительными объектами;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- добиваться снижения сроков оформления разрешений на строительство, способствовать снижению административных барьеров.<br/>
<br/>  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>1.4. Сельское хозяйство.</b><br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Рязанской области в 2008 году наблюдается тенденция сокращения убыточных сельскохозяйственных предприятий.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Удельный вес прибыльных сельскохозяйственных предприятий в общем их числе в 2008 году вырос на 5,8 п.п. и составил 87%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В 2008 году из 25 муниципальных районов области 11 районов имеют уровень данного показателя выше среднеобластного, в том числе в 6 районах он составляет 100%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Обеспечен рост данного показателя в 15 районах области. В 2 районах произошло снижение доли прибыльных предприятий: в Сасовском районе на 4,2 п.п. и Скопинском районе на 2,3 п. п. В 4 районах этот показатель остался на уровне 2007 года и в 4  районах он уже в 2007 году составлял 100%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;При этом обращает на себя внимание тот факт, что в Пителинском районе совсем отсутствуют прибыльные крупные и средние сельскохозяйственные предприятия.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В 2008 году обеспечен рост фактически используемых сельскохозяйственных угодий в общей площади сельскохозяйственных угодий в 12 районах. В 8 районах произошло снижение, а в 5 районах доля площадей осталась без изменений, в том числе в Новодеревенском районе, где она уже в 2007 году составляла 100%.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Так как, сельское хозяйство занимает значительное  место в экономике региона, дальнейшее его развитие является одной из важнейших задач, стоящих перед муниципальными образованиями.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Для этого главам районных администраций необходимо:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- провести мероприятия, направленные на обеспечение рентабельной работы сельскохозяйственных предприятий;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- добиваться увеличения используемых в сельскохозяйственном производстве земель;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- усилить контроль за использованием средств, направляемых на развитие сельского хозяйства, как финансовых средств государственной поддержки, так и привлекаемых кредитных ресурсов;<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- принять меры по выполнению задач, поставленных Государственной программой развития сельского хозяйства и регулирования рынков сельскохозяйственной продукции, сырья и продовольствия на 2008-2012 годы и областной программой развития АПК Рязанской области до 2012 года.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Проведенным анализом докладов глав администраций муниципальных образований Рязанской области о достигнутых значениях показателей по направлению «Экономическое развитие» лучшими муниципальными районами признаны Ермишенский район (1 место), Ряжский район (2 место), Михайловский район (3 место) худшим муниципальным районом признан Шиловский район (последнее место), а среди городских округов лучшим признан город Сасово (1 место), худшим город Рязань (последнее место).<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Результаты оценки могут быть скорректированы в случае уточнении данных, отраженных в докладах.
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
