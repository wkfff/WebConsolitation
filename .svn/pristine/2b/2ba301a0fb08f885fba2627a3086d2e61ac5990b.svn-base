/********************************************************************
	Переводит базу Oracle из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 


/* Start - <Номер задачи в ClearQuest> -Добавление перечня пользоваельских операции в OlapObjects - tsvetkov - 05.10.2010 */

alter table OlapObjects add BatchOperations VARCHAR(255) default null

go 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (89, '2.7.0.10', CONVERT(datetime, '2010.10.05', 102), GETDATE(), 'Добавление перечня пользоваельских операции в OlapObjects', 0);

go

/* End - <Номер задачи в ClearQuest> - Добавление перечня пользоваельских операции в OlapObjects - tsvetkov - 05.10.2010 */
