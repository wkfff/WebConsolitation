/*******************************************************************
 Переводит базу Oracle из версии 2.5 в следующую версию 2.6
*******************************************************************/

whenever SQLError continue commit;

@.\Alter2_5_01.sql;
@.\Alter2_5_02.sql;
@.\Alter2_5_06.sql;
@.\Alter2_5_07.sql;
@.\Alter2_5_09.sql;
@.\Alter2_5_10.sql;
@.\Alter2_5_11.sql;

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (57, '2.6.0.0', To_Date('27.11.2008', 'dd.mm.yyyy'), SYSDATE, '', 0);

whenever SQLError exit rollback;
