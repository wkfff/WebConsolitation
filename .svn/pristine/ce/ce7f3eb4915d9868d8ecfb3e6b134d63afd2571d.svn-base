/*******************************************************************
 Переводит базу Oracle из версии 2.4.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */

whenever SQLError exit rollback;

/* End   - Стандартная часть */

/* Start -  - Добавлен модуль для прогноза развития региона - chubov - 03.02.2009 */

insert into RegisteredUIModules (ID, Name, FullName, Description) 
values (160, 'ForecastUI', 'Krista.FM.Client.ViewObjects.ForecastUI.ForecastNavigation, Krista.FM.Client.ViewObjects.ForecastUI', 'Прогноз развития региона');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (62, '2.5.0.6', To_Date('03.02.2009', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль для прогноза развития региона.', 0);

commit;

/* End   -  - Добавлен модуль для прогноза развития региона - chubov - 03.02.2009 */
