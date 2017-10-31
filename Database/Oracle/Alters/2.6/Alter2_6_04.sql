/* Start - 11715 � 11813 - �� 35 - �������� ���� ��������� - shahov - 26.10.2009 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
               <DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
					<ProcessData StageInitialState="InQueue" Comment="����������� ���������� ������� � ������� ������."/>
					<AssociateData StageInitialState="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
					<ProcessCube StageInitialState="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������, ����������� ������ �����."/>
			   </DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FO35Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (75, '2.6.0.4', To_Date('26.10.2009', 'dd.mm.yyyy'), SYSDATE, '�� 35 - �������� ���� ���������', 0);

commit;

/* End - 11715 � 11813 - �� 35 - �������� ���� ��������� - shahov - 26.10.2009 */
