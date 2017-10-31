<%@ Page Language="C#" Title="Оценка экономического развития" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="DefaultCity.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0002._0003_.Default" %>

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
               &nbsp;<asp:HyperLink id="HyperLink4" runat="server" NavigateUrl="DefaultCity.doc" SkinID="HyperLink" ToolTip="Справка"><img src="../../../../images/getHelp.gif" align="absmiddle" title="Аналитический материал" alt="Аналитический материал" border="0" /></asp:HyperLink>&nbsp;
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
               &nbsp;<asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="default.aspx" SkinID="HyperLink" Visible="False">Муниципальные&nbsp;районы</asp:HyperLink> 
            </td>                        
            <td>
               &nbsp;<asp:HyperLink id="HyperLink2" runat="server" SkinID="HyperLink" Visible="False">Городские&nbsp;округа</asp:HyperLink> 
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
                TableLayout="Fixed" Version="4.00" ScrollBar="Always" SelectTypeCellDefault="Single" SelectTypeColDefault="Single" AllowSortingDefault="OnClient" HeaderClickActionDefault="SortMulti" ScrollBarView="Horizontal">
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
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Отношение среднемесячной номинальной начисленной заработной платы муниципальных учреждений к среднемесячной номинальной начисленной заработной платы работников крупных и средних предприятий и некоммерческих организаций минимально в г. Рязани и составляет 47,77 процента, а максимально в г. Скопин и составляет 74,5процента, в то же время среднемесячная номинальная начисленная заработная плата работников крупных и средних предприятий и некоммерческих организаций городского округа (муниципального района) в г. Рязань составляет 15 646,90 рублей, что составляет максимальное значение по городским округам, а в г. Скопин составляет 8 558 рублей – минимальное значение по городским округам.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Среднемесячная номинальная начисленная заработная плата учителей муниципальных общеобразовательных учреждений и детских дошкольных учреждений так же в г. Рязань составляет максимальное значение по городским округам 6 356 и 5 481 рублей соответственно.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Доходы населения» лучшим городским округом признан город Рязань, худшим в данной сфере признан город Сасово.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Здоровье</b><br/>
 <br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городских округах доля населения, охваченного профилактическими осмотрами максимальна в г. Скопин и составляет 99,4 процента, минимальное значение данного показателя в г. Рязань и составляет всего 42 процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городах Рязань, Касимов и Скопин доля муниципальных медицинских учреждений применяющих медико-экономические стандарты оказания медицинской помощи составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Средняя продолжительность пребывания пациента на койке в круглосуточном стационаре муниципальных учреждений здравоохранения минимальна в городе Рязань и составляет 11 дней, максимальна в городе Сасово и составляет 12,5 дней.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Здоровье» лучшим городским округом признан город Рязань, худшим в данной сфере признан город Сасово.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Дошкольное и дополнительное образование детей</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля детей в возрасте от 3 до 7 лет, получающих дошкольную образовательную услугу и (или) услугу по их содержанию в организациях различной организационно – правовой  формы и формы собственности, в общей численности детей от 3 до 7 лет в городе Сасово составляет 87 процентов, что является максимальным значением по городским округам, а в городе Скопин всего 62 процента – минимальное значение. В то же время удельный вес детей в возрасте 5 - 18 лет, получающих услуги по дополнительному образованию в организациях различной организационно – правовой  формы наоборот в городе Скопин составляет максимальное значение из всех городских округов и составляет 79 процентов, а в городе Сасово данный показатель составляет минимальное значение 43,3 процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля детских дошкольных муниципальных учреждений от общего числа организаций оказывающих услуги по содержанию детей в таком учреждении и получающих средства бюджета городского округа на оказание таких услуг в городе Касимов и городе Скопин составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Дошкольное и дополнительное образование детей» лучшим городским округом признан город Сасово, худшим в данной сфере признан город Скопин.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Образование (общее)</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городе Скопин 97,9 процентов лиц от числа выпускников образовательных муниципальных учреждений, сдали единый государственный экзамен, это является максимальным значением по городским округам, в тоже время в городе Касимов данный показатель составляет всего 62,4 процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Численность учащихся, приходящихся на одного учителя  в муниципальных общеобразовательных учреждениях максимальна в городе Сасово и составляет 15 учеников на одного учителя, а минимальна в городе Скопин – 10,7 учеников на одного учителя.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Средняя наполняемость классов в муниципальных общеобразовательных учреждениях в городских округах в городе Рязань составила 23 человека, минимальная наполняемость в городе Скопин 17,8 человек.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Образование (общее)» лучшим городским округом признан город Скопин, худшим в данной сфере признан город Касимов.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Физическая культура и спорт</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Максимальный охват населения систематическими занятиями физической культурой и спортом составляет в городе Скопин – 26 процентов от всего населения, а минимальный в городе Рязань – всего 10,5 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Физическая культура и спорт» лучшим городским округом признан город Скопин, худшим в данной сфере признан город Рязань.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Жилищно-коммунальное хозяйство</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля объема отпуска коммунальных ресурсов, счета за которые выставлены по показаниям приборов учета, что позволяет лучше учитывать использование отпускаемых ресурсов,  максимальна в городе Сасово и составляет 70 процентов, минимальна в городе Скопин, всего 10 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Уровень собираемости платежей за предоставленные жилищно-коммунальные услуги в городе Рязань составляет 98 процентов, в то же время в городе Скопин только 91 процент.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По всем городским округам процент подписанных паспортов готовности жилищного фонда и котельных (по состоянию на 15 ноября отчетного года) составляет 100 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Отношение тарифов для промышленных потребителей к тарифам для населения по водоснабжению максимальна в городе Рязань – 148 процентов, минимальная в городе Скопин 50,7 процента. Отношение тарифов для промышленных потребителей к тарифам для населения по водоотведению максимальна в городе Рязань – 135,8 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Жилищно-коммунальное хозяйство» лучшим городским округом признан город Рязань, худшим в данной сфере признан город Скопин.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Доступность и качество жилья</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В городе Скопин общая площадь жилых помещений, приходящаяся в среднем на одного жителя в сравнении с остальными городскими округами составляет максимальное значение 24,1 квадратных метра, минимальное значение по данному показателю в городе Сасово 21,9 квадратных метра. В то же время общая площадь жилых помещений, приходящаяся в среднем на одного жителя, введенная в действие за год в городе Рязань больше чем в остальных городских округах и составляет 0,76 квадратного метра на 1 жителя, минимальна в городе Скопин 0,05 квадратного метра на жителя.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля многоквартирных домов, расположенных на земельных участках, в отношении которых осуществлен государственный кадастровый учет максимальна в городе Скопин 10,5 процента, минимальна в городе Рязань 4,2 процента.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Доступность и качество жилья» лучшим городским округом признан город Рязань, худшим в данной сфере признан город Сасово.<br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Организация местного самоуправления</b><br/>
<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля собственных доходов местного бюджета (за исключением безвозмездных  поступлений, поступлений налоговых доходов по дополнительным нормативам отчислений и доходов от платных услуг, оказываемых муниципальными бюджетными учреждениями) в общем объеме доходов бюджета муниципального образования максимальна в городе Скопин  и составляет 65 процентов, минимальна в городе Сасово и составляет всего 28 процентов.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Охват населения для участия в культурно-досуговых мероприятиях, организованных органами местного самоуправления городских округов и муниципальных районов,  максимален в городе Касимов – 58 процентов от всего населения городского округа, минимален в городе Сасово – 31 процент от всего населения.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;По результатам оценки городских округов в сфере «Организация местного самоуправления» лучшим городским округом признан город Рязань, худшим в данной сфере признан город Сасово.<br/>
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
