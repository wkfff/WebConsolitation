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

/* Start - 9239 - Авторизиция для web интерфейса. Добавлен тип объекта web интерфейс. - avolkov - 08.10.2008 */

insert into objects (name, caption, objecttype, objectkey) 
values ('WebReports', 'Web-интерфейс', 20000, 'WebReports');

commit;

whenever SQLError exit rollback;

/* End - 9239 - Авторизиция для web интерфейса. Добавлен тип объекта web интерфейс. - avolkov - 08.10.2008 */


/* Start -  - Добавлена таблица для хранения настроек web parts. - gbelov - 08.10.2008 */

create table Personalization 
(
	UserName        varchar2 (255),
	Application     varchar2 (1000),
	PageSettings    blob default empty_blob(),
	constraint UK_Pers UNIQUE ( UserName, Application )
);

whenever SQLError exit rollback;

/* End - - Добавлена таблица для хранения настроек web parts. - gbelov - 08.10.2008  */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (54, '2.4.1.6', To_Date('08.10.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен тип объекта web интерфейс.', 0);

commit;

