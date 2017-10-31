<%@ Page Language="C#" AutoEventWireup="true" Codebehind="OOS_0001_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.OOS_0001_0001" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
	<style type="text/css">		
		body 
		{
			font-family: Arial;
			font-size: 16px;
		}
		a {color: #fff;}
		table.htmlGrid th
		{
			padding-bottom: 3px;
			color: #fff;
			font-size: 15px;			
			font-family: Arial;
		}
		table.htmlGrid td
		{
			color: #d1d1d1;			
			font-family: Arial;
			text-align: center;
		}
		table.Calendar th
		{
			padding: 10px 0;			
			color: #fff;
			font-size: 16px;
			font-family: Arial;
		}
		table.Calendar th.MonthTitle,
		table.Calendar th.MonthTitleActive		
		{
			padding: 8px 0;
			font-size: 18px;			
		}	
		table.Calendar th.MonthTitle
		{
			background: url(images/monthBg.png) no-repeat center center;	
		}
		table.Calendar th.MonthTitleActive
		{			
			background: url(images/monthBg_act.png) no-repeat center center;
		}		
		table.Calendar th div.BorderDashed
		{
			border-left: 1px dashed #777;
			background: url(images/dowBg.png) no-repeat center 2px;
		}
		table.Calendar th:first-child div.BorderDashed
		{
			border-left: none;
		}
		table.Calendar td
		{						
			color: #fff;
			font-size: 12px;
			font-family: Arial;
			text-align: center;
		}
		table.Calendar td.Day
		{
			height: 73px;
			background: #000 url(images/dayBg.png) center center;
			vertical-align: top;
		}	
		table.Calendar td.Today
		{
			background: #0080ff url(images/todayBg.png) center center !important;
		}		
		table.Calendar td.Day div.CalendarDay 
		{
			padding-left: 5px;
			font-size: 18px;
			font-weight: bold;
			text-align: left;
		}
		table.Calendar td.Day div.CalendarCount 
		{
			margin-top: -4px;
			padding-bottom: 2px;
		}
		table.Calendar td.Day div.CalendarPrice 
		{
			padding-top: 2px;
			padding-bottom: 6px;
		}
		table.Calendar td.Day a 
		{			
			text-decoration: none;	
		}
		.CalendarScrollButton
		{			 			
			padding-top: 58px;
			text-align: center;
		}
		.BorderTop
		{
			border-top: 1px solid #0067cd;
			/*-webkit-border-image: -webkit-gradient(linear, 0 0, 100% 0, from(#00A9FF), to(#00A9FF), color-stop(0.5, #007FFF)) 1;*/
		}
		.BorderLeft
		{			
			border-left: 1px solid #0067cd;
		}
		.BorderRight
		{
			border-right: 1px solid #0067cd;
		}
		.BorderBottom
		{
			border-bottom: 1px solid #0067cd;
		}
		.TypePurch 
		{			
			font-size: 14px;
			text-align: left !important; 			
			font-weight: bold;	
		}
		.PurWrapper
		{
			color: #d1d1d1;
			margin: 0 5px 15px 5px;	
		}
		.PurBoldWhite 
		{
			color: #fff;
			font-weight: bold;
		}
		.PurMethod
		{
			color: #fff;
			font-size: 14px;			
			font-weight: bold;
		}
		.PurLink 
		{			
			text-align: right;
			font-weight: normal;
			white-space: nowrap;  	
		}		
		.ShadowContest
		{			
			color: #c4eeaf !important;
			text-shadow:0px 0px 0px black,
						0px 0px 3px hsl(100, 63%, 40%),						 
						0px 0px 5px hsl(100, 63%, 35%),
						0px 0px 8px hsl(100, 63%, 30%);
		}
		.ShadowTender
		{			
			color: #b0d8ff !important;
			text-shadow:0px 0px 0px black,
						0px 0px 3px hsl(210, 100%, 40%),						
						0px 0px 5px hsl(210, 100%, 35%),
						0px 0px 8px hsl(210, 100%, 30%);
		}
		.ShadowQuote
		{			
			color: #ffa6cc !important;
			text-shadow:0px 0px 0px black,
						0px 0px 3px hsl(334, 100%, 40%),						
						0px 0px 5px hsl(334, 100%, 35%),
						0px 0px 8px hsl(334, 100%, 30%);
		}
		.ShadowChoice
		{		
			color: #ffd686 !important;	
			text-shadow:0px 0px 0px black,
						0px 0px 3px hsl(40, 100%, 40%), 						
						0px 0px 5px hsl(40, 100%, 35%),
						0px 0px 8px hsl(40, 100%, 30%);
		}
		.ShadowInterest
		{			
			color: #8bffe2 !important;	
			text-shadow:0px 0px 0px black,
						0px 0px 3px hsl(165, 100%, 40%),						
						0px 0px 5px hsl(165, 100%, 35%),
						0px 0px 8px hsl(165, 100%, 30%);
		}
	</style>	
</head>
<body style="background-color: black;" onload="onload();">
	<form id="form1" runat="server">
	
		<touchelementbounds src="../../../TemporaryImages/TouchElementBounds.xml"></touchelementbounds>
		<input type="hidden" id="CurrentMonth" runat="server"/>
		<input type="hidden" id="MonthWidth" runat="server"/>

		<div style="position: absolute; width: 768px; height: 950px; top: 0px; left: 0px; z-index: 2; background-color: black; color: White;">
			
			<uc:iPadElementHeader ID="CalendarHeader" runat="server" width="100%" />			
			<div style="margin-bottom: 20px;">				
				<div class="CalendarScrollButton" style="float: left; width: 50px;"><a href="javascript:ScrollCalendarTo(-1);"><img src="images/scrollBack.png"/></a></div>				
				<div id="CalendarBody" runat="server" style="float: left; width: 668px;"></div>				
				<div class="CalendarScrollButton" style="float: right; width: 50px;"><a href="javascript:ScrollCalendarTo(+1);"><img src="images/scrollNext.png"/></a></div>								
				<div style="clear: both;"></div>
			</div>

			<uc:iPadElementHeader ID="PurchasesHeader" runat="server" width="100%" />			
			<div id="PurchasesBody" runat="server" style="margin-bottom: 20px;"></div>
						
			<uc:iPadElementHeader ID="TopPurchasesHeader" runat="server" width="100%" />			
			<div id="TopPurchasesBody" runat="server" style="margin-bottom: 20px;"></div>
		</div>
	</form>

<SCRIPT type="text/javascript">

	function onload() 
	{
		// заменяем псевдопробелы на нормальные пробелы
		var divs = document.getElementsByTagName('div');
		for (var index = 0; index < divs.length; index++) {
			var div = divs[index];
			if (div.innerHTML.indexOf("__") != -1) {
				div.innerHTML = div.innerHTML.replace("__", " ");
			}
		}

		ScrollCalendar("month_" + document.getElementById("CurrentMonth").getAttribute("value"));
	}

	function ScrollCalendar(id) 
	{
		var monthWidth = parseInt(document.getElementById("MonthWidth").getAttribute("value"));
		var calendar = document.getElementById("calendar");
		var scrollLeft = 0;
		for (var index = 0; index < calendar.childNodes.length; index++) {
			var node = calendar.childNodes.item(index);
			if (node.nodeType == 1) {
				if (node.getAttribute("id") == id) {
					if (index < calendar.childNodes.length - 1) {
						scrollLeft -= (document.getElementById("CalendarBody").offsetWidth - monthWidth) / 2;
					}
					break;
				}
				scrollLeft += monthWidth;
			}
		}
		document.getElementById("CalendarBody").scrollLeft = scrollLeft;
	}

	function ScrollCalendarTo(delta) 
	{
		var maxWidth = parseInt(document.getElementById("calendar").style.width);
		var monthWidth = parseInt(document.getElementById("MonthWidth").getAttribute("value"));
		var scrollLeft = document.getElementById("CalendarBody").scrollLeft;
		var epsilon = 1 / monthWidth;
		var offset = scrollLeft / monthWidth;

		if (delta > 0) {
			scrollLeft = Math.ceil(offset + epsilon) * monthWidth;
		} else if (delta < 0) {
			scrollLeft = Math.floor(offset - epsilon) * monthWidth;
		}
		
		if (scrollLeft < 0) scrollLeft = 0;
		if (scrollLeft > maxWidth) scrollLeft = maxWidth;
		
		document.getElementById("CalendarBody").scrollLeft = scrollLeft;
	}
</SCRIPT>

</body>
</html>
