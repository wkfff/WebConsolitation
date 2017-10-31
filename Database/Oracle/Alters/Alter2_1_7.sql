/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.7 � ��������� ������ 2.1.8
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 2456 - ���� JOB � ������� TASKS �� ������ ���� nullable - ������� - 11.04.2006 */

update Tasks set Job = '�� �������' where Job is null;
alter table Tasks modify Job not null;

commit;

/* End - 2456 - ���� JOB � ������� TASKS �� ������ ���� nullable - ������� - 11.04.2006 */



/* Start - 2527 - ������������� ������.���� - �������������� ������� - gbelov - 18.04.2006 */

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearDay (ID, Year, HalfYear, Quarter, Month, MonthName, Day) values (-1, 0, ' ', ' ', 0, ' ', 0);

	for tmpYear in 1998..2010 loop

  	for tmpMonth in 13..13 loop

      if tmpMonth = 0 then
         tmpHalfYear := '������� �� ������ ����';
      elsif tmpMonth = 13 then
         tmpHalfYear := '�������������� �������';
      elsif tmpMonth > 6 then
         tmpHalfYear := '��������� 2';
      else
         tmpHalfYear := '��������� 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := '������� �� ������ ����';
      elsif tmpMonth = 13 then
         tmpQuarter := '�������������� �������';
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
			when 13 then '�������������� �������'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

commit;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 1998..2010 loop

  	for tmpMonth in 0..13 loop

      if tmpMonth = 0 then
         tmpHalfYear := '������� �� ������ ����';
      elsif tmpMonth = 13 then
         tmpHalfYear := '�������������� �������';
      elsif tmpMonth > 6 then
         tmpHalfYear := '��������� 2';
      else
         tmpHalfYear := '��������� 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := '������� �� ������ ����';
      elsif tmpMonth = 13 then
         tmpQuarter := '�������������� �������';
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
			when 13 then '�������������� �������'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

/* End - 2527 - ������������� ������.���� - �������������� ������� - gbelov - 18.04.2006 */


/* Start - NoNumber - ��������� ������� ������� 33.00 - mik-a-el - 11.05.2006 */

update PUMPREGISTRY set
     COMMENTS = '�������������� ������ �� �� ������: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

/* End - NoNumber - ��������� ������� ������� 33.00 - mik-a-el - 11.05.2006 */


/* Start - 2685 - ���������� � ��������� ���������� ����� ����� � �������������� ������" - paluh - 17.05.2006 */

-- �������������� �����
alter table DisintRules_KD
	rename column CONSMO_PERCENT to CONSMR_PERCENT;

alter table DisintRules_Ex
	rename column CONSMO_PERCENT to CONSMR_PERCENT;

alter table DisintRules_Ex
	rename column MO_PERCENT to MR_PERCENT;

alter table DisintRules_KD
	rename column MO_PERCENT to MR_PERCENT;

-- ���������� �����
alter table DisintRules_KD
	add CONSMO_Percent number (5,2) default 0 not null;

alter table DisintRules_Ex
	add CONSMO_Percent number (5,2) default 0 not null;

alter table DisintRules_KD
	add GO_Percent number (5,2) default 0 not null;

alter table DisintRules_Ex
	add GO_Percent number (5,2) default 0 not null;

commit;

/* End   - 2685 - ���������� � ��������� ���������� ����� ����� � �������������� ������" - paluh - 17.05.2006 */



/* Start - 2715 - ��������� ������ �������� - ���������� ���� - paluh - 22.05.2006 */

-- ���������� ����. �������� �������� ����, ��� ��������� ���� ����������� ������ ���� ������ ������������� � ����������� ��������

update DisintRules_KD
	set CONSMO_Percent = CONSMR_Percent;

update DisintRules_Ex
	set CONSMO_Percent = CONSMR_Percent;

commit;

/* End   - 2715 - ��������� ������ �������� - ���������� ���� - paluh - 22.05.2006 */



/* Start -  - ��������� �������� ������ - mik-a-el - 22.05.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
		<ProcessData State="InQueue" Comment="����������� ������������� ���� ������ �� �������� ���������������."/>
		<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
		<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
		<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump' or	PROGRAMIDENTIFIER = 'SKIFYearRepPump' or PROGRAMIDENTIFIER = 'FKMonthRepPump';

commit;

/* End -  - ��������� �������� ������ - mik-a-el - 22.05.2006 */



/* Start -  - ��������� ��������� �������� - gbelov - 22.05.2006 */

alter table MetaObjects
	drop constraint FKMetaObjectsRefPackages;

alter table MetaObjects
	add constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade;

alter table MetaLinks
	drop constraint FKMetaLinksRefPackages;

alter table MetaLinks
	add constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade;

/* ������ �� ���������� � ������� ��������� ������� ������������� */
alter table MetaConversionTable
	add RefAssociation number (10);

alter table MetaConversionTable
	add constraint FKMetaCTRefAssociation foreign key ( RefAssociation )
		references MetaLinks ( ID ) on delete cascade;

declare
	LinkID pls_integer;
begin
	-- ��� ������ ������� ������������� �� ��������� ����� Semantic � Association
	-- ���� ID ��������������� ���������� � ���������� ��� � RefAssociation
	for CT in (select ID, Association from MetaConversionTable)
	loop
		begin
			select ID into LinkID from MetaLinks where ('a.' || Semantic || '.' || Name) = CT.Association and Class = 2;
			update MetaConversionTable set RefAssociation = LinkID where ID = CT.ID;
		exception
			when No_Data_Found then
				null;
		end;
	end loop;
end;
/

alter table MetaConversionTable
	modify RefAssociation not null;

alter table MetaConversionTable
	drop column Association;

alter table MetaConversionTable
	drop column Semantic;

/* End -  - ��������� ��������� �������� - gbelov - 22.05.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (13, '2.1.8', SYSDATE, SYSDATE, '');

commit;
