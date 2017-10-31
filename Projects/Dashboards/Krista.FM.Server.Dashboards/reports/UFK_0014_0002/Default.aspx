    <%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.UFK_0014_0002.Default"  %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebTab.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDateChooser.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>

<%@ Register Src="../../components/PeriodParameter.ascx" TagName="PeriodParameter" TagPrefix="uc1" %>
<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Language" content="ru"/>
    <meta http-equiv="Content-Type" content="text/html; charset=windows-1251"/>
    <meta http-equiv="expires" content="0"/>
    <title>Структура поступлений по плательщикам и кодам доходов</title>
    <style>
    <!--
        tr
        {
            	text-align:left;
	            vertical-align:top;

        }
        
        .thin
        {
            height: 1px;
            text-align:left;
	        vertical-align:top;            
        }
    -->
    </style>
    
    

    <script id="igClientScript" type="text/javascript">
<!--
  function getWidth() {
	  form1.screen_width.value = screen.availWidth;
	  form1.screen_height.value = screen.availHeight;	  
  }
  

function HighlightButton()
{
    var sb = ig_getWebControlById("panelParameters_SubmitButton");
    if (sb == null) { return; }                                
                 
    sb.setBorderColorAt("Red", 0, false);
    sb.paint(true);
}



function SubmitButton_Click(oButton, oEvent){
	getWidth();
}


function date_AfterCloseUp(oDateChooser, dropDownPanel, oEvent){
    HighlightButton();
    var chooser = igdrp_getComboById("date");
    if(chooser == null){return;}                          
}


function date_CalendarMonthChanging(oCalendar, oDate, oEvent){
    HighlightButton();
    igdrp_getComboById("date");
    if(chooser == null){return;}                    
    var result = chooser.getValue();
    
    result.setFullYear(oDate.getFullYear(), oDate.getMonth(), result.getDate());
    chooser.setValue(result);
           
}

function detailmode_AfterSelectChange(webComboId){
	 HighlightButton();
}
// -->
</script>
    
</head>
<body style="font-size: 9pt; font-family: verdana" bottommargin="0" topmargin="5">
    <form id="form1" runat="server">
    
    <table height="1" width="1" align="left" cellpadding="0" cellspacing="0">    
        <!-- Заголовок -->
        <tr>
            <td class="thin" colspan="1" style="height: 1px">
            <font style="font-size: 16px; font-family: Verdana">&nbsp;<asp:Label ID="Label1"
                runat="server" CssClass="PageTitle" Text="Структура поступлений по плательщикам и кодам доходов"></asp:Label></font></td>
            
                       
        </tr>

        <!-- Параметры -->
        <tr>
            <td class="thin" colspan="2">
                <igmisc:WebPanel ID="panelParameters" runat="server" EnableAppStyling="True" Height="82px"
                    StyleSetName="Office2007Blue" ToolTip="Параметры страницы" Width="99%" ExpandEffect="None">
                    <Header Text="Параметры страницы" TextAlignment="Left">
                    </Header>
                    <Template>
                        
                        <table height="1" cellpadding="2" cellspacing="0">
                            <tr>
                                <td style="height: 16px">
                                    <strong>
                                        <asp:Label ID="Label2" runat="server" CssClass="ParamChooseTitle" Text="Уровни бюджетов "></asp:Label></strong></td>
                                <td style="height: 16px">
                                    <asp:Label ID="Label3" runat="server" CssClass="ParamChooseTitle" Text="Районы"></asp:Label></td>
                                <td style="height: 16px">
                                    <asp:Label ID="Label4" runat="server" CssClass="ParamChooseTitle" Text="ОКВЭД "></asp:Label></td>
                                <td style="height: 16px">
                                    <asp:Label ID="Label5" runat="server" CssClass="ParamChooseTitle" Text="Период "></asp:Label></td>
                                <td/>
                            </tr>
                            
                            <tr>
                                                                                    
                                <td>
                                
        <table border="0" runat="server" id="m1"><tr><td style="width: 200px" runat="server" id="dtBudgetLevelContainer">        
                                    <uc2:DimensionTree ID="dtBudgetLevel" runat="server" CubeName="УФК_Выписка из сводного реестра_c расщеплением" DefaultMember="[Уровни бюджетов].[Все уровни]" HierarchyName="[Уровни бюджетов]" MultipleChoice="true" ProviderKind="Primary"/>               
        </td></tr></table>
                                
                                

                                </td>
                                <td>
        <table border="0" runat="server" id="m2"><tr><td style="width: 200px" runat="server" id="dtRegionsContainer">                                        
                                    <uc2:DimensionTree ID="dtRegions" runat="server" CubeName="УФК_Выписка из сводного реестра_c расщеплением" DefaultMember="[Районы].[Сопоставимый].[Все районы]" HierarchyName="[Районы].[Сопоставимый]" MultipleChoice="true" ProviderKind="Primary" />
        </td></tr></table>                                    
                                </td>
                                <td>
        <table border="0" runat="server" id="m3"><tr><td style="width: 200px" runat="server" id="dtOKVDContainer">                                        
                                    <uc2:DimensionTree ID="dtOKVD" runat="server" CubeName="УФК_Выписка из сводного реестра_c расщеплением" DefaultMember="[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД]" HierarchyName="[ОКВЭД].[Сопоставимый]" MultipleChoice="true" ProviderKind="Primary" />
        </td></tr></table>                                    
                                </td>
                                <td>
                                    <table cellpadding="0" cellspacing="5">
                                        <tr>
                                            <td style="clear: none; padding-right: 0px; display: inline; padding-left: 0px; padding-bottom: 0px;
                                                margin: 0px; padding-top: 0px">
                                                <table border="0" height="100%" cellpadding="4" cellspacing="0" style="display: inline">
                                                <tr>
                                                <td>
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
                                    <ClientSideEvents AfterCloseUp="date_AfterCloseUp" CalendarMonthChanging="date_CalendarMonthChanging" />
                                </igsch:WebDateChooser>                                                
                                                </td>
                                                </tr>
                                                <tr>
                                                <td>
                                                            <igcmbo:WebCombo ID="detailmode" runat="server" BackColor="White" BorderColor="Silver"
                                                                BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                                                                Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                                                                StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="130px">
                                                                <Columns>
                                                                    <igtbl:UltraGridColumn Width="125px">
                                                                        <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                                                                        <header caption="Уровень детализации" key="mode"></header>
                                                                        <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                                                            font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                                                                    </igtbl:UltraGridColumn>
                                                                </Columns>
                                                                <ExpandEffects ShadowColor="LightGray" />
                                                                <DropDownLayout ColHeadersVisible="No" DropdownHeight="106px" DropdownWidth="128px"
                                                                    GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                                                                    XmlLoadOnDemandType="Synchronous">
                                                                    <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                                        Font-Size="X-Small" Height="106px" Width="128px">
                                                                    </FrameStyle>
                                                                    <RowStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px"
                                                                        Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px">
                                                                        <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                                                                    </RowStyle>
                                                                    <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid"
                                                                        BorderWidth="1px" />
                                                                    <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                                        Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                                                                        <BorderDetails StyleLeft="None" />
                                                                    </RowSelectorStyle>
                                                                    <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                                                                    </RowAlternateStyle>
                                                                </DropDownLayout>
                                                                <Rows>
                                                                    <igtbl:UltraGridRow Height="">
                                                                        <cells>
<igtbl:UltraGridCell Text="Год"></igtbl:UltraGridCell>
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
                                                                <ClientSideEvents BeforeSelectChange="detailmode_AfterSelectChange" />
                                                            </igcmbo:WebCombo>                                                
                                                </td>
                                                </tr>
                                                
                                                <tr>
                                                <td style="height: 15px">
                                                </td>
                                                </tr>
                                                <tr>
                                                <td>
                                                    &nbsp;</td>
                                                </tr>
                                                
                                                <tr valign="bottom">
                                                <td>
                                                    <br />
                                                    <input id="screen_width" runat="server" name="screen_width" style="visibility: hidden;
                                                        width: 33px; color: white; border-top-style: none; border-right-style: none;
                                                        border-left-style: none; height: 3px; border-bottom-style: none" type="text" /><input
                                                            id="screen_height" runat="server" name="screen_height" style="visibility: hidden;
                                                            width: 34px; color: white; border-top-style: none; border-right-style: none;
                                                            border-left-style: none; height: 2px; border-bottom-style: none" type="text" /><br />
                                                    <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2" Text="Обновить данные">
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
                                    <ClientSideEvents Click="SubmitButton_Click"  />
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
                                                </tr>
                                                
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                    
                                </td> 
                                <td />                                                               
                            </tr>                            
                        </table>
                        
                        
                    </Template>
                </igmisc:WebPanel>
            </td>
        </tr>
        
        <!-- Элементы -->
        <tr>
            <td class="thin">
                <igtbl:ultrawebgrid id="MasterTable" runat="server" enableappstyling="True" enabletheming="True"
                    height="349px" stylesetname="Office2007Blue" width="405px" OnDataBinding="MasterTable_DataBinding" OnInitializeLayout="MasterTable_InitializeLayout" OnActiveRowChange="MasterTable_ActiveRowChange" SkinID="UltraWebGrid"><Bands>
<igtbl:UltraGridBand>
<AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="Hierarchical" Version="4.00" AllowSortingDefault="OnClient" NoDataMessage="Нет данных за указанный период" StationaryMargins="Header" AllowColSizingDefault="Free" StationaryMarginsOutlookGroupBy="True" CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="MasterTable" BorderCollapseDefault="Separate" ColWidthDefault="130px" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="15px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
<GroupByBox Hidden="True">
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

<FilterOptionsDefault AllowRowFiltering="OnClient">
<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterOperandDropDownStyle>

<FilterHighlightRowStyle ForeColor="White" BackColor="#151C55"></FilterHighlightRowStyle>

<FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterDropDownStyle>
</FilterOptionsDefault>

<HeaderStyleDefault Wrap="True" HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyleDefault>

<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

<FrameStyle BorderWidth="1px" BorderStyle="Solid" Font-Italic="False" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" BackColor="Window" Width="405px" Height="349px">
<Padding Top="0px" Left="0px" Bottom="0px" Right="0px"></Padding>
</FrameStyle>

<Pager MinimumPagesForDisplay="2">
<PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</PagerStyle>
</Pager>

<AddNewBox>
<BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</BoxStyle>
</AddNewBox>
</DisplayLayout>
</igtbl:ultrawebgrid>
                
            </td>
            
            <td class="thin" style="width: 258px">
                <igmisc:WebAsyncRefreshPanel ID="refreshPanel" runat="server" TriggerControlIDs="MasterTable">                    
                    <igtab:UltraWebTab ID="detailTabs" runat="server" EnableAppStyling="True"
                        StyleSetName="Office2007Blue" AsyncMode="Off" OnTabClick="detailTabs_TabClick" SelectedTab="2">
                        <Tabs>
                            <igtab:Tab Text="Таблица">
                                <ContentTemplate>
                <igtbl:ultrawebgrid id="DetailTable" runat="server" enableappstyling="True" enabletheming="False"
                    height="158px" stylesetname="Office2007Blue" width="380px" OnDataBinding="DetailTable_DataBinding" OnInitializeLayout="DetailTable_InitializeLayout" EnableViewState="False" SkinID="UltraWebGrid"><Bands>
<igtbl:UltraGridBand>
<AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>

<DisplayLayout Version="4.00" AllowSortingDefault="OnClient" NoDataMessage="Нет данных за указанный период" StationaryMargins="Header" AllowColSizingDefault="Free" StationaryMarginsOutlookGroupBy="True" CellClickActionDefault="NotSet" HeaderClickActionDefault="SortMulti" Name="DetailTable" BorderCollapseDefault="Separate" ColWidthDefault="130px" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="15px">
<GroupByBox>
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

<FilterOptionsDefault AllowRowFiltering="OnClient">
<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterOperandDropDownStyle>

<FilterHighlightRowStyle ForeColor="White" BackColor="#151C55"></FilterHighlightRowStyle>

<FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterDropDownStyle>
</FilterOptionsDefault>

<HeaderStyleDefault Wrap="True" HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyleDefault>

<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

<FrameStyle BorderWidth="1px" BorderStyle="Solid" Font-Italic="False" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" BackColor="Window" Width="380px" Height="158px">
<Padding Top="0px" Left="0px" Bottom="0px" Right="0px"></Padding>
</FrameStyle>

<Pager MinimumPagesForDisplay="2">
<PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</PagerStyle>
</Pager>

<AddNewBox>
<BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</BoxStyle>
</AddNewBox>
</DisplayLayout>
</igtbl:ultrawebgrid>
                                </ContentTemplate>
                            </igtab:Tab>
                            <igtab:Tab Text="Диаграмма">
                                <ContentTemplate>
                <igchart:ultrachart id="DetailChart" runat="server" backgroundimagefilename=""  
                    borderwidth="0px" charttype="PieChart3D" emptycharttext="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    height="153px" version="7.3" width="388px" OnDataBinding="DetailChart_DataBinding">
<Tooltips EnableFadingEffect="True" Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False" HotTrackingEnabled="False"></Tooltips>

<Border Thickness="0"></Border>

<Data>
<EmptyStyle Text="Пусто"></EmptyStyle>
</Data>

<PieChart3D OthersCategoryText="Прочие" OthersCategoryPercent="2"></PieChart3D>

<ColorModel ColorEnd="DarkRed" AlphaLevel="255" ColorBegin="Pink"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="10" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="10" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>

<Legend SpanPercentage="30" Visible="True" Location="Bottom"></Legend>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_ufk14_002_#SEQNUM(100).png" />
</igchart:ultrachart>
                                </ContentTemplate>
                            </igtab:Tab>
                            <igtab:Tab Text="Информация по плательщику">
                                <ContentTemplate>
                                    <table border="0" cellpadding="5" runat="server" id="PayerInfoContainer">
                                        <tr>
                                            <td align="left" style="height: 23px; width: 249px;" valign="top">
                                                <font id="chartdata" runat="server" style="font-size: 10pt; font-family: verdana">нет данных</font>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </igtab:Tab>
                        </Tabs>
                    </igtab:UltraWebTab>
                </igmisc:WebAsyncRefreshPanel>
<br />


</td>
            
        </tr>
        
    </table>
    
    
    </form>
</body>
</html>
