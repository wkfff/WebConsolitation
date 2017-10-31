/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY - vPetrov - 29.08.2011 */

ALTER TABLE PUMPHISTORY MODIFY (PROGRAMVERSION varchar(20));

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (101, '3.0.0.6', To_Date('29.08.2011', 'dd.mm.yyyy'), SYSDATE, 'Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY', 0);

commit;

whenever SQLError exit rollback;

/* End - Увеличение размера поля PROGRAMVERSION таблицы PUMPHISTORY - vPetrov - 29.08.2011 */
