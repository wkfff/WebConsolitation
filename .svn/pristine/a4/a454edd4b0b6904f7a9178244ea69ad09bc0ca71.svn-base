/*******************************************************************
 Переводит базу Oracle из версии 2.1.5 в следующую версию 2.1.6
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 2103 - Идентификация пользователя по паролю - borisov - 16.03.06 15:00 */

alter table Actions disable constraint FKactionsRefUsers;
alter table Memberships disable constraint FKmembershipsRefUsers;
alter table Notifications disable constraint FKnotificationsRefUsers;
alter table Permissions disable constraint FKpermissionsRefUsers;
alter table Tasks disable constraint FKtasksrefcurator;
alter table Tasks disable constraint FKtasksrefdoer;
alter table Tasks disable constraint FKtasksreflockbyuser;
alter table Tasks disable constraint FKtasksrefowner;
alter table TasksTemp disable constraint FKTasksTemprefdoer;
alter table TasksTemp disable constraint FKTasksTemprefowner;

update Users set ID = ID + 100 where ID > 0;
update Users set DnsName = Name where ID > 0;

update Actions set RefUsers = RefUsers + 100 where RefUsers > 0;
update Memberships set RefUsers = RefUsers + 100 where RefUsers > 0;
update Notifications set RefUsers = RefUsers + 100 where RefUsers > 0;
update Permissions set RefUsers = RefUsers + 100 where RefUsers > 0;

update Tasks set doer = doer + 100 where doer > 0;
update Tasks set owner = owner + 100 where owner > 0;
update Tasks set curator = curator + 100 where curator > 0;
update Tasks set lockbyuser = lockbyuser + 100 where lockbyuser > 0;

update TasksTemp set doer = doer + 100 where doer > 0;
update TasksTemp set owner = owner + 100 where owner > 0;
update TasksTemp set curator = curator + 100 where curator > 0;
update TasksTemp set lockbyuser = lockbyuser + 100 where lockbyuser > 0;

alter table actions enable constraint FKactionsRefUsers;
alter table memberships enable constraint FKmembershipsRefUsers;
alter table notifications enable constraint FKnotificationsRefUsers;
alter table permissions enable constraint FKpermissionsRefUsers;
alter table tasks enable constraint FKtasksrefcurator;
alter table tasks enable constraint FKtasksrefdoer;
alter table tasks enable constraint FKtasksreflockbyuser;
alter table tasks enable constraint FKtasksrefowner;
alter table TasksTemp enable constraint FKTasksTemprefdoer;
alter table TasksTemp enable constraint FKTasksTemprefowner;

-- Резервируем диапазон в 100 значений под фиксированных пользователей

rem Настройки отображения/протоколирования
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

spool RecreateSequence_g_Users.sql;

prompt drop sequence g_Users;;
select 'create sequence g_Users start with ' || case (select count(ID) from Users) when 0 then 1 else (select max(ID) + 1 from Users) end || ';' from Dual;
prompt

spool off;

set echo on;
set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 200;
set PageSize 20;
set SQLPrompt "SQL>";

@RecreateSequence_g_Users.sql;

--drop sequence G_USERS;
--create sequence G_USERS start with 100;

-- Создаем поле под хэш пароля
alter table users add PwdHashSHA varchar2 (255);

/* Создаем фиксированных пользователей */
-- Пользователь не определен
update Users set Name = 'Пользователь не определен', Blocked = 1 where ID = 0;

-- Фиксированный администратор
insert into Users (ID, Name, Description, UserType, Blocked, PwdHashSha)
values (1, 'FMADMIN', 'Администратор по умолчанию', 0, 0,
'LHP69eGnG4GqrXmVPUsBDPc90eNW/KA9zz/0we8babte9c9DMo+mY1NM1XwkmM67fUUZASVCywIC3Q8hEps+kg==');

-- System (для закачек по расписанию)
insert into Users (ID, Name, Description, UserType, Blocked)
values (2, 'SYSTEM', '', 0, 0);

commit;

/* End - 2103 - Идентификация пользователя по паролю - borisov - 16.03.06 15:00 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (11, '2.1.6', SYSDATE, SYSDATE, '');

commit;

