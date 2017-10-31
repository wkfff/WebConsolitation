<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FNS0101Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FNS0101Gadget" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<table width="100%">
<tr>
    <td style="width:50%">
        <asp:Label ID="Label1" skinid="ServeText" runat="server" Text="Label"></asp:Label>
    </td>
    <td style="width:100%">
        <div class="GadgetTopDate"><asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></div>    
    </td>
</tr>
</table>    
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="190px" Width="99%">
  <igtbl:UltraWebGrid ID="Grid" runat="server" Height="100%" Width="100%" OnDataBinding="Grid_DataBinding" OnInitializeLayout="Grid_InitializeLayout" stylesetname="Office2007Blue" enableappstyling="True" enabletheming="True" OnInitializeRow="Grid_InitializeRow" SkinID="GadgetGrid">
    <Bands>
        <igtbl:UltraGridBand>
            <HeaderStyle Wrap="True" />
            <AddNewRow View="NotSet" Visible="NotSet">
            </AddNewRow>
            <RowStyle Wrap="True" />
        </igtbl:UltraGridBand>
    </Bands>
    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
        AllowSortingDefault="OnClient" BorderCollapseDefault="Separate"
        HeaderClickActionDefault="SortMulti" Name="ctl00xGrid" RowHeightDefault="23px"
        RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
        ReadOnly="LevelZero">
        <FrameStyle BorderColor="InactiveCaption" BorderStyle="Solid"
            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="100%"
            Width="100%">
        </FrameStyle>
        <Pager MinimumPagesForDisplay="2">
            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </PagerStyle>
        </Pager>
        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
        </EditCellStyleDefault>
        <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </FooterStyleDefault>
        <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left" Wrap="True">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </HeaderStyleDefault>
        <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
            <Padding Left="3px" />
            <BorderDetails ColorLeft="Window" ColorTop="Window" />
        </RowStyleDefault>
        <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
        </GroupByRowStyleDefault>
        <GroupByBox Hidden="True">
            <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
            </BoxStyle>
        </GroupByBox>
        <AddNewBox>
            <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </BoxStyle>
        </AddNewBox>
        <ActivationObject BorderColor="" BorderWidth="">
        </ActivationObject>
        <FilterOptionsDefault>
            <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px" Height="300px" Width="200px">
                <Padding Left="2px" />
            </FilterDropDownStyle>
            <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
            </FilterHighlightRowStyle>
            <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px">
                <Padding Left="2px" />
            </FilterOperandDropDownStyle>
        </FilterOptionsDefault>
    </DisplayLayout>
</igtbl:UltraWebGrid>      

</asp:Panel>