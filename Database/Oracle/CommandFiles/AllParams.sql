/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		AllParams.sql - Скрипт определения глобальных переменных
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Скрипт определения глобальных переменных
pro ================================================================================

define DatabaseName=&Имя_Базы_Данных	/* База данных, к которой производится подключение */
define UserName=DV						/* Имя главного пользователя - создателя, владельца и администратора */
										/* всей структуры базы и всех содержащихся в ней данных. Указывается в верхнем регистре для */
										/* использования в качестве имени схемы */
define UserPassword=&UserName			/* Пароль главного пользователя */
define SystemPassword=system			/* Пароль главного администратора SYSTEM базы данных. Он пораждает главного пользователя */

rem Основные пути
define DataFilePath=D:\oracle\oradata\&DatabaseName	/* Путь к файлам базы */
define ScriptPath=.									/* Путь к файлам-скриптам */

rem Место хранения базы данных "&DatabaseName"
define TableSpace=&UserName										/* Табличная область для хранения данных */
define IndexTableSpace=&UserName.Indx							/* Табличная область для индексов */
define AuditTableSpace=&UserName.Audit							/* Табличная область для хранения данных аудита */
define TableSpaceFile=&DataFilePath\&TableSpace..dbf			/* Файл для хранения данных */
define IndexTableSpaceFile=&DataFilePath\&IndexTableSpace..dbf	/* Файл для хранения индексов */
define AuditTableSpaceFile=&DataFilePath\&AuditTableSpace..dbf	/* Файл для хранения данных аудита */

rem Протокол
define LogFile=&ScriptPath\CreateDatabase_&DatabaseName..log

rem  А вот эти параметры настраивать скорее всего не придётся. Они получаются из вышенаписанных.
define SchemaName=&UserName
