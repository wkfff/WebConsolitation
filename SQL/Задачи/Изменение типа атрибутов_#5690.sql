/* Start - #5690 - Меняем тип атрибута - shelpanov - 27.05.2013 */

print('Удаление констрейнов')
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_ResultWork_Staff')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_ResultWork_Staff')
    and name = 'BeginYear'
    )

set @sql = N'alter table DV.f_ResultWork_Staff drop constraint ' + @default

exec sp_executesql @sql

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_ResultWork_Staff')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_ResultWork_Staff')
    and name = 'EndYear'
    )

set @sql = N'alter table DV.f_ResultWork_Staff drop constraint ' + @default

exec sp_executesql @sql

go

print N'Отключение триггеров'
GO

disable trigger t_f_ResultWork_Staff_Source_lc on [DV].f_ResultWork_Staff;
disable trigger t_f_ResultWork_Staff_aa on [DV].f_ResultWork_Staff;

print N'Добавление временных полей'
GO

alter table [DV].f_ResultWork_Staff add BeginYearT numeric(20,2) NULL;
alter table [DV].f_ResultWork_Staff add EndYearT numeric(20,2) NULL;

GO

print N'Заполнение временных полей'
GO

update [DV].f_ResultWork_Staff set BeginYearT = BeginYear;
update [DV].f_ResultWork_Staff set EndYearT = EndYear;
GO

print N'Удаление старых полей'
GO

alter table [DV].f_ResultWork_Staff drop column BeginYear;
alter table [DV].f_ResultWork_Staff drop column EndYear;

print N'Добавление полей'
GO

alter table [DV].f_ResultWork_Staff add BeginYear numeric(20,2);
alter table [DV].f_ResultWork_Staff add EndYear numeric(20,2);

print N'Заполнение полей'
GO

update [DV].f_ResultWork_Staff set BeginYear = BeginYearT;
update [DV].f_ResultWork_Staff set EndYear = EndYearT;
GO

print N'Удаление констрейнов'
GO

declare @default sysname, @sql nvarchar(max)

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_ResultWork_Staff')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_ResultWork_Staff')
    and name = 'BeginYearT'
    )

set @sql = N'alter table DV.f_ResultWork_Staff drop constraint ' + @default

exec sp_executesql @sql

select @default = name 
from sys.default_constraints 
where parent_object_id = object_id('DV.f_ResultWork_Staff')
AND type = 'D'
AND parent_column_id = (
    select column_id 
    from sys.columns 
    where object_id = object_id('DV.f_ResultWork_Staff')
    and name = 'EndYearT'
    )

set @sql = N'alter table DV.f_ResultWork_Staff drop constraint ' + @default

exec sp_executesql @sql
GO

print N'Удаление временных полей'
GO

alter table [DV].f_ResultWork_Staff drop column BeginYearT;
alter table [DV].f_ResultWork_Staff drop column EndYearT;

print N'Обновление метаданных'
GO

UPDATE [DV].MetaObjects SET Configuration = '<?xml version="1.0" encoding="windows-1251"?>
		<DatabaseConfiguration>
		<DataTable objectKey="f4c6d82e-97bd-4cb0-b0fe-a3b323ac2095" semantic="ResultWork" name="Staff" caption="Штат сотрудников" takeMethod="ВВОД" 
		dataSourceKinds="ОРГАНИЗАЦИИ\0001;ОРГАНИЗАЦИИ\0002;ОРГАНИЗАЦИИ\0003;ОРГАНИЗАЦИИ\0004;ОРГАНИЗАЦИИ\0006;ОРГАНИЗАЦИИ\0007;ОРГАНИЗАЦИИ\0008;ОРГАНИЗАЦИИ\0009;ОРГАНИЗАЦИИ\0010;ОРГАНИЗАЦИИ\0011;ОРГАНИЗАЦИИ\0012;ФК\0003">
		<Attributes>
			<Attribute objectKey="f817c331-d9e1-44da-9a74-f28ffc7e95ea" name="AvgSalary" caption="Средняя заработная плата" type="ftDouble" size="20" scale="2" lookupType="1" position="1" />
			<Attribute objectKey="0b4b62a9-6fc1-46e5-97f4-35e7387facfa" name="BeginYear" caption="Количество на начало года" type="ftDouble" size="20" scale="2" lookupType="1" position="3" />
			<Attribute objectKey="ae9cf76f-2813-4108-99a8-c7bbb1d366e6" name="EndYear" caption="Количество на конец года" type="ftDouble" size="20" scale="2" lookupType="1" position="4" />
		</Attributes>
		</DataTable>
		</DatabaseConfiguration>'
WHERE objectkey = 'f4c6d82e-97bd-4cb0-b0fe-a3b323ac2095';
GO

print N'Включение триггеров'
GO

enable trigger t_f_ResultWork_Staff_Source_lc on [DV].f_ResultWork_Staff;
enable trigger t_f_ResultWork_Staff_aa on [DV].f_ResultWork_Staff;

/* End - #5690 - Меняем тип атрибута - shelpanov - 27.05.2013 */


