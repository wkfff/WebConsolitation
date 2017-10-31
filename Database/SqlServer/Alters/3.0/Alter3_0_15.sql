/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #19256 - ���������� ������ ���������� "������� ���������" - vorontsov - 23.11.2011 */

insert into DV.RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', '������� ���������', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');

if (not exists (select null from Groups where [Name] = '��15_�� ���������'))
begin
   insert into g.Groups default values;
   delete from g.Groups where ID = @@IDENTITY;
   insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, '��15_�� ���������', '�������� ������� ��������', 0);
end;

if (not exists (select null from Groups where [Name] = '��15_�� ������������'))
begin
   insert into g.Groups default values;
   delete from g.Groups where ID = @@IDENTITY;
   insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, '��15_�� ������������', '�� ������� ��������', 0);
end;

insert into DV.DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (111, '3.0.0.15', CONVERT(datetime, '2011.11.23', 102), GETDATE(), '��������������� web-��������� ������������ "������� ���������"', 0);

go

/* End - #19256 - ���������� ������ ���������� "������� ���������" - vorontsov - 23.11.2011 */
