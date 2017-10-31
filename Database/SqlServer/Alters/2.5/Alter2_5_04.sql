/*******************************************************************
 ��������� ���� Oracle �� ������ 2.4.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* Start -  ��������� ������� ��� ������ � �������� ��������������� - tsvetkov - 10.12.2008 */

/* ������ ��������������� */
create table ObjectVersions
(
	ID			           int not null, /* PK */
	SourceID           int null, /* �������� ������*/
	ObjectKey          varchar (36) not null, /* ���������� ������������� ������� */
	PresentationKey    varchar (36) default '00000000-0000-0000-0000-000000000000' not null, /* ���������� ������������� ������������� */
	Name				       varchar (100) not null,	/* ������������ ������ */
	constraint PKVersions primary key ( ID ),
	constraint FKVersionsRefSource foreign key ( SourceID )
		references HUB_DataSources ( ID ) on delete set null
)
go

create table g.ObjectVersions ( ID int identity not null );
go

create trigger t_ObjectVersions_BI on ObjectVersions instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME)
		select ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME from inserted;
	else if @nullsCount = 1
	begin
		insert into g.ObjectVersions default values;
		delete from g.ObjectVersions where ID = @@IDENTITY;
		insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME)
			select @@IDENTITY, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;
go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (60, '2.5.0.4',CONVERT(datetime, '2009.02.25', 102), GETDATE(), '��������� ������� ��� ������ � �������� ���������������', 0);
go


/* End -  ��������� ������� ��� ������ � �������� ��������������� - tsvetkov - 10.12.2008 */
