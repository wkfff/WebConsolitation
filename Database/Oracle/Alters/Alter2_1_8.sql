/*******************************************************************
 Переводит базу Oracle из версии 2.1.8 в следующую версию 2.1.9
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */



/* Start - 2736 - Изменение параметров закачки формы 16 - mik-a-el - 26.05.2006 */

update PUMPREGISTRY
set PROGRAMCONFIG =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
		<ControlsGroup Type="Control" ParamsKind="General">
			<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
		</ControlsGroup>
		<ControlsGroup Type="Control" ParamsKind="Individual">
			<Check Name="ucbShowPreview" Text="Показывать окно предварительного просмотра результатов разбора текстовых файлов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
		</ControlsGroup>
	</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form16Pump';

commit;

/* End - 2736 - Изменение параметров закачки формы 16 - mik-a-el - 26.05.2006 */


/* Start - 2737 - Поддержка версий 33.01 и 33.02 Бюджета - mik-a-el - 29.05.2006 */

update PUMPREGISTRY
set COMMENTS = 'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00, 33.01, 33.02'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 2737 - Поддержка версий 33.01 и 33.02 Бюджета - mik-a-el - 29.05.2006 */


/* Start - 2647 - ФК_0001_МесОтч- закачка изменения - mik-a-el - 30.05.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FKMonthRepPump';

commit;

/* End   - 2647 - ФК_0001_МесОтч- закачка изменения - mik-a-el - 30.05.2006 */


/* Start - 2529 - УФК_0002_СводФУ - закачка заключительных оборотов - mik-a-el - 31.05.2006 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="Закачка заключительных оборотов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

commit;

/* End - 2529 - УФК_0002_СводФУ - закачка заключительных оборотов - mik-a-el - 31.05.2006 */



/* Start - 2727 - Период.День - соотвествие дней - gbelov - 31.05.2006 */

whenever SQLError continue commit;

create or replace procedure sp_ConversionFKDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpFODate pls_integer;
tmpFOMonth pls_integer;
tmpFODay pls_integer;
tmpDayOfWeek pls_integer;
begin
  tmpDayOfWeek := 4;
	for tmpYear in 1998..2010 loop

  	for tmpMonth in 0..13 loop

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

        if (tmpYear <> 2010) and (tmpDay <> 0) and (tmpMonth <> 0) then

          if tmpMonth = 13 then
            tmpFOMonth := 1;
          else
            tmpFOMonth := tmpMonth;
          end if;

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
            else
              tmpFOMonth := tmpFOMonth + 1;
            end if;
          else
            tmpFODay := tmpFODay + 1;
          end if;

          tmpFODate := tmpYear * 10000 + tmpFOMonth * 100 + tmpFODay;
          tmpDayOfWeek := tmpDayOfWeek + 1;

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpFODate);

        else

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpNewDate);

        end if;

      end loop;

	  end loop;

	end loop;

end sp_ConversionFKDate;
/

begin sp_ConversionFKDate; end;
/

whenever SQLError exit rollback;

/* End - 2727 - Период.День - соотвествие дней - gbelov - 31.05.2006 */



/* Start -  - Добавляем каскадное удаление пакетов - gbelov - 1.06.2006 */

alter table MetaPackages
	drop constraint FKMetaPackagesRefParent;

alter table MetaPackages
	add constraint FKMetaPackagesRefParent foreign key ( RefParent )
		references MetaPackages ( ID ) on delete cascade;

/* End -  - Добавляем каскадное удаление пакетов - gbelov - 1.06.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (14, '2.1.9', SYSDATE, SYSDATE, '');

commit;
