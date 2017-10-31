/*******************************************************************
 Переводит базу SQLServer из версии 2.4.0 в следующую версию 2.X.X
*******************************************************************/

:r .\Alter2_4_0_2.sql
:r .\Alter2_4_0_3.sql
/*:r .\Alter2_4_0_5.sql*/
:r .\Alter2_4_0_6.sql
:r .\Alter2_4_0_8.sql
:r .\Alter2_4_0_10.sql
:r .\Alter2_4_0_12.sql
:r .\Alter2_4_0_13.sql

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (48, '2.4.1.0', CONVERT(datetime, '2008.05.29', 102), GETDATE(), '', 1);

go
