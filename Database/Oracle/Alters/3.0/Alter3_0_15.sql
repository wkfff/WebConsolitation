/********************************************************************
    ��������� ���� Oracle �� ������ 3.0 � ��������� ������ 3.x 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - #19256 - ���������� ������ ���������� "������� ���������" - vorontsov - 23.11.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', '������� ���������', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');

declare
  n number;
begin
  select null into n from groups G where G.name = '��15_�� ���������';
exception
  when no_data_found then
    insert into groups (name, description, blocked) values ('��15_�� ���������', '�������� ������� ��������', 0);
end;

declare
  n number;
begin
  select null into n from groups G where G.name = '��15_�� ������������';
exception
  when no_data_found then
    insert into groups (name, description, blocked) values ('��15_�� ������������', '�� ������� ��������', 0);
end;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (111, '3.0.0.10', To_Date('23.11.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� web-��������� ������������ "�������������� ��������"', 0);

commit;

/* End - #19256 - ���������� ������ ���������� "������� ���������" - vorontsov - 23.11.2011 */
