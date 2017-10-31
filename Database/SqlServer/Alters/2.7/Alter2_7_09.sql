/********************************************************************
	Переводит базу Oracle из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 


/* Start - <Номер задачи в ClearQuest> - Добавление № ревизии куба в SVN - tsvetkov - 04.10.2010 */

alter table OlapObjects add Revision VARCHAR(10) default null

go 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (88, '2.7.0.9', CONVERT(datetime, '2010.10.04', 102), GETDATE(), 'Добавление № ревизии куба в SVN', 0);

go

/* End - <Номер задачи в ClearQuest> - Добавление № ревизии куба в SVN - tsvetkov - 04.10.2010 */
