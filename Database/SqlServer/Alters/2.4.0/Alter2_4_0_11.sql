/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */

alter table Templates add DocumentFileName varchar2 (500);
alter table Templates drop column UIDTemplate;
alter table Templates drop column DocumentName;

go 

insert into RegisteredUIModules (ID, Name, Description) values (130, 'TemplatesViewObj', 'Репозиторий эксперта');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (45, '2.4.0.10', CONVERT(datetime, '2008.05.06', 102), GETDATE(), 'Изменения в структуре хранения отчетов репозитория', 0);

go

/* End - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */
