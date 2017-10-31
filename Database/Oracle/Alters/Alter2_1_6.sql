/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.6 � ��������� ������ 2.1.7
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */



/* Start - 2347 - ������������ ��� ������� - ������� - 23.03.2006 */

insert into Users (ID, Name, UserType, Blocked)
values (3, '������� ������', 0, 1);

commit;

/* End - 2347 - ������������ ��� ������� - ������� - 23.03.2006 */



/* Start - 2292 - ������ ���������  "���������� ������������ ��������" - ������� - 24.03.2006 */

whenever SQLError continue commit;

insert into RegisteredUIModules
(ID, Name, Description)
values
(5, 'MDObjectsManagementUI', '���������� ������������ ��������');

whenever SQLError exit rollback;

commit;

/* End - 2292 - ������ ���������  "���������� ������������ ��������" - ������� - 24.03.2006 */



/* Start -  - ����������� �� ���������, �������� ��������� � HUB_DataSources - gbelov - 29.03.2006 */

-- ���
alter table HUB_DataSources
	add Year number (4);

-- ��������
alter table HUB_DataSources
	add Name varchar2 (255);

-- �����
alter table HUB_DataSources
	add Month number (2);

-- �������
alter table HUB_DataSources
	add Variant varchar2 (255);

-- �������
alter table HUB_DataSources
	add Quarter number (1);

-- ����������
alter table HUB_DataSources
	add Territory varchar2 (255);

-- ��������� ������ �� ��������� � HUB_DataSources */
declare
	tmpYear number (4);
	tmpName varchar2 (255);
	tmpMonth number (2);
	tmpVariant varchar2 (255);
	tmpQuarter number (1);
begin
	for cr in (select ID, KindsOfParams from HUB_DataSources) loop
		if cr.KindsOfParams = 0 then
			begin
				select Year, Name into tmpYear, tmpName from SAT_DataSources_Budget where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear, Name = tmpName
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;

		elsif cr.KindsOfParams = 1 then
			begin
				select Year into tmpYear from SAT_DataSources_Y where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;

		elsif cr.KindsOfParams = 2 then
			begin
				select Year, Month into tmpYear, tmpMonth from SAT_DataSources_YM where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear, Month = tmpMonth
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;

		elsif cr.KindsOfParams = 3 then
			begin
				select Year, Month, Variant into tmpYear, tmpMonth, tmpVariant from SAT_DataSources_YMV where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear, Month = tmpMonth, Variant = tmpVariant
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;

		elsif cr.KindsOfParams = 4 then
			begin
				select Year, Variant into tmpYear, tmpVariant from SAT_DataSources_YV where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear, Variant = tmpVariant
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;

		elsif cr.KindsOfParams = 5 then
			begin
				select Year, Quarter into tmpYear, tmpQuarter from SAT_DataSources_YQ where ID = cr.ID;
				update HUB_DataSources
					set Year = tmpYear, Quarter = tmpQuarter
					where ID = cr.ID;
			exception
				when No_Data_Found then
					null;
			end;
		end if;
	end loop;
end;
/

-- ������� ��������
drop table SAT_DataSources_Budget;
drop table SAT_DataSources_Y;
drop table SAT_DataSources_YM;
drop table SAT_DataSources_YMV;
drop table SAT_DataSources_YV;
drop table SAT_DataSources_YQ;

-- ������� ������������� ������������ ��������
drop view v_DataSources;

-- ����������� �������������
create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
	SupplierCode ||' '|| DataName ||' - '|| CASE KindsOfParams WHEN 0 THEN Name || ' ' || Year WHEN 1 THEN cast(Year as varchar(4)) WHEN 2 THEN Year || ' ' || Month  WHEN 3 THEN Year || ' ' || Month || ' ' || Variant WHEN 4 THEN Year || ' ' || Variant WHEN 5 THEN Year || ' ' || Quarter END
from HUB_DataSources;

commit work;

call DBMS_UTILITY.COMPILE_SCHEMA ('DV');

/* End -  - ����������� �� ���������, �������� ��������� � HUB_DataSources - gbelov - 29.03.2006 */



/* Start - ��� - � ������� TasksTemp ����������� ���������� �� �������� - borisov - 31.03.06 16:00 */

alter table TasksTemp add constraint FKTasksTempRefCurator foreign key ( Curator )
		references Users ( ID );

commit;

/* End - ��� - � ������� TasksTemp ����������� ���������� �� �������� - borisov - 31.03.06 16:00 */



/* Start - 2410 - ���������������� �������� �� ��������� ���� - borisov - 03.04.06 20.00 */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40307, 0, '������� ������ ����������� ��������');

commit;

/* End - 2410 - ���������������� �������� �� ��������� ���� - borisov - 03.04.06 20.00 */



insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (12, '2.1.7', SYSDATE, SYSDATE, '');

commit;
