/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - FMQ00014944- Прогноз консолидированного бюджета - Paluh - 22.02.2011 */

whenever SQLError continue commit;

/* Регистрация интерфейса пользоватебя "Прогноз консолидированного бюджета" */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (180, 'ConsBudgetForecastUI', 'Прогноз консолидированного бюджета', 'Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.ConsBudgetForecastNavigation, Krista.FM.Client.ViewObjects.ConsBudgetForecastUI');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (92, '2.7.0.13', To_Date('29.03.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован интерфейс пользоватебя "Прогноз консолидированного бюджета"', 0);

commit;

whenever SQLError exit rollback;

/* End - FMQ00014944- Прогноз консолидированного бюджета - Paluh - 22.02.2011 */
