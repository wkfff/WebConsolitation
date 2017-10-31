/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - #20138 - ���������� ������ ���������� "�c�������� �������� ���" - barhonina - 29.01.2012 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (264, 'RIA.EO15ExcCostsAIP', '��15 �c�������� �������� ���', 'Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (117, '3.0.0.22', CONVERT(datetime, '2012.01.29', 102), GETDATE(), '��������������� web-��������� ������������ "�c�������� �������� ���"', 0);

if (not exists (select null from Groups where [Name] = '��15_������������� ��'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '��15_������������� ��', '�� ���������� �������� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '��15_������������� ���������'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '��15_������������� ���������', '��������� ���������� �������� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '��15_������������� ������������'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '��15_������������� ������������', '������������ ���������� �������� ���', 0, '')
end;

if (not exists (select null from Groups where [Name] = '��15_������������� ������������'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, '��15_������������� ������������', '������������ ���������� �������� ���', 0, '')
end;

go

/* End - #20138 - ���������� ������ ���������� "�c�������� �������� ���" - barhonina - 29.01.2012 */
