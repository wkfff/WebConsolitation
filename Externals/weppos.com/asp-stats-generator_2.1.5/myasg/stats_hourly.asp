<%@LANGUAGE="VBSCRIPT"%>
<% Option Explicit %>
<!--#include file="config.asp" -->
<%

'/**
' * ASP Stats Generator - Powerful and reliable ASP website counter
' *
' * This file is part of the ASP Stats Generator package.
' * (c) 2003-2008 Simone Carletti <weppos@weppos.net>, All Rights Reserved
' *
' * 
' * COPYRIGHT AND LICENSE NOTICE
' *
' * The License allows you to download, install and use one or more free copies of this program 
' * for private, public or commercial use.
' * 
' * You may not sell, repackage, redistribute or modify any part of the code or application, 
' * or represent it as being your own work without written permission from the author.
' * You can however modify source code (at your own risk) to adapt it to your specific needs 
' * or to integrate it into your site. 
' *
' * All links and information about the copyright MUST remain unchanged; 
' * you can modify or remove them only if expressly permitted.
' * In particular the license allows you to change the application logo with a personal one, 
' * but it's absolutly denied to remove copyright information,
' * including, but not limited to, footer credits, inline credits metadata and HTML credits comments.
' *
' * For the full copyright and license information, please view the LICENSE.htm
' * file that was distributed with this source code.
' *
' * Removal or modification of this copyright notice will violate the license contract.
' *
' *
' * @category        ASP Stats Generator
' * @package         ASP Stats Generator
' * @author          Simone Carletti <weppos@weppos.net>
' * @copyright       2003-2008 Simone Carletti
' * @license         http://www.weppos.com/asg/en/license.asp
' * @version         SVN: $Id: stats_hourly.asp 150 2008-04-27 19:17:20Z weppos $
' */
 
'/* 
' * Any disagreement of this license behaves the removal of rights to use this application.
' * Licensor reserve the right to bring legal action in the event of a violation of this Agreement.
' */


'// ATTENZIONE! Protezione statistiche.
'	Modificare solo se necessario e se sicuri.
'	Impostazioni errate possono compromettere la privacy.
Call AllowEntry("True", "False", "False", intAsgSecurity)


'Dichiara Variabili
Dim mese			'Riferimento per output
Dim giorno			'Riferimento per output
Dim elenca			'Tutti | Mese
Dim intAsgCiclo


Dim intAsgNumCol		'Numero Colonne
Dim intAsgAltColMax		'Altezza massima in px delle colonne dipendente dalla pag
Dim intAsgLarCol		'Larghezza delle colonne dipendente dalla pag

Dim intAsgMax				'Valore massimo di pagine visitate

Dim intAsgTotMeseHits		'Valore totale per mese di pagine visitate
Dim intAsgTotMeseVisits		'Valore totale per mese di accessi unici

Dim intAsgParte

Dim intAsgValHits(24)		'Valori assunti per l'immagine
Dim intAsgValVisits(24)		'Valori assunti per l'immagine

Dim intAsgTotHits(24)		'Valore totale di pagine visitate 	| Per statistica grafica
Dim intAsgTotVisits(24)		'Valore totale di accessi unici		| Per statistica grafica


'Read setting variables from querystring
mese = Request.QueryString("mese")
elenca = Request.QueryString("elenca")


'If period variable is empty then set it to the current month
If mese = "" Then mese = dtmAsgMonth & "-" & dtmAsgYear
'If the variable is empty set it to monthly report
If elenca = "" Then elenca = "mese"
'If a time period has been chosen then build the variable to query the database
If Request.QueryString("showperiod") = strAsgTxtShow Then mese = Request.QueryString("periodmm") & "-" & Request.QueryString("periodyy")


'Set max total column width
intAsgNumCol = 24 + 2				'Numero colonne | 1 per ogni ora
intAsgAltColMax = 200				'Altezza massima colonne | Rapportata alla dimensione della pagina
intAsgLarCol = (800/intAsgNumCol)	'Larghezza per ogni colonna | Calcolata sul totale delle necessarie


'Richiama totale
If elenca = "mese" Then 
	strAsgSQL = "SELECT Sum(Hits) As SumHits, Sum(Visits) AS SumVisits FROM "&strAsgTablePrefix&"Hourly WHERE Mese = '" & mese & "' "
ElseIf elenca = "tutti" Then 
	strAsgSQL = "SELECT Sum(Hits) As SumHits, Sum(Visits) AS SumVisits FROM "&strAsgTablePrefix&"Hourly "
End If
objAsgRs.Open strAsgSQL, objAsgConn
If objAsgRs.EOF Then
	intAsgTotMeseHits = 0
	intAsgTotMeseVisits = 0
Else
	intAsgTotMeseHits = objAsgRs("SumHits")
	intAsgTotMeseVisits = objAsgRs("SumVisits")
End If
objAsgRs.Close
'Accertati che non siano nulli
If intAsgTotMeseHits = 0 OR "[]" & intAsgTotMeseHits = "[]" Then intAsgTotMeseHits = 0
If intAsgTotMeseVisits = 0 OR "[]" & intAsgTotMeseVisits = "[]" Then intAsgTotMeseVisits = 0


'Richiama valore Massimo
If elenca = "mese" Then 
	strAsgSQL = "SELECT Max(Hits) As MaxHits FROM "&strAsgTablePrefix&"Hourly WHERE Mese = '" & mese & "' "
ElseIf elenca = "tutti" Then 
	strAsgSQL = "SELECT Sum(Hits) AS MaxHits FROM "&strAsgTablePrefix&"Hourly GROUP BY Ora ORDER BY Sum(Hits) DESC"
End If
objAsgRs.Open strAsgSQL, objAsgConn
If objAsgRs.EOF Then
	intAsgMax = 0
Else
	objAsgRs.MoveFirst
	intAsgMax = objAsgRs("MaxHits")
End If
objAsgRs.Close


'Calcola unit� singola
If intAsgMax = 0 OR "[]" & intAsgMax = "[]" Then intAsgMax = 1
intAsgParte = intAsgAltColMax/intAsgMax


'Richiama valori statistica
If elenca = "mese" Then 
	
	strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Hourly WHERE Mese = '" & mese & "' ORDER BY Ora "
	
	objAsgRs.Open strAsgSQL, objAsgConn
	If objAsgRs.EOF Then
	'
	Else
	
		Do While NOT objAsgRs.EOF
		
			intAsgTotHits(objAsgRs("Ora")) = objAsgRs("Hits")
			intAsgTotVisits(objAsgRs("Ora")) = objAsgRs("Visits")
		
		objAsgRs.MoveNext
		Loop
	
	End If
	objAsgRs.Close
	
ElseIf elenca = "tutti" Then 
	
	strAsgSQL = "SELECT Sum(Hits) As SumHits, Sum(Visits) As SumVisits, Ora FROM "&strAsgTablePrefix&"Hourly GROUP BY Ora ORDER BY Ora "
	
	objAsgRs.Open strAsgSQL, objAsgConn
	If objAsgRs.EOF Then
	'
	Else
	
		Do While NOT objAsgRs.EOF
		
			intAsgTotHits(objAsgRs("Ora")) = objAsgRs("SumHits")
			intAsgTotVisits(objAsgRs("Ora")) = objAsgRs("SumVisits")
		
		objAsgRs.MoveNext
		Loop
	
	End If
	objAsgRs.Close
	
End If


'Ripassiamo tutto per filtrare i valori nulli o vuoti
'...contemporaneamente impostiamo i valori
For intAsgCiclo = 1 to (intAsgNumCol - 2) -1
	
	If "[]" & intAsgTotHits(intAsgCiclo) = "[]" Then intAsgTotHits(intAsgCiclo) = 0
	If "[]" & intAsgTotVisits(intAsgCiclo) = "[]" Then intAsgTotVisits(intAsgCiclo) = 0
	
	intAsgValHits(intAsgCiclo) = FormatNumber(intAsgTotHits(intAsgCiclo)*intAsgParte, 2)
	intAsgValVisits(intAsgCiclo) = FormatNumber(intAsgTotVisits(intAsgCiclo)*intAsgParte, 2)
	
Next

'Reset Server Objects
Set objAsgRs = Nothing
objAsgConn.Close
Set objAsgConn = Nothing


%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
<title><%= strAsgSiteName %> | powered by ASP Stats Generator <%= strAsgVersion %></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="copyright" content="Copyright (C) 2003-2008 Carletti Simone, All Rights Reserved" />
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
			<td align="center" background="<%= strAsgSknPathImage & strAsgSknTableBarBgImage %>" bgcolor="<%= strAsgSknTableBarBgColour %>" height="20" class="bartitle">ACCESSI per ORA</td>
		  </tr>
		  <tr bgcolor="<%= strAsgSknTableLayoutBorderColour %>">
			<td align="center" height="1"></td>
		  </tr>
		</table><br />
		<table width="90%" border="0" align="center" cellpadding="1" cellspacing="1">
		  <tr bgcolor="<%= strAsgSknTableTitleBgColour %>" align="center" class="normaltitle">
			<td width="100%" background="<%= strAsgSknPathImage & strAsgSknTableTitleBgImage %>" height="16"><%
				If elenca = "mese" Then
					Response.Write(UCase(strAsgTxtStatsOfTheMonth) & "&nbsp;" & mese)
				ElseIf elenca = "tutti" Then
					Response.Write(UCase(strAsgTxtStatsOfTheYear))
				End If
			%></td>
		  </tr>
		  <tr class="smalltext" bgcolor="<%= strAsgSknTableContBgColour %>">
			<td background="<%= strAsgSknPathImage & strAsgSknTableContBgImage %>" width="100%"><br />
			<!-- Grafico -->
			<table width="100%" border="0" cellspacing="1" cellpadding="0" align="center">
			  <tr valign="bottom" align="center">
				<td width="<%= intAsgLarCol * 2 %>" background="<%= strAsgSknPathImage %>layout/bg_graph_value.gif" colspan="2" nowrap>
					<table border="0" cellpadding="0" cellspacing="0" width="100%">
					<% For intAsgCiclo = 1 to 5 %>
					<tr height="<%= intAsgAltColMax/5 %>" align="right"><td width="100%" class="smalltext" valign="top"><%= CLng(intAsgMax / 5) * (6 - intAsgCiclo) %></td></tr>
					<% Next %>
					</table>
				</td>
				<% For intAsgCiclo = 0 to (intAsgNumCol - 2) -1 %>
				<td width="<%= intAsgLarCol %>" background="<%= strAsgSknPathImage %>layout/bg_graph_cell.gif">
					<img src="images/bar_graph_image_visits.gif" width="5" height="<%= intAsgValVisits(intAsgCiclo) %>" alt="<%= CalcolaPercentuale(intAsgTotMeseVisits, intAsgTotVisits(intAsgCiclo)) & " --> " & intAsgTotVisits(intAsgCiclo) & "&nbsp;" & strAsgTxtVisits %>" />
					<img src="images/bar_graph_image_hits.gif" width="5" height="<%= intAsgValHits(intAsgCiclo) %>" alt="<%= CalcolaPercentuale(intAsgTotMeseHits, intAsgTotHits(intAsgCiclo)) & " --> " & intAsgTotHits(intAsgCiclo) & "&nbsp;" & strAsgTxtHits %>" />
				</td>
			  	<% Next %>
			  </tr>
			  <tr class="smalltext" align="center">
				<td width="<%= intAsgLarCol * 2 %>" colspan="2" align="right">
					<a href="stats_daily.asp?mese=<%= mese %>" title="<%= strAsgTxtShow & "&nbsp;" & aryAsgMonth(Left(mese,2),2) %>" class="linksmalltext"><%= Left(aryAsgMonth(Left(mese,2),2),3) %></a>
					<a href="stats_monthly.asp?showperiod=<%= strAsgTxtShow %>&anno=<%= Right(mese , 4) %>" title="<%= strAsgTxtShow & "&nbsp;" & Right(mese,4) %>" class="linksmalltext"><%= Right(mese,4) %></a>
				</td>
				<% For intAsgCiclo = 0 to (intAsgNumCol - 2) -1 %>
				<td width="<%= intAsgLarCol %>" nowrap><%= intAsgCiclo %></td>
			  	<% Next %>
			  </tr>
<!--#include file="templates/_graph_legend.asp" -->
			</table>
			<!-- Fine Grafico -->
			<br />
			</td>
		  </tr>
		  <%
					
		'// Row - End table spacer			
		Call BuildTableContEndSpacer(intAsgNumCol)

		'// Row - Data output panels
		Response.Write(vbCrLf & "<tr class=""smalltext"" align=""center"" valign=""top""><td colspan=""" & intAsgNumCol & """ height=""25""><br />")
		Call GoToPeriod("stats_hourly.asp", "")
		Call GoToGrouping("stats_hourly.asp", "")
		Response.Write(vbCrLf & "</td></tr>")
		
		%>
		</table><br />
<%

' Footer
Response.Write(vbCrLf & "<table width=""100%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""1"">")
'// Row - Footer Border Line
Call BuildFooterBorderLine()

' ***** START WARNING - REMOVAL or MODIFICATION IN PART or ALL OF THIS CODE WILL VIOLATE THE LICENSE AGREEMENT	******
' ***** INIZIO AVVERTENZA - RIMOZIONE o MODIFICA PARZIALE/TOTALE DEL CODICE COMPORTA VIOLAZIONE DELLA LICENZA  	******
Response.Write("<tr align=""center"" valign=""middle"">")
Response.Write("<td align=""center"" background=""" & strAsgSknPathImage & strAsgSknTableBarBgImage & """ bgcolor=""" & strAsgSknTableBarBgColour & """ height=""20"" class=""footer""><a href=""http://www.asp-stats.com/"" class=""linkfooter"" title=""ASP Stats Generator Homepage"">ASP Stats Generator</a> [" & strAsgVersion & "] - &copy; 2003-2008 <a href=""http://www.weppos.com/"" class=""linkfooter"" title=""Weppos.com Homepage"">weppos</a>")
if blnAsgElabTime then Response.Write(" - " & asgElabtime())
Response.Write("</td>")
Response.Write("</tr>")
' ***** END WARNING - REMOVAL or MODIFICATION IN PART or ALL OF THIS CODE WILL VIOLATE THE LICENSE AGREEMENT	******
' ***** FINE AVVERTENZA - RIMOZIONE o MODIFICA PARZIALE/TOTALE DEL CODICE COMPORTA VIOLAZIONE DELLA LICENZA  ******

Response.Write("</table>")
Response.Write("</td></tr>")
Response.Write("</table>")
Response.Write("</td></tr>")
Response.Write("</table>")

%>
<!-- footer -->
<!--#include file="includes/footer.asp" -->
</body></html>
