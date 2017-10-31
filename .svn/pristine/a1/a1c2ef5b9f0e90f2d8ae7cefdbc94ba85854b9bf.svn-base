/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 9239 - Авторизиция для web интерфейса. Добавлен тип объекта web интерфейс. - avolkov - 08.10.2008 */

insert into g.Objects default values;
insert into objects (ID, name, caption, objecttype, objectkey) 
values (@@IDENTITY, 'WebReports', 'Web-интерфейс', 20000, 'WebReports');

go

/* End - 9239 - Авторизиция для web интерфейса. Добавлен тип объекта web интерфейс. - avolkov - 08.10.2008 */


/* Start -  - Добавлена таблица для хранения настроек web parts. - gbelov - 08.10.2008 */

CREATE TABLE Personalization 
(
	UserName        varchar (255),
	Application     varchar (645),
	pagesettings    varchar (max)
	constraint UK_Pers UNIQUE (UserName, Application)
);

/* End - - Добавлена таблица для хранения настроек web parts. - gbelov - 08.10.2008  */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (54, '2.4.1.6', CONVERT(datetime, '2008.10.08', 102), GETDATE(), 'Добавлен тип объекта web интерфейс.', 0);

go

