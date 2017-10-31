/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.2 � ��������� ������ 2.1.3
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 1506 - ������ ������� �� ���������� - kutnyashenko - 06.02.2006 */

alter table PUMPREGISTRY drop column PUMPDATE;

alter table PUMPREGISTRY drop column JOBNAME;

alter table PUMPREGISTRY drop column ISRUNNING;

alter table PUMPREGISTRY drop column LOCALMACHINENAME;

alter table PUMPREGISTRY add SCHEDULE CLOB;

commit;

update PUMPREGISTRY set SCHEDULE =
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>';

commit;

/* End - 1506 - ������ ������� �� ���������� - kutnyashenko - 06.02.2006 */



/* Start - 2028 - ���������� ���. �������������� "������.�����" �� 98,99 ����� - gbelov - 08.02.2006 */

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearDay (ID, Year, HalfYear, Quarter, Month, MonthName, Day) values (-1, 0, ' ', ' ', 0, ' ', 0);

	for tmpYear in 1998..1999 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := '������� �� ������ ����';
      elsif tmpMonth > 6 then
         tmpHalfYear := '��������� 2';
      else
         tmpHalfYear := '��������� 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := '������� �� ������ ����';
      elsif tmpMonth < 4 then
         tmpQuarter := '������� 1';
      elsif tmpMonth < 7 then
         tmpQuarter := '������� 2';
      elsif tmpMonth < 10 then
         tmpQuarter := '������� 3';
      else
         tmpQuarter := '������� 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 31
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then '������� �� ������ ����'
			when 1 then '������'
			when 2 then '�������'
			when 3 then '����'
			when 4 then '������'
			when 5 then '���'
			when 6 then '����'
			when 7 then '����'
			when 8 then '������'
			when 9 then '��������'
			when 10 then '�������'
			when 11 then '������'
			when 12 then '�������'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

/*        	insert into HUB_DataCls (ID, TypeID)
        	values (tmpNewDate, 1);
			insert into SAT_DataCls (ID, TypeID, Code1, Code2, Code3, Name1, Name2, Name3)
		    values (tmpNewDate, 1, tmpYear, tmpMonth, tmpDay, tmpHalfYear, tmpQuarter, tmpMonthName);*/
      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

create or replace procedure sp_FillDateMonth as
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarterNo pls_integer;
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearMonth (ID, Year, HalfYear, Quarter, Month, MonthName) values (-1, 0, ' ', ' ', 0, ' ');

  for tmpYear in 1998..1999 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := '������� �� ������ ����';
      elsif tmpMonth > 6 then
         tmpHalfYear := '��������� 2';
      else
         tmpHalfYear := '��������� 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarterNo := 0;
         tmpQuarter := '������� �� ������ ����';
      elsif tmpMonth < 4 then
         tmpQuarterNo := 1;
         tmpQuarter := '������� 1';
      elsif tmpMonth < 7 then
         tmpQuarterNo := 2;
         tmpQuarter := '������� 2';
      elsif tmpMonth < 10 then
         tmpQuarterNo := 3;
         tmpQuarter := '������� 3';
      else
         tmpQuarterNo := 4;
         tmpQuarter := '������� 4';
      end if;

	  tmpMonthName := case tmpMonth
			when 0 then '������� �� ������ ����'
			when 1 then '������'
			when 2 then '�������'
			when 3 then '����'
			when 4 then '������'
			when 5 then '���'
			when 6 then '����'
			when 7 then '����'
			when 8 then '������'
			when 9 then '��������'
			when 10 then '�������'
			when 11 then '������'
			when 12 then '�������'
			else '0'
      end;

	  if tmpMonth in (1, 4, 7, 10)  then

        tmpNewDate := tmpYear * 10000 + 9990 + tmpQuarterNo;

        insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, '������� ' || tmpQuarterNo, 90 + tmpQuarterNo, '������� ' || tmpQuarterNo);

	  end if;

      tmpNewDate := tmpYear * 10000 + tmpMonth * 100;

      insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName);

	end loop;

  end loop;

end sp_FillDateMonth;
/

exec sp_FillDateMonth;

create or replace procedure sp_FillDateYear as
begin
    --insert into fx_Date_Year (ID, Year) values (-1, 0);
	for tmpYear in 1998..1999 loop
        insert into fx_Date_Year (ID, DateYear) values (tmpYear, tmpYear);
	end loop;
end sp_FillDateYear;
/

exec sp_FillDateYear;

commit;

/* End - 2028 - ���������� ���. �������������� "������.�����" �� 98,99 ����� - gbelov - 08.02.2006 */

/* Begin - 2059 - ���������������� �������� ���������� ����������������� - borisov - 09.02.2006 */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40301, 0, '������� ������ �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40302, 0, '������� ������ �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40303, 0, '������� ���������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40304, 0, '������� ���������� �����������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40305, 0, '������� ���������� ����� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40306, 0, '�������� �������� � �������');
/* End - 2059 - ���������������� �������� ���������� ����������������� - borisov - 09.02.2006 */

/* Begin - 2058 - �������� ������ �������� ��������� � ���� - borisov - 10.02.2006 */
create table RegisteredUIModules
(
		ID					number (10) not null,			 /* PK */
		Name				varchar2 (255) not null,		 /* ��� ������� ��������� */
		Description			varchar2 (500),		 /* �������� ������� ��������� */
		constraint PKRegisteredUIModules primary key ( ID ), /* ID ������ ���� ���������� */
		constraint UKRegisteredUIModules unique ( Name ) /* ��� ������ ���� ���������� */
);

insert into RegisteredUIModules (ID, Name, Description) values (0,   'AdministrationUI', 		'��������� �����������������');
insert into RegisteredUIModules (ID, Name, Description) values (10,  'ProtocolsViewObject', 	'���������');
insert into RegisteredUIModules (ID, Name, Description) values (20,  'FixedClsUI',	 			'��������� ������������� ���������������');
insert into RegisteredUIModules (ID, Name, Description) values (30,  'DataClsUI', 				'��������� ��������������� ������');
insert into RegisteredUIModules (ID, Name, Description) values (40,  'AssociatedClsUI', 		'��������� ������������ ���������������');
insert into RegisteredUIModules (ID, Name, Description) values (50,  'AssociationUI', 			'��������� ������������� ���������������');
insert into RegisteredUIModules (ID, Name, Description) values (60,  'TranslationsTablesUI', 	'��������� ������ �������������');
insert into RegisteredUIModules (ID, Name, Description) values (70,  'DataPumpUI', 				'��������� ������� ������');
insert into RegisteredUIModules (ID, Name, Description) values (80,  'DataSourcesUI', 			'��������� ���������� ������');
insert into RegisteredUIModules (ID, Name, Description) values (90,  'DisintRulesUI', 			'��������� ���������� ���������� �������');
insert into RegisteredUIModules (ID, Name, Description) values (100, 'TasksViewObj', 			'��������� �����');
insert into RegisteredUIModules (ID, Name, Description) values (110, 'FactTablesUI', 			'��������� ������ ������');
insert into RegisteredUIModules (ID, Name, Description) values (120, 'AdminConsole', 			'��������� ���������� ������');

commit;

/* End - 2058 - �������� ������ �������� ��������� � ���� - borisov - 10.02.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (8, '2.1.3', SYSDATE, SYSDATE, '');

commit;






