<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0002_Settlement.Default" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    	
	<div class="PageTitleWrapper">
		<div class="PageTitleHelp"> 
			<uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
		</div>		
		<div class="PageTitleExport">
			<uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
			<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
		</div>
		<asp:Label ID="PageTitle" runat="server" CssClass="PageTitle" /><br />
		<asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle" />		
	</div>

	<div class="ParamsWrapper">		
		<div class="Param"><uc3:CustomMultiCombo ID="ArgYear" runat="server" /></div>		
		<div class="Param"><uc3:CustomMultiCombo ID="ArgMonth" runat="server" /></div>	
		<div class="Param"><uc3:CustomMultiCombo ID="ArgArea" runat="server" /></div>
		<div class="Param">
			<asp:RadioButtonList ID="ArgScale" runat="server" RepeatDirection="horizontal" Width="160px">
				<asp:ListItem Selected="True">тыс.руб.</asp:ListItem>
				<asp:ListItem>млн.руб.</asp:ListItem>
            </asp:RadioButtonList>
		</div>
		<div class="Param"><uc1:RefreshButton ID="RefreshButton1" runat="server" /></div>
	</div>    

	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div style="padding: 5px; overflow: visible;">			
			<uc5:UltraGridBrick ID="GridBrick" runat="server"/>			
			<asp:Label ID="Label1" runat="server" CssClass="PageSubTitle" />
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>
		<div class="SpaceHeader"><div><div>
			<asp:Label ID="LabelChart" runat="server" CssClass="ElementTitle" />
		</div></div></div>
		<div class="RoundedCenter"><div><div>
			<div style="text-align: right; margin-right: 10px;">
				<asp:CheckBox ID="CheckFact" runat="server" Text="Факт" AutoPostBack="true" Checked="true"
                    Style="font-family: Verdana; font-size: 10pt;" />
                <asp:CheckBox ID="CheckPlan" runat="server" Text="План" AutoPostBack="true" Checked="false" 
					Style="font-family: Verdana; font-size: 10pt;" />
			</div>
			<igmisc:WebAsyncRefreshPanel ID="asyncChart" runat="server">
				<igchart:UltraChart ID="Chart" runat="server" >
					<DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FO00020002#SEQNUM(100).png" />
				</igchart:UltraChart>
			 </igmisc:WebAsyncRefreshPanel>
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

	<script type="text/javascript">
	<!--
		function uncheck(checkBoxId) 
		{
			var checkbox = document.getElementById(checkBoxId);
			checkbox.checked = !checkbox.checked;
		} 
	// -->
    </script>

</asp:Content>
