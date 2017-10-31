/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		NewDataFill.sql - Генерация дат
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Генерация дат
pro ================================================================================

create table fx_Date_YearDay
(
	ID					number (10) not null,		/* PKID */
	ROWTYPE				number (10) default 0 not null,
	DateYear			number (4) not null,
	DateHalfYear		varchar2 (255) not null,
	DateQuarter			varchar2 (255) not null,
	DateMonthID			number (2) not null,
	DateMonth			varchar2 (255) not null,
	DateDay				number (10) not null,
	constraint PKDateYearDay primary key ( ID )
);

create table fx_Date_YearMonth
(
	ID					number (10) not null,		/* PKID */
	ROWTYPE				number (10) default 0 not null,
	DateYear			number (4) not null,
	DateHalfYear		varchar2 (255) not null,
	DateQuarter			varchar2 (255) not null,
	DateMonthID			number (2) not null,
	DateMonth			varchar2 (255) not null,
	constraint PKDateYearMonth primary key ( ID )
);

create table fx_Date_Year
(
	ID					number (10) not null,		/* PKID */
	ROWTYPE				number (10) default 0 not null,
	DateYear			number (4) not null,
	constraint PKfx_Date_Year primary key ( ID )
);

create table fx_Date_YearDayUNV
(
	ID					number (10) not null,		/* PKID */
	ROWTYPE				number (10) default 0 not null,
	DateYear			varchar2 (255) not null,
	DateYearID			number (10) not null,
	DateHalfYear		varchar2 (255) not null,
	DateHalfYearID		number (10) not null,
	DateQuarter			varchar2 (255) not null,
	DateQuarterID		number (10) not null,
	DateMonth			varchar2 (255) not null,
	DateMonthID			number (10) not null,
	DateDay				varchar2 (255) not null,
	DateDayID			number (10) not null,
	Name				varchar2 (255) not null,
	ParentID			number (10),
	OrderByDefault		number (10),
	constraint PKDateYearDayUNV primary key ( ID ),
	constraint FKDateYearDayUNVParentID foreign key ( ParentID )
		references fx_Date_YearDayUNV ( ID ) on delete set null
);

insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
values (-1, 0, 0, 'Значение не указано', 0, 'Значение не указано', 0, 'Значение не указано', 0, 'Значение не указано', 0, 'Значение не указано', 0, 'Значение не указано', null);

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 1998..2025 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
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
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
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
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop

      		/*if (tmpDay = 32) and (tmpMonth = 12) then
      			tmpHalfYear := 'Заключительные обороты';
      			tmpQuarter := 'Заключительные обороты';
      			tmpMonthName := 'Заключительные обороты';
      		end if;*/

      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

			/* Вставляем 32-й день в каждый месяц */
      		if (tmpDay = tmpMaxDay and tmpMonth <> 0) then
	      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + 32;
	            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
	            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);
      		end if;

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

create or replace procedure sp_FillDateMonth as
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarterNo pls_integer;
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearMonth (ID, Year, HalfYear, Quarter, Month, MonthName) values (-1, 0, ' ', ' ', 0, ' ');

	for tmpYear in 1998..2020 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarterNo := 0;
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarterNo := 1;
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarterNo := 2;
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarterNo := 3;
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarterNo := 4;
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
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
			else '0'
      end;

	  if tmpMonth in (1, 4, 7, 10)  then

        tmpNewDate := tmpYear * 10000 + 9990 + tmpQuarterNo;

        insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, 'Квартал ' || tmpQuarterNo, 90 + tmpQuarterNo, 'Квартал ' || tmpQuarterNo);

	  end if;

      tmpNewDate := tmpYear * 10000 + tmpMonth * 100;

      insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName);

	end loop;

  end loop;

end sp_FillDateMonth;
/

exec sp_FillDateMonth;

create or replace procedure sp_FillDateYear as
begin
    --insert into fx_Date_Year (ID, Year) values (-1, 0);
	for tmpYear in 1998..2020 loop
        insert into fx_Date_Year (ID, DateYear) values (tmpYear, tmpYear);
	end loop;
end sp_FillDateYear;
/

exec sp_FillDateYear;

commit;


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

	for tmpYear in 1998..2025 loop

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

begin sp_FillDateUNV; end;
/

create or replace procedure sp_FillDateUNV_update as
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

    for tmpYear in 1998..2025 loop

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


end sp_FillDateUNV_update;
/

begin sp_FillDateUNV_update; end;
/

commit;
