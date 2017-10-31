/* Start - #5690 - ��������� size ��������� - shelpanov - 03.06.2013 */

print('�������� �����������')
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_F_GosZadanie')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_F_GosZadanie')
    and name = 'CenaEd'
    )

set @sql = N'alter table DV.f_F_GosZadanie drop constraint ' + @default

exec sp_executesql @sql

go

print N'���������� ���������'
GO

disable trigger t_f_F_GosZadanie_SourceID_lc on [DV].f_F_GosZadanie;
disable trigger t_f_F_GosZadanie_aa on [DV].f_F_GosZadanie;

print N'���������� ��������� �����'
GO

alter table [DV].f_F_GosZadanie add CenaEdT numeric(20,2) NULL;

GO

print N'���������� ��������� �����'
GO

update [DV].f_F_GosZadanie set CenaEdT = CenaEd;

GO

print N'�������� ������ �����'
GO

alter table [DV].f_F_GosZadanie drop column CenaEd;

print N'���������� �����'
GO

alter table [DV].f_F_GosZadanie add CenaEd numeric(20,2);

print N'���������� �����'
GO

update [DV].f_F_GosZadanie set CenaEd = CenaEdT;

GO

print N'�������� �����������'
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_F_GosZadanie')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_F_GosZadanie')
    and name = 'CenaEdT'
    )

set @sql = N'alter table DV.f_F_GosZadanie drop constraint ' + @default

exec sp_executesql @sql

GO

print N'�������� ��������� �����'
GO

alter table [DV].f_F_GosZadanie drop column CenaEdT;

print N'���������� ����������'
GO

UPDATE [DV].MetaObjects SET Configuration = '<?xml version="1.0" encoding="windows-1251"?>
			<DatabaseConfiguration>
			  <DataTable objectKey="5e89ca1c-f287-414d-b216-0e7d14369079" semantic="F" name="GosZadanie" caption="����������" description="����������." takeMethod="����" dataSourceKinds="�����������\0001;�����������\0002;�����������\0003;�����������\0004;�����������\0006;�����������\0007;�����������\0008;�����������\0009;�����������\0010;�����������\0011;�����������\0012;��\0003">
			  <Attributes>
				<Attribute objectKey="9de0597d-5464-42b7-b1ce-0fbad20bbb77" name="CenaEd" caption="���������������� ���� �� ��" type="ftDouble" size="20" scale="2" nullable="true" lookupType="1" position="5" />
				<Attribute objectKey="dff8a9dc-d09a-440e-8a0e-ad0a22cf73a7" name="RazdelN" caption="������" type="ftInteger" size="10" default="0" lookupType="0" position="5" />
				</Attributes></DataTable>
			</DatabaseConfiguration>'
WHERE objectkey = '5e89ca1c-f287-414d-b216-0e7d14369079';
GO

print N'��������� ���������'
GO

enable trigger t_f_F_GosZadanie_SourceID_lc on [DV].f_F_GosZadanie;
enable trigger t_f_F_GosZadanie_aa on [DV].f_F_GosZadanie;

/* End - #5690 - ��������� size ��������� - shelpanov - 03.06.2013 */


