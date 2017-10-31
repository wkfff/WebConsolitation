/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		ChangeTemp.sql - Скрипт очистки табличного пространства TEMP
	СУБД	Oracle 9.2
*/

host chcp 1251
define DataBaseName = &Имя_Базы_Данных
define DiskLetter = &Укажите_диск
define OraDataPath = ':\ORACLE\ORADATA\'
define TempTBS = 'TEMP'
define TempTBSTemp = 'TEMPTEMP'
define TempFileName = &DiskLetter.&OraDataPath.&DataBaseName.\TEMP01.DBF
define TempTempFileName = &DiskLetter.&OraDataPath.&DataBaseName.\TEMPTEMP.ora

conn sys/sys@&DataBaseName as sysdba;

-- Создаем временный TEMP и переключаемся на него

CREATE TEMPORARY TABLESPACE "&TempTBSTemp" TEMPFILE '&TempTempFileName' SIZE 5M AUTOEXTEND ON NEXT 50M MAXSIZE UNLIMITED EXTENT MANAGEMENT LOCAL UNIFORM SIZE 1M;
ALTER DATABASE DEFAULT TEMPORARY TABLESPACE "&TempTBSTemp";

DROP TABLESPACE "&TempTBS" INCLUDING CONTENTS AND DATAFILES;

host pause

CREATE TEMPORARY TABLESPACE "&TempTBS" TEMPFILE '&TempFileName' SIZE 5M AUTOEXTEND ON NEXT 50M MAXSIZE UNLIMITED EXTENT MANAGEMENT LOCAL UNIFORM SIZE 1M;
ALTER DATABASE DEFAULT TEMPORARY TABLESPACE "&TempTBS";

DROP TABLESPACE "&TempTBSTemp" INCLUDING CONTENTS AND DATAFILES;
