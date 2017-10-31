<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0016_0005.Default" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;<asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label><br/>
                <asp:Label ID="CommentTextLabel" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter
                    ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table style="vertical-align: top;">
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboQuarter" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboMOGroup" runat="server"></uc3:CustomMultiCombo>
                        </td>                        
                        <td valign="top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
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
                                                   <uc5:UltraGridBrick ID="MainGridBrick" runat="server">
                            </uc5:UltraGridBrick>
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
        </tr>
    </table>
</asp:Content>
