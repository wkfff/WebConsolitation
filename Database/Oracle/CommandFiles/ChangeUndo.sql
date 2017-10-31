/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		ChangeUndo.sql - Скрипт очистки табличного пространства UNDO
	СУБД	Oracle 9.2
*/

/*
==============================================================================
  Перед выполнением скрипта необходимо завершить все активные транзакции
  и отключить всех пользователей от базы данных.
  Обязательно сделать резервную копию базы данных.
  Перед выполнением скрипта желательно перезапустить экземпляр базы данных,
  для гарантированного завершения всех транзакций и отключения всех
  пользователей.

  В качестве значения для параметра "Алиас_базы_данных" необходимо указать
  имя алиаса базы данных, в которой необходимо очистить табличное
  пространство UNDO. Например: Region.

  В качестве значения для параметра "имя_базы_данных" необходимо указать
  имя экземпляра базы данных. Например: Region.

  В качестве значения параметра "диск" нужно задать имя (букву) логического
  диска, на котором размещены файлы табличного пространства UNDO.
  Если фалы, к примеру, расположены по следующему пути:
  D:\ORACLE\ORADATA\Region\UNDOTBS1.DBF, то необходимо указать букву D.

  После задания всех необходимых параметров скрипт создаст временное табличное
  пространство отмены UNDOTBST и переключит на него экземпляр базы данных,
  после чего будет удалено старое пространство отмены.

  Далее будет выполнена команда приостанова выполнения скрипта, для того, чтоб
  визуально убедиться что файл <ДИСК>:\ORACLE\ORADATA\<ИМЯ БАЗЫ>\UNDOTBS1.DBF
  удачно удален, если этот файл присутствует, то его необходимо удалить
  вручную и продолжить выполнение скрипта.
============================================================================*/

host chcp 1251
define DataBaseAlias = &Алиас_Базы_Данных
define DataBaseName = &Имя_Базы_Данных
define DiskLetter = &диск
define OraDataPath = ':\ORACLE\ORADATA\'
define UndoTBS = 'UNDOTBS1'
define UndoTBSTemp = 'UNDOTBST'
define UndoFileName = &DiskLetter.&OraDataPath.&DataBaseName.\UNDOTBS01.DBF
define UndoTempFileName = &DiskLetter.&OraDataPath.&DataBaseName.\UNDOTBST.ora

conn sys/sys@&DataBaseAlias as sysdba;

-- Создаем временное UNDO и переключаемся на него

CREATE UNDO TABLESPACE "&UndoTBSTemp" DATAFILE '&UndoTempFileName' SIZE 5M AUTOEXTEND ON NEXT 5M MAXSIZE UNLIMITED;
alter system set UNDO_TABLESPACE = &UndoTBSTemp;

-- Удаляем рабочее UNDO

ALTER TABLESPACE "&UndoTBS" OFFLINE NORMAL;
DROP TABLESPACE "&UndoTBS" INCLUDING CONTENTS AND DATAFILES;

host pause

-- Создаем рабочее UNDO и переключаемся на него

CREATE UNDO TABLESPACE "&UndoTBS" DATAFILE '&UndoFileName' SIZE 5M AUTOEXTEND ON NEXT 5M MAXSIZE UNLIMITED;
alter system set UNDO_TABLESPACE = &UndoTBS;

-- Удаляем временное UNDO

ALTER TABLESPACE "&UndoTBSTemp" OFFLINE NORMAL;
DROP TABLESPACE "&UndoTBSTemp" INCLUDING CONTENTS AND DATAFILES;
