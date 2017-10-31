/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.11 � ��������� ������ 2.1.12
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 3226 - ���_0003_1�� - ������� - mik-a-el - 24.07.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('���', 0003, 'Form1NMPump', '������� ������� ����� 1��',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'����� � ���������� �������, ������ � ���� ������������ �������� � ��������� ������� ��, ������ ����������� ��������� ������ �� 19 ������ 2004 �. N ���-3-10/108@  "�� ����������� ���� �������������� ��������� ���������� ����������� ��������� ������ �� 2005 ���".',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="������� �������� �� �����������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

commit;

/* End   - 3226 - ���_0003_1�� - ������� - mik-a-el - 24.07.2006 */

/* Start - 3296 - ���_0008_1� - mik-a-el - 28.07.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('���', 0007, 'Form1NApp7Pump', '������� ������� ����� 1�, ���������� 7',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'������ ������������ ������������ �� 22 ����� 2005 �. � 1� "�� ����������� ������� ��������� ������������ ���������� �������� ��������� ���������� ��������� � ������� �������� ���������������� �������� ������������ ������������". ������� ��������� �� �������� ������������ (�����������) �� ����� �������� ���������� � 7 � 1�',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="������� �������� �� �����������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('���', 0008, 'Form1NDPPump', '������� ������� ����� 1�, ������ ������������� �����������',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'������ ������������ ������������ �� 22 ����� 2005 �. � 1� "�� ����������� ������� ��������� ������������ ���������� �������� ��������� ���������� ��������� � ������� �������� ���������������� �������� ������������ ������������". ������ ������������� �����������',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="������� �������� �� �����������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

commit;

/* End   - 3296 - ���_0008_1� - mik-a-el - 28.07.2006 */



/* Start -  - ��������� ��������� ���������� ������� - gbelov - 29.07.2006 */

alter table MetaPackages
	add PrivatePath varchar2 (1000);

update MetaPackages set PrivatePath = 'Packages\��_0001_�� ������\��_0001_�� ������.xml' where RefParent is null and BuiltIn = 0 and Name = '��_0001_�� ������';
update MetaPackages set PrivatePath = 'Packages\' || Name || '.xml' where PrivatePath is null and RefParent is null and BuiltIn = 0;
update MetaPackages set PrivatePath = Name || '.xml' where RefParent is not null and BuiltIn = 0;

commit;

/* End -  - ��������� ��������� ���������� ������� - gbelov - 29.07.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (17, '2.1.12', SYSDATE, SYSDATE, '');

commit;
