<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Default.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FK_0001_0014_1.Default"
    Title="Сравнение темпа роста доходов по субъектам РФ" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
    
<div style="width: 220px" class="bujetHeader">
    <asp:HyperLink ID="HyperLink1" runat="server">HyperLink</asp:HyperLink></div>
<igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" Height="195px" OnDataBinding="UltraWebGrid_DataBinding"
    OnInitializeLayout="UltraWebGrid_InitializeLayout" StyleSetName="Office2007Blue" SkinID="UltraWebGrid"
    Width="325px" OnInitializeRow="UltraWebGrid_InitializeRow">
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
            BackColor="Window" Width="325px" Height="195px">
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

<div style="width: 200px">
<asp:Table ID="Table1" runat="server">
</asp:Table>
    <asp:Label ID="Label1" runat="server"></asp:Label>&nbsp;<asp:Image ID="Image1" runat="server" /></div>
<div style="text-align: right;width: 230px">
    <asp:HyperLink ID="HyperLink2" runat="server" SkinID="HyperLink"></asp:HyperLink>
</div>   
