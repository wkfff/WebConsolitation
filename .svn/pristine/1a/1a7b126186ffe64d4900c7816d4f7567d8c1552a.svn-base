/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Добавление признака текущей версии классификатора - tsvetkov - 15.06.2011 */

ALTER TABLE OBJECTVERSIONS DROP CONSTRAINT FKVERSIONSREFSOURCE; 
ALTER TABLE ObjectVersions ADD IsCurrent int DEFAULT 0 NOT NULL; 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (98, '3.0.0.3', To_Date('15.06.2011', 'dd.mm.yyyy'), SYSDATE, 'Добавление признака текущей версии классификатора', 0);

commit;

whenever SQLError exit rollback;

/* End - Добавление признака текущей версии классификатора - tsvetkov - 15.06.2011 */
