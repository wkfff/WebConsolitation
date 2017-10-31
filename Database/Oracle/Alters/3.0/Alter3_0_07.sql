/********************************************************************
    Переводит базу Oracle из версии 3.0 в следующую версию 3.x 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - #17838 - Добавление нового интерфейса "Инвестиционные проекты" - vorontsov - 01.09.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (240, 'RIA.EO12InvestProjects', 'Инвестиционные проекты', 'Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (102, '3.0.0.7', To_Date('01.09.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "Инвестиционные проекты"', 0);

commit;

/* End - #17838 - Добавление нового интерфейса "Инвестиционные проекты" - vorontsov - 01.09.2011 */
