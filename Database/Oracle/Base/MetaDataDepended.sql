/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		MetaDataDepended.sql - скрипты зависимые от генерируемым метаданных
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Системные таблицы
pro ================================================================================

/* Чтобы аудит не матерился */
begin
  DBMS_SESSION.SET_IDENTIFIER('script_identifier');
end;
/

begin
  -- Call the procedure
  dvcontext.setvalue('SESSIONID',
                     'script_session',
                     'DV',
                      'script_identifier');
end;
/
begin
  -- Call the procedure
  dvcontext.setvalue('USERNAME',
                     'script_user',
                     'DV',
                     'script_identifier');
end;
/

/* Заполняет классификатор соотвествие дней */
create or replace procedure sp_ConversionFKDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpFODate pls_integer;
tmpFOYear pls_integer;
tmpFOMonth pls_integer;
tmpFODay pls_integer;
tmpDayOfWeek pls_integer;
begin
  tmpDayOfWeek := 4;
	for tmpYear in 1998..2020 loop

  	for tmpMonth in 0..12 loop

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 28
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

      if (MOD(tmpYear, 4) = 0) and (tmpMonth = 2) then
        tmpMaxDay := 29;
      end if;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

        if (tmpYear <> 2020) and (tmpDay <> 0) and (tmpMonth <> 0) then

          tmpFOMonth := tmpMonth;
          tmpFOYear := tmpYear;

          tmpFODay := tmpDay;
          if tmpDayOfWeek = 5 then
            tmpFODay := tmpFODay + 2;
          elsif tmpDayOfWeek = 6 then
            tmpFODay := tmpFODay + 1;
          elsif tmpDayOfWeek = 7 then
            tmpDayOfWeek := 0;
          end if;

          if tmpFODay >= tmpMaxDay then
            tmpFODay := tmpFODay - tmpMaxDay + 1;
            if tmpFOMonth = 12 then
              tmpFOMonth := 1;
              tmpFOYear := tmpFOYear + 1;
            else
              tmpFOMonth := tmpFOMonth + 1;
            end if;
          else
            tmpFODay := tmpFODay + 1;
          end if;

          tmpFODate := tmpFOYear * 10000 + tmpFOMonth * 100 + tmpFODay;
          tmpDayOfWeek := tmpDayOfWeek + 1;

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpFODate);

		  if (tmpDay = 31) and (tmpMonth = 12) then
		  	  tmpFODay := 32;
	          tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpFODay;
	          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
    	      values (tmpNewDate, tmpNewDate, tmpNewDate);
		  end if;

        else

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpNewDate);

        end if;

      end loop;

	  end loop;

	end loop;

end sp_ConversionFKDate;
/

whenever SQLError continue commit;

alter trigger t_d_date_conversionfk_aa disable;

whenever SQLError exit rollback;

begin sp_ConversionFKDate; end;
/

commit;

whenever SQLError continue commit;

alter trigger t_d_date_conversionfk_aa enable;

/* Скрипт на заполнение классификатора «Период.Рабочие дни» */
DECLARE
   CURSOR DaysWorkingCursor
     IS SELECT CAST(TO_CHAR(DayoffDate, 'YYYYMMDD') AS NUMBER) as DayoffDate 
     FROM d_Fact_Holidays 
    WHERE DayOff = 0;
   CURSOR ConversionFKCursor
     IS SELECT RefFODate 
     FROM d_Date_ConversionFK con
    WHERE NOT EXISTS ( SELECT NULL 
                       FROM d_Fact_Holidays 
                        WHERE TO_CHAR(DayoffDate, 'YYYYMMDD') = CAST(con.RefFODate AS VARCHAR2(8))
                                AND DayOff = 1 )
     AND TO_CHAR(RefFODate) >= (SELECT TO_CHAR(MIN(DayoffDate), 'YYYYMMDD') FROM d_Fact_Holidays)
     AND TO_CHAR(RefFODate) <= (SELECT TO_CHAR(MAX(DayoffDate), 'YYYYMMDD') FROM d_Fact_Holidays);
   ------------------------------------------------------------

BEGIN
   FOR r IN ConversionFKCursor LOOP
         INSERT INTO d_Date_WorkingDays 
           ( DaysWorking )
           VALUES ( r.RefFODate );
   END LOOP;
   FOR r IN DaysWorkingCursor LOOP
         INSERT INTO d_Date_WorkingDays 
           ( DaysWorking )
           VALUES ( r.DayoffDate );
   END LOOP;
   ------------------------------------------------------------
   -- удалим дубликаты
   DELETE d_Date_WorkingDays 

    WHERE ( id NOT IN ( SELECT MAX(t2.id) 
                        FROM d_Date_WorkingDays  t2
                          GROUP BY t2.DaysWorking )
           )
            AND ( id NOT IN ( SELECT MAX(t2.id) 
                              FROM d_Date_WorkingDays  t2
                                GROUP BY t2.DaysWorking )
           ); 
     
END;
/

commit;



whenever SQLError exit rollback;

