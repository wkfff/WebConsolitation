<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.EO_0010_0001.Default" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc5" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table id="HeaderTable" runat="server" style="vertical-align: top; border-collapse: collapse" width="100%">
        <tr>
            <td colspan="2">
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
                <uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                &nbsp;
                <uc5:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
        <tr>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboYears" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboExecutor" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse; background-color: White; margin-top: 10px;">
        <tr>
            <td style="overflow: visible;">
                <asp:Label ID="Label" runat="server" CssClass="PageSubTitle"></asp:Label>
                <igtbl:UltraWebGrid ID="Grid" runat="server" SkinID="UltraWebGrid">
                </igtbl:UltraWebGrid>
            </td>
        </tr>
    </table>
</asp:Content>
