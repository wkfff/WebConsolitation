/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #17838 - ���������� ������ ���������� "���� �� ���" - barhonina - 07.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', '���� �� ���', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (104, '3.0.0.9', CONVERT(datetime, '2011.10.07', 102), GETDATE(), '��������������� web-��������� ������������ "���� �� ���"', 0);

if (not exists (select null from Groups where [Name] = '�� ���� ���'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '�� ���� ���', '�� ���� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '����� ���� ���'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '����� ���� ���', '����� ���� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '����������� ���� ���'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '����������� ���� ���', '����������� ���� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '���� ���� ���'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '���� ���� ���', '���� ���� ���', 0, '')
end;

go

/* End - #17838 - ���������� ������ ���������� "���� �� ���" - barhonina - 07.10.2011 */

