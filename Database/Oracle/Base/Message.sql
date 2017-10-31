/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Message.sql - скрипт на создание таблиц для работы с сообщениями
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Сообщения
pro ================================================================================

/* Таблица с прикрепленными файлами */
create table MessageAttachment
(
	ID                       number(10) default null not null,          
	Document                 blob DEFAULT null not null,                     /* Документ */
	DocumentName             varchar2(255) default null not null,            /* Наименование документа */
	DocumentFileName         varchar2(255) default null not null,            /* Наименование файла с расширением */
	constraint PKMessageAttachment primary key ( ID )
);

create sequence g_MessageAttachment start with 1 increment by 1;

create or replace trigger T_MessageAttachment_BI before insert on MessageAttachment for each row
begin
if :new.ID is null then select g_MessageAttachment.NextVal into :new.ID from Dual; end if;
end;
/

/* таблица с сообщениями */
create table Message
(
	ID                       number(10) default null not null,
	Subject                  varchar2(255) default null null,                 /* Тема сообщения .Нефиг создавать длинные сообщения */
	DateTimeOfCreation       date default null not null,                      /* Дата создания сообщения */
	DateTimeOfActual         date default null not null,                      /* Дата, до которой актуально сообщение */
	MessageType              number(2) default null not null,                 /* Тип сообщения: от администратора, от системы и т д */
	MessageImportance        number(2) default null not null,                 /* важность сообщения */
	MessageStatus            number(2) default null not null,                 /* статус сообщения: прочитано/непрочитано/удалено */
	RefUserSender            number(10) default null null,                    /* Отправитель */
	RefUserRecipient         number(10) default null not null,                /* Получатель */
	TransferLink             varchar2 (1000) null,                            /* Тег для перехода*/
	RefMessageAttachment     number(10) default null null,                    /* Вложение */
	constraint PKMessage primary key ( ID )
);

create sequence g_Message start with 1 increment by 1;

create or replace trigger T_Message_BI before insert on Message for each row
begin
  if :new.ID is null then select g_Message.NextVal into :new.ID from Dual; end if;
  if :new.DateTimeOfCreation is null then select SYSDATE into :new.DateTimeOfCreation from Dual; end if;
  if :new.DateTimeOfActual is null then select SYSDATE + 10/1440 into :new.DateTimeOfActual  from Dual; end if;
end;
/

create or replace trigger T_Message_BU before update on Message for each row
begin
  if :new.DateTimeOfCreation is null then select SYSDATE into :new.DateTimeOfCreation from Dual; end if;
  if :new.DateTimeOfActual is null then select SYSDATE + 10/1440 into :new.DateTimeOfActual from Dual; end if;
end;
/

/* Поскольку у нас один аттачмент для группы сообщений, не удаляем его пока есть хотябы одно письмо, ссылающееся на него*/
create or replace package delete_messages_pkg 
as 
    type array is table of message%rowtype index by binary_integer; 
      
     oldvals array; 
     empty array;
end; 
/

create or replace trigger T_Message_bd 
before delete on Message 
begin 
    delete_messages_pkg.oldvals := delete_messages_pkg.empty; 
end; 
/ 

create or replace trigger T_Message_bdfer 
before delete on Message 
for each row 
declare 
   i    number default delete_messages_pkg.oldvals.count+1; 
begin 
   delete_messages_pkg.oldvals(i).RefMessageAttachment := :old.RefMessageAttachment; 
   delete_messages_pkg.oldvals(i).id := :old.id; 
end; 
/ 

create or replace trigger T_Message_AD after delete on Message
begin
  declare c NUMBER;
  begin
    for i in 1 .. delete_messages_pkg.oldvals.count loop
        select COUNT(*) into c from Message where RefMessageAttachment = delete_messages_pkg.oldvals(i).RefMessageAttachment
               and ID <> delete_messages_pkg.oldvals(i).id;
        if c = 0 then
           delete from MessageAttachment where id = delete_messages_pkg.oldvals(i).refmessageattachment;
        end if;
     end loop;
  end;
END;
/

-- ограничения
alter table Message add constraint FK_MessageAttachment foreign key ( RefMessageAttachment ) references MessageAttachment ( ID );

alter table Message add constraint FK_UserSender foreign key ( RefUserSender ) references Users ( ID );
alter table Message add constraint FK_UserRecipient foreign key ( RefUserRecipient ) references Users ( ID );

-- индексы
create index I_FK_RefUserSender on Message ( RefUserSender ) tablespace dvindx compute statistics;
create index I_FK_RefUserRecipient on Message ( RefUserRecipient ) tablespace dvindx compute statistics;
create index I_FK_RefMessageAttachment on Message ( RefMessageAttachment ) tablespace dvindx compute statistics;

commit;
