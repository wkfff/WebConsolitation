/*******************************************************************
  Переводит базу Sql Server 2005 из версии 2.6 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - FMQ00011203 - Долговая книга - Paluh - 24.08.2009 */

/* Регистрация интерфейса пользоватебя "Долговая книга" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (170, 'FinSourceDebtorBookUI', 'Долговая книга', 'Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI');

go

/* Добавление ссылки в таблице пользователей на классификатор "Районы.Анализ" */
alter table Users Add RefRegion int;

go


insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (72, '2.6.0.1', CONVERT(datetime, '2009.08.24', 102), GETDATE(), 'Долговая книга', 0);

go

/* End - FMQ00011203 - Изменения в структуре хранения отчетов репозитория - Paluh - 24.08.2009 */
