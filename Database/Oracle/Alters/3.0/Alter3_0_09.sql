/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #17838 - Добавление нового интерфейса "Сбор по ЖКХ" - barhonina - 26.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', 'Сбор по ЖКХ', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (104, '3.0.0.9', To_Date('26.10.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "Сбор по ЖКХ"', 0);

DECLARE
  m_id number;
BEGIN
    SELECT ID INTO m_id FROM GROUPS WHERE Name = 'МО сбор ЖКХ';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('МО сбор ЖКХ', 'МО сбор ЖКХ', 0);
END;  
/

DECLARE a_id number;
BEGIN
    SELECT ID INTO a_id FROM GROUPS WHERE Name = 'Аудит сбор ЖКХ';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('Аудит сбор ЖКХ', 'Аудит сбор ЖКХ', 0);

END;  
/

DECLARE o_id number;
BEGIN
    SELECT ID INTO o_id FROM GROUPS WHERE Name = 'Организации сбор ЖКХ';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('Организации сбор ЖКХ', 'Организации сбор ЖКХ', 0);

END;  
/

DECLARE i_id number;
BEGIN
    SELECT ID INTO i_id FROM GROUPS WHERE Name = 'ИОГВ сбор ЖКХ';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('ИОГВ сбор ЖКХ', 'ИОГВ сбор ЖКХ', 0);

END;  
/
commit;

whenever SQLError exit rollback;

/* End - #17838 - Добавление нового интерфейса "Сбор по ЖКХ" - barhonina - 26.10.2011 */

