<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SEP_0001_0001_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPad.SEP_0001_0001_v" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
      <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title> 
    <style type="text/css">

p.ConsPlusNonformat
	{margin-bottom:.0001pt; 
	text-autospace:none;
	font-size:10.0pt;
	font-family:"Courier New";
	        margin-left: 0cm;
            margin-right: 0cm;
            margin-top: 0cm;
        }
    </style>
</head> 
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; height: 1050; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible;">
        <table>
            <tr>
                <td>
                    <asp:Label ID="lbInfo" runat="server" SkinID="InformationText" ></asp:Label>
                </td>
            </tr>
        </table>


        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td align="left" valign="top">
                       
                             <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" 
                            Width="325px" SkinID="UltraWebGrid">
<DisplayLayout Name="UltraWebGrid1" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" 
                                AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" 
                                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" 
                                HeaderClickActionDefault="SortMulti" RowHeightDefault="20px" 
                                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" 
                                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" 
                                TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
        BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
        Font-Size="8.25pt" Height="200px" Width="325px">
    </FrameStyle>
    <Pager MinimumPagesForDisplay="2">
        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
        </PagerStyle>
    </Pager>
    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
    </EditCellStyleDefault>
    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </FooterStyleDefault>
    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
        HorizontalAlign="Left">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </HeaderStyleDefault>
    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
            BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                WidthTop="1px" />
        </BoxStyle>
    </AddNewBox>
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
    <FilterOptionsDefault>
        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
            BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
            Width="200px">
            <Padding Left="2px" />
        </FilterDropDownStyle>
        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
        </FilterHighlightRowStyle>
        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
            BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
            <Padding Left="2px" />
        </FilterOperandDropDownStyle>
    </FilterOptionsDefault>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>
                        </igtbl:UltraWebGrid>
                        
                    </td>
                </tr>
            </table>

        <table>
            <tr>
                <td>
                    <asp:Label ID="lbInfo1" runat="server" SkinID="InformationText" ></asp:Label>
                </td>
            </tr>
        </table>


        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td align="left" valign="top">
                       
                             <igtbl:UltraWebGrid ID="UltraWebGrid2" runat="server" Height="200px" 
                            Width="325px" SkinID="UltraWebGrid">
<DisplayLayout Name="UltraWebGrid1" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" 
                                AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" 
                                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" 
                                HeaderClickActionDefault="SortMulti" RowHeightDefault="20px" 
                                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" 
                                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" 
                                TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
        BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
        Font-Size="8.25pt" Height="200px" Width="325px">
    </FrameStyle>
    <Pager MinimumPagesForDisplay="2">
        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
        </PagerStyle>
    </Pager>
    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
    </EditCellStyleDefault>
    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </FooterStyleDefault>
    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
        HorizontalAlign="Left">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </HeaderStyleDefault>
    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
            BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                WidthTop="1px" />
        </BoxStyle>
    </AddNewBox>
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
    <FilterOptionsDefault>
        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
            BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
            Width="200px">
            <Padding Left="2px" />
        </FilterDropDownStyle>
        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
        </FilterHighlightRowStyle>
        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
            BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
            <Padding Left="2px" />
        </FilterOperandDropDownStyle>
    </FilterOptionsDefault>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>
                        </igtbl:UltraWebGrid>
                        
                    </td>
                </tr>
            </table>
    </div>
    </form>
</body>
</html>
