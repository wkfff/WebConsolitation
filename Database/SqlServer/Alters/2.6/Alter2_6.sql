/*******************************************************************
 Переводит базу Oracle из версии 2.6 в следующую версию 2.7
*******************************************************************/

:On Error ignore

:r Alter2_6_01.sql
:r Alter2_6_02.sql
:r Alter2_6_03.sql
:r Alter2_6_04.sql
:r Alter2_6_05.sql
:r Alter2_6_06.sql
:r Alter2_6_07.sql


--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (57, '2.6.0.0', CONVERT(datetime, '2008.11.27', 102), GETDATE(), '', 0);

:On Error exit
