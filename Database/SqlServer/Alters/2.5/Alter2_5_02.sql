/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start -  - Добавлен модуль для планирования источников финансирования - gbelov - 16.07.2008 */

insert into templates (NAME, TYPE, PARENTID) values ('Шаблоны отчетов', 0, NULL);
insert into templates (NAME, TYPE, PARENTID) values ('Источники финансирования', 0, @@IDENTITY)

go

insert into RegisteredUIModules (ID, Name, FullName, Description) values (140, 'FinSourcePlanningUI', 'Krista.FM.Client.ViewObjects.FinSourcePlanningUI.FinSourcePlanningNavigation, Krista.FM.Client.ViewObjects.FinSourcePlanningUI', 'Источники финансирования');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (58, '2.5.0.2', CONVERT(datetime, '2008.12.02', 102), GETDATE(), 'Добавлен модуль для планирования источников финансирования.', 0);

go

/* End   -  - Добавлен модуль для планирования источников финансирования - gbelov - 16.07.2008 */
