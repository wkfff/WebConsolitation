<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0013_Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.DashboardFederal.Dashboard.reports.DashboardFederal.FK_0001_0013_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<div class="GadgetTopDate">
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</div>
<asp:Panel ID="MainPanel" runat="server" Height="100%" Width="100%">
    <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="325px" EnableAppStyling="True"
        StyleSetName="Office2007Blue" OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
        OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="GadgetGrid">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
            AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
            Name="ctl00xUltraWebGrid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
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
                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="Gainsboro"></BorderDetails>
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
            <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Double" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                BackColor="Window" Width="325px" Height="200px">
                <BorderDetails StyleLeft="Solid" WidthLeft="1px" />
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
    <div style="text-align: right;">
        <asp:HyperLink ID="HyperLink1" runat="server" SkinID="HyperLink">HyperLink</asp:HyperLink><br />
        <asp:HyperLink ID="HyperLink2" runat="server" SkinID="HyperLink">HyperLink</asp:HyperLink>
    </div>
</asp:Panel>
