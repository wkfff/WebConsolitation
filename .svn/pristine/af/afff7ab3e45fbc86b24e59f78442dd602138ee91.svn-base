<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.reports.SKK.SKK_004.Default" %>

<%@ Register Src="../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>
<%@ Register Src="../../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<div class="PageTitleWrapper">
		<div class="PageTitleHelp">
			<uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
		</div>
		<div class="PageTitleExport">
			<uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" /><br />
			<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
		</div>
		<asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle" /><br />
		<asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle" />
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">
		<div class="Param"><uc3:CustomMultiCombo ID="comboMark" runat="server" /></div>
		<div class="Param"><uc3:CustomMultiCombo ID="comboDirection" runat="server" /></div>
		<div class="Param"><uc3:CustomMultiCombo ID="comboTransport" runat="server" /></div>
		<br />
		<div class="ParamLabel" style="width: 150px;"><asp:Label ID="txtPeriod" runat="server" Text="" /></div>
		<div class="Param"><uc3:CustomMultiCombo ID="comboPeriodYear" runat="server" /></div>
		<div class="Param"><uc3:CustomMultiCombo ID="comboPeriodMonth" runat="server" /></div>
		<div class="Param"><uc1:RefreshButton ID="RefreshButton1" runat="server" /></div>
	</div>    

	<div id="scriptBlock" runat="server"/>

	<!-- карта РФ, ПП по участкам -->
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div>			
			<div class="SpaceMap">	
				<DMWC:MapControl ID="DundasMapRF" runat="server" BackColor="White" ImageUrl="../../../TemporaryImages/map_skk040_#SEQ(300,3)"
					RenderingImageUrl="../../../TemporaryImages/">
				</DMWC:MapControl>
			</div>			
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

	<!-- карта субъекта, ПП по субъектам -->
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>
		<div class="SpaceHeader"><div><div>
			<asp:Label ID="labelSubjects" runat="server" CssClass="ElementTitle"/>
		</div></div></div>		
		<div class="RoundedCenter"><div><div>		
			<div class="SpaceGrid" style="float: left; width: 400px;">	
				<uc5:UltraGridBrick ID="GridSubjects" runat="server"/>			
			</div>
			<div class="SpaceGrid" style="float: left; width: 400px;">	
				<uc5:UltraGridBrick ID="GridBorders" runat="server"/>			
			</div>
			<div class="SpaceMap" style="float: left;">	
				<DMWC:MapControl ID="DundasMapSubject" runat="server" BackColor="White" ImageUrl="../../../TemporaryImages/map_skk041_#SEQ(300,3)"
					RenderingImageUrl="../../../TemporaryImages/">					
				</DMWC:MapControl>
			</div>
			<div style="clear: both;"></div>		
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

</asp:Content>
