/*******************************************************************
 ��������� ���� Oracle �� ������ 2.2.0 � ��������� ������ 2.3.0
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 4578 - �����_0003_������ �������� - mik-a-el - 21.11.2006 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Name="gbVariant" Text="�������" LocationX="13" LocationY="0" Width="320" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="edVariant" Text="��������" Type="Edit" Value="��������" LocationX="6" LocationY="20" Width="300" Height="20"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - 4578 - �����_0003_������ �������� - mik-a-el - 21.11.2006 */


/* Start - 3229 - ���������� ������ ������� �������������� - borisov - 06.12.2006 */

whenever SQLError continue commit;

-- ��������� ���� ��� ������ ����������� ����� � ������ �������
alter table Users add
(
	AllowDomainAuth number (1) default 0 not null,
	AllowPwdAuth number (1) default 0 not null
);

-- ���� ������� ������������� ��������� ���� ������ � ������ �������� ��������������
update Users set AllowDomainAuth = 1 where (ID >= 100);

-- ������� � �������������� �� ��������� - ������ � ������ �����/������
update Users set AllowDomainAuth = 0, AllowPwdAuth = 1 where (ID in (1, 3));

commit;

whenever SQLError exit rollback;

/* End - 3229 - ���������� ������ ������� �������������� - borisov - 06.12.2006 */


/* Start - 4867 - ��_0001_�� ������ - ��������� ������ �� 34.01 - mik-a-el - 19.12.2006 */

update PUMPREGISTRY
set COMMENTS = '�������������� ������ �� �� ������: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00, 33.01, 33.02, 33.03, 34.00, 34.01'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 4867 - ��_0001_�� ������ - ��������� ������ �� 34.01 - mik-a-el - 19.12.2006 */



/* Start - 2251 - ���� MS SQL Server 2005 - ��������� - gbelov - 21.12.2006 */
/* ��������� �������� � ���� 2.2.0.36 (15.01.2007 - borisov) */

whenever SQLError continue commit;

alter table MetaConversionTable
	rename column Rule to AssociateRule;

/* ������������� ��� ��������� ������ ������ ������������� */
create or replace view MetaConversionTablesCatalog (ID, AssociationName, RuleName) as
select CT.ID, 'a.' || L.Semantic || '.' || L.Name, CT.AssociateRule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

whenever SQLError exit rollback;

/* End   - 2251 - ���� MS SQL Server 2005 - ��������� - gbelov - 21.12.2006 */


/* Start - 4957 - ���_0011_������� ��������� ���������� -����� - mik-a-el - 9.01.2007 */

update PUMPREGISTRY
set PROGRAMIDENTIFIER = 'Form1NApp7MonthPump'
where PROGRAMIDENTIFIER = 'Form1NApp7Pump';

update PUMPREGISTRY
set NAME = '������� ������� ����� 1�, ���������� 7 (�����������)'
where PROGRAMIDENTIFIER = 'Form1NApp7MonthPump';

whenever SQLError continue commit;

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('���', 00011, 'Form1NApp7DayPump', '������� ������� ����� 1�, ���������� 7 (����������)',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="������� �������������� ��������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ��� � ����� ������������ ������ ��� ������� ��������� ������." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="13" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>',
'������ ������������ ������������ �� 22 ����� 2005 �. �1� ��� ����������� ������� ��������� ������������ ���������� �������� ��������� ���������� ��������� � ������� �������� ���������������� �������� ������������ ������������.',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData StageInitialState="InQueue" Comment="����������� ���� ����� �� ���� ������� ������ � ��� ���������� �����."/>
	<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

whenever SQLError exit rollback;

commit;

/* End   - 4957 - ���_0011_������� ��������� ���������� -����� - mik-a-el - 9.01.2007 */


/* Start - Unknown - ���_0005_������. ������� �������� �������� ���������� ������ - mik-a-el - 19.01.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ����������� ������ ������������ ������ ��� ������� ��������� (�����������) ������." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbDisintegrationMode" Text="����� �����������" LocationX="13" LocationY="70" Width="232" Height="70" Type="GroupBox" ParamsKind="Individual">
		<Radio Name="rbtnDisintegratedOnly" Text="������ ��������������" LocationX="13" LocationY="20" Width="400" Height="20" Value="true" FontBold="false"/>
		<Radio Name="rbtnDisintAll" Text="���������� ���" LocationX="13" LocationY="40" Width="400" Height="20" Value="false" FontBold="false"/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="489" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

commit;

/* End   - Unknown - ���_0005_������. ������� �������� �������� ���������� ������ - mik-a-el - 19.01.2007 */


/* Start - Unknown - �����_0003_������ ��������. ������� �������� �������������� �������� - mik-a-el - 23.01.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="������� ���������� ����� ������ �� ���� �� ���������." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - Unknown - �����_0003_������ ��������. ������� �������� �������������� �������� - mik-a-el - 23.01.2007 */


/* Start - 4657 - ��������� - paluh - 29.01.2007 */

create sequence g_GlobalConsts;

create table GlobalConsts
(
	ID				number (10) not null,		/* PK */
	Name				varchar2 (255) not null,	/* ���������� ��� ��������� */
	Caption				varchar2 (1000) not null,	/* ������� ������������ */
	Description			varchar2 (2000) not null,	/* �������� ��������� */
	Value				varchar2 (4000) not null,	/* �������� ��������� */
	ConstValueType			number (10) not null,		/* ��� �������� ��������� */
	ConstCategory			number (10) not null,		/* ��������� ��������� */
	ConstType 			number (10) not null,		/* ��� ��������� */
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


/* End   - 4657 - ��������� - paluh - 29.01.2007 */


/* Start - 5255 - ��_0001_�� ������ - ������ 6.8.2 - mik-a-el - 6.02.2007 */

update PUMPREGISTRY
set COMMENTS = '�������������� ������ �� �� ������: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07 - 32.10, 33.00 - 33.03, 34.00 - 34.02'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 5255 - ��_0001_�� ������ - ������ 6.8.2 - mik-a-el - 6.02.2007 */


/* Start -  - ���������� ���������� �������� - paluh - 06.02.2007 */

insert into RegisteredUIModules (ID, Name, Description) values (120, 'GlobalConstsViewObj', '���������');

commit;

/* End   -  - ��������� - paluh - 06.02.2007 */


/* Start - 5025 - ���_0003_1-�� - ������������� ������� - mik-a-el - 12.02.2007 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData StageInitialState="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData StageInitialState="InQueue" Comment="����������� ������������� ���� ������ �� �������� ���������������."/>
	<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form1NMPump';

commit;

/* End   - 5025 - ���_0003_1-�� - ������������� ������� - mik-a-el - 12.02.2007 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (20, '2.3.0', SYSDATE, SYSDATE, '');

commit;
