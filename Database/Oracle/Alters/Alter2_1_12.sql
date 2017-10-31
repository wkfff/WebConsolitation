/*******************************************************************
 Переводит базу Oracle из версии 2.1.12 в следующую версию 2.1.13
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 3366 - ФО_0001_АС Бюджет - обработка изменения - mik-a-el - 9.08.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PreviewData State="InQueue" Comment="Никаких действий не выполняется."/>
		<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData State="InQueue" Comment="Заполняется классификатор показателей АС Бюджет."/>
		<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
		<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 3366 - ФО_0001_АС Бюджет - обработка изменения - mik-a-el - 9.08.2006 */


/* Start - 3374 - Добавить измерение "Период.Год Квартал Месяц" - gbelov - 11.08.2006 */

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
	DateMonthID			number (2) not null,
	DateDay				varchar2 (255) not null,
	DateDayID			number (10) not null,
	Name				varchar2 (255) not null,
	ParentID			number (10),
	OrderByDefault		number (10),
	constraint PKDateYearDayUNV primary key ( ID ),
	constraint FKDateYearDayUNVParentID foreign key ( ParentID )
		references fx_Date_YearDayUNV ( ID ) on delete set null
);

insert into MetaObjects (ID, Semantic, Name, Class, SubClass, RefPackages) values (0, 'Date', 'YearDayUNV', 2, 4, 1);

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

	for tmpYear in 1998..2010 loop

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

			if monthKey = 12 then
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

commit;

/* End - 3374 - Добавить измерение "Период.Год Квартал Месяц" - gbelov - 11.08.2006 */

/* Start - несколько задач по протоколам - Именения по протоколам - paluh - 21.08.2006 */
/* Сильно ошибочный скрипт, поэтому комментируем его дабы не привести к выполнению */
/*
delete from KINDSOFEVENTS where (ID >= 40101 and ID <= 40108) or (ID >= 40201 and ID <= 40208);

commit;

delete from HUB_EVENTPROTOCOL where (REFKINDSOFEVENTS >= 40101 and REFKINDSOFEVENTS <= 40108) or (ID >= 40201 and ID <= 40208);

commit;
*/
/* End - несколько задач по протоколам - Именения по протоколам - paluh - 21.08.2006 */


/* Start - 3226 - ФНС_0003_1НМ - закачка - mik-a-el - 22.08.2006 */

update PUMPREGISTRY
set PROGRAMCONFIG =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
		<ControlsGroup Type="Control" ParamsKind="General">
			<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
		</ControlsGroup>
		<ControlsGroup Type="Control" ParamsKind="General">
			<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
		</ControlsGroup>
	</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form1NMPump';

commit;

/* End - 3226 - ФНС_0003_1НМ - закачка - mik-a-el - 22.08.2006 */


/* Start -  - Реорганизация структуры пакетов - gbelov - 22.08.2006 */

create global temporary table tempLog
(
	ID					number (10) not null,		/* PKID */
	Data				varchar2 (255) not null,
	constraint PKtemp_log primary key ( ID )
);

create sequence g_tempLog;

create or replace trigger t_tempLog_bi before insert on tempLog for each row
begin
	if :new.ID is null then select g_tempLog.NextVal into :new.ID from Dual; end if;
end t_tempLog_bi;
/

declare
	packageID pls_integer;
	recordCount pls_integer;
begin

	select count(*) into recordCount from MetaPackages where Name like 'АДМИН%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'АДМИН', 0, 'АДМИН.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'АДМИН_0001_Отчет об освоении средств.xml' where Name = 'АДМИН_0001_Отчет об освоении средств';
		update MetaPackages set RefParent = packageID, PrivatePath = 'АДМИН_0002_Результат доходов.xml' where Name = 'АДМИН_0002_Результат доходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'АДМИН_0003_Проект расходов.xml' where Name = 'АДМИН_0003_Проект расходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'АДМИН_0004_Результат ИФ.xml' where Name = 'АДМИН_0004_Результат ИФ';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов АДМИН');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'МОФО%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'МОФО', 0, 'МОФО.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0001_Недоимка.xml' where Name = 'МОФО_0001_Недоимка';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0002_Задолженность.xml' where Name = 'МОФО_0002_Задолженность';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0003_Уточненный план.xml' where Name = 'МОФО_0003_Уточненный план';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0004_Результат доходов.xml' where Name = 'МОФО_0004_Результат доходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0005_Субсидии населению.xml' where Name = 'МОФО_0005_Субсидии населению';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0006_Потребление и оплата услуг.xml' where Name = 'МОФО_0006_Потребление и оплата услуг';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0007_Сельские специалисты.xml' where Name = 'МОФО_0007_Сельские специалисты';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МОФО_0008_Результат доходов_краткосрочное.xml' where Name = 'МОФО_0008_Результат доходов_краткосрочное';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов МОФО');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'МФРФ%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'МФРФ', 0, 'МФРФ.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'МФРФ_0001_Фонды.xml' where Name = 'МФРФ_0001_Фонды';
		update MetaPackages set RefParent = packageID, PrivatePath = 'МФРФ_0002_Мониторинг требований БК.xml' where Name = 'МФРФ_0002_Мониторинг требований БК';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов МФРФ');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'РЕГИОН%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'РЕГИОН', 0, 'РЕГИОН.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'РЕГИОН_0002_131ФЗ.xml' where Name = 'РЕГИОН_0002_131ФЗ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'РЕГИОН_0003_Паспорт региона.xml' where Name = 'РЕГИОН_0003_Паспорт региона';
		update MetaPackages set RefParent = packageID, PrivatePath = 'РЕГИОН_0007_131ФЗ Полномочия МОРФ.xml' where Name = 'РЕГИОН_0007_131ФЗ Полномочия МОРФ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'РЕГИОН_0008_131ФЗ Полномочия субъектов РФ.xml' where Name = 'РЕГИОН_0008_131ФЗ Полномочия субъектов РФ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'РЕГИОН_0009_131ФЗ_Бюджет МО.xml' where Name = 'РЕГИОН_0009_131ФЗ_Бюджет МО';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов РЕГИОН');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'СТАТ%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'СТАТ', 0, 'СТАТ.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'СТАТ_0003_Население.xml' where Name = 'СТАТ_0003_Население';
		update MetaPackages set RefParent = packageID, PrivatePath = 'СТАТ_0007_Образование.xml' where Name = 'СТАТ_0007_Образование';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов СТАТ');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'УФК%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'УФК', 0, 'УФК.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = '_Общие классификаторы_УФК.xml' where Name = '_Общие классификаторы_УФК';
		update MetaPackages set RefParent = packageID, PrivatePath = '_Общие классификаторы_УФК_Планирование.xml' where Name = '_Общие классификаторы_УФК_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0001_Форма 16.xml' where Name = 'УФК_0001_Форма 16';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0001_Форма 16_Планирование.xml' where Name = 'УФК_0001_Форма 16_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0002_СводФУ.xml' where Name = 'УФК_0002_СводФУ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0002_СводФУ_Планирование.xml' where Name = 'УФК_0002_СводФУ_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0003_Форма 14.xml' where Name = 'УФК_0003_Форма 14';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0003_Форма 14.xml' where Name = 'УФК_0004_Аренда';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0005_Доходы.xml' where Name = 'УФК_0005_Доходы';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0006_Форма 13.xml' where Name = 'УФК_0006_Форма 13';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0007_1Н 7 Сводная ведомость поступлений (ежемесячная).xml' where Name = 'УФК_0007_1Н 7 Сводная ведомость поступлений (ежемесячная)';
		update MetaPackages set RefParent = packageID, PrivatePath = 'УФК_0008_1Н DP Реестр перечисленных поступлений.xml' where Name = 'УФК_0008_1Н DP Реестр перечисленных поступлений';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов УФК');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'ФК%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'ФК', 0, 'ФК.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФК_0001_МесОтч.xml' where Name = 'ФК_0001_МесОтч';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов ФК');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'ФНС%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'ФНС', 0, 'ФНС.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФНС_0001_28н.xml' where Name = 'ФНС_0001_28н';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФНС_0001_28н_Планирование.xml' where Name = 'ФНС_0001_28н_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФНС_0002_1 ОБЛ.xml' where Name = 'ФНС_0002_1 ОБЛ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФНС_0003_1 НМ.xml' where Name = 'ФНС_0003_1 НМ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФНС_0004_5 НИО.xml' where Name = 'ФНС_0004_5 НИО';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов ФНС');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'ФО%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'ФО', 0, 'ФО.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0001_АС Бюджет\ФО_0001_АС Бюджет.xml' where Name = 'ФО_0001_АС Бюджет';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0001_АС Бюджет\ФО_0001_АС Бюджет_Планирование.xml' where Name = 'ФО_0001_АС Бюджет_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0002_МесОтч.xml' where Name = 'ФО_0002_МесОтч';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0002_МесОтч_Планирование.xml' where Name = 'ФО_0002_МесОтч_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0003_Проект доходов.xml' where Name = 'ФО_0003_Проект доходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0005_ГодОтч.xml' where Name = 'ФО_0005_ГодОтч';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0005_ГодОтч_Планирование.xml' where Name = 'ФО_0005_ГодОтч_Планирование';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0007_Проект расходов.xml' where Name = 'ФО_0007_Проект расходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0008_Проект ИФ.xml' where Name = 'ФО_0008_Проект ИФ';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0009_Фонды.xml' where Name = 'ФО_0009_Фонды';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0010_Поступление доходов.xml' where Name = 'ФО_0010_Поступление доходов';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0011_Проект доходов_краткосрочное.xml' where Name = 'ФО_0011_Проект доходов_краткосрочное';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0012_Проект доходов по объектам.xml' where Name = 'ФО_0012_Проект доходов по объектам';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0014_Предельные объемы бюджетного финансирования.xml' where Name = 'ФО_0014_Предельные объемы бюджетного финансирования';
		update MetaPackages set RefParent = packageID, PrivatePath = 'ФО_0015_Показатели для планирования.xml' where Name = 'ФО_0015_Показатели для планирования';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов ФО');

	end if;

	select count(*) into recordCount from MetaPackages where Name like 'ЭО%';
	if recordCount > 0 then

		select g_MetaPackages.NextVal into packageID from dual;

		insert into MetaPackages (ID, Name, BuiltIn, PrivatePath, RefParent) values (packageID, 'ЭО', 0, 'ЭО.xml', null);
		update MetaPackages set RefParent = packageID, PrivatePath = 'ЭО_0001_Экономические показатели.xml' where Name = 'ЭО_0001_Экономические показатели';

		insert into tempLog (Data) values ('Изменено ' || recordCount || ' пакетов ЭО');

	end if;

end;
/

select * from tempLog order by ID;

commit;

drop table tempLog;
drop sequence g_tempLog;

commit;

/* End -  - Реорганизация структуры пакетов - gbelov - 22.08.2006 */



/* Start - Изменения структуры протоколов - paluh - 24.08.2006 */

-- Добовление записи о протоколе необработанных ошибок

whenever SQLError continue commit;

alter table SAT_USERSOPERATIONS add USERHOST varchar2 (255);

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40666, 0, 'Необработанная ошибка');

commit;

alter table HUB_EVENTPROTOCOL modify (Module varchar2(255));

commit;

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40100, 0, 'Пользователь начал работу');

commit;


/* Представление на протокол "Действия пользователей" для выборки, вставки и удаления записей */
create or replace view UsersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage,
	UserName, ObjectName, ActionName, UserHost
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage,
	SAT.UserName, SAT.ObjectName, SAT.ActionName, SAT.USERHOST
from HUB_EventProtocol HUB join SAT_UsersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_UsersOperations_i instead of insert on UsersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	NewID := :new.ID;
	-- Если ИД не было передано - получаем значение из генератора
	if NewID is null then select g_HUB_EventProtocol.NextVal into NewID from Dual; end if;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, ClassOfProtocol)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, 5);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_UsersOperations (ID, UserName, ObjectName, ActionName, UserHost)
	values (NewID, :new.UserName, :new.ObjectName, :new.ActionName, :new.UserHost);
end t_UsersOperations_i;
/

create or replace trigger t_UsersOperations_u instead of update on UsersOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_UsersOperations_u;
/

create or replace trigger t_UsersOperations_d instead of delete on UsersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UsersOperations_d;
/

commit;

-- добавление сообщений

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (901, 0, 'Начало операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (902, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (903, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (904, 5, 'Успешное окончание операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (905, 6, 'Окончание операции закачки с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (906, 3, 'Ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (907, 4, 'Критическая ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (908, 0, 'Начало закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (909, 5, 'Завершение закачки файла с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (910, 6, 'Успешное завершение закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (911, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (912, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (913, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (111, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (112, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (113, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (211, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (212, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (213, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (311, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (312, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (313, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (611, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (612, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (613, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (711, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (712, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (713, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (811, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (812, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (813, 6, 'Успешное завершение обработки источника данных');

commit;

/* Предпросмотр данных */
create table SAT_PreviewData
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DataPreview primary key ( ID ),
	constraint FKSAT_DataPreview foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Закачка данных" для выборки, вставки и удаления записей */
create or replace view PreviewDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_PreviewData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_PreviewDataProtocol_i instead of insert on PreviewDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 1);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_PreviewData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_PreviewDataProtocol_i;
/

create or replace trigger t_PreviewDataProtocol_u instead of update on PreviewDataProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_PreviewDataProtocol_u;
/

create or replace trigger t_PreviewDataProtocol_d instead of delete on PreviewDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_PreviewDataProtocol_d;
/

whenever SQLError exit rollback;

/* End - Изменения структуры протоколов - paluh - 24.08.2006 */



/* Start - 3396 - Разные конфигурации для разных организаций - Paluh - 13.09.2006 */

create or replace trigger t_REGISTEREDUIMODULES_d before delete on REGISTEREDUIMODULES for each row
begin
	delete from OBJECTS obj where obj.NAME = :old.NAME;
end t_REGISTEREDUIMODULES_d;
/

commit;

/* End - 3396 - Разные конфигурации для разных организаций - Paluh - 13.09.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (18, '2.1.13', SYSDATE, SYSDATE, '');

commit;
