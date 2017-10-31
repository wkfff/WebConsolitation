/*******************************************************************
 Переводит базу Oracle из версии 2.4.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start -  Добавлена таблица для работы с версиями классификаторов - tsvetkov - 10.12.2008 */

/* Версии классификаторов */
create table ObjectVersions
(
	ID			           number (10) not null, /* PK */
	SourceID           number (10) not null, /* Источник данных*/
	ObjectKey          varchar2 (36) not null, /* Уникальный идентификатор объекта */
	PresentationKey    varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор представления */
	Name				       varchar2 (100) not null,	/* Наименование версии */
	constraint PKVersions primary key ( ID ),
	constraint FKVersionsRefSource foreign key ( SourceID )
		references HUB_DataSources ( ID ) on delete set null
);

create sequence g_objectversions;

create or replace trigger t_ObjectVersions_BI before insert on ObjectVersions for each row
begin
	if :new.ID is null then select g_ObjectVersions.NextVal into :new.ID from Dual; end if;
	if :new.presentationkey is null then :new.presentationkey := '00000000-0000-0000-0000-000000000000'; end if;
end;
/

create or replace trigger t_ObjectVersions_BU before update on ObjectVersions for each row
begin
	if :new.presentationkey is null then :new.presentationkey := '00000000-0000-0000-0000-000000000000'; end if;
end;
/

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (60, '2.5.0.4', To_Date('10.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлена таблица для работы с версиями классификаторов', 0);

commit;

/* End -  Добавлена таблица для работы с версиями классификаторов - tsvetkov - 10.12.2008 */
