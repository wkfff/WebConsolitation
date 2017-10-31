<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0003_0005.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0003_0005" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=2.0; minimum-scale=1.0; user-scalable=1;" />
    <meta name="format-detection" content="telephone=no" />
    <style>
        /* Quickie Reset */
        *
        {
            margin: 0;
            padding: 0;
            border: 0;
            float: none;
        }
        
        /* Footer */
        #footer
        {
            position: absolute;
            top: 182px;
            height: 900px;
            background-position: center bottom;
            width: 301px;
            overflow: hidden;
            color: white;
            float: none;
        }
        
        /* Header */
        #header
        {
            position: absolute;
            top: 0px;
            height: 0px;
            background-position: center bottom;
            width: 0px;
            overflow: hidden;
            color: white;
            float: none;
        }
    </style>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <table style="border-collapse: collapse">
        <tr>
            <td class="InformationText">
                <asp:Label ID="lbDescription" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
    <div id="container">
        <div id="content">
            <div style="width: 1500px; height: 900px;">
                <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="509px"
                    SkinID="UltraWebGrid">
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
        </div>
    </div>
    <div id="footer">
        <igtbl:UltraWebGrid ID="UltraWebGrid2" runat="server" Height="200px" Width="509px"
            SkinID="UltraWebGrid">
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
    <div id="header">
        
    </div>   
    </form>
</body>
</html>