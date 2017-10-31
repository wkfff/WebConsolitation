/********************************************************************
	��������� ���� SQL Server �� ������ 2.7 � ��������� ������ 2.8 
********************************************************************/

/* Start - 13711 - ��5 - �������� ���� ������������� - shahov - 20.05.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'EO5Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (86, '2.7.0.7', To_Date('20.05.2010', 'dd.mm.yyyy'), SYSDATE, '��5 - �������� ���� �������������', 0);

commit;

/* End - 13711 - ��5 - �������� ���� ������������� - shahov - 20.05.2010 */
