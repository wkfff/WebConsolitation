/*******************************************************************
 Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 3981 - GUID для объектов - пиудщм - 14.03.2008 */

-- Уникальный идентификатор объекта
alter table MetaPackages
	add ObjectKey varchar (36) default '00000000-0000-0000-0000-000000000000' not null;

-- Уникальный идентификатор объекта
alter table MetaObjects
	add ObjectKey varchar (36) default '00000000-0000-0000-0000-000000000000' not null;

-- Уникальный идентификатор объекта
alter table MetaLinks
	add ObjectKey varchar (36) default '00000000-0000-0000-0000-000000000000' not null;

-- Уникальный идентификатор объекта
alter table MetaDocuments
	add ObjectKey varchar (36) default '00000000-0000-0000-0000-000000000000' not null;

-- Уникальный идентификатор объекта
alter table OlapObjects
	add ObjectKey varchar (36);

go

drop view MetaLinksWithRolesNames;

go

/* Представление для получения имен ролей */
create view MetaLinksWithRolesNames (ID, Semantic, Name, Class, ObjectKey, RoleAName, RoleBName, RoleAObjectKey, RoleBObjectKey, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class, L.ObjectKey,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OP.Semantic + '.' + OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OC.Semantic + '.' + OC.Name,
	OP.ObjectKey, OC.ObjectKey,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

go

drop view MetaConversionTablesCatalog;

go

/* Представление для получения списка таблиц перекодировок */
create view MetaConversionTablesCatalog (ID, AssociationKey, AssociationName, RuleName) as
select CT.ID, L.ObjectKey, 'a.' + L.Semantic + '.' + L.Name, CT.AssociateRule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

go

-- Уникальный идентификатор объекта
alter table Objects
	add ObjectKey varchar (255);

update MetaPackages set ObjectKey = 'c7a4196e-568e-482f-8219-921423b8a77a' where ID = 1;
update MetaObjects set ObjectKey = 'b4612528-0e51-4e6b-8891-64c22611816b' where ID = 0;
update MetaObjects set ObjectKey = '675ede52-a0b4-423a-b0f6-365ad02d0f6f' where ID = 1;
update MetaObjects set ObjectKey = '30fd54c6-de78-4664-afb2-d5fcadc10e9c' where ID = 2;
update MetaObjects set ObjectKey = 'c66d6056-7282-4ab0-ab0b-ea43ad68cb4c' where ID = 3;

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (42, '2.4.0.8', CONVERT(datetime, '2008.03.14', 102), GETDATE(), 'GUID для объектов.', 0);

go

/* End - 3981 - GUID для объектов - пиудщм - 14.03.2008 */
