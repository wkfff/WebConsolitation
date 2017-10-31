/*******************************************************************
 Переводит базу Oracle из версии 2.7 в следующую версию 2.8
*******************************************************************/

whenever SQLError continue commit;

@.\Alter2_7_01.sql;
@.\Alter2_7_03.sql;
@.\Alter2_7_05.sql;
@.\Alter2_7_07.sql;
@.\Alter2_7_08.sql;
@.\Alter2_7_11.sql;
@.\Alter2_7_12.sql;

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (80, '2.7.0.0', To_Date('12.03.2010', 'dd.mm.yyyy'), SYSDATE, '', 0);

whenever SQLError exit rollback;
