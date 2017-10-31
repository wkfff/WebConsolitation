<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UltraGridExporter.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.UltraGridExporter" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.ExcelExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.ExcelExport" TagPrefix="igtblexp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.DocumentExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.DocumentExport" TagPrefix="igtbldocexp" %>
<asp:LinkButton ID="excelExportButton" runat="server">.xls</asp:LinkButton>&nbsp;&nbsp;
<asp:LinkButton ID="pdfExportButton" runat="server">.pdf</asp:LinkButton>&nbsp;&nbsp;
<asp:LinkButton ID="wordExportButton" runat="server" OnClick="wordExportButton_Click" visible="false">.doc</asp:LinkButton>
<igtblexp:UltraWebGridExcelExporter ID="ultraWebGridExcelExporter" runat="server" visible="false"></igtblexp:UltraWebGridExcelExporter>
<igtbldocexp:ultrawebgriddocumentexporter id="ultraWebGridDocumentExporter" runat="server"></igtbldocexp:ultrawebgriddocumentexporter>
