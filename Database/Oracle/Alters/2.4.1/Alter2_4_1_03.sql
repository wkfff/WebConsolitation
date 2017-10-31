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

/* Start -  - Добавлен модуль для планирования источников финансирования - gbelov - 16.07.2008 */

insert into RegisteredUIModules (ID, Name, FullName, Description) values (140, 'FinSourcePlanningUI', 'Krista.FM.Client.ViewObjects.FinSourcePlanningUI.FinSourcePlanningNavigation, Krista.FM.Client.ViewObjects.FinSourcePlanningUI', 'Источники финансирования');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (51, '2.4.1.3', To_Date('16.07.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль для планирования источников финансирования.', 0);

commit;

whenever SQLError exit rollback;

/* End   -  - Добавлен модуль для планирования источников финансирования - gbelov - 16.07.2008 */
