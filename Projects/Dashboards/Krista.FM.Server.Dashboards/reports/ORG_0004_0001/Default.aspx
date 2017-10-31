<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.ORG_0004_0001.Default" %>

<%@ Register Src="../../Components/FindButton.ascx" TagName="FindButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc2" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc9" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc8" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<table width="100%">
		<tr>
			<td valign="top" style="width: 80%; vertical-align: top;">
				<uc2:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
				&nbsp;&nbsp;
				<asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br />
				<asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
			</td>
			<td align="right">
				<div>
					<uc9:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
					&nbsp;<uc8:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
					<br />
					<asp:HyperLink ID="CrossLink" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
					<asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink></div>
			</td>
		</tr>
	</table>

	<table style="vertical-align: top; border-collase: collapse">
		<tr>
			<td valign="top">
				<div id="wrapper" onkeydown="javascript:resetFind(event); EnableSubmitButton();">
					<uc3:CustomMultiCombo ID="ComboRegions" runat="server"></uc3:CustomMultiCombo>
				</div>
			</td>
			<td valign="top">
				<div id="wrapper" onkeydown="javascript:resetFind(event); EnableSubmitButton();">
					<uc3:CustomMultiCombo ID="ComboDebts" runat="server"></uc3:CustomMultiCombo>
				</div>
			</td>
			<td>&nbsp;</td>
		</tr>
	</table>
	<table style="vertical-align: top; border-collase: collapse;">
		<tr>
			<td>
				<asp:Label ID="Label1" runat="server" CssClass="PageSubTitle"></asp:Label>
			</td>
			<td>
				<asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
			</td>
			<td>
				<asp:Label ID="Label3" runat="server" CssClass="PageSubTitle"></asp:Label>
			</td>
			<td>&nbsp;</td>
		</tr>
		<tr>		
			<td>
				<asp:TextBox ID="innText" onkeydown="javascript:resetFind(event);EnableSubmitButton();"
					runat="server"></asp:TextBox>
			</td>
			<td>
				<asp:TextBox ID="kppText" onkeydown="javascript:resetFind(event);EnableSubmitButton();"
					runat="server"></asp:TextBox>
			</td>
			<td>
				<asp:TextBox ID="nameText" onkeydown="javascript:resetFind(event);EnableSubmitButton();"
					runat="server"></asp:TextBox>
			</td>
			<td valign="top">
				<uc1:FindButton ID="RefreshButton1" runat="server" />
			</td>
		</tr>
	</table>
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
			<asp:HiddenField ID="PageNumber" runat="server"/>
				<asp:Label ID="Label4" runat="server" CssClass="PageSubTitle"></asp:Label>
				<table style="border-collapse: collapse; width: 100%">
					<tr>
					<td style="width: 90px; text-align: left;">&nbsp;<asp:HyperLink ID="linkFirst" runat="server" />&nbsp;</td>
					<td style="width: 90px; text-align: center;">&nbsp;<asp:HyperLink ID="linkPrev" runat="server" />&nbsp;</td>
					<td>&nbsp;</td>
					<td style="width: 90px; text-align: center;">&nbsp;<asp:HyperLink ID="linkNext" runat="server" />&nbsp;</td>
					<td style="width: 90px; text-align: right;">&nbsp;<asp:HyperLink ID="linkLast" runat="server" />&nbsp;</td>
					</tr>
				</table>
				
				
				
				
				<uc5:UltraGridBrick ID="UltraWebGrid1" runat="server"/>				
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
	<script type='text/javascript' language='JavaScript'>
		function resetFind(e) {
			var keyCode = e.keyCode;
			if (keyCode == 13) {
				if (e.stopPropagation != null) e.stopPropagation();
				if (e.preventDefault != null) e.preventDefault();
				e.cancelBubble = true;
				e.returnValue = false;
				__doPostBack(this.ID, '');
			}
		}
	</script>
</asp:Content>
