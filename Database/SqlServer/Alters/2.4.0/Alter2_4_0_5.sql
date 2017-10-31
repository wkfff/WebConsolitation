/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 6748 - Расширение поля DV.OlapObjects.ProcessResult до 2000 символов - gbelov - 20.02.2008 */

alter table OlapObjects
	alter column ProcessResult varchar (2000);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (39, '2.4.0.5', CONVERT(datetime, '2008.02.20', 102), GETDATE(), 'Расширение поля DV.OlapObjects.ProcessResult до 2000 символов', 0);

go

/* End   - 6748 - Расширение поля DV.OlapObjects.ProcessResult до 2000 символов - gbelov - 20.02.2008 */
