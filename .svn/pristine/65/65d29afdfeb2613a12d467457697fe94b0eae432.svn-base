<%@ Page Language="C#" AutoEventWireup="true" Codebehind="OOS_0001_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.OOS_0001_0002" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraChartItem.ascx" TagName="UltraChartItem" TagPrefix="uc"%>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc" %>

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
		table.Switcher
		{
			width: 100%;
			border: 0;
			margin-bottom: 20px;
			padding: 0;
			border-collapse: collapse;	
		}
		table.Switcher td.SwitcherTD
		{
			width: 190px;
			padding: 0 1px;
			vertical-align: top;
		}		
		.BriefSummary
		{
			margin-top: 10px;
			font-size: 16px;
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
		
		<touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
		<includes id="Includes" runat="server"></includes>		

		<div style="position: absolute; width: 768px; height: 950px; background-color: black; top: 0px; left: 0px; z-index: 2;">					

			<div id="alarm" style="display: none; color: white; font-size: 20px; font-weight: bold; text-align: center; margin-top: 400px;">
				Просмотр отчета возможен в приложении<br />версии 3.3.3 (или выше)
			</div>

			<div id="reportWrapper" style="display: none;">			
				<uc:iPadElementHeader ID="PurchasesHeader" runat="server" width="100%" />				
				<div style="color: white;">					
					<table class="Switcher">
						<tr>
						<td class="SwitcherTD">
							<a href="javascript:ShowContent('public');" style="text-decoration: none;"><uc:iPadElementHeader ID="PublicContentHeader" runat="server" width="100%"/></a><div id="PublicBrief"></div>
						</td>
						<td class="SwitcherTD">
							<a href="javascript:ShowContent('give');" style="text-decoration: none;"><uc:iPadElementHeader ID="GiveContentHeader" runat="server" width="100%"/></a><div id="GiveBrief"></div>
						</td>
						<td class="SwitcherTD">
							<a href="javascript:ShowContent('consider');" style="text-decoration: none;"><uc:iPadElementHeader ID="ConsiderContentHeader" runat="server" width="100%"/></a><div id="ConsiderBrief"></div>
						</td>
						<td class="SwitcherTD">
							<a href="javascript:ShowContent('result');" style="text-decoration: none;"><uc:iPadElementHeader ID="ResultContentHeader" runat="server" width="100%"/></a><div id="ResultBrief"></div>
						</td>
						</tr>
					</table>					
					<uc:iPadElementHeader ID="ContentHeader" runat="server" width="100%" />
					<div id="ReportContent"></div>				
				</div>
			</div>
		</div>
	</form>

<SCRIPT type="text/javascript">

	var _needIndication;
	var _timeInterval = 10;
	var _timeIntervalLarge = 500;
	var _step = 10;
	var _date;
	var _data;
	var _index;
	var _contentElement;
	var _contentType;

	function onload() 
	{
		_contentElement = document.getElementById("ReportContent");
		setTimeout(showAlarm, 3000);
		
		// для отладки на компе
		//onview(window.location);
	}
	
	function showAlarm() 
	{
		var alarm = document.getElementById("alarm");
		if (alarm) {
			alarm.style.display = "block";
			document.getElementById("reportWrapper").style.display = "none";
		}
	}

	function isNeedIndication() 
	{
		return _needIndication;
	}

	function onview(arg) 
	{
		var alarm = document.getElementById("alarm");
		if (alarm) {
			alarm.parentNode.removeChild(alarm);
			document.getElementById("reportWrapper").style.display = "block";
		}

		var params = {};
		var args = (arg.toString().substr(arg.toString().indexOf("?")+1)).split("&");
		for (var argument in args) {
			var arr = args[argument].toString().split("=");
			params[arr[0]] = arr[1];
		}

		_needIndication = true;
		_date = params.date;
		_contentElement.innerHTML = "";
		
		setTimeout(LoadData, _timeIntervalLarge);
	}

	function LoadData() 
	{
		var Request = new XMLHttpRequest();
		Request.open("GET", document.getElementById("db_" + _date).getAttribute("src"), false);
		Request.send(null);
		_data = eval("(" + Request.responseText + ")");
		_needIndication = false;
		
		document.getElementById("PurchasesHeader_elementCaption").innerHTML = "Процедуры, проводимые " + FormatDateValue(_date);
		document.getElementById("PublicContentHeader_elementCaption").innerHTML =
			"<div id='PublicBrief' style='text-decoration: underline;'>Объявленные закупки<br /><br /></div>" + "<div class='BriefSummary'>" + (_data.PublicCount ? _data.PublicCount + " шт" : "нет") + "<br />" + (_data.PublicPrice ? _data.PublicPrice : "") + "</div>";
		document.getElementById("GiveContentHeader_elementCaption").innerHTML =
			"<div id='GiveBrief' style='text-decoration: underline;'>Окончание подачи заявок<br /><br /></div>" + "<div class='BriefSummary'>" + (_data.GiveCount ? _data.GiveCount + " шт" : "нет") + "<br />" + (_data.GivePrice ? _data.GivePrice : "") + "</div>";
		document.getElementById("ConsiderContentHeader_elementCaption").innerHTML =
			"<div id='ConsiderBrief' style='text-decoration: underline;'>Окончание рассмотрения заявок</div>" + "<div class='BriefSummary'>" + (_data.ConsiderCount ? _data.ConsiderCount + " шт" : "нет") + "<br />" + (_data.ConsiderPrice ? _data.ConsiderPrice : "") + "</div>";
		document.getElementById("ResultContentHeader_elementCaption").innerHTML =
			"<div id='ResultBrief' style='text-decoration: underline;'>Подведение итогов<br /><br /></div>" + "<div class='BriefSummary'>" + (_data.ResultCount ? _data.ResultCount + " шт" : "нет") + "<br />" + (_data.ResultPrice ? _data.ResultPrice : "") + "</div>";
		
		if (_data.PublicCount && _data.PublicCount > 0) {
			ShowContent("public");
		} else if (_data.GiveCount && _data.GiveCount > 0) {
			ShowContent("give");
		} else if (_data.ConsiderCount && _data.ConsiderCount > 0) {
			ShowContent("consider");
		} else if (_data.ResultCount && _data.ResultCount > 0) {
			ShowContent("result");
		} else {
			_contentElement.innerHTML = "Нет данных";
		}
	}

	function ShowContent(contentType) 
	{
		_index = 0;
		_needIndication = true;
		_contentElement.innerHTML = "";
		_contentType = contentType;

		document.getElementById("PublicBrief").style.fontWeight = "normal";
		document.getElementById("GiveBrief").style.fontWeight = "normal";
		document.getElementById("ConsiderBrief").style.fontWeight = "normal";
		document.getElementById("ResultBrief").style.fontWeight = "normal";
		var title = "";
		switch (_contentType) {
			case "public":
				title = "Объявленные закупки";
				document.getElementById("PublicBrief").style.fontWeight = "bold";
				break;
			case "give":
				title = "Окончание подачи заявок, рассмотрение котировочных заявок";
				document.getElementById("GiveBrief").style.fontWeight = "bold";
				break;
			case "consider":
				title = "Окончание рассмотрения заявок, окончание рассмотрения первых частей заявок на открытый аукцион в электронной форме";
				document.getElementById("ConsiderBrief").style.fontWeight = "bold";
				break;
			case "result":
				title = "Проведение открытого аукциона в электронной форме, подведение итогов открытого конкурса";
				document.getElementById("ResultBrief").style.fontWeight = "bold";
				break;
		}
		document.getElementById("ContentHeader_elementCaption").innerHTML = title;
		setTimeout(ParseData, _timeIntervalLarge);
	}

	function ParseData() 
	{
		for (var i = _index; i < _index + _step; i++) {
			if (i == _data.Array.length) {
				_needIndication = false;
				return;
			}
			PrintPurchase(_data.Array[i]);
		}
		_index += _step;
		setTimeout(ParseData, _timeInterval);
	}

	function PrintPurchase(item) 
	{
		var show = false;
		switch (_contentType) {
			case "public":
				show = (_date == item.RefPublicDate);
				break;
			case "give":
				show = (_date == item.RefGiveDate);
				break;
			case "consider":
				show = (_date == item.RefConsiderDate);
				break;
			case "result":
				show = (_date == item.RefMatchDate || _date == item.RefResultDate);
				break;
		}

		if (show) 
		{
			var methodClass = "";
			var html = "";
			var date;
			
			html += "<div class=\"PurWrapper\">";

			switch (item.MethodID) 
			{
				case 1:
					methodClass = "ShadowContest";
					break;
				case 2:
				case 3:
					methodClass = "ShadowTender";
					break;
				case 4:
					methodClass = "ShadowQuote";
					break;
				case 5:
					methodClass = "ShadowChoice";
					break;
				case 8:
					methodClass = "ShadowInterest";
					break;
			}
			html += "<div class=\"PurMethod " + methodClass + "\">" + item.Method + "</div>";

			html += "<div class=\"PurBoldWhite\">" + item.Purchase + " " +
						"<span class=\"PurLink\"><a href=\"" + item.Link + "\">Подробнее на сайте zakupki.gov.ru >></a></span></div>";


			html += "<div>";
			html += "Источник финансирования:&nbsp;";
			switch (item.BudgetLevel) 
			{
				case "Уровень субъекта РФ":
					html += "<span class=\"PurBoldWhite\">Бюджет субъекта РФ</span>";
					break;
				case "Муниципальный уровень":
					html += "<span class=\"PurBoldWhite\">Бюджет муниципального образования (" + item.Terra + ")</span>";
					break;
			}
			html += "</div>";

			html += "<div>Заказчик:&nbsp;<span class=\"PurBoldWhite\">" + item.Customer + "</span></div>";

			if (item.Price) {
				html += "<div>Начальная максимальная цена контракта:&nbsp;" +
					"<span class=\"PurBoldWhite " + methodClass + "\" style=\"color: #fff !important;\">" + item.Price + "</span></div>";
			}

			var stage = "";
			switch (item.MethodID)
			{
				case 1:
					if (_date >= item.RefPublicDate && _date < item.RefGiveDate)
						stage = "Подача заявок";
					else if (_date == item.RefGiveDate)
						stage = "Вскрытие конвертов с заявками";
					else if (_date > item.RefGiveDate && _date < item.RefResultDate)
						stage = "Рассмотрение заявок";
					else if (_date == item.RefResultDate)
						stage = "Подведение итогов";
					break;
				case 2:
				case 3:
					if (_date >= item.RefPublicDate && _date <= item.RefGiveDate)
						stage = "Подача заявок";
					else if (_date > item.RefGiveDate && _date <= item.RefConsiderDate)
						stage = "Рассмотрение первых частей заявок";
					else if (_date == item.RefMatchDate)
						stage = "Начало аукциона";
					break;
				case 4:
					if (_date >= item.RefPublicDate && _date < item.RefGiveDate)
						stage = "Подача заявок";
					else if (_date == item.RefGiveDate)
						stage = "Подведение итогов";
					break;
				case 5:
					if (_date >= item.RefPublicDate && _date < item.RefGiveDate)
						stage = "Подача заявок";
					else if (_date == item.RefGiveDate)
						stage = "Подведение итогов отбора";
					break;
				case 8:
					if (_date >= item.RefPublicDate && _date < item.RefGiveDate)
						stage = "Подача заявок";
					else if (_date == item.RefGiveDate)
						stage = "Подведение итогов";
					break;
			}
			var stageStatus = "";
			var dates = [item.RefPublicDate, item.RefGiveDate, item.RefConsiderDate, item.RefMatchDate, item.RefResultDate];
			if (_data.GenerationDate < dates.min())
				stageStatus = "(подготовка)";
			else if (_data.GenerationDate > dates.max())
				stageStatus = "(завершено)";
			html += "<div>Стадия:&nbsp;<span class=\"PurBoldWhite\">" + stage + " " + stageStatus + "</span></div>";
			
			if (item.RefPublicDate != -1)
				html += "<div>Публикация извещения:&nbsp;<span class=\"PurBoldWhite\">" + FormatDateValue(item.RefPublicDate) + "</span></div>";

			if (item.RefGiveDate != -1) 
			{
				html += "<div>";
				date = "<span class=\"PurBoldWhite\">" + FormatDateValue(item.RefGiveDate) + "</span>";
				switch (item.MethodID) 
				{
					case 1:
					case 8:
						html += "Вскрытие заявок:&nbsp;" + date;
						break;
					case 4:
					case 5:
						html += "Рассмотрение котировочных заявок:&nbsp;" + date;
						break;
					case 2:
					case 3:
						html += "Окончание подачи заявок:&nbsp;" + date;
						break;
				}
				html += "</div>";
			}

			if (item.RefConsiderDate != -1) 
			{
				html += "<div>";
				date = "<span class=\"PurBoldWhite\">" + FormatDateValue(item.RefConsiderDate) + "</span>";
				switch (item.Method) 
				{
					case 1:
						html += "Рассмотрение заявок:&nbsp;" + date;
						break;
					case 2:
					case 3:
						html += "Рассмотрение первых частей заявок:&nbsp;" + date;
						break;
				}
				html += "</div>";
			}

			if (item.RefMatchDate != -1) 
			{
				html += "<div>";
				date = "<span class=\"PurBoldWhite\">" + FormatDateValue(item.RefMatchDate) + "</span>";
				switch (item.Method) 
				{
					case 2:
					case 3:
						html += "Проведение аукциона в электронной форме:&nbsp;" + date;
						break;
				}
				html += "</div>";
			}

			if (item.RefResultDate != -1) 
			{
				html += "<div>";
				date = "<span class=\"PurBoldWhite\">" + FormatDateValue(item.RefResultDate) + "</span>";
				switch (item.Method) 
				{
					case 1:
						html += "Подведение итогов:&nbsp;" + date;
						break;
				}
				html += "</div>";
			}

			html += "</div>";

			_contentElement.innerHTML += html;
		}
	}
	
	function FormatDateValue(str) 
	{
		return str.substr(6, 2) + "." + str.substr(4, 2) + "." + str.substr(0, 4);
	}

	Array.prototype.max = function() {
		var max = 0;
		var len = this.length;
		for (var i = 0; i < len; i++) 
			if (this[i] > max) max = this[i];
		return max;
	};
	
	Array.prototype.min = function() {
		var min = Number.MAX_VALUE;
		var len = this.length;
		for (var i = 0; i < len; i++) 
			if (this[i] < min) min = this[i];
		return min;
	};
</SCRIPT>

</body>
</html>
