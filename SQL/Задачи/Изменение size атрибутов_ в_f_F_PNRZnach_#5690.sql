/* Start - #5690 - ��������� size ��������� � f_F_PNRZnach - shelpanov - 10.06.2013 */

print('�������� �����������')
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_F_PNRZnach')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_F_PNRZnach')
    and name = 'Source'
    )

set @sql = N'alter table DV.f_F_PNRZnach drop constraint ' + @default

exec sp_executesql @sql

go

print N'���������� ���������'
GO

disable trigger t_f_F_PNRZnach_SourceID_lc on [DV].f_F_PNRZnach;
disable trigger t_f_F_PNRZnach_aa on [DV].f_F_PNRZnach;

print N'���������� ��������� �����'
GO

alter table [DV].f_F_PNRZnach add SourceT varchar(4000) NULL;

GO

print N'���������� ��������� �����'
GO

update [DV].f_F_PNRZnach set SourceT = Source;

GO

print N'�������� ������ �����'
GO

alter table [DV].f_F_PNRZnach drop column Source;

print N'���������� �����'
GO

alter table [DV].f_F_PNRZnach add Source varchar(4000);

print N'���������� �����'
GO

update [DV].f_F_PNRZnach set Source = SourceT;

GO

print N'�������� �����������'
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_F_PNRZnach')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_F_PNRZnach')
    and name = 'SourceT'
    )

set @sql = N'alter table DV.f_F_PNRZnach drop constraint ' + @default

exec sp_executesql @sql

GO

print N'�������� ��������� �����'
GO

alter table [DV].f_F_PNRZnach drop column SourceT;

print N'���������� ����������'
GO

UPDATE [DV].MetaObjects SET Configuration = '<?xml version="1.0" encoding="windows-1251"?>
		<DatabaseConfiguration>
		<DataTable objectKey="2f578037-98dd-4da3-91dd-8b7a3093cfdc" semantic="F" name="PNRZnach" caption="���_��������" description="�������_�������� ���." takeMethod="����" dataSourceKinds="�����������\0001;�����������\0002;�����������\0003;�����������\0004;�����������\0006;�����������\0007;�����������\0008;�����������\0009;�����������\0010;�����������\0011;�����������\0012;��\0003">
		<Attributes>
			<Attribute objectKey="4945f6ed-d2e6-4573-96e9-ace25255f2f2" name="CurrentYear" caption="������� ���" type="ftString" size="200" nullable="true" lookupType="1" position="6" />
			<Attribute objectKey="6f3446a9-592e-40ce-a981-ca6e447e3283" name="ReportingYear" caption="�������� ���" type="ftString" size="200" nullable="true" lookupType="1" position="5" />
			<Attribute objectKey="00a14372-ac50-43a7-8d6f-fa0ec9a161ff" name="Protklp" caption="������� ���������� �� �����" type="ftString" size="500" nullable="true" lookupType="1" position="4" />
			<Attribute objectKey="0cbf2a81-f999-4a43-8357-f8b689659ef5" name="ComingYear" caption="��������� ���" type="ftString" size="200" nullable="true" lookupType="1" position="7" />
			<Attribute objectKey="565b2ca1-4616-4fd1-9e18-731806f74e50" name="FirstPlanYear" caption="������ �������� ���" type="ftString" size="200" nullable="true" lookupType="1" position="8" />
			<Attribute objectKey="90949222-446a-4a37-a04b-72ae6c9f227b" name="SecondPlanYear" caption="������ �������� ���" type="ftString" size="200" nullable="true" lookupType="1" position="9" />
			<Attribute objectKey="17a87a2e-651e-41fa-9627-3200ff6c95eb" name="ActualValue" caption="����������� ��������" type="ftString" size="200" nullable="true" lookupType="1" position="10" />
			<Attribute objectKey="a598371d-5e4d-4b2e-bcde-d65741722b38" name="Info" caption="�������������� ����������" type="ftString" size="200" nullable="true" lookupType="1" position="13" />
			<Attribute objectKey="987ce7ce-427a-4362-a785-49cd1d6813fe" name="Source" caption="�������� ����������" type="ftString" size="4000" nullable="true" lookupType="1" position="14" />
		</Attributes>
		</DataTable>
		</DatabaseConfiguration>'
WHERE objectkey = '2f578037-98dd-4da3-91dd-8b7a3093cfdc';
GO

print N'��������� ���������'
GO

enable trigger t_f_F_PNRZnach_SourceID_lc on [DV].f_F_PNRZnach;
enable trigger t_f_F_PNRZnach_aa on [DV].f_F_PNRZnach;

/* End - #5690 - ��������� size ��������� � f_F_PNRZnach - shelpanov - 10.06.2013 */


