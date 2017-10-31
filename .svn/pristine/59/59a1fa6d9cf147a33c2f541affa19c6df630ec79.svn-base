/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		OlapView.sql - Вьюшки для построения кубов.
	СУБД	Oracle 9.2
*/

/* Представления для измерений "В Бюджет..." и "Из Бюджета..."*/

:on error ignore

drop VIEW dv_regions_budget_refbudsource;

go

CREATE VIEW dv_regions_budget_refbudsource
AS
SELECT *
FROM dv_Regions_Budget;

go

drop VIEW dv_regions_budget_refbuddest;

go

CREATE VIEW dv_regions_budget_refbuddest
AS
SELECT *
FROM dv_Regions_Budget;

go

drop VIEW bv_regions_bridge_refbudsource;

go

CREATE VIEW bv_regions_bridge_refbudsource
AS
SELECT *
FROM bv_Regions_Bridge;

go

drop VIEW bv_regions_bridge_refbuddest;

go

CREATE VIEW bv_regions_bridge_refbuddest
AS
SELECT *
FROM bv_Regions_Bridge;

go


/* Представления для вычисляемых полей в измерениях "КИФ..." */

drop view FV_KIF_DIRECTION_BRIDGE;

go

CREATE view FV_KIF_DIRECTION_BRIDGE
as select *
from FX_KIF_DIRECTION;

go

drop view FV_KIF_CLSASPECT_BRIDGE;

go

CREATE view FV_KIF_CLSASPECT_BRIDGE
as select *
from FX_KIF_CLSASPECT;

go

/* Представления для вычисляемых полей в измерении "МФ РФ.Сопоставимый" */

drop view dv_Units_OKEI_Bridge;

go

CREATE view dv_Units_OKEI_Bridge
as select *
from Dv_Units_OKEI;

go

drop view fv_FX_VerificationConditions

go

CREATE view fv_FX_VerificationConditions
as select *
from fx_FX_VerificationConditions;

go

/* Представление для измерения "Период.День ФО", чтобы использовать это измерение совместно с "Период.День ФК" в кубе "УФК_Свод ФУ" */

drop VIEW fv_date_yearday;

go

CREATE VIEW fv_date_yearday
AS
SELECT *
FROM fx_date_yearday;

go

/* Представления для измерений "РасчСчета.Корреспонд Счет" и "РасчСчета.Корресп сопост" - необходимы для куба "ФО_АС Бюджет_Операции со счетами" */

drop VIEW dv_Accounts_BudgetFOOwn_Cor;

go

CREATE VIEW dv_Accounts_BudgetFOOwn_Cor
AS
SELECT *
FROM dv_Accounts_Budgetfoown;

go

drop VIEW bv_Accounts_Bridge_Cor;

go

CREATE VIEW bv_Accounts_Bridge_Cor
AS
SELECT *
FROM b_Accounts_Bridge;

go

/* Представление с полем name, разбитым на 3 части для измерения "Фонды.Типы фондов" */
/* Удалить после перхода на 2005 */
drop VIEW DV_FUND_FO9TYPES

go

CREATE VIEW DV_FUND_FO9TYPES
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 510) as short_name_part2, substring(T.Name, 511, 765) as short_name_part3
FROM d_Fund_FO9Types T;

go

/* Представление с полем name, разбитым на 3 части для измерения "КЦСР.Сопоставимый планирование" */
/* Удалить после перхода на 2005 */
drop VIEW BV_KCSR_BRIDGEPLAN;

go

CREATE VIEW BV_KCSR_BRIDGEPLAN
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 510) as short_name_part2, substring(T.Name, 511, 765) as short_name_part3
FROM b_KCSR_BridgePlan T;

go

/* Блок ФО_0016_Мониторинг_БК_КУ */
drop VIEW fv_FX_VerificationConditions;

go

create view fv_FX_VerificationConditions as
select ID, RowType, Name, Value
from fx_FX_VerificationConditions;

go

/* Представление для измерения "Организации.Сопоставимый" */
drop VIEW BV_ORGANIZATIONS_BRIDGE;

go

CREATE VIEW BV_ORGANIZATIONS_BRIDGE
AS
SELECT T.*, T.Name as short_name
FROM b_Organizations_Bridge T;

go

/* Представление для измерения "Организации.Сопоставимый планирование" */
drop VIEW BV_ORGANIZATIONS_BRIDGEPLAN;

go

CREATE VIEW BV_ORGANIZATIONS_BRIDGEPLAN
AS
SELECT T.*, T.Name as short_name
FROM b_Organizations_BridgePlan T;

go

/* Представление для измерения Тип территориий */
drop VIEW dv_FX_FX_TERRITORIALPART;

go

create view dv_FX_FX_TERRITORIALPART as
select ID, ROWTYPE, NAME, FULLNAME
from FX_FX_TERRITORIALPARTITIONTYPE;

go

/* Представление для измерения Тип территориий */
drop VIEW dv_FX_FX_TERRITORIALPARTTYPE;

go

create view dv_FX_FX_TERRITORIALPARTTYPE as
select ID, ROWTYPE, NAME, FULLNAME
from FX_FX_TERRITORIALPARTITIONTYPE;

go

/* Start - 7861 - Представление с полем name, разбитым на 3 части для измерения "КИФ.АС Бюджет 2005" - zaharchenko- 14.02.2008 */
/* Удалить после перхода на 2005 */
drop VIEW DV_KIF_BUDGET2005;

go

CREATE VIEW DV_KIF_BUDGET2005
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM D_KIF_BUDGET2005 T;

go


/* Start - 8811 - Представления для куба "УФК_Справка органа ФК" - zaharchenko - 03.07.2008 */
drop VIEW DV_KD_UFK_Debet;

go

CREATE VIEW DV_KD_UFK_Debet
AS
SELECT *
FROM DV_KD_UFK;

go

drop VIEW BV_KVSR_BRIDGE_Debet;

go

CREATE VIEW BV_KVSR_BRIDGE_Debet
AS
SELECT *
FROM BV_KVSR_BRIDGE;

go

drop VIEW BV_KD_BRIDGE_Debet;

go

CREATE VIEW BV_KD_BRIDGE_Debet
AS
SELECT *
FROM BV_KD_BRIDGE;

go

drop VIEW BV_KD_BRIDGEPLAN_Debet;

go

CREATE VIEW BV_KD_BRIDGEPLAN_Debet
AS
SELECT *
FROM BV_KD_BRIDGEPLAN;

go

/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
drop VIEW DV_ORG_UFKPAYERS;

go

CREATE VIEW DV_ORG_UFKPAYERS
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM D_ORG_UFKPAYERS T;

go

/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
drop VIEW bv_org_payersbridge;

go

CREATE VIEW bv_org_payersbridge
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM b_org_payersbridge T;

go

/* Start - 9590 - ФО_КЦ Система_Доходы-изменить куб, представление для измерения Период.Дата Утверждения - zaharchenko - 21.11.2008 */
drop VIEW dV_fx_date_yeardayunv;

go

CREATE VIEW dV_fx_date_yeardayunv
AS
SELECT *
FROM fx_date_yeardayunv;

go

/* Start - 9590 - В измерении КД.КЦ Система поле name расширено до 1000 знаков. - zaharchenko - 26.11.2008 */
/* Представление с полем name, разбитым на 5 частей для измерения "КД.КЦ Система" */
/* Удалить после перхода на 2005 */
drop VIEW DV_KD_KCSYSTEMA_bridge;

go

CREATE VIEW DV_KD_KCSYSTEMA_bridge
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4, substring(T.Name, 1021, 255) as short_name_part5
FROM D_KD_KCSYSTEMA T;

go

/* Start - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */
drop VIEW BV_KD_BRIDGE;

go

CREATE VIEW BV_KD_BRIDGE
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_KD_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '5cd4f631-6276-4a9f-b466-980282500b50' and t2.IsCurrent = 1) or ID = -1;

go

/* End - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */

/* Start - 10815 -  Кубы для мониторинга ФП и КУ (представления для свойств в измерениях)  - zaharchenko - 02.06.2009 */
drop VIEW fv_fx_verificatconditions_max;

go

create view fv_fx_verificatconditions_max
as select *
from fx_fx_verificationconditions;

go

drop VIEW fv_fx_verifcond_bridge_min;

go

create view fv_fx_verifcond_bridge_min
as select *
from fx_fx_verificationconditions;

go

drop VIEW fv_fx_verifcond_bridge_max;

go

create view fv_fx_verifcond_bridge_max
as select *
from fx_fx_verificationconditions;

go

/* End - 10815 -  Кубы для мониторинга ФП и КУ (представления для свойств в измерениях)  - zaharchenko - 02.06.2009 */

/* Start - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */
drop VIEW Bv_Kd_Bridgeplan;

go

CREATE VIEW Bv_Kd_Bridgeplan
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_Kd_Bridgeplan T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '65fefc22-6135-4ee0-8fc3-3801a368991a' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW Bv_r_bridge;

go

CREATE VIEW Bv_r_bridge
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_r_bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0a626485-8481-4058-aa0f-a917df395f3c' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW Bv_r_Bridgeplan;

go

CREATE VIEW Bv_r_Bridgeplan
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgeplan T;

go

drop VIEW Bv_r_Bridgerep;

go

CREATE VIEW Bv_r_Bridgerep
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgerep T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '8319a6d9-2adf-417d-9f93-8b0c12ec071c' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW Dv_r_Plan;

go

CREATE VIEW Dv_r_Plan
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM d_r_Plan T;

go

drop VIEW Dv_r_Admprojectoutcome;

go

CREATE VIEW Dv_r_Admprojectoutcome
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM d_r_Admprojectoutcome T;

go

drop VIEW BV_KIF_BRIDGE;

go

CREATE VIEW BV_KIF_BRIDGE
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, kvsr, descriptioncode, maindescriptioncode, itemcode, programcode, kesr, parentid, refdirection, refclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T.ROWTYPE, T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.KVSR, T.DescriptionCode, T.MainDescriptionCode, T.ItemCode, T.ProgramCode, T.KESR, T.ParentID, T.RefDirection, T.RefClsAspect
, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2
FROM b_KIF_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0773168f-923d-4140-98cc-01328f353e40' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW BV_KIF_BRIDGEPLAN;

go

CREATE VIEW BV_KIF_BRIDGEPLAN
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, parentid, refkif, refkifclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T.ROWTYPE, T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.ParentID, T.RefKIF, T.RefKIFClsAspect
, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2
FROM b_KIF_BridgePlan T;

go

drop VIEW BV_KCSR_BRIDGE;

go

CREATE VIEW BV_KCSR_BRIDGE
(id, rowtype, sourceid, code, code1, code2, code3, name, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T.ROWTYPE, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID
, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM b_KCSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'ee980110-fa1f-43c4-b03e-b92c6fec5035' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW DV_R_FO9KINDS;

go

CREATE VIEW DV_R_FO9KINDS
(id, rowtype, code, name, note, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T.ROWTYPE, T.Code, T.Name, T.Note, T.ParentID
, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM d_R_FO9Kinds T;

go

drop VIEW dv_r_Fo14grbs;

go

CREATE VIEW dv_r_Fo14grbs AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3
FROM d_r_Fo14grbs T;

go

/* End - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */

/* Start - 8924 -  Представление для измерения Период.Дата принятия  - zaharchenko - 21.07.2009 */
drop VIEW fv_date_yeardayunv;

go

CREATE VIEW fv_date_yeardayunv AS
SELECT *
FROM fx_date_yeardayunv;

go

/* End - 8924 -  Представление для измерения Период.Дата принятия  - zaharchenko - 21.07.2009 */


/* Start -  - Добавлен модуль для прогноза развития региона - chubov - 19.12.2008 */

drop view fv_forecast_scenario_data;
go

create view fv_forecast_scenario_data (valuebase, valueestimate, valuey1, valuey2, valuey3, valuey4, valuey5, minbound, maxbound, refscenario, refparams) as
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

go

drop view v_forecast_val_indicators;
go

create view v_forecast_val_indicators(id, basescenario, refscenario, valuebase, valueestimate, v_est_b, valuey1, v_y1_b, valuey2, v_y2_b,
valuey3, v_y3_b, valuey4, v_y4_b, valuey5, v_y5_b, minbound, maxbound, leftpenaltycoef, rightpenaltucoef, userid, refparams, designation, groupname) as
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
     where substring(str(i.Code,6,0), 1, 1) = substring(str(o.code,6,0), 1, 1) and substring(str(i.Code,6,0), 2, 2) = substring(str(o.Code,6,0), 2, 2) and substring(str(i.Code,6,0), 5, 2) = '00'
  )
  from d_Forecast_Parametrs o
  where o.id = t.refparams
) as groupname

from t_forecast_indvalues t join t_forecast_indvalues d on (t.refparams = d.refparams) and (t.refscenario<>d.refscenario)
join d_forecast_parametrs p on (p.id = t.refparams);
go

drop view v_forecast_val_adjusters;
go

create view v_forecast_val_adjusters(id, basescenario, refscenario, valuebase, valueestimate, v_est_b, valuey1, v_y1_b, valuey2, v_y2_b,
valuey3, v_y3_b, valuey4, v_y4_b, valuey5, v_y5_b, minbound, maxbound, userid, refparams, designation, groupname) as
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
     where substring(str(i.Code,6,0), 1, 1) = substring(str(o.code,6,0), 1, 1) and substring(str(i.Code,6,0), 2, 2) = substring(str(o.Code,6,0), 2, 2) and substring(str(i.Code,6,0), 5, 2) = '00'
  )
  from d_Forecast_Parametrs o
  where o.id = t.refparams
) as groupname
from t_forecast_adjvalues t join t_forecast_adjvalues d on (t.refparams = d.refparams) and (t.refscenario<>d.refscenario)
join d_forecast_parametrs p on (p.id = t.refparams);

go


drop view dv_forecast_parametrs;
go

create view dv_forecast_parametrs (id, rowtype, code, name, refunits, description, signat, groupecode, groupename) as
select d.id, d.rowtype, d.code, d.name, d.refunits, d.description, d.signat,
Floor((d.code / 100000)) as groupcode,
(case Floor((d.code  / 100000))
 when 1 then 'Индикаторы'
 when 2 then 'Регуляторы'
 when 3 then 'Статистические параметры'
 when 5 then 'Нерегулируемые параметры'
end) as groupname
from d_forecast_parametrs d;

go

/* End   -  - Добавлен модуль для прогноза развития региона - chubov - 19.12.2008 */

/* Start - 11693 - Представление на Районы.Планирование - zaharchenko - 02.10.2009 */

drop VIEW DV_REGIONS_PLAN_Bridge;
go

CREATE VIEW DV_REGIONS_PLAN_Bridge
AS
SELECT *
FROM DV_REGIONS_PLAN;

go

/* Start -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

drop VIEW DV_ORG_PRODUCT;

go

CREATE VIEW DV_ORG_PRODUCT AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 2;

go

drop VIEW DV_ORG_MARKET;

go

CREATE VIEW DV_ORG_MARKET AS
select * from d_Org_RegistrOrg t
where t.RefOrg <> 1;

go

/* End -  - Представления для измерений куба ОРГАНИЗАЦИИ_Цены и тарифы - dianova - 09.12.2009 */

/* Start - 12303 - Представление на територии - zaharchenko - 02.03.2010 */

drop VIEW FV_FX_TERRITORIALPARTITIONTYPE;

go

CREATE VIEW FV_FX_TERRITORIALPARTITIONTYPE
AS
SELECT *
FROM FX_FX_TERRITORIALPARTITIONTYPE;

/* End - 12303 - Представление на територии - zaharchenko - 02.03.2010 */

go

/* Start - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */

drop VIEW DV_KD_EXCTCACHPL;

go

CREATE VIEW DV_KD_EXCTCACHPL
(id, rowtype, pumpid, sourcekey, name, codestr, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, CODE7, CODE8, CODE9, CODE10, CODE11, REFKVSR, REFKVSRBRIDGE, REFKDPLAN, REFPROGBR, sourceid, refbridgekd, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.Name, T.CodeStr, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.CODE7, T.CODE8, T.CODE9, T.CODE10, T.CODE11, T.REFKVSR, T.REFKVSRBRIDGE, T.REFKDPLAN, T.REFPROGBR, T.SourceID, T.RefBridgeKD,
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_KD_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_MeansType_ExctCachPl;

go

CREATE VIEW DV_MeansType_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, name, RefBridgeMT, REFMTBRIDG, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.Name, T.RefBridgeMT, T.REFMTBRIDG, T.SourceID, 
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_MeansType_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_Regions_ExctCachPl;

go

CREATE VIEW DV_Regions_ExctCachPl
(id, rowtype, pumpid, sourcekey, OKATO, CODE1, CODE2, CODE3, CODE4, name, RefBridgeOKATO, RefRegBridg, RefRegPlan, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.OKATO, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.Name, T.RefBridgeOKATO, T.RefRegBridg,  T.RefRegPlan, T.SourceID, 
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_Regions_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_R_ExctCachPl;

go

CREATE VIEW DV_R_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, name, RefFKR, RefKCSR, RefKVR, RefRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.Name, T.RefFKR, T.RefKCSR, T.RefKVR, T.RefRBridge, T.SourceID, 
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_R_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_EKR_ExctCachPl;

go

CREATE VIEW DV_EKR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefKOSGU, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefKOSGU, T.SourceID, 
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_EKR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_SubKESR_ExctCachPl;

go

CREATE VIEW DV_SubKESR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefSEKRBridge, RefKOSGUBridge, RefKCSRBridge, sourceid, datasourcename, short_name, short_name2)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefSEKRBridge, T.RefKOSGUBridge, T.RefKCSRBridge, T.SourceID, 
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
, T.Name as short_name, substring(T.Name, 256, 255) as short_name2
FROM d_SubKESR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

/* End - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */

/* Start - 12968 - Представления для измерения показателей куба РЕГИОН_Оценка ЭРБС - dianova - 23.03.2010 */

DROP VIEW DV_ERBS_MARKS;
go

CREATE VIEW DV_ERBS_MARKS (ID, CODESTR, NAME, REFGROUP, REFTREND, REFUNITS, REVVALUE, GROUPNAME, GROUPCODESTR, GROUPCOEFF, TRENDNAME, TRENDCODESTR) AS
select t1.id, t1.codestr, t1.name, t1.referbsgroupofmarks, t1.referbs, t1.refunits, t1.revvalue, t2.name , t2.codestr, t2.coeff, t3.name, t3.codestr
from d_Erbs_Marks t1 JOIN d_Erbs_Groupofmarks t2 ON (t1.referbsgroupofmarks=t2.id) JOIN d_Erbs_Trend t3 ON (t1.referbs=t3.id);
go

/* End - 12968 - Представления для измерения показателей куба РЕГИОН_Оценка ЭРБС - dianova - 23.03.2010 */

/* Start - 9425 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.01.2010 */

/* Измерения */
/* Представление для измерения - Организаторы */
drop VIEW DV_STORDER_ORGANIZER;
go

CREATE VIEW DV_STORDER_ORGANIZER (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE, REFORGANIZERBRIDGE) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RefGrbsBridge, Org1.RefOrganizerBridge, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_Tender Tn where Tn.RefOrganizer = Org1.id )
                 or exists (select null from dv.d_Storder_Request Rq where Rq.RefOrganizer = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RefGrbsBridge, Org2.RefOrganizerBridge, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp, T.RefGrbsBridge, T.RefOrganizerBridge
  from Res T;

go

/* Представление для измерения - Специализированные организации */
DROP VIEW DV_STORDER_ORGANIZATION;
go

CREATE VIEW DV_STORDER_ORGANIZATION (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
SELECT id, code, name, address, cubeparentid, kpp
FROM d_StOrder_Organization
WHERE ID in (select distinct REFSPORGANIZER from d_Storder_Tender);
go

/* Представление для измерения - Заказчики */
DROP VIEW DV_STORDER_CUSTOMER;
go

CREATE VIEW DV_STORDER_CUSTOMER  (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE) AS 
SELECT id, code, name, address, cubeparentid, kpp, refgrbsbridge
FROM d_StOrder_Organization
WHERE ID in (select distinct REFCUSTOMER from d_Storder_Request) 
or ID in (select distinct REFCUSTOMER from d_Storder_Contract)
or ID in (select distinct REFCUSTOMER from d_Storder_Agreements);
go

/* Представление для измерения - Поставщики */
DROP VIEW DV_STORDER_SUPPLIER;
go

CREATE VIEW DV_STORDER_SUPPLIER (ID, CODE, NAME, ADDRESS, REFOKATO, CUBEPARENTID, KPP) AS 
SELECT id, code, name, address, refokato, cubeparentid, kpp
from d_StOrder_Organization
WHERE ID in (select distinct REFSUPPLIER from d_Storder_Offer) 
or ID in (select distinct REFSUPPLIER from d_Storder_Contract)
or ID in (select distinct REFSUPPLIER from d_storder_agreements);
go

/* Представление для измерения - Пользователи (пользователи сайта) */
DROP VIEW DV_STORDER_PERSONSITE;
go

CREATE VIEW DV_STORDER_PERSONSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
SELECT id, code, name, address, cubeparentid, kpp
FROM d_StOrder_Organization
WHERE ID in (select distinct REFPERSON from d_Storder_SiteUser);
go

/* Представление для измерения - Организации (пользователи сайта) */
DROP VIEW DV_STORDER_ORGSITE;
go

CREATE VIEW DV_STORDER_ORGSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
SELECT id, code, name, address, cubeparentid, kpp
FROM d_StOrder_Organization
WHERE ID in (select distinct REFORGANIZATION from d_Storder_SiteUser);
go

/* Представление для измерения - Исполнители */
DROP VIEW DV_STORDER_EXECUTOR;
go

CREATE VIEW DV_STORDER_EXECUTOR (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
SELECT id, code, name, address, cubeparentid, kpp
FROM d_StOrder_Organization
WHERE ID in (select distinct d_Storder_Request.RefExecutor from d_Storder_Request) 
or ID in (select distinct d_Storder_Tender.RefExecutor from d_Storder_Tender);
go

/* Представление для измерения - Состояния сведений об ИспПрДействия */
DROP VIEW DV_STORDER_STATUSORDEREXE;
go

CREATE VIEW DV_STORDER_STATUSORDEREXE AS 
SELECT t2.ID, t2.Code, t2.Name
FROM d_StOrder_DocumentsStatus t2
WHERE t2.ID in (select distinct t1.RefExeStatus from d_Storder_Contract t1);
go

/* Представление для измерения - Год закупки */
DROP VIEW DV_STORDER_REQUESTYEAR;
go

CREATE VIEW DV_STORDER_REQUESTYEAR AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE t.parentid is null;
go

/* Представление для измерения - Дата начала договора */
DROP VIEW DV_DATE_AGREEMENTBEG;
go

CREATE VIEW DV_DATE_AGREEMENTBEG AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

/* Представление для измерения - Дата завершения контракта */
DROP VIEW DV_DATE_CONTRACTPLEND;
go

CREATE VIEW DV_DATE_CONTRACTPLEND AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

/* Представление для измерения - Состояния документа */
DROP VIEW DV_STORDER_STATUSAGREE;
go

CREATE VIEW DV_STORDER_STATUSAGREE AS 
SELECT t2.ID, t2.Code, t2.Name
FROM d_StOrder_DocumentsStatus t2
WHERE t2.ID in (select distinct t1.RefDocStatus from d_StOrder_Agreements t1);
go

/* Представление для измерения - Тип документа */
DROP VIEW DV_STORDER_DOCTYPEAGREE;
go

CREATE VIEW DV_STORDER_DOCTYPEAGREE AS
select t1.Id, cast(t1.name as VARCHAR(255)) Name
from d_StOrder_DocType t1
where t1.Id in (select distinct t2.RefDocType from d_StOrder_Agreements t2);
go

/* Представление для измерений класса ДаНет */
DROP VIEW DV_STORDER_YESNO;
go

CREATE VIEW DV_STORDER_YESNO AS 
select * from dv.fx_storder_smbusiness t;
go

/* Представление для измерения - Тип заявки */
DROP VIEW DV_STORDER_REQUESTTYPE;
go

CREATE VIEW DV_STORDER_REQUESTTYPE AS 
select t.id, 1 Code, 'Заявка на закупку' Name  from d_storder_doctype t where t.code in (205,258)
union
select t.id, 2 Code, 'План закупок' Name from d_storder_doctype t where t.code in (672,673)
union
select -1 id, -1 Code, 'Тип не указан' Name from d_storder_doctype;
go

/* Представление для измерения - Преимущества */
DROP VIEW DV_STORDER_PREFERENCES;
go

CREATE VIEW DV_STORDER_PREFERENCES (ID, NAME) AS 
SELECT distinct t.reftenderpreferences, t.tenderpreferences
FROM d_StOrder_Contract t
where t.id>0;
go

/* Факты */
/* Контракты слили вместе со сведениями. Вьюшку вообще можно удалить если начнет работать мера count после нашей консоли */
DROP VIEW FV_STORDER_CONTRACT;
go

CREATE VIEW FV_STORDER_CONTRACT AS
select t1.Id, t1.refacceptdate, t1.refyeardayunv, t1.refterritory, t1.refcustomer, t1.refsupplier, t1.Id RefContract, t1.RefCause, 
t1.refexestatus, t1.refexedate, t1.refexetype, t1.ContractCost, t1.RefDateEnd, t1.actualpayment, 1 Val
from d_StOrder_Contract t1 
where (t1.Id > 0);
go

/* Заявки */
DROP VIEW DV_STORDER_REQUESTS;
go

CREATE VIEW DV_STORDER_REQUESTS AS
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.RequestCost, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, t2.RefTender
FROM DV_STORDER_REQUEST t JOIN DV_STORDER_REQUESTTENDER t2 ON (t.ID = t2.RefRequest)
union
select t.Id, t.SourceId, t.DataSourceName, t.Name, t.Code, t.RequestCost, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, -1 RefTender
FROM DV_STORDER_REQUEST t 
where not t.id  in (select t2.Refrequest from DV_STORDER_REQUESTTENDER t2);
go

DROP VIEW FV_STORDER_REQUEST;
go

CREATE VIEW FV_STORDER_REQUEST AS
select t.Id, t.Id RefRequest, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.RefTender, t.refdocdate, t.purchaseyear, t.RequestCost, 1 Val
from DV_STORDER_REQUESTS t
where t.ID>0;
go

/* Закупки */
DROP VIEW FV_STORDER_TENDER;
go

CREATE VIEW FV_STORDER_TENDER  AS
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
go

/* Источники финансирования */
DROP VIEW FV_STORDER_FINANCEDISTRIB;
go

CREATE VIEW FV_STORDER_FINANCEDISTRIB AS
select t1.refkif, t1.acceptedamount, t2.refproduct, t2.refrequest, t2.refekr, t2.refkvr, t2.refmeasure, t3.refperiod, t3.refterritory
from d_Storder_Financedistribution t1 JOIN d_storder_askitems t2 ON (t1.Refaskitems=t2.id) JOIN d_storder_request t3 ON (t2.refrequest=t3.id)
where t1.Refaskitems >0;
go

/*Пользователи сайта*/
DROP VIEW FV_STORDER_SITEUSER;
go

CREATE VIEW FV_STORDER_SITEUSER AS
select t.Refperson, t.reforganization, t.refdateregister, t.refdatediscard, t.RefIsActive, 1 Val
from d_storder_siteuser t
where t.id>0;
go

DROP VIEW FV_STORDER_EXEORDER;
go

CREATE VIEW FV_STORDER_EXEORDER AS
select 
t1.RefRequest, t1.RefProduct, t1.RefMeasure, t1.RefKVR, t1.RefEKR, t1.Quantity QuantityAsk, t1.Price PriceAsk, t1.Cost CostAsk,
t2.SourceID, t2.RefPeriod, t2.RefTerritory,
t3.RefLot, t3.RefContract,t3.MinPrice, t3.Quantity QuantityContract, t3.Price PriceContract, t3.Cost CostContract, 
t4.RefTender
from dv_StOrder_AskItems t1, d_StOrder_Request t2, fv_StOrder_StrLotOfferContract t3, d_StOrder_Lot t4 
where (t2.ID = t1.RefRequest)
and (t3.ID = t1.RefLotItems)
and (t4.ID = t3.RefLot);
go

/* Убираем ненужные представления */
DROP VIEW FV_STORDER_CONTRACTCOST;
DROP VIEW FV_STORDER_CONTRACTWITHCOST;
DROP VIEW DV_STORDER_ASKITEMSPR;
DROP VIEW FV_STORDER_ASKITEM;
DROP VIEW FV_STORDER_ASKITEMFIN;
go

/* Start - Пересоздание вьюшки, в таблицу добавили новое поле - zaharchenko - 05.05.2010 */

drop VIEW DV_REGIONS_MONTHREP;

go

CREATE VIEW DV_REGIONS_MONTHREP
AS
SELECT T.*, T.Name as short_name
FROM d_Regions_MonthRep as T;

go

/* End - Пересоздание вьюшки, в таблицу добавили новое поле - zaharchenko - 05.05.2010 */

/* Start - Вьюха для интерфейса формы 2п - chubov */

create view v_forecast_val_form2p as
select te.id, te.refparametrs, te.refforecasttype, (te.value) as est, (tf1.value) as y1, (tf2.value) as y2,
(tf3.value) as y3, (tr1.value) as r1,(tr2.value) as r2, te.yearof, te.refvarf2p
from t_forecast_paramvalues te
left join t_forecast_paramvalues tf1 on (te.refparametrs=tf1.refparametrs) and (te.yearof = tf1.yearof-1) and (te.refvarf2p = tf1.refvarf2p)
left join t_forecast_paramvalues tf2 on (te.refparametrs=tf2.refparametrs) and (te.yearof = tf2.yearof-2) and (te.refvarf2p = tf2.refvarf2p)
left join t_forecast_paramvalues tf3 on (te.refparametrs=tf3.refparametrs) and (te.yearof = tf3.yearof-3) and (te.refvarf2p = tf3.refvarf2p)
left join t_forecast_paramvalues tr1 on (te.refparametrs=tr1.refparametrs) and (te.yearof = tr1.yearof+2) and (te.refvarf2p = tr1.refvarf2p)
left join t_forecast_paramvalues tr2 on (te.refparametrs=tr2.refparametrs) and (te.yearof = tr2.yearof+1) and (te.refvarf2p = tr2.refvarf2p)
where te.paramtype=2;

go

/* End - Вьюха для интерфейса формы 2п - chubov */

/* Start - 13894 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.07.2010 */

/* Измерения */

/* Представление для измерения - Решения */
DROP VIEW DV_STORDER_LOTRESOLVE;
go

CREATE VIEW DV_STORDER_LOTRESOLVE AS 
select distinct t.Code ID, t.Name from d_storder_resolvedlot t;
go

/* Представление для измерения Заявки - нужно без плановых заявок */
DROP VIEW DV_STORDER_REQUESTNOPLAN;
go

CREATE VIEW DV_STORDER_REQUESTNOPLAN
(id, rowtype, sourceid, pumpid, sourcekey, code, name, datemodify, requestcost, refcustomer, refperiod, refterritory, refdocstatus, refexecutor, reforganizer, refdoctype, reftypetender, refdocdate, purchaseyear, refforsmb, datasourcename)
AS
SELECT T.ID, T.RowType, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name, T.DateModify, T.RequestCost, T.RefCustomer, T.RefPeriod, T.RefTerritory, T.RefDocStatus, T.RefExecutor, T.RefOrganizer, T.RefDocType, T.RefTypeTender, T.RefDocDate, T.PurchaseYear, T.RefForSMB,
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
FROM d_StOrder_Request T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)
JOIN d_storder_doctype DT ON (T.Refdoctype=DT.ID)
WHERE DT.Code in (205, 258);
go

/* Представление для измерения - Организаторы */
DROP VIEW DV_STORDER_ORGANIZER;
go

CREATE VIEW DV_STORDER_ORGANIZER (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE, REFORGANIZERBRIDGE) AS 
SELECT id, code, name, address, cubeparentid, kpp, refgrbsbridge, RefOrganizerBridge
FROM d_StOrder_Organization
WHERE ID in (select distinct REFORGANIZER from d_Storder_Tender) 
or ID in (select distinct REFORGANIZER from d_Storder_Request)
or ID=CUBEPARENTID;
go

/* Представление для измерения - Специализированные организации */
DROP VIEW DV_STORDER_SPECIALORG;
go

CREATE VIEW DV_STORDER_SPECIALORG (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_Tender Tn where Tn.RefSpOrganizer = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp
  from Res T;

go

/* Представление для измерения - Заказчики */
DROP VIEW DV_STORDER_CUSTOMER;
go

CREATE VIEW DV_STORDER_CUSTOMER  (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP, REFGRBSBRIDGE) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RefGrbsBridge, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_Request Rq where Rq.RefCustomer = Org1.id)
                 or exists (select null from dv.d_Storder_Contract Cn where Cn.RefCustomer = Org1.id)
                 or exists (select null from dv.d_Storder_Agreements Ag where Ag.RefCustomer = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RefGrbsBridge, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp, T.RefGrbsBridge
  from Res T;

go

/* Представление для измерения - Поставщики */
DROP VIEW DV_STORDER_SUPPLIER;
go

CREATE VIEW DV_STORDER_SUPPLIER (ID, CODE, NAME, ADDRESS, REFOKATO, CUBEPARENTID, KPP) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.RefOkato, Org1.CubeParentId, Org1.Kpp, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_Offer St where St.RefSupplier = Org1.id)
                 or exists (select null from dv.d_Storder_Contract Cn where Cn.RefSupplier = Org1.id)
                 or exists (select null from dv.d_Storder_Agreements Ag where Ag.RefSupplier = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.RefOkato, Org2.CubeParentId, Org2.Kpp, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.RefOkato, T.CubeParentId, T.Kpp
  from Res T;

go

/* Представление для измерения - Пользователи (пользователи сайта) */
DROP VIEW DV_STORDER_PERSONSITE;
go

CREATE VIEW DV_STORDER_PERSONSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_SiteUser St where St.RefPerson = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp
  from Res T;

go

/* Представление для измерения - Организации (пользователи сайта) */
DROP VIEW DV_STORDER_ORGSITE;
go

CREATE VIEW DV_STORDER_ORGSITE(ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_SiteUser St where St.RefOrganization = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp
  from Res T;

go

/* Представление для измерения - Исполнители */
DROP VIEW DV_STORDER_EXECUTOR;
go

CREATE VIEW DV_STORDER_EXECUTOR (ID, CODE, NAME, ADDRESS, CUBEPARENTID, KPP) AS 
with 
    Res as (select Org1.Id, Org1.Code, Org1.Name, Org1.Address, Org1.CubeParentId, Org1.Kpp, Org1.RowType
              from dv.d_StOrder_Organization Org1
              where exists (select null from dv.d_Storder_Request Rq where Rq.RefExecutor = Org1.id)
                 or exists (select null from dv.d_Storder_Tender Tn where Tn.RefExecutor = Org1.id)
        union all  --Рекурсия до корневой записи дерева
             select Org2.Id, Org2.Code, Org2.Name, Org2.Address, Org2.CubeParentId, Org2.Kpp, Org2.RowType
               from dv.d_StOrder_Organization Org2 INNER JOIN Res R 
                       on Org2.Id = R.CubeParentId 
                       and R.RowType <> 2
            )
select distinct T.Id, T.Code, T.Name, T.Address, T.CubeParentId, T.Kpp
  from Res T;

go

/* Факты */

/* Заявки */
DROP VIEW FV_STORDER_REQUEST;
go

CREATE VIEW FV_STORDER_REQUEST AS
select t.Id, t.Id RefRequest, t.RefForSMB, t.RefCustomer, t.RefPeriod, t.RefTerritory, t.RefDocStatus, 
t.RefTypeTender, t.RefExecutor, t.RefOrganizer, t.RefDocType, t.refdocdate, t.purchaseyear, t.RequestCost, 1 Val
from D_STORDER_REQUEST t
where t.ID>0;
go

/* Размещение заказа */
/* 
1. Заявки беруться только те, по которым организована закупка и есть контракт, п.у. везде JOIN 
*/
DROP VIEW FV_STORDER_EXEORDER;
go

CREATE VIEW FV_STORDER_EXEORDER AS
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
go

/* Закупки */
/* 
1. Внутренний запрос нужен чтобы для каждой закупки определить ее стоимость = сумма стоимостей ее лотов по d_storder_lot.lotcost. В принципе стоимость закупки можно смотреть в кубе по лотам, но каприз постановки - чтобы стоимость была и в этом кубе.
2. LEFT OUTER JOIN нужен, т.к. у закупки лотов вообще может не быть, хотя это недопустимо с т.з. предметной области. JOIN отсечет такие заявки и как результат количество заявок будет расчитываться неправильно
3. 1 TenderCount нужно, т.к. мера количество закупок с функцией аггрегирования Count пока почему то нашей консолью в 2005 не восстанавливается 
*/
DROP VIEW FV_STORDER_TENDER;
go

CREATE VIEW FV_STORDER_TENDER  AS
SELECT t1.ID, t1.RefPeriod, t1.RefTerritory, t1.ID RefTender, t1.RefOrganizer, t1.RefSPOrganizer, t1.RefDocStatus, t1.RefTypeTender, 
t1.RefForSMB, t1.RefDatePublication, t1.RefDateOpening, t1.RefDateConsider, t1.RefDateMatching, t1.RefExecutor, 1 TenderCount, t2.TenderCost
FROM  
     d_StOrder_Tender t1 
LEFT OUTER JOIN 
     (SELECT t.reftender RefTender, SUM(t.lotcost) TenderCost FROM d_storder_lot t GROUP BY t.reftender) t2 
     ON (t1.ID=t2.RefTender)
WHERE t1.ID > 0;
go

/* Лоты */

/* DV_STORDER_LOTLASTRESOLVED - в результате лоты, расширенные последним решением по каждому */

DROP VIEW DV_STORDER_LOTLASTRESOLVED;
go

CREATE VIEW DV_STORDER_LOTLASTRESOLVED  AS
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
go

/* DV_STORDER_LOTOFFERCOUNT - количество предложений на лот, не по всем лотам, а только по тем, по которым предложения есть п.у. далее LEFT OUTER JOIN */

DROP VIEW DV_STORDER_LOTOFFERCOUNT;
go

CREATE VIEW DV_STORDER_LOTOFFERCOUNT  AS
SELECT t.RefLot RefLot, Count(*) OfferCount 
FROM d_storder_purchaselotbids t 
GROUP BY t.reflot;
go

DROP VIEW FV_STORDER_LOT;
go

CREATE VIEW FV_STORDER_LOT  AS
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
go

/* Убираем ненужные представления */
DROP VIEW DV_STORDER_REQUESTS;
DROP VIEW FV_STORDER_TENDERRESOLVLOT;
DROP VIEW FV_STORDER_TENDERRESOLV;
DROP VIEW DV_STORDER_REQUESTTENDER;
DROP VIEW FV_STORDER_STRLOTOFFERCONTRACT;
DROP VIEW DV_STORDER_LOTITEMSMINPRICE;
go

/* End - 13894 - Представления для кубов блока Мониторинг государственных закупок - dianova - 20.07.2010 */

/* Start - FMQ00015192 - Представления для куба ФО_БОР_Оценка ОИВ - dianova - 3.12.2010 */

DROP VIEW DV_BRB_OLAPIDR;
go

CREATE VIEW DV_BRB_OLAPIDR
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, taskid, taskname, taskcode, taskweight, id, idrname, idrcode, idrunit, idrweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS.KINDSOFPARAMS 
WHEN 1 THEN DS.SUPPLIERCODE + ' ' + DS.DATANAME + ' - ' + cast(DS.YEAR as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DT.ID, DT.NAME, DT.CODE, DT.WEIGHTINGOAL,
DI.ID, DI.NAME, DI.CODE, DI.UNIT, DI.WEIGHTINDEXINTASK
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_Brb_Task DT ON (DT.RefGoal=DG.ID)
JOIN d_Brb_Idr DI ON (DI.Reftask=DT.ID);
go

DROP VIEW DV_BRB_OLAPIER;
go

CREATE VIEW DV_BRB_OLAPIER
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, id, iername, iercode, ierunit, ierweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS.KINDSOFPARAMS
WHEN 1 THEN DS.SUPPLIERCODE + ' ' + DS.DATANAME + ' - ' + cast(DS.YEAR as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DI.ID, DI.NAME, DI.CODE, DI.UNIT, DI.WEIGHTINGOAL
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_BRB_IER DI ON (DI.RefGoal=DG.ID);
go

DROP VIEW DV_BRB_OLAPTASK;
go

CREATE VIEW DV_BRB_OLAPTASK
(goalid, goalname, goalsourceid, goalsourcename, goalcode, goalweight, id, taskname, taskcode, taskweight)
AS
SELECT DG.ID, DG.Name, DG.SourceID, 
CASE DS.KINDSOFPARAMS 
WHEN 1 THEN DS.SUPPLIERCODE + ' ' + DS.DATANAME + ' - ' + cast(DS.YEAR as varchar(4)) 
ELSE 'Значение не указано'
END, DG.Code, DG.WeightGoal,
DT.ID, DT.NAME, DT.CODE, DT.WEIGHTINGOAL
FROM d_BRB_Goal DG 
LEFT OUTER JOIN DataSources DS ON (DG.SourceID = DS.ID)
JOIN d_Brb_Task DT ON (DT.RefGoal=DG.ID);
go

/* End - FMQ00015192 - Представления для куба ФО_БОР_Оценка ОИВ - dianova - 3.12.2010 */

/* Start - 15355 - Представления для кубов блока Мониторинг государственных закупок - dianova - 10.12.2010 */

/* Представление для измерения - Преимущества */
DROP VIEW DV_STORDER_PREFERENCES;
go

CREATE VIEW DV_STORDER_PREFERENCES (ID, NAME) AS 
SELECT distinct t.reftenderpreferences, t.tenderpreferences
FROM d_StOrder_Contract t;
go

/* End - 15355 - Представления для кубов блока Мониторинг государственных закупок - dianova - 10.12.2010 */

/* Start - FMQ00015356 - Преобразование логических полей в числовой формат - zaharchenko- 27.12.2010 */

/* Измерения */

/* Представление для измерения - Решения */
DROP VIEW fV_F_BKKUMarks;
go

CREATE VIEW fV_F_BKKUMarks (ID, SourceID, TaskID, Value, Rate, RefMarks, RefYearDayUNV, RefRegions)
AS SELECT T.ID, T.SourceID, T.TaskID, T.Value, cast(Rate as numeric), T.RefMarks, T.RefYearDayUNV, T.RefRegions
FROM f_F_BKKUMarks T;
go

DROP VIEW fV_Marks_FOQualityFM;
go

CREATE VIEW fV_Marks_FOQualityFM (ID, SourceID, TaskID, Value, Rate, Consider, Employ, RefMarks, RefYearDayUNV, RefKVSR)
 AS 
SELECT T.ID, T.SourceID, T.TaskID, T.Value, T.Rate, T.Consider, cast(Employ as numeric), T.RefMarks, T.RefYearDayUNV, T.RefKVSR
FROM f_Marks_FOQualityFM T;

go

DROP VIEW fV_MARKS_FOBKKUMONTHREP;
go

CREATE VIEW fV_MARKS_FOBKKUMONTHREP (ID, SourceID, TaskID, Value, Rate, RefMarks, RefYearMonth, RefRegions,RefYearDayUNV)
AS SELECT T.ID, T.SourceID, T.TaskID, T.Value, cast(Rate as numeric), T.RefMarks, T.RefYearMonth, T.RefRegions, T.RefYearDayUNV
FROM F_MARKS_FOBKKUMONTHREP as T;
go

/* End - FMQ00015356 - Преобразование логических полей в числовой формат - zaharchenko- 27.12.2010 */

/* Start - 15591 - Представления для кубов блока Мониторинг государственных закупок - dianova - 27.01.2011 */

/* Количество лотов, количество предложений на лот, стоимость лотов. Куб - Лоты */

/* Вспомогательная. DV_STORDER_LOTLASTRESOLVED - в результате лоты, расширенные последним решением по каждому */

DROP VIEW DV_STORDER_LOTLASTRESOLVED;
go

CREATE VIEW DV_STORDER_LOTLASTRESOLVED  AS
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
go

/* Вспомогательная. DV_STORDER_LOTOFFEROFFCOUNT - количество принятых предложений на лот, не по всем лотам, а только по тем, по которым предложения есть п.у. далее LEFT OUTER JOIN */

DROP VIEW DV_STORDER_LOTOFFEROFFCOUNT;
go

CREATE VIEW DV_STORDER_LOTOFFEROFFCOUNT  AS
SELECT t.RefLot RefLot, Count(*) OfferOffCount 
FROM d_storder_purchaselotbids t 
WHERE t.refusal = 0
GROUP BY t.reflot;
go

DROP VIEW FV_STORDER_LOT;
go

CREATE VIEW FV_STORDER_LOT  AS
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
go

/* Куб - Размещение заказа */
/* Заявки беруться только те, по которым организована закупка и есть контракт, п.у. везде JOIN */

DROP VIEW FV_STORDER_EXEORDER;
go

CREATE VIEW FV_STORDER_EXEORDER AS
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
go

/* End - 15591 - Представления для кубов блока Мониторинг государственных закупок - dianova - 27.01.2011 */

/* Start - Пересоздание представления, поле Name расширили до 500 символов - zaharchenko - 24.02.2011 */

drop VIEW DV_KSSHK_FOYR;

go

CREATE VIEW DV_KSSHK_FOYR
(id, rowtype, sourceid, pumpid, sourcekey, code, short_name, short_name_part2, refksshkbridge, datasourcename) AS
SELECT T.ID, T.ROWTYPE, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name as short_name, substring(T.Name, 256, 510) as short_name_part2, T.RefKSSHKBridge,
DS.SUPPLIERCODE + ' ' + DS.DATANAME + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS.KINDSOFPARAMS WHEN 0 THEN DS.NAME + ' ' + cast(DS.YEAR as varchar(4)) WHEN 1 THEN cast(DS.YEAR as varchar(4)) WHEN 2 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 3 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.MONTH as varchar(2)) + ' ' + DS.VARIANT WHEN 4 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.VARIANT WHEN 5 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) WHEN 6 THEN cast(DS.YEAR as varchar(4)) + ' ' + DS.TERRITORY WHEN 7 THEN cast(DS.YEAR as varchar(4)) + ' ' + cast(DS.QUARTER as varchar(1)) + ' ' + cast(DS.MONTH as varchar(2)) WHEN 9 THEN DS.VARIANT END ) as varchar(1000)) END
FROM d_KSSHK_FOYR T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW BV_KSSHK_BRIDGE;

go

CREATE VIEW BV_KSSHK_BRIDGE
AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2
FROM B_KSSHK_BRIDGE T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '20ddab01-0f33-4273-881c-f2420462680a' and t2.IsCurrent = 1) or ID = -1;

go

drop VIEW DV_KSSHK_FOProject;

go

CREATE VIEW DV_KSSHK_FOProject 
(ID, RowType, SourceID, PumpID, SourceKey, Code, short_name, short_name_part2, RefKSSHKBridge, DataSourceName)
AS SELECT T.ID, T.RowType, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name as short_name, substring(T.Name, 256, 510) as short_name_part2, T.RefKSSHKBridge,
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_KSSHK_FOProject T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go
/* End - Пересоздание вьюшки, в таблицу добавили новое поле - zaharchenko - 05.05.2010 */

DROP VIEW DV_ORG_RESPON_BRIDGE;
go

CREATE VIEW DV_ORG_RESPON_BRIDGE AS 
select ID, NAME
from D_ORG_RESPON;
go

/*Версионность сопоставимых*/
DROP VIEW BV_EKR_BRIDGE;
go

CREATE VIEW BV_EKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID, T.RefEKRBridge, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3
FROM b_EKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '9a85c0c8-390d-41cb-839c-f57ef54f7ff3' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_FKR_BRIDGE;
go

CREATE VIEW BV_FKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Name, T.ParentID, T.RefFKRBridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3
FROM b_FKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '1acb453b-dd86-438a-83b9-c27ce4fd8bda' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_KVSR_BRIDGE;
go

CREATE VIEW BV_KVSR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.FIO, T.Post, T.WebSite, T.Email, T.Telephone, T.ShortName, T.AddressSkype, T.AddressFaceTime, T.CodeLine, T.RefKVSRBridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3
FROM b_KVSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'eb64ed07-4635-4b25-8452-0b0d119458e3' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_KVR_BRIDGE;
go

CREATE VIEW BV_KVR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.RefKVRBridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3, substring(T.Name, 766, 255) as Short_Name_Part4
FROM b_KVR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c073b576-58dd-4873-9806-55ec0ce93929' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_FACIALACC_BRIDGE;
go

CREATE VIEW BV_FACIALACC_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Name, T.KVSRID, T.KVSRCode, T.KVSRName, T.KCSRID, T.KCSRCode, T.KCSRName, T.KFSRID, T.KFSRCode, T.KFSRName, T.KVRID, T.KVRCode, T.KVRName, T.KESRID, T.KESRCode, T.KESRName, T.FinTypeID, T.FinTypeCode, T.FinTypeName, T.RegionClsID, T.RegionClsCode, T.RegionClsName, T.MeansTypeID, T.MeansTypeCode, T.MeansTypeName, T.OrgID, T.OrgCode, T.OrgName, T.GeneralOrgID, T.GeneralOrgCode, T.GeneralOrgName, T.HigherOrgID, T.HigherOrgCode, T.HigherOrgName, T.ParentID, T.RefFABridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2
FROM b_FacialAcc_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c51c2054-1e9b-411a-85a3-b9f6e16ec699' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_OKVED_BRIDGE;
go

CREATE VIEW BV_OKVED_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Name, T.Section, T.SubSection, T.ShortName, T.ParentID, T.RefKVSRDepartment, T.RefOKVEDBridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3
FROM b_OKVED_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'a5f87962-8af2-4419-8b5a-746a1e3540e8' and t2.IsCurrent = 1) or ID = -1;
go

DROP VIEW BV_MEANSTYPE_BRIDGE;
go

CREATE  VIEW BV_MEANSTYPE_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ActivityKind, T.ParentID, T.RefMTBridge
, T.Name as short_name, substring(T.Name, 256, 255) as Short_Name_Part2, substring(T.Name, 511, 255) as Short_Name_Part3
FROM b_MeansType_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '836220a8-5c5e-4237-a49d-9677be9d6d33' and t2.IsCurrent = 1) or ID = -1;
go
/*Версионность сопоставимых*/


/* Start - 16955 - Представления для измерения территорий куба СТАТ_СЭП_Годовой сборник - dianova - 25.05.2011 */
DROP VIEW DV_TERRITORY_OLAPSEP;
go

CREATE VIEW DV_TERRITORY_OLAPSEP AS
SELECT * 
FROM D_TERRITORY_RF t 
WHERE t.REFTERRITORIALPARTTYPE IN (1,2,3);
go

/* Start - Feature #18115 - Представления для таблиц фактов - zaharchenko- 13.12.2011 */
DROP VIEW fV_r_foyroutcomes;
go

CREATE VIEW fV_r_foyroutcomes 
AS
SELECT *
FROM f_r_foyroutcomes;

go

DROP VIEW FV_F_MONTHREPOUTCOMES;
go

CREATE VIEW FV_F_MONTHREPOUTCOMES
AS
SELECT *
FROM F_F_MONTHREPOUTCOMES;

go

/* Start - FMQ00019517 - Представления для куба ЭО_Прогноз_Планирование - dianova - 08.12.2011 */
DROP VIEW DV_FORECAST_OLAPPMETHODS;
go

CREATE VIEW DV_FORECAST_OLAPPMETHODS (GroupId, GroupName, Id, MethodName) as
select g.Code, g.TextName, M.code, M.TextName
from 
(select t.code, t.textname, t.code1 from d_Forecast_PMethods t where  t.parentid is null) g,
(select t.code, t.textname, t.code1 from d_Forecast_PMethods t where not t.parentid is null) m
where g.Code1 = m.Code1;
go

DROP VIEW DV_FORECAST_OLAPYEAR;
go

CREATE VIEW DV_FORECAST_OLAPYEAR (Id, Name) 
AS
SELECT distinct t.dateyearid, t.dateyear 
FROM fx_date_yeardayunv t
WHERE t.dateyearid > 0;
go

DROP VIEW DV_FORECAST_OLAPPYEAR;
go

CREATE VIEW DV_FORECAST_OLAPPYEAR (Id, Name) 
AS
SELECT distinct t.dateyearid, t.dateyear 
FROM fx_date_yeardayunv t
WHERE t.dateyearid > 0;
go

/* Start - FMQ00019423 - Представление для измерения Исполнение расходов.Реестр программ - dianova - 16.12.2011 */
DROP VIEW DV_EXCCOSTS_OLAPLISTPRG;
go

CREATE VIEW DV_EXCCOSTS_OLAPLISTPRG
(ID, CODE, NAME, SHORTNAME, NOTE, PARENTID, NPA, KBK, REFCREATORS, REFTYPEPROG, REFTERRITORY, CREATOR, TYPEPROG, STATYEAR, BPERIOD, EPERIOD) 
AS
select d.id, d.code, d.name, d.shortname, d.note, d.parentid, d.NPA, d.KBK, d.refcreators, d.reftypeprog, d.refterritory, cp1.name, cp2.name, cp3.name, cp4.name, cp5.name
from d_exccosts_listprg d 
JOIN d_exccosts_creators cp1 on (d.refcreators=cp1.id)
JOIN fx_exccosts_tpprg cp2 on (d.reftypeprog=cp2.id)
JOIN fx_date_yeardayunv cp3 on (d.refapyear=cp3.id)
JOIN fx_date_yeardayunv cp4 on (d.refbegdate=cp4.id)
JOIN fx_date_yeardayunv cp5 on (d.refenddate=cp5.id);
go

/* Start - Feature #17380 Создать кубы по блоку "ФО_0041_Оценка эффективности льгот" - 19.12.2011 */
drop VIEW fV_FX_TypeTax_bridge;
go

CREATE VIEW fV_FX_TypeTax_bridge
AS
SELECT *
FROM fx_FX_TypeTax;

go

/* Start - Feature #19774 ФНС_0030_Куб "ФНС_Задолженность - zaharchenko - 26.12.2011 */
drop VIEW dv_org_fnsdebtor
go

CREATE VIEW dv_org_fnsdebtor (ID, RowType, PumpID, SourceKey, INN, KPP, Name, DATAKIND, IFNS, REFREG, INNNAME)
AS SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.INN, T.KPP, T.Name, T.DATAKIND, T.IFNS, T.REFREG,
CAST(datakind as varchar(50)) + ' '  + CAST(INN as varchar(12)) + ' '  + CAST(KPP as varchar(10)) + ' ' + CAST(name as varchar(500))
FROM d_org_fnsdebtor T;

go

/* Start - Feature #19897 Измерение "Показатели.Мониторинг местных бюджетов" свойство "Форма" - zaharchenko - 12.01.2012 */
drop VIEW DV_FORM_MONITORINGMB
go

CREATE VIEW DV_FORM_MONITORINGMB
AS
SELECT *
FROM D_FORM_MONITORINGMB;

go

drop VIEW DV_FORM_MONITORINGMB_BRIDGE
go

CREATE VIEW DV_FORM_MONITORINGMB_BRIDGE
AS
SELECT *
FROM D_FORM_MONITORINGMB;

go

* Start - Feature #20077 ФО_0035_Информация по объектам_Новосибирск - zaharchenko - 24.01.2012 */
drop VIEW FV_DATE_YEAR
go

CREATE VIEW FV_DATE_YEAR AS
SELECT * 
FROM fx_date_year;

go

/* Start - Feature #17798 ФО_0001_АС Бюджет_Новые измерения Код целевых средств и Вид плана - zaharchenko - 30.01.2012 */
drop VIEW BV_TRANSFERT_BRIDGE;
go

CREATE VIEW BV_TRANSFERT_BRIDGE AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM b_Transfert_Bridge T;

go

drop VIEW DV_TRANSFERT_BUDGET;
go

CREATE VIEW DV_TRANSFERT_BUDGET AS
SELECT T.*, T.Name as short_name, substring(T.Name, 256, 255) as short_name_part2, substring(T.Name, 511, 255) as short_name_part3, substring(T.Name, 766, 255) as short_name_part4
FROM d_Transfert_Budget T;

go

/* Start - FMQ00020383 - Представления для измерений дат блока ООС - dianova - 15.02.2012 */
DROP VIEW DV_OOS_OLAPPUBLICDATE;
go

CREATE VIEW DV_OOS_OLAPPUBLICDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPGIVEDATE;
go

CREATE VIEW DV_OOS_OLAPGIVEDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPCONSDATE;
go

CREATE VIEW DV_OOS_OLAPCONSDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPMATCHDATE;
go

CREATE VIEW DV_OOS_OLAPMATCHDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPRESULTDATE;
go

CREATE VIEW DV_OOS_OLAPRESULTDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPPUBCONTRDATE;
go

CREATE VIEW DV_OOS_OLAPPUBCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPSIGCONTRDATE;
go

CREATE VIEW DV_OOS_OLAPSIGCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPPROTCONTRDATE;
go

CREATE VIEW DV_OOS_OLAPPROTCONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV;
go

DROP VIEW DV_OOS_OLAPEXECONTRDATE;
go

CREATE VIEW DV_OOS_OLAPEXECONTRDATE AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE (t.DateDayID < -1) or (t.ID = -1);
go

DROP VIEW DV_OOS_OLAPTYPEPURCONTR;
go

CREATE VIEW DV_OOS_OLAPTYPEPURCONTR AS 
SELECT *
FROM FX_OOS_TYPEPURCH;
go

/* Start - Представление для измерения Тип территориий, бля использования в Районы.Планирование - zaharchenko - 16.03.2012 */
drop VIEW dv_FX_FX_TERRITORIALPART_br;
go

create view dv_FX_FX_TERRITORIALPART_br as
select *
from FX_FX_TERRITORIALPARTITIONTYPE;

go

/* Start - FMQ00020786 - Представления для измерений дат блока ООС - dianova - 22.03.2012 */
DROP VIEW DV_OOS_OLAPTYPEPURPL;
go

CREATE VIEW DV_OOS_OLAPTYPEPURPL AS 
SELECT *
FROM FX_OOS_TYPEPURCH;
go

DROP VIEW DV_OOS_OLAPPUBPLDATEPL;
go

CREATE VIEW DV_OOS_OLAPPUBPLDATEPL (ID,DATEYEARID,DATEYEAR,DATEHALFYEARID,DATEHALFYEAR,DATEQUARTERID,DATEQUARTER,DATEMONTHID,DATEMONTH,DATEDAY,ORDERBYDEFAULT)
AS 
SELECT t.id, 
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
go

DROP VIEW DV_OOS_OLAPPUBDATEPL;
go

CREATE VIEW DV_OOS_OLAPPUBDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));
go

DROP VIEW DV_OOS_OLAPBEGINDATEPL;
go

CREATE VIEW DV_OOS_OLAPBEGINDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));
go

DROP VIEW DV_OOS_OLAPENDDATEPL;
go

CREATE VIEW DV_OOS_OLAPENDDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));
go

DROP VIEW DV_OOS_OLAPUPDATEPL;
go

CREATE VIEW DV_OOS_OLAPUPDATEPL AS 
SELECT *
FROM fx_Date_YearDayUNV t
WHERE not ((t.DATEDAYID < 0) or (t.DATEDAY = 'Заключительные обороты'));
go

DROP VIEW DV_OOS_OLAPEXEDATEPL;
go

CREATE VIEW DV_OOS_OLAPEXEDATEPL (ID,DATEYEARID,DATEYEAR,DATEHALFYEARID,DATEHALFYEAR,DATEQUARTERID,DATEQUARTER,DATEMONTHID,DATEMONTH,DATEDAY,ORDERBYDEFAULT)
AS 
SELECT t.id, 
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
go


:on error exit
