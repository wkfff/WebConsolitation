/********************************************************************
	��������� ���� Oracle �� ������ 2.7 � ��������� ������ 2.8 
********************************************************************/

/* ������ ���������� �������: */ 
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 
	/* ��� SQL-������ */
/* End - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 


/* Start - ����������� ����� */ 


/* Start - <����� ������ � ClearQuest> -���������� ������� ��������������� �������� � OlapObjects - tsvetkov - 05.10.2010 */

alter table OlapObjects add BatchOperations VARCHAR(255) default null

go 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (89, '2.7.0.10', CONVERT(datetime, '2010.10.05', 102), GETDATE(), '���������� ������� ��������������� �������� � OlapObjects', 0);

go

/* End - <����� ������ � ClearQuest> - ���������� ������� ��������������� �������� � OlapObjects - tsvetkov - 05.10.2010 */
