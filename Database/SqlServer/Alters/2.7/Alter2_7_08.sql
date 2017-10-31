


/* Start - 14136 - �� 1 - �������� ���� �������� - vpetrov - 28.06.2010 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ������������� ����������� �� ������, ����������� �������� ���������������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
					<CheckData StageInitialState="InQueue" Comment="�������� ���������� ����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (87, '2.7.0.8', CONVERT(datetime, '2010.06.28', 102), GETDATE(), '��1 - �������� ���� ��������', 0);

go

/* End - 14136 - �� 1 - �������� ���� �������� - vpetrov - 28.06.2010 */
