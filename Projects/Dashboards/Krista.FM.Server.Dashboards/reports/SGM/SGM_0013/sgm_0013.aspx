<%@ Page Title="Сведения об инфекционных и паразитарных заболеваниях" MasterPageFile="~/Reports.Master"
    Language="C#" AutoEventWireup="true" Codebehind="sgm_0013.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.SGM.SGM_013.sgm_013" %>

<%@ Register Src="../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc2" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table style="vertical-align: top;">
            <tr>
                <td style="width: 100%;" align="left" valign="top">
                    <asp:Label ID="LabelTitle" runat="server" CssClass="PageTitle" Text="Label"></asp:Label></td>
                <td align="left" style="height: 21px" valign="top">
                    <uc3:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top" style="width: 100%">
                    <asp:Label ID="LabelSubTitle" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
                <td align="left" valign="top">
                </td>
            </tr>
        </table>
    </div>
    <table style="vertical-align: top;">
        <tr>
            <td align="left" valign="top">
                <uc1:CustomMultiCombo ID="ComboMap" runat="server" ParentSelect="true" Title="Территория"
                    Width="310" />
            </td>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboYear" runat="server" Title="Год" Width="90" />
            </td>
            <td align="left" valign="top">
                <uc1:CustomMultiCombo ID="ComboMonth" runat="server" MultiSelect="true" Width="180"
                    Title="Месяца" />
            </td>
            <td align="left" valign="top">
                <uc1:CustomMultiCombo ID="ComboCompareYear" runat="server" MultiSelect="false" Title="Год для сравнения"
                    Width="190" />
            </td>
            <td align="left" valign="top">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top; width: 607px">
        <tr>
            <td align="left" valign="top">
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                            <igtbl:UltraWebGrid ID="grid" runat="server" Height="124px" Width="411px" OnInitializeLayout="grid_InitializeLayout"
                                SkinID="UltraWebGrid" OnInitializeRow="grid_InitializeRow">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="grid"
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
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
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
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="411px" Height="124px">
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
