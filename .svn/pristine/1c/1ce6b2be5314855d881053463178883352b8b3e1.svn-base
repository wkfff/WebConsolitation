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

/* Представление с полем name, разбитым на 3 части для измерения "Фонды.Типы фондов" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW DV_FUND_FO9TYPES
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_Fund_FO9Types T;

/* Представление с полем name, разбитым на 3 части для измерения "КЦСР.Сопоставимый планирование" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW BV_KCSR_BRIDGEPLAN
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_KCSR_BridgePlan T;

CREATE OR REPLACE VIEW DV_KD_KCSYSTEMA_bridge
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4, substr(T.Name, 1021, 255) as short_name_part5
FROM D_KD_KCSYSTEMA T;


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

/* Start - 7861 - Представление с полем name, разбитым на 3 части для измерения "КИФ.АС Бюджет 2005" - zaharchenko- 14.02.2008 */
/* Удалить после перхода на 2005 */

CREATE OR REPLACE VIEW DV_KIF_BUDGET2005
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM D_KIF_BUDGET2005 T;

/* End - 7861 - Представление с полем name, разбитым на 3 части для измерения "КИФ.АС Бюджет 2005" - zaharchenko- 14.02.2008 */

/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW DV_ORG_UFKPAYERS
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM D_ORG_UFKPAYERS T;


/* Start - Представление с полем name, разбитым на 3 части для измерения "Организации.УФК_Плательщики" */
/* Удалить после перхода на 2005 */
CREATE OR REPLACE VIEW bv_org_payersbridge
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_org_payersbridge T;


/* Start - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */

CREATE OR REPLACE VIEW BV_KD_BRIDGE
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_KD_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '5cd4f631-6276-4a9f-b466-980282500b50' and t2.IsCurrent = 1) or ID = -1;

/* End - 10459 -  КД.Сопоставимый дополнительные поля  - zaharchenko - 10.04.2009 */

/* Start - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */

CREATE OR REPLACE VIEW Bv_Kd_Bridgeplan
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_Kd_Bridgeplan T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '65fefc22-6135-4ee0-8fc3-3801a368991a' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Bv_r_bridge
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0a626485-8481-4058-aa0f-a917df395f3c' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Bv_r_Bridgeplan
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgeplan T;

CREATE OR REPLACE VIEW Bv_r_Bridgerep
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_r_Bridgerep T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '8319a6d9-2adf-417d-9f93-8b0c12ec071c' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW Dv_r_Plan
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_r_Plan T;

CREATE OR REPLACE VIEW Dv_r_Admprojectoutcome
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_r_Admprojectoutcome T;

CREATE OR REPLACE VIEW BV_KIF_BRIDGE
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, kvsr, descriptioncode, maindescriptioncode, itemcode, programcode, kesr, parentid, refdirection, refclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.KVSR, T.DescriptionCode, T.MainDescriptionCode, T.ItemCode, T.ProgramCode, T.KESR, T.ParentID, T.RefDirection, T.RefClsAspect
, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM b_KIF_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '0773168f-923d-4140-98cc-01328f353e40' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KIF_BRIDGEPLAN
(id, rowtype, sourceid, codestr, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, parentid, refkif, refkifclsaspect, short_name, short_name_part2)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.CodeStr, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Code8, T.Code9, T.Code10, T.Name, T.ParentID, T.RefKIF, T.RefKIFClsAspect
, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM b_KIF_BridgePlan T;

CREATE OR REPLACE VIEW BV_KCSR_BRIDGE
(id, rowtype, sourceid, code, code1, code2, code3, name, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID
, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM b_KCSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'ee980110-fa1f-43c4-b03e-b92c6fec5035' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW DV_R_FO9KINDS
(id, rowtype, code, name, note, parentid, short_name, short_name_part2, short_name_part3)
AS
SELECT T.ID, T."ROWTYPE", T.Code, T.Name, T.Note, T.ParentID
, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_R_FO9Kinds T;

CREATE OR REPLACE VIEW dv_r_Fo14grbs AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3
FROM d_r_Fo14grbs T;

/* End - 10990 -  Выводим в измерениях свойства, в которых поле "Наименование" длиннее 256 символов  - zaharchenko - 02.07.2009 */

/* Start - Пересоздание представления, поле Name расширили до 500 символов - zaharchenko - 20.02.2011 */

CREATE OR REPLACE VIEW DV_KSSHK_FOYR
(id, rowtype, sourceid, pumpid, sourcekey, code, short_name, short_name_part2, refksshkbridge, datasourcename)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.PumpID, T.SourceKey, T.Code, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2,T.RefKSSHKBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_KSSHK_FOYR T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW BV_KSSHK_BRIDGE
AS
SELECT T.*, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2
FROM B_KSSHK_BRIDGE T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '20ddab01-0f33-4273-881c-f2420462680a' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW DV_KSSHK_FOPROJECT
(id, rowtype, sourceid, pumpid, sourcekey, code, short_name, short_name_part2, refksshkbridge, datasourcename)
AS
SELECT T.ID, T."ROWTYPE", T.SourceID, T.PumpID, T.SourceKey, T.Code, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name_part2, T.RefKSSHKBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
FROM d_KSSHK_FOProject T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);
/* End - Пересоздание представления, поле Name расширили до 500 символов - zaharchenko - 20.02.2011 */

/*Версионность сопоставимых*/
CREATE OR REPLACE VIEW BV_EKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ParentID, T.RefEKRBridge, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_EKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '9a85c0c8-390d-41cb-839c-f57ef54f7ff3' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_FKR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Name, T.ParentID, T.RefFKRBridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_FKR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '1acb453b-dd86-438a-83b9-c27ce4fd8bda' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KVSR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.FIO, T.Post, T.WebSite, T.Email, T.Telephone, T.ShortName, T.AddressSkype, T.AddressFaceTime, T.CodeLine, T.RefKVSRBridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_KVSR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'eb64ed07-4635-4b25-8452-0b0d119458e3' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_KVR_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Name, T.RefKVRBridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3, substr(T.Name, 766, 255) as Short_Name_Part4
FROM b_KVR_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c073b576-58dd-4873-9806-55ec0ce93929' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_FACIALACC_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Name, T.KVSRID, T.KVSRCode, T.KVSRName, T.KCSRID, T.KCSRCode, T.KCSRName, T.KFSRID, T.KFSRCode, T.KFSRName, T.KVRID, T.KVRCode, T.KVRName, T.KESRID, T.KESRCode, T.KESRName, T.FinTypeID, T.FinTypeCode, T.FinTypeName, T.RegionClsID, T.RegionClsCode, T.RegionClsName, T.MeansTypeID, T.MeansTypeCode, T.MeansTypeName, T.OrgID, T.OrgCode, T.OrgName, T.GeneralOrgID, T.GeneralOrgCode, T.GeneralOrgName, T.HigherOrgID, T.HigherOrgCode, T.HigherOrgName, T.ParentID, T.RefFABridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2
FROM b_FacialAcc_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'c51c2054-1e9b-411a-85a3-b9f6e16ec699' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_OKVED_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Code4, T.Code5, T.Code6, T.Code7, T.Name, T.Section, T.SubSection, T.ShortName, T.ParentID, T.RefKVSRDepartment, T.RefOKVEDBridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_OKVED_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = 'a5f87962-8af2-4419-8b5a-746a1e3540e8' and t2.IsCurrent = 1) or ID = -1;

CREATE OR REPLACE VIEW BV_MEANSTYPE_BRIDGE AS
SELECT T.ID, T.RowType, T.SourceID, T.Code, T.Code1, T.Code2, T.Code3, T.Name, T.ActivityKind, T.ParentID, T.RefMTBridge
, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as Short_Name_Part2, substr(T.Name, 511, 255) as Short_Name_Part3
FROM b_MeansType_Bridge T where sourceID in (select sourceID from ObjectVersions t2 where t2.ObjectKey = '836220a8-5c5e-4237-a49d-9677be9d6d33' and t2.IsCurrent = 1) or ID = -1;

/*Версионность сопоставимых*/

/* Start - Feature #17798 ФО_0001_АС Бюджет_Новые измерения Код целевых средств и Вид плана - zaharchenko - 30.01.2012 */

CREATE OR REPLACE VIEW BV_TRANSFERT_BRIDGE AS
SELECT T.*, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM b_Transfert_Bridge T;

CREATE OR REPLACE VIEW DV_TRANSFERT_BUDGET AS
SELECT T.*, substr(T.Name, 1, 255) as Short_Name, substr(T.Name, 256, 255) as short_name_part2, substr(T.Name, 511, 255) as short_name_part3, substr(T.Name, 766, 255) as short_name_part4
FROM d_Transfert_Budget T;

/* Start - Feature #20705 Увеличить "наименование" в "СубКОСГУ_ИспКасПлан" для версии 3.1 - zaharchenko - 15.03.2012 */
CREATE OR REPLACE VIEW DV_SUBKESR_EXCTCACHPL
(id, rowtype, sourceid, pumpid, sourcekey, code, name, refsekrbridge, refkosgubridge, refkcsrbridge, datasourcename, short_name, short_name2)
AS
SELECT T.ID, T.RowType, T.SourceID, T.PumpID, T.SourceKey, T.Code, T.Name, T.RefSEKRBridge, T.RefKOSGUBridge, T.RefKCSRBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, substr(T.Name, 1, 255) as short_name, substr(T.Name, 256, 255) as short_name2
FROM d_SubKESR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

commit work;

whenever SQLError exit rollback;