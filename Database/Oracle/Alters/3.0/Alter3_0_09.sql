/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #17838 - ���������� ������ ���������� "���� �� ���" - barhonina - 26.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', '���� �� ���', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (104, '3.0.0.9', To_Date('26.10.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� web-��������� ������������ "���� �� ���"', 0);

DECLARE
  m_id number;
BEGIN
    SELECT ID INTO m_id FROM GROUPS WHERE Name = '�� ���� ���';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('�� ���� ���', '�� ���� ���', 0);
END;  
/

DECLARE a_id number;
BEGIN
    SELECT ID INTO a_id FROM GROUPS WHERE Name = '����� ���� ���';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('����� ���� ���', '����� ���� ���', 0);

END;  
/

DECLARE o_id number;
BEGIN
    SELECT ID INTO o_id FROM GROUPS WHERE Name = '����������� ���� ���';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('����������� ���� ���', '����������� ���� ���', 0);

END;  
/

DECLARE i_id number;
BEGIN
    SELECT ID INTO i_id FROM GROUPS WHERE Name = '���� ���� ���';
    EXCEPTION
        WHEN no_data_found then
    INSERT INTO DV.Groups
        (Name, Description, Blocked) 
        VALUES ('���� ���� ���', '���� ���� ���', 0);

END;  
/
commit;

whenever SQLError exit rollback;

/* End - #17838 - ���������� ������ ���������� "���� �� ���" - barhonina - 26.10.2011 */

