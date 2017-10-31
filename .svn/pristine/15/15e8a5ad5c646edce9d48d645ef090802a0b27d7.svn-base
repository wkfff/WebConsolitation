/* Test
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		ForImport.sql - Скрипт для подготовки базы данных перед импортом экспортного dmp-файла
	СУБД	Oracle 9.2
*/

set linesize 4000

define DatabaseName = &DatabaseName
define UserName=DV						/* Имя главного пользователя - создателя, владельца и администратора */
define UserPassword=&UserName			/* Пароль главного пользователя */
define SystemPassword=system			/* Пароль главного администратора SYSTEM базы данных. Он пораждает главного пользователя */
define DataFilePath=D:\oracle\oradata\&DatabaseName	/* Путь к файлам базы */
rem Место хранения базы данных "&DatabaseName"
define TableSpace=&UserName										/* Табличная область для хранения данных */
define IndexTableSpace=&UserName.Indx							/* Табличная область для индексов */
define TableSpaceFile=&DataFilePath\&TableSpace..dbf			/* Файл для хранения данных */
define IndexTableSpaceFile=&DataFilePath\&IndexTableSpace..dbf	/* Файл для хранения индексов */
define SchemaName=&UserName

connect system/system@&DatabaseName;

whenever SQLError continue commit

drop tablespace &TableSpace including contents and datafiles cascade constraints;
drop tablespace &IndexTableSpace including contents and datafiles cascade constraints;

/* Теперь будем создавать новые табличные области */
create tablespace &TableSpace datafile '&TableSpaceFile' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;
create tablespace &IndexTableSpace datafile '&IndexTableSpaceFile' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;

drop user &UserName cascade;

whenever SQLError exit rollback;

/* Создадим нового пользователя - владельца всего */
create user &UserName identified by &UserPassword default tablespace &TableSpace;

/* Выделим пользователю администраторские права, чтобы он мог создавать структуру базы */
grant DBA to &UserName;

commit work;

disconnect;

exit;