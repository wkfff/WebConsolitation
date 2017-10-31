<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.MinSport.MinSport_001.Default" %>

<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<div class="PageTitleWrapper">
		<asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle" /><br />
		<asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle" />
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">
		<div class="Param"><uc3:CustomMultiCombo ID="comboTerritory" runat="server" /></div>
		<div class="Param"><uc3:CustomMultiCombo ID="comboYear" runat="server" /></div>	
		<div class="Param"><uc1:RefreshButton ID="RefreshButton1" runat="server" /></div>					
	</div>    

	<!-- таблица -->	
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div>
			<div class="SpaceGrid">	
				<uc5:UltraGridBrick ID="GridBrick" runat="server"/>			
			</div>
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

</asp:Content>
