/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		MakeDatabase.sql - Основной скрипт создания пустой базы данных
	СУБД	Oracle 9.2
*/

set LineSize 10000;
set echo off;

rem Настраиваем глобальные параметры
@.\CommandFiles\AllParams.sql;

spool &LogFile;

pro ================================================================================
pro Создаем новую схему
pro ================================================================================
@.\CommandFiles\CreateNewBase.sql;

whenever SQLError exit rollback

rem Подсоединяемся под главным пользователем для создания основных объектов схемы
connect &UserName/&UserPassword@&DatabaseName;
pro ================================================================================
pro Создаем основные объекты схемы
pro ================================================================================

@.\Base\Audit.sql
@.\Base\SystemTables.sql;
@.\Base\UsersAndGroups.sql;
@.\Base\Tasks.sql;
@.\Base\EventProtocol.sql;
@.\Base\DataSources.sql;
@.\Base\DataPump.sql;
@.\Base\TablesCatalog.sql;
@.\Base\OLAPProcessor.sql;
@.\Base\Templates.sql;
@.\Base\ObjectVersions.sql;
@.\Base\Message.sql;

@.\CommandFiles\NewDataFill.sql;

disconnect;

pro ================================================================================
pro Генерация скриптов
pro ================================================================================
--host mkdir -p .\Generated
--host cscript //Nologo .\VBS_Utils_Generate\GenerateScriptsDVOracle.vbs

pro ================================================================================
pro Выполнение сгенерированных скриптов
pro ================================================================================
connect &UserName/&UserPassword@&DatabaseName;
@.\Generated\ForeignKeyIndices.sql
@.\Generated\MoveIndices.sql

pro ================================================================================
pro Применение текущих изменений
pro ================================================================================
--@.\Alters\Alter2_3_0.sql

disconnect;

spool off;

spool on;