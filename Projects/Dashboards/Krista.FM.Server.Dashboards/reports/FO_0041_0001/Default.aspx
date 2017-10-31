﻿<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.SKK.FO_0041_0001.Default" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<div class="PageTitleWrapper">
		<div class="PageTitleHelp">
			<uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
		</div>		
		<div class="PageTitleExport">
			<uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" /><br />
			<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
		</div>
		<asp:Label ID="PageTitle" runat="server" CssClass="PageTitle" />
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">		
		<div class="Param"><uc3:CustomMultiCombo ID="ComboYear" runat="server" /></div>		
		<div class="Param"><uc3:CustomMultiCombo ID="ComboTax" runat="server" /></div>	
		<div class="Param"><uc1:RefreshButton ID="RefreshButton1" runat="server" /></div>
	</div>    

	<div id="scriptBlock" runat="server"/>
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div style="padding: 5px; overflow: visible;">			
			<uc5:UltraGridBrick ID="GridBrick" runat="server"/>			
			<asp:Label ID="Label1" runat="server" CssClass="PageSubTitle" />
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>
	
</asp:Content>
