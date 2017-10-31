/*******************************************************************
 Переводит базу Oracle из версии 2.5 в следующую версию 2.6
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 10026 - Изменения в структуре хранения отчетов репозитория - gbelov - 24.02.2009 */

-- Код отчета
alter table Templates add Code varchar2 (100);
--  Поле для сортировки
alter table Templates add SortIndex number (10);
--  Поле для флажков (1 - фаворит; 2 - важный; 4 - новый)
alter table Templates add Flags number (10) default 0;

-- Виды шаблонов
create table TemplatesTypes
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* Название вида шаблона */
	Description			varchar2 (2048),			/* Описание вида шаблона */
	constraint PKTemplatesTypes primary key ( ID )
);

create sequence g_TemplatesTypes;

create or replace trigger t_TemplatesTypes_bi before insert on TemplatesTypes for each row
begin
	if :new.ID is null then select g_TemplatesTypes.NextVal into :new.ID from Dual; end if;
end t_TemplatesTypes_bi;
/

-- вставляем в справочник задач временную фиктивную запись
insert into TemplatesTypes (ID, Name, Description) values (1, 'Шаблоны отчетов', 'Шаблоны отчетов MDX Эксперта');
insert into TemplatesTypes (ID, Name, Description) values (2, 'Отчеты системы', 'Отчеты блоков: Источники финансирования');
insert into TemplatesTypes (ID, Name, Description) values (3, 'Веб-отчеты', 'Отчеты веб-сайта и дашборды');
insert into TemplatesTypes (ID, Name, Description) values (4, 'iPhone-отчеты', 'Отчеты для отображения на iPhone');

-- регистрируем в качестве объекта системы
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('1_TemplateType', '1_TemplateType', 'Шаблоны отчетов', 'Шаблоны отчетов MDX Эксперта', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('2_TemplateType', '2_TemplateType', 'Отчеты системы', 'Отчеты блоков: Источники финансирования', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('3_TemplateType', '3_TemplateType', 'Веб-отчеты', 'Отчеты веб-сайта и дашборды', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('4_TemplateType', '4_TemplateType', 'iPhone-отчеты', 'Отчеты для отображения на iPhone', 22000);

commit;

-- Ссылка на справочник типов шаблонов
alter table Templates add RefTemplatesTypes number (10);

alter trigger t_Templates_aa disable; 

update Templates set RefTemplatesTypes = 1;

alter trigger t_Templates_aa enable; 

alter table Templates modify RefTemplatesTypes number (10) not null;

alter table Templates 
	add	constraint FKTemplatesRefTemplatesTypes foreign key (RefTemplatesTypes)
		references TemplatesTypes (ID);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (65, '2.5.0.9', To_Date('25.02.2009', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория.', 0);

commit;

/* End - 10026 - Изменения в структуре хранения отчетов репозитория - gbelov - 24.02.2009 */
