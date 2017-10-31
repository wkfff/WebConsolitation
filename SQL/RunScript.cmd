@echo off
chcp 1251

@rem === ПАРАМЕТРЫ ПОДКЛЮЧЕНИЯ ===============================================
@rem Алиас базы данных. Вместо значения "DB_Alias" необходимо указать алиас целевой базы данных.
@set DATABASE_ALIAS=demo
@set SERVER_NAME=SHELPANOV\SQLEXPRESS01
@set USER_Name=MikhailMamonov
@set USER_PASSWORD=mmm201093
cd %1

@rem Следующая строчка запускает макро-скрипт DatabaseScript.sql на выполнение
sqlcmd -S %SERVER_NAME% -i DatabaseScript.sql -v ServerName="%SERVER_NAME%" UserName="%USER_Name%" Password="%USER_PASSWORD%" DatabaseName="%DATABASE_ALIAS%" -o DatabaseScript.log

@rem Для завершения выполнения необходимо нажать любую клавишу.
pause
