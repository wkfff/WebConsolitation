/*******************************************************************
 ��������� ���� SqlServer �� ������  � ��������� ������
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - 7855 - �������� ���������� ������ � ������� - avolkov - 19.05.2008 */

/* ��������� � ������� ���� - ������� ��������.*/
alter table HUB_DataSources
   add Deleted NUMERIC(1) default 0 not null;

go

/* ��������� ��� �� ���� � �������������. */
drop view DataSources;

go

create view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted,
	SupplierCode + ' ' + DataName + cast((
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' +
			CASE KindsOfParams
				WHEN 0 THEN Name + ' ' + cast(Year as varchar(4))
				WHEN 1 THEN cast(Year as varchar(4))
				WHEN 2 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2))
				WHEN 3 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2)) + ' ' + Variant
				WHEN 4 THEN cast(Year as varchar(4)) + ' ' + Variant
				WHEN 5 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1))
				WHEN 6 THEN cast(Year as varchar(4)) + ' ' + Territory
				WHEN 7 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1)) + ' ' + cast(Month as varchar(2))
				WHEN 9 THEN Variant
			END
		END) as varchar)
from HUB_DataSources;

go

/* ��������� ���� ������� �������� ��������� */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1018, 13, '�������� ��������� ������' );

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (46, '2.4.0.12', CONVERT(datetime, '2008.05.19', 102), GETDATE(), '���������� ���� - �������� �������� ��������� � ������� HUB_DataSources � ������������� DataSources; ���������� ������� �������� ���������.', 0);

go

/* End - 7855 - �������� ���������� ������ � ������� - avolkov - 19.05.2008 */



