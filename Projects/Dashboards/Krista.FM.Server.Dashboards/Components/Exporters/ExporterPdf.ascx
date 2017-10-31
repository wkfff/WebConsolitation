<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExporterPdf.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.Exporters.ExporterPdf" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.DocumentExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.DocumentExport" TagPrefix="IgGridDocExp" %>

<asp:LinkButton ID="pdfExportButton" runat="server">.pdf</asp:LinkButton>
<IgGridDocExp:UltraWebGridDocumentExporter id="ultraWebGridDocumentExporter" runat="server" />