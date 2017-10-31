<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sgm_0016.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.SGM.SGM_0016.sgm_0016"  Title="Данные о прививках" MasterPageFile="~/Reports.Master" %>

<%@ Register Src="../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc3" %>

<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table id="tableCaption">
            <tr>
                <td style="width: 100%">
                    <asp:Label ID="LabelTitle" runat="server" CssClass="PageTitle" Text="Label"></asp:Label></td>
                <td>
                    <uc3:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
        </table>    
        <table style="width: 200px">
            <tr>
                <td align="left" valign="top">
                    <uc1:CustomMultiCombo ID="ComboYear" runat="server" Title="Год" />
                </td>
                <td align="left" valign="top">
                    <uc2:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
            </tr>
        </table>
    
    </div>
    <table style="width: 604px">
        <tr>
            <td>      
    <table style="vertical-align: top;">
        <tr>
            <td align="left" valign="top"><table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
    <tr>
        <td class="topleft">
        </td>
        <td class="top">
        </td>
        <td class="topright">
        </td>
    </tr>
    
    <tr>
        <td class="left">
        </td>
        <td style="overflow: visible;">                  
                <igtbl:ultrawebgrid id="grid" runat="server" height="200px" skinid="UltraWebGrid"
                    width="325px" OnInitializeLayout="grid_InitializeLayout" OnInitializeRow="grid_InitializeRow"><Bands>
<igtbl:UltraGridBand>
<AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
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

<FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px"></FrameStyle>

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
</igtbl:ultrawebgrid>
        </td>
        <td class="right">
        </td>
    </tr>
    <tr>
        <td class="bottomleft">
        </td>
        <td class="bottom">
        </td>
        <td class="bottomright">
        </td>
    </tr>
</table>
            </td>
        </tr>
    </table>
</asp:Content>
