/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 3.0 
********************************************************************/

/* Start - #16079 - Оценка ОМСУ - gbelov - 29.03.2011 */

/* Регистрация интерфейса пользоватебя "Оценка ОМСУ" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (190, 'RIA.MarksOMSU', 'Оценка ОМСУ', 'Krista.FM.RIA.Extensions.MarksOMSU');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (93, '2.7.0.14', CONVERT(datetime, '2011.03.29', 102), GETDATE(), 'Зарегистрирован интерфейс пользоватебя "Оценка ОМСУ"', 0);

go

/* End - #16079 - Оценка ОМСУ - gbelov - 29.03.2011 */
