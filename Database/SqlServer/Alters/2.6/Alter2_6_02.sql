/* Start - 11471 - ��� 19 - ��������� ����������� � ����� ��������� - feanor - 11.09.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ��������� �������� � ���������� ���� ����_����� � �������������� ��������.��ʻ."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK19Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (73, '2.6.0.2', CONVERT(datetime, '2009.09.11', 102), GETDATE(), '��� 19 - ��������� ����������� � ����� ���������', 0);

go

/* End - 11471 - ��� 19 - ��������� ����������� � ����� ��������� - feanor - 11.09.2009 */
