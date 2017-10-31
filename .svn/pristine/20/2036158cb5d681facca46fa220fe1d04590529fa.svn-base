/*******************************************************************
 Переводит базу Oracle из версии 2.4.0 в следующую версию 2.X.X
*******************************************************************/

whenever SQLError continue commit;

@.\Alter2_4_0_2.sql;
@.\Alter2_4_0_3.sql;
@.\Alter2_4_0_5.sql;
@.\Alter2_4_0_6.sql;
@.\Alter2_4_0_8.sql;
@.\Alter2_4_0_10.sql;
@.\Alter2_4_0_12.sql;
@.\Alter2_4_0_13.sql;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (48, '2.4.1.0', To_Date('29.05.2008', 'dd.mm.yyyy'), SYSDATE, '', 1);

whenever SQLError exit rollback;
