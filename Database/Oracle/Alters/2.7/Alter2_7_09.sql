/********************************************************************
	Переводит базу Oracle из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
whenever SQLError exit rollback; 
/* End   - Стандартная часть */ 


/* Start - <Номер задачи в ClearQuest> - Добавление № ревизии куба в SVN - tsvetkov - 04.10.2010 */

alter table OlapObjects add Revision VARCHAR2(10) default null; 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (88, '2.7.0.9', To_Date('04.10.2010', 'dd.mm.yyyy'), SYSDATE, 'Добавление № ревизии куба в SVN', 0);

commit;

/* End - <Номер задачи в ClearQuest> - Добавление № ревизии куба в SVN - tsvetkov - 04.10.2010 */
