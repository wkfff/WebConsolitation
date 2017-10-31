/********************************************************************
	��������� ���� SQL Server �� ������ 2.7 � ��������� ������ 2.8 
********************************************************************/

/* Start - 15072 - ���22 - �������� ���� ��������� - shahov - 09.11.2010 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ��������� ������ � �������������� �������� �������.��ʻ �� ������������� ��������������.������ �������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK22Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (90, '2.7.0.11', CONVERT(datetime, '2010.11.09', 102), GETDATE(), '���22 - �������� ���� ���������', 0);

go

/* End - 15072 - ���22 - �������� ���� ��������� - shahov - 09.11.2010 */
