/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #18344 - Добавление подсистемы обмена сообщениями - tsvetkov - 01.02.2012 */

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'DV' AND  TABLE_NAME = 'MessageAttachment'))
BEGIN
	alter table Message drop constraint FK_MessageAttachment
    drop table DV.MessageAttachment
END
go

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'g' AND  TABLE_NAME = 'MessageAttachment'))
BEGIN
    drop table g.MessageAttachment
END
go

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'DV' AND  TABLE_NAME = 'Message'))
BEGIN
    drop table DV.Message
END
go

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'g' AND  TABLE_NAME = 'Message'))
BEGIN
    drop table g.Message
END
go

-- Таблица с прикрепленными файлами
create table [MessageAttachment](
	[Id] [int] NOT NULL,
	[DocumentName] [nvarchar] (255) NULL,
	[DocumentFileName] [nvarchar] (255) NULL,
	[Document] [varbinary](max) NULL 
CONSTRAINT [PKMessageAttachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
go

CREATE TABLE [g].[MessageAttachment](
	[ID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
go

create trigger t_MessageAttachment_BI on MessageAttachment instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into MessageAttachment (ID, DocumentName, DocumentFileName, Document)
		select ID, DocumentName, DocumentFileName, Document from inserted;
	else if @nullsCount = 1
	begin
		insert into g.MessageAttachment default values;
		delete from g.MessageAttachment where ID = @@IDENTITY;
		insert into MessageAttachment (ID, DocumentName, DocumentFileName, Document)
			select @@IDENTITY, DocumentName, DocumentFileName, Document from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;
go

-- таблица с сообщениями
create table [Message](
	[Id] [int] NOT NULL,
	[Subject] [nvarchar](255) NULL,              -- нефиг создавать длинные сообщения
	[DateTimeOfCreation] [datetime] NOT NULL,
	[DateTimeOfActual] [datetime] NOT NULL,
	[MessageType] [int] NOT NULL,                -- тип сообщения: от администратора, от системы
	[MessageImportance] [int] NOT NULL,          -- важность сообщения
	[MessageStatus] [int] NOT NULL,              -- статус сообщения: прочитано или нет
	[RefUserSender] [int] NULL,
	[RefUserRecipient] [int] NOT NULL,
	[RefMessageAttachment] [int] NULL,
	[TransferLink] [nvarchar] (1000) NULL
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- генератор 
create table g.Message ( ID int identity not null );
go

create trigger t_Messages_BI on Message instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Message (ID, Subject, DateTimeOfCreation, DateTimeOfActual, MessageType, MessageImportance, MessageStatus, RefUserSender, RefUserRecipient, RefMessageAttachment, TransferLink)
		select ID, Subject, DateTimeOfCreation, DateTimeOfActual, MessageType, MessageImportance, MessageStatus, RefUserSender, RefUserRecipient, RefMessageAttachment, TransferLink from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Message default values;
		delete from g.Message where ID = @@IDENTITY;
		insert into Message (ID, Subject, DateTimeOfCreation, DateTimeOfActual, MessageType, MessageImportance, MessageStatus, RefUserSender, RefUserRecipient, RefMessageAttachment, TransferLink)
			select @@IDENTITY, Subject, DateTimeOfCreation, DateTimeOfActual, MessageType, MessageImportance, MessageStatus, RefUserSender, RefUserRecipient, RefMessageAttachment, TransferLink from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;
go

CREATE TRIGGER t_Message_d ON [Message] INSTEAD OF DELETE AS
BEGIN
	ALTER TRIGGER [DV].[t_Message_d] ON [DV].[Message] INSTEAD OF DELETE AS
BEGIN
	SET NOCOUNT ON;
	delete from Message where ID in (select ID from deleted)

	declare @c int;
	
	select @c = COUNT(*) from Message where RefMessageAttachment = (select RefMessageAttachment from deleted)
		and ID <> (select ID from deleted)

	if @c = 0 
	begin
		delete from MessageAttachment where ID in (select RefMessageAttachment from deleted)
	end;
END;
END;
go


-- индексы 


CREATE NONCLUSTERED INDEX [I_FK_RefUserSender] ON [Message] 
(
	[RefUserSender] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [I_FK_RefUserRecipient] ON [Message] 
(
	[RefUserRecipient] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [I_FK_RefMessageAttachment] ON [Message] 
(
	[RefMessageAttachment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- констрены

ALTER TABLE [Message] ADD  CONSTRAINT [DF_Messages_DateTimeOfCreation]  DEFAULT (getdate()) FOR [DateTimeOfCreation]
GO
ALTER TABLE [Message] ADD  CONSTRAINT [DF_Messages_DateTimeOfActual]  DEFAULT (DATEADD(minute, 10, getdate())) FOR [DateTimeOfActual]
GO

ALTER TABLE [Message]  WITH CHECK ADD  CONSTRAINT [FK_UserSender] FOREIGN KEY([RefUserSender])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [Message] CHECK CONSTRAINT [FK_UserSender]
GO

ALTER TABLE [Message]  WITH CHECK ADD  CONSTRAINT [FK_UserRecipient] FOREIGN KEY([RefUserRecipient])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [Message] CHECK CONSTRAINT [FK_UserRecipient]
GO

ALTER TABLE [Message]  WITH CHECK ADD  CONSTRAINT [FK_MessageAttachment] FOREIGN KEY([RefMessageAttachment])
REFERENCES [MessageAttachment] ([Id])
GO
ALTER TABLE [Message] CHECK CONSTRAINT [FK_MessageAttachment]
GO

-- Добавление интерфейса

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (270, 'MessagesUI', 'Подсистема сообщений', 'Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation, Krista.FM.Client.ViewObjects.MessagesUI');
go

-- Протоколы
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1201, 0, 'Создание нового сообщения от администратора');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1202, 0, 'Удаление сообщения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1203, 0, 'Очистка неактуальных сообщений');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1204, 0, 'Создание нового сообщения от интерфейса расчета кубов');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1210, 0, 'Создание нового сообщения(прочее)');
go

/* Протокол операций над функцией перевода базы на новый год */
create table SAT_MessageExchangeOperations
(
	ID	int not null,
	constraint PKSAT_MessageExchangeOperations primary key ( ID ),
	constraint FKSAT_MessageExchangeOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);
go

/* И представление для них */
create view MessageExchangeOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid
from HUB_EventProtocol HUB join SAT_MessageExchangeOperations SAT on (HUB.ID = SAT.ID);
go

CREATE TRIGGER T_MessageExchangeOperations_I on MessageExchangeOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MessageExchangeOperations (ID)
			select ID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, RefUsersOperations, InfoMessage, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MessageExchangeOperations (ID)
			select @@IDENTITY from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;
go

create trigger t_MessageExchangeOperations_u on MessageExchangeOperations instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_MessageExchangeOperations_d on MessageExchangeOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (118, '3.0.0.23', CONVERT(datetime, '2012.01.29', 102), GETDATE(), 'Добавлена подсистема обиена сообщениями"', 0);
go

/* End - #18344 - Добавление подсистемы обмена сообщениями - tsvetkov - 01.02.2012 */


