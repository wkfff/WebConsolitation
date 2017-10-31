/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - 8881 - ����� 0003 - �� ����� ��������� ����������� ��������� ������������� - feanor - 23.07.2008 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ������������ ������� ���������� �������������� ''�������.�����_������ ��������''."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (52, '2.4.1.4', CONVERT(datetime, '2008.07.23', 102), GETDATE(), '����� 0003 - �� ����� ��������� ����������� ��������� �������������', 0);

go

/* End - 8881 - ����� 0003 - �� ����� ��������� ����������� ��������� ������������� - feanor - 23.07.2008 */


