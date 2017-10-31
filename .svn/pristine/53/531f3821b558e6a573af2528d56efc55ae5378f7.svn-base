<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
	Inherits="Krista.FM.Server.Dashboards.MinSport_099.Default.reports.MinSport.MinSport_099.Default" %>


<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc5" %>
<%@ Register Src="../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	

	<div class="PageTitleWrapper">
		<asp:Label ID="PageTitle" runat="server" CssClass="PageTitle" /> <br/>
        <div class="PageTitleExport">
            <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
		</div>
		<div class="clear"></div>
	</div>

	<div class="ParamsWrapper">
        <asp:Label ID="LabelEnterCode" runat="server"/>
        <asp:TextBox ID="EnterCodeBox" runat="server" width="100"/>	
    </div>
    <br />
    <div class="ParamsWrapper">
        <asp:DropDownList ID="FactorList" runat="server" 
            onselectedindexchanged="FactorList_SelectedIndexChanged" AutoPostBack="true" Width="600"/>	
        <asp:DropDownList ID="YearList" runat="server" Width="100" />
        <br /><br />
        <uc3:CustomMultiCombo ID="MultiTerritory" runat="server" Width="700" />
        <br /> 
	    <asp:Button ID="BtnGetData" runat="server" 
            Text="Выполнить" Width="200px" onclick="BtnGetData_Click" />  
	</div>    

     <div class="ParamsWrapper">
        <asp:CheckBox ID="VisibleTotal" runat="Server" Text="Видимость элементов 'всего'" Checked="true" />
        <asp:CheckBox ID="VisibleDefinition" runat="Server" Text="Видимость столбца описания данных" Checked="false" />
    </div>


	<!-- таблица -->	
	<div class="SpaceWrapper">
		<div class="RoundedTop"><div><div></div></div></div>		
		<div class="RoundedCenter"><div><div>
            <div class="SpaceText" id="report" runat="server">								
			</div>
			<div class="SpaceGrid">	
				<uc5:ultragridbrick ID="GridBrick" runat="server"/>			
			</div>
		</div></div></div>
		<div class="RoundedBottom"><div><div></div></div></div>
	</div>

</asp:Content>
