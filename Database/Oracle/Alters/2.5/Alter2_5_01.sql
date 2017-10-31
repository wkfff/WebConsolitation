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

/* Start - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */

whenever SQLError continue commit;

alter table Templates add DocumentFileName varchar2 (500);
alter table Templates drop column UIDTemplate;
alter table Templates drop column DocumentName;

commit;

alter trigger t_Templates_AA disable;

alter table templates add Editor number(10) default -1;

update templates set Editor = -1;

alter trigger t_Templates_AA enable;

commit;

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (130, 'TemplatesViewObj', 'Репозиторий эксперта', 'Krista.FM.Client.ViewObjects.TemplatesUI.TemplatesNavigation, Krista.FM.Client.ViewObjects.TemplatesUI');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (57, '2.5.0.1', To_Date('02.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория', 0);

commit;

whenever SQLError exit rollback;

/* End - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */
