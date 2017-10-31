/*******************************************************************
 Переводит базу SQLиз версии 3.0 в следующую версию 3.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Период.Период - Изменен тип поля - zaharchenko - 30.08.2011 */

alter table fx_Date_YearDayUNV alter column DateMonthID int not null; 

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (100, '3.0.0.5', CONVERT(datetime, '2011.08.30', 102), GETDATE(), 'Период.Период - изменена генерация ID для полей', 0);

go

/* End - Период.Период - Изменен тип поля - zaharchenko - 30.08.2011 */=======