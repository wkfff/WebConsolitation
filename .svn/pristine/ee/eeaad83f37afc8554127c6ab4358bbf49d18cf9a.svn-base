/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY - vPetrov - 29.08.2011  */

ALTER TABLE PUMPHISTORY ALTER COLUMN PROGRAMVERSION NVARCHAR(20) NOT NULL;

insert into DV.DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (101, '3.0.0.6', CONVERT(datetime, '2011.06.29', 102), GETDATE(), 'Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY', 0);

go

/* End - Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY - vPetrov - 29.08.2011 */
