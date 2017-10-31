<!--#include file="config_common.asp" -->
<!--#include file="wbstat/wbstat3_class.asp"-->
<!--#include file="includes/functions_count.asp" -->
<!--#include file="asg-lib/track.asp" -->
<%

'---------------------------------------------------
'	Chiudi connessione al Database
'---------------------------------------------------
If blnConnectionIsOpen = True Then
	'Se non si usavano variabili application chiudi 
	'connessione dato che è stata aperta per 
	'richiamare i dati.
	objAsgConn.Close
'---------------------------------------------------
End If
'---------------------------------------------------

Dim strAsgSQLtmp
	
	'-----------------------------------------------------------------------------------------
	' Get country information	
	'-----------------------------------------------------------------------------------------
	' Funzione:	Get country information using client IP
	' Data: 	07.04.2004
	' Commenti:	Thanks to http://www.ip-to-country.com/ for providing country/ip database
	'-----------------------------------------------------------------------------------------
	function GetCountry()
	
		'Convert IP
		strDottedIp = DottedIp(strAsgIP)
		
		Dim objIP2Conn
		Dim objIP2Rs
		Dim strIP2SQL
		
		'If there is a valid IP address
		If strDottedIp <> "noIp" Then
			
			strIP2SQL = "SELECT * FROM Country WHERE IP_From <= " & strDottedIp & " and IP_To >= " & strDottedIp
				
			Set objIP2Conn = Server.CreateObject("ADODB.Connection")
			Set objIP2Rs = Server.CreateObject("ADODB.Recordset")
				
			objIP2Conn.Open asgDatabaseAccessConnectionString(strAsgMapPathIP) 
				
			objIP2Rs.Open strIP2SQL, objIP2Conn
				If objIP2Rs.EOF Then
					strCountry = "(unknown)"
					strCountry2 = "xx"
				Else
					strCountry = objIP2Rs("Country")
					strCountry2 = objIP2Rs("Country2")
				End If
			objIP2Rs.Close
					
			Set objIP2Rs = Nothing
			objIP2Conn.Close
			Set objIP2Conn = Nothing
				
		Else 
					
			strCountry = "(unknown)"
			strCountry2 = "xx"
					
		End If
	
	end function 
	
	function GetCountry2()
		strDottedIp = DottedIp(strAsgIP)
		
		Dim objIP2Conn
		Dim objIP2Rs, objIP2Data
		Dim strIP2SQL, strIP2Data
		
		strCountry = "(unknown)" : strCountry2 = "xx" : strRegion = "" : strCity = "" : strLatitude = "" : strLongitude = ""
		
		If strDottedIp <> "noIp" Then			
			strIP2SQL = "SELECT * FROM GeoLiteCityBlocks WHERE startIpNum <= " & strDottedIp & " and endIpNum >= " & strDottedIp
					
			Set objIP2Conn = Server.CreateObject("ADODB.Connection")
			Set objIP2Rs = Server.CreateObject("ADODB.Recordset")
			Set objIP2Data = Server.CreateObject("ADODB.Recordset")			
				
			objIP2Conn.Open asgDatabaseAccessConnectionString(strAsgMapPathIP2) 
				
			objIP2Rs.Open strIP2SQL, objIP2Conn
			If not objIP2Rs.EOF Then
				strIP2DataSQL = "SELECT * FROM GeoLiteCityLocation WHERE LocId = " & objIP2Rs("LocId")			
				objIP2Data.Open strIP2DataSQL, objIP2Conn
				If not objIP2Rs.EOF Then
					strCountry = Replace(objIP2Data("Country"), """", "")
					strCountry2 = Replace(objIP2Data("Country"), """", "")
					strRegion = Replace(objIP2Data("Region"), """", "")
					strCity = Replace(objIP2Data("City"), """", "")
					strLatitude = objIP2Data("Latitude")
					strLongitude = objIP2Data("Longitude")
				End if
			End If
			
			objIP2Rs.Close
					
			Set objIP2Rs = Nothing
			Set objIP2Data = Nothing			
			objIP2Conn.Close
			Set objIP2Conn = Nothing					
		End If	
	end function 	
	
	
Sub Log()	
	'Procedi anche con errore
	'On Error Resume Next
	
	'-----------------------------------------------------------------------------------------
	' Richiama i dettagli del visitatore
	'-----------------------------------------------------------------------------------------
		
	'// Dal Js
	strAsgResolution = Request("w") & "x" & Request("h")	
	strAsgColor = Request("c")
	strAsgPage = Request("u")
	strAsgReferer = Request("r")	

	'// Da asp
	strAsgIP = Request.ServerVariables("REMOTE_ADDR")
	strAsgUA = Request.ServerVariables("HTTP_USER_AGENT")
	strHost = Request.ServerVariables("REMOTE_HOST")
	strLogin = Request("l")		

	'-----------------------------------------------------------------------------------------
	' Esclusione PC dalle statistiche
	'-----------------------------------------------------------------------------------------
	'
	' Controllo tramite Cookie
	Call ExitCountByCookie()
	'
		'Interrompi monitoraggio se variabile True (= IP Escluso)
		If blnExitCount Then Exit Sub
	'
	'
	' Controllo tramite IP
	If Trim("[]" & strAsgFilterIP) <> "[]" then 
	'		
		'Escludi By IP
		Call ExitCountByIP(strAsgIP)
		'Interrompi monitoraggio se variabile True (= IP Escluso)
		If blnExitCount Then Exit Sub
	'
	End If
	'
	' Controllo completo... prosegui con monitoraggio client!
	'-----------------------------------------------------------------------------------------

	'========================================
	'	Elabora i dettagli	
	'========================================
		
	'Risoluzione Video
	If strAsgResolution = "x" then 
		strAsgResolution = "(unknown)"
	End If
		
	'Referer
	If strAsgReferer = "" then strAsgReferer = Request.ServerVariables("HTTP_REFERER")
	If strAsgReferer = "" then strAsgReferer = "(unknown)"
		
	'Profondità Colore
	If strAsgColor = "" Then 
		strAsgColor = "(unknown)"
	End If
		

	'Calcola proprio server come referer
	If blnRefererServer = False then
		If InStr(asgUrlStripQuery(strAsgReferer), strAsgSiteURLremote) then
			strAsgReferer = "(ownserver)"
		End if
	End if
		
	
	'Taglia QueryString della Pagina
	If blnStripPathQS then
		strAsgPage = asgUrlStripQuery(strAsgPage)
	End If
		
	'Calcola pagina nuda e cruda
	strAsgPageStripped = asgUrlStripQuery(strAsgPage)

	'Imposta una pagina nulla se non esistente
	If strAsgPage = "" then strAsgPage = "(unknown)"
	strAsgPage = FilterSQLInput(strAsgPage)


	'========================================
	'	Monitoraggio Referer
	'========================================
			
		If blnMonitReferer OR blnMonitEngine Then
			
			If strAsgReferer = "(unknown)" Then
				strAsgRefererDom = "(unknown)"
			Else
				strAsgRefererDom = asgUrlGetDomain(strAsgReferer)
			End If
			
		End If


	'========================================
	'	Filtro Motori e Query
	'========================================
		
		If blnMonitEngine AND strAsgReferer <> "(unknown)" Then
			
			%><!--#include file="includes/inc_search_engines.asp" --><%			
			
			'Ciclo
			For intLoop = 1 to UBound(aryEngine)
				
				If ("http://"&strAsgRefererDom) = aryEngine(intLoop, 1) Then
					
					strAsgEngineName = aryEngine(intLoop, 2)

					strBuffer = Instr(strAsgReferer, aryEngine(intLoop, 3))
					If strBuffer Then
						strAsgEngineQS = Right(strAsgReferer, Len(strAsgReferer) - (Len(aryEngine(intLoop, 3)) + strBuffer - 1))
						strBuffer = Instr(strAsgEngineQS, "&")
						'17.11.03 Aggiunto controllo altrimenti se la query si trova al fondo
						'e non trova altri dati restituisce un errore!
						If strBuffer > 0 Then
							strAsgEngineQS = Left(strAsgEngineQS, strBuffer - 1)
						Else
						'
						End If
						strAsgEngineQS = DecodeURL(strAsgEngineQS, True)
						'Pulisci da caratteri conflittuali tipo '
						strAsgEngineQS = FilterSQLInput(strAsgEngineQS)
						
					'||----------------------------- Modifica ----------------------						
						strBuffer = Instr(strAsgReferer, aryEngine(intLoop, 4))
						If strBuffer Then
							strAsgEnginePG = Right(strAsgReferer, Len(strAsgReferer) - (Len(aryEngine(intLoop, 4)) + strBuffer - 1))
							strBuffer = Instr(strAsgEnginePG, "&")
							'17.11.03 Aggiunto controllo altrimenti se la query si trova al fondo
							'e non trova altri dati restituisce un errore!
							If strBuffer > 0 Then
								strAsgEnginePG = Left(strAsgEnginePG, strBuffer - 1)
							Else
							'
							End If
							strAsgEnginePG = DecodeURL(strAsgEnginePG, True)
							'Pulisci da caratteri conflittuali tipo '
							strAsgEnginePG = FilterSQLInput(strAsgEnginePG)
							blnExitEnginePage = True
						End If

						If blnExitEnginePage then
							strAsgEnginePG = GetSearchResultPage(aryEngine(intLoop, 5),strAsgEnginePG)
							If strAsgEnginePG = -1 Then blnExitEnginePage = False
						else
							strAsgEnginePG = 1
							blnExitEnginePage = True
						end if
					'----------------------------- Modifica ----------------------||						
					
						blnExitEngine = True
					End If
										
					'Esci dal ciclo
					If blnExitEngine = True Then Exit For
					
				End If
				
			Next
		
		End IF

		
	'-----------------------------------------------------------------------------------------
	' Get last client details from Imente class
	'-----------------------------------------------------------------------------------------
		
		' Class Object
		Set objClassI = CreateWBstat("wbstat/wbstat3_spec/", false, "(unknown)", 1, 0, True, False, False, True, True, True, True, True, False,   True   , True, True, True, False, False)
		
		strAsgBrowser = objClassI("Browser")
		strAsgBrowserLang = objClassI("Browser.Language")
		strAsgOS = objClassI("OS")
		strAsgBrowserActCookie = objClassI("Browser.Act.Cookie")

		' Release Object
		Set objClassI = Nothing
		
	'-----------------------------------------------------------------------------------------
	' Filter input from malicious SQL code
	'-----------------------------------------------------------------------------------------
	
		strAsgReferer = FilterSQLInput(strAsgReferer)
		strAsgOS = FilterSQLInput(strAsgOS)
		strAsgBrowser = FilterSQLInput(strAsgBrowser)
		strAsgResolution = FilterSQLInput(strAsgResolution)
		strAsgColor = FilterSQLInput(strAsgColor)
		strAsgBrowserLang = FilterSQLInput(strAsgBrowserLang)
	
	'========================================
	'	Apri connessione e procedi salvataggi	
	'========================================
	
		' Apri connessione
		objAsgConn.Open strAsgConn


	'-----------------------------------------------------------------------------------------
	' IMPORTANTE: Visita o No?
	'-----------------------------------------------------------------------------------------
		
		'Verifica se è una visita

		'-----------------------------------------------------------------------------------------
		' VARIANTE 1.
		'-----------------------------------------------------------------------------------------
		' Analizza l'ID attirbuita dal Server. Il problema è che se qualcuno
		' accede aprendo/chiudendo più finestre lo stesso utente viene riconosciuto
		' come diverse visite!
		'-----------------------------------------------------------------------------------------
		' strAsgSQL = "SELECT Details_ID FROM "&strAsgTablePrefix&"Detail WHERE Visitor_ID = '" & Session.SessionID & "' AND IP = '" & strAsgIP & "'"
		'-----------------------------------------------------------------------------------------

		'-----------------------------------------------------------------------------------------
		' VARIANTE 2.
		'-----------------------------------------------------------------------------------------
		' Verifica in un determinato lasso di tempo se c'è qualcuno con dati uguali
		' ed in base a quello procede.
		'
		' Comincia con il verificare qualcuno con quell'IP e stesso User Agent 
		'-----------------------------------------------------------------------------------------
		strAsgSQL = "SELECT TOP 1 Data, Visitor_ID, User_Agent, Country, Country2 FROM "&strAsgTablePrefix&"Detail WHERE IP = '" & strAsgIP & "' AND User_Agent = '" & strAsgUA & "' ORDER BY Details_ID DESC"
		'-----------------------------------------------------------------------------------------
		' VERSIONE ERRATA: 
		' Fino al 13.02.04 ordinava invece che per il campo ID chiave primaria
		' per il campo Visitor_ID, ovvero la Session.ID attibuita dal server!
		' Risultato? Venivano calcolati come unici alcuni accessi che non lo erano per niente!
		'-----------------------------------------------------------------------------------------
		' strAsgSQL = "SELECT TOP 1 Data, Visitor_ID, User_Agent FROM "&strAsgTablePrefix&"Detail WHERE IP = '" & strAsgIP & "' AND User_Agent = '" & strAsgUA & "' ORDER BY Visitor_ID DESC"
		'-----------------------------------------------------------------------------------------

		objAsgRs.Open strAsgSQL, objAsgConn
		
		If NOT objAsgRs.EOF Then
			'23.11.2003 Se trova un risultato
			'23.11.2003 Se la differenza di ultima visita è minore di 2 ore
			'23.11.2003 Se l'User Agent è lo stesso | Per ora sicuramente lo stesso data la query! -> In testing
			Dim dtmDiffVisitTime
			dtmDiffVisitTime = DateDiff("h", CDate(objAsgRs("Data")), dtmAsgNow) 
			If IsNumeric(dtmDiffVisitTime) Then dtmDiffVisitTime = Clng(dtmDiffVisitTime)
			If dtmDiffVisitTime < 6 OR objAsgRs("Visitor_ID") = strAsgSessionID Then 'AND objAsgRs("User_Agent") = strAsgUA Then 
				' Check that the day is the same also if the difference is smaller than 6 hours
				if not Day(objAsgRs("Data")) <> Day(dtmAsgNow) then
					blnAsgIsVisit = False
					intAsgVisitValue = 0
					'Per i dettagli visitatori mostra id iniziale
					strAsgSessionID = objAsgRs("Visitor_ID")
					'Per evitare elaborazioni inutili salva i dettagli della nazione
					strCountry = objAsgRs("Country")
					strCountry2 = objAsgRs("Country2")
				end if
			End If
		End If
		objAsgRs.Close

	'========================================
	'	Inserisci nel database	
	'========================================
		
		'========================================
		'	Counters	
		'========================================
		
		strAsgSQL = "SELECT Counters_ID FROM "&strAsgTablePrefix&"Counter WHERE Anno = " & dtmAsgYear & " "
			
		objAsgRs.Open strAsgSQL, objAsgConn
			
		If objAsgRs.EOF Then 
			strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Counter (Anno, Hits, Visits) "
			strAsgSQL = strAsgSQL & " VALUES (" & dtmAsgYear & ", 1, " & intAsgVisitValue & ")"
		Else
			strAsgSQL = "UPDATE "&strAsgTablePrefix&"Counter SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Anno = " & dtmAsgYear & " "
		End IF
			
		objAsgRs.Close
			
		objAsgConn.Execute(strAsgSQL)
		
		
		'========================================
		'	Monitorizza Referers	
		'========================================
		
		If blnMonitReferer Then
		
			strAsgSQL = "SELECT Referer_ID FROM "&strAsgTablePrefix&"Referer WHERE Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' AND Referer = '" & strAsgReferer & "' "
			
			objAsgRs.Open strAsgSQL, objAsgConn
			
			If objAsgRs.EOF Then 
				strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Referer (Referer, Dominio, Visits, Last_Access, Mese) "
				strAsgSQL = strAsgSQL & " VALUES ('" & strAsgReferer & "', '" & strAsgRefererDom & "', 1, #" & dtmAsgNow & "#, '" & dtmAsgMonth & "-" & dtmAsgYear & "')"
			Else
				strAsgSQL = "UPDATE "&strAsgTablePrefix&"Referer SET Visits = Visits + 1, Last_Access = #" & dtmAsgNow & "# WHERE Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' AND Referer = '" & strAsgReferer & "' "
			End If
			
			objAsgRs.Close
			
			objAsgConn.Execute(strAsgSQL)			
		End If
		
		'-----------------------------------------------------------------------------------------
		' Monitoraggio	->	Country
		'-----------------------------------------------------------------------------------------
		If blnMonitCountry Then
			If blnAsgIsVisit Then Call GetCountry()
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"Country SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Country = '" & strCountry & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			If blnAsgIsVisit Then
				strAsgSQL = "SELECT Country_ID FROM "&strAsgTablePrefix&"Country WHERE Country = '" & strCountry & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "'"
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Country (Country, Country2, Hits, Visits, Mese) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strCountry & "', '" & strCountry2 & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
				Else
					strAsgSQL = strAsgSQLtmp
				End IF
				objAsgRs.Close
			Else	
				strAsgSQL = strAsgSQLtmp
			End If
			objAsgConn.Execute(strAsgSQL)			
		End If

		'-----------------------------------------------------------------------------------------
		' Monitoring	->	City
		'-----------------------------------------------------------------------------------------
		If blnMonitCountry Then					'
			If blnAsgIsVisit Then Call GetCountry()			'
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"City SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE City = '" & strCountry & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			If blnAsgIsVisit Then
				strAsgSQL = "SELECT City_ID FROM " & strAsgTablePrefix&"City WHERE City = '" & strCity & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "'"
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"City (City, Hits, Visits, Mese) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strCity & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
				Else
					strAsgSQL = strAsgSQLtmp
				End IF
				objAsgRs.Close
			Else	
					strAsgSQL = strAsgSQLtmp
			End If
			objAsgConn.Execute(strAsgSQL)			
		End If		
		
		'-----------------------------------------------------------------------------------------
		' Monitoring	->	Region
		'-----------------------------------------------------------------------------------------
		If blnMonitCountry Then					'
			If blnAsgIsVisit Then Call GetCountry()			'
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"Region SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Region = '" & strCountry & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			If blnAsgIsVisit Then
				strAsgSQL = "SELECT Region_ID FROM " & strAsgTablePrefix&"Region WHERE Region = '" & strCity & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "'"
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Region (Region, Hits, Visits, Mese) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strRegion & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
				Else
					strAsgSQL = strAsgSQLtmp
				End IF
				objAsgRs.Close
			Else	
				strAsgSQL = strAsgSQLtmp
			End If
			objAsgConn.Execute(strAsgSQL)			
		End If		
		
		'========================================
		'	Daily monitoring	
		'========================================
		
		If blnMonitDaily Then		
			strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Daily WHERE Data = #" & dtmAsgDate & "# "			
			objAsgRs.Open strAsgSQL, objAsgConn			
			If objAsgRs.EOF Then 
				strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Daily (Data, Mese, Hits, Visits) "
				strAsgSQL = strAsgSQL & " VALUES (#" & dtmAsgDate & "#, '" & dtmAsgMonth & "-" & dtmAsgYear & "', 1, " & intAsgVisitValue & ")"
			Else
				strAsgSQL = "UPDATE "&strAsgTablePrefix&"Daily SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Data = #" & dtmAsgDate & "# "
			End If			
			objAsgRs.Close			
			objAsgConn.Execute(strAsgSQL)			
		End If

		
		'-----------------------------------------------------------------------------------------
		' Monitoraggio	->	IP Address
		'-----------------------------------------------------------------------------------------
		If blnMonitIP Then
		
			'-----------------------------------------------------------------------------------------
			' Controllo hits o visits
			'-----------------------------------------------------------------------------------------
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"IP SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Last_Access = #" & dtmAsgDate & "# AND IP = '" & strAsgIP & "' "
			'
			' E' la prima visita e per sicurezza procedi al controllo completo
			If blnAsgIsVisit Then
			'
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"IP WHERE Last_Access = #" & dtmAsgDate & "# AND IP = '" & strAsgIP & "' "
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"IP (IP, Last_Access, Hits, Visits) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strAsgIP & "', #" & dtmAsgDate & "#, 1, " & intAsgVisitValue & ")"
				Else
					strAsgSQL = strAsgSQLtmp
				End If
				objAsgRs.Close
			'
			' E' già stato catalogato quindi procedi abbreviatamente
			Else	
					strAsgSQL = strAsgSQLtmp
			End If
			'
			' Esegui l'effetivo monitoraggio!
			objAsgConn.Execute(strAsgSQL)
			
		End If
		
		'-----------------------------------------------------------------------------------------
		' Login monitoring
		'-----------------------------------------------------------------------------------------
		If blnMonitIP Then		
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"Login SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Last_Access = #" & dtmAsgDate & "# AND Login = '" & strLogin & "' "
			If blnAsgIsVisit Then
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Login WHERE Last_Access = #" & dtmAsgDate & "# AND Login = '" & strLogin & "' "
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Login (Login, Last_Access, Hits, Visits) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strLogin & "', #" & dtmAsgDate & "#, 1, " & intAsgVisitValue & ")"
				Else
					strAsgSQL = strAsgSQLtmp
				End If
				objAsgRs.Close
			Else	
					strAsgSQL = strAsgSQLtmp
			End If
			objAsgConn.Execute(strAsgSQL)			
		End If

		'========================================
		'	Hourly monitoring	
		'========================================
		
		If blnMonitHourly Then		
			strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Hourly WHERE Ora = " & Hour(dtmAsgNow) & " AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "			
			objAsgRs.Open strAsgSQL, objAsgConn			
			If objAsgRs.EOF Then 
				strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Hourly (Ora, Hits, Visits, Mese) "
				strAsgSQL = strAsgSQL & " VALUES (" & Hour(dtmAsgNow) & ", 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
			Else
				strAsgSQL = "UPDATE "&strAsgTablePrefix&"Hourly SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Ora = " & Hour(dtmAsgNow) & " AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			End If
			
			objAsgRs.Close			
			objAsgConn.Execute(strAsgSQL)			
		End If

		
		'-----------------------------------------------------------------------------------------
		' Monitoraggio	->	Browsing System
		'-----------------------------------------------------------------------------------------
		If blnMonitSystem Then
		
			'-----------------------------------------------------------------------------------------
			' Controllo hits o visits
			'-----------------------------------------------------------------------------------------
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"System SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' AND OS = '" & strAsgOS & "' AND Browser = '" & strAsgBrowser & "' AND Reso = '" & strAsgResolution & "' AND Color = '" & strAsgColor & "' "
			'
			' E' la prima visita e per sicurezza procedi al controllo completo
			If blnAsgIsVisit Then
			'
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"System WHERE Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' AND OS = '" & strAsgOS & "' AND Browser = '" & strAsgBrowser & "' AND Reso = '" & strAsgResolution & "' AND Color = '" & strAsgColor & "' "
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"System (OS, Browser, Reso, Color, Hits, Visits, Mese) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strAsgOS & "', '" & strAsgBrowser & "', '" & strAsgResolution & "', '" & strAsgColor & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
				Else
					strAsgSQL = strAsgSQLtmp
				End IF
				objAsgRs.Close
			'
			' E' già stato catalogato quindi procedi abbreviatamente
			Else	
					strAsgSQL = strAsgSQLtmp
			End If
			'
			' Esegui l'effetivo monitoraggio!
			objAsgConn.Execute(strAsgSQL)
			
		End If
		


		'-----------------------------------------------------------------------------------------
		' Monitoraggio	->	Browser Language
		'-----------------------------------------------------------------------------------------
		If blnMonitLanguages Then
		
			'-----------------------------------------------------------------------------------------
			' Controllo hits o visits
			'-----------------------------------------------------------------------------------------
			strAsgSQLtmp = "UPDATE "&strAsgTablePrefix&"Language SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Languages = '" & strAsgBrowserLang & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			'
			' E' la prima visita e per sicurezza procedi al controllo completo
			If blnAsgIsVisit Then
			'
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Language WHERE Languages = '" & strAsgBrowserLang & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
				objAsgRs.Open strAsgSQL, objAsgConn
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Language (Languages, Hits, Visits, Mese) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strAsgBrowserLang & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
				Else
					strAsgSQL = strAsgSQLtmp
				End IF
				objAsgRs.Close
			'
			' E' già stato catalogato quindi procedi abbreviatamente
			Else	
					strAsgSQL = strAsgSQLtmp
			End If
			'
			' Esegui l'effetivo monitoraggio!
			objAsgConn.Execute(strAsgSQL)
			
		End If
		
		'========================================
		'	Monitorizza pagine	
		'========================================
		
		If blnMonitPages Then
		
			strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Page WHERE Page = '" & strAsgPage & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			
			objAsgRs.Open strAsgSQL, objAsgConn
			
			If objAsgRs.EOF Then 
				strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Page (Page, Page_Stripped, Hits, Visits, Mese) "
				strAsgSQL = strAsgSQL & " VALUES ('" & strAsgPage & "', '" & strAsgPageStripped & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "' )"
			Else
				strAsgSQL = "UPDATE "&strAsgTablePrefix&"Page SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & " WHERE Page = '" & strAsgPage & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			End IF
			
			objAsgRs.Close
			
			objAsgConn.Execute(strAsgSQL)
			
		End If
		
		'========================================
		'	Monitorizza Motori di Ricerca	
		'========================================
		
		If blnMonitEngine AND strAsgReferer <> "(unknown)" Then
			
			'Se ha trovato il motore di ricerca ed è presente una QS procedi ad inserire
			If "[]" & strAsgEngineName <> "[]" AND "[]" & strAsgEngineQS <> "[]" Then
			
				strAsgSQL = "SELECT * FROM "&strAsgTablePrefix&"Query WHERE Query = '" & strAsgEngineQS & "' AND Engine = '" & strAsgEngineName & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
			
				objAsgRs.Open strAsgSQL, objAsgConn
				
				If objAsgRs.EOF Then 
					strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Query (Query, Engine, Hits, Visits, Mese, SERP_Page) "
					strAsgSQL = strAsgSQL & " VALUES ('" & strAsgEngineQS & "', '" & strAsgEngineName & "', 1, " & intAsgVisitValue & ", '" & dtmAsgMonth & "-" & dtmAsgYear & "', " & strAsgEnginePG & " )"
				Else
					strAsgSQL = "UPDATE "&strAsgTablePrefix&"Query SET Hits = Hits + 1, Visits = Visits + " & intAsgVisitValue & ", SERP_Page = " & strAsgEnginePG & " WHERE Query = '" & strAsgEngineQS & "' AND Engine = '" & strAsgEngineName & "' AND Mese = '" & dtmAsgMonth & "-" & dtmAsgYear & "' "
				End IF
				
				objAsgRs.Close
			
				objAsgConn.Execute(strAsgSQL)
			
			End If
			
		End If
		

		'========================================
		'	Details	
		'========================================
		
		strAsgSQL = "INSERT INTO "&strAsgTablePrefix&"Detail "
		strAsgSQL = strAsgSQL & "(Visitor_ID, Data, IP, Country, Country2, User_Agent, OS, Browser, Browser_Lang, Reso, Color, Referer, Page, Query, Engine, Login, City, Region) "
		strAsgSQL = strAsgSQL & " VALUES ('" & strAsgSessionID & "', #" & dtmAsgNow & "# , '" & strAsgIP & "', '" & strCountry & "', '" & strCountry2 & "', '" & strAsgUA & "', '" & strAsgOS & "', '" & strAsgBrowser & "', '" & strAsgBrowserLang & "', '" & strAsgResolution & "', '" & strAsgColor & "', '" & strAsgReferer & "', '" & strAsgPage & "', '" & strAsgEngineQS & "', '" & strAsgEngineName & "', '" & strLogin & "', '" & strCity & "', '" & strRegion & "') "

		objAsgConn.Execute(strAsgSQL)
		
		
		'Chiudi e distruggi
		objAsgConn.Close
		Set objAsgConn = Nothing	

End Sub

'Conteggia
Log()

'Mostra l'immagine
Response.Redirect strAsgImage
	
'Response.Write(FormatNumber(Timer() - startAsgElab, 4))
	
%>

