/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 3.0 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - #16079 - Оценка ОМСУ - gbelov - 29.03.2011 */

/* Регистрация интерфейса пользоватебя "Оценка ОМСУ" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (190, 'RIA.MarksOMSU', 'Оценка ОМСУ', 'Krista.FM.RIA.Extensions.MarksOMSU');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (93, '2.7.0.14', To_Date('29.03.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован интерфейс пользоватебя "Оценка ОМСУ"', 0);

commit;

whenever SQLError exit rollback;

/* End - #16079 - Оценка ОМСУ - gbelov - 29.03.2011 */
