<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.UFK_0014_0001._Default"  %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDateChooser.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Language" content="ru"/>
    <meta http-equiv="Content-Type" content="text/html; charset=windows-1251"/>
    <meta http-equiv="expires" content="0"/>
    <title>Структура поступлений по территориям и плательщикам</title>    
    <meta content="text/html; charset=windows-1251" http-equiv="Content-Type" />

    <script id="igClientScript" type="text/javascript">
<!--
  function getWidth() {
	  document.forms[0].screen_width.value = screen.availWidth;
	  document.forms[0].screen_height.value = screen.availHeight;	  
  }
  

        function RusMonths(monthNum)
        {
            switch (monthNum)
            {
                case 0:
                    return "Январь";
                case 1:
                    return "Февраль";
                case 2:
                    return "Март";
                case 3:
                    return "Апрель";
                case 4:
                    return "Май";
                case 5:
                    return "Июнь";
                case 6:
                    return "Июль";
                case 7:
                    return "Август";
                case 8:
                    return "Сентябрь";
                case 9:
                    return "Октябрь";
                case 10:
                    return "Ноябрь";
                case 11:
                    return "Декабрь";
                default:
                    return "Январь";
            }
        }
        
        function RusMonthGenitive(monthNum)
        {
            switch (monthNum)
            {
                case 0:
                    return "Января";
                case 1:
                    return "Февраля";
                case 2:
                    return "Марта";
                case 3:
                    return "Апреля";
                case 4:
                    return "Мая";
                case 5:
                    return "Июня";
                case 6:
                    return "Июля";
                case 7:
                    return "Августа";
                case 8:
                    return "Сентября";
                case 9:
                    return "Октября";
                case 10:
                    return "Ноября";
                case 11:
                    return "Декабря";
                default:
                    return "Января";
            }
        }

        function QuarterNumByMothNum(MonthNum)
        {
            //странно работает
            //return (int)Math.Ceiling((double)(MothNum / 3));
            if (MonthNum <= 2) return 1;
            if (MonthNum <= 5) return 2;
            if (MonthNum <= 8) return 3;
            return 4;
        }

        function HalfYerNumByMonthNum(MonthNum)
        {
           if (MonthNum < 6)
           {
            return 1;
           }
           else
           {
            return 2;
           }             
        }        
        
        function PeriodDescr(Year, Month, Day, detailLevel)
        {
            switch (detailLevel)
            {
                case 1: //год
                    return Year + " год";
                    break;
                case 2: //полугодие
                    return HalfYerNumByMonthNum(Month) + " полугодие " + Year + " года"; 
                    break;
                case 3: //квартал
                    return QuarterNumByMothNum(Month) + " квартал " + Year + " года";
                    break;
                case 4: //месяц
                    return RusMonths(Month) + " " + Year + " года";
                    break;
                case 5: //день
                    return "";
                    //return string.Format("{0} {1} {2} г.", 
                    //    date.Day, RusMonthGenitive(date.Month), date.Year.ToString());
                    break;                    
                    
                default:
                    return "";
                    break;
            }
        }        

function UpdateCurTimeLabel()
{
    var chooser = igdrp_getComboById("date");
    if(chooser == null){return;} 
    var result = chooser.getValue();
    
    var detail = igcmbo_getComboById("detailmode");
    if (detail == null) {return;}
    
    var labStr = PeriodDescr(result.getFullYear(), result.getMonth(), result.getDate(), detail.getSelectedIndex() + 1);    
    document.getElementById("curtime").innerText = labStr;          
}

function HighlightButton()
{
    var sb = ig_getWebControlById("SubmitButton");
    if (sb == null) {return;}                                
                 
    //sb.setForeColorAt("Red", 0, false);
    sb.setBorderColorAt("Red", 0, false);
    sb.paint(true);
}

function date_CalendarMonthChanging(oCalendar, oDate, oEvent){

    var chooser = igdrp_getComboById("date");
    if(chooser == null){return;}                    
    var result = chooser.getValue();
    
    result.setFullYear(oDate.getFullYear(), oDate.getMonth(), result.getDate());
    chooser.setValue(result);
    UpdateCurTimeLabel(); 
    HighlightButton();       
}

function date_AfterCloseUp(oDateChooser, dropDownPanel, oEvent){
    var chooser = igdrp_getComboById("date");
    if(chooser == null){return;}                    
    UpdateCurTimeLabel(); 
    HighlightButton();       
}

function detailmode_AfterCloseUp(webComboId){

}

function detailmode_AfterSelectChange(webComboId){               
    UpdateCurTimeLabel();
    HighlightButton();
}

function detailmode_BeforeSelectChange(webComboId){
	//Add code to handle your event here.
}

function chart_ClientOnMouseClick(this_ref, row, column, value, row_label, column_label, evt_type, layer_id){
	//Add code to handle your event here.
	//alert("11111yes");
	//var area = document.getElementById("chartdata");
	//if (area == null) {alert("not"); return;}
	//area.innerText = column_label + " === " + row_label + row + column;
}

function chart_ClientOnShowTooltip(text, tooltip_ref){
	//Add code to handle your event here.
}

function SubmitButton_Click(oButton, oEvent){
	getWidth();
}

// -->
</script>
</head>
<body OnLoad="javascript:UpdateCurTimeLabel()" bottommargin="0" topmargin="5">
    <form id="form1" runat="server">
    <div>
    <div>
    <uc1:Header ID="Header1" runat="server" />
        <table height="100%" width="1" align="left" cellpadding="0" cellspacing="0">
            <tr>
                <td colspan="2" style="height: 1px">                    
                    
                    
                    
                    <font style="font-size: 8px; font-family: Verdana">
                        <asp:Label ID="Label1" runat="server" SkinID="PageTitle" Text="Структура поступлений по территориям и плательщикам"></asp:Label></font></td>
            
                               <td>
                    <br />
                    <nobr><a style="font-size: 10px; font-family: Verdana" href="../../reports/UFK_0014_0002/Default.aspx">Структура поступлений по плательщикам и кодам доходов</a></nobr>
               </td>
            </tr>
            <tr>
                <td colspan="3" style="height: 5px; display: inline;">                
                    <table id="TABLE1" style="width: 470px" border="0" cellpadding="0" cellspacing="0">
                        <tr valign="top">
                            <td style="width: 299px">
                                <b></b>
                                <asp:Label ID="Label2" runat="server" SkinID="ElementTitle" Text="Период:"></asp:Label>
                                <table style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; border-bottom: gray 1px solid; display: inline;" cellpadding="0" cellspacing="5">
                                <tr>
                                    <td style="display: inline; clear: none; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px; width: 274px;">
                                <table style="display: inline" cellpadding="0" cellspacing="0">
                                    <tr>
                            <td style="display: inline; height: 1px">
                                <igsch:WebDateChooser ID="date" runat="server" AllowNull="False" BorderColor="Silver"
                                    BorderStyle="Solid" Height="20px" Value="06/05/2008 16:27:51" Width="130px" Font-Names="Verdana" Font-Size="8pt" Format="Long" EnableAppStyling="False" StyleSetName="Office2007Blue">
                                    <CalendarLayout AllowNull="False" CellPadding="1" CellSpacing="1"
                                        ShowFooter="False" ShowGridLines="Both" ShowTitle="False" ChangeMonthToDateClicked="True" GridLineColor="White">
                                        <SelectedDayStyle Font-Bold="False" BackColor="LightSteelBlue" ForeColor="Black" />
                                        <DayHeaderStyle BackColor="#F3C46A" Font-Bold="False" BorderColor="Silver" Font-Names="Verdana" Font-Size="X-Small" />
                                        <DropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" Font-Bold="False" BorderWidth="1px" Font-Names="verdana" Font-Size="8pt">
                                        </DropDownStyle>
                                        <TitleStyle Font-Bold="False" BackColor="#E4ECF7" />
                                        <CalendarStyle BackColor="White" BorderColor="Gray" BorderStyle="Solid">
                                        </CalendarStyle>
                                        <OtherMonthDayStyle ForeColor="DimGray" />
                                        <DayStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                            Font-Names="verdana" Font-Size="X-Small" />
                                        <NextPrevStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                    </CalendarLayout>
                                    <ClientSideEvents CalendarMonthChanging="date_CalendarMonthChanging" AfterCloseUp="date_AfterCloseUp" />
                                </igsch:WebDateChooser>
                            </td>
                            <td>&nbsp;</td>
                            <td style="height: 1px">
                                <igcmbo:WebCombo ID="detailmode" runat="server" Height="20px" SelBackColor="White"
                                    SelForeColor="Black" TabIndex="1" Version="4.00" Width="130px" Font-Names="verdana" Font-Size="8pt" BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ForeColor="Black" EnableAppStyling="False">
                                    <Columns>
                                        <igtbl:UltraGridColumn Width="125px">
                                            <header caption="Уровень детализации" key="mode"></header>
                                            <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                                            <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                                font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                                        </igtbl:UltraGridColumn>
                                    </Columns>
                                    <ExpandEffects ShadowColor="LightGray" />
                                    <DropDownLayout ColHeadersVisible="No" DropdownHeight="106px"
                                        DropdownWidth="128px" RowHeightDefault="17px" Version="4.00" RowSelectors="NotSet" GridLines="None">
                                        <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Font-Size="X-Small" Height="106px" Width="128px">
                                        </FrameStyle>
                                        <RowStyle BorderColor="White" BorderStyle="Solid" Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px" BackColor="White" BorderWidth="1px">
                                            <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                                        </RowStyle>
                                        <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                        <RowSelectorStyle Font-Bold="False" BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Font-Names="verdana" Font-Size="X-Small">
                                            <BorderDetails StyleLeft="None" />
                                        </RowSelectorStyle>
                                        <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                                        </RowAlternateStyle>
                                    </DropDownLayout>
                                    <Rows>
                                        <igtbl:UltraGridRow Height="">
                                            <cells>
<igtbl:UltraGridCell Text="Год  "></igtbl:UltraGridCell>
</cells>
                                        </igtbl:UltraGridRow>
                                        <igtbl:UltraGridRow Height="">
                                            <cells>
<igtbl:UltraGridCell Text="Полугодие"></igtbl:UltraGridCell>
</cells>
                                        </igtbl:UltraGridRow>
                                        <igtbl:UltraGridRow Height="">
                                            <cells>
<igtbl:UltraGridCell Text="Квартал"></igtbl:UltraGridCell>
</cells>
                                        </igtbl:UltraGridRow>
                                        <igtbl:UltraGridRow Height="">
                                            <cells>
<igtbl:UltraGridCell Text="Месяц"></igtbl:UltraGridCell>
</cells>
                                        </igtbl:UltraGridRow>
                                        <igtbl:UltraGridRow Height="">
                                            <cells>
<igtbl:UltraGridCell Text="День"></igtbl:UltraGridCell>
</cells>
                                        </igtbl:UltraGridRow>
                                    </Rows>
                                    <ClientSideEvents AfterCloseUp="detailmode_AfterCloseUp" AfterSelectChange="detailmode_AfterSelectChange" BeforeSelectChange="detailmode_BeforeSelectChange" />
                                </igcmbo:WebCombo>
                            </td>
                                    
                                    </tr>
                                </table>
                                    
                                    </td>
                                </tr>
                                </table>
                                </td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td id="submittd" runat="server">
                                <font style="font-size: 12pt; font-family: verdana"><br /></font>
                                <igtxt:WebImageButton ID="SubmitButton" runat="server" Text="Обновить данные" ImageTextSpacing="2" Height="20px">
                                    <Appearance>
                                        <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"></Style>
                                    </Appearance>
                                    <PressedAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </PressedAppearance>
                                    <DisabledAppearance>
                                        <Style BorderColor="Control"></Style>
                                    </DisabledAppearance>
                                    <ClientSideEvents Click="SubmitButton_Click" />
                                    <HoverAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </HoverAppearance>
                                    <FocusAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </FocusAppearance>
                                </igtxt:WebImageButton>
                                
                            </td>
                            <td>
                            
    <input id="screen_height" type="text" runat="server" style="border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; color: white; visibility:hidden" name="screen_height"/>
    <input id="screen_width" type="text" runat="server" style="border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; color: white; visibility:hidden" name="screen_width" />                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr valign="top">
                <td>
                    <br />                    
                    <asp:Label ID="gridLabel" runat="server" Text="Поступления на (date) в тысячах рублей" Font-Bold="True" SkinID="ElementTitle"></asp:Label>
                    <igtbl:UltraWebGrid ID="MainTable" runat="server" EnableAppStyling="True" Height="300px"
                        OnActiveRowChange="MainTable_ActiveRowChange" OnInitializeLayout="MainTable_InitializeLayout"
                        StyleSetName="Office2007Blue" Width="500px" EnableTheming="True" OnDataBinding="MainTable_DataBinding" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="MainTable" RowHeightDefault="15px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                            StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical" ColWidthDefault="130px" NoDataMessage="Нет данных за указанный период">
                            <GroupByBox Hidden="True">
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
                            <FilterOptionsDefault AllowRowFiltering="OnClient">
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
                            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left" Wrap="True">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Window" BorderStyle="Solid" BorderWidth="1px"
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="300px" Width="500px" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False">
                                <Padding Bottom="0px" Left="0px" Right="0px" Top="0px" />
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </PagerStyle>
                            </Pager>
                            <AddNewBox>
                                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </BoxStyle>
                            </AddNewBox>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid></td>
                <td style="width: 7px; height: 1px;">
                </td>
                <td style="height: 1px; width: 357px;">
                    <table style="width: 300px" cellpadding="0" cellspacing="0" border="0">
                        <tr valign="top">
                            <td style="height: 342px">
                                <br />                                                                
                                    
                    

                                
                                <igmisc:WebAsyncRefreshPanel ID="ChartRefreshPanel" runat="server" TriggerControlIDs="MainTable">
                                
                                    <asp:Label ID="chartLabel" runat="server" Font-Bold="True" Text='Структура по плательщикам' Width="300px" SkinID="ElementTitle"></asp:Label>&nbsp;
                                    <asp:Label ID="NoChartData" runat="server" Font-Names="verdana" Font-Size="8pt" ForeColor="Gray"
                                        Text="&nbsp;&nbsp;Выделите район" Width="165px"></asp:Label>
                                    <igchart:UltraChart ID="chart" runat="server" BackgroundImageFileName=""  
                                        BorderWidth="0px" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                        Version="7.3" Width="300px" ChartType="PieChart3D">
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False" EnableFadingEffect="True" HotTrackingEnabled="False" />
                                        <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString=""
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
                                            <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                        VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Y2>
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                        VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                                                        VerticalAlign="Center" FormatString="">
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
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="Horizontal"
                                                        VerticalAlign="Center" FormatString="">
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
                                        <ClientSideEvents ClientOnMouseClick="chart_ClientOnMouseClick" ClientOnShowTooltip="chart_ClientOnShowTooltip" />
                                        <Border Thickness="0" />
                                        <Legend Location="Bottom" SpanPercentage="30" Visible="True"></Legend>
                                        <PieChart3D OthersCategoryPercent="2" OthersCategoryText="Прочие">
                                        </PieChart3D>
                                        <Data>
                                            <EmptyStyle Text="Пусто">
                                            </EmptyStyle>
                                        </Data>
                                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                                    </igchart:UltraChart>
                                        
                                        
                                        </igmisc:WebAsyncRefreshPanel>
                                        
                                        
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 744px; height: 1px">
                </td>
                <td style="width: 7px; height: 1px;">
                </td>
                <td style="height: 1px; width: 357px;">
                </td>
            </tr>
        </table>        
        </div>
  </div>
    </form>
</body>
</html>
