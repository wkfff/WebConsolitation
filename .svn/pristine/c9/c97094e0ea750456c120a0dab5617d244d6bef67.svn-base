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

/* Start - 8052 - Какой пользователь запускал закачку - Feanor - 04.04.2008 */

whenever SQLError continue commit;

alter table PumpHistory add UserName varchar2 (255);
alter table PumpHistory add UserHost varchar2 (255);
alter table PumpHistory add SessionID varchar2 (24);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (44, '2.4.0.10', To_Date('04.04.2008', 'dd.mm.yyyy'), SYSDATE, 'Инфа о пользователе в PumpHistory', 0);

commit;

whenever SQLError exit rollback;

/* End - 8052 - Какой пользователь запускал закачку - Feanor - 04.04.2008 */
