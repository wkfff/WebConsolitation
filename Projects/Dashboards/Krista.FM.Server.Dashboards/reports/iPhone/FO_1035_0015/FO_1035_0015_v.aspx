<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_1035_0015_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_1035_0015_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; background-color: black; top: 0px; left: 0px;
            z-index: 2; overflow: hidden">
            <table style="width: 768px; background-color: Black; top: 0px; left: 0px; border-collapse: collapse">
                <tr>
                    <td>
                        <table style="border-collapse: collapse; background-color: Black; width: 765px; height: 100%; margin-right: 10px;">
                            <tr>
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Консолидированный бюджет района"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="overflow: visible; background-color: Black;">
                                    <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="509px" OnDataBinding="UltraWebGridBudget_DataBinding"
                                        OnInitializeLayout="UltraWebGridBudget_InitializeLayout" SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
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
                                                BackColor="Window" Width="509px" Height="200px">
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
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;" runat="server" id="selfBudgetHeader">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label7" runat="server" CssClass="ElementTitle" Text="Собственный бюджет района"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <igtbl:UltraWebGrid ID="UltraWebGridBudgetSelf" runat="server" Height="200px" Width="509px" OnDataBinding="UltraWebGridBudgetSelf_DataBinding"
                                        OnInitializeLayout="UltraWebGridBudget_InitializeLayout" SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
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
                                                BackColor="Window" Width="509px" Height="200px">
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
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Соблюдение БК РФ"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="TableFont">
                                    <table style="width: 763px">
                                        <tr>
                                            <td valign="top">
                                                <igchart:UltraChart ID="UltraChart11" runat="server" BackgroundImageFileName="" SkinID="UltraWebColumnChart">
                                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                                </igchart:UltraChart>
                                            </td>
                                            <td>                                                
                                                <asp:Label ID="lbDebts" runat="server" CssClass="InformationText" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="TableFont" style="padding-top: 20px">
                                    <table style="width: 763px">
                                        <tr>
                                            <td valign="top">
                                                <igchart:UltraChart ID="UltraChart12" runat="server" SkinID="UltraWebColumnChart">
                                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_12#SEQNUM(100).png" />
                                                </igchart:UltraChart>
                                            </td>
                                            <td>
                                               
                                                <asp:Label ID="lbDeficite" runat="server" CssClass="InformationText" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="TableFont" style="padding-top: 20px">
                                    <table style="width: 763px">
                                        <tr>
                                            <td valign="top">
                                                <igchart:UltraChart ID="UltraChart13" runat="server" SkinID="UltraWebColumnChart">
                                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_21#SEQNUM(100).png" />
                                                </igchart:UltraChart>
                                            </td>
                                            <td>                                               
                                                <asp:Label ID="lbDebtServe" runat="server" CssClass="InformationText" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Просроченная кредиторская задолженность"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 315px; background-color: black;" valign="top">
                                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" SkinID="UltraWebGrid">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                            BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                    CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                                    <Padding Left="2px" />
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" Width="200px">
                                                    <Padding Left="2px" />
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                            </EditCellStyleDefault>
                                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                Font-Size="8.25pt" Height="200px" Width="315px">
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
                                    </igtbl:UltraWebGrid>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Недоимка"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                             <tr>
                                <td>                                   
                                    <igtbl:UltraWebGrid ID="UltraWebGridArrearAll" runat="server" Height="200px" OnDataBinding="UltraWebGridArrearAll_DataBinding"
                                        OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid" OnInitializeRow="Grid_InitializeRow">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                            BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                    CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                                    <Padding Left="2px" />
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" Width="200px">
                                                    <Padding Left="2px" />
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                            </EditCellStyleDefault>
                                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                Font-Size="8.25pt" Height="200px" Width="315px">
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
                                    </igtbl:UltraWebGrid><br/>                                                                   
                                    <asp:Label ID="Label6" runat="server" CssClass="TableFont" Text="5 доходных источников с наибольшей недоимкой"></asp:Label>
                                    <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" OnDataBinding="UltraWebGrid_DataBinding"
                                        OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid" OnInitializeRow="Grid_InitializeRow">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                            BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                    CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                                    <Padding Left="2px" />
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" Width="200px">
                                                    <Padding Left="2px" />
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                            </EditCellStyleDefault>
                                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                Font-Size="8.25pt" Height="200px" Width="315px">
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
                                    </igtbl:UltraWebGrid><br/>
                                    <asp:Label ID="Label1" runat="server" CssClass="TableFont" Text="5 ОКВЭД с наибольшей недоимкой"></asp:Label>
                                    <igtbl:UltraWebGrid ID="UltraWebGridOkved" runat="server" Height="200px" OnDataBinding="UltraWebGridOkved_DataBinding"
                                        OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid" OnInitializeRow="Grid_InitializeRow">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                            BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
                                                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                    CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                                    <Padding Left="2px" />
                                                </FilterOperandDropDownStyle>
                                                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                </FilterHighlightRowStyle>
                                                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;"
                                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" Width="200px">
                                                    <Padding Left="2px" />
                                                </FilterDropDownStyle>
                                            </FilterOptionsDefault>
                                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                            </EditCellStyleDefault>
                                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                Font-Size="8.25pt" Height="200px" Width="315px">
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
                                    </igtbl:UltraWebGrid>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table style="border-collapse: collapse; width: 763px;" runat="server" id="settlementHeader">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label8" runat="server" CssClass="ElementTitle" Text="Бюджеты поселений"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                <asp:Label ID="lbSettlementsList" runat="server" CssClass="InformationText" Text="Бюджеты поселений"></asp:Label>
                                    <igtbl:UltraWebGrid ID="UltraWebGridBudgetSettlement" runat="server" Height="200px" Width="509px" OnDataBinding="UltraWebGridBudgetSettlement_DataBinding"
                                        OnInitializeLayout="UltraWebGridBudget_InitializeLayout" SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
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
                                                BackColor="Window" Width="509px" Height="200px">
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
