/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #19087 - ���������� ������ ���������� "��_0051_������� ��" - barhonina - 12.11.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (252, 'RIA.FO51PassportMO', '��_0051_������� ��','Krista.FM.RIA.Extensions.FO51PassportMO.FO51ExtensionInstaller, Krista.FM.RIA.Extensions.FO51PassportMO');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (109, '3.0.0.13', CONVERT(datetime, '2011.11.12', 102), GETDATE(), '��������������� web-��������� ������������ "��_0051_������� ��"', 0);

go

/* End - #19087 - ���������� ������ ���������� "��_0051_������� ��" - barhonina - 12.11.2011 */

