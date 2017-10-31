/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */

alter table Templates add DocumentFileName varchar (500);
alter table Templates drop column UIDTemplate;
alter table Templates drop column DocumentName;

go 


alter table templates add Editor int default -1;

update templates set Editor = -1;

go

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (130, 'TemplatesViewObj', 'Репозиторий эксперта', 'Krista.FM.Client.ViewObjects.TemplatesUI.TemplatesNavigation, Krista.FM.Client.ViewObjects.TemplatesUI');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (57, '2.5.0.1', CONVERT(datetime, '2008.12.02', 102), GETDATE(), 'Изменения в структуре хранения отчетов репозитория', 0);

go

/* End - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */
