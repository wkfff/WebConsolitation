/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start -  - Добавлен модуль для прогноза развития региона - chubov - 19.12.2008 */


insert into RegisteredUIModules (ID, Name, FullName, Description) values (150, 'ForecastUI', 'Krista.FM.Client.ViewObjects.ForecastUI.ForecastNavigation, Krista.FM.Client.ViewObjects.ForecastUI', 'Прогноз развития региона');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (62, '2.5.0.6', CONVERT(datetime, '2008.12.19', 102), GETDATE(), 'Добавлен модуль для прогноза развития региона.', 0);

go

/* End   -  - Добавлен модуль для прогноза развития региона - chubov - 19.12.2008 */
