/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 3.0 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - #15948 - Сбор данных с МО - gbelov - 29.03.2011 */

/* Регистрация интерфейса пользоватебя "Сбор данных с МО" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (200, 'RIA.MOFOWebForms', 'Сбор данных с МО', 'Krista.FM.RIA.Extensions.MOFOWebForms');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (94, '2.7.0.15', To_Date('29.03.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован интерфейс пользоватебя "Сбор данных с МО"', 0);

commit;

whenever SQLError exit rollback;

/* End - #15948 - Сбор данных с МО - gbelov - 29.03.2011 */
