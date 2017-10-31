/*
	��� "������ � ������������"
	������	3.1
	������
		ObjectVersions.sql - ����� ��������� ������.
	����	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  ������ ���������������
:!!echo ================================================================================

/* ������ ��������������� */
create table ObjectVersions
(
	ID			           int not null,                                                             /* PK */
	SourceID               int null,                                                                 /* �������� ������*/
	ObjectKey              varchar (36) not null,                                                    /* ���������� ������������� ������� */
	PresentationKey        varchar (36) default '00000000-0000-0000-0000-000000000000' not null,     /* ���������� ������������� ������������� */
	Name				   varchar (100) not null,	                                                 /* ������������ ������ */
	IsCurrent              int default 0 NOT null,                                                   /* ������� ������� ������*/
	constraint PKVersions primary key ( ID )
)
go

create table g.ObjectVersions ( ID int identity not null );
go

create trigger t_ObjectVersions_BI on ObjectVersions instead of insert as
		begin
		declare @nullsCount int;
		set nocount on;
		select @nullsCount = count(*) from inserted where ID is null;
		if @nullsCount = 0 insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT)
			select ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT from inserted;
		else if @nullsCount = 1
		begin
			insert into g.ObjectVersions default values;
			delete from g.ObjectVersions where ID = @@IDENTITY;
			insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME,ISCURRENT)
				select @@IDENTITY, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT from inserted;
		end else
			RAISERROR (500001, 1, 1);
	end;
go

