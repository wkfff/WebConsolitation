/*******************************************************************
 Переводит базу Oracle из версии 2.6 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Период.Период - изменена генерация ID для полей - zaharchenko - 09.12.2009 */

alter table fx_Date_YearDayUNV alter column DateMonthID numeric(10); 


        update fx_Date_YearDayUNV set DateYearID = DateYearID * 10000 +1;

        

go
        
        update fx_Date_YearDayUNV set DateHalfYearID = DateYearID - 1 + (DateHalfYearID *10);

        update fx_Date_YearDayUNV set DateHalfYearID = -1 where DateDay = 'Остатки на начало года';

        update fx_Date_YearDayUNV set DateHalfYearID = -2 where DateHalfYear = 'Данные года';

        update fx_Date_YearDayUNV set DateQuarterID = -2 where DateQuarter = 'Данные года';

        update fx_Date_YearDayUNV set DateMonthID = -2 where DateMonth = 'Данные года';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные года';


       

go
        
        update fx_Date_YearDayUNV set DateQuarterID = DateYearID - 1 + 9990 + DateQuarterID;

        update fx_Date_YearDayUNV set DateQuarterID = -1 where DateDay = 'Остатки на начало года';

 update fx_Date_YearDayUNV set DateQuarterID = -2 where DateQuarter = 'Данные полугодия';

update fx_Date_YearDayUNV set DateQuarterID = -2 where DateQuarter = 'Данные года';

        update fx_Date_YearDayUNV set DateMonthID = -2 where DateMonth = 'Данные полугодия';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные полугодия';


       

go

        update fx_Date_YearDayUNV set DateMonthID = DateYearID - 1 + (DateMonthID * 100);

        update fx_Date_YearDayUNV set DateMonthID = -1 where DateDay = 'Остатки на начало года';

        update fx_Date_YearDayUNV set DateMonthID = -2 where DateMonth = 'Данные года';

        update fx_Date_YearDayUNV set DateMonthID = -2 where DateMonth = 'Данные полугодия';

        update fx_Date_YearDayUNV set DateMonthID = -2 where DateMonth = 'Данные квартала';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные квартала';





go

        update fx_Date_YearDayUNV set DateDayID = DateMonthID + DateDayID;

        update fx_Date_YearDayUNV set DateDayID = DateMonthID + 32 where DateDay = 'Заключительные обороты';

        update fx_Date_YearDayUNV set DateDayID = -1 where DateDay = 'Остатки на начало года';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные года';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные полугодия';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные квартала';

        update fx_Date_YearDayUNV set DateDayID = -2 where DateDay = 'Данные месяца';


go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (77, '2.6.0.6', To_Date('09.12.2009', 'dd.mm.yyyy'), SYSDATE, 'Период.Период - изменена генерация ID для полей', 0);

go

/* End - Период.Период - изменена генерация ID для полей - zaharchenko - 09.12.2009 */
