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

/* Start - Период.Период - изменена генерация ID для полей - zaharchenko - 09.12.2009 */

whenever SQLError continue commit;

alter table fx_Date_YearDayUNV modify DateMonthID NUMBER(10); 

create or replace procedure sp_FillDateUNV as
tmpNewDate pls_integer;
yearKey pls_integer;
yearBase pls_integer;
yearKeyPCH pls_integer;
halfKey pls_integer;
half1KeyPCH pls_integer;
half2KeyPCH pls_integer;
quarterKey pls_integer;
quarterKeyPCH pls_integer;
quarter1KeyPCH pls_integer;
quarter2KeyPCH pls_integer;
quarter3KeyPCH pls_integer;
quarter4KeyPCH pls_integer;
monthKey pls_integer;
monthBase pls_integer;
monthKeyPCH pls_integer;
monthName varchar2(22);
MaxDaysInMonth pls_integer;
dayKey pls_integer;
fullKey pls_integer;
begin

    halfKey := 1;
    quarterKey := 1;

    for tmpYear in 1998..2020 loop

        yearKey := tmpYear;
        yearBase := yearKey * 10000;
        yearKeyPCH := yearBase + 1;

        -- Здесь вставляем служебные записи
        
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1 where ID = yearKeyPCH;
        
        half1KeyPCH := yearBase + 10;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+10 where ID = half1KeyPCH;
        
        half2KeyPCH := yearBase + 20;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+20 where ID = half2KeyPCH;
        
        quarter1KeyPCH := yearBase + 9991;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+10, DateQuarterID = yearKey*10000+9991 where ID = quarter1KeyPCH;
        
        quarter2KeyPCH := yearBase + 9992;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+10, DateQuarterID = yearKey*10000+9992 where ID = quarter2KeyPCH;
        
        quarter3KeyPCH := yearBase + 9993;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+20, DateQuarterID = yearKey*10000+9993 where ID = quarter3KeyPCH;

        quarter4KeyPCH := yearBase + 9994;
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000+20, DateQuarterID = yearKey*10000+9994 where ID = quarter4KeyPCH;
        
        for tmpMonth in 1..12 loop

            monthKey := tmpMonth;
            monthBase := monthKey * 100;

            halfKey := case monthKey
                when 1 then 1
                when 2 then 1
                when 3 then 1
                when 4 then 1
                when 5 then 1
                when 6 then 1
                when 7 then 2
                when 8 then 2
                when 9 then 2
                when 10 then 2
                when 11 then 2
                when 12 then 2
                else 2
            end;

            quarterKey := case monthKey
                when 1 then 1
                when 2 then 1
                when 3 then 1
                when 4 then 2
                when 5 then 2
                when 6 then 2
                when 7 then 3
                when 8 then 3
                when 9 then 3
                when 10 then 4
                when 11 then 4
                when 12 then 4
                else 4
            end;

            monthName := case monthKey
                when 1 then 'Январь'
                when 2 then 'Февраль'
                when 3 then 'Март'
                when 4 then 'Апрель'
                when 5 then 'Май'
                when 6 then 'Июнь'
                when 7 then 'Июль'
                when 8 then 'Август'
                when 9 then 'Сентябрь'
                when 10 then 'Октябрь'
                when 11 then 'Ноябрь'
                when 12 then 'Декабрь'
            end;

            monthKeyPCH   := yearBase + monthBase;
            quarterKeyPCH := yearBase + 9990 + quarterKey;

            update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000 + (halfKey*10), DateQuarterID = yearKey*10000 + 9990 + quarterKey, DateMonthID = yearKey*10000 + (monthKey*100)  where ID = monthKeyPCH;

            MaxDaysInMonth := case monthKey
                when 1 then 31
                when 2 then 29
                when 3 then 31
                when 4 then 30
                when 5 then 31
                when 6 then 30
                when 7 then 31
                when 8 then 31
                when 9 then 30
                when 10 then 31
                when 11 then 30
                when 12 then 31
                else 0
            end;
            if (MOD(yearKey, 4) = 0) and (monthKey = 2) then
                MaxDaysInMonth := 29;
            end if;

            for tmpDay in 1..MaxDaysInMonth loop

                dayKey := tmpDay;
                fullKey := yearBase + monthBase + dayKey;
                
                update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000 + (halfKey*10), DateQuarterID = yearKey*10000 + 9990 + quarterKey, DateMonthID = yearKey*10000 + (monthKey*100), DateDayID = yearKey*10000 + (monthKey*100)+dayKey  where ID = fullKey;

            end loop;

        -- Заключительные обороты
        update fx_Date_YearDayUNV set DateYearID = yearKey*10000+1, DateHalfYearID = yearKey*10000 + (halfKey*10), DateQuarterID = yearKey*10000 + 9990 + quarterKey, DateMonthID = yearKey*10000 + (monthKey*100), DateDayID = yearBase + monthBase + 32  where ID = yearBase + monthBase + 32;
                    
        end loop;

        -- Остатки на начало года
    
    update fx_Date_YearDayUNV set DateYearID = yearKey*10000 +1, DateHalfYearID = -1, DateQuarterID = -1, DateMonthID = -1, DateDayID = -1  where ID = yearKey*10000;
     
    end loop;


end sp_FillDateUNV;
/

begin sp_FillDateUNV; end;

/

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (77, '2.6.0.6', To_Date('09.12.2009', 'dd.mm.yyyy'), SYSDATE, 'Период.Период - изменена генерация ID для полей', 0);

commit;

whenever SQLError exit rollback;

/* End - Период.Период - изменена генерация ID для полей - zaharchenko - 09.12.2009 */
