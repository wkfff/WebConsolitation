@echo off
chcp 1251

@rem === ��������� ����������� ===============================================
@rem ����� ���� ������. ������ �������� "DB_Alias" ���������� ������� ����� ������� ���� ������.
@set DATABASE_ALIAS=demo
@set SERVER_NAME=SHELPANOV\SQLEXPRESS01
@set USER_Name=MikhailMamonov
@set USER_PASSWORD=mmm201093
cd %1

@rem ��������� ������� ��������� �����-������ DatabaseScript.sql �� ����������
sqlcmd -S %SERVER_NAME% -i DatabaseScript.sql -v ServerName="%SERVER_NAME%" UserName="%USER_Name%" Password="%USER_PASSWORD%" DatabaseName="%DATABASE_ALIAS%" -o DatabaseScript.log

@rem ��� ���������� ���������� ���������� ������ ����� �������.
pause
