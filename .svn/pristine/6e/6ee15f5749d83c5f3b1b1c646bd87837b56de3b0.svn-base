<%@LANGUAGE="VBSCRIPT"%>
<% Option Explicit %>
<!--#include file="config.asp" -->
<%

Call AllowEntry("True", "False", "False", intAsgSecurity)

Dim dtmAsgElabDate
Dim strAsgSelectedLogin
Dim asgOutputPage

'Richiama informazioni
strAsgSelectedLogin = Trim(Request.QueryString("Login"))

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
<title><%= strAsgTxtIPTracking & "&nbsp;" & strAsgTxtFor & "&nbsp;" & strAsgSelectedLogin %></title>
<meta http-equiv="Content-Type" content="text/html; charset=windows-1251" />
<meta name="copyright" content="Copyright (C) 2009-... Krista" />
<meta name="generator" content="ASP Stats Generator <%= strAsgVersion %>" />

<!--#include file="includes/html-head.asp" -->

<!--
  ASP Stats Generator (release <%= strAsgVersion %>) is a free software package
  completely written in ASP programming language, for real time visitor tracking.
  Get your own copy for free at http://www.asp-stats-com/ !
-->

</head>

<%

'HEADER
'---------------------------------------------------|
Response.Write(vbCrLf & "<body bgcolor=""" & strAsgSknPageBgColour & """ background=""" & strAsgSknPageBgImage & """>")

	'CONTENITORE
	'---------------------------------------------------|
Response.Write(vbCrLf & "<table width=""95%"" border=""0"" align=""center"" cellpadding=""1"" cellspacing=""0"" bgcolor=""" & strAsgSknTableLayoutBorderColour & """>")
Response.Write(vbCrLf & "  <tr><td>")
Response.Write(vbCrLf & "	<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"">")
Response.Write(vbCrLf & "	  <tr><td bgcolor=""" & strAsgSknTableLayoutBgColour & """ background=""" & strAsgSknPathImage & strAsgSknTableLayoutBgImage & """>")

'TITOLO BARRA
'---------------------------------------------------|
Response.Write(vbCrLf & "		<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""1"">")
Response.Write(vbCrLf & "		  <tr align=""center"" valign=""middle"">")
Response.Write(vbCrLf & "			<td align=""center"" background=""" & strAsgSknPathImage & strAsgSknTableBarBgImage & """ bgcolor=""" & strAsgSknTableBarBgColour & """ height=""20"" class=""bartitle"">" & UCase(strAsgTxtLoginTracking) & " : " & strAsgSelectedLogin & "</td>")
Response.Write(vbCrLf & "		  </tr>")
Response.Write(vbCrLf & "		  <tr bgcolor=""" & strAsgSknTableLayoutBorderColour & """>")
Response.Write(vbCrLf & "			<td align=""center"" height=""1""></td>")
Response.Write(vbCrLf & "		  </tr>")
Response.Write(vbCrLf & "		</table>")

'TITOLO
'---------------------------------------------------|
Response.Write(vbCrLf & "		<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""1"">")
Response.Write(vbCrLf & "		  <tr valign=""middle"" bgcolor=""" & strAsgSknTableContBgColour & """ class=""normaltitle"">")
Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""center"" widht=""25%"" height=""16"">" & UCase(strAsgTxtTime) & "</td>")
Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""center"" widht=""75%"">" & UCase(strAsgTxtPage) & "</td>")
Response.Write(vbCrLf & "		  </tr>")
	
If NOT Len(strAsgSelectedLogin) > 0 Then 
	Response.Write(vbCrLf & "		  <tr valign=""middle"" bgcolor=""" & strAsgSknTableContBgColour & """ class=""smalltext"">")
	Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""center"" colspan=""2"" height=""16""><br />" & strAsgTxtMissedDataToElab & "<br /><br /></td>")
	Response.Write(vbCrLf & "		  </tr>")
Else	
	strAsgSQL = "SELECT Details_ID, Data, Page FROM "&strAsgTablePrefix&"Detail WHERE Login = '" & strAsgSelectedLogin & "' ORDER BY Data DESC "
	objAsgRs.Open strAsgSQL, objAsgConn	
	
	If NOT objAsgRs.EOF Then		
		dtmAsgElabDate = ""		
		Do While Not objAsgRs.EOF	
			If FormatOutTimeZone(objAsgRs("Data"), "Date") <> dtmAsgElabDate Then
				dtmAsgElabDate = FormatOutTimeZone(objAsgRs("Data"), "Date")
				Response.Write(vbCrLf & "		  <tr valign=""middle"" bgcolor=""" & strAsgSknTableContBgColour & """ class=""smalltext"">")
				Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""left"" colspan=""2"" height=""16""><img src=""images/arrow_small_dx.gif"" align=""absmiddle"" border=""0"" />&nbsp;" & strAsgTxtDetails & "&nbsp;" & dtmAsgElabDate & "</td>")
				Response.Write(vbCrLf & "		  </tr>")
			End If
		
			asgOutputPage = objAsgRs("Page")
			If Mid(asgOutputPage, 1, Len(strAsgSiteURLremote)) = strAsgSiteURLremote Then asgOutputPage = Mid(asgOutputPage, Len(strAsgSiteURLremote) + 1) 
		
			Response.Write(vbCrLf & "		  <tr valign=""middle"" bgcolor=""" & strAsgSknTableContBgColour & """ class=""smalltext"">")
			Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""center"">" & FormatOutTimeZone(objAsgRs("Data"), "Time") & "</td>")
			Response.Write(vbCrLf & "			<td background=""" & strAsgSknPathImage & strAsgSknTableContBgImage & """ align=""left"">")

			Response.Write(ChooseDomainIcon(objAsgRs("Page"), "classic" ))						
			Response.Write(StripValueTooLong(asgOutputPage, 55, 25, 25))			
			Response.Write("</a></td>")
			Response.Write(vbCrLf & "		  </tr>")	
			objAsgRs.MoveNext
		Loop
	Else
		Call BuildTableContNoRecord(2, "standard")				
	End If	
	objAsgRs.Close
End If

Response.Write(vbCrLf & "		</table>")

'Reset Server Objects
Set objAsgRs = Nothing
objAsgConn.Close
Set objAsgConn = Nothing

'FOOTER
'---------------------------------------------------|
Response.Write(vbCrLf & "		<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""1"">")

'SPACER
'---------------------------------------------------|
Response.Write(vbCrLf & "		  <tr bgcolor=""" & strAsgSknTableLayoutBorderColour & """>")
Response.Write(vbCrLf & "			<td align=""center"" height=""1""></td>")
Response.Write(vbCrLf & "		  </tr>")

Response.Write(vbCrLf & "		  <tr align=""center"" valign=""middle"">")
Response.Write(vbCrLf & "			<td align=""center"" background=""" & strAsgSknPathImage & strAsgSknTableBarBgImage & """ bgcolor=""" & strAsgSknTableBarBgColour & """ height=""20"" class=""footer""></td>")
Response.Write(vbCrLf & "		  </tr>")

'CONTENITORE (Chiusura)
'---------------------------------------------------|
Response.Write(vbCrLf & "		</table>")
Response.Write(vbCrLf & "	  </td></tr>")
Response.Write(vbCrLf & "	</table>")
Response.Write(vbCrLf & "  </td></tr>")
Response.Write(vbCrLf & "</table><br />")
Response.Write(vbCrLf & "<div class=""smalltext"" align=""center""><a href=""JavaScript:onClick=window.close();"" class=""linksmalltext"" title=""" & strAsgTxtCloseWindow & """>" & strAsgTxtCloseWindow & "</a></div>")
Response.Write(vbCrLf & "</body></html>")

%>