<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="DefaultDetail.aspx.cs"
    Title="Группы субъектов по доле межбюджетных трансфертов" Inherits="Krista.FM.Server.Dashboards.reports.FK_0001_0005.DefaultDetail" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc3" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table width="100%" style="height: 100%;">
            <tr style="height: 20%;">
                <td valign="top" colspan="2" style="width: 100%;">
                    <table>
                        <tr>
                            <td style="width: 100%;">
                                <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="DefaultDetail.html" />
                                &nbsp;&nbsp;
                                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:Label ID="PageSubTitle1" runat="server" CssClass="PageSubTitle"></asp:Label>
                            </td>
                            <td>
                                <uc2:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="1" style="width: 100%; height: 20%;">
                                <asp:Label ID="PageSubTitle2" runat="server" CssClass="PageDescription"></asp:Label>
                                <table style="width: 100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="group1" runat="server" CssClass="PageDescription"></asp:Label>
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="group2" runat="server" CssClass="PageDescription"></asp:Label>
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="group3" runat="server" CssClass="PageDescription"></asp:Label>
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="group4" runat="server" CssClass="PageDescription"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <uc1:GridSearch ID="GridSearch1" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td id="gridTD" runat="server">
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
                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="325px"
                                    OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                                    OnInitializeRow="UltraWebGrid_InitializeRow" EnableAppStyling="True" StyleSetName="Office2007Blue"
                                    SkinID="UltraWebGrid">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                        StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                        StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
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
                                            Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
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
    </div>
</asp:Content>
