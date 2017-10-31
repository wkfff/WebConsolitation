<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Sport_0001_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.Sport_0001_0002" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraChartItem.ascx" TagName="UltraChartItem" TagPrefix="uc"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
</head>
<body style="background-color: black;">
	<form id="form1" runat="server">
		<touchelementbounds src="../../../TemporaryImages/TouchElementBounds.xml"></touchelementbounds>
		<div style="position: absolute; width: 768px; height: 950px; background-color: black; top: 0px; left: 0px; z-index: 2;">
			
			<div style="position: relative;">
				<uc:iPadElementHeader ID="SporterHeader" runat="server" Width="100%" />				
				<div style="position: absolute; top: 5px; right: 6px;"><a href="" id="SporterDetailTime" runat="server"><img src="../../../images/detail.png"/></a></div>
			</div>
			<div style="margin-bottom: 10px;">
				<div style="float: left; width: 270px; margin: 0 5px 0 5px;">
					<uc:UltraChartItem ID="UltraChartSporter" runat="server"></uc:UltraChartItem>		
				</div>
				<div style="float: left; width: 480px;"><asp:Label ID="SporterText" runat="server" SkinID="InformationText"/></div>
				<div style="clear: both;"></div>
			</div>

			<div style="position: relative;">
				<uc:iPadElementHeader ID="StaffHeader" runat="server" Width="100%" />
				<div style="position: absolute; top: 5px; right: 6px;"><a href="" id="StaffDetailTime" runat="server"><img src="../../../images/detail.png"/></a></div>
			</div>
			<div style="margin-bottom: 20px;">
				<div style="float: left; width: 270px; margin: 0 5px 0 5px;">
					<uc:UltraChartItem ID="UltraChartStaff" runat="server"></uc:UltraChartItem>		
				</div>
				<div style="float: left; width: 480px; margin-bottom: 10px;"><asp:Label ID="StaffText" runat="server" SkinID="InformationText"/></div>
				<div style="clear: both;"></div>				
			</div>
			
			<div style="position: relative;">
				<uc:iPadElementHeader ID="RecreationHeader" runat="server" Width="100%" />
				<div style="position: absolute; top: 5px; right: 6px;"><a href="" id="RecreationDetailTime" runat="server"><img src="../../../images/detail.png"/></a></div>
			</div>
			<div style="margin: 0 10px;">
				<asp:Label ID="RecreationText" runat="server" SkinID="InformationText"/>				
			</div>
			
		</div>		
	</form>
</body>
</html>
	