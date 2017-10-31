/*******************************************************************
 Переводит базу Oracle из версии 2.5 в следующую версию 2.6
*******************************************************************/

:On Error ignore

:r Alter2_5_01.sql;
:r Alter2_5_02.sql;
:r Alter2_5_06.sql;
:r Alter2_5_07.sql;
:r Alter2_5_08.sql;
:r Alter2_5_09.sql;
:r Alter2_5_10.sql;
:r Alter2_5_11.sql;

--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (57, '2.6.0.0', CONVERT(datetime, '2008.11.27', 102), GETDATE(), '', 0);

:On Error exit
