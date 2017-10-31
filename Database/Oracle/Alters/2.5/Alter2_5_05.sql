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

/* Start - - Добавлен уникальный идентификатор правила сопоставления по умолчанию - gbelov - 17.12.2008 */

/* Уникальный идентификатор правила сопоставления по умолчанию */
alter table MetaLinks
	add DefaultAssociateRule varchar2 (255);

/* Устанавливаем правило по умолчанию в то правило, которое указано в xml-конфигурации */	
update MetaLinks set DefaultAssociateRule = 
	SYS.dbms_lob.substr(
		SYS.dbms_lob.substr(Configuration, 100, SYS.dbms_lob.instr(Configuration, 'associateRuleDefault', 1, 1) + 22),
		SYS.dbms_lob.instr(SYS.dbms_lob.substr(Configuration, 100, SYS.dbms_lob.instr(Configuration, 'associateRuleDefault', 1, 1) + 22), '">', 1, 1) - 1,
		1
	)
where Class = 2 and DefaultAssociateRule is null and Configuration like '%associateRuleDefault%';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (61, '2.5.0.5', To_Date('17.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен уникальный идентификатор правила сопоставления по умолчанию.', 0);

commit;

/* Start - - Добавлен уникальный идентификатор правила сопоставления по умолчанию - gbelov - 17.12.2008 */
