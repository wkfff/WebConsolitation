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


/* Start - <Номер задачи в ClearQuest> - обавление перечня пользоваельских операции в OlapObjects - tsvetkov - 05.10.2010 */

alter table OlapObjects add BatchOperations VARCHAR2(255) default null; 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (89, '2.7.0.10', To_Date('05.10.2010', 'dd.mm.yyyy'), SYSDATE, 'Добавление перечня пользоваельских операции в OlapObjects', 0);

commit;

/* End - <Номер задачи в ClearQuest> - обавление перечня пользоваельских операции в OlapObjects - tsvetkov - 05.10.2010 */
