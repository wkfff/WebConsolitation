/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Изменение название интерфейса "Прогноз консолидированного бюджета" - paluh - 19.09.2011 */

update RegisteredUIModules set Description = 'Планирование доходов' where id = 180;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (103, '3.0.0.8', To_Date('09.09.2011', 'dd.mm.yyyy'), SYSDATE, 'Изменение название интерфейса "Прогноз консолидированного бюджета"', 0);

commit;

whenever SQLError exit rollback;

/* End - Изменение название интерфейса "Прогноз консолидированного бюджета" - paluh - 19.09.2011 */
