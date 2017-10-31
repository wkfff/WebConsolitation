/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #18344 - Добавление подсистемы обмена сообщениями - tsvetkov - 01.02.2012 */

CREATE TABLE MessageAttachment
(
	ID NUMBER(10) DEFAULT NULL NOT NULL,
	Document BLOB DEFAULT NULL NOT NULL,
	DocumentName VARCHAR2(255) DEFAULT NULL NOT NULL,
	DocumentFileName VARCHAR2(255) DEFAULT NULL NOT NULL,
	CONSTRAINT PKMessageAttachment PRIMARY KEY ( ID )
);

CREATE SEQUENCE g_MessageAttachment START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER T_MessageAttachment_BI BEFORE INSERT ON MessageAttachment FOR EACH ROW
BEGIN
if :new.ID is null then select g_MessageAttachment.NextVal into :new.ID from Dual; end if;

END;
/


CREATE TABLE Message
(
	ID NUMBER(10) DEFAULT NULL NOT NULL,
	Subject VARCHAR2(255) DEFAULT NULL NULL,
	DateTimeOfCreation DATE DEFAULT NULL NOT NULL,
	DateTimeOfActual DATE DEFAULT NULL NOT NULL,
	MessageType NUMBER(2) DEFAULT NULL NOT NULL,
	MessageImportance NUMBER(2) DEFAULT NULL NOT NULL,
	MessageStatus NUMBER(2) DEFAULT NULL NOT NULL,
	RefUserSender NUMBER(10) DEFAULT NULL NULL,
	RefUserRecipient NUMBER(10) DEFAULT NULL NOT NULL,
	TransferLink VARCHAR2 (1000) NULL,
	RefMessageAttachment NUMBER(10) DEFAULT NULL NULL,
	CONSTRAINT PKMessage PRIMARY KEY ( ID )
);

CREATE SEQUENCE g_Message START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER T_Message_BI BEFORE INSERT ON Message FOR EACH ROW
BEGIN
  if :new.ID is null then select g_Message.NextVal into :new.ID from Dual; end if;
  if :new.DateTimeOfCreation is null then select SYSDATE into :new.DateTimeOfCreation from Dual; end if;
  if :new.DateTimeOfActual is null then select SYSDATE + 10/1440 into :new.DateTimeOfActual  from Dual; end if;
END;
/

CREATE OR REPLACE TRIGGER T_Message_BU BEFORE UPDATE ON Message FOR EACH ROW
BEGIN
  if :new.DateTimeOfCreation is null then select SYSDATE into :new.DateTimeOfCreation from Dual; end if;
  if :new.DateTimeOfActual is null then select SYSDATE + 10/1440 into :new.DateTimeOfActual from Dual; end if;
END;
/

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


CREATE OR REPLACE TRIGGER T_Message_AD AFTER DELETE ON Message
BEGIN
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
ALTER TABLE Message ADD CONSTRAINT FK_MessageAttachment FOREIGN KEY ( RefMessageAttachment ) REFERENCES MessageAttachment ( ID );

ALTER TABLE Message ADD CONSTRAINT FK_UserSender FOREIGN KEY ( RefUserSender ) REFERENCES Users ( ID );
ALTER TABLE Message ADD CONSTRAINT FK_UserRecipient FOREIGN KEY ( RefUserRecipient ) REFERENCES Users ( ID );

-- индексы
CREATE  INDEX I_FK_RefUserSender ON Message ( RefUserSender )  TABLESPACE DVINDX COMPUTE STATISTICS;
CREATE  INDEX I_FK_RefUserRecipient ON Message ( RefUserRecipient )  TABLESPACE DVINDX COMPUTE STATISTICS;
CREATE  INDEX I_FK_RefMessageAttachment ON Message ( RefMessageAttachment )  TABLESPACE DVINDX COMPUTE STATISTICS;

-- добавление интерфейса
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (270, 'MessagesUI', 'Подсистема сообщений', 'Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation, Krista.FM.Client.ViewObjects.MessagesUI');
commit;

-- Протоколы
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1201, 0, 'Создание нового сообщения от администратора');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1202, 0, 'Удаление сообщения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1203, 0, 'Очистка неактуальных сообщений');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1204, 0, 'Создание нового сообщения от интерфейса расчета кубов');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1210, 0, 'Создание нового сообщения(прочее)');

commit;

create table SAT_MessageExchangeOperations
(
  ID          number (10) not null,
  constraint PKSAT_MessageOperations primary key ( ID ),
  constraint FKSAT_MessageOperations foreign key ( ID )
    references HUB_EventProtocol (ID) on delete cascade
);

/* И представление для них */
create or replace view MessageExchangeOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid
from HUB_EventProtocol HUB join SAT_MessageExchangeOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_MessageExchangeOperations_I instead of insert on MessageExchangeOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_MessageExchangeOperations (ID)
	values (NewID);
end T_MessageExchangeOperations_I;
/

create or replace trigger t_MessageExchangeOperations_u instead of update on MessageExchangeOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MessageExchangeOperations_u;
/

create or replace trigger t_MessageExchangeOperations_d instead of delete on MessageExchangeOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MessageExchangeOperations_d;
/

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (118, '3.0.0.23', To_Date('01.02.2012', 'dd.mm.yyyy'), SYSDATE, 'Добавлена подсистема обиена сообщениями"', 0);

commit;

/* End - #18344 - Добавление подсистемы обмена сообщениями - tsvetkov - 01.02.2012 */

