<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.REGION.REGION_0010.Report825._default" %>


   <%@ Register Src="../../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>

    <%@ Register Src="../../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
    <%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
        TagPrefix="uc2" %>
    <%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
        TagPrefix="uc3" %>
    <%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
        TagPrefix="uc4" %>
    <%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
        TagPrefix="uc5" %>
          
           


    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
    <%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
     
    <%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

         

    <%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
    <%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
        Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>



    <asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
        <div>
            <table>
                <tr>
                    <td style="width: 70%">
                        <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="true" HelpPageUrl="help.html" />
                        <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle" Width="73%">Доклад высших должностных лиц (руководителей высших исполнительных органов государственной власти) субъектов РФ (Указ № 825)</asp:Label><br />
                        <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle" Text="Label" 
                            Width="50%"></asp:Label></td>
                    <td width="20%">
                       <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                &nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                    </td>
                </tr>
            </table>
            <table style="vertical-align: top">
                <tr>
                    <td valign="top" colspan="2">
                        <uc2:CustomMultiCombo ID="Type3" runat="server" Title="Год" Width="100" />
                    </td>
                    <td valign="top">
                        <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                    </td>
                    <td align="right" style="width: 100%" valign="top">
                        &nbsp;
                        </td>
                </tr>
            </table>
            <table style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td colspan="2" style="vertical-align: top; height: 100%;">
                        <table style="width: 100%; border-collapse: collapse; background-color: white;">
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
                                <td style="vertical-align: top;">
                                    <asp:Label ID="Label1" runat="server" CssClass="ElementTitle"></asp:Label><br />
                                    <igtbl:UltraWebGrid
                                        ID="G" runat="server" OnDataBinding="G_DataBinding"
                                        SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                            CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="G"
                                            RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                            Version="4.00" ViewType="OutlookGroupBy">
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
                                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
                                    </igtbl:UltraWebGrid></td>
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
            &nbsp; &nbsp; &nbsp;
            </div>


     </asp:Content>

