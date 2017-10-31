<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.HMAO_ARC._0004._default" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
    <%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %> 
    <%@ Register Src="../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc5" %>
     <%@ Register Src="../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc6" %>
    
    <%@ Register Src="../../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
    <asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <table style="width: 100%; margin-top: 6px;" >
            <tr>
                <td>
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true" />
                    &nbsp;<asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Label"></asp:Label>
                    <br />
                    <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label>
                </td>
                <td style="text-align: right">
                    <uc5:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                    <uc6:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="margin-top: 5px">
            <tr>
                <td>
                <uc3:CustomMultiCombo ID="ComboYear" runat="server" />
                </td>
                <td>
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
            </tr>
        </table>
            <table id="TABLE1" 
            style="margin-top: 8px; width: 100%; border-collapse: collapse">
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
            <td style="background-color: white">
        <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" Height="200px" 
                ondatabinding="Grid_DataBinding" SkinID="UltraWebGrid" 
                StyleSetName="Office2007Blue" Width="325px" 
                oninitializelayout="Grid_InitializeLayout" oninitializerow="Grid_InitializeRow">
            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" 
                AllowDeleteDefault="Yes" AllowSortingDefault="OnClient" 
                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" 
                HeaderClickActionDefault="SortMulti" Name="Grid" RowHeightDefault="20px" 
                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" 
                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" 
                TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
                <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
                    BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
                    Font-Size="8.25pt" Height="200px" Width="325px">
                </FrameStyle>
                <Pager MinimumPagesForDisplay="2">
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                        WidthTop="1px" />
                    </PagerStyle>
                </Pager>
                <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                </EditCellStyleDefault>
                <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                        WidthTop="1px" />
                </FooterStyleDefault>
                <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
                    HorizontalAlign="Left" Font-Bold="True">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                        WidthTop="1px" />
                </HeaderStyleDefault>
                <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                    <Padding Left="3px" />
                    <BorderDetails ColorLeft="Window" ColorTop="Window" />
                </RowStyleDefault>
                <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                </GroupByRowStyleDefault>
                <GroupByBox>
                    <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                    </BoxStyle>
                </GroupByBox>
                <AddNewBox Hidden="False">
                    <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
                        BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                            WidthTop="1px" />
                    </BoxStyle>
                </AddNewBox>
                <ActivationObject BorderColor="" BorderWidth="">
                </ActivationObject>
                <FilterOptionsDefault>
                    <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
                        BorderWidth="1px" CustomRules="overflow:auto;" 
                        Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
                        Width="200px">
                        <Padding Left="2px" />
                    </FilterDropDownStyle>
                    <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                    </FilterHighlightRowStyle>
                    <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
                        BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
                        Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                        <Padding Left="2px" />
                    </FilterOperandDropDownStyle>
                </FilterOptionsDefault>
            </DisplayLayout>
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
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
    <table id="TABLE2" 
            style="margin-top: 10px; width: 100%; border-collapse: collapse; margin-bottom: 8px;">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label>

            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="background-color: white">
            
            <dmwc:mapcontrol ID="DundasMap" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap2#"
                                ImageUrl="../../TemporaryImages/lenin_region_3_#SEQ(300,3)" 
                    RenderingImageUrl="../../TemporaryImages/">
                                <NavigationPanel Visible="True">
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="90" Width="90"></Size>
                                </NavigationPanel>
                                <Viewport >
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="100" Width="100"></Size>
                                </Viewport>
                                <ZoomPanel Visible="True">
                                    <Size Height="200" Width="40"></Size>
                                    <Location X="0" Y="24.06417"></Location>
                                    
                                </ZoomPanel>
                                <ColorSwatchPanel Visible="True">
                                    <Location X="0" Y="78.87701"></Location>
                                    <Size Height="80" Width="180"></Size>
                                </ColorSwatchPanel>
                                <DistanceScalePanel>
                                    <Location X="74.1483" Y="85.5615"></Location>
                                    <Size Height="55" Width="130"></Size>
                                </DistanceScalePanel>
                            </dmwc:mapcontrol></td>
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
    </asp:Content>