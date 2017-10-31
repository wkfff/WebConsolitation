/*******************************************************************
 Переводит базу Oracle из версии 2.6 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start -  - Объекты для назначения прав доступа к интерфейсу прогнозирования развития региона - Chubov - 06.10.2009 */

whenever SQLError continue commit;

insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', 'Сценарий', 'Сценарий для прогноза развития региона', 25000);

insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', 'Вариант расчета', 'Вариант расчета для прогноза развития региона', 25000);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (74, '2.6.0.3', To_Date('06.10.2009', 'dd.mm.yyyy'), SYSDATE, 'Объекты для назначения прав доступа к интерфейсу прогнозирования', 0);

commit;

whenever SQLError exit rollback;

/* End - Объекты для назначения прав доступа к интерфейсу прогнозирования развития региона - Chubov - 06.10.2009 */
