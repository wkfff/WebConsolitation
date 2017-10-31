/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		MakeDatabase.sql - Основной скрипт создания пустой базы данных
	СУБД	SQL Server 2005
*/

:On Error exit
/*
:setvar ServerName MRRSERV\SQLSERVER2005
:setvar UserName DV
:setvar DatabaseName DV
*/
:!!echo ================================================================================
:!!echo  Создаем новую базу
:!!echo ================================================================================
:r X:\dotNET\Database\SqlServer\CreateDatabase.sql
:r X:\dotNET\Database\SqlServer\Base\ErrorMessages.sql

:!!echo Подсоединяемся под главным пользователем для создания основных объектов схемы
:connect $(ServerName) -U $(UserName) -P $(UserName)

use $(DatabaseName)

:!!echo ================================================================================
:!!echo Создаем основные объекты схемы
:!!echo ================================================================================
:r X:\dotNET\Database\SqlServer\Base\Audit.sql
:r X:\dotNET\Database\SqlServer\Base\SystemTables.sql
:r X:\dotNET\Database\SqlServer\Base\UsersAndGroups.sql
:r X:\dotNET\Database\SqlServer\Base\Tasks.sql
:r X:\dotNET\Database\SqlServer\Base\EventProtocol.sql
:r X:\dotNET\Database\SqlServer\Base\DataSources.sql
:r X:\dotNET\Database\SqlServer\Base\DataPump.sql
:r X:\dotNET\Database\SqlServer\Base\TablesCatalog.sql
:r X:\dotNET\Database\SqlServer\Base\OLAPProcessor.sql
:r X:\dotNET\Database\SqlServer\Base\fx_Date.sql
:r X:\dotNET\Database\SqlServer\Base\ObjectVersions.sql
:r X:\dotNET\Database\SqlServer\Base\Templates.sql
:r X:\dotNET\Database\SqlServer\Base\Message.sql

:out insert_data.log
:!!echo fx_date_year
:r X:\dotNET\Database\SqlServer\Generated\fx_date_year.sql
:!!echo fx_date_yearday
:r X:\dotNET\Database\SqlServer\Generated\fx_date_yearday.sql
:!!echo fx_date_yeardayunv
:r X:\dotNET\Database\SqlServer\Generated\fx_date_yeardayunv.sql
:!!echo fx_date_yearmonth
:r X:\dotNET\Database\SqlServer\Generated\fx_date_yearmonth.sql

:out STDOUT
