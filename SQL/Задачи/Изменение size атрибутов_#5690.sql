/* Start - #5690 - Изменение size атрибутов - shelpanov - 03.06.2013 */

print('Удаление констрейнов')
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

print N'Отключение триггеров'
GO

disable trigger t_f_F_GosZadanie_SourceID_lc on [DV].f_F_GosZadanie;
disable trigger t_f_F_GosZadanie_aa on [DV].f_F_GosZadanie;

print N'Добавление временных полей'
GO

alter table [DV].f_F_GosZadanie add CenaEdT numeric(20,2) NULL;

GO

print N'Заполнение временных полей'
GO

update [DV].f_F_GosZadanie set CenaEdT = CenaEd;

GO

print N'Удаление старых полей'
GO

alter table [DV].f_F_GosZadanie drop column CenaEd;

print N'Добавление полей'
GO

alter table [DV].f_F_GosZadanie add CenaEd numeric(20,2);

print N'Заполнение полей'
GO

update [DV].f_F_GosZadanie set CenaEd = CenaEdT;

GO

print N'Удаление констрейнов'
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

print N'Удаление временных полей'
GO

alter table [DV].f_F_GosZadanie drop column CenaEdT;

print N'Обновление метаданных'
GO

UPDATE [DV].MetaObjects SET Configuration = '<?xml version="1.0" encoding="windows-1251"?>
			<DatabaseConfiguration>
			  <DataTable objectKey="5e89ca1c-f287-414d-b216-0e7d14369079" semantic="F" name="GosZadanie" caption="Госзадание" description="Госзадание." takeMethod="ВВОД" dataSourceKinds="ОРГАНИЗАЦИИ\0001;ОРГАНИЗАЦИИ\0002;ОРГАНИЗАЦИИ\0003;ОРГАНИЗАЦИИ\0004;ОРГАНИЗАЦИИ\0006;ОРГАНИЗАЦИИ\0007;ОРГАНИЗАЦИИ\0008;ОРГАНИЗАЦИИ\0009;ОРГАНИЗАЦИИ\0010;ОРГАНИЗАЦИИ\0011;ОРГАНИЗАЦИИ\0012;ФК\0003">
			  <Attributes>
				<Attribute objectKey="9de0597d-5464-42b7-b1ce-0fbad20bbb77" name="CenaEd" caption="Средневзвешенная цена за ед" type="ftDouble" size="20" scale="2" nullable="true" lookupType="1" position="5" />
				<Attribute objectKey="dff8a9dc-d09a-440e-8a0e-ad0a22cf73a7" name="RazdelN" caption="Раздел" type="ftInteger" size="10" default="0" lookupType="0" position="5" />
				</Attributes></DataTable>
			</DatabaseConfiguration>'
WHERE objectkey = '5e89ca1c-f287-414d-b216-0e7d14369079';
GO

print N'Включение триггеров'
GO

enable trigger t_f_F_GosZadanie_SourceID_lc on [DV].f_F_GosZadanie;
enable trigger t_f_F_GosZadanie_aa on [DV].f_F_GosZadanie;

/* End - #5690 - Изменение size атрибутов - shelpanov - 03.06.2013 */


