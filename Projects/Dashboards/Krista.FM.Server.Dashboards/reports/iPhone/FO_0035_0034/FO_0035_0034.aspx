<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0035_0034.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0034" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register TagPrefix="uc1" TagName="iPadElementHeader" Src="~/Components/iPadElementHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: #a8a8a8">
    <form id="form1" runat="server">
    <table style="width: 670px; height: 900; border-collapse: collapse; background-color: #a8a8a8;
        position: absolute; top: 0px; left: 0px">
        <tr>
            <td valign="top" colspan="3">
                <table class="InformationTextPopup">
                    <tr>
                        <td>
                            <div style="float: left; margin-right: 10px">
                                <asp:Image ID="Image1" runat="server" /></div>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:HyperLink ID="HyperLinkSite" runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lbFIO" runat="server" CssClass="InformationTextPopup" Font-Names="Arial"
                                            Style="margin-top: 10px;"></asp:Label><br />
                                        <!--<asp:Label ID="lbDirector" runat="server" CssClass="InformationTextPopup" Font-Names="Arial" Text="Губернатор Ярославской области, руководитель правительства Ярославской области"></asp:Label><br />-->
                                        <asp:Label ID="lbPhone" runat="server" Font-Names="Arial" CssClass="InformationTextPopup"></asp:Label><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" CssClass="InformationTextPopup" Style="margin-top: 10px;"
                                            Font-Names="Arial" Text="E-mail:"></asp:Label>&nbsp;<asp:HyperLink ID="HyperLinkMail"
                                                runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lbDescription" CssClass="InformationTextPopup" Font-Names="Arial" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="left">
                <!-- <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Платежные поручения на рассмотрении ФО" Width="100%" />-->
                <igtbl:UltraWebGrid ID="ReviewGrid" runat="server" Height="200px" Width="509px" OnDataBinding="ReviewGrid_DataBinding"
                    OnInitializeLayout="Grid_InitializeLayout" SkinID="UltraWebGridPopup" OnInitializeRow="Grid_InitializeRow">
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
                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                            </BorderDetails>
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
                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                            </BorderDetails>
                        </HeaderStyleDefault>
                        <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                        </EditCellStyleDefault>
                        <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                            Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
                        </FrameStyle>
                        <Pager MinimumPagesForDisplay="2">
                            <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </PagerStyle>
                        </Pager>
                        <AddNewBox Hidden="False">
                            <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </BoxStyle>
                        </AddNewBox>
                    </DisplayLayout>
                </igtbl:UltraWebGrid>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
