/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - FMQ00014944- Прогноз консолидированного бюджета - Paluh - 22.02.2011 */
/* Регистрация интерфейса пользоватебя "Прогноз консолидированного бюджета" */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (180, 'ConsBudgetForecastUI', 'Прогноз консолидированного бюджета', 'Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.ConsBudgetForecastNavigation, Krista.FM.Client.ViewObjects.ConsBudgetForecastUI');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (92, '2.7.0.13', CONVERT(datetime, '2011.03.29', 102), GETDATE(), 'Зарегистрирован интерфейс пользоватебя "Прогноз консолидированного бюджета"', 0);

go

/* End - FMQ00014944- Прогноз консолидированного бюджета - Paluh - 22.02.2011 */
