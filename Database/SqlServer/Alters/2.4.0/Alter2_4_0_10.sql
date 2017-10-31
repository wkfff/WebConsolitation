/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 8052 - Какой пользователь запускал закачку - Feanor - 04.04.2008 */

alter table PumpHistory add UserName varchar (255);
alter table PumpHistory add UserHost varchar (255);
alter table PumpHistory add SessionID varchar (24);

go 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (44, '2.4.0.10', CONVERT(datetime, '2008.04.04', 102), GETDATE(), 'Инфа о пользователе в PumpHistory', 0);

go

/* End - 8052 - Какой пользователь запускал закачку - Feanor - 04.04.2008 */
