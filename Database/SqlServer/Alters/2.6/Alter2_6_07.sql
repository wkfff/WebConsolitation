/********************************************************************
	Переводит базу Oracle из версии 2.5 в следующую версию 2.6 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Start - 12575 - Процедура удаления констрейнов, перед удалением самой таблицы - tsvetkov - 18.01.2010 */

create  procedure utility$removeRelationships
(
 @table_schema  sysname = 'DV',
 @parent_table_name sysname = '%',
 @child_table_name sysname = '%',
      --to another
 @constraint_name sysname = '%'
) as
 begin
 set nocount on
 declare @statements cursor
 set @statements = cursor static for
 select  'alter table ' + quotename(ctu.table_schema) + '.' + quotename(ctu.table_name) +
         ' drop constraint ' + quotename(cc.constraint_name)
 from  information_schema.referential_constraints as cc
          join information_schema.constraint_table_usage as ctu
           on cc.constraint_catalog = ctu.constraint_catalog
              and cc.constraint_schema = ctu.constraint_schema
              and cc.constraint_name = ctu.constraint_name
 where   ctu.table_schema = @table_schema 
   and ctu.table_name like @child_table_name
   and cc.constraint_name like @constraint_name
   and   exists (select *
   from information_schema.constraint_table_usage ctu2
   where cc.unique_constraint_catalog = ctu2.constraint_catalog
      and  cc.unique_constraint_schema = ctu2.constraint_schema
      and  cc.unique_constraint_name = ctu2.constraint_name
      and ctu2.table_schema = @table_schema
      and ctu2.table_name like @parent_table_name)
 open @statements
 declare @statement nvarchar(1000)
 While  (1=1)
  begin
         fetch from @statements into @statement
                if @@fetch_status <> 0
                 break
                exec (@statement)
         end
 end
go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (78, '2.6.0.7', CONVERT(datetime, '2010.01.18', 102), GETDATE(), 'Процедура удаления констрейнов, перед удалением самой таблицы', 0);

go

/* End - 12575 - Процедура удаления констрейнов, перед удалением самой таблицы - tsvetkov - 18.01.2010 */
