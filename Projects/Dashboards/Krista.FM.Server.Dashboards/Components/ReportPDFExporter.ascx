<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportPDFExporter.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.ReportPDFExporter" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.DocumentExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.DocumentExport" TagPrefix="igtbldocexp" %>
<asp:LinkButton ID="pdfExportButton" runat="server">.pdf</asp:LinkButton>&nbsp;&nbsp;
<igtbldocexp:ultrawebgriddocumentexporter id="ultraWebGridDocumentExporter" runat="server"></igtbldocexp:ultrawebgriddocumentexporter>
