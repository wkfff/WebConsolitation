/*******************************************************************
 Переводит базу Oracle из версии 2.7 в следующую версию 2.8
*******************************************************************/

:On Error ignore

:r Alter2_7_01.sql
:r Alter2_7_03.sql
:r Alter2_7_04.sql
:r Alter2_7_05.sql
:r Alter2_7_07.sql
:r Alter2_7_08.sql
:r Alter2_7_11.sql
:r Alter2_7_12.sql

--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (80, '2.7.0.0', CONVERT(datetime, '2010.03.12', 102), GETDATE(), '', 0);

:On Error exit
