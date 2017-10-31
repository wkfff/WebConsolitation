/*******************************************************************
 Переводит базу Oracle из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/
 
/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 1753 - добавление поля "Тип объекта БД" в протокол "Классификаторы и таблицы" - paluh roman - 26.06.2007 */

whenever SQLError continue commit;

alter table SAT_ClassifiersOperations add ObjectType number (10);

-- создание представления на протокол "Классификаторы"
create or replace view ClassifiersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	Classifier, PumpHistoryID, DataSourceID, ObjectType
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.Classifier, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectType
from HUB_EventProtocol HUB join SAT_ClassifiersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ClassifiersOperations_i instead of insert on ClassifiersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_ClassifiersOperations (ID, Classifier, PumpHistoryID, DataSourceID, ObjectType)
	values (NewID, :new.Classifier, :new.PumpHistoryID, :new.DataSourceID, :new.ObjectType);
end t_ClassifiersOperations_i;
/

create or replace trigger t_ClassifiersOperations_u instead of update on ClassifiersOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_ClassifiersOperations_u;
/

create or replace trigger t_ClassifiersOperations_d instead of delete on ClassifiersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ClassifiersOperations_d;
/

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (22, '2.3.1.1', SYSDATE, SYSDATE, '', 0);

commit;

whenever SQLError exit rollback;

/* End   - 1753 - добавление поля "Тип объекта БД" в протокол "Классификаторы и таблицы" - paluh roman - 26.06.2007 */



/* Start - 6629 - Год Месяц День - 32 число в каждом месяце - gbelov - 04.07.2007 */

whenever SQLError continue commit;

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980132, 0, 1998, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980232, 0, 1998, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980332, 0, 1998, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980432, 0, 1998, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980532, 0, 1998, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980632, 0, 1998, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980732, 0, 1998, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980832, 0, 1998, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19980932, 0, 1998, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19981032, 0, 1998, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19981132, 0, 1998, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990132, 0, 1999, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990232, 0, 1999, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990332, 0, 1999, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990432, 0, 1999, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990532, 0, 1999, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990632, 0, 1999, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990732, 0, 1999, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990832, 0, 1999, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19990932, 0, 1999, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19991032, 0, 1999, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (19991132, 0, 1999, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000132, 0, 2000, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000232, 0, 2000, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000332, 0, 2000, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000432, 0, 2000, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000532, 0, 2000, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000632, 0, 2000, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000732, 0, 2000, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000832, 0, 2000, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20000932, 0, 2000, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20001032, 0, 2000, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20001132, 0, 2000, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010132, 0, 2001, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010232, 0, 2001, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010332, 0, 2001, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010432, 0, 2001, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010532, 0, 2001, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010632, 0, 2001, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010732, 0, 2001, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010832, 0, 2001, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20010932, 0, 2001, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20011032, 0, 2001, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20011132, 0, 2001, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020132, 0, 2002, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020232, 0, 2002, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020332, 0, 2002, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020432, 0, 2002, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020532, 0, 2002, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020632, 0, 2002, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020732, 0, 2002, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020832, 0, 2002, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20020932, 0, 2002, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20021032, 0, 2002, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20021132, 0, 2002, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030132, 0, 2003, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030232, 0, 2003, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030332, 0, 2003, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030432, 0, 2003, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030532, 0, 2003, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030632, 0, 2003, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030732, 0, 2003, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030832, 0, 2003, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20030932, 0, 2003, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20031032, 0, 2003, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20031132, 0, 2003, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040132, 0, 2004, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040232, 0, 2004, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040332, 0, 2004, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040432, 0, 2004, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040532, 0, 2004, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040632, 0, 2004, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040732, 0, 2004, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040832, 0, 2004, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20040932, 0, 2004, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20041032, 0, 2004, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20041132, 0, 2004, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050132, 0, 2005, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050232, 0, 2005, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050332, 0, 2005, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050432, 0, 2005, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050532, 0, 2005, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050632, 0, 2005, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050732, 0, 2005, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050832, 0, 2005, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20050932, 0, 2005, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20051032, 0, 2005, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20051132, 0, 2005, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060132, 0, 2006, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060232, 0, 2006, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060332, 0, 2006, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060432, 0, 2006, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060532, 0, 2006, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060632, 0, 2006, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060732, 0, 2006, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060832, 0, 2006, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20060932, 0, 2006, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20061032, 0, 2006, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20061132, 0, 2006, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070132, 0, 2007, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070232, 0, 2007, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070332, 0, 2007, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070432, 0, 2007, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070532, 0, 2007, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070632, 0, 2007, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070732, 0, 2007, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070832, 0, 2007, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20070932, 0, 2007, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20071032, 0, 2007, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20071132, 0, 2007, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080132, 0, 2008, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080232, 0, 2008, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080332, 0, 2008, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080432, 0, 2008, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080532, 0, 2008, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080632, 0, 2008, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080732, 0, 2008, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080832, 0, 2008, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20080932, 0, 2008, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20081032, 0, 2008, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20081132, 0, 2008, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090132, 0, 2009, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090232, 0, 2009, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090332, 0, 2009, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090432, 0, 2009, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090532, 0, 2009, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090632, 0, 2009, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090732, 0, 2009, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090832, 0, 2009, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20090932, 0, 2009, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20091032, 0, 2009, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20091132, 0, 2009, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100132, 0, 2010, 'Полугодие 1', 'Квартал 1', 01, 'Январь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100232, 0, 2010, 'Полугодие 1', 'Квартал 1', 02, 'Февраль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100332, 0, 2010, 'Полугодие 1', 'Квартал 1', 03, 'Март', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100432, 0, 2010, 'Полугодие 1', 'Квартал 2', 04, 'Апрель', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100532, 0, 2010, 'Полугодие 1', 'Квартал 2', 05, 'Май', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100632, 0, 2010, 'Полугодие 1', 'Квартал 2', 06, 'Июнь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100732, 0, 2010, 'Полугодие 2', 'Квартал 3', 07, 'Июль', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100832, 0, 2010, 'Полугодие 2', 'Квартал 3', 08, 'Август', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20100932, 0, 2010, 'Полугодие 2', 'Квартал 3', 09, 'Сентябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20101032, 0, 2010, 'Полугодие 2', 'Квартал 4', 10, 'Октябрь', 32);
insert into FX_DATE_YEARDAY (ID, ROWTYPE, DATEYEAR, DATEHALFYEAR, DATEQUARTER, DATEMONTHID, DATEMONTH, DATEDAY)
values (20101132, 0, 2010, 'Полугодие 2', 'Квартал 4', 11, 'Ноябрь', 32);

commit;

whenever SQLError exit rollback;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 1998..2010 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 31
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop

      		/*if (tmpDay = 32) and (tmpMonth = 12) then
      			tmpHalfYear := 'Заключительные обороты';
      			tmpQuarter := 'Заключительные обороты';
      			tmpMonthName := 'Заключительные обороты';
      		end if;*/
      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

			/* Вставляем 32-й день в каждый месяц */
      		if (tmpDay = tmpMaxDay and tmpMonth <> 0) then
	      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + 32;
	            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
	            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);
      		end if;

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

whenever SQLError continue commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (23, '2.3.1.2', SYSDATE, SYSDATE, '', 0);

whenever SQLError exit rollback;

commit;

/* End   - 6629 - Год Месяц День - 32 число в каждом месяце - gbelov - 04.07.2007 */



/* Start - 6630 - Перод - расширение после 2010 года - gbelov - 04.07.2007 */

whenever SQLError continue commit;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 2011..2015 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 31
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop

      		/*if (tmpDay = 32) and (tmpMonth = 12) then
      			tmpHalfYear := 'Заключительные обороты';
      			tmpQuarter := 'Заключительные обороты';
      			tmpMonthName := 'Заключительные обороты';
      		end if;*/

      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

			/* Вставляем 32-й день в каждый месяц */
      		if (tmpDay = tmpMaxDay and tmpMonth <> 0) then
	      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + 32;
	            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
	            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);
      		end if;

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

commit;

create or replace procedure sp_FillDateMonth as
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarterNo pls_integer;
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearMonth (ID, Year, HalfYear, Quarter, Month, MonthName) values (-1, 0, ' ', ' ', 0, ' ');

  for tmpYear in 2011..2015 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarterNo := 0;
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarterNo := 1;
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarterNo := 2;
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarterNo := 3;
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarterNo := 4;
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			else '0'
      end;

	  if tmpMonth in (1, 4, 7, 10)  then

        tmpNewDate := tmpYear * 10000 + 9990 + tmpQuarterNo;

        insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, 'Квартал ' || tmpQuarterNo, 90 + tmpQuarterNo, 'Квартал ' || tmpQuarterNo);

	  end if;

      tmpNewDate := tmpYear * 10000 + tmpMonth * 100;

      insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName);

	end loop;

  end loop;

end sp_FillDateMonth;
/

exec sp_FillDateMonth;

commit;

create or replace procedure sp_FillDateYear as
begin
    --insert into fx_Date_Year (ID, Year) values (-1, 0);
	for tmpYear in 2011..2015 loop
        insert into fx_Date_Year (ID, DateYear) values (tmpYear, tmpYear);
	end loop;
end sp_FillDateYear;
/

exec sp_FillDateYear;

commit;

create or replace procedure sp_FillDateUNV as
tmpNewDate pls_integer;
yearKey pls_integer;
yearBase pls_integer;
yearKeyPCH pls_integer;
halfKey pls_integer;
half1KeyPCH pls_integer;
half2KeyPCH pls_integer;
quarterKey pls_integer;
quarterKeyPCH pls_integer;
quarter1KeyPCH pls_integer;
quarter2KeyPCH pls_integer;
quarter3KeyPCH pls_integer;
quarter4KeyPCH pls_integer;
monthKey pls_integer;
monthBase pls_integer;
monthKeyPCH pls_integer;
monthName varchar2(22);
MaxDaysInMonth pls_integer;
dayKey pls_integer;
fullKey pls_integer;
begin

	halfKey := 1;
	quarterKey := 1;

	for tmpYear in 2011..2015 loop

		yearKey := tmpYear;
		yearBase := yearKey * 10000;
		yearKeyPCH := yearBase + 1;

		-- Здесь вставляем служебные записи

		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name)
		values (yearKeyPCH, 0,
			yearKey, yearKey,
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			yearKeyPCH, yearKey);

		half1KeyPCH := yearBase + 10;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half1KeyPCH, 'Полугодие 1', yearKeyPCH);

		half2KeyPCH := yearBase + 20;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half2KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half2KeyPCH, 'Полугодие 2', yearKeyPCH);

		quarter1KeyPCH := yearBase + 9991;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			1, 'Квартал 1',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 1 * 10 + 20, 'Квартал 1', half1KeyPCH);

		quarter2KeyPCH := yearBase + 9992;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter2KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			2, 'Квартал 2',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 2 * 10 + 20, 'Квартал 2', half1KeyPCH);

		quarter3KeyPCH := yearBase + 9993;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter3KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			3, 'Квартал 3',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 3 * 10 + 20, 'Квартал 3', half2KeyPCH);

		quarter4KeyPCH := yearBase + 9994;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter4KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			4, 'Квартал 4',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 4 * 10 + 20, 'Квартал 4', half2KeyPCH);


	  	for tmpMonth in 1..12 loop

			monthKey := tmpMonth;
			monthBase := monthKey * 100;

			halfKey := case monthKey
				when 1 then 1
				when 2 then 1
				when 3 then 1
				when 4 then 1
				when 5 then 1
				when 6 then 1
				when 7 then 2
				when 8 then 2
				when 9 then 2
				when 10 then 2
				when 11 then 2
				when 12 then 2
				else 2
			end;

			quarterKey := case monthKey
				when 1 then 1
				when 2 then 1
				when 3 then 1
				when 4 then 2
				when 5 then 2
				when 6 then 2
				when 7 then 3
				when 8 then 3
				when 9 then 3
				when 10 then 4
				when 11 then 4
				when 12 then 4
				else 4
			end;

			monthName := case monthKey
				when 1 then 'Январь'
				when 2 then 'Февраль'
				when 3 then 'Март'
				when 4 then 'Апрель'
				when 5 then 'Май'
				when 6 then 'Июнь'
				when 7 then 'Июль'
				when 8 then 'Август'
				when 9 then 'Сентябрь'
				when 10 then 'Октябрь'
				when 11 then 'Ноябрь'
				when 12 then 'Декабрь'
			end;

			monthKeyPCH   := yearBase + monthBase;
			quarterKeyPCH := yearBase + 9990 + quarterKey;

			insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
			values (monthKeyPCH, 0,
				yearKey, yearKey,
				halfKey, 'Полугодие ' || halfKey,
				quarterKey, 'Квартал ' || quarterKey,
				monthKey, monthName,
				-2, 'Данные месяца',
				monthKeyPCH, monthName, quarterKeyPCH);

			MaxDaysInMonth := case monthKey
				when 1 then 31
				when 2 then 29
				when 3 then 31
				when 4 then 30
				when 5 then 31
				when 6 then 30
				when 7 then 31
				when 8 then 31
				when 9 then 30
				when 10 then 31
				when 11 then 30
				when 12 then 31
				else 0
			end;
			if (MOD(yearKey, 4) = 0) and (monthKey = 2) then
				MaxDaysInMonth := 29;
			end if;

			for tmpDay in 1..MaxDaysInMonth loop

				dayKey := tmpDay;
				fullKey := yearBase + monthBase + dayKey;

				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (fullKey, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					dayKey, dayKey,
					fullKey, dayKey, monthKeyPCH);

			end loop;

			if monthKey = 12 then
				-- Заключительные обороты
				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (yearBase + monthBase + 32, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					32, 'Заключительные обороты',
					yearBase + monthBase + 32, 'Заключительные обороты', monthKeyPCH);

			end if;

		end loop;

		-- Остатки на начало года
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (yearBase, 0,
			yearKey, yearKey,
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			yearBase, 'Остатки на начало года', yearKeyPCH);

	end loop;

end sp_FillDateUNV;
/

begin sp_FillDateUNV; end;
/

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (24, '2.3.1.3', SYSDATE, SYSDATE, '', 0);

commit;

whenever SQLError exit rollback;

/* End   - 6630 - Перод - расширение после 2010 года - gbelov - 04.07.2007 */


/* Start - 6682 - Изменения в протоколе многомерных баз - paluh roman - 16.07.2007 */

whenever SQLError continue commit;

alter table SAT_MDProcessing add ObjectIdentifier 	varchar2(255);
alter table SAT_MDProcessing add InvalidateReason 	number (10);
alter table SAT_MDProcessing add OlapObjectType 	number (10);

whenever SQLError exit rollback;

/* Изменения на протокол "Расчет многомерной базы" */
create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, PumpHistoryID, DataSourceID, ObjectIdentifier, InvalidateReason, OlapObjectType
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectIdentifier, SAT.InvalidateReason, SAT.OlapObjectType
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace trigger t_MDProcessing_i instead of insert on MDProcessing
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_MDProcessing (ID, MDObjectName, PumpHistoryID, DataSourceID, ObjectIdentifier, InvalidateReason, OlapObjectType)
	values (NewID, :new.MDObjectName, :new.PumpHistoryID, :new.DataSourceID, :new.ObjectIdentifier, :new.InvalidateReason, :new.OlapObjectType);
end t_MDProcessing_i;
/

create or replace trigger t_MDProcessing_u instead of update on MDProcessing
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MDProcessing_u;
/

create or replace trigger t_MDProcessing_d instead of delete on MDProcessing
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MDProcessing_d;
/

commit;

whenever SQLError continue commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (25, '2.3.1.4', SYSDATE, SYSDATE, '', 0);

whenever SQLError exit rollback;

commit;

/* End   - 6682 - Изменения в протоколе многомерных баз - paluh roman - 16.07.2007 */



/* Start - 6698 - Расширена разрядность полей в протоколах сопоставления. - gbelov - 17.07.2007 */

whenever SQLError continue commit;

alter table SAT_BridgeOperations
	modify BridgeRoleA varchar2 (100);

alter table SAT_BridgeOperations
	modify BridgeRoleB varchar2 (100);

whenever SQLError exit rollback;

alter view BridgeOperations compile;
alter trigger t_BridgeOperations_i compile;
alter trigger t_BridgeOperations_u compile;
alter trigger t_BridgeOperations_d compile;

whenever SQLError continue commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (26, '2.3.1.5', SYSDATE, SYSDATE, '', 0);

whenever SQLError exit rollback;

commit;

/* End   - 6698 - Расширена разрядность полей в протоколах сопоставления. - gbelov - 17.07.2007 */



/* Start - 6103 - Изменение таблиц для автоматического расчета. - tartushkin - 19.07.2007 */

whenever SQLError continue commit;

/* Пакеты. */
alter table Batch add BatchId varchar2 (132);

alter table Batch add constraint UKBatchId unique (BatchId);

/* Содержит записи по объектам многомерной базы */
create table OlapObjects
(
	ID					number (10) not null,			/* PK */
	ObjectType			number (10) not null,			/* Тип объекта:
																база = 0,
																куб = 1,
																группа мер = 2,
																раздел = 3,
																измерение = 4*/
	ObjectId			varchar2 (132) not null,		/* Идентификатор объекта. */
	ObjectName			varchar2 (132) not null,		/* Наименование объекта. */
	ParentId			varchar2 (132),					/* Идентификатор родителя. */
	ParentName			varchar2 (132),					/* Наименование родителя. */
	Used				number (1) default 0 not null,	/* 1 - если объект должен поддерживаться в актуальном состоянии, 0 - нет. */
	NeedProcess			number (1) default 0 not null,	/* 1 - если объект нуждается в расчете, 0 - нет. */
	State				number (10) default 0 not null,	/* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		date,							/* Дата последнего расчета */
	RefBatchId			varchar2 (132),					/* Ссылка на идентификатор пакета */
	ProcessType			number (10) default 0 not null,	/* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. По умолчанию - ProcessFull */
	ProcessResult		varchar2 (255),					/* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	Synchronized		number (1) default 1 not null,	/* 1 - если записи соответсвует объект в многомерной базе, 0 - объекта в многомерной базе нет. */
	constraint PKOlapObjects primary key ( ID )
);

create sequence g_OlapObjects;

create or replace trigger t_OlapObjects_bi before insert on OlapObjects for each row
begin
	if :new.ID is null then select g_OlapObjects.NextVal into :new.ID from Dual; end if;
end t_OlapObjects_bi;
/

drop sequence g_Accumulator;
drop table Accumulator;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (27, '2.3.1.6', To_Date('19.07.2007', 'dd.mm.yyyy'), SYSDATE, '', 0);

whenever SQLError exit rollback;

commit;

/* End - 6103 - Изменение таблиц для автоматического расчета. - tartushkin - 19.07.2007 */



/* Start - 6748 - Автоматический расчет кубов - gbelov - 13.08.2007 */

delete from Batch;
delete from OlapObjects;

alter table SAT_MDProcessing drop column InvalidateReason;
alter table SAT_MDProcessing
	add BatchId varchar2 (132);


create or replace view v_MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, PumpHistoryID, DataSourceID, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectIdentifier,
	CASE SAT.OlapObjectType
		WHEN 0 THEN 'Пакет'
		WHEN 1 THEN 'Куб'
		WHEN 2 THEN 'Группа мер'
		WHEN 3 THEN 'Раздел куба'
		WHEN 4 THEN 'Измерение'
	END,
	SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, PumpHistoryID, DataSourceID, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectIdentifier, SAT.OlapObjectType, SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace trigger t_MDProcessing_i instead of insert on MDProcessing
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_MDProcessing (ID, MDObjectName, PumpHistoryID, DataSourceID, ObjectIdentifier, OlapObjectType, BatchId)
	values (NewID, :new.MDObjectName, :new.PumpHistoryID, :new.DataSourceID, :new.ObjectIdentifier, :new.OlapObjectType, :new.BatchId);
end t_MDProcessing_i;
/

create or replace trigger t_MDProcessing_u instead of update on MDProcessing
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MDProcessing_u;
/

create or replace trigger t_MDProcessing_d instead of delete on MDProcessing
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MDProcessing_d;
/


/* Приоритет */
alter table Batch
	add Priority number (5) not null;

/* Полное имя серверного объекта */
alter table OlapObjects
	add FullName varchar2 (64);

/* Изменяем типы сообщений */
delete from HUB_EventProtocol where ClassOfProtocol = 3;

delete from KindsOfEvents where ID = 311 or ID = 312 or ID = 313;

update KindsOfEvents set Name = 'Начало операции' where ID = 301;
update KindsOfEvents set Name = 'Успешное окончание операции' where ID = 304;
update KindsOfEvents set Name = 'Окончание операции с ошибкой' where ID = 305;
update KindsOfEvents set Name = 'Ошибка в процессе выполнения' where ID = 306;
update KindsOfEvents set Name = 'Критическая ошибка в процессе выполнения' where ID = 307;
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (308, 0, 'Требование на расчет');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (28, '2.3.1.7', To_Date('13.08.2007', 'dd.mm.yyyy'), SYSDATE, '', 0);

commit;

/* End   - 6748 - Автоматический расчет кубов - gbelov - 13.08.2007 */



/* Start - 6821 - Новый "Эксперт" - структура для хранения отчетов в базе - paluh - 22.08.2007 */

create sequence g_Templates;

create table Templates
(
	ID			number (10)  not null,
	ParentID		number (10),
	UIDTemplate		varchar2(36)  not null,
	Type			number (10)  not null,
	Name			varchar2(255) not null,
	Description		varchar2(510),
	Document		blob,
	DocumentName		varchar2(255),
	constraint PKTemplates primary key (ID),
	constraint FKTemplatesRef foreign key (ParentID)
		references Templates (ID) on delete cascade
);

/* триггер на добавление новых отчетов */
create or replace trigger t_Templates_bi before insert on Templates for each row
begin
	if :new.ID is null then select g_Templates.NextVal into :new.ID from Dual;
	end if;
end t_Templates_bi;
/

create or replace trigger t_Templates_AA before insert OR update OR delete on Templates for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'Templates', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'Templates', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'Templates', 5, UserName, SessionID, :old.ID); end if;
end t_Templates_AA;
/

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (29, '2.3.1.8', To_Date('22.08.2007', 'dd.mm.yyyy'), SYSDATE, 'Новый "Эксперт" - структура для хранения отчетов в базе', 0);

commit;

/* End   - 6821 - Новый "Эксперт" - структура для хранения отчетов в базе - paluh - 22.08.2007 */



/* Start - 6748 - Удаление полей SourceID и PumpID из протокола многомерных объектов - gbelov - 04.10.2007 */

alter table SAT_MDProcessing
	drop column DataSourceID;

alter table SAT_MDProcessing
	drop column PumpHistoryID;


create or replace view v_MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier,
	CASE SAT.OlapObjectType
		WHEN 0 THEN 'Пакет'
		WHEN 1 THEN 'Куб'
		WHEN 2 THEN 'Группа мер'
		WHEN 3 THEN 'Раздел куба'
		WHEN 4 THEN 'Измерение'
	END,
	SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier, SAT.OlapObjectType, SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace trigger t_MDProcessing_i instead of insert on MDProcessing
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_MDProcessing (ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId)
	values (NewID, :new.MDObjectName, :new.ObjectIdentifier, :new.OlapObjectType, :new.BatchId);
end t_MDProcessing_i;
/

create or replace trigger t_MDProcessing_u instead of update on MDProcessing
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MDProcessing_u;
/

create or replace trigger t_MDProcessing_d instead of delete on MDProcessing
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MDProcessing_d;
/

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (30, '2.3.1.9', To_Date('04.10.2007', 'dd.mm.yyyy'), SYSDATE, 'Удалены поля SourceID и PumpID из протокола многомерных объектов', 0);

commit;

/* End   - 6748 - Удаление полей SourceID и PumpID из протокола многомерных объектов - gbelov - 04.10.2007 */



/* Start - 6748 - Добавляем поле идентификатора пакета в таблицу PumpHistory - gbelov - 09.10.2007 */

alter table PumpHistory
	add BatchId varchar2 (132);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (31, '2.3.1.10', To_Date('09.10.2007', 'dd.mm.yyyy'), SYSDATE, 'Добавлено поле BatchId в таблицу PumpHistory', 0);

commit;

/* End   - 6748 - Добавляем поле идентификатора пакета в таблицу PumpHistory - gbelov - 09.10.2007 */



/* Start - 7226 - Расширение кода поставщика до 11 символов - gbelov - 19.10.2007 */

alter table PumpRegistry
	modify SupplierCode varchar2 (11);

alter table HUB_DataSources
	modify SupplierCode varchar2 (11);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (32, '2.3.1.11', To_Date('19.10.2007', 'dd.mm.yyyy'), SYSDATE, 'Расширение кода поставщика до 11 символов', 0);

commit;

/* End   - 7226 - Расширение кода поставщика до 11 символов - gbelov - 19.10.2007 */



/* Start - 6630 - расширение классификаторов 'период' до 2020 года  - feanor - 14.11.2007 */

whenever SQLError continue commit;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 2016..2020 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 32
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop

      		--if (tmpDay = 32) and (tmpMonth = 12) then
      		--	tmpHalfYear := 'Заключительные обороты';
      		--	tmpQuarter := 'Заключительные обороты';
      		--	tmpMonthName := 'Заключительные обороты';
      		--end if;

      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

create or replace procedure sp_FillDateMonth as
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarterNo pls_integer;
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearMonth (ID, Year, HalfYear, Quarter, Month, MonthName) values (-1, 0, ' ', ' ', 0, ' ');

  for tmpYear in 2016..2020 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarterNo := 0;
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarterNo := 1;
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarterNo := 2;
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarterNo := 3;
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarterNo := 4;
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			else '0'
      end;

	  if tmpMonth in (1, 4, 7, 10)  then

        tmpNewDate := tmpYear * 10000 + 9990 + tmpQuarterNo;

        insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, 'Квартал ' || tmpQuarterNo, 90 + tmpQuarterNo, 'Квартал ' || tmpQuarterNo);

	  end if;

      tmpNewDate := tmpYear * 10000 + tmpMonth * 100;

      insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName);

	end loop;

  end loop;

end sp_FillDateMonth;
/

exec sp_FillDateMonth;

create or replace procedure sp_FillDateYear as
begin
    --insert into fx_Date_Year (ID, Year) values (-1, 0);
	for tmpYear in 2016..2020 loop
        insert into fx_Date_Year (ID, DateYear) values (tmpYear, tmpYear);
	end loop;
end sp_FillDateYear;
/

exec sp_FillDateYear;

commit;


create or replace procedure sp_FillDateUNV as
tmpNewDate pls_integer;
yearKey pls_integer;
yearBase pls_integer;
yearKeyPCH pls_integer;
halfKey pls_integer;
half1KeyPCH pls_integer;
half2KeyPCH pls_integer;
quarterKey pls_integer;
quarterKeyPCH pls_integer;
quarter1KeyPCH pls_integer;
quarter2KeyPCH pls_integer;
quarter3KeyPCH pls_integer;
quarter4KeyPCH pls_integer;
monthKey pls_integer;
monthBase pls_integer;
monthKeyPCH pls_integer;
monthName varchar2(22);
MaxDaysInMonth pls_integer;
dayKey pls_integer;
fullKey pls_integer;
begin

	halfKey := 1;
	quarterKey := 1;

	for tmpYear in 2016..2020 loop

		yearKey := tmpYear;
		yearBase := yearKey * 10000;
		yearKeyPCH := yearBase + 1;

		-- Здесь вставляем служебные записи

		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name)
		values (yearKeyPCH, 0,
			yearKey, yearKey,
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			-2, 'Данные года',
			yearKeyPCH, yearKey);

		half1KeyPCH := yearBase + 10;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half1KeyPCH, 'Полугодие 1', yearKeyPCH);

		half2KeyPCH := yearBase + 20;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (half2KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			-2, 'Данные полугодия',
			half2KeyPCH, 'Полугодие 2', yearKeyPCH);

		quarter1KeyPCH := yearBase + 9991;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter1KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			1, 'Квартал 1',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 1 * 10 + 20, 'Квартал 1', half1KeyPCH);

		quarter2KeyPCH := yearBase + 9992;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter2KeyPCH, 0,
			yearKey, yearKey,
			1, 'Полугодие 1',
			2, 'Квартал 2',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 2 * 10 + 20, 'Квартал 2', half1KeyPCH);

		quarter3KeyPCH := yearBase + 9993;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter3KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			3, 'Квартал 3',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 3 * 10 + 20, 'Квартал 3', half2KeyPCH);

		quarter4KeyPCH := yearBase + 9994;
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (quarter4KeyPCH, 0,
			yearKey, yearKey,
			2, 'Полугодие 2',
			4, 'Квартал 4',
			-2, 'Данные квартала',
			-2, 'Данные квартала',
			(yearKey * 10000) + 4 * 10 + 20, 'Квартал 4', half2KeyPCH);


	  	for tmpMonth in 1..12 loop

			monthKey := tmpMonth;
			monthBase := monthKey * 100;

			halfKey := case monthKey
				when 1 then 1
				when 2 then 1
				when 3 then 1
				when 4 then 1
				when 5 then 1
				when 6 then 1
				when 7 then 2
				when 8 then 2
				when 9 then 2
				when 10 then 2
				when 11 then 2
				when 12 then 2
				else 2
			end;

			quarterKey := case monthKey
				when 1 then 1
				when 2 then 1
				when 3 then 1
				when 4 then 2
				when 5 then 2
				when 6 then 2
				when 7 then 3
				when 8 then 3
				when 9 then 3
				when 10 then 4
				when 11 then 4
				when 12 then 4
				else 4
			end;

			monthName := case monthKey
				when 1 then 'Январь'
				when 2 then 'Февраль'
				when 3 then 'Март'
				when 4 then 'Апрель'
				when 5 then 'Май'
				when 6 then 'Июнь'
				when 7 then 'Июль'
				when 8 then 'Август'
				when 9 then 'Сентябрь'
				when 10 then 'Октябрь'
				when 11 then 'Ноябрь'
				when 12 then 'Декабрь'
			end;

			monthKeyPCH   := yearBase + monthBase;
			quarterKeyPCH := yearBase + 9990 + quarterKey;

			insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
			values (monthKeyPCH, 0,
				yearKey, yearKey,
				halfKey, 'Полугодие ' || halfKey,
				quarterKey, 'Квартал ' || quarterKey,
				monthKey, monthName,
				-2, 'Данные месяца',
				monthKeyPCH, monthName, quarterKeyPCH);

			MaxDaysInMonth := case monthKey
				when 1 then 31
				when 2 then 29
				when 3 then 31
				when 4 then 30
				when 5 then 31
				when 6 then 30
				when 7 then 31
				when 8 then 31
				when 9 then 30
				when 10 then 31
				when 11 then 30
				when 12 then 31
				else 0
			end;
			if (MOD(yearKey, 4) = 0) and (monthKey = 2) then
				MaxDaysInMonth := 29;
			end if;

			for tmpDay in 1..MaxDaysInMonth loop

				dayKey := tmpDay;
				fullKey := yearBase + monthBase + dayKey;

				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (fullKey, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					dayKey, dayKey,
					fullKey, dayKey, monthKeyPCH);

			end loop;

			if monthKey = 12 then
				-- Заключительные обороты
				insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
				values (yearBase + monthBase + 32, 0,
					yearKey, yearKey,
					halfKey, 'Полугодие ' || halfKey,
					quarterKey, 'Квартал ' || quarterKey,
					monthKey, monthName,
					32, 'Заключительные обороты',
					yearBase + monthBase + 32, 'Заключительные обороты', monthKeyPCH);

			end if;

		end loop;

		-- Остатки на начало года
		insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID, DateHalfYear, DateQuarterID, DateQuarter, DateMonthID, DateMonth, DateDayID, DateDay, OrderByDefault, Name, ParentID)
		values (yearBase, 0,
			yearKey, yearKey,
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			-1, 'Остатки на начало года',
			yearBase, 'Остатки на начало года', yearKeyPCH);

	end loop;

end sp_FillDateUNV;
/

begin sp_FillDateUNV; end;
/

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (33, '2.3.1.12', To_Date('14.11.2007', 'dd.mm.yyyy'), SYSDATE, 'Расширение классификаторов "Период" до 2020 года', 0);

commit;

whenever SQLError exit rollback;

/* End - 6630 - расширение классификаторов 'период' до 2020 года  - feanor - 14.11.2007 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (34, '2.4.0.0', To_Date('28.11.2007', 'dd.mm.yyyy'), SYSDATE, '', 0);

commit;

