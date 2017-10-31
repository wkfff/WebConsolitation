/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		ObjectVersions.sql - Версии классификаторов.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Версии классификаторов
pro ================================================================================

/* Версии классификаторов */
create table ObjectVersions
(
	ID			           number (10) not null,                                                  /* PK */
	SourceID               number (10) not null,                                                  /* Источник данных*/
	ObjectKey              varchar2 (36) not null,                                                /* Уникальный идентификатор объекта */
	PresentationKey        varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор представления */
	Name				   varchar2 (100) not null,	                                              /* Наименование версии */
	IsCurrent              NUMBER DEFAULT 0 NOT NULL,                                             /* Признак текущей версии */
	constraint PKVersions primary key ( ID )
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

commit;
