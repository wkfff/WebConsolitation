<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="DefaultCompare.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0003_HMAO.DefaultCompare"
    Title="Сравнение темпа роста доходов по субъектам РФ" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td valign="top" style="width: 100%">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="DefaultCompare.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label></td>
            <td align="right" valign="top">
                <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                <br />
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboMonth" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top" align="left" style="font-family:Verdana; font-size:12px;" >
                <asp:RadioButtonList ID="RubMiltiplierButtonList" runat="server" AutoPostBack="True" RepeatDirection="horizontal"
                    Width="170px">
                    <asp:ListItem Selected="True">тыс.руб.</asp:ListItem>
                    <asp:ListItem>млн.руб.</asp:ListItem>
                </asp:RadioButtonList>
            </td>              
            <td style="width: 100%" align="right" valign="top">
                <uc2:GridSearch ID="GridSearch1" runat="server" />
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
                            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" Height="200px"
                                OnDataBinding="UltraWebGrid_DataBinding"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeRow="UltraWebGrid_InitializeRow">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
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
</asp:Content>