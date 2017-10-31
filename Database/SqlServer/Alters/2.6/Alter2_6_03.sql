/*******************************************************************
  Переводит базу Sql Server 2005 из версии 2.6 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - - Объекты для назначения прав доступа к интерфейсу прогнозирования развития региона - Chubov - 06.10.2009 */


insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', 'Сценарий', 'Сценарий для прогноза развития региона', 25000);

go

insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', 'Вариант расчета', 'Вариант расчета для прогноза развития региона', 25000);

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (74, '2.6.0.3', CONVERT(datetime, '2009.10.06', 102), GETDATE(), 'Объекты для назначения прав доступа к интерфейсу прогнозирования', 0);

go

/* End  - Объекты для назначения прав доступа к интерфейсу прогнозирования развития региона - Chubov - 06.10.2009 */
