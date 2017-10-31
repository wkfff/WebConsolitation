/*******************************************************************
 Переводит базу Oracle из версии 2.5 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 11122 - добавление в классификатор fx.Date.YearDayUNV (Период.Год Квартал Месяц) заключительных оборотов по каждому месяцу - feanor - 22.07.2009 */

-- добавляем заключительные обороты по всем месяцам кроме 12 (в декабрях уже присутствуют)

create or replace procedure sp_FillFinalDate as
yearKey pls_integer;
yearBase pls_integer;
halfKey pls_integer;
quarterKey pls_integer;
monthKey pls_integer;
monthBase pls_integer; 
monthKeyPCH pls_integer;
monthName varchar2(22);
begin

	halfKey := 1;
	quarterKey := 1;

	for tmpYear in 1998..2020 loop

		yearKey := tmpYear;
		yearBase := yearKey * 10000;

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
      
			if (monthKey <> 12) then
				-- Заключительные обороты
				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (yearBase + monthBase + 32, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					32, 'Заключительные обороты',
					yearBase + monthBase + 32, 'Заключительные обороты', monthKeyPCH);
			end if;
			
		end loop;

	end loop;

end sp_FillFinalDate;
/

begin sp_FillFinalDate; end;
/

drop procedure sp_FillFinalDate;

commit;

-- заменяем процедуру формирования fx.Date.YearDayUNV: добавляем заключительные по всем месяцам (было только по декабрям)

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

		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name)
		values (yearKeyPCH, 0,
			yearKey, yearKey,
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			yearKeyPCH, yearKey);

		half1KeyPCH := yearBase + 10;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half1KeyPCH, 'Полугодие 1', yearKeyPCH);

		half2KeyPCH := yearBase + 20;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half2KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half2KeyPCH, 'Полугодие 2', yearKeyPCH);

		quarter1KeyPCH := yearBase + 9991;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			1, 'Квартал 1',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 1 * 10 + 20, 'Квартал 1', half1KeyPCH);

		quarter2KeyPCH := yearBase + 9992;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter2KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			2, 'Квартал 2',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 2 * 10 + 20, 'Квартал 2', half1KeyPCH);

		quarter3KeyPCH := yearBase + 9993;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter3KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			3, 'Квартал 3',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 3 * 10 + 20, 'Квартал 3', half2KeyPCH);

		quarter4KeyPCH := yearBase + 9994;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter4KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			4, 'Квартал 4',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 4 * 10 + 20, 'Квартал 4', half2KeyPCH);


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

			insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
			values (monthKeyPCH, 0,
				yearKey, yearKey,
				halfKey, 'Полугодие ' || halfKey,
				quarterKey, 'Квартал ' || quarterKey,
				monthKey, monthName,
				-2, 'Данные месяца',
				monthKeyPCH, monthName, quarterKeyPCH);

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

				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (fullKey, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					dayKey, dayKey,
					fullKey, dayKey, monthKeyPCH);

			end loop;

			-- Заключительные обороты
			insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
			values (yearBase + monthBase + 32, 0,
				yearKey, yearKey,
				halfKey, 'Полугодие ' || halfKey,
				quarterKey, 'Квартал ' || quarterKey,
				monthKey, monthName,
				32, 'Заключительные обороты',
				yearBase + monthBase + 32, 'Заключительные обороты', monthKeyPCH);

		end loop;

		-- Остатки на начало года
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (yearBase, 0,
			yearKey, yearKey,
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			yearBase, 'Остатки на начало года', yearKeyPCH);

	end loop;

end sp_FillDateUNV;
/

commit;
 
insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (70, '2.5.0.14', To_Date('22.07.2009', 'dd.mm.yyyy'), SYSDATE, 'fx.Date.YearDayUNV - закл. обороты за каждый месяц', 0);

commit;

/* End - 11122 - добавление в классификатор fx.Date.YearDayUNV (Период.Год Квартал Месяц) заключительных оборотов по каждому месяцу - feanor - 22.07.2009 */
