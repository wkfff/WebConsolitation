/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.5 в следующую версию 2.6
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 10026 - Изменения в структуре хранения отчетов репозитория - gbelov - 24.02.2009 */

-- Код отчета
alter table Templates add Code varchar (100);
--  Поле для сортировки
alter table Templates add SortIndex int;
--  Поле для флажков (1 - фаворит; 2 - важный; 4 - новый)
alter table Templates add Flags int default 0;

-- Виды шаблонов
create table TemplatesTypes
(
	ID					int not null,		/* PK */
	Name				varchar (500) not null,	/* Название вида шаблона */
	Description			varchar (2048),			/* Описание вида шаблона */
	constraint PKTemplatesTypes primary key ( ID )
)

go 

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

go

-- Ссылка на справочник типов шаблонов
alter table Templates add RefTemplatesTypes int;

update Templates set RefTemplatesTypes = 1;

alter table Templates alter column RefTemplatesTypes int not null;

go

alter table Templates 
	add	constraint FKTemplatesRefTemplatesTypes foreign key (RefTemplatesTypes)
		references TemplatesTypes (ID);

go
		
insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (65, '2.5.0.9', CONVERT(datetime, '2009.02.25', 102), GETDATE(), 'Изменения в структуре хранения отчетов репозитория.', 0);

go

/* End - 10026 - Изменения в структуре хранения отчетов репозитория - gbelov - 24.02.2009 */
