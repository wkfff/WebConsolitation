<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.UFK_0016_0001.Default"  %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Language" content="ru">
    <meta http-equiv="Content-Type" content="text/html; charset=windows-1251">
    <meta http-equiv="expires" content="0">
    <title>Результаты мониторинга БК и КУ</title>
    <meta content="text/html; charset=windows-1251" http-equiv="Content-Type" />

    
    <meta content="text/html; charset=windows-1251" http-equiv="Content-Type" />

    <script id="igClientScript" type="text/javascript">
<!--

function SubmitButton_Click(oButton, oEvent){
	  document.forms[0].screen_width.value = screen.availWidth;
	  document.forms[0].screen_height.value = screen.availHeight;	  
}

function ComboQuarter_AfterSelectChange(webComboId){
    var sb = ig_getWebControlById("SubmitButton");
    if (sb == null) {return;}                                
                 
    //sb.setForeColorAt("Red", 0, false);
    sb.setBorderColorAt("Red", 0, false);
    sb.paint(true);
}
// -->
</script>
</head>
<body  style="font-size: 9pt; font-family: verdana" bottommargin="0" topmargin="5">
    <form id="form1" runat="server">
    
        <table height="80%" align="left" cellpadding="0" cellspacing="0" style="width: 128px">
            <tr valign="top">
                <td colspan="2" style="height: 18px">
                    <font runat="server" id="PageTitle" style="font-size: 16px; font-family: Verdana; font-weight: bold;">
                        <uc1:Header ID="Header1" runat="server" />
                        <asp:Label ID="Label1" runat="server" Text="Результаты мониторинга БК и КУ в разрезе районов" CssClass="PageTitle"></asp:Label></font></tr>
            <tr valign="top" style="height: 1px">
                <td colspan="3" height="1" style="display: inline">
                    
                    <table style="width: 443px; display: inline;" height="1" border="0">
                        <tr>
                            <td style="height: 16px">
                                <asp:Label ID="Label2" runat="server" CssClass="ParamChooseTitle" Text="Год"></asp:Label></td>
                            <td style="width: 3px; height: 16px">
                                <asp:Label ID="Label3" runat="server" CssClass="ParamChooseTitle" Text="Label"></asp:Label></td>
                            <td style="width: 344px; height: 16px">
                            </td>
                            <td style="width: 344px; height: 16px">
                            </td>
                        </tr>
                        <tr>
                            <td>
                    <igcmbo:WebCombo ID="ComboYear" runat="server" BackColor="White" BorderColor="Silver"
                        BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                        Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                        StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="80px" SelectedIndex="9">
                        <Columns>
                            <igtbl:UltraGridColumn Width="30px">
                                <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                                <header caption="Уровень детализации" key="mode"></header>
                                <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                    font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                            </igtbl:UltraGridColumn>
                        </Columns>
                        <ExpandEffects ShadowColor="LightGray" />
                        <DropDownLayout ColHeadersVisible="No" DropdownHeight="275px" DropdownWidth="80px"
                            GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                            XmlLoadOnDemandType="Synchronous">
                            <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                            </RowAlternateStyle>
                            <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                                <BorderDetails StyleLeft="None" />
                            </RowSelectorStyle>
                            <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                Font-Size="X-Small" Height="275px" Width="80px">
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
                        </Rows>
                        <ClientSideEvents BeforeSelectChange="ComboQuarter_AfterSelectChange" />
                    </igcmbo:WebCombo>
                            </td>
                            <td style="width: 3px">
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
                    </igcmbo:WebCombo>
                            </td>
                            <td style="width: 344px" valign="top">
                                &nbsp;<igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2"
                                    Text="Обновить данные" OnClick="SubmitButton_Click">
                                    <DisabledAppearance>
                                        <Style BorderColor="Control"></Style>
                                    </DisabledAppearance>
                                    <Appearance>
                                        <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"></Style>
                                    </Appearance>
                                    <HoverAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </HoverAppearance>
                                    <FocusAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </FocusAppearance>
                                    <PressedAppearance>
                                        <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False"></Style>
                                    </PressedAppearance>
                                    <ClientSideEvents Click="SubmitButton_Click" />
                                </igtxt:WebImageButton>
                            </td>
                            <td style="width: 344px">
                                <input id="screen_width" runat="server" name="screen_width" style="visibility: hidden;
                                    color: white; border-top-style: none; border-right-style: none; border-left-style: none;
                                    border-bottom-style: none; height: 3px;" type="text" />
                                <input id="screen_height" runat="server" name="screen_height" style="visibility: hidden;
                                    color: white; border-top-style: none; border-right-style: none; border-left-style: none;
                                    border-bottom-style: none; height: 2px;" type="text" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr valign="top">
                <td colspan="3">
                    <!--<b>Количество нарушений БК и КУ и значения индикаторов</b>-->
                    <igtbl:UltraWebGrid ID="MasterTable" runat="server" EnableAppStyling="True" EnableTheming="True"
                        Height="166px" OnActiveCellChange="MasterTable_ActiveCellChange" OnDataBinding="MasterTable_DataBinding"
                        OnInitializeLayout="MasterTable_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="500px" OnActiveRowChange="MasterTable_ActiveRowChange" OnInitializeRow="MasterTable_InitializeRow" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" ColWidthDefault="130px"
                            HeaderClickActionDefault="SortMulti" Name="MasterTable" NoDataMessage="Нет данных за указанный период"
                            RowHeightDefault="15px" SelectTypeCellDefault="Extended" StationaryMargins="Header"
                            StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
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
                            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left"
                                Wrap="True">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Window" BorderStyle="Solid" BorderWidth="1px" Font-Italic="False"
                                Font-Names="Microsoft Sans Serif" Font-Overline="False" Font-Size="8.25pt" Font-Strikeout="False"
                                Font-Underline="False" Height="166px" Width="500px">
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
            </tr>
            <tr valign="top">
                <td colspan="3" style="height: 101px">
   <!--                <igmisc:WebAsyncRefreshPanel ID="DetailRefreshPanel" runat="server" TriggerControlIDs="MasterTable"> -->
                        
                        <igmisc:WebPanel ID="RowDetailPanel" runat="server" Height="83px" StyleSetName="" Width="517px">
                            <Template>
                                <br />
                                
                                <table align="left" cellpadding="0" cellspacing="0" width="100%">
                                    <tr valign="top">
                                        <td style="width: 158px">
                                            &nbsp;<nobr><asp:Label id="chartTitleLabel" runat="server" Text="Значение индикатора БК1" Width="220px" Font-Bold="True" CssClass="ElementTitle"></asp:Label></nobr></br>
                                            <igchart:UltraChart ID="chart" runat="server" BackgroundImageFileName=""  
                                                 EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Version="7.3" Height="123px" Width="231px" OnFillSceneGraph="chart_FillSceneGraph" OnInvalidDataReceived="chart_InvalidDataReceived">
                                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                    Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt; &lt;DATA_VALUE:0.00&gt;" />
                                                <ColorModel AlphaLevel="150" ColorBegin="DeepSkyBlue" ColorEnd="DeepSkyBlue" ModelStyle="CustomLinear">
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
                                                    <Y2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
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
                                                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="130">
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
                                                <ColumnChart>
                                                    <ChartText>
                                                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
                                                            Row="-2" VerticalAlign="Far" Visible="True">
                                                        </igchartprop:ChartTextAppearance>
                                                    </ChartText>
                                                </ColumnChart>
                                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo16_001_#SEQNUM(100).png" />
                                            </igchart:UltraChart>
                                        </td>
                                        <td style="font-size: 8pt; font-family: verdana; text-decoration: none">
                                            <asp:Label ID="descrTitleLabel" runat="server" Font-Bold="True" Text="Описание индикатора БК1"
                                                Width="224px" CssClass="ElementTitle"></asp:Label>
                                             <br />
                                             <br />
                                             
                                                        <b>Индикатор:&nbsp;</b>
                                                        <font runat="server" style="font-size: 8pt; font-family: verdana;" id="descrTD1">(Имя)</font>
                                                        <br />
                                                        
                                                        <b>Содержание индикатора:&nbsp;</b>
                                                        <font runat="server" style="font-size: 8pt; font-family: verdana;" id="descrTD2">(Содержание)</font>
                                                        <br />
                                                        
                                                        <b>Формула:&nbsp;</b>
                                                        <font runat="server" style="font-size: 8pt; font-family: verdana;" id="descrTD3">(Формула)</font>
                                                        <br />
                                                        
                                                        <b>Нормативное значение:&nbsp;</b>
                                                        <font runat="server" style="font-size: 8pt; font-family: verdana;" id="descrTD4">(Нормативное значение)</font>
                                             
                                                
                                            </td>
                                        
                                     </tr>
                                 </table>
                                
                            </Template>
                            <Header Visible="False">
                            </Header>
                        </igmisc:WebPanel>
                        
                        
                        <igmisc:WebPanel ID="ColumnDetailPanel" runat="server" Height="83px" StyleSetName="" Width="516px">
                            <Template>
                                <table cellpadding="0" cellspacing="0" border="0">
                                    <tr valign="top">
                                        <td align="left">
                                            <asp:Label ID="regionDetailLabel" runat="server" Text="(regionDetailLabel)" Font-Bold="True"></asp:Label>:&nbsp;
                                            Общее количество нарушений&nbsp;-&nbsp;
                                            <asp:Label ID="TotalVioletionCountLabel" runat="server" Text="(TotalVioletionCount)"></asp:Label>, группа&nbsp;МО -&nbsp;
                                            <asp:Label ID="GroupMO" runat="server" Text="(GroupMOlabel)"></asp:Label>
                                            <br /><br /> 
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>                                        
                                            <td>БК1<br />
                                                <igGauge:UltraGauge ID="GaugeBK1" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 10, 2, 10, Pixels" Orientation="Vertical">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>
                                            <td>БК2<br />
                                                <igGauge:UltraGauge ID="GaugeBK2" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>
                                            <td>БК3_обл<br />
                                                <igGauge:UltraGauge ID="GaugeBK3_obl" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК3_мест<br />
                                                <igGauge:UltraGauge ID="GaugeBK3_mest" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК4<br />
                                                <igGauge:UltraGauge ID="GaugeBK4" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК5<br />
                                                <igGauge:UltraGauge ID="GaugeBK5" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                              </td>                                                                                
                                            <td>БК6_a<br />
                                                <igGauge:UltraGauge ID="GaugeBK6_a" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="0" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК6_б<br />                                         
                                                <igGauge:UltraGauge ID="GaugeBK6_b" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="0" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                        </tr>
                                        <tr>                                        
                                            <td>БК6_в<br />                                         
                                                <igGauge:UltraGauge ID="GaugeBK6_v" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>
                                            <td>БК7<br />                                         
                                                <igGauge:UltraGauge ID="GaugeBK7" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>
                                            <td>БК8<br />                                         
                                                <igGauge:UltraGauge ID="GaugeBK8" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК9<br />
                                                <igGauge:UltraGauge ID="GaugeBK9" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="0" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК10<br />
                                                <igGauge:UltraGauge ID="GaugeBK10" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                    <DeploymentScenario Mode="Session" />
                                                    <Gauges>
                                                        <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                            <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                            <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                        </igGaugeProp:LinearGauge>
                                                    </Gauges>
                                                </igGauge:UltraGauge>
                                            </td>                                                                                
                                            <td>БК11<br />
                                                <igGauge:UltraGauge ID="GaugeBK11" runat="server" BackColor="Transparent" Height="100px"
                                                    Width="58px">
                                                <DeploymentScenario Mode="Session" />
                                                <Gauges>
                                                    <igGaugeProp:LinearGauge CornerExtent="10" Orientation="Vertical" MarginString="2, 10, 2, 10, Pixels">
                                                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="25">
<MinorTickmarks EndExtent="55" Frequency="0.2" StartExtent="45">
<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker BulbSpan="35" SegmentSpan="1" ValueString="58" StartExtent="-13"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="200, 178, 34, 34" StartColor="200, 255, 0, 0" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeNeedle MidWidth="1" EndWidth="5" MidExtent="18" EndExtent="18" ValueString="1" StartWidth="1"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Blue"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Blue"></StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="60" StartExtent="40" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="2" TickmarkInterval="0.5"></igGaugeProp:NumericAxis>
</Axes>

<Labels ZPosition="AboveMarkers" Extent="25" FormatString="&lt;DATA_VALUE:0.##&gt;" Font="Microsoft Sans Serif, 10pt"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow Depth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                                                        <brushelements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</brushelements>
                                                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                                                    </igGaugeProp:LinearGauge>
                                                </Gauges>
                                            </igGauge:UltraGauge>
                                            </td>                                                                                
                                        </tr>                                        
                                        </table>
                                    </td>
                                    </tr>
                                                                                                            
                                </table>

                                
                            </Template>
                            <Header Visible="False">
                            </Header>
                        </igmisc:WebPanel>
                 <!--   </igmisc:WebAsyncRefreshPanel> -->
                </td>
            </tr>
        </table>
    
    
    </form>
</body>
</html>
