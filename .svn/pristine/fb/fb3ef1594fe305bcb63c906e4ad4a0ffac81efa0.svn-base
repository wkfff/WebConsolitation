/*******************************************************************
 Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start 8569 - Добавлена фиксированная запись в системный классификатор fx_Date_YearDayUNV - gbelov - 29.05.2008 */

insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID,          DateHalfYear, DateQuarterID,           DateQuarter, DateMonthID,             DateMonth, DateDayID,               DateDay, OrderByDefault,     Name, ParentID)
						values (-1,       0,          0, 'Значение не указано', 0, 'Значение не указано',             0, 'Значение не указано',           0, 'Значение не указано',         0, 'Значение не указано', 0, 'Значение не указано', null);


insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (47, '2.4.0.13', CONVERT(datetime, '2008.05.29', 102), GETDATE(), 'Добавлена фиксированная запись в системный классификатор fx_Date_YearDayUNV.', 0);

go

/* End   8569 - Добавлена фиксированная запись в системный классификатор fx_Date_YearDayUNV - gbelov - 29.05.2008 */


