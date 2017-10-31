<%

' Server MapPath
Dim strAsgMapPath
Dim strAsgMapPathTo
Dim strAsgMapPathIP, strAsgMapPathIP2 



' ----- BEGINNING OF CONFIGURATION SETTINGS -----


' ***** PUBLIC FOLDER *****
' This is the path to database folder.
' Each web host usually provides a special folder for Access databases
' with write permission enabled and browse permission denied.
' The folder is writeable by the web user but the public access is denied
' in order to prevent anyone from downloading your database.
'
' Be sure to move your database into your database folder
' and change the following path.
' You can use both relative and absolute path.
'
' Each path starts from /myasg folder and follows the same rules ad links in web pages.
' To specify a path to a child folder (for example 'myasg/mdb/') simply enter 'mdb/'.
' If your folder is a parent directory then use '../' to go back to parent folder
' or use an absolute path from your web root, for example '/absolute/path/to/folder/'
' means the folder available at http://example.com/absolute/path/to/folder/.
Const strAsgPathFolderDb = "mdb/" 

' ***** WRITE PERMISSION FOLDER *****
' This is the path to a folder with write permission enabled
' Write permission are required by inc_skin_file.asp skin file
' It could be the same as database folder
Const strAsgPathFolderWr = "mdb/"


' ***** DATABASE NAME *****
' Main application database name
Const strAsgDatabaseSt = "dbstats" 

' ***** IP2COUNTRY DATABASE NAME *****
' This is the name of the ip-to-country database
' used to get country information from IP address
Const strAsgDatabaseIp = "ip-to-country" 
Const strAsgDatabaseIp2 = "geocitylite" 

' ***** TABLE PREFIX *****
' Prefix that all ASG database tables will have.
' This is useful if you want to run multiple versions or copies on the same database 
' or if you are sharing the database with other applications.
Const strAsgTablePrefix = "tblst_"


' ***** COOKIE PREFIX *****
' Prefix that all ASG cookies will have.
' This is useful if you run multiple copies of the program on the same site so that 
' cookies don't interfer with each other.
Const strAsgCookiePrefix = "asg_"


' ----- END OF CONFIGURATION SETTINGS -----



strAsgMapPath = Server.MapPath(strAsgPathFolderDb & strAsgDatabaseSt & ".mdb")
strAsgMapPathTo = Server.MapPath(strAsgPathFolderDb & strAsgDatabaseSt & ".bak")
strAsgMapPathIP = Server.MapPath(strAsgPathFolderDb & strAsgDatabaseIp & ".mdb")
strAsgMapPathIP2 = Server.MapPath(strAsgPathFolderDb & strAsgDatabaseIp2 & ".mdb")

strAsgConn = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & strAsgMapPath

%>
