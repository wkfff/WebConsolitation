<%@ LANGUAGE="VBSCRIPT" %>
<% Option Explicit %>
<!--#include file="asg-lib/track.asp" -->
<%
' Tracking file
Response.Write "var file = '" & asgTrackFilePath & "';"

' Referer
Response.Write "var f = escape(document.referrer);"

' Current page
Response.Write "var u = escape(document.URL); "

' Video resolution
Response.Write "var w = screen.width; "
Response.Write "var h = screen.height; "

' Login info
Response.Write "var l = escape('" & Request.QueryString("login") & "'); "

' Color depth according to browser type
Response.Write "var v = navigator.appName; "
Response.Write "var c = ''; "
Response.Write "if (v != 'Netscape') { c = screen.colorDepth; }"
Response.Write "else { c = screen.pixelDepth; }"

' Tracking string
Response.Write "info='l=' + l + '&h=' + h + '&c=' + c + '&r=' + f + '&u=' + u + '&w=' + w; "
' Write image
Response.Write "document.open();"

'Response.Write "document.write('<div id=""StatisticPanel"" style=""display:none;"">');"
Response.Write "document.write('<img src=""' + file + '?' + info + '"" border=""0"">');"
'Response.Write "document.write('</div>);"
Response.Write "document.close();"

%>