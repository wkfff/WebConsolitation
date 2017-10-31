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

/* Start -  - Добавлен модуль "Классификаторы и таблицы" взамен все модулей  работы с классификаторами - gbelov - 8.12.2008 */

delete from RegisteredUIModules where ID in (20, 30, 40, 50, 60, 110);

insert into RegisteredUIModules (ID, Name, FullName, Description) values (150, 'EntityNavigationListUI', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation.EntityNavigationControl, Krista.FM.Client.ViewObjects.AssociatedCLSUI', 'Классификаторы и таблицы');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (59, '2.5.0.3', To_Date('08.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль "Классификаторы и таблицы" взамен все модулей  работы с классификаторами.', 0);

commit;

/* End   -  - Добавлен модуль "Классификаторы и таблицы" взамен все модулей  работы с классификаторами - gbelov - 8.12.2008 */
