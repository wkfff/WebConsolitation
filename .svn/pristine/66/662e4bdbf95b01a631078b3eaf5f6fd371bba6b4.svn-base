/*******************************************************************
 Переводит базу Oracle из версии 2.4.1 в следующую версию 2.5
*******************************************************************/

whenever SQLError continue commit;

--@.\Alter2_4_1_01.sql; -- Репозиторий отчетов в версию не включаем
@.\Alter2_4_1_02.sql;
--@.\Alter2_4_1_03.sql; -- Блок источников финансирования в версию не включаем
@.\Alter2_4_1_04.sql;
@.\Alter2_4_1_05.sql;
@.\Alter2_4_1_06.sql;
@.\Alter2_4_1_07.sql;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (57, '2.5.0.0', To_Date('27.11.2008', 'dd.mm.yyyy'), SYSDATE, '', 0);

whenever SQLError exit rollback;
