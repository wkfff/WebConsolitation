<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.SKK.STAT_0005_0001.Default" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>


<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<div class="PageTitleWrapper">
		<div class="PageTitleHelp">
			<uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
		</div>		
		<asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle" /><br />
		<asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle" />
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">		
		<div class="Param"><uc3:CustomMultiCombo ID="ComboSection" runat="server" /></div>		
		<div class="Param"><uc1:RefreshButton ID="RefreshButton1" runat="server" /></div>
	</div>    

	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div style="overflow: visible">			
			<div class="SpaceText" id="ReportText" runat="server">								
			</div>
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

</asp:Content>
