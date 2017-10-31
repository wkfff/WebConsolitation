<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0033_0001_V.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0033_0001_V" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse;">
        <tr><td colspan=2 style="background-color: black; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; left: 0px; border-left: 0px solid; width: 314px; border-bottom: 0px solid; position: static; top: 195px; border-collapse: collapse; height: 211px;" valign="top">
            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="183px" OnDataBinding="Grid_DataBinding"
                OnInitializeLayout="Grid_InitializeLayout" OnInitializeRow="Grid_InitializeRow"
                SkinID="UltraWebGrid" Width="310px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                    HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" RowHeightDefault="20px"
                    RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                    StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
                    <GroupByBox>
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
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                    </EditCellStyleDefault>
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="183px"
                        Width="310px">
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
            <asp:Label ID="Label1" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br/>
            <asp:Label ID="Label2" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br/><br/></td></tr>            
                
            <tr><td style="width: 315px; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 25px; left: 0px; position: static; top: 408px; background-image: url(../../../images/servePane.gif);" align="left" valign="top" colspan="2">
                <asp:Label ID="Label" runat="server" Font-Names="Arial" Font-Size="12px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label><br>
                <asp:Label ID="LabelDate" runat="server" Font-Names="Arial" Font-Size="12px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label></td></tr> 
       </table>
    </div>
    </form>
</body>
</html>
