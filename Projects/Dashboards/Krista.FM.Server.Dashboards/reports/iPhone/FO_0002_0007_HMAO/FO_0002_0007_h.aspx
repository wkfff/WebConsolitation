<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0002_0007_HMAO_h.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0002_0007_HMAO_h" %>

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
        <table style="left: 0px; position: absolute; top: 0px; border-collapse: collapse;">      
            <tr><td style="width: 480px; height: 360px; border-right: lime 0px solid; border-top: lime 0px solid; border-left: lime 0px solid; border-bottom: lime 0px solid; position: relative; background-color: black; left: 0px; top: 0px; padding-left: 5px; border-collapse: collapse;" align="left" valign="top" colspan="" rowspan="">
                <asp:TextBox ID="TextBox1" runat="server" CssClass="InformationText" SkinID="TextBoxInformationText"
                    Width="470px" ReadOnly="True"></asp:TextBox><br />
                <asp:TextBox ID="TextBox2" runat="server" CssClass="InformationText" SkinID="TextBoxInformationText"
                    Width="470px" ReadOnly="True"></asp:TextBox><br />
                <asp:TextBox ID="TextBox3" runat="server" CssClass="InformationText" SkinID="TextBoxInformationText"
                    Width="470px" ReadOnly="True"></asp:TextBox><br />
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="476px" OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid"><Bands>
<igtbl:UltraGridBand>
<AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
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

<FilterOptionsDefault>
<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterOperandDropDownStyle>

<FilterHighlightRowStyle ForeColor="White" BackColor="#151C55"></FilterHighlightRowStyle>

<FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterDropDownStyle>
</FilterOptionsDefault>

<HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyleDefault>

<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

<FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="476px" Height="200px"></FrameStyle>

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
</igtbl:UltraWebGrid><asp:TextBox ID="TextBox5" runat="server" CssClass="ServeText"
                    SkinID="TextBoxServeText" Width="470px" ReadOnly="True"></asp:TextBox><br />
                <asp:TextBox ID="TextBox7" runat="server" CssClass="ServeText" SkinID="TextBoxServeText"
                    Width="470px" ReadOnly="True"></asp:TextBox></td></tr><tr><td style="width: 297px; border-right: 0px solid; border-top: 0px solid; background-image: url(../../../images/servePane.gif); border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 25px; padding-left: 5px; left: 0px; margin-left: 0px; position: static; top: 408px;" align="left" valign="top" colspan="" rowspan="">
    
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server"><asp:Label ID="Label1" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br />
    <asp:Label ID="Label2" runat="server" Text="Label" SkinID="ServeText"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
        </table>
    </div>
    </form>
</body>
</html>
