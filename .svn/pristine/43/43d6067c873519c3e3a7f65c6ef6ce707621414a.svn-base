/*******************************************************************
 Переводит базу SqlServer из версии 2.5 в следующую версию 2.6
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
/* End   - Стандартная часть */

/* Start - - Добавлен уникальный идентификатор правила сопоставления по умолчанию - gbelov - 17.12.2008 */

/* Уникальный идентификатор правила сопоставления по умолчанию */
alter table MetaLinks
	add DefaultAssociateRule varchar (255);
	
go

/* Устанавливаем правило по умолчанию в то правило, которое указано в xml-конфигурации */	
update MetaLinks set DefaultAssociateRule = 
	substring(
		substring(Configuration, charindex('associateRuleDefault', Configuration) + 22, 100),
		1,
		charindex(
			'">', 
			substring(Configuration, charindex('associateRuleDefault', Configuration) + 22,
			100)) - 1
	)
where Class = 2 and DefaultAssociateRule is null and Configuration like '%associateRuleDefault%';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (61, '2.5.0.5', CONVERT(datetime, '2008.12.17', 102), GETDATE(), 'Добавлен уникальный идентификатор правила сопоставления по умолчанию.', 0);

go

/* Start - - Добавлен уникальный идентификатор правила сопоставления по умолчанию - gbelov - 17.12.2008 */
