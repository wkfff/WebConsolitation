/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		OlapView.sql - Вьюшки для построения кубов.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Представления для построения кубов
pro ================================================================================

whenever SQLError continue commit;

/* Представления для измерений "В Бюджет..." и "Из Бюджета..."*/

CREATE OR REPLACE VIEW dv_regions_budget_refbudsource
AS
SELECT *
FROM dv_Regions_Budget;

CREATE OR REPLACE VIEW dv_regions_budget_refbuddest
AS
SELECT *
FROM dv_Regions_Budget;

CREATE OR REPLACE VIEW bv_regions_bridge_refbudsource
AS
SELECT *
FROM bv_Regions_Bridge;

CREATE OR REPLACE VIEW bv_regions_bridge_refbuddest
AS
SELECT *
FROM bv_Regions_Bridge;

/* Представления для вычисляемых полей в измерениях "КИФ..." */

create or replace view FV_KIF_DIRECTION_BRIDGE
as select *
from FX_KIF_DIRECTION;

create or replace view FV_KIF_CLSASPECT_BRIDGE
as select *
from FX_KIF_CLSASPECT;

/* Представления для вычисляемых полей в измерении "МФ РФ.Сопоставимый" */

create or replace view dv_Units_OKEI_Bridge
as select *
from Dv_Units_OKEI;

create or replace view fv_FX_VerificationConditions
as select *
from fx_FX_VerificationConditions;

/* Представление для измерения "Период.День ФО", чтобы использовать это измерение совместно с "Период.День ФК" в кубе "УФК_Свод ФУ" */

CREATE OR REPLACE VIEW fv_date_yearday
AS
SELECT *
FROM fx_date_yearday;

/* Представления для измерений "РасчСчета.Корреспонд Счет" и "РасчСчета.Корресп сопост" - необходимы для куба "ФО_АС Бюджет_Операции со счетами" */

CREATE OR REPLACE VIEW dv_Accounts_BudgetFOOwn_Cor
AS
SELECT *
FROM dv_Accounts_Budgetfoown;

CREATE OR REPLACE VIEW bv_Accounts_Bridge_Cor
AS
SELECT *
FROM b_Accounts_Bridge;

/* Представление с полем name, разбитым на 3 части для измерения "Фонды.Типы фондов" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW DV_FUND_FO9TYPES
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_Fund_FO9Types T;

/* Представление с полем name, разбитым на 3 части для измерения "КЦСР.Сопоставимый планирование" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW BV_KCSR_BRIDGEPLAN
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_KCSR_BridgePlan T;


/* Блок ФО_0016_Мониторинг_БК_КУ */
create or replace view fv_FX_VerificationConditions as
select ID, RowType, Name, Value
from fx_FX_VerificationConditions;


/* Представление для измерения "Организации.Сопоставимый" */
CREATE OR REPLACE VIEW BV_ORGANIZATIONS_BRIDGE
AS
SELECT T.*, substr(T.Name, 0, 255) as short_name
FROM b_Organizations_Bridge T;

/* Представление для измерения "Организации.Сопоставимый планирование" */
CREATE OR REPLACE VIEW BV_ORGANIZATIONS_BRIDGEPLAN
AS
SELECT T.*, substr(T.Name, 0, 255) as short_name
FROM b_Organizations_BridgePlan T;

/* Представление для измерения "Период.Месяц отчет" */
CREATE OR REPLACE VIEW fv_date_yearmonth AS
SELECT * FROM fx_date_yearmonth;

/* Представление для измерения Тип территориий */
create or replace view dv_FX_FX_TERRITORIALPART as
select ID, ROWTYPE, NAME, FULLNAME
from FX_FX_TERRITORIALPARTITIONTYPE;

/* Представление для измерения Тип территориий */
create or replace view dv_FX_FX_TERRITORIALPARTTYPE as
select ID, ROWTYPE, NAME, FULLNAME
from FX_FX_TERRITORIALPARTITIONTYPE;

/* Start - 7861 - Представление с полем name, разбитым на 3 части для измерения "КИФ.АС Бюджет 2005" - zaharchenko- 14.02.2008 */
/* Удалить после перхода на 2005 */

CREATE OR REPLACE VIEW DV_KIF_BUDGET2005
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM D_KIF_BUDGET2005 T;

/* End - 7861 - Представление с полем name, разбитым на 3 части для измерения "КИФ.АС Бюджет 2005" - zaharchenko- 14.02.2008 */

/* Start - xxxx - Представления для кубов блока Мониторинг государственных закупок - dianova - 29.09.2008 */
/* Удалить когда будет закачка по нормальной постановке */

/* Количество заявок */
CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select distinct t3.ID, t3.ID RefRequest, t3.SourceID, t3.RefPeriod, t3.RefTerritory, t4.RefTender, 1 val
from d_StOrder_LotItems t1, d_StOrder_AskItems t2, d_StOrder_Request t3, d_StOrder_Lot t4
where (t1.ID = t2.RefLotItems)
and (t3.ID = t2.RefRequest)
and (t4.ID = t1.RefLot);

/* Количество закупок */
CREATE OR REPLACE VIEW FV_STORDER_TENDER AS
select distinct RefTender ID, RefTender, SourceID, RefPeriod, RefTerritory, 1 val
from fv_StOrder_Request;

/* Количество контрактов */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACT AS
select distinct t4.RefContract ID, t4.RefContract, t3.SourceID, t3.RefPeriod, t3.RefTerritory, 1 val
from d_StOrder_LotItems t1, d_StOrder_AskItems t2, d_StOrder_Request t3, d_StOrder_ContractItems t4
where (t1.ID = t2.RefLotItems)
and (t3.ID = t2.RefRequest)
and (t1.ID = t4.RefLotItems);

/* Количество лотов */
CREATE OR REPLACE VIEW FV_STORDER_LOT AS
select distinct t4.ID, t4.ID RefLot, t3.SourceID, t3.RefPeriod, t3.RefTerritory, t4.RefTender, 1 val
from d_StOrder_LotItems t1, d_StOrder_AskItems t2, d_StOrder_Request t3, d_StOrder_Lot t4 
where (t1.ID = t2.RefLotItems)
and (t3.ID = t2.RefRequest)
and (t4.ID = t1.RefLot);

/* Количество предложений поставщиков */
CREATE OR REPLACE VIEW FV_STORDER_OFFER AS
select distinct t4.RefOffer ID, t4.RefOffer, t3.SourceID, t3.RefPeriod, t3.RefTerritory, 1 val
from d_StOrder_LotItems t1, d_StOrder_AskItems t2, d_StOrder_Request t3, d_StOrder_BidItems t4
where (t1.ID = t2.RefLotItems)
and (t3.ID = t2.RefRequest)
and (t1.ID = t4.RefLotItems);

/* Про организации - таблица базы одна - Организации, измерений по данной таблице много, причем в одном кубе, т.е. перекрестные ссылки. */
/* Создаю представления */
/* Представление для измерения - Организаторы */
CREATE OR REPLACE VIEW DV_STORDER_ORGANIZER (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE, REFORGANIZERBRIDGE) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_Tender Tn where Tn.reforganizer = O.id)
                 or exists (select null from d_Storder_Request Rq where Rq.reforganizer = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp, T.refgrbsbridge, T.RefOrganizerBridge
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Специализированные организации */
CREATE OR REPLACE VIEW DV_STORDER_SPECIALORG (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_Tender Tn where Tn.refsporganizer = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Заказчики */
CREATE OR REPLACE VIEW DV_STORDER_CUSTOMER  (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_Request Rq where Rq.refcustomer = O.id)
                 or exists (select null from d_Storder_Contract Cn where Cn.refcustomer = O.id)
                 or exists (select null from d_Storder_Agreements Ag where Ag.refcustomer = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp, T.refgrbsbridge
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Поставщики */
CREATE OR REPLACE VIEW DV_STORDER_SUPPLIER (ID, CODE, NAME, ADDRESS, REFOKATO, CUBEPARENTID, KPP) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_Offer St where St.refsupplier = O.id)
                 or exists (select null from d_Storder_Contract Cn where Cn.refsupplier = O.id)
                 or exists (select null from d_Storder_Agreements Ag where Ag.refsupplier = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.refokato, T.cubeparentid, T.kpp
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Пользователи (пользователи сайта) */
CREATE OR REPLACE VIEW DV_STORDER_PERSONSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_SiteUser St where St.refperson = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Организации (пользователи сайта) */
CREATE OR REPLACE VIEW DV_STORDER_ORGSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_SiteUser St where St.reforganization = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Представление для измерения - Исполнители */
CREATE OR REPLACE VIEW DV_STORDER_EXECUTOR (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with Dep as (select * 
              from d_StOrder_Organization O
              where exists (select null from d_Storder_Tender Tn where Tn.refexecutor = O.id)
                 or exists (select null from d_Storder_Request Rq where Rq.refexecutor = O.id)
             )
select distinct T.id, T.code, T.name, T.address, T.cubeparentid, T.kpp
  from d_StOrder_Organization  T
  connect by prior T.cubeparentid = T.id 
         and prior T.rowtype <> 2  --избегаем зацикливания, т.к. у головной записи id=cubeparentid (rowtype=2) 
  start with (T.id in (select D.id from Dep D));

/* Аналогичная ситуация с состояниями. Таблица базы одна - Состояния документов, измерений много.*/
/* Представление для измерения - Состояния заявок на закупку */
CREATE OR REPLACE VIEW DV_STORDER_STATUSREQUEST AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Request t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния предложений поставщиков */
CREATE OR REPLACE VIEW DV_STORDER_STATUSOFFER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Offer t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния контрактов */
CREATE OR REPLACE VIEW DV_STORDER_STATUSCONTRACT AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Contract t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния закупок */
CREATE OR REPLACE VIEW DV_STORDER_STATUSTENDER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Tender t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Продукция - берем по заявке*/
/* Вся продукция заявки может еще детализироваться по источнику финансирования, т.у. привожу уровень детализации к продукции */
CREATE OR REPLACE VIEW DV_STORDER_ASKITEMSPR AS
select t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure, 
SUM(t1.Quantity) Quantity, SUM(t1.Price) Price, SUM(t1.Cost) Cost, SUM(t2.AcceptedAmount) AcceptedAmount
from d_StOrder_AskItems t1, d_StOrder_FinanceDistribution t2
where (t1.ID = t2.RefAskItems)
Group by t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure;

/* Выбираю всю информацию уровень детализации - продукция заявки */
CREATE OR REPLACE view TEMP1 (IDSLot, RefLot, IDSSup) as
SELECT distinct SLot.ID, SLot.RefLot, SSup.ID
FROM d_StOrder_LotItems SLot LEFT OUTER JOIN d_StOrder_BidItems SSup ON (SLot.ID = SSup.RefLotItems)
where not SSup.RefLotItems is null
union
SELECT distinct SLot.ID, SLot.RefLot, -1
FROM d_StOrder_LotItems SLot LEFT OUTER JOIN d_StOrder_BidItems SSup ON (SLot.ID = SSup.RefLotItems)
where SSup.RefLotItems is null;

CREATE OR REPLACE view TEMP2 (IDSLot, RefLot, IDSSup, IDSContr) as
SELECT SLot.IDSLot, SLot.RefLot, SLot.IDSSup, SContr.ID 
FROM Temp1 SLot LEFT OUTER JOIN d_StOrder_ContractItems SContr ON (SLot.IDSLot = SContr.RefLotItems)
where not SContr.RefLotItems is null
union
SELECT SLot.IDSLot, SLot.RefLot, SLot.IDSSup, -1 
FROM Temp1 SLot LEFT OUTER JOIN d_StOrder_ContractItems SContr ON (SLot.IDSLot = SContr.RefLotItems)
where SContr.RefLotItems is null;

CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
select t4.ID, 
t1.RefLot,
t2.RefRequest, t2.RefProduct, t2.RefMeasure, t2.RefKVR, t2.RefEKR, t2.Quantity QuantityAsk, t2.Price PriceAsk, t2.Cost CostAsk, t2.AcceptedAmount,
t3.RefContract, t3.Quantity QuantityContract, t3.Price PriceContract, t3.Cost CostContract, 
t4.RefOffer, t4.Quantity QuantityOffer, t4.Price PriceOffer, t4.Cost CostOffer, 
t5.RefTender, 
t6.SourceID, t6.RefPeriod, t6.RefTerritory
from Temp2 t1, dv_StOrder_AskItemsPR t2, d_StOrder_ContractItems t3, d_StOrder_BidItems t4, d_StOrder_Lot t5, d_StOrder_Request t6
where (t1.IDSLot = t2.RefLotItems)
and (t3.ID = t1.IDSContr)
and (t4.ID = t1.IDSSup)
and (t5.ID = t1.RefLot)
and (t6.ID = t2.RefRequest);

/* Start - 8811 - Представления для куба "УФК_Справка органа ФК" - zaharchenko - 03.07.2008 */
CREATE OR REPLACE VIEW DV_KD_UFK_Debet
AS
SELECT *
FROM DV_KD_UFK;

CREATE OR REPLACE VIEW BV_KVSR_BRIDGE_Debet
AS
SELECT *
FROM BV_KVSR_BRIDGE;

CREATE OR REPLACE VIEW BV_KD_BRIDGE_Debet
AS
SELECT *
FROM BV_KD_BRIDGE;

CREATE OR REPLACE VIEW BV_KD_BRIDGEPLAN_Debet
AS
SELECT *
FROM BV_KD_BRIDGEPLAN;

/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW DV_ORG_UFKPAYERS
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM D_ORG_UFKPAYERS T;


/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW bv_org_payersbridge
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_org_payersbridge T;


/* Start - 9590 - ФО_КЦ Система_Доходы-изменить куб, представление для измерения Период.Дата Утверждения - zaharchenko - 21.11.2008 */
CREATE OR REPLACE VIEW dV_fx_date_yeardayunv
AS
SELECT *
FROM fx_date_yeardayunv;


/* Start - 9590 - В измерении КД.КЦ Система поле name расширено до 1000 знаков. - zaharchenko - 26.11.2008 */
/* Представление с полем name, разбитым на 5 частей для измерения "КД.КЦ Система" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW DV_KD_KCSYSTEMA_bridge
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4, substr(T.Name, 1021, 255) as short_name_part5
FROM D_KD_KCSYSTEMA T;

/* End - xxxx - Представления для кубов блока Мониторинг государственных закупок - dianova - 29.09.2008 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 16.02.2009 */
/* Удалить когда будет закачка по нормальной постановке */

/* Таблицы измерений */
/* Про организации - таблица базы одна - Организации, измерений по данной таблице много, причем в одном кубе, т.е. перекрестные ссылки. */
/* Создаю представления */

/* Аналогичная ситуация с состояниями. Таблица базы одна - Состояния документов, измерений много.*/
/* Представление для измерения - Состояния заявок на закупку */
CREATE OR REPLACE VIEW DV_STORDER_STATUSREQUEST AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Request t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния предложений поставщиков */
CREATE OR REPLACE VIEW DV_STORDER_STATUSOFFER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Offer t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния контрактов */
CREATE OR REPLACE VIEW DV_STORDER_STATUSCONTRACT AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Contract t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния закупок */
CREATE OR REPLACE VIEW DV_STORDER_STATUSTENDER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Tender t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Таблицы фактов */
/* Количество заявок */
CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select distinct t3.ID, t3.ID RefRequest, t3.SourceID, t3.RefCustomer, t3.RefPeriod, t3.RefTerritory, t3.RefDocStatus, t3.RefTypeTender, t4.RefTender, 1 val
from d_StOrder_LotItems t1, d_StOrder_AskItems t2, d_StOrder_Request t3, d_StOrder_Lot t4
where (t1.ID = t2.RefLotItems)
and (t3.ID = t2.RefRequest)
and (t4.ID = t1.RefLot);

/* Создаю вспомогательное представление */
CREATE OR REPLACE VIEW FV_STORDER_ST AS
select distinct t.SourceID, t.RefTerritory
from d_StOrder_Request t;

/* Количество закупок */
CREATE OR REPLACE VIEW FV_STORDER_TENDER AS
select distinct t1.ID, t1.ID RefTender, t1.SourceID, t1.RefTypeTender, t1.RefDocStatus, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefPeriod, t3.RefTerritory, 1 val, Sum(t2.LotCost) LotCost
from d_StOrder_Tender t1 LEFT OUTER JOIN  d_Storder_Lot t2 ON (t1.Id = t2.RefTender) JOIN fv_StOrder_ST t3 ON (t1.SourceId=t3.SourceID)
Group by t1.ID, t1.SourceID, t1.RefTypeTender, t1.RefDocStatus, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefPeriod, t3.RefTerritory;

/* Количество контрактов */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACT AS
select distinct t1.ID, t1.ID RefContract, t1.SourceID, t1.RefYearDayUNV, t1.RefDocStatus, t1.RefCustomer, t1.RefSupplier, t2.RefTerritory, 1 val
from d_StOrder_Contract t1 JOIN fv_StOrder_ST t2 ON (t1.SourceId = t2.SourceId);

/* Количество лотов */
CREATE OR REPLACE VIEW FV_STORDER_LOT AS
select distinct t1.ID, t1.ID RefLot, t1.SourceID, t1.RefTender, t2.RefTerritory, t1.LotCost, 1 val
from d_StOrder_Lot t1 JOIN fv_StOrder_ST t2 ON (t1.SourceId = t2.SourceId);

/* Источники финансирования */
CREATE OR REPLACE VIEW FV_STORDER_FINANCEDISTRIB AS
select t1.id, t1.refkif, t1.acceptedamount, t2.refproduct, t2.refrequest
from d_Storder_Financedistribution t1 JOIN d_storder_askitems t2 ON (t1.Refaskitems=t2.id);

/* Мониторинг */
/* Вся продукция заявки может еще детализироваться по источнику финансирования, п.у. привожу уровень детализации к продукции */

CREATE OR REPLACE VIEW DV_STORDER_ASKITEMSPR AS
select t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure, 
SUM(t1.Quantity) Quantity, SUM(t1.Price) Price, SUM(t1.Cost) Cost, SUM(t2.AcceptedAmount) AcceptedAmount
from d_StOrder_AskItems t1 LEFT OUTER JOIN d_StOrder_FinanceDistribution t2 ON (t1.ID = t2.RefAskItems)
Group by t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure;

/* Мониторинг предложений поставщиков */
CREATE OR REPLACE VIEW FV_STORDER_OFFER AS
select t1.ID, t1.RefOffer, t1.Quantity QuantityOffer, t1.Price PriceOffer, t1.Cost CostOffer, 
t2.RefRequest, t2.RefProduct, t2.Quantity QuantityAsk, t2.Price PriceAsk, t2.Cost CostAsk
from d_StOrder_BidItems t1 JOIN DV_STORDER_ASKITEMSPR t2 ON (t1.RefLotItems = t2.RefLotItems)
where t1.ReflotItems <> -1;

/* Минимальное предложение поставщиков по строке лота */
CREATE OR REPLACE view DV_STORDER_LOTITEMSMINPRICE AS
select t.Reflotitems, Min(t.Price) as MinPrice
from d_Storder_Biditems t
where t.Reflotitems <> -1
Group by  t.Reflotitems;

/* Вспомогательное представление для связи строк лотов с контрактами и предложениями */
CREATE OR REPLACE view FV_STORDER_STRLOTOFFERCONTRACT AS
select t1.Id, t1.RefLot, t2.MinPrice, t3.RefContract, t3.Quantity, t3.Price, t3.Cost
from d_StOrder_LotItems t1 LEFT OUTER JOIN Dv_Storder_LotitemsMinPrice t2 ON (t1.id = t2.Reflotitems) LEFT OUTER JOIN d_Storder_Contractitems t3 ON (t1.id=t3.reflotitems)
where (not t3.RefLotItems is null) and (t3.RefLotItems<>-1) 
union
select t1.Id, t1.RefLot, t2.MinPrice, -1, null, null, null
from d_StOrder_LotItems t1 LEFT OUTER JOIN Dv_Storder_LotitemsMinPrice t2 ON (t1.id = t2.Reflotitems) LEFT OUTER JOIN d_Storder_Contractitems t3 ON (t1.id=t3.reflotitems)
where t3.RefLotItems is null
union
select t1.Id, t1.RefLot,null,-1, null, null, null
from d_StOrder_LotItems t1
where t1.id=-1;

CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
select 
t1.RefRequest, t1.RefProduct, t1.RefMeasure, t1.RefKVR, t1.RefEKR, t1.Quantity QuantityAsk, t1.Price PriceAsk, t1.Cost CostAsk, t1.AcceptedAmount,
t2.SourceID, t2.RefPeriod, t2.RefTerritory,
t3.RefLot, t3.RefContract,t3.MinPrice, t3.Quantity QuantityContract, t3.Price PriceContract, t3.Cost CostContract, 
t4.RefTender
from dv_StOrder_AskItemsPR t1, d_StOrder_Request t2, fv_StOrder_StrLotOfferContract t3, d_StOrder_Lot t4 
where (t2.ID = t1.RefRequest)
and (t3.ID = t1.RefLotItems)
and (t4.ID = t3.RefLot);

Drop view TEMP1;
Drop view TEMP2;

/* End - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 16.02.2009 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 18.02.2009 */
/* Удалить когда будет закачка по нормальной постановке */

/* Количество закупок */
CREATE OR REPLACE VIEW FV_STORDER_TENDER AS
select distinct t1.ID, t1.ID RefTender, t1.SourceID, t1.RefTypeTender, t1.RefDocStatus, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefPeriod, t1.RefTerritory, 1 val, Sum(t2.LotCost) LotCost
from d_StOrder_Tender t1 LEFT OUTER JOIN  d_Storder_Lot t2 ON (t1.Id = t2.RefTender)
Where t1.ID <> -1
Group by t1.ID, t1.SourceID, t1.RefTypeTender, t1.RefDocStatus, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefPeriod, t1.RefTerritory;

/* Количество контрактов */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACT AS
select distinct t1.ID, t1.ID RefContract, t1.SourceID, t1.RefYearDayUNV, t1.RefDocStatus, t1.RefCustomer, t1.RefSupplier, t1.RefTerritory, 1 val, Sum(t2.Cost) ContractCost
from d_StOrder_Contract t1 LEFT OUTER JOIN  d_Storder_ContractItems t2 ON (t1.Id = t2.RefContract)
Where t1.ID <> -1
Group by t1.ID, t1.SourceID, t1.RefYearDayUNV, t1.RefDocStatus, t1.RefCustomer, t1.RefSupplier, t1.RefTerritory;

/* Количество лотов */
CREATE OR REPLACE VIEW FV_STORDER_LOT AS
select distinct t1.ID, t1.ID RefLot, t1.SourceID, t1.RefTender, t1.LotCost, 1 val
from d_StOrder_Lot t1
where t1.ID <> -1;

Drop view FV_STORDER_ST;

/* End - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 18.02.2009 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 19.03.2009 */
/* Удалить когда будет закачка по нормальной постановке */

/* Количество заявок */
CREATE OR REPLACE VIEW FV_STORDER_REQUESTADD AS
select distinct t1.Refrequest, t3.reftender
from d_StOrder_AskItems t1 JOIN d_Storder_Lotitems t2 ON (t1.reflotitems = t2.ID) JOIN d_Storder_Lot t3 ON (t2.reflot=t3.id);

CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select t1.ID, t1.ID RefRequest, t1.SourceID, t1.RefCustomer, t1.RefPeriod, t1.RefTerritory, t1.RefDocStatus, t1.RefTypeTender, t2.RefTender, 1 val
from d_StOrder_Request t1 JOIN FV_STORDER_REQUESTADD t2 ON (t2.Refrequest=t1.id);

/* End - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 19.03.2009 */

/* Start - xxxx - Представления для измерения Ответственный и куба Организации_Цены и тарифы - dianova - 04.02.2009 */


CREATE OR REPLACE VIEW DV_ORG_RESPON_BRIDGE AS 
select ID, NAME
from D_ORG_RESPON;

CREATE OR REPLACE VIEW DV_ORG_REGISTRORG AS
select *
from D_ORG_REGISTRORG;

CREATE OR REPLACE VIEW DV_OK_OKFS AS
select *
from D_OK_OKFS;

CREATE OR REPLACE VIEW DV_OK_OKOPF AS
select *
from D_OK_OKOPF;

/* End - xxxx - Представления для измерения Ответственный и куба Организации_Цены и тарифы - dianova - 04.02.2009 */

/* Start - 9892 - Представления для куба РЕГИОН_Оценка ЭРБС - dianova - 07.04.2009 */

/* ЭРБС.Показатели */

CREATE OR REPLACE VIEW DV_ERBS_MARKS (ID, CODESTR, NAME, REFGROUP, REFTREND, REFUNITS, GROUPNAME, GROUPCODESTR, GROUPCOEFF, TRENDNAME, TRENDCODESTR) AS
select t1.id, t1.codestr, t1.name, t1.referbsgroupofmarks, t1.referbs, t1.refunits, t2.name , t2.codestr, t2.coeff, t3.name, t3.codestr
from d_Erbs_Marks t1 JOIN d_Erbs_Groupofmarks t2 ON (t1.referbsgroupofmarks=t2.id) JOIN d_Erbs_Trend t3 ON (t1.referbs=t3.id);

/* End - 9892 - Представления для куба РЕГИОН_Оценка ЭРБС - dianova - 07.04.2009 */

/* Start - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */

CREATE OR REPLACE VIEW BV_KD_BRIDGE
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_KD_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '5cd4f631-6276-4a9f-b466-980282500b50' and t2.IsCurrent = 1) or ID = -1;

/* End - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */

/* Start - 10815 -  Кубы для мониторинга ФП и КУ (представления для свойств в измерениях)  - zaharchenko - 02.06.2009 */

create or replace view fv_fx_verificatconditions_max
as select *
from fx_fx_verificationconditions;

create or replace view fv_fx_verifcond_bridge_min
as select *
from fx_fx_verificationconditions;

create or replace view fv_fx_verifcond_bridge_max
as select *
from fx_fx_verificationconditions;


/* End - 10815 -  Кубы для мониторинга ФП и КУ (представления для свойств в измерениях)  - zaharchenko - 02.06.2009 */

/* Start - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */

CREATE OR REPLACE VIEW Bv_Kd_Bridgeplan
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_Kd_Bridgeplan T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '65fefc22-6135-4ee0-8fc3-3801a368991a' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Bv_r_bridge
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0a626485-8481-4058-aa0f-a917df395f3c' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Bv_r_Bridgeplan
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgeplan T;

CREATE OR REPLACE VIEW Bv_r_Bridgerep
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgerep T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '8319a6d9-2adf-417d-9f93-8b0c12ec071c' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Dv_r_Plan
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_r_Plan T;

CREATE OR REPLACE VIEW Dv_r_Admprojectoutcome
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_r_Admprojectoutcome T;

CREATE OR REPLACE VIEW BV_KIF_BRIDGE
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, kvsr, descriptioncode, maindescriptioncode, itemcode, programcode, kesr, parentid, refdirection, refclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.KVSR, T.DescriptionCode, T.MainDescriptionCode, T.ItemCode, T.ProgramCode, T.KESR, T.ParentID, T.RefDirection, T.RefClsAspect
, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM b_KIF_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0773168f-923d-4140-98cc-01328f353e40' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KIF_BRIDGEPLAN
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, parentid, refkif, refkifclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.ParentID, T.RefKIF, T.RefKIFClsAspect
, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM b_KIF_BridgePlan T;

CREATE OR REPLACE VIEW BV_KCSR_BRIDGE
(id, rowtype, sourceid, code, code1, code2, code3, name, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID
, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_KCSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'ee980110-fa1f-43c4-b03e-b92c6fec5035' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW DV_R_FO9KINDS
(id, rowtype, code, name, note, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T."ROWTYPE", T.Code, T.Name, T.Note, T.ParentID
, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_R_FO9Kinds T;

CREATE OR REPLACE VIEW dv_r_Fo14grbs AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_r_Fo14grbs T;

/* End - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */

/* Start - 8924 -  Представление для измерения Период.Дата принятия  - zaharchenko - 21.07.2009 */

CREATE OR REPLACE VIEW fv_date_yeardayunv AS
SELECT *
FROM fx_date_yeardayunv;

/* End - 8924 -  Представление для измерения Период.Дата принятия  - zaharchenko - 21.07.2009 */

/* Start -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

CREATE OR REPLACE VIEW DV_ORG_PRODUCT AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 2;

CREATE OR REPLACE VIEW DV_ORG_MARKET AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 1;

commit work;

/* End -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

/* Start -  - Добавлен модуль для прогноза развития региона - chubov - 03.02.2009 */

create or replace view fv_forecast_scenario_data (valuebase, valueestimate, valuey1, valuey2, valuey3, valuey4, valuey5, minbound, maxbound, refscenario, refparams) as
select null as valuebase, adj.valueestimate, adj.valuey1, adj.valuey2, adj.valuey3, adj.valuey4, adj.valuey5, adj.minbound, adj.maxbound, adj.refscenario, adj.refparams
from t_forecast_adjvalues adj
union
select null as valuebase, ind.valueestimate, ind.valuey1, ind.valuey2, ind.valuey3, ind.valuey4, ind.valuey5, ind.minbound, ind.maxbound, ind.refscenario, ind.refparams
from t_forecast_indvalues ind
union
select st.valuebase, st.valueestimate, null as valuey1, null as valuey2, null as valuey3, null as valuey4, null as valuey5, null as minbound, null as maxbound, st.refscenario, st.refparams
from t_forecast_staticvalues st
union
select null as valuebase, ur.valueestimate, ur.valuey1, ur.valuey2, ur.valuey3, ur.valuey4, ur.valuey5, null as minbound, null as maxbound, ur.refscenario, ur.refparams
from t_forecast_unregadj ur;

create or replace view v_forecast_val_indicators as
select d.id as id, t.refscenario as basescenario, d.refscenario as refscenario, t.valuebase as valuebase,
t.valueestimate as valueestimate, NULLIF (d.valueestimate, t.valueestimate) as v_est_b,
t.valuey1 as valuey1, NULLIF (d.valuey1, t.valuey1)  as v_y1_b,
t.valuey2 as valuey2, NULLIF (d.valuey2, t.valuey2)  as v_y2_b,
t.valuey3 as valuey3, NULLIF (d.valuey3, t.valuey3)  as v_y3_b,
t.valuey4 as valuey4, NULLIF (d.valuey4, t.valuey4) as v_y4_b,
t.valuey5 as valuey5, NULLIF (d.valuey5, t.valuey5) as v_y5_b,
d.minbound as minbound, d.maxbound as maxbound,
d.leftpenaltycoef as leftpenaltycoef, d.rightpenaltucoef as rightpenaltucoef,
t.userid as userid, t.refparams as refparams,
( 
  select designation 
  from d_units_okei o 
  where p.refunits = o.id
) as designation,
( 
  select
  (  
     select Name
     from d_Forecast_Parametrs i
     where substr(i.Code, 1, 1) = substr(o.code, 1, 1) and substr(i.Code, 2, 2) = substr(o.Code, 2, 2) and substr(i.Code, 5, 2) = '00'
  )
  from d_Forecast_Parametrs o
  where o.id = t.refparams
) as GroupName
from t_forecast_indvalues t join t_forecast_indvalues d on (t.refparams = d.refparams) and (t.refscenario<>d.refscenario)
join d_forecast_parametrs p on (p.id = t.refparams);


create or replace view v_forecast_val_adjusters as
select d.id as id, t.refscenario as basescenario, d.refscenario as refscenario, t.valuebase as valuebase,
t.valueestimate as valueestimate, NULLIF (d.valueestimate, t.valueestimate) as v_est_b,
t.valuey1 as valuey1, NULLIF (d.valuey1, t.valuey1)  as v_y1_b,
t.valuey2 as valuey2, NULLIF (d.valuey2, t.valuey2)  as v_y2_b,
t.valuey3 as valuey3, NULLIF (d.valuey3, t.valuey3)  as v_y3_b,
t.valuey4 as valuey4, NULLIF (d.valuey4, t.valuey4) as v_y4_b,
t.valuey5 as valuey5, NULLIF (d.valuey5, t.valuey5) as v_y5_b,
d.minbound as minbound, d.maxbound as maxbound,
t.userid as userid, t.refparams as refparams,
(
  select designation 
  from d_units_okei o 
  where p.refunits = o.id
) as designation,
( 
  select
  (  
     select Name
     from d_Forecast_Parametrs i
     where substr(i.Code, 1, 1) = substr(o.code, 1, 1) and substr(i.Code, 2, 2) = substr(o.Code, 2, 2) and substr(i.Code, 5, 2) = '00'
  )
  from d_Forecast_Parametrs o
  where o.id = t.refparams
) as GroupName
from t_forecast_adjvalues t join t_forecast_adjvalues d on (t.refparams = d.refparams) and (t.refscenario<>d.refscenario)
join d_forecast_parametrs p on (p.id = t.refparams);


create or replace view dv_forecast_parametrs
(id, rowtype, code, name, refunits, description, signat, groupecode, groupename)
as
select d.id, d.rowtype, d.code, d.name, d.refunits, d.description, d.signat,
(Trunc(d.code / 100000,0)) as groupcode,
(case Trunc(d.code / 100000,0)
 when 1 then 'Индикаторы'
 when 2 then 'Регуляторы'
 when 3 then 'Статистические параметры'
 when 5 then 'Нерегулируемые параметры'
end) as groupname
from d_forecast_parametrs d;
 
commit;

/* End - Добавлен модуль для прогноза развития региона - chubov - 03.02.2009 */

/* Start - 11693 - Представление на Районы.Планирование - zaharchenko - 02.10.2009 */

CREATE OR REPLACE VIEW DV_REGIONS_PLAN_Bridge
AS
SELECT *
FROM DV_REGIONS_PLAN;

commit;

/* End - 11693 - Представление на Районы.Планирование - zaharchenko - 02.10.2009 */

/* Start -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

CREATE OR REPLACE VIEW DV_ORG_PRODUCT AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 2;

CREATE OR REPLACE VIEW DV_ORG_MARKET AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 1;

commit work;

/* End -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.01.2010 */
/* Удалить когда будет закачка по нормальной постановке */

/* Таблицы измерений */

/* Про организации - таблица базы одна - Организации, измерений по данной таблице много, причем в одном кубе, т.е. перекрестные ссылки. */



/* Аналогичная ситуация с состояниями. Таблица базы одна - Состояния документов, измерений много.*/

/* Представление для измерения - Состояния заявок на закупку */
CREATE OR REPLACE VIEW DV_STORDER_STATUSREQUEST AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Request t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния предложений поставщиков */
CREATE OR REPLACE VIEW DV_STORDER_STATUSOFFER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Offer t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния контрактов */
CREATE OR REPLACE VIEW DV_STORDER_STATUSCONTRACT AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Contract t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния закупок */
CREATE OR REPLACE VIEW DV_STORDER_STATUSTENDER AS 
SELECT distinct  t2.ID, t2.Code, t2.Name
FROM d_StOrder_Tender t1, d_StOrder_DocumentsStatus t2
WHERE t2.ID = t1.RefDocStatus;

/* Представление для измерения - Состояния сведений об ИспПрДействия */
CREATE OR REPLACE VIEW DV_STORDER_STATUSORDEREXE AS 
SELECT t2.ID, t2.Code, t2.Name
FROM d_StOrder_DocumentsStatus t2
WHERE t2.ID in (select distinct t1.RefDocStatus from d_StOrder_OrderExecution t1);

/* Представление для измерений класса ДаНет */
CREATE OR REPLACE VIEW DV_STORDER_YESNO AS 
select * from dv.fx_storder_smbusiness t
where t.id > -1;

/* Представление для измерений - Аккредитован (ДаНет) */
CREATE OR REPLACE VIEW DV_STORDER_YNACCRED AS 
select * from dv.fx_storder_smbusiness t
where t.id > -1;

/* Представление для измерения - Тип заявки */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTTYPE AS 
select t.id, 1 Code, 'Заявка на закупку' Name  from d_storder_doctype t where t.code in (205,258)
union
select t.id, 2 Code, 'План закупок' Name from d_storder_doctype t where t.code in (672,673);

/* Представление для измерения - Способы закупок - приходится делать представление, т.к. в таблице свалка */
CREATE OR REPLACE VIEW DV_STORDER_DOCTYPETENDER AS
select t1.Id, cast(t1.name as VARCHAR2(255)) Name
from d_StOrder_DocType t1
where t1.Id in (select distinct d_StOrder_Request.RefTypeTender from d_StOrder_Request)
   or t1.Id in (select distinct d_StOrder_Tender.RefTypeTender from d_StOrder_Tender)
   or t1.Id in (select distinct d_StOrder_Contract.RefTypeTender from d_StOrder_Contract);

/* Представление для измерения - Категории заказчиков */
CREATE OR REPLACE VIEW DV_STORDER_CATEGORYCUSTOMER (ID, SOURCEID, CODESTR, NAME, DATASOURCENAME, REFORGANIZATION) AS 
SELECT distinct T.ID, T.SourceID, T.CodeStr, T.Name,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END,
T.RefOrganization
FROM d_StOrder_CategoryOrganization T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)
WHERE t.RefOrganization in (select t2.Id from DV_STORDER_CUSTOMER t2);

/* Представление для измерения - Категории поставщиков */
CREATE OR REPLACE VIEW DV_STORDER_CATEGORYSUPPLIER (ID, SOURCEID, CODESTR, NAME, DATASOURCENAME, REFORGANIZATION) AS 
SELECT distinct T.ID, T.SourceID, T.CodeStr, T.Name,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END,
T.RefOrganization
FROM d_StOrder_CategoryOrganization T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)
WHERE t.RefOrganization in (select t2.Id from DV_STORDER_SUPPLIER t2);
   
/* Представление для измерения - Преимущества */
CREATE OR REPLACE VIEW DV_STORDER_PREFERENCES (ID, NAME) AS 
SELECT distinct t.reftenderpreferences, t.tenderpreferences
FROM d_StOrder_Contract t;

/* Представление для измерения - Дата публикации */
CREATE OR REPLACE VIEW DV_DATE_PUBLICATION AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата вскрытия */
CREATE OR REPLACE VIEW DV_DATE_OPENING AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата рассмотрения */
CREATE OR REPLACE VIEW DV_DATE_CONSIDER AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата оценки и сопоставления */
CREATE OR REPLACE VIEW DV_DATE_MATCHING AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата принятия сведений */
CREATE OR REPLACE VIEW DV_DATE_ACCEPT AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата регистрации */
CREATE OR REPLACE VIEW DV_DATE_REGISTER AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата блокировки */
CREATE OR REPLACE VIEW DV_DATE_DISCARD AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата договора */
CREATE OR REPLACE VIEW DV_DATE_AGREEMENT AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата завершения договора */
CREATE OR REPLACE VIEW DV_DATE_AGREEMENTEND AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата ИспПрДействия */
CREATE OR REPLACE VIEW DV_DATE_CONTRACTEND AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата договора */
CREATE OR REPLACE VIEW DV_DATE_AGREEMENT AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата завершения договора */
CREATE OR REPLACE VIEW DV_DATE_AGREEMENTEND AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Виды документации */
CREATE OR REPLACE VIEW DV_STORDER_TENDERFILE AS 
select t.id, cast(t.name as VARCHAR2(255)) Name 
from d_storder_doctype t 
where t.id in (select distinct t2.RefDocType from d_storder_tenderfiles t2);

/* Представление для измерения - Решения */
CREATE OR REPLACE VIEW DV_STORDER_RESOLVEDLOT AS 
select distinct t.Code ID, t.Name from d_storder_resolvedlot t;

/* Представление для измерения Типы протокола */
CREATE OR REPLACE VIEW DV_STORDER_TYPEPROTOCOL AS 
select t1.id, cast(t1.name as VARCHAR2(255)) Name 
from d_Storder_Doctype t1
where t1.id in ( select distinct t2.reftypeprotocol from d_Storder_Protocol t2);

/* Представление для измерения Место предложения */
CREATE OR REPLACE VIEW DV_STORDER_SPRIORITY AS
select distinct t1.Spriority ID 
from d_storder_PurchaseLotBids t1;

/* Таблицы фактов */

/* Заявки */

CREATE OR REPLACE VIEW DV_STORDER_REQUESTTENDER AS
select distinct t1.Refrequest, t3.reftender
from d_StOrder_AskItems t1 JOIN d_Storder_Lotitems t2 ON (t1.reflotitems = t2.ID) JOIN d_Storder_Lot t3 ON (t2.reflot=t3.id)
where t3.reftender > 0; 

/* Создаю представление для измерения заявок расширенное закупкой */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTS AS
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.ForSmallBusiness, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t2.RefTender
FROM DV_STORDER_REQUEST t JOIN DV_STORDER_REQUESTTENDER t2 ON (t.ID = t2.RefRequest)
union
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.ForSmallBusiness, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, -1 RefTender
FROM DV_STORDER_REQUEST t 
where not t.id  in (select t2.Refrequest from DV_STORDER_REQUESTTENDER t2);

/* Количество заявок */
CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select t.Id, t.Id RefRequest, t.ForSmallBusiness, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.RefTender, 1 Val
from DV_STORDER_REQUESTS t
where t.ID>0;

/* Определяю финансирование строки заявки */
CREATE OR REPLACE VIEW FV_STORDER_ASKITEMFIN AS
select t.Refaskitems ID, Sum(t.acceptedamount) AcceptedAmount
from d_Storder_Financedistribution t
where t.acceptedamount>0
Group by t.Refaskitems;

/* Соединяю строки заявок с финансированием */
CREATE OR REPLACE VIEW FV_STORDER_ASKITEM AS
select t.Id, t.quantity, t.price, t.cost, t.refrequest, t.refproduct, t.refekr, t.refkvr, t.refmeasure, t2.acceptedamount
from d_storder_askitems t LEFT OUTER JOIN FV_STORDER_ASKITEMFIN t2 ON (t.id=t2.ID)
where t.Id>0;

/* Закупки */

/* Вспомогательное. В результате множество записей - лот - последнее решение по нему (определяю по макс. ID протокола) */
CREATE OR REPLACE VIEW FV_STORDER_TENDERRESOLV  AS
select t.RefLot, Max(t.RefProtocol) RefProtocol, Count(t.RefProtocol) ProtocolCount 
from d_Storder_Resolvedlot t 
where t.id>0
group by t.RefLot;

/* Вспомогательное. В результате таблица лотов, только расширенная столбцом Code - последнее решение по лоту*/
CREATE OR REPLACE VIEW FV_STORDER_TENDERRESOLVLOT AS
select t1.Id, t1.reftender, t1.lotcost, t2.code, t4.reftypeprotocol, t3.ProtocolCount
from d_Storder_Lot t1, d_Storder_Resolvedlot t2, fv_StOrder_TenderResolv t3, d_storder_protocol t4
WHERE (t1.ID=t2.reflot) AND (t2.reflot=t3.reflot) AND (t2.refprotocol=t3.refprotocol) AND (t3.RefProtocol=t4.id)
union
select t4.ID , t4.reftender, t4.lotcost, 0 Code, -1 reftypeprotocol, 0 ProtocolCount
from d_Storder_Lot t4
WHERE not t4.id in (select t5.reflot from FV_STORDER_TENDERRESOLV t5 );

/* Закупки */
CREATE OR REPLACE VIEW FV_STORDER_TENDER  AS
select t2.RefPeriod, t2.RefTerritory, t2.ID RefTender, t2.RefOrganizer, t2.RefSPOrganizer, t2.RefDocStatus, t2.RefTypeTender, 
t2.forsmallbusiness, t2.refdatepublication, t2.refdateopening, t2.refdateconsider, t2.refdatematching, t2.refexecutor,
t1.id RefLot, t1.reftypeprotocol, t1.code, t1.ProtocolCount, t1.LotCost, 1 val
from  FV_STORDER_TENDERRESOLVLOT t1 JOIN d_StOrder_Tender t2 ON (t1.reftender = t2.ID)
Where t1.ID > 0
union
select t1.RefPeriod, t1.RefTerritory, t1.ID RefTender, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefDocStatus, t1.RefTypeTender, 
t1.ForSmallBusiness, t1.RefDatePublication, t1.RefDateOpening, t1.RefDateConsider, t1.RefDateMatching, t1.RefExecutor,
-1 RefLot, -1 reftypeprotocol, 0 code, 0 ProtocolCount, 0 LotCost, 0 val
from d_StOrder_Tender t1 
Where not t1.id  in (select t2.reftender from d_Storder_Lot t2);

/* Контракты */
/* Таблицу D_StOrder_Contract надо расширить стоимостью контрактов и сведением о прекращении, при этом сохранив количество строк*/
/* П.у. две вспомогательных вьюшки FV_STORDER_CONTRACTCOST, FV_STORDER_CONTRACTWITHCOST в итоге FV_STORDER_CONTRACT */

/* Определяю стоимость контрактов */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACTCOST AS
select RefContract ID, Sum(Cost) ContractCost
from d_Storder_Contractitems
Group by RefContract;

/* Соединяю контракты со стоимостью */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACTWITHCOST AS
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer, t1.refsupplier, t1.Id RefContract, t1.RefCause, t2.ContractCost
from d_StOrder_Contract t1 JOIN FV_STORDER_CONTRACTCOST t2 ON (t1.Id = t2.ID)
union
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer,t1.refsupplier, t1.Id RefContract, t1.RefCause, 0 ContractCost
from d_StOrder_Contract t1 
where not t1.id  in (select t2.ID from FV_STORDER_CONTRACTCOST t2);

/* Соединяю контракты с информацией о прекращении и исполнении */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACT AS
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer, t1.refsupplier, t1.Id RefContract, t1.RefCause, t1.ContractCost,
t2.refdocstatus, t2.refexecutiondate, t2.refexecutiontype, t2.cancellationcause, t2.actualpayment, 1 Val
from FV_STORDER_CONTRACTWITHCOST t1 JOIN d_StOrder_OrderExecution t2  ON (t1.Id = t2.RefContract)
and (t1.Id > 0)
union
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer, t1.refsupplier, t1.Id RefContract, t1.RefCause, t1.ContractCost,
-1 refdocstatus, -1 refexecutiondate, -1 refexecutiontype, null, null, 1 Val
from FV_STORDER_CONTRACTWITHCOST t1
where not t1.id  in (select t2.RefContract from d_StOrder_OrderExecution t2)
and (t1.Id > 0);

/* Лоты */
CREATE OR REPLACE VIEW FV_STORDER_LOT AS
select distinct t1.ID, t1.ID RefLot, t1.SourceID, t1.RefTender, t1.LotCost, 1 val
from d_StOrder_Lot t1
where t1.ID <> -1;

/* Источники финансирования */
CREATE OR REPLACE VIEW FV_STORDER_FINANCEDISTRIB AS
select t1.refkif, t1.acceptedamount, t2.refproduct, t2.refrequest, t2.refekr, t2.refkvr, t2.refmeasure, t3.refperiod, t3.refterritory
from d_Storder_Financedistribution t1 JOIN d_storder_askitems t2 ON (t1.Refaskitems=t2.id) JOIN d_storder_request t3 ON (t2.refrequest=t3.id);

/* Сопоставляю заявки и предложения, уровень детализации - продукция */
CREATE OR REPLACE VIEW DV_STORDER_BIDITEMSREQUEST AS
select t1.ID, t1.RefOffer, t1.RefProduct, t1.Quantity QuantityOffer, t1.Price PriceOffer, t1.Cost CostOffer, 
t2.RefRequest, t2.Quantity QuantityAsk, t2.Price PriceAsk, t2.Cost CostAsk
from d_StOrder_BidItems t1 JOIN d_StOrder_AskItems t2 ON (t1.RefLotItems = t2.RefLotItems)
where t2.RefLotItems >0
union
select t1.ID, t1.RefOffer, t1.RefProduct, t1.Quantity QuantityOffer, t1.Price PriceOffer, t1.Cost CostOffer, 
-1, 0 QuantityAsk, 0 PriceAsk, 0 CostAsk
from d_StOrder_BidItems t1 
where not t1.reflotitems in (select t2.reflotitems from D_STORDER_ASKITEMS t2);

/* Сопоставляю закупки и предложения, уровень детализации - продукция */
CREATE OR REPLACE VIEW DV_STORDER_BIDITEMSTENDER AS
select t1.ID, t3.ID RefLot, t4.ID RefTender, t4.refperiod, t4.refterritory
from d_StOrder_BidItems t1 JOIN d_Storder_LotItems t2 ON (t1.RefLotItems=t2.ID)
JOIN d_Storder_Lot t3 ON (t2.RefLot=t3.ID) 
JOIN d_Storder_Tender t4 ON (t3.RefTender=t4.ID);

CREATE OR REPLACE VIEW FV_STORDER_OFFER AS
select 
t1.ID, t1.RefOffer, t1.RefProduct, t1.QuantityOffer, t1.PriceOffer, t1.CostOffer,
t1.RefRequest, t1.QuantityAsk, t1.PriceAsk, t1.CostAsk, 
t2.RefLot, t2.RefTender, t2.refperiod, t2.refterritory,
t3.Spriority, t3.RefUsal
from dv_storder_BidItemsRequest t1, dv_storder_BidItemsTender t2, d_storder_PurchaseLotBids t3
where t1.ID=t2.ID
and t3.RefOffer = t1.RefOffer
and t3.RefLot = t2.RefLot;

/* Мониторинг */
/* Вся продукция заявки может еще детализироваться по источнику финансирования, п.у. привожу уровень детализации к продукции */
CREATE OR REPLACE VIEW DV_STORDER_ASKITEMSPR AS
select t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure, 
SUM(t1.Quantity) Quantity, SUM(t1.Price) Price, SUM(t1.Cost) Cost, SUM(t2.AcceptedAmount) AcceptedAmount
from d_StOrder_AskItems t1 LEFT OUTER JOIN d_StOrder_FinanceDistribution t2 ON (t1.ID = t2.RefAskItems)
Group by t1.RefRequest, t1.RefProduct, t1.RefLotItems, t1.RefEKR, t1.RefKVR, t1.RefMeasure;

/* Минимальное предложение поставщиков по строке лота */
CREATE OR REPLACE view DV_STORDER_LOTITEMSMINPRICE AS
select t.Reflotitems, Min(t.Price) as MinPrice
from d_Storder_Biditems t
where t.Reflotitems <> -1
Group by  t.Reflotitems;

/* Вспомогательное представление для связи строк лотов с контрактами и предложениями */
CREATE OR REPLACE view FV_STORDER_STRLOTOFFERCONTRACT AS
select t1.Id, t1.RefLot, t2.MinPrice, t3.RefContract, t3.Quantity, t3.Price, t3.Cost
from d_StOrder_LotItems t1 LEFT OUTER JOIN Dv_Storder_LotitemsMinPrice t2 ON (t1.id = t2.Reflotitems) LEFT OUTER JOIN d_Storder_Contractitems t3 ON (t1.id=t3.reflotitems)
where (not t3.RefLotItems is null) and (t3.RefLotItems<>-1) 
union
select t1.Id, t1.RefLot, t2.MinPrice, -1, null, null, null
from d_StOrder_LotItems t1 LEFT OUTER JOIN Dv_Storder_LotitemsMinPrice t2 ON (t1.id = t2.Reflotitems) LEFT OUTER JOIN d_Storder_Contractitems t3 ON (t1.id=t3.reflotitems)
where t3.RefLotItems is null
union
select t1.Id, t1.RefLot,null,-1, null, null, null
from d_StOrder_LotItems t1
where t1.id=-1;

CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
select 
t1.RefRequest, t1.RefProduct, t1.RefMeasure, t1.RefKVR, t1.RefEKR, t1.Quantity QuantityAsk, t1.Price PriceAsk, t1.Cost CostAsk, t1.AcceptedAmount,
t2.SourceID, t2.RefPeriod, t2.RefTerritory,
t3.RefLot, t3.RefContract,t3.MinPrice, t3.Quantity QuantityContract, t3.Price PriceContract, t3.Cost CostContract, 
t4.RefTender
from dv_StOrder_AskItemsPR t1, d_StOrder_Request t2, fv_StOrder_StrLotOfferContract t3, d_StOrder_Lot t4 
where (t2.ID = t1.RefRequest)
and (t3.ID = t1.RefLotItems)
and (t4.ID = t3.RefLot);

/*Документация к закупке*/
CREATE OR REPLACE VIEW FV_STORDER_TENDERFILES  AS
select t1.refdoctype, t1.reftender, 
t2.refdocstatus, t2.reforganizer, t2.refsporganizer, t2.refperiod, t2.refterritory, t2.reftypetender, 1 val
from d_Storder_Tenderfiles t1 JOIN d_storder_tender t2 ON (t1.reftender=t2.id);

/*Пользователи сайта*/
CREATE OR REPLACE VIEW FV_STORDER_SITEUSER AS
select t.Refperson, t.reforganization, t.refdateregister, t.refdatediscard, t.isactive, 1 Val
from d_storder_siteuser t;

/* Убираем ненужные представления */
DROP VIEW DV_STORDER_REQUESTPLAN;
DROP VIEW FV_STORDER_REQUESTPLAN;
DROP VIEW FV_STORDER_REQUESTADD;
DROP VIEW DV_STORDER_ASKITEMSLOT;

commit work;

/* End - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.01.2010 */

/* Start - 12303 - Представление на територии - zaharchenko - 02.03.2010 */

CREATE OR REPLACE VIEW FV_FX_TERRITORIALPARTITIONTYPE
AS
SELECT *
FROM FX_FX_TERRITORIALPARTITIONTYPE;

commit;

/* End - 12303 - Представление на територии - zaharchenko - 02.03.2010 */

/* Start - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */

CREATE OR REPLACE VIEW DV_KD_EXCTCACHPL
(id, rowtype, pumpid, sourcekey, name, codestr, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, CODE7, CODE8, CODE9, CODE10, CODE11, REFKVSR, REFKVSRBRIDGE, REFKDPLAN, REFPROGBR, sourceid, refbridgekd, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.Name, T.CodeStr, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.CODE7, T.CODE8, T.CODE9, T.CODE10, T.CODE11, T.REFKVSR, T.REFKVSRBRIDGE, T.REFKDPLAN, T.REFPROGBR, T.SourceID, T.RefBridgeKD, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_KD_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_MeansType_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, name, RefBridgeMT, REFMTBRIDG, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.Name, T.RefBridgeMT, T.REFMTBRIDG, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_MeansType_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_Regions_ExctCachPl
(id, rowtype, pumpid, sourcekey, OKATO, CODE1, CODE2, CODE3, CODE4, name, RefBridgeOKATO, RefRegBridg, RefRegPlan, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.OKATO, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.Name, T.RefBridgeOKATO, T.RefRegBridg,  T.RefRegPlan, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_Regions_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_R_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, name, RefFKR, RefKCSR, RefKVR, RefRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.Name, T.RefFKR, T.RefKCSR, T.RefKVR, T.RefRBridge, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_R_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_EKR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefKOSGU, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefKOSGU, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_EKR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_SUBKESR_EXCTCACHPL
(id, rowtype, sourceid, pumpid, sourcekey, code, name, refsekrbridge, refkosgubridge, refkcsrbridge, datasourcename, short_name, short_name2)
AS
SELECT T.ID, T.RowType, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name, T.RefSEKRBridge, T.RefKOSGUBridge, T.RefKCSRBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, T.Name as short_name, substr(T.Name, 256, 255) as short_name2
FROM d_SubKESR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

commit;

/* End - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */

/* Start - 12968 - Представления для измерения показателей куба РЕГИОН_Оценка ЭРБС - dianova - 23.03.2010 */

CREATE OR REPLACE VIEW DV_ERBS_MARKS (ID, CODESTR, NAME, REFGROUP, REFTREND, REFUNITS, REVVALUE, GROUPNAME, GROUPCODESTR, GROUPCOEFF, TRENDNAME, TRENDCODESTR) AS
select t1.id, t1.codestr, t1.name, t1.referbsgroupofmarks, t1.referbs, t1.refunits, t1.revvalue, t2.name , t2.codestr, t2.coeff, t3.name, t3.codestr
from d_Erbs_Marks t1 JOIN d_Erbs_Groupofmarks t2 ON (t1.referbsgroupofmarks=t2.id) JOIN d_Erbs_Trend t3 ON (t1.referbs=t3.id);

commit work;

/* End - 12968 - Представления для измерения показателей куба РЕГИОН_Оценка ЭРБС - dianova - 23.03.2010 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.01.2010 */

/* Измерения */


/* Представление для измерения - Состояния сведений об ИспПрДействия */
CREATE OR REPLACE VIEW DV_STORDER_STATUSORDEREXE AS 
SELECT t2.ID, t2.Code, t2.Name
FROM d_StOrder_DocumentsStatus t2
WHERE t2.ID in (select distinct t1.RefExeStatus from d_Storder_Contract t1);

/* Представление для измерения - Год закупки */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTYEAR AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE t.parentid is null;

/* Представление для измерения - Дата начала договора */
CREATE OR REPLACE VIEW DV_DATE_AGREEMENTBEG AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Дата завершения контракта */
CREATE OR REPLACE VIEW DV_DATE_CONTRACTPLEND AS 
SELECT *
FROM fx_Date_YearDayUNV;

/* Представление для измерения - Состояния документа */
CREATE OR REPLACE VIEW DV_STORDER_STATUSAGREE AS 
SELECT t2.ID, t2.Code, t2.Name
FROM d_StOrder_DocumentsStatus t2
WHERE t2.ID in (select distinct t1.RefDocStatus from d_StOrder_Agreements t1);

/* Представление для измерения - Тип документа */
CREATE OR REPLACE VIEW DV_STORDER_DOCTYPEAGREE AS
select t1.Id, cast(t1.name as VARCHAR2(255)) Name
from d_StOrder_DocType t1
where t1.Id in (select distinct t2.RefDocType from d_StOrder_Agreements t2);

/* Представление для измерений класса ДаНет */
CREATE OR REPLACE VIEW DV_STORDER_YESNO AS 
select * from dv.fx_storder_smbusiness t;

/* Представление для измерения - Тип заявки */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTTYPE AS 
select t.id, 1 Code, 'Заявка на закупку' Name  from d_storder_doctype t where t.code in (205,258)
union
select t.id, 2 Code, 'План закупок' Name from d_storder_doctype t where t.code in (672,673)
union
select -1 id, -1 Code, 'Тип не указан' Name from d_storder_doctype;

/* Представление для измерения - Преимущества */
CREATE OR REPLACE VIEW DV_STORDER_PREFERENCES (ID, NAME) AS 
SELECT distinct t.reftenderpreferences, t.tenderpreferences
FROM d_StOrder_Contract t
where t.id>0;

/* Факты */
/* Контракты слили вместе со сведениями. Вьюшку вообще можно удалить если начнет работать мера count после нашей консоли */
CREATE OR REPLACE VIEW FV_STORDER_CONTRACT AS
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer, t1.refsupplier, t1.Id RefContract, t1.RefCause, 
t1.refexestatus, t1.refexedate, t1.refexetype, t1.ContractCost, t1.RefDateEnd, t1.actualpayment, 1 Val
from d_StOrder_Contract t1 
where (t1.Id > 0);

/* Заявки */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTS AS
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.RequestCost, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, t2.RefTender
FROM DV_STORDER_REQUEST t JOIN DV_STORDER_REQUESTTENDER t2 ON (t.ID = t2.RefRequest)
union
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.RequestCost, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, -1 RefTender
FROM DV_STORDER_REQUEST t 
where not t.id  in (select t2.Refrequest from DV_STORDER_REQUESTTENDER t2);

CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select t.Id, t.Id RefRequest, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.RefTender, t.refdocdate, t.purchaseyear, t.RequestCost, 1 Val
from DV_STORDER_REQUESTS t
where t.ID>0;

/* Закупки */
CREATE OR REPLACE VIEW FV_STORDER_TENDER  AS
select t2.RefPeriod, t2.RefTerritory, t2.ID RefTender, t2.RefOrganizer, t2.RefSPOrganizer, t2.RefDocStatus, t2.RefTypeTender, 
t2.RefForSMB, t2.refdatepublication, t2.refdateopening, t2.refdateconsider, t2.refdatematching, t2.refexecutor,
t1.id RefLot, t1.reftypeprotocol, t1.code, t1.ProtocolCount, t1.LotCost, 1 val
from  FV_STORDER_TENDERRESOLVLOT t1 JOIN d_StOrder_Tender t2 ON (t1.reftender = t2.ID)
Where t1.ID > 0
union
select t1.RefPeriod, t1.RefTerritory, t1.ID RefTender, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefDocStatus, t1.RefTypeTender, 
t1.RefForSMB, t1.RefDatePublication, t1.RefDateOpening, t1.RefDateConsider, t1.RefDateMatching, t1.RefExecutor,
-1 RefLot, -1 reftypeprotocol, 0 code, 0 ProtocolCount, 0 LotCost, 0 val
from d_StOrder_Tender t1 
Where not t1.id  in (select t2.reftender from d_Storder_Lot t2);

/* Источники финансирования */
CREATE OR REPLACE VIEW FV_STORDER_FINANCEDISTRIB AS
select t1.refkif, t1.acceptedamount, t2.refproduct, t2.refrequest, t2.refekr, t2.refkvr, t2.refmeasure, t3.refperiod, t3.refterritory
from d_Storder_Financedistribution t1 JOIN d_storder_askitems t2 ON (t1.Refaskitems=t2.id) JOIN d_storder_request t3 ON (t2.refrequest=t3.id)
where t1.Refaskitems >0;

/*Пользователи сайта*/
CREATE OR REPLACE VIEW FV_STORDER_SITEUSER AS
select t.Refperson, t.reforganization, t.refdateregister, t.refdatediscard, t.RefIsActive, 1 Val
from d_storder_siteuser t
where t.id>0;

CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
select 
t1.RefRequest, t1.RefProduct, t1.RefMeasure, t1.RefKVR, t1.RefEKR, t1.Quantity QuantityAsk, t1.Price PriceAsk, t1.Cost CostAsk,
t2.SourceID, t2.RefPeriod, t2.RefTerritory,
t3.RefLot, t3.RefContract,t3.MinPrice, t3.Quantity QuantityContract, t3.Price PriceContract, t3.Cost CostContract, 
t4.RefTender
from dv_StOrder_AskItems t1, d_StOrder_Request t2, fv_StOrder_StrLotOfferContract t3, d_StOrder_Lot t4 
where (t2.ID = t1.RefRequest)
and (t3.ID = t1.RefLotItems)
and (t4.ID = t3.RefLot);

/* Убираем ненужные представления */
DROP VIEW FV_STORDER_CONTRACTCOST;
DROP VIEW FV_STORDER_CONTRACTWITHCOST;
DROP VIEW DV_STORDER_ASKITEMSPR;
DROP VIEW FV_STORDER_ASKITEM;
DROP VIEW FV_STORDER_ASKITEMFIN;

commit work;

/* End - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.01.2010 */

/* Start - Пересоздание вьюшки, в таблицу добавили новое поле - zaharchenko - 05.05.2010 */

CREATE OR REPLACE VIEW DV_REGIONS_MONTHREP
AS
SELECT T.*, T.Name as short_name
FROM d_Regions_MonthRep T;

/* End - Пересоздание вьюшки, в таблицу добавили новое поле - zaharchenko - 05.05.2010 */

/* Start - Вьюха для интерфейса формы 2п - chubov */

create or replace view v_forecast_val_form2p as
select te.id, te.refparametrs, te.refforecasttype, (te.value) as est, (tf1.value) as y1, (tf2.value) as y2,
(tf3.value) as y3, (tr1.value) as r1,(tr2.value) as r2, te.yearof, te.refvarf2p
from t_forecast_paramvalues te
left join t_forecast_paramvalues tf1 on (te.refparametrs=tf1.refparametrs) and (te.yearof = tf1.yearof-1) and (te.refvarf2p = tf1.refvarf2p)
left join t_forecast_paramvalues tf2 on (te.refparametrs=tf2.refparametrs) and (te.yearof = tf2.yearof-2) and (te.refvarf2p = tf2.refvarf2p)
left join t_forecast_paramvalues tf3 on (te.refparametrs=tf3.refparametrs) and (te.yearof = tf3.yearof-3) and (te.refvarf2p = tf3.refvarf2p)
left join t_forecast_paramvalues tr1 on (te.refparametrs=tr1.refparametrs) and (te.yearof = tr1.yearof+2) and (te.refvarf2p = tr1.refvarf2p)
left join t_forecast_paramvalues tr2 on (te.refparametrs=tr2.refparametrs) and (te.yearof = tr2.yearof+1) and (te.refvarf2p = tr2.refvarf2p)
where te.paramtype=2;

commit;

/* End - Вьюха для интерфейса формы 2п - chubov */

/* Start - 13894 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.07.2010 */

/* Измерения */

/* Представление для измерения - Решения */
CREATE OR REPLACE VIEW DV_STORDER_LOTRESOLVE AS 
select distinct t.Code ID, t.Name from d_storder_resolvedlot t;

/* Представление для измерения Заявки - нужно без плановых заявок */
CREATE OR REPLACE VIEW DV_STORDER_REQUESTNOPLAN
(id, rowtype, sourceid, pumpid, sourcekey, code, name, datemodify, requestcost, refcustomer, refperiod, refterritory, refdocstatus, refexecutor, reforganizer, refdoctype, reftypetender, refdocdate, purchaseyear, refforsmb, datasourcename)
AS
SELECT T.ID, T.RowType, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name, T.DateModify, T.RequestCost, T.RefCustomer, T.RefPeriod, T.RefTerritory, T.RefDocStatus, T.RefExecutor, T.RefOrganizer, T.RefDocType, T.RefTypeTender, T.RefDocDate, T.PurchaseYear, T.RefForSMB,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_StOrder_Request T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)
JOIN d_storder_doctype DT ON (T.Refdoctype=DT.ID)
WHERE DT.Code in (205, 258);




/* Заявки */
CREATE OR REPLACE VIEW FV_STORDER_REQUEST AS
select t.Id, t.Id RefRequest, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, t.RequestCost, 1 Val
from D_STORDER_REQUEST t
where t.ID>0;

/* Размещение заказа */
/* 
1. Заявки беруться только те, по которым организована закупка и есть контракт, п.у. везде JOIN 
*/

CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
SELECT t1.Cost AskCost, t1.RefRequest, t2.RefLot, t3.Cost ContrCost, t3.RefContract, t4.RefTender
FROM 
     d_StOrder_AskItems t1 
JOIN 
     d_Storder_LotItems t2 
     ON (t1.RefLotItems=t2.ID) 
JOIN 
     d_Storder_ContractItems t3 
     ON (t3.RefLotItems=t2.ID)
JOIN 
     d_Storder_Lot t4 
     ON (t2.RefLot=t4.ID)
WHERE (t1.RefLotItems >0) and (t3.RefLotItems >0);

/* Закупки */
/* 
1. Внутренний запрос нужен чтобы для каждой закупки определить ее стоимость = сумма стоимостей ее лотов по d_storder_lot.lotcost. В принципе стоимость закупки можно смотреть в кубе по лотам, но каприз постановки - чтобы стоимость была и в этом кубе.
2. LEFT OUTER JOIN нужен, т.к. у закупки лотов вообще может не быть, хотя это недопустимо с т.з. предметной области. JOIN отсечет такие заявки и как результат количество заявок будет расчитываться неправильно
3. 1 TenderCount нужно, т.к. мера количество закупок с функцией аггрегирования Count пока почему то нашей консолью в 2005 не восстанавливается 
*/

CREATE OR REPLACE VIEW FV_STORDER_TENDER  AS
SELECT t1.ID, t1.RefPeriod, t1.RefTerritory, t1.ID RefTender, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefDocStatus, t1.RefTypeTender, 
t1.RefForSMB, t1.RefDatePublication, t1.RefDateOpening, t1.RefDateConsider, t1.RefDateMatching, t1.RefExecutor, 1 TenderCount, t2.TenderCost
FROM  
     d_StOrder_Tender t1 
LEFT OUTER JOIN 
     (SELECT t.reftender RefTender, SUM(t.lotcost) TenderCost FROM d_storder_lot t GROUP BY t.reftender) t2 
     ON (t1.ID=t2.RefTender)
WHERE t1.ID > 0;

/* Лоты */

/* DV_STORDER_LOTLASTRESOLVED - в результате лоты, расширенные последним решением по каждому */

CREATE OR REPLACE VIEW DV_STORDER_LOTLASTRESOLVED  AS
SELECT 
t1.ID RefLot, t2.Code RefResolve
FROM  
     d_Storder_Lot t1 
JOIN 
    (SELECT v1.RefLot, v1.Code
    FROM 
        d_Storder_Resolvedlot v1,
        (SELECT v2.RefLot RefLot, Max(v2.ID) ID FROM d_Storder_Resolvedlot v2 GROUP BY v2.RefLot ) LastModify
    WHERE (v1.RefLot = LastModify.RefLot) and (v1.ID=LastModify.ID)) t2 
    ON (t1.ID=t2.RefLot)
WHERE t1.ID > 0
UNION
SELECT 
t3.ID, 0 RefResolve
FROM  
     d_Storder_Lot t3 
WHERE (t3.ID > 0) and (not t3.id in (SELECT t4.RefLot FROM d_Storder_Resolvedlot t4));

/* DV_STORDER_LOTOFFERCOUNT - количество предложений на лот, не по всем лотам, а только по тем, по которым предложения есть п.у. далее LEFT OUTER JOIN */

CREATE OR REPLACE VIEW DV_STORDER_LOTOFFERCOUNT  AS
SELECT t.RefLot RefLot, Count(*) OfferCount 
FROM d_storder_purchaselotbids t 
GROUP BY t.reflot;

CREATE OR REPLACE VIEW FV_STORDER_LOT  AS
SELECT 
t1.ID, t1.LotCost, 1 LotCount, t1.ID RefLot,
t2.RefPeriod, t2.RefTerritory, t2.ID RefTender, t2.RefOrganizer, t2.RefSPOrganizer, t2.RefDocStatus, t2.RefTypeTender, 
t2.RefForSMB, t2.RefDatePublication, t2.RefDateOpening, t2.RefDateConsider, t2.RefDateMatching, t2.RefExecutor,
t3.RefResolve, t4.OfferCount
FROM  
    d_Storder_Lot t1 
JOIN 
    d_StOrder_Tender t2 
    ON (t1.RefTender = t2.ID)
JOIN 
    DV_STORDER_LOTLASTRESOLVED t3
    ON (t1.ID=t3.RefLot)
LEFT OUTER JOIN 
    DV_STORDER_LOTOFFERCOUNT t4
    ON (t1.ID=t4.RefLot)
WHERE t1.ID > 0;

/* Убираем ненужные представления */
DROP VIEW DV_STORDER_REQUESTS;
DROP VIEW FV_STORDER_TENDERRESOLVLOT;
DROP VIEW FV_STORDER_TENDERRESOLV;
DROP VIEW DV_STORDER_REQUESTTENDER;
DROP VIEW FV_STORDER_STRLOTOFFERCONTRACT;
DROP VIEW DV_STORDER_LOTITEMSMINPRICE;

commit work;

/* End - 13894 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.07.2010 */

/* Start - FMQ00014760 - ИФ_Журнал ставок ЦБ: Представление для изменения формата поля - zaharchenko- 04.10.2010 */


CREATE OR REPLACE VIEW dv_s_journalcb
(id, rowtype, INPUTDATE, PERCENTRATE, REFERENCEDDATA)
AS
SELECT T.ID, T."ROWTYPE", to_number(to_char(INPUTDATE, 'YYYYMMDD'), '99999999'), T.PERCENTRATE, T.REFERENCEDDATA
FROM d_s_journalcb T;
commit;

/* End - FMQ00014760 - ИФ_Журнал ставок ЦБ: Представление для изменения формата поля - zaharchenko- 04.10.2010 */

/* Start - FMQ00015192 - Представления для куба ФО_БОР_Оценка ОИВ - dianova - 3.12.2010 */

CREATE OR REPLACE VIEW DV_BRB_OLAPIDR
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, taskid, taskname, taskcode, taskweight, id, idrname, idrcode, idrunit, idrweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS."KINDSOFPARAMS" 
WHEN 1 THEN DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || ' - ' || cast(DS."YEAR" as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DT.ID, DT.NAME, DT.CODE, DT.WEIGHTINGOAL,
DI.ID, DI.NAME, DI.CODE, DI.UNIT, DI.WEIGHTINDEXINTASK
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_Brb_Task DT ON (DT.RefGoal=DG.ID)
JOIN d_Brb_Idr DI ON (DI.Reftask=DT.ID);

CREATE OR REPLACE VIEW DV_BRB_OLAPIER
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, id, iername, iercode, ierunit, ierweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS."KINDSOFPARAMS" 
WHEN 1 THEN DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || ' - ' || cast(DS."YEAR" as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DI.ID, DI.NAME, DI.CODE, DI.UNIT, DI.WEIGHTINGOAL
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_BRB_IER DI ON (DI.RefGoal=DG.ID);

CREATE OR REPLACE VIEW DV_BRB_OLAPTASK
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, id, taskname, taskcode, taskweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS."KINDSOFPARAMS" 
WHEN 1 THEN DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || ' - ' || cast(DS."YEAR" as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DT.ID, DT.NAME, DT.CODE, DT.WEIGHTINGOAL
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_Brb_Task DT ON (DT.RefGoal=DG.ID);

commit work;

/* End - FMQ00015192 - Представления для куба ФО_БОР_Оценка ОИВ - dianova - 3.12.2010 */

/* Start - 15355 - Представления для кубов блока Мониторинг государственных закупок - dianova - 10.12.2010 */

/* Представление для измерения - Преимущества */
CREATE OR REPLACE VIEW DV_STORDER_PREFERENCES (ID, NAME) AS 
SELECT distinct t.reftenderpreferences, t.tenderpreferences
FROM d_StOrder_Contract t;

commit work;

/* End - 15355 - Представления для кубов блока Мониторинг государственных закупок - dianova - 10.12.2010 */

/* Start - FMQ00015356 - Преобразование логических полей в числовой формат - zaharchenko- 27.12.2010 */

CREATE OR REPLACE VIEW FV_F_BKKUMARKS AS
SELECT *
FROM f_F_BKKUMarks T;

CREATE OR REPLACE VIEW FV_Marks_FOQualityFM AS
SELECT *
FROM f_Marks_FOQualityFM T;

CREATE OR REPLACE VIEW FV_MARKS_FOBKKUMONTHREP AS
SELECT *
FROM F_MARKS_FOBKKUMONTHREP T;

commit;

/* End - FMQ00015356 - Преобразование логических полей в числовой формат - zaharchenko- 27.12.2010 */

/* Start - 15591 - Представления для кубов блока Мониторинг государственных закупок - dianova - 27.01.2011 */

/* Количество лотов, количество предложений на лот, стоимость лотов. Куб - Лоты */

/* Вспомогательная. DV_STORDER_LOTLASTRESOLVED - в результате лоты, расширенные последним решением по каждому */

CREATE OR REPLACE VIEW DV_STORDER_LOTLASTRESOLVED  AS
SELECT 
t1.ID RefLot, t1.RefTender RefTender ,t2.Code RefResolve
FROM  
     d_Storder_Lot t1 
JOIN 
    (SELECT v1.RefLot, v1.Code
    FROM 
        d_Storder_Resolvedlot v1,
        (SELECT v2.RefLot RefLot, Max(v2.ID) ID FROM d_Storder_Resolvedlot v2 GROUP BY v2.RefLot ) LastModify
    WHERE (v1.RefLot = LastModify.RefLot) and (v1.ID=LastModify.ID)) t2 
    ON (t1.ID=t2.RefLot)
WHERE t1.ID > 0
UNION
SELECT 
t3.ID, t3.RefTender RefTender, 0 RefResolve
FROM  
     d_Storder_Lot t3 
WHERE (t3.ID > 0) and (not t3.id in (SELECT t4.RefLot FROM d_Storder_Resolvedlot t4));

/* Вспомогательная. DV_STORDER_LOTOFFEROFFCOUNT - количество принятых предложений на лот, не по всем лотам, а только по тем, по которым предложения есть п.у. далее LEFT OUTER JOIN */

CREATE OR REPLACE VIEW DV_STORDER_LOTOFFEROFFCOUNT  AS
SELECT t.RefLot RefLot, Count(*) OfferOffCount 
FROM d_storder_purchaselotbids t 
WHERE t.refusal = 0
GROUP BY t.reflot;

CREATE OR REPLACE VIEW FV_STORDER_LOT  AS
SELECT 
t1.ID, t1.LotCost, 1 LotCount, t1.ID RefLot,
t2.RefPeriod, t2.RefTerritory, t2.ID RefTender, t2.RefOrganizer, t2.RefSPOrganizer, t2.RefDocStatus, t2.RefTypeTender, 
t2.RefForSMB, t2.RefDatePublication, t2.RefDateOpening, t2.RefDateConsider, t2.RefDateMatching, t2.RefExecutor,
t3.RefResolve, t4.OfferCount, t5.OfferOffCount
FROM  
    d_Storder_Lot t1 
JOIN 
    d_StOrder_Tender t2 
    ON (t1.RefTender = t2.ID)
JOIN 
    DV_STORDER_LOTLASTRESOLVED t3
    ON (t1.ID=t3.RefLot)
LEFT OUTER JOIN 
    DV_STORDER_LOTOFFERCOUNT t4
    ON (t1.ID=t4.RefLot)
LEFT OUTER JOIN 
    DV_STORDER_LOTOFFEROFFCOUNT t5
    ON (t1.ID=t5.RefLot)
WHERE t1.ID > 0;

/* Куб - Размещение заказа */
/* Заявки беруться только те, по которым организована закупка и есть контракт, п.у. везде JOIN */
CREATE OR REPLACE VIEW FV_STORDER_EXEORDER AS
SELECT t1.Cost AskCost, t1.RefRequest, t2.RefLot, t3.Cost ContrCost, t3.RefContract, t4.RefTender, t4.RefResolve
FROM 
     d_StOrder_AskItems t1 
JOIN 
     d_Storder_LotItems t2 
     ON (t1.RefLotItems=t2.ID) 
JOIN 
     d_Storder_ContractItems t3 
     ON (t3.RefLotItems=t2.ID)
JOIN 
     DV_STORDER_LOTLASTRESOLVED t4 
     ON (t2.RefLot=t4.RefLot)
WHERE (t1.RefLotItems >0) and (t3.RefLotItems >0);

commit work;

/* End - 15591 - Представления для кубов блока Мониторинг государственных закупок - dianova - 27.01.2011 */

/* Start - Пересоздание представления, поле Name расширили до 500 символов - zaharchenko - 20.02.2011 */

CREATE OR REPLACE VIEW DV_KSSHK_FOYR
(id, rowtype, sourceid, pumpid, sourcekey, code, short_name, short_name_part2, refksshkbridge, datasourcename)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2,T.RefKSSHKBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_KSSHK_FOYR T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW BV_KSSHK_BRIDGE
AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM B_KSSHK_BRIDGE T;

CREATE OR REPLACE VIEW DV_KSSHK_FOPROJECT
(id, rowtype, sourceid, pumpid, sourcekey, code, short_name, short_name_part2, refksshkbridge, datasourcename)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, T.RefKSSHKBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_KSSHK_FOProject T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);
/* End - Пересоздание представления, поле Name расширили до 500 символов - zaharchenko - 20.02.2011 */

/*Версионность сопоставимых*/
CREATE OR REPLACE VIEW BV_EKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID, T.RefEKRBridge, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_EKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '9a85c0c8-390d-41cb-839c-f57ef54f7ff3' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_FKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Name, T.ParentID, T.RefFKRBridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_FKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '1acb453b-dd86-438a-83b9-c27ce4fd8bda' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KVSR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.FIO, T.Post, T.WebSite, T.Email, T.Telephone, T.ShortName, T.AddressSkype, T.AddressFaceTime, T.CodeLine, T.RefKVSRBridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_KVSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'eb64ed07-4635-4b25-8452-0b0d119458e3' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KVR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.RefKVRBridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3, substr(T.Name, 766, 255) as Short_Name_Part4
FROM b_KVR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c073b576-58dd-4873-9806-55ec0ce93929' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_FACIALACC_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Name, T.KVSRID, T.KVSRCode, T.KVSRName, T.KCSRID, T.KCSRCode, T.KCSRName, T.KFSRID, T.KFSRCode, T.KFSRName, T.KVRID, T.KVRCode, T.KVRName, T.KESRID, T.KESRCode, T.KESRName, T.FinTypeID, T.FinTypeCode, T.FinTypeName, T.RegionClsID, T.RegionClsCode, T.RegionClsName, T.MeansTypeID, T.MeansTypeCode, T.MeansTypeName, T.OrgID, T.OrgCode, T.OrgName, T.GeneralOrgID, T.GeneralOrgCode, T.GeneralOrgName, T.HigherOrgID, T.HigherOrgCode, T.HigherOrgName, T.ParentID, T.RefFABridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2
FROM b_FacialAcc_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c51c2054-1e9b-411a-85a3-b9f6e16ec699' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_OKVED_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Name, T.Section, T.SubSection, T.ShortName, T.ParentID, T.RefKVSRDepartment, T.RefOKVEDBridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_OKVED_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'a5f87962-8af2-4419-8b5a-746a1e3540e8' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_MEANSTYPE_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ActivityKind, T.ParentID, T.RefMTBridge
, T.Name as short_name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_MeansType_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '836220a8-5c5e-4237-a49d-9677be9d6d33' and t2.IsCurrent = 1) or ID = -1;
/*Версионность сопоставимых*/

/* Start - 16955 - Представления для измерения территорий куба СТАТ_СЭП_Годовой сборник - dianova - 25.05.2011 */
CREATE OR REPLACE VIEW DV_TERRITORY_OLAPSEP AS
SELECT * 
FROM D_TERRITORY_RF t 
WHERE t.REFTERRITORIALPARTTYPE IN (1,2,3);

/* Start - Feature #18115 - Представления для таблиц фактов - zaharchenko- 10.09.2011 */
CREATE OR REPLACE VIEW fV_r_foyroutcomes 
AS
SELECT *
FROM f_r_foyroutcomes;

CREATE OR REPLACE VIEW FV_F_MONTHREPOUTCOMES
AS
SELECT *
FROM F_F_MONTHREPOUTCOMES;

/* Start - FMQ00019517 - Представления для куба ЭО_Прогноз_Планирование - dianova - 08.12.2011 */
CREATE OR REPLACE VIEW DV_FORECAST_OLAPPMETHODS (GroupId, GroupName, Id, MethodName) as
select g.Code, g.TextName, M.code, M.TextName
from 
(select t.code, t.textname, t.code1 from d_Forecast_PMethods t where  t.parentid is null) g,
(select t.code, t.textname, t.code1 from d_Forecast_PMethods t where not t.parentid is null) m
where g.Code1 = m.Code1;

CREATE OR REPLACE VIEW DV_FORECAST_OLAPYEAR (Id, Name) 
AS
SELECT distinct t.dateyearid, t.dateyear 
FROM fx_date_yeardayunv t
WHERE t.dateyearid > 0;

CREATE OR REPLACE VIEW DV_FORECAST_OLAPPYEAR (Id, Name) 
AS
SELECT distinct t.dateyearid, t.dateyear 
FROM fx_date_yeardayunv t
WHERE t.dateyearid > 0;

/* Start - FMQ00019423 - Представление для измерения Исполнение расходов.Реестр программ - dianova - 16.12.2011 */
CREATE OR REPLACE VIEW DV_EXCCOSTS_OLAPLISTPRG
(ID, CODE, NAME, SHORTNAME, NOTE, PARENTID, NPA, KBK, REFCREATORS, REFTYPEPROG, REFTERRITORY, CREATOR, TYPEPROG, STATYEAR, BPERIOD, EPERIOD) 
AS
select d.id, d.code, d.name, d.shortname, d.note, d.parentid, d.NPA, d.KBK, d.refcreators, d.reftypeprog, d.refterritory, cp1.name, cp2.name, cp3.name, cp4.name, cp5.name
from d_exccosts_listprg d 
JOIN d_exccosts_creators cp1 on (d.refcreators=cp1.id)
JOIN fx_exccosts_tpprg cp2 on (d.reftypeprog=cp2.id)
JOIN fx_date_yeardayunv cp3 on (d.refapyear=cp3.id)
JOIN fx_date_yeardayunv cp4 on (d.refbegdate=cp4.id)
JOIN fx_date_yeardayunv cp5 on (d.refenddate=cp5.id);

/* Start - Feature #17380 Создать кубы по блоку "ФО_0041_Оценка эффективности льгот" - 19.12.2011 */
CREATE OR REPLACE VIEW fV_FX_TypeTax_bridge
AS
SELECT *
FROM fx_FX_TypeTax;

/* Start - Feature #19774 ФНС_0030_Куб "ФНС_Задолженность - zaharchenko - 26.12.2011 */
CREATE OR REPLACE VIEW dv_org_fnsdebtor (ID, RowType, PumpID, SourceKey, INN, KPP, Name, DATAKIND, IFNS, REFREG, INNNAME)
AS SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.INN, T.KPP, T.Name, T.DATAKIND, T.IFNS, T.REFREG,
CAST(datakind as varchar2(50)) || ' ' || CAST(INN as varchar2(12)) || ' ' || CAST(KPP as varchar2(10)) || ' ' || CAST(name as varchar2(500)) END
FROM d_org_fnsdebtor T;

/* Start - Feature #19897 Измерение "Показатели.Мониторинг местных бюджетов" свойство "Форма" - zaharchenko - 12.01.2012 */
CREATE OR REPLACE VIEW DV_FORM_MONITORINGMB
AS
SELECT *
FROM D_FORM_MONITORINGMB;

CREATE OR REPLACE VIEW DV_FORM_MONITORINGMB_BRIDGE
AS
SELECT *
FROM D_FORM_MONITORINGMB;

/* Start - Feature #20077 ФО_0035_Информация по объектам_Новосибирск - zaharchenko - 24.01.2012 */
CREATE OR REPLACE VIEW FV_DATE_YEAR AS
SELECT * 
FROM fx_date_year;

/* Start - Feature #17798 ФО_0001_АС Бюджет_Новые измерения Код целевых средств и Вид плана - zaharchenko - 30.01.2012 */
CREATE OR REPLACE VIEW BV_TRANSFERT_BRIDGE AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_Transfert_Bridge T;

CREATE OR REPLACE VIEW DV_TRANSFERT_BUDGET AS
SELECT T.*, T.Name as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_Transfert_Budget T;

/* Start - FMQ00020383 - Представления для измерений дат блока ООС - dianova - 15.02.2012 */
CREATE OR REPLACE VIEW DV_OOS_OLAPPUBLICDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPGIVEDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPCONSDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPMATCHDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPRESULTDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPPUBCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPSIGCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPPROTCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;

CREATE OR REPLACE VIEW DV_OOS_OLAPEXECONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE (t.DateDayID < -1) or (t.ID = -1);

CREATE OR REPLACE VIEW DV_OOS_OLAPTYPEPURCONTR AS 
SELECT *
FROM FX_OOS_TYPEPURCH;

/* Start - Представление для измерения Тип территориий, бля использования в Районы.Планирование - zaharchenko - 16.03.2012 */
create or replace view dv_FX_FX_TERRITORIALPART_br as
select *
from FX_FX_TERRITORIALPARTITIONTYPE;

/* Start - FMQ00020786 - Представления для измерений дат блока ООС - dianova - 22.03.2012 */
CREATE OR REPLACE VIEW DV_OOS_OLAPTYPEPURPL AS 
SELECT *
FROM FX_OOS_TYPEPURCH;

CREATE OR REPLACE VIEW DV_OOS_OLAPPUBPLDATEPL 
(ID,DATEYEARID,DATEYEAR,DATEHALFYEARID,DATEHALFYEAR,DATEQUARTERID,DATEQUARTER,DATEMONTHID,DATEMONTH,DATEDAY,ORDERBYDEFAULT)
AS 
select t.id, 
t.dateyearid, t.dateyear,
CASE t.datehalfyearid	WHEN -2 
  THEN t.dateyearid	
  ELSE t.datehalfyearid	
  END , t.datehalfyear,
CASE t.datequarterid	WHEN -2 
  THEN  CASE t.datehalfyearid	WHEN -2 
          THEN t.dateyearid	
          ELSE t.datehalfyearid	 
          END
  ELSE t.datequarterid	
  END , t.datequarter,
CASE t.datemonthid	WHEN -2 
  THEN  CASE t.datequarterid	WHEN -2 
          THEN 	CASE t.datehalfyearid	WHEN -2 
                  THEN t.dateyearid	
                  ELSE t.datehalfyearid	 
                  END
          ELSE t.datequarterid	 
          END
  ELSE t.datemonthid	
  END , t.datemonth,
  t.dateday, t.orderbydefault
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAY = 'Заключительные обороты') or (t.DATEDAY = 'Остатки на начало года') or (t.Id=-1))
UNION
select -1,-1,'Значение не указано',-1,'Значение не указано',-1,'Значение не указано',-1,'Значение не указано','Значение не указано', -1
FROM fx_Date_YearDayUNV t2;

CREATE OR REPLACE VIEW DV_OOS_OLAPPUBDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));

CREATE OR REPLACE VIEW DV_OOS_OLAPBEGINDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));

CREATE OR REPLACE VIEW DV_OOS_OLAPENDDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));

CREATE OR REPLACE VIEW DV_OOS_OLAPUPDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));

CREATE OR REPLACE VIEW DV_OOS_OLAPEXEDATEPL 
(ID,DATEYEARID,DATEYEAR,DATEHALFYEARID,DATEHALFYEAR,DATEQUARTERID,DATEQUARTER,DATEMONTHID,DATEMONTH,DATEDAY,ORDERBYDEFAULT)
AS 
select t.id, 
t.dateyearid, t.dateyear,
CASE t.datehalfyearid	WHEN -2 
  THEN t.dateyearid	
  ELSE t.datehalfyearid	
  END , t.datehalfyear,
CASE t.datequarterid	WHEN -2 
  THEN  CASE t.datehalfyearid	WHEN -2 
          THEN t.dateyearid	
          ELSE t.datehalfyearid	 
          END
  ELSE t.datequarterid	
  END , t.datequarter,
CASE t.datemonthid	WHEN -2 
  THEN  CASE t.datequarterid	WHEN -2 
          THEN 	CASE t.datehalfyearid	WHEN -2 
                  THEN t.dateyearid	
                  ELSE t.datehalfyearid	 
                  END
          ELSE t.datequarterid	 
          END
  ELSE t.datemonthid	
  END , t.datemonth,
  t.dateday, t.orderbydefault
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAY = 'Заключительные обороты') or (t.DATEDAY = 'Остатки на начало года') or (t.Id=-1))
UNION
select -1,-1,'Значение не указано',-1,'Значение не указано',-1,'Значение не указано',-1,'Значение не указано','Значение не указано',-1
FROM fx_Date_YearDayUNV t2;


commit work;

whenever SQLError exit rollback;