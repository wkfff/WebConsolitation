@echo off
chcp 1251
SET DBName=DV
SET USERNAME=DV
cd %1
sqlcmd -S PRSERV\MSSQL2005 -i MakeDatabase.sql   -v ServerName="PRSERV\MSSQL2005" UserName="%USERNAME%" DatabaseName="%DBName%"
pause

rem sqlcmd -S PRSERV\MSSQL2005 -i CreateDatabase.sql -v ServerName="PRSERV\MSSQL2005" UserName="%DBName%" DatabaseName="%DBName%"
