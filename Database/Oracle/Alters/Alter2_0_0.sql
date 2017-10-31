/*******************************************************************
 Переводит DV-базу Oracle из версии 2.0.0 в следующую версию 2.0.1
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Нет - Вьюшки для объединения ссылок на сопоставимые - maha - 09.09.2005 17:18 */

/*	!!! После создания двух следующих вьюшек в них необходимо ID ассоциаций на актуальные !!! */

/* Блок "План доходов", сопоставимый КД */

whenever SQLError continue commit;

create or replace view bu_IncomesData_KD
(
id, RefBridgeKD
) as
select
     H.ID, D1.RefBridgeKD2004
from (LNK_Fact H join LNK_Fact2DataCls L on (H.ID = L.ID and H.TypeID = L.TypeID and L.LinkTypeID = 50))
     join d_KD_Budget2004 D1 on (L.RefID = D1.ID and L.RefID <> -1)
union all
select
     H.ID, D1.RefBridgeKD
from (LNK_Fact H join LNK_Fact2DataCls L on (H.ID = L.ID and H.TypeID = L.TypeID and L.LinkTypeID = 49))
     join d_KD_Budget D1 on (L.RefID = D1.ID and L.RefID <> -1);

/* Блок "Доходы", сопоставимый КД */

create or replace view bu_Incomes32_KD
(
id, RefBridgeKD
) as
select
     H.ID, D1.RefBridgeKD
from (LNK_Fact H join LNK_Fact2DataCls L on (H.ID = L.ID and H.TypeID = L.TypeID and L.LinkTypeID = 26))
     join d_KD_Budget D1 on (L.RefID = D1.ID and L.RefID <> -1)
union all
select
     H.ID, D1.RefBridgeKD2004
from (LNK_Fact H join LNK_Fact2DataCls L on (H.ID = L.ID and H.TypeID = L.TypeID and L.LinkTypeID = 27))
     join d_KD_Budget2004 D1 on (L.RefID = D1.ID and L.RefID <> -1);

whenever SQLError exit rollback;

/* End - Нет - Вьюшки для объединения ссылок на сопоставимые - maha - 09.09.2005 17:18 */


/* Start - Нет - Добавление индексов на внешние ключи для ускорения уданения данных - gbelov - 08.09.2005 19:31 */

whenever SQLError continue commit;

create index idx_PumpHistoryRefPumpRegistry on PumpHistory (RefPumpRegistry) tablespace DVINDX compute statistics;

create index idx_LNK_Fact2DataCls on LNK_Fact2DataCls (TypeID, ID) tablespace DVINDX compute statistics;
create index idx_LNK_Fact2DataClsRef on LNK_Fact2DataCls (RefTypeID, RefID) tablespace DVINDX compute statistics;

commit;

whenever SQLError exit rollback;

/* End   - Нет - Добавление индексов на внешние ключи для ускорения уданения данных - gbelov - 08.09.2005 19:31 */

/* Start - Нет - Дополнительный уровень представлений - gbelov - 09.09.2005 17:18 */

/* Строим дополнительный уровень представлений для измерений на классификаторы
в которых присутствуют длинные имена, иначе не расчитываются кубы */

whenever SQLError continue commit;

create or replace view bv_kd_bridge as select T.*, cast(T.Name as varchar2(255)) as ShortName from b_kd_bridge T;
create or replace view dv_kd_budget as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_kd_budget T;
create or replace view bv_incomeskinds_bridge as select T.*, cast(T.Name as varchar2(255)) as ShortName from b_incomeskinds_bridge T;
create or replace view dv_incomeskinds_budget as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_incomeskinds_budget T;

whenever SQLError exit rollback;

/* End - Нет - Дополнительный уровень представлений - gbelov - 09.09.2005 17:18 */

/* Start - Нет - Фиктивный пользователь - borisov - 09.09.2005 17:18 */

/* ЗАГЛУШКА. Пока работа с пользователями не реализована - вставляем временную запись */
insert into Users (ID, Name, Caption, Description, UserType, Blocked)
values (0, 'TEMPUSER', 'Временная запись', 'Фиктивный пользователь', 0, 0);

commit;

/* End - Нет - Фиктивный пользователь - borisov - 09.09.2005 17:18 */

/* Start - Нет - Новые типы сообщений с логах - borisov - 09.09.2005 17:18 */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (108, 0, 'Начало закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (109, 5, 'Завершение закачки файла с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (110, 6, 'Успешное завершение закачки файла');

commit;

/* End - Нет - Новые типы сообщений с логах - borisov - 09.09.2005 17:18 */

/* Start - Нет - Новые новые поля для документов - borisov - 09.09.2005 17:18 */

delete from Documents;

/* Название документа */
alter table Documents
	add Name varchar2 (255) not null;

/* Название первоначального файла */
alter table Documents
	add SourceFileName varchar2 (255) not null;

/* Сам документ */
alter table Documents
	add Document blob;

/* End - Нет - Новые новые поля для документов - borisov - 09.09.2005 17:18 */

/* Start - Нет - Дополнительный уровень представлений - barkova - 19.09.2005 */

/* Строим дополнительный уровень представлений для измерений на классификаторы
в которых присутствуют длинные имена, иначе не расчитываются кубы */

whenever SQLError continue commit;

create or replace view bv_kd_bridgeplan as select T.*, cast(T.Name as varchar2(255)) as ShortName from b_kd_bridgeplan T;

whenever SQLError exit rollback;

/* End - Нет - Дополнительный уровень представлений - barkova - 19.09.2005 */

/* Start - Нет - Изменение структуры таблиц прекодировок - gbelov - 19.09.05 19:31 */

delete from ConversionTable;

delete from ConversionInputAttributes;

delete from ConversionOutAttributes;

commit;

alter table ConversionInputAttributes
	drop column RefConversionTable;

alter table ConversionOutAttributes
	drop column RefConversionTable;

alter table ConversionInputAttributes
	drop constraint PKConversionInputAttributes;

alter table ConversionOutAttributes
	drop constraint PKConversionOutAttributes;

alter table ConversionInputAttributes
	add TypeID number (10) not null;

alter table ConversionOutAttributes
	add TypeID number (10) not null;

alter table ConversionInputAttributes
	add constraint PKConversionInputAttributes primary key ( TypeID, ID, Name );

alter table ConversionOutAttributes
	add constraint PKConversionOutAttributes primary key ( TypeID, ID, Name );

alter table ConversionInputAttributes
	add constraint FKConversionInputAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade;

alter table ConversionOutAttributes
	add constraint FKConversionOutAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade;

commit;

/* End - Нет - Изменение структуры таблиц прекодировок - gbelov - 19.09.05 19:31 */

commit;

/* Start - Нет - Добавление поля комментария к таблице документов (задачи) - borisov - 22.09.05 14:15 */

alter table Documents
	add Description varchar2 (4000);

commit;

/* End - Нет - Добавление поля комментария к таблице документов (задачи) - borisov - 22.09.05 14:15 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (2, '2.0.1', SYSDATE, SYSDATE, '');

commit;
