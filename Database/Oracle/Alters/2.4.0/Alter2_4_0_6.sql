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

/* Start - 7699 - Архивирование протоколов  - blinkov - 27.02.2008 */

whenever SQLError continue commit;

insert into KindsOfEvents (ID, ClassOfEvent, Name) 
values (40701, 5, 'Архивирование протоколов');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (40, '2.4.0.6', To_Date('27.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Архивирование протоколов', 0);

commit;

whenever SQLError exit rollback;

/* End - 7699 - Архивирование протоколов  - blinkov - 27.02.2008 */

