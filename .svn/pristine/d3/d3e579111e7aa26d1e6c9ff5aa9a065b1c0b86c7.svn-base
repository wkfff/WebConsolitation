/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #20138 - Добавление нового интерфейса "Иcполнение расходов АИП" - barhonina - 29.01.2012 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (264, 'RIA.EO15ExcCostsAIP', 'ЭО15 Иcполнение расходов АИП', 'Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (117, '3.0.0.22', To_Date('29.01.2012', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "ЭО15 Иcполнение расходов АИП"', 0);

DECLARE
  m_id number;
BEGIN
    SELECT ID INTO m_id FROM GROUPS WHERE Name = 'ЭО15_Строительство МО';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('ЭО15_Строительство МО', 'МО Исполнение расходов АИП', 0);
END;  
/

DECLARE
  c_id number;
BEGIN
    SELECT ID INTO c_id FROM GROUPS WHERE Name = 'ЭО15_Строительство Заказчики';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('ЭО15_Строительство Заказчики', 'Заказчики Исполнение расходов АИП', 0);
END;  
/

DECLARE
  k_id number;
BEGIN
    SELECT ID INTO k_id FROM GROUPS WHERE Name = 'ЭО15_Строительство Координаторы';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('ЭО15_Строительство Координаторы', 'Координаторы Исполнение расходов АИП', 0);
END;  
/

DECLARE
  u_id number;
BEGIN
    SELECT ID INTO u_id FROM GROUPS WHERE Name = 'ЭО15_Строительство Пользователи';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('ЭО15_Строительство Пользователи', 'Пользователи Исполнение расходов АИП', 0);
END;  
/
commit;

/* End - #20138 - Добавление нового интерфейса "Иcполнение расходов АИП" - barhonina - 29.01.2012 */

