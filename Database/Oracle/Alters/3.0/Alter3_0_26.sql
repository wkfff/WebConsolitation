/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/
whenever SQLError exit rollback;

/* Start - #20568 - ���������� ������ ���������� "������ ���" - vorontsov - 02.03.2012 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (280, 'RIA.Region10MarksOIV', '������ ���', 'Krista.FM.RIA.Extensions.Region10MarksOIV.Region10MarksOivExtensionInstaller, Krista.FM.RIA.Extensions.Region10MarksOIV');

declare
  l_group_id number;
  c_role_name constant varchar2(100):='������ ���';
  c_role_description constant varchar2(100):= '������������ ���� � ��� ����� "������ ���"';
begin
  select G.id 
    into l_group_id 
    from groups G 
   where name = c_role_name;
exception
   when no_data_found then
      insert into groups (name, description, blocked) 
        values (c_role_name, c_role_description, 0);
end;  
/

declare
  l_group_id number;
  c_role_name constant varchar2(100):='������ ���_����������� �������';
  c_role_description constant varchar2(100):= '������������ ��� ����� "������ ���"';
begin
  select G.id 
    into l_group_id 
    from groups G 
   where name = c_role_name;
exception
   when no_data_found then
      insert into groups (name, description, blocked) 
        values (c_role_name, c_role_description, 0);
end;  
/

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (121, '3.0.0.26', To_Date('02.03.2012', 'dd.mm.yyyy'), SYSDATE, '��������������� web-��������� ������������ "������ ���"', 0);

commit;

/* End - #20568 - ���������� ������ ���������� "������ ���" - vorontsov - 02.03.2012 */
