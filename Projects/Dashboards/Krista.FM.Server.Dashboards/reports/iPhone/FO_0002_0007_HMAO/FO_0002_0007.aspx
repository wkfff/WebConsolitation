<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0002_0007_HMAO.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0002_0007_HMAO" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style=" background-color: black; width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse;">
        <tr><td valign="top" style="padding-left: 5px; width: 332px">
            <asp:Label ID="Label" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" OnDataBinding="UltraWebGrid_DataBinding"
                OnInitializeLayout="UltraWebGrid_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow"
                SkinID="UltraWebGrid" Width="309px">
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
                        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                        Width="309px">
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
            </igtbl:UltraWebGrid></td></tr>
        </table>
        </div>
    </form>
</body>
</html>
