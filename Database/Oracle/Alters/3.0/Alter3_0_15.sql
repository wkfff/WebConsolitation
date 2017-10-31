/********************************************************************
    Переводит базу Oracle из версии 3.0 в следующую версию 3.x 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - #19256 - Добавление нового интерфейса "Целевые программы" - vorontsov - 23.11.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', 'Целевые программы', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');

declare
  n number;
begin
  select null into n from groups G where G.name = 'ЭО15_ЦП Заказчики';
exception
  when no_data_found then
    insert into groups (name, description, blocked) values ('ЭО15_ЦП Заказчики', 'Заказчик Целевых программ', 0);
end;

declare
  n number;
begin
  select null into n from groups G where G.name = 'ЭО15_ЦП Координаторы';
exception
  when no_data_found then
    insert into groups (name, description, blocked) values ('ЭО15_ЦП Координаторы', 'ЭО Целевых программ', 0);
end;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (111, '3.0.0.10', To_Date('23.11.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "Инвестиционные площадки"', 0);

commit;

/* End - #19256 - Добавление нового интерфейса "Целевые программы" - vorontsov - 23.11.2011 */
