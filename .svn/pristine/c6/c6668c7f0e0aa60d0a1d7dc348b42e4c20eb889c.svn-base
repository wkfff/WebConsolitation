<%@ LANGUAGE="VBSCRIPT" %>
<% Option Explicit %>
<!--#include file="config.asp" -->
<%

Call AllowEntry("True", "False", "False", intAsgSecurity)


'Dichiara Variabili
Dim mese			'Riferimento per output
Dim gruppo			'
Dim dettagli		'IP di cui mostrare le informazioni
Dim asgOutput
Dim intAsgCount		'Conteggio record


'Read setting variables from querystring
mese = Request.QueryString("mese")
dettagli = Request.QueryString("dettagli")
strAsgSortBy = Request.QueryString("sort")
strAsgSortOrder = Request.QueryString("order")


'If period variable is empty then set it to the current month
If mese = "" Then mese = dtmAsgMonth & "-" & dtmAsgYear
' Set the sorting order depending on querystring
if strAsgSortOrder = "ASC" then 
	strAsgSortOrder = "ASC"
else
	strAsgSortOrder = "DESC"
end if
'If a time period has been chosen then build the variable to query the database
If Request.QueryString("showperiod") = strAsgTxtShow Then mese = Request.QueryString("periodmm") & "-" & Request.QueryString("periodyy")


'Read sorting order from querystring
'// Filter QS values and associate them 
'// with their respective database fields
Select Case strAsgSortBy
	Case "hits" strAsgSortByFld = "SUM(Hits)"
	Case "visits" strAsgSortByFld = "SUM(Visits)"
	Case "ip" strAsgSortByFld = "IP"
	Case "data" strAsgSortByFld = "MAX(Last_Access)"
	Case "login" strAsgSortByFld = "LOGIN"	
	Case Else strAsgSortByFld = "SUM(Visits)"	
End Select

Call DimPaginazioneAvanzata()

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
<title><%= strAsgSiteName %></title>
<meta http-equiv="Content-Type" content="text/html; charset=windows-1251" />
<meta name="copyright" content="Copyright (C) 2009- Krista, All Rights Reserved" />
<meta name="generator" content="ASP Stats Generator <%= strAsgVersion %>" /> <!-- leave this for stats -->

<!--#include file="includes/html-head.asp" -->

<!--
  ASP Stats Generator (release <%= strAsgVersion %>) is a free software package
  completely written in ASP programming language, for real time visitor tracking.
  Get your own copy for free at http://www.asp-stats-com/ !
-->

</head>
<!--#include file="includes/header.asp" -->
		<table width="100%" border="0" align="center" cellpadding="0" cellspacing="1">
		  <tr align="center" valign="middle">
			<td align="center" background="<%= strAsgSknPathImage & strAsgSknTableBarBgImage %>" bgcolor="<%= strAsgSknTableBarBgColour %>" height="20" class="bartitle"><%= UCase(strAsgTxtIPAddress) %></td>
		  </tr>
		  <tr bgcolor="<%= strAsgSknTableLayoutBorderColour %>">
			<td align="center" height="1"></td>
		  </tr>
		</table><br />
		<table width="90%" border="0" align="center" cellpadding="1" cellspacing="1">
		  <tr bgcolor="<%= strAsgSknTableTitleBgColour %>" align="center" class="normaltitle">
			<td width="5%"  background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" height="16"></td>
			<td width="35%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>"><%= UCase(strAsgTxtLogin) %>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=login&order=DESC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtLogin & "&nbsp;" & strAsgTxtDesc %>">
				<img src="<%= strAsgSknPathImage%>arrow_down.gif" border="0" align="absmiddle" /></a>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=login&order=ASC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtLogin & "&nbsp;" & strAsgTxtAsc %>">
				<img src="<%= strAsgSknPathImage%>arrow_up.gif" border="0" align="absmiddle" /></a></td>			
			<td width="12%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>"><%= UCase(strAsgTxtLastAccess) %>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=data&order=DESC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtLastAccess & "&nbsp;" & strAsgTxtDesc %>">
				<img src="<%= strAsgSknPathImage%>arrow_down.gif" border="0" align="absmiddle" /></a>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=data&order=ASC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtLastAccess & "&nbsp;" & strAsgTxtAsc %>">
				<img src="<%= strAsgSknPathImage%>arrow_up.gif" border="0" align="absmiddle" /></a></td>
			<td width="12%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>"><%= UCase(strAsgTxtSmHits) %>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=hits&order=DESC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtHits & "&nbsp;" & strAsgTxtDesc %>">
				<img src="<%= strAsgSknPathImage%>arrow_down.gif" border="0" align="absmiddle" /></a>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=hits&order=ASC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtHits & "&nbsp;" & strAsgTxtAsc %>">
				<img src="<%= strAsgSknPathImage%>arrow_up.gif" border="0" align="absmiddle" /></a></td>
			<td width="12%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>"><%= UCase(strAsgTxtSmVisits) %>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=visits&order=DESC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtVisits & "&nbsp;" & strAsgTxtDesc %>">
				<img src="<%= strAsgSknPathImage%>arrow_down.gif" border="0" align="absmiddle" /></a>
				<a href="login_stats.asp?<%= "mese=" & mese & "&dettagli=" & dettagli & "&sort=visits&order=ASC" %>" title="<%= strAsgTxtOrderBy & "&nbsp;" & strAsgTxtVisits & "&nbsp;" & strAsgTxtAsc %>">
				<img src="<%= strAsgSknPathImage%>arrow_up.gif" border="0" align="absmiddle" /></a></td>
			<td width="4%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>"></td>
		  </tr>
		<%
		
		'Initialise SQL string to select data
		strAsgSQL = "SELECT Login, Max(Last_Access) AS MaxData, Sum(Visits) AS SumVisits, Sum(Hits) AS SumHits FROM "&strAsgTablePrefix&"Login "
		'Call the function to search into the database if there are enought information to do that
		strAsgSQL = CheckSearchForData(strAsgSQL, true)
		'Group information by following fields
		strAsgSQL = strAsgSQL & " GROUP BY Login "
		'Order record by selected field 
		strAsgSQL = strAsgSQL & " ORDER BY " & strAsgSortByFld & " " & strAsgSortOrder & ""
		
		'Prepara il Rs
		objAsgRs.CursorType = 3
		objAsgRs.LockType = 3
		
		'Apri il Rs
		objAsgRs.Open strAsgSQL, objAsgConn
			
			'Il Rs  vuoto
			If objAsgRs.EOF Then
				
				'If it is a search query then show no results advise
				If Len(asgSearchfor) > 0 AND Len(asgSearchin) > 0 Then
	
					'// Row - No current record	for search terms		
					Call BuildTableContNoRecord(6, "search")
					
				'Else show general no record information
				Else
	
					'// Row - No current record			
					Call BuildTableContNoRecord(6, "standard")
					
				End If
				
			Else

				objAsgRs.PageSize = RecordsPerPage
				objAsgRs.AbsolutePage = page
				intAsgCount = (RecordsPerPage * (page-1))

				
				For PaginazioneLoop = 1 To RecordsPerPage
					
					If Not objAsgRs.EOF Then
					intAsgCount = intAsgCount + 1

		%>		  
		  <tr class="smalltext" bgcolor="<%= strAsgSknTableContBgColour %>">
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="center"><%= intAsgCount %></td>
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="left">&nbsp;<%
									
				'Write an anchor
				Response.Write(vbCrLf & "<a name=""" & objAsgRs("Login") & """></a>")
	
				'Espandi Dettagli
				'// Link
				Response.Write(vbCrLf & "				<a href=""login_stats.asp?dettagli=" & objAsgRs("Login") & "&mese=&page=" & page & "&searchfor=" & asgSearchfor & "&searchin=" & asgSearchin & "&sort=" & strAsgSortBy & "&order=" & strAsgSortOrder & "#" & objAsgRs("Login") & """ title=""" & strAsgTxtShowLoginInformation & """>")
				'// Icona espansa se Corrisponde
				If Trim(dettagli) <> "" AND objAsgRs("Login") = Trim(dettagli) Then
					Response.Write(vbCrLf & "				<img src=""" & strAsgSknPathImage & "expanded.gif"" alt=""" & strAsgTxtShowLoginInformation & """ border=""0"" align=""absmiddle"" />")
				'// Icona espandi se Differente
				Else
					Response.Write(vbCrLf & "				<img src=""" & strAsgSknPathImage & "expand.gif"" alt=""" & strAsgTxtShowLoginInformation & """ border=""0"" align=""absmiddle"" />")
				End If
				'// Chiudi Link
				Response.Write("</a>&nbsp;")
				
				'Mostra solo se Loggato
				If Session("AsgLogin") = "Logged" Then
	
					'Icona Filter IP
					Call ShowIconFilterIp(objAsgRs("Login"))
						
				End If
				
				%>
				<a href="JavaScript:openWin('popup_tracking_login.asp?login=<%= objAsgRs("Login") %>','profile','toolbar=0,location=0,status=0,menubar=0,scrollbars=1,resizable=1,width=550,height=425')" class="linksmalltext" title="<%= strAsgTxtLoginTracking %>"><%= HighlightSearchKey(objAsgRs("Login"), "Login") %></a>
			</td>
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="center"><%= FormatOutTimeZone(objAsgRs("MaxData"), "Date") %></td>
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="right"><%= objAsgRs("SumHits") %></td>
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="right"><%= objAsgRs("SumVisits") %></td>
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="center">
			<% 
				
			'// Link PopUp
			Response.Write(vbCrLf & "				<a href=""JavaScript:openWin('popup_tracking_login.asp?login=" & objAsgRs("Login") & "','Tracking','toolbar=0,location=0,status=0,menubar=0,scrollbars=1,resizable=1,width=550,height=425')"" title=""" & strAsgTxtLoginTracking & """>")
			'// Icona espansa se Corrisponde
			Response.Write(vbCrLf & "				<img src=""" & strAsgSknPathImage & "tracking.gif"" alt=""" &  strAsgTxtLoginTracking & """ border=""0"" />")
			'// Chiudi Link PopUp
			Response.Write("</a>")

			%>
			</td>
		  </tr>
		<% 
			If Trim(dettagli) <> "" AND objAsgRs("Login") = Trim(dettagli) Then
				
				Dim objAsgRs2
				
				'Mostra le query al motore
				Set objAsgRs2 = Server.CreateObject("ADODB.Recordset")
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Login WHERE Login = '" & dettagli & "' "
				strAsgSQL = strAsgSQL & " ORDER BY Visits DESC, Hits DESC"
		
		
		%>
		  <tr class="smalltext">
			<td colspan="7"><br />
				<!-- Contenitore Dettagli -->
				<table width="100%" border="0" cellspacing="0" cellpadding="1" align="center">
				  <tr>
					<td width="7%" valign="top" align="center"><img src="<%= strAsgSknPathImage %>openarrow.gif" width="25" height="25" border="0" alt="<%= strAsgTxtDetails %>"></td>
					<td width="86%">
					<!-- Dettagli IP -->
					<table width="100%" border="0" align="center" cellpadding="1" cellspacing="1">
					  <tr bgcolor="<%= strAsgSknTableTitleBgColour %>" align="center" class="normaltitle">
						<td background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" width="40%" height="16"><%= UCase(strAsgTxtLogin) %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" width="30%"><%= UCase(strAsgTxtLastAccess) %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" width="15%"><%= UCase(strAsgTxtSmHits) %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" width="15%"><%= UCase(strAsgTxtSmVisits) %></td>
					  </tr>
					  <% 
					  objAsgRs2.Open strAsgSQL, objAsgConn
						If objAsgRs2.EOF Then
							
							'// Row - No current record			
							Call BuildTableContNoRecord(4, "standard")
							
						Else
							Do While NOT objAsgRs2.EOF

					  %>
					  <tr class="smalltext" bgcolor="<%= strAsgSknTableContBgColour %>">
						<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="left" height="16">&nbsp;<%= objAsgRs2("Login") %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="center"><%= FormatOutTimeZone(objAsgRs2("Last_Access"), "Date") %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="right"><%= objAsgRs2("Hits") %></td>
						<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" align="right"><%= objAsgRs2("Visits") %></td>
					  </tr>
					  <%
							objAsgRs2.MoveNext
							Loop
						End If
								
						'// Row - End table spacer			
						Call BuildTableContEndSpacer(4)
				
					  objAsgRs2.Close
					  Set objAsgRs2 = Nothing
					  %>
					</table><br />
					</td>
					<td width="7%"></td>
				  </tr>
				</table>
			</td>
		  </tr>
		
		<%
			End If		
				objAsgRs.MoveNext
				End If
			Next
			End If
				
		'// Row - End table spacer			
		Call BuildTableContEndSpacer(7)

		'// Row - Advanced data sorting
		Response.Write(vbCrLf & "<tr class=""smalltext""><td colspan=""7"" align=""center""><br />")
		Call PaginazioneAvanzata("login_stats.asp", "")
		Response.Write(vbCrLf & "<br /><br /></td></tr>")
	
		objAsgRs.Close
		
		'Reset Server Objects
		Set objAsgRs = Nothing
		objAsgConn.Close
		Set objAsgConn = Nothing

		'// Row - Data output panels
		Response.Write(vbCrLf & "<tr class=""smalltext"" align=""center"" valign=""top""><td colspan=""7"" height=""25""><br />")
		Call SearchForData("login_stats.asp", "", "Login")
		Response.Write(vbCrLf & "</td></tr>")
		%>		  
		</table><br />
<%

' Footer
Response.Write(vbCrLf & "<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""1"">")
'// Row - Footer Border Line
Call BuildFooterBorderLine()

Response.Write("<tr align=""center"" valign=""middle"">")
Response.Write("<td align=""center"" background=""" & strAsgSknPathImage & strAsgSknTableBarBgImage & """ bgcolor=""" & strAsgSknTableBarBgColour & """ height=""20"" class=""footer"">")
if blnAsgElabTime then Response.Write(asgElabtime())
Response.Write("</td>")
Response.Write("</tr>")

Response.Write("</table>")
Response.Write("</td></tr>")
Response.Write("</table>")
Response.Write("</td></tr>")
Response.Write("</table>")

%>
<!-- footer -->
<!--#include file="includes/footer.asp" -->
</body></html>