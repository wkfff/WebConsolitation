/*******************************************************************
 ��������� ���� Oracle �� ������ 2.5 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - 4661 - ���1 - ���������� ����� �������� ������ - shahov - 03.06.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ����������� ���� ������ �� ���������� ���������� ������� � ��������� ���� ����������, ����������� �������� ���������������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
					<CheckData StageInitialState="InQueue" Comment="����������� ��������� �������� ��, �����, ����� �� ������������� ��������� � ������ �� ����������� ���������."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (69, '2.5.0.13', To_Date('03.06.2009', 'dd.mm.yyyy'), SYSDATE, '���1 - ���������� ����� �������� ������', 0);

commit;

/* End - 4661 - ���1 - ���������� ����� �������� ������ - shahov - 03.06.2009 */
