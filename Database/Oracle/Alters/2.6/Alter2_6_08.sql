/********************************************************************
	Переводит базу Oracle из версии 2.5 в следующую версию 2.6 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
whenever SQLError exit rollback; 
/* End   - Стандартная часть */ 


/* Start - FMQ00012750 - Период.Соответствие операц дней - исправление дял 2010 года - tsvetkov - 04.02.2010 */

alter trigger t_d_Date_ConversionFK_aa disable;

declare
	tmpMaxDay pls_integer;
	tmpNewDate pls_integer;
	tmpFODate pls_integer;
	tmpFOYear pls_integer;
	tmpFOMonth pls_integer;
	tmpFODay pls_integer;
	tmpDayOfWeek pls_integer;
	tmpYear pls_integer;
begin
  tmpDayOfWeek := 5;
  tmpYear := 2010;

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

        if (tmpDay <> 0) and (tmpMonth <> 0) then

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

          update d_Date_ConversionFK set RefFODate = tmpFODate where ID = tmpNewDate;

      if (tmpDay = 31) and (tmpMonth = 12) then
          tmpFODay := 32;
            tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpFODay;
            update d_Date_ConversionFK set RefFODate = tmpNewDate where ID = tmpNewDate;
      end if;

        else

        update d_Date_ConversionFK set RefFODate = tmpNewDate where ID = tmpNewDate;

        end if;

      end loop;
      
  end loop;

end;
/

alter trigger t_d_Date_ConversionFK_aa enable;

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (79, '2.6.0.8', To_Date('03.02.2010', 'dd.mm.yyyy'), SYSDATE, 'Период.Соответствие операц дней - исправление дял 2010 года', 0);

/* End - FMQ00012750 - Период.Соответствие операц дней - исправление дял 2010 года - tsvetkov - 04.02.2010 */


