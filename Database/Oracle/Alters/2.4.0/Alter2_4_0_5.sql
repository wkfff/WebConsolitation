/*******************************************************************
 Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 6748 - Расширение поля DV.OlapObjects.ProcessResult до 2000 символов - gbelov - 20.02.2008 */

alter table OlapObjects
	modify ProcessResult varchar2 (2000);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (39, '2.4.0.5', To_Date('20.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Расширение поля DV.OlapObjects.ProcessResult до 2000 символов', 0);

commit;

whenever SQLError exit rollback;

/* End   - 6748 - Расширение поля DV.OlapObjects.ProcessResult до 2000 символов - gbelov - 20.02.2008 */
