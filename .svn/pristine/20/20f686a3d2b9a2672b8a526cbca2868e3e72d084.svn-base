<%@ Page Language="C#" title="Инвестиционный паспорт (Новоорский муниципальный район)" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_00060.EO_0000._default" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>



<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
   
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br />
<br>
    <igmisc:WebPanel ID="WebPanel1" runat="server" BackColor="White" EnableAppStyling="True"
        ExpandEffect="None" StyleSetName="Office2007Blue" Width="100%" EnableTheming="False" EnableViewState="False" OnExpandedStateChanged="WebPanel1_ExpandedStateChanged" OnExpandedStateChanging="WebPanel1_ExpandedStateChanging">
        <PanelStyle BackColor="White" Font-Size="12pt">
            <Padding Bottom="3px" Left="3px" Right="3px" Top="3px" />
        </PanelStyle>
        <AutoPostBack ExpandedStateChanged="False" ExpandedStateChanging="False" />
        <Header Text="ОБЩИЕ СВЕДЕНИЯ" TextAlignment="Left">
            <ExpandedAppearance>
                <Styles BackColor="White" Font-Bold="True" Font-Names="Arial" Font-Overline="False"
                    Font-Size="Small" Font-Strikeout="False" Font-Underline="False">
                </Styles>
            </ExpandedAppearance>
        </Header>
        <Template>
            Новоорский район образован в <b>1935</b> году. <br>Численность населения - <b>33,8</b> тыс. человек.<br>Удаленность от областного центра составляет <b>330</b> км.
<br><br><b>Район граничит</b>: 
<br>- на севере с Кваркенским, 
<br>- на северо-востоке с Адамовским, 
<br>- на юге с Ясненским и Домбаровским районами, 
<br>- на западе с Гайским районом, 
<br>- на юго-западе с г. Орском.

<br><br><b>В состав района входит</b> 9 муниципальных образований: 
<br>- Энергетикский поссовет; 
<br>- Новоорский поссовет; 
<br>- Приреченский сельсовет; 
<br>- Чапаевский сельсовет; 
<br>- Кумакский сельсовет; 
<br>- Добровольский сельсовет; 
<br>- Караганский сельсовет; 
<br>- Горьковский сельсовет; 
<br>- Будамшинский сельсовет.

<br><br>На территории Новоорского муниципального района находится <b>крупнейшее</b> в Оренбургской области <b>Ириклинское водохранилище</b>.

<br><br><b>Климат</b> характеризуется <b>континентальностью</b>, что объясняется значительной удаленностью области от океанов и морей: 
<br>- большая амплитуда колебаний средних температур воздуха (до <b>34 - 38</b> ºС); 
<br>- недостаточность атмосферных осадков (около <b>350 мм за год</b>).
<br>Около <b>60—70%</b> годового количества <b>осадков</b> приходится <b>на теплый период</b>.
<br>Продолжительность <b>залегания снегового покрова</b> составляет от <b>135 дней</b>.

<br><br><b>Основные полезные ископаемые</b> и другие природные ресурсы:
<br>- медно-цинковые руды (месторождение - «Барсучий лог»);
<br>- разнообразные виды глины (месторождения - Кумакское, Новоорское, Приморское);
<br>- строительный камень (Мусогоатское, Новоорское, Ульяновское);
<br>- песчано-гравийные смеси (Озёрное, Оринское и др.);
<br>- известняки (Ириклинское).

<br><br><b>Вредные производства</b> - ОАО «ОГК-1» филиал Ириклинская ГРЭС.

<br><br><b>Туристический потенциал:</b> 
<br>- спортивный комплекс «Дельфин» с плавательным бассейном,саунами и спортивными залами; 
<br>- оздоровительный комплекс санаторий-профилакторий «Лукоморье».

<br>
<br><b>Среднегодовая численность населения</b> в <b>2009</b> году составила <b>31600</b> чел. <b>Доля экономически активного населения</b> (<b>19650</b> чел.) в среднегодовой численности равна <b>0,62</b>.
<br>В <b>2008</b> году среднегодовая численность населения составила <b>31615</b> чел., численность экономически активного населения – <b>19650</b> чел.
<br>В <b>2007</b> году среднегодовая численность населения равна <b>31900</b> чел., численность экономически активного населения – <b>19750</b> чел.

<br><br><b>Коэффициент рождаемости</b> (в расчете на 1000 человек населения) в <b>2009</b> году составил <b>14,1</b>. По сравнению с 2008 годом данный коэффициент не изменился. Темп прироста <img src="../../../../images/ArrowUpGreen.gif" /> 
    коэффициента по сравнению с 2007 годом составил 4,44% (+0,6).

<br><br><b>Уровень безработицы</b> за <b>2009</b> год увеличился <img src="../../../../images/ArrowUpRed.gif" /> на 0,7% и составил <b>2,1%</b> (темп прироста - 425%).
<br>
<br>В <b>2009</b> году заработная плата (по средним и крупным предприятиям) увеличилась <img src="../../../../images/ArrowUpGreen.gif" />  на <b>5</b><strong>%</strong>, и к концу года она составила <b>15322</b> руб. В <b>2008</b> году темп прироста <img src="../../../../images/ArrowUpGreen.gif" />  заработной платы составил <b>19,99</b><strong>%</strong> В <b>2007</b> году среднемесячный размер заработной платы (по средним и крупным предприятиям) был равен <b>12161,9</b> руб., к концу <b>2008</b> года он увеличился до <b>14593</b> руб.
<br>По состоянию на конец <b>2009</b> года в <b>среднем на одного жителя</b> приходится <b>32,02</b> кв. м <b>жилых помещений</b>, что на <b>0,38</b> кв. м больше <img src="../../../../images/ArrowUpGreen.gif" />, чем в 2008 году (темп прироста <b>1,2</b><strong>%</strong>). В <b>2008</b> году площадь жилых помещений (на одного жителя) увеличилась <img src="../../../../images/ArrowUpGreen.gif" />  в 1,5 раза (с <b>21,09</b> кв. м в <b>2007</b> году до <b>31,64</b> кв. м к концу <b>2008</b> года).
<br>
        </Template>
    </igmisc:WebPanel>
    <br />
    <igmisc:WebPanel ID="WebPanel2" runat="server" BackColor="White" EnableAppStyling="True"
        ExpandEffect="None" StyleSetName="Office2007Blue" Width="100%" OnExpandedStateChanging="WebPanel2_ExpandedStateChanging">
        <PanelStyle BackColor="White" Font-Size="12pt">
            <Padding Bottom="3px" Left="3px" Right="3px" Top="3px" />
        </PanelStyle>
        <AutoPostBack ExpandedStateChanged="False" ExpandedStateChanging="False" />
        <Header Text="ЭКОНОМИКА" TextAlignment="Left">
            <ExpandedAppearance>
                <Styles BackColor="White" Font-Bold="True" Font-Names="Arial" Font-Overline="False"
                    Font-Size="Small" Font-Strikeout="False" Font-Underline="False">
                </Styles>
            </ExpandedAppearance>
        </Header>
        <Template>
            <strong>Основная отрасль промышленности</strong> в муниципальном образовании – <b>энергетика</b>.
            <br />
            Крупнейшим предприятием МО, занимающимся выработкой электроэнергии, является ОАО
            «ОГК-1» филиал Ириклинская ГРЭС.
            <br />
            <br />
            Перечень наиболее крупных предприятий
            <br />
            <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" EnableAppStyling="True" OnDataBinding="UltraWebGrid1_DataBinding" OnInitializeLayout="UltraWebGrid1_InitializeLayout" SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                    HeaderClickActionDefault="SortMulti" Name="Grid" RowHeightDefault="20px" RowSelectorsDefault="No"
                    SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                    TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                        Width="325px">
                    </FrameStyle>
                    <Pager MinimumPagesForDisplay="2">
                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </PagerStyle>
                    </Pager>
                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                    </EditCellStyleDefault>
                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </FooterStyleDefault>
                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                        <Padding Left="3px" />
                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                    </RowStyleDefault>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <GroupByBox>
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <AddNewBox Hidden="False">
                        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </BoxStyle>
                    </AddNewBox>
                    <ActivationObject BorderColor="" BorderWidth="">
                    </ActivationObject>
                    <FilterOptionsDefault>
                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px" Height="300px" Width="200px">
                            <Padding Left="2px" />
                        </FilterDropDownStyle>
                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                        </FilterHighlightRowStyle>
                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px">
                            <Padding Left="2px" />
                        </FilterOperandDropDownStyle>
                    </FilterOptionsDefault>
                </DisplayLayout>
            </igtbl:UltraWebGrid><br />
            <strong>В <b>2009</b> году индекс промышленного производства составил <b>98,56%</b></strong>.
            По сравнению с 2008 годом он снизился
            <img src="../../../../images/ArrowDownRed.gif" /><strong> на 6,74% (в <b>2008</b></strong>
            году - <b>105,3%</b>) и на 14,24% по сравнению с 2007 годом (в <b>2007</b> году
            - <b>112,8%</b>).
            <br />
           
            <br />
            <b>Инвестиции в основной капитал</b> в <b>2009</b> году составили <b>1024</b> млн.
            руб. Это на <b>708,3</b> млн. руб. ниже
            <img src="../../../../images/ArrowDownRed.gif" /><strong> , чем в 2008 году (темп прироста
                равен <b>-40,89</b>%).
                <br />
            </strong>В <b>2008</b> году инвестиции составили <b>1732,3</b> млн. руб. - по сравнению
            с <b>2007</b> годом (<b>1113</b> млн. руб.) темп их прироста
            <img src="../../../../images/ArrowUpGreen.gif" />
            составил <b>55,64</b><strong>%</strong>
            <br />
            <b>Индекс физического объема инвестиций в основной капитал</b> в <b>2009</b> году
            снизился
            <img src="../../../../images/ArrowDownRed.gif" />
            по сравнению с 2008 годом на 75,6% и составил <b>56,4%</b>. В <b>2008</b> году
            он составил <b>132%</b>, что на <b>30</b><strong>%</strong> ниже, чем в <b>2007</b> году (<b>162%</b>).
            <br />
            В <b>2009</b> году по сравнению с 2008 годом снизились
            <img src="../../../../images/ArrowDownRed.gif" /><strong> объемы <b>ввода жилых домов</b>
                (темп прироста составил <b>-27,3</b>%). В <b>2007</b> и <b>2008</b> годах, наоборот,
                наблюдался рост
                <img src="../../../../images/ArrowUpGreen.gif" /></strong> объемов ввода жилых
            домов (темп прироста - <b>10,2</b><strong>%</strong> и <b>17</b><strong>%</strong>
            соответственно).
            <br />
            <br />
            В муниципальном образовании присутствуют три основные площадки <b> под реализацию инвестиционных проектов:</b><br />
                - Новоорский кирпичный завод;<br />
                - Дробильно-сортировочный завод по переработке гранита;<br />
                - Новоорский щебеночный завод им. Ф.С.Сибагатуллина.<br />
            <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" OnDataBinding="Grid_DataBinding" OnInitializeLayout="Grid_InitializeLayout" SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                    HeaderClickActionDefault="SortMulti" Name="Grid" RowHeightDefault="20px" RowSelectorsDefault="No"
                    SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                    TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                        Width="325px">
                    </FrameStyle>
                    <Pager MinimumPagesForDisplay="2">
                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </PagerStyle>
                    </Pager>
                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                    </EditCellStyleDefault>
                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </FooterStyleDefault>
                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                        <Padding Left="3px" />
                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                    </RowStyleDefault>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <GroupByBox>
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <AddNewBox Hidden="False">
                        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </BoxStyle>
                    </AddNewBox>
                    <ActivationObject BorderColor="" BorderWidth="">
                    </ActivationObject>
                    <FilterOptionsDefault>
                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px" Height="300px" Width="200px">
                            <Padding Left="2px" />
                        </FilterDropDownStyle>
                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                        </FilterHighlightRowStyle>
                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px">
                            <Padding Left="2px" />
                        </FilterOperandDropDownStyle>
                    </FilterOptionsDefault>
                </DisplayLayout>
            </igtbl:UltraWebGrid>&nbsp;
            <strong>
                <br />
                <b>Малое предпринимательство</b>
                <br />
                <b>Количество малых предприятий:</b></strong> в <b>2009</b> году – <b>145</b>
            единиц; &nbsp;в <b>2008</b> году – <b>145</b> единиц;&nbsp; в <b>2007</b> году –
            <b>140</b> единиц.
            <br />
            <b>Оборот малых предприятий:</b>
            <br />
            <strong>- в <b>2009</b> году – <b>725</b></strong> млн. руб. (темп прироста
            <img src="../../../../images/ArrowUpGreen.gif" /><strong> по сравнению с 2008 годом
                – 5,95%);
                <br />
            </strong>- в <b>2008</b> году – <b>684,3</b> млн. руб. (темп прироста
            <img src="../../../../images/ArrowUpGreen.gif" /><strong> по сравнению с 2007 годом
                – 62,77%);
                <br />
            </strong>- в <b>2007</b> году – <b>420,4</b> млн. руб.
            <br />
            <b>Численность занятых в малом бизнесе</b> по состоянию на <b>2009</b> год составила
            <b>1,97</b> тыс. человек, что на 0,14 тыс. человек больше
            <img src="../../../../images/ArrowUpGreen.gif" />
            , чем в 2008 году. В 2008 году численность занятых в малом бизнесе выросла
            <img src="../../../../images/ArrowUpGreen.gif" />
            по сравнению с 2007 году на 0,06 тыс. человек (с <b>1,77</b> тыс. человек в <b>2007</b>
            году до <b>1,83</b> тыс.человек в <b>2008</b> году).
            <br />
            <br />
            <b>Инфраструктура поддержки предпринимательства:</b>
            <br />
            - разработана и утверждена <b>программа «О развитии малого и среднего предпринимательства
                в муниципальном образовании Новоорский район на 2009-2011 годы»</b>; в целях
            реализации мероприятий, предусмотренных вышеуказанной программой в <b>2009</b> году
            из районного бюджета было выделено <b>500</b> тыс. рублей на субсидирование процентных
            ставок по кредитам, получаемым субъектами МСП в кредитных организациях;
            <br />
            - при администрации МО «Новоорский район» был создан <b>Общественный Совет предпринимателей</b>,
            в состав которого, вошло 12 предпринимателей муниципального образования Новоорский
            район, были рассмотрены вопросы о штрафах на энергоносители (за недоиспользование
            и превышение заявленных мощностей) и по многочисленным проверкам федеральных служб;
            <br />
            - в МО «Энергетикский поссовет» создано муниципальное автономное учреждение <b>«Центр
                по поддержке и развитию малого и среднего предпринимательства»</b>, который
            успешно работает по одному из мероприятий программы «По стабилизации ситуации на
            рынке труда» - содействие развитию малого предпринимательства и самозанятости безработных
            граждан;
            <br />
            - совместно с Центром занятости населения Новоорского района в районе оказано <b>содействие
                в открытии собственного дела 33</b> начинающим предпринимателям. Полученные
            субсидии в основном направлены на производство сельскохозяйственной продукции (26
            человек), оказание бытовых услуг (4 человека);
            <br />
            - в рамках реализации национального проекта <b>«Развитие АПК»</b> получили кредиты:
            <b>26</b> ЛПХ на сумму – <b>2927</b> тыс. рублей и <b>11</b> КФХ – на сумму <b>25396</b>
            тыс. руб.
            <br />
            - за счет средств областного бюджета, выделенных в соответствии с Постановлением
            Правительства Оренбургской области, 5 предпринимателей получили <b>гранты на создание
                и развитие собственного бизнеса</b>, на общую сумму <b>920</b> тыс. рублей.
            <br />
        </Template>
    </igmisc:WebPanel>
    <br />
    <igmisc:WebPanel ID="WebPanel3" runat="server" BackColor="White" EnableAppStyling="True"
        ExpandEffect="None" StyleSetName="Office2007Blue" Width="100%" EnableTheming="True" OnExpandedStateChanging="WebPanel3_ExpandedStateChanging">
        <PanelStyle BackColor="White" Font-Size="12pt">
            <Padding Bottom="3px" Left="3px" Right="3px" Top="3px" />
        </PanelStyle>
        <AutoPostBack ExpandedStateChanged="False" ExpandedStateChanging="False" />
        <Header Text="ИНФРАСТРУКТУРА" TextAlignment="Left">
            <ExpandedAppearance>
                <Styles BackColor="White" Font-Bold="True" Font-Names="Arial" Font-Overline="False"
                    Font-Size="Small" Font-Strikeout="False" Font-Underline="False">
                </Styles>
            </ExpandedAppearance>
        </Header>
        <Template>
            <strong>Уровень газификации</strong> (в 2007-2009 годах) – <b>98%</b>.

<br><br><b>Свободные мощности:</b>
 <br>- в теплоснабжении: котельная п.Гранитный - 10%, котельная ОАО «НОЭМЗ» - 10%;
 <br>- в электроснабжении: Ириклинская ГРЭС.
<br>Суммарная мощность Ириклинской  ГРЭС - 2400 МВт (восемь турбоагрегатов по 300 МВт каждый). Отпуск электроэнергии осуществляется по высоковольтным линиям   напряжением 500 кВ, 220 кВ,  110 кВ.
<br><br><b>Транспортная система</b>
<br><b>Железнодорожная станция</b> Новоорск, относящаяся к Южно-Уральской железной дороге, находится на направлении «Орск-Челябинск», одном из крупнейших направлений, перевозящем как внутренние грузы, так и внешние (Средняя Азия, Китай и т.д.).
<br>На сегодняшний день протяженность всех <b>автомобильных дорог</b> общего пользования составляет <b>343,9</b> км:
 <br>- с капитальным типом покрытием - около <b>175</b> км;
 <br>- гравийных дорог – <b>157,9</b> км; 
 <br>- полевой дороги до Новосевостополя - <b>11</b> км. 
<br>Практически все население района имеет доступ к регулярному круглогодичному транспортному сообщению с административным центром муниципального района, региональная магистраль Оренбург-Шильда-Челябинск.
<br><b>Аэропорт «Орск»</b>, принимающий российские рейсы, находится в <b>45</b> км от п.Новоорск.
<br><b>Водных путей</b> нет.
<br><br><b>Связь</b>
<br>На территории Новоорского района действует одно предприятие связи - <b>ОАО «ВолгаТелеком»</b>.
<br><br><b>Здравоохранение</b>
<br>Здравоохранение района представлено <b>20 учреждениями</b>, в том числе:
 <br>- центральная районная больница (п. Новоорск);
 <br>- районная больница (п. Энергетик);
 <br>- 4 амбулатории;
 <br>- 14 фельдшерско-акушерских пункта (ФАП).
<br>На реализацию ПНП «Здоровье» из федерального бюджета получено в <b>2008</b> году <b>13,3</b> млн.руб.
<br><br><b>Образование, физическая культура и спорт</b>
<br>В районе функционирует <b>21 общеобразовательное учреждение</b>, в которых обучается <b>4194</b> человек:
 <br>- <b>7</b> основных школ;
 <br>- <b>13</b> средних школ;
 <br>- <b>1</b> лицей.
<br><b>Учреждения дополнительного образования для детей:</b>
 <br>- Детский центр;
 <br>- ДЮСШ;
 <br>- МДОЛ «Мечта».
<br>В настоящий момент действуют <b>17</b> детских дошкольных образовательных учреждений и <b>2</b> учреждения среднего профессионального образования с контингентом учащихся <b>503</b> человек.
<br><b>Высших учебных заведений</b> нет.
        </Template>
    </igmisc:WebPanel>
</asp:Content>