<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.SKK.SKK_018.Default" %>

<%@ Register Src="../../../Components/Exporters/ExporterPdf.ascx" TagName="ExporterPdf" TagPrefix="uc" %>
<%@ Register Src="../../../Components/Exporters/ExporterDoc.ascx" TagName="ExporterDoc" TagPrefix="uc" %>
<%@ Register Src="../../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<div class="PageTitleWrapper">
		<div class="PageTitleHelp">
			<uc:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
		</div>
		<div class="PageTitleExport">
			<uc:ExporterDoc ID="ExporterDoc" runat="server" /><br />
			<uc:ExporterPdf ID="ExporterPdf" runat="server" />
		</div>
		<asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle" /><br />
		<asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle" />
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">		
		<div class="ParamLabel" style="width: 120px;"><asp:Label ID="txtPeriod" runat="server" Text="" /></div>
		<div class="Param"><uc:CustomMultiCombo ID="comboPeriodYear" runat="server" /></div>
		<div class="Param"><uc:CustomMultiCombo ID="comboPeriodMonth" runat="server" /></div>
		<div class="Param"><uc:CustomMultiCombo ID="comboBorder" runat="server" /></div>
		<div class="Param"><uc:RefreshButton ID="RefreshButton1" runat="server" /></div>     
	</div>    
		
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>
		<div class="RoundedCenter"><div><div>
			<div class="SpaceGrid">	
				<div class="SpaceText" id="report" runat="server"></div>
			</div>
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

</asp:Content>
