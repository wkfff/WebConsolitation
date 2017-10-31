/*******************************************************************
 ��������� ���� Oracle �� ������ 2.4.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - 8521 - ��� �� 4 �� - ��������� ����������� � ����� ��������� - feanor - 26.01.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ��������� �������� � �������������� ��������������.��ѻ � ��������� ���� ������ �� ������ ���������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNSRF1Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (63, '2.5.0.7', CONVERT(datetime, '2009.01.26', 102), GETDATE(), '��� �� - 4 �� - ��������� ����������� � ����� ���������', 0);

go

/* End - 8521 - ��� �� 4 �� - ��������� ����������� � ����� ��������� - feanor - 26.01.2009 */
