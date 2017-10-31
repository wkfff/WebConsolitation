/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
:on error exit
/* End   - Стандартная часть */ 


/* Start - Исправление а трггере каскадного удаления для MetaPackages - tsvetkov - 07.04.2010 */

print ('Исправление а трггере каскадного удаления для MetaPackages')

print('ALTER TRIGGER [DV].[t_MetaPackages_d] ON [DV].[MetaPackages] INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	-- Каскадное удаление
	delete MetaObjects where RefPackages in (select ID from deleted)
	delete MetaDocuments where RefPackages in (select ID from deleted)
    -- удаляем объекты у вложенных пакетов
	delete MetaObjects where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	delete MetaDocuments where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	-- удаляем вложенные пакеты
	delete MetaPackages where RefParent in (select ID from deleted)
	delete MetaPackages where ID in (select ID from deleted)
END')

go

ALTER TRIGGER [DV].[t_MetaPackages_d] ON [DV].[MetaPackages] INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	-- Каскадное удаление
	delete MetaObjects where RefPackages in (select ID from deleted)
	delete MetaDocuments where RefPackages in (select ID from deleted)
    -- удаляем объекты у вложенных пакетов
	delete MetaObjects where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	delete MetaDocuments where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	-- удаляем вложенные пакеты
	delete MetaPackages where RefParent in (select ID from deleted)
	delete MetaPackages where ID in (select ID from deleted)
END

go

print ('Триггер изменен')

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (84, '2.7.0.4', CONVERT(datetime, '2010.04.07', 102), GETDATE(), 'Исправление а трггере каскадного удаления для MetaPackages', 0);

go

/* End - Исправление а трггере каскадного удаления для MetaPackages - tsvetkov - 07.04.2010 */
