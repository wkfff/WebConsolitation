/*******************************************************************
 Переводит базу Oracle из версии 2.2.0 в следующую версию 2.3.0
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 4578 - АДМИН_0003_Проект расходов - mik-a-el - 21.11.2006 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Name="gbVariant" Text="Вариант" LocationX="13" LocationY="0" Width="320" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="edVariant" Text="Основной" Type="Edit" Value="Основной" LocationX="6" LocationY="20" Width="300" Height="20"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - 4578 - АДМИН_0003_Проект расходов - mik-a-el - 21.11.2006 */


/* Start - 3229 - Добавление флагов режимов аутентификации - borisov - 06.12.2006 */

whenever SQLError continue commit;

-- добавляем поля для флагов возможности входа в разных режимах
alter table Users add
(
	AllowDomainAuth number (1) default 0 not null,
	AllowPwdAuth number (1) default 0 not null
);

-- всем обычным пользователям разрешаем вход только в режиме доменной аутентификации
update Users set AllowDomainAuth = 1 where (ID >= 100);

-- закачке и администратору по умолчанию - только в режиме логин/пароль
update Users set AllowDomainAuth = 0, AllowPwdAuth = 1 where (ID in (1, 3));

commit;

whenever SQLError exit rollback;

/* End - 3229 - Добавление флагов режимов аутентификации - borisov - 06.12.2006 */


/* Start - 4867 - ФО_0001_АС Бюджет - поддержка версии БД 34.01 - mik-a-el - 19.12.2006 */

update PUMPREGISTRY
set COMMENTS = 'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00, 33.01, 33.02, 33.03, 34.00, 34.01'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 4867 - ФО_0001_АС Бюджет - поддержка версии БД 34.01 - mik-a-el - 19.12.2006 */



/* Start - 2251 - СУБД MS SQL Server 2005 - адаптация - gbelov - 21.12.2006 */
/* Изменение включено в патч 2.2.0.36 (15.01.2007 - borisov) */

whenever SQLError continue commit;

alter table MetaConversionTable
	rename column Rule to AssociateRule;

/* Представление для получения списка таблиц перекодировок */
create or replace view MetaConversionTablesCatalog (ID, AssociationName, RuleName) as
select CT.ID, 'a.' || L.Semantic || '.' || L.Name, CT.AssociateRule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

whenever SQLError exit rollback;

/* End   - 2251 - СУБД MS SQL Server 2005 - адаптация - gbelov - 21.12.2006 */


/* Start - 4957 - УФК_0011_Сводная ведомость ежедневная -новая - mik-a-el - 9.01.2007 */

update PUMPREGISTRY
set PROGRAMIDENTIFIER = 'Form1NApp7MonthPump'
where PROGRAMIDENTIFIER = 'Form1NApp7Pump';

update PUMPREGISTRY
set NAME = 'Закачка отчетов формы 1Н, приложение 7 (ежемесячная)'
where PROGRAMIDENTIFIER = 'Form1NApp7MonthPump';

whenever SQLError continue commit;

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('УФК', 00011, 'Form1NApp7DayPump', 'Закачка отчетов формы 1Н, приложение 7 (ежедневная)',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="Закачка заключительных оборотов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры Год и Месяц используются только при запуске обработки данных." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>',
'Приказ Федерального казначейства от 22 марта 2005 г. №1н «Об утверждении Порядка кассового обслуживания исполнения бюджетов субъектов Российской Федерации и местных бюджетов территориальными органами Федерального казначейства».',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData StageInitialState="InQueue" Comment="Заполняются поля Сумма за день таблицы фактов и Тип территории ОКАТО."/>
	<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

whenever SQLError exit rollback;

commit;

/* End   - 4957 - УФК_0011_Сводная ведомость ежедневная -новая - mik-a-el - 9.01.2007 */


/* Start - Unknown - УФК_0005_Доходы. Убираем параметр удаления закачанных данных - mik-a-el - 19.01.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры расщепления данных используются только при запуске обработки (расщепления) данных." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbDisintegrationMode" Text="Режим расщепления" LocationX="13" LocationY="70" Width="232" Height="70" Type="GroupBox" ParamsKind="Individual">
		<Radio Name="rbtnDisintegratedOnly" Text="Только нерасщепленные" LocationX="13" LocationY="20" Width="400" Height="20" Value="true" FontBold="false"/>
		<Radio Name="rbtnDisintAll" Text="Расщеплять все" LocationX="13" LocationY="40" Width="400" Height="20" Value="false" FontBold="false"/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="489" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

commit;

/* End   - Unknown - УФК_0005_Доходы. Убираем параметр удаления закачанных данных - mik-a-el - 19.01.2007 */


/* Start - Unknown - АДМИН_0003_ПРОЕКТ РАСХОДОВ. Убираем параметр заключительных оборотов - mik-a-el - 23.01.2007 */

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
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - Unknown - АДМИН_0003_ПРОЕКТ РАСХОДОВ. Убираем параметр заключительных оборотов - mik-a-el - 23.01.2007 */


/* Start - 4657 - Константы - paluh - 29.01.2007 */

create sequence g_GlobalConsts;

create table GlobalConsts
(
	ID				number (10) not null,		/* PK */
	Name				varchar2 (255) not null,	/* английское имя константы */
	Caption				varchar2 (1000) not null,	/* русское наименование */
	Description			varchar2 (2000) not null,	/* описание константы */
	Value				varchar2 (4000) not null,	/* значение константы */
	ConstValueType			number (10) not null,		/* тип значения константы */
	ConstCategory			number (10) not null,		/* категория константы */
	ConstType 			number (10) not null,		/* тип константы */
	constraint PKGlobalConsts primary key ( ID ),
	constraint UKGlobalConsts_Name unique ( Name )
);

create or replace trigger t_GlobalConsts_i before insert on GlobalConsts for each row
begin
	if :new.ID is null then
		select g_GlobalConsts.NextVal into :new.ID from Dual;
	end if;
end t_GlobalConsts_i;
/

commit;


/* End   - 4657 - Константы - paluh - 29.01.2007 */


/* Start - 5255 - ФО_0001_АС Бюджет - версия 6.8.2 - mik-a-el - 6.02.2007 */

update PUMPREGISTRY
set COMMENTS = 'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07 - 32.10, 33.00 - 33.03, 34.00 - 34.02'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 5255 - ФО_0001_АС Бюджет - версия 6.8.2 - mik-a-el - 6.02.2007 */


/* Start -  - Добавление интерфейса констант - paluh - 06.02.2007 */

insert into RegisteredUIModules (ID, Name, Description) values (120, 'GlobalConstsViewObj', 'Константы');

commit;

/* End   -  - Константы - paluh - 06.02.2007 */


/* Start - 5025 - ФНС_0003_1-НМ - корректировка закачки - mik-a-el - 12.02.2007 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData StageInitialState="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData StageInitialState="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
	<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form1NMPump';

commit;

/* End   - 5025 - ФНС_0003_1-НМ - корректировка закачки - mik-a-el - 12.02.2007 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (20, '2.3.0', SYSDATE, SYSDATE, '');

commit;
