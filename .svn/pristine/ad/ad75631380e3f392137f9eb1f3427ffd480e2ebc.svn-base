<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SGM_0001_0001_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.SGM_0001_0001_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: Black">
    <form id="form1" runat="server">   
    <div style="position: absolute; width: 768px; background-color: black; top: 0px; left: 0px">
        </div>     
            <table style="position: absolute; width: 760px; background-color: Black; top: 0px; left: 0px;">
                <tr>
                    <td style="text-align: left; background-color: Black;" align="left" valign="top">
                        <table style="position: absolute; width: 760px; background-color: Black; top: 0px; left: 0px">
                            <tr>
                                <td style="width: 760px; text-align: left; background-color: Black;" align="left" valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 750px; height: 100%">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px; background-color: Black">
                                            </td>
                                            <td style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
                                                background-color: Black; height: 3px;">
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
                                                width: 2px; background-color: Black;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
                                                width: 2px; height: 36px; background-color: Black">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Заболеваемость на 100 тыс. населения"></asp:Label>
                                            </td>
                                            <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <igtbl:UltraWebGrid ID="grid" runat="server" Height="200px" SkinID="UltraWebGrid" Width="325px" OnInitializeLayout="grid_InitializeLayout"
                                        OnInitializeRow="grid_InitializeRow">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
                                            AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                            TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
                                            <GroupByBox>
                                                <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                                </BoxStyle>
                                            </GroupByBox>
                                            <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                            </GroupByRowStyleDefault>
                                            <ActivationObject BorderWidth="" BorderColor="">
                                            </ActivationObject>
                                            <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </FooterStyleDefault>
                                            <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                BackColor="Window">
                                                <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                                <Padding Left="3px"></Padding>
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                                                    <Padding Left="2px"></Padding>
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                    BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                                                    <Padding Left="2px"></Padding>
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                            </EditCellStyleDefault>
                                            <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                BackColor="Window" Width="325px" Height="200px">
                                            </FrameStyle>
                                            <Pager MinimumPagesForDisplay="2">
                                                <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                                </PagerStyle>
                                            </Pager>
                                            <AddNewBox Hidden="False">
                                                <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                                </BoxStyle>
                                            </AddNewBox>
                                        </DisplayLayout>
                                    </igtbl:UltraWebGrid>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 755px; padding-left: 10px; padding-right: 10px;" valign="top">
                                    <div style="width: 745px; text-align: justify">
                                        <asp:Label ID="LabelPart2Text" runat="server" SkinID="InformationText"></asp:Label></div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 755px; padding-left: 10px; padding-right: 10px;" valign="top">
                                    <div style="width: 745px; text-align: justify">
                                        <asp:Label ID="LabelPart3Text" runat="server" SkinID="InformationText"></asp:Label></div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 760px;" valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 750px; height: 100%">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px; background-color: Black">
                                            </td>
                                            <td style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
                                                background-color: Black; height: 3px;">
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
                                                width: 2px; background-color: Black;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
                                                width: 2px; height: 36px; background-color: Black">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Охват вакцинацией"></asp:Label>
                                            </td>
                                            <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 750px;">
                                    <asp:Label ID="lbInjection" runat="server" SkinID="InformationText"></asp:Label>
                                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="760px" SkinID="UltraWebGrid">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
                                            AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                            TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
                                            <GroupByBox>
                                                <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                                </BoxStyle>
                                            </GroupByBox>
                                            <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                            </GroupByRowStyleDefault>
                                            <ActivationObject BorderWidth="" BorderColor="">
                                            </ActivationObject>
                                            <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </FooterStyleDefault>
                                            <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                BackColor="Window">
                                                <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                                <Padding Left="3px"></Padding>
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                                                    <Padding Left="2px"></Padding>
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                    BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                                                    <Padding Left="2px"></Padding>
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                            </EditCellStyleDefault>
                                            <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                BackColor="Window" Width="760px" Height="200px">
                                            </FrameStyle>
                                            <Pager MinimumPagesForDisplay="2">
                                                <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                                </PagerStyle>
                                            </Pager>
                                            <AddNewBox Hidden="False">
                                                <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                                </BoxStyle>
                                            </AddNewBox>
                                        </DisplayLayout>
                                    </igtbl:UltraWebGrid>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
