/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - ���������� ������� ���� PROGRAMVERSION ������� PUMPHISTORY - vPetrov - 29.08.2011 */

ALTER TABLE PUMPHISTORY MODIFY (PROGRAMVERSION varchar(20));

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (101, '3.0.0.6', To_Date('29.08.2011', 'dd.mm.yyyy'), SYSDATE, '���������� ������� ���� PROGRAMVERSION ������� PUMPHISTORY', 0);

commit;

whenever SQLError exit rollback;

/* End - ���������� ������� ���� PROGRAMVERSION ������� PUMPHISTORY - vPetrov - 29.08.2011 */
