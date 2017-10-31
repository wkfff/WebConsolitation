/*******************************************************************
 Переводит базу Oracle из версии 2.6 в следующую версию 2.7
*******************************************************************/

whenever SQLError continue commit;

@.\Alter2_6_01.sql;
@.\Alter2_6_02.sql;

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (57, '2.7.1.0', To_Date('24.11.2008', 'dd.mm.yyyy'), SYSDATE, '', 0);

whenever SQLError exit rollback;
