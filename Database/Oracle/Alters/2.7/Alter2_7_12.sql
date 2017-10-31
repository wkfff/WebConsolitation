/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */

whenever SQLError exit rollback;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (91, '2.7.0.12', To_Date('21.01.2011', 'dd.mm.yyyy'), SYSDATE, 'Создана наблица DUAL', 0);

commit;

/* End - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */
