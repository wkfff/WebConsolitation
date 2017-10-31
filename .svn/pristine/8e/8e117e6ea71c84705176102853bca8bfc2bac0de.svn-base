<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FST_0001_0003_Horizontal.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FST_0001_0003_Horizontal" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../iPadBricks/FST_0001_0003_Horizontal_Text.ascx" TagName="FST_0001_0003_Horizontal_Text"
    TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: Black">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/FST_0001_0003_Horizontal/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 1022px; height: 702px; top: 0px; left: 0px;
        z-index: 2; overflow: hidden;">
        <table>
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Электроснабжение"
                        Width="100%" />
                    <div style="margin-top: -5px">
                        <uc2:FST_0001_0003_Horizontal_Text ID="TagCloudFST_0001_0003_Horizontal_Text1" runat="server" />
                    </div>
                </td>
                <td rowspan="3" valign=top>
                    <div>
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Регулируемые организации<br/>в 2011 году" Width="100%"/>
                        <igtbl:UltraWebGrid ID="OutcomesGrid" runat="server" Height="200px" Width="509px"
                            OnDataBinding="OutcomesGrid_DataBinding" OnInitializeLayout="OutcomesGrid_InitializeLayout"
                            SkinID="UltraWebGrid" OnInitializeRow="OutcomesGrid_InitializeRow">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
                                BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                SelectTypeRowDefault="Extended">
                                <GroupByBox>
                                    <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                    </BoxStyle>
                                </GroupByBox>
                                <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                </GroupByRowStyleDefault>
                                <ActivationObject BorderWidth="" BorderColor="">
                                </ActivationObject>
                                <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </FooterStyleDefault>
                                <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                    Font-Names="Microsoft Sans Serif" BackColor="Window">
                                    <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                    <Padding Left="3px"></Padding>
                                </RowStyleDefault>
                                <FilterOptionsDefault>
                                    <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                        Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                        CustomRules="overflow:auto;">
                                        <Padding Left="2px"></Padding>
                                    </FilterOperandDropDownStyle>
                                    <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                    </FilterHighlightRowStyle>
                                    <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                        Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                        Height="300px" CustomRules="overflow:auto;">
                                        <Padding Left="2px"></Padding>
                                    </FilterDropDownStyle>
                                </FilterOptionsDefault>
                                <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </HeaderStyleDefault>
                                <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                </EditCellStyleDefault>
                                <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                    Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
                                </FrameStyle>
                                <Pager MinimumPagesForDisplay="2">
                                    <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                        </BorderDetails>
                                    </PagerStyle>
                                </Pager>
                                <AddNewBox Hidden="False">
                                    <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                        </BorderDetails>
                                    </BoxStyle>
                                </AddNewBox>
                            </DisplayLayout>
                        </igtbl:UltraWebGrid>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <div style="margin-top: -10px">
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Теплоснабжение"
                            Width="100%" />
                        <div style="margin-top: -5px">
                            <uc2:FST_0001_0003_Horizontal_Text ID="TagCloudFST_0001_0003_Horizontal_Text2" runat="server" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <div style="margin-top: -10px">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Водоснабжение"
                            Width="100%" />
                        <div style="margin-top: -5px">
                            <uc2:FST_0001_0003_Horizontal_Text ID="TagCloudFST_0001_0003_Horizontal_Text3" runat="server" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
