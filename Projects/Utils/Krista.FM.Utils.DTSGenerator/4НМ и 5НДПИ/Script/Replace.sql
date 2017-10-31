/********************************************************************
	Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
whenever SQLError exit rollback; 
/* End   - Стандартная часть */ 


/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

-- переименуем Олины источники, кот она заполнила у себя дома по ФНС РФ

-------------------------------------------

alter table d_Territory_FNSRF disable all triggers;

update  d_Territory_FNSRF t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table d_Territory_FNSRF enable all triggers;
queryString
------------------------------------------

alter table b_Arrears_FNSBridge disable all triggers;

update  b_Arrears_FNSBridge t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table b_Arrears_FNSBridge enable all triggers;

-------------------------------------------

alter table b_Arrears_FNSBridge disable all triggers;

update  b_Arrears_FNSBridge t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table b_Arrears_FNSBridge enable all triggers;

-----------------------------------------------

alter table d_Arrears_FNS disable all triggers;

update  d_Arrears_FNS t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table d_Arrears_FNS enable all triggers;

----------------------------------------------

alter table f_D_FNSRF4NM disable all triggers;

update  f_D_FNSRF4NM t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table f_D_FNSRF4NM enable all triggers;

-------------------------------------------

alter table b_Marks_FNSRF5NDPIBridge disable all triggers;

update  b_Marks_FNSRF5NDPIBridge t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table b_Marks_FNSRF5NDPIBridge enable all triggers;

----------------------------------------------

alter table d_Marks_FNSRF5NDPI disable all triggers;

update  d_Marks_FNSRF5NDPI t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table d_Marks_FNSRF5NDPI enable all triggers;

-------------------------------------------

alter table f_D_FNSRF5NDPI disable all triggers;

update  f_D_FNSRF5NDPI t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table f_D_FNSRF5NDPI enable all triggers;

-------------------------------------------

alter table b_D_Group disable all triggers;

update  b_D_Group t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table b_D_Group enable all triggers;

-------------------------------------------

alter table d_d_groupfns disable all triggers;

update  d_d_groupfns t set t.sourceID = t.sourceID + 7000 where t.sourceID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 45, 46, 47, 65, 66, 67, 68, 69, 85, 86, 87, 88, 105, 106, 107, 108, 109, 110, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 145, 165, 166, 167, 168, 169, 170, 171, 172, 173);

alter table d_d_groupfns enable all triggers;


----------------------------------------
-- убираем ссылку на Территории.Сопоставимый

alter table d_Territory_FNSRF disable all triggers;

update d_Territory_FNSRF t set t.refterritoryrfbridge = -1;

alter table d_Territory_FNSRF enable all triggers;

----------------------------------------

  
alter table OBJECTVERSIONS
  disable constraint FKVERSIONSREFSOURCE;

update hub_datasources t set t.id = t.id + 7000;


commit;


/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
