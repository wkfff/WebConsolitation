/*******************************************************************
 Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X
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

insert into RegisteredUIModules (ID, Name, Description) values (130, 'TemplatesViewObj', 'Репозиторий эксперта');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (45, '2.4.0.11', To_Date('06.05.2008', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория', 0);

commit;

whenever SQLError exit rollback;

/* End - 6821 - Изменения в структуре хранения отчетов репозитория - Paluh - 06.05.2008 */
