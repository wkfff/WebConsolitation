/********************************************************************
    Переводит базу Oracle из версии 3.0 в следующую версию 3.x 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - #18503 - Добавление нового интерфейса "Инвестиционные площадки" - vorontsov - 24.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (250, 'RIA.EO14InvestAreas', 'Инвестиционные площадки', 'Krista.FM.RIA.Extensions.EO14InvestAreas.InvestAreasExtensionInstaller, Krista.FM.RIA.Extensions.EO14InvestAreas');

insert into groups (name, description, blocked) values ('EO14_Creator', 'Создатели ИП', 0);
insert into groups (name, description, blocked) values ('EO14_Coordinator', 'Координаторы ИП', 0);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (105, '3.0.0.10', To_Date('24.10.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "Инвестиционные площадки"', 0);

commit;

/* End - #18503 - Добавление нового интерфейса "Инвестиционные площадки" - vorontsov - 24.10.2011 */
