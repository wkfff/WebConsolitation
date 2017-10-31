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

/* Start - FMQ00011203 - Долговая книга - Paluh - 24.08.2009 */

whenever SQLError continue commit;

/* Регистрация интерфейса пользоватебя "Долговая книга" */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (170, 'FinSourceDebtorBookUI', 'Долговая книга', 'Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI');

commit;

/* Добавление ссылки в таблице пользователей на классификатор "Районы.Анализ" */
alter table Users Add RefRegion number (10);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (72, '2.6.0.1', To_Date('24.08.2009', 'dd.mm.yyyy'), SYSDATE, 'Долговая книга', 0);

commit;

whenever SQLError exit rollback;

/* End - FMQ00011203 - Изменения в структуре хранения отчетов репозитория - Paluh - 24.08.2009 */
