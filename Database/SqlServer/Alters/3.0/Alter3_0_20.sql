/*******************************************************************
  ��������� ���� Sql Server 2005 �� ������ 3.0 � ��������� ������ 3.X
*******************************************************************/

/* Start - - ��������� ������� ��� �������� ������������ ������� � ����������� ��������������� - gbelov - 22.12.2011 */

create table HashObjectsNames
(
     HashName varchar(30) not null, 	-- ������������ ��� 
     LongName varchar(2048) not null, 	-- ������ ��� 
     ObjectType numeric(10) not null, 	-- ��� ������� (������������ Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes)
     constraint PKHashObjectsNames primary key (HashName, ObjectType)
)
go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (116, '3.0.0.20', CONVERT(datetime, '2011.12.22', 102), GETDATE(), '��������� ������� ��� �������� ������������ ������� � ����������� ���������������', 0);

go

/* End - - ��������� ������� ��� �������� ������������ ������� � ����������� ��������������� - gbelov - 22.12.2011 */
