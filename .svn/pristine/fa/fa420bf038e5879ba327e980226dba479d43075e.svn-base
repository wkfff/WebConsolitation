<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports.Master" CodeBehind="sgm_0020.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.SGM.SGM_0020.sgm_0020" Title="Анализ роста/снижения заболеваемости" %>

<%@ Register Src="../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc3" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table style="vertical-align: top;">
            <tr>
                <td align="left" valign="top" style="width: 100%;">
                    <asp:Label ID="LabelTitle" runat="server" CssClass="PageTitle"></asp:Label>
                </td>
                <td align="left" valign="top">
                    <uc3:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <asp:Label ID="LabelSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                </td>
            </tr>
        </table>
        <table style="vertical-align: top;">
            <tr>
                <td align="left" valign="top">
                    <uc1:CustomMultiCombo ID="ComboMap" runat="server" MultiSelect="false" ParentSelect="true"
                        Title="Территория" Width="300" />
                </td>
                <td align="left" valign="top">
                    <uc1:CustomMultiCombo ID="ComboYear" runat="server" MultiSelect="true" Title="Год" />
                </td>
                <td align="left" valign="top">
                    <uc1:CustomMultiCombo ID="ComboDesease" runat="server" MultiSelect="true" Title="Заболевание"
                        Width="250" />
                </td>
                <td align="left" valign="top">
                    <uc1:CustomMultiCombo ID="ComboChildGroup" runat="server" MultiSelect="false" Title="Группа детей"
                        Width="250" />
                </td>
                <td align="left" valign="top">
                    <uc2:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="vertical-align: top;">
            <tr>
                <td valign="top" align="left">
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
                                <igtbl:UltraWebGrid ID="grid" runat="server" Height="200px" OnInitializeLayout="grid_InitializeLayout"
                                    SkinID="UltraWebGrid" Width="325px">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        HeaderClickActionDefault="SortMulti" Name="grid" RowHeightDefault="25px" RowSelectorsDefault="No"
                                        SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                                        TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                                            Width="325px">
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
    </div>
</asp:Content>
