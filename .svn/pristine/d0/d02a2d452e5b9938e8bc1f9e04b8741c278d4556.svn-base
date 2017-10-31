/*******************************************************************
 Переводит базу Oracle из версии 2.4.1 в следующую версию 2.5
*******************************************************************/

:On Error ignore

:r Alter2_4_1_02.sql
:r Alter2_4_1_04.sql
:r Alter2_4_1_05.sql
:r Alter2_4_1_06.sql
:r Alter2_4_1_07.sql

insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (57, '2.5.0.0', CONVERT(datetime, '2008.11.27', 102), GETDATE(), '', 0);

:On Error exit
