/********************************************************************
	��������� ���� SQL Server �� ������ 2.7 � ��������� ������ 2.8 
********************************************************************/

/* Start - 13213 - ���20 - �������� ���� ��������� - shahov - 09.04.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ��������� ������ �� ������ ������� � �������������� �������� �������.��ʻ"/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
					<CheckData StageInitialState="InQueue" Comment="����������� ������ ���� �� �������� ������������ � �������, ���������� � ������������."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK20Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (85, '2.7.0.5', To_Date('09.04.2010', 'dd.mm.yyyy'), SYSDATE, '���20 - �������� ���� ���������', 0);

commit;

/* End - 13213 - ���20 - �������� ���� ��������� - shahov - 09.04.2010 */
