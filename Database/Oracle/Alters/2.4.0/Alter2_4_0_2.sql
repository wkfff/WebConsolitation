/*******************************************************************
 ��������� ���� Oracle �� ������ 2.X.X � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - 7431 - ��� 11 - ����� �� ��������� ��� ��, ��������� ����������� � ����� ��������� - feanor - 1.02.2008 */

whenever SQLError continue commit;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ���� ''��� ����������'' ����� � �������������� ������� ������� �����."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form1NApp7DayPump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (36, '2.4.0.2', To_Date('01.02.2008', 'dd.mm.yyyy'), SYSDATE, '��� 11 - ����� �� ��������� ��� ��, ��������� ����������� � ����� ���������', 0);

commit;

whenever SQLError exit rollback;

/* End - 7431 - ��� 11 - ����� �� ��������� ��� ��, ��������� ����������� � ����� ��������� - feanor - 1.02.2008 */
