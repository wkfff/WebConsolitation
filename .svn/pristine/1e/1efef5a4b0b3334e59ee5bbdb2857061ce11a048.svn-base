/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Message.sql - скрипт на создание таблиц для работы с сообщениями
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Сообщения
:!!echo ================================================================================

/* Таблица с прикрепленными файлами */
create table MessageAttachment(
	Id                             int NOT NULL,
	DocumentName                   nvarchar (255) NULL,          /* Наименование документа */
	DocumentFileName               nvarchar (255) NULL,          /* Наименование файла с расширением */
	Document                       varbinary(max) NULL,          /* Документ */
constraint PKMessageAttachment primary key (ID) 
);

go

create table g.MessageAttachment(
	ID int identity(1,1) not null
) on [primary]
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

/* таблица с сообщениями */
create table Message(
	Id                            int not null,
	Subject                       nvarchar(255) null,              /* Тема сообщения .Нефиг создавать длинные сообщения */
	DateTimeOfCreation            datetime not null,               /* Дата создания сообщения */
	DateTimeOfActual              datetime not null,               /* Дата, до которой актуально сообщение */
	MessageType                   int not null,                    /* Тип сообщения: от администратора, от системы и т д */
	MessageImportance             int not null,                    /* важность сообщения */
	MessageStatus                 int not null,                    /* статус сообщения: прочитано/непрочитано/удалено */
	RefUserSender                 int null,                        /* Отправитель */
	RefUserRecipient              int not null,                    /* Получатель */
	RefMessageAttachment          int null,                        /* Вложение */
	TransferLink                  nvarchar (1000) null,            /* Тег для перехода*/
 constraint PK_Messages primary key (ID) 
);
go

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

create trigger t_Message_d ON [Message] instead of delete as
begin
	set nocount on;
	delete from Message where ID in (select ID from deleted)

	declare @c int;
	
	select @c = COUNT(*) from Message where RefMessageAttachment = (select RefMessageAttachment from deleted)
		and ID <> (select ID from deleted)

	if @c = 0 
	begin
		delete from MessageAttachment where ID in (select RefMessageAttachment from deleted)
	end;
end;
go

-- индексы 

create nonclustered index [I_FK_RefUserSender] on [Message] 
(
	[RefUserSender] asc
)
go

create nonclustered index [I_FK_RefUserRecipient] on [Message] 
(
	[RefUserRecipient] asc
)
go

create nonclustered index [I_FK_RefMessageAttachment] on [Message] 
(
	[RefMessageAttachment] asc
)
go

-- констрены

alter table [Message] add  constraint [DF_Messages_DateTimeOfCreation]  default (getdate()) FOR [DateTimeOfCreation]
go
alter table [Message] add  constraint [DF_Messages_DateTimeOfActual]  default (dateadd(minute, 10, getdate())) for [DateTimeOfActual]
go

alter table [Message]  with check add  constraint [FK_UserSender] foreign key([RefUserSender])
references [Users] ([Id])
go
alter table [Message] check constraint [FK_UserSender]
go

alter table [Message]  with check add  constraint [FK_UserRecipient] foreign key([RefUserRecipient])
references [Users] ([Id])
go
alter table [Message] check constraint [FK_UserRecipient]
go

alter table [Message]  with check add constraint [FK_MessageAttachment] foreign key([RefMessageAttachment])
references [MessageAttachment] ([Id])
go
alter table [Message] check constraint [FK_MessageAttachment]
go