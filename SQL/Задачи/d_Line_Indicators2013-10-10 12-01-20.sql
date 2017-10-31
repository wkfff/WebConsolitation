﻿print N'Отключение триггеров'
GO

disable trigger t_d_Line_Indicators_aa on [DV].[d_Line_Indicators];
GO

print N'Вставляем данные'
GO

INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (1,0,'010','Основные средства (балансовая стоимость, 010100000), всего',1)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (2,0,'011','недвижимое имущество учреждения (010110000)',2)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (3,0,'012','особо ценное движимое имущество учреждения (010120000)*',3)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (4,0,'013','иное движимое имущество учреждения (010130000)',4)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (5,0,'014','предметы лизинга (010140000)',5)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (6,0,'020','Амортизация основных средств',6)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (7,0,'021','амортизация недвижимого имущества учреждения (010410000)',7)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (8,0,'022','амортизация особо ценного движимого имущества учреждения (010420000)* ',8)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (9,0,'023','амортизация иного движимого имущества учреждения (010430000)',9)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (10,0,'024','амортизация предметов лизинга (010440000)',10)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (11,0,'030','Основные средства (остаточная стоимость, стр. 010 - стр. 020)        ',11)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (12,0,'031','недвижимое имущество учреждения (остаточная стоимость,
стр. 011 - стр. 021)',12)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (13,0,'032','особо ценное движимое имущество учреждения (остаточная стоимость, стр. 012 -  стр. 022)         ',13)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (14,0,'033','иное движимое имущество учреждения (остаточная стоимость,   стр. 013 - стр. 023)',14)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (15,0,'034','предметы лизинга (остаточная стоимость, стр. 014 - стр. 024)',15)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (16,0,'040','Нематериальные активы (балансовая стоимость, 010200000)*, всего     ',16)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (17,0,'041','особо ценное движимое имущество учреждения (010220000)*',17)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (18,0,'042','иное движимое имущество учреждения (010230000)*',18)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (19,0,'043','предметы лизинга (010240000)*',19)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (20,0,'050','Амортизация нематериальных активов *',20)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (21,0,'051','особо ценное движимое имущество учреждения (010429000)*',21)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (22,0,'052','иного движимого имущества учреждения (010439000)*',22)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (23,0,'053','предметов лизинга (010449000)*',23)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (24,0,'060','Нематериальные активы (остаточная стоимость, стр. 040 - стр. 050)      ',24)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (25,0,'061','особо ценное имущество учреждения (остаточная стоимость, стр. 041 - стр. 051)',25)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (26,0,'062','иное движимое имущество учреждения (остаточная стоимость,   стр. 042 - стр. 052)',26)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (27,0,'063','предметы лизинга (остаточная стоимость, стр. 043 - стр. 053)',27)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (28,0,'070','Непроизведенные активы (балансовая стоимость, 010300000)   ',28)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (29,0,'080','Материальные запасы (010500000)',29)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (30,0,'081','особо ценное движимое имущество учреждения (010520000)*',30)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (31,0,'090','Вложения в нефинансовые активы (010600000)',31)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (32,0,'091','в недвижимое имущество учреждения (010610000)',32)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (33,0,'092','в особо ценное движимое имущество учреждения (010620000)',33)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (34,0,'093','в иное движимое имущество учреждения (010630000)',34)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (35,0,'094','в предметы лизинга (010640000)',35)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (36,0,'100','Нефинансовые активы в пути (010700000)',36)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (37,0,'101','недвижимое имущество учреждения в пути (010710000)',37)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (38,0,'102','особо ценное имущество учреждения в пути (010720000)',38)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (39,0,'103','иное движимое имущество учреждения в пути (010730000)',39)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (40,0,'104','предметы лизинга в пути (010740000)',40)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (41,0,'110','Нефинансовые активы имущества казны (балансовая стоимость, 010800000)*',41)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (42,0,'120','Амортизация имущества, составляющего казну (010450000)*',42)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (43,0,'130','Нефинансовые активы имущества казны  (остаточная стоимость, стр. 110 - стр. 120)',43)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (44,0,'140','Затраты на изготовление готовой продукции, выполнение работ, услуг (010900000)',44)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (45,0,'150','Итого по разделу I
(стр. 030 + стр. 060 + стр. 070 + стр. 080 + стр. 090 + стр. 100 + стр. 140)',45)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (46,0,'150','Итого по разделу I
(стр. 030 + стр. 060 + стр. 070 + стр. 080 + стр. 090 + стр. 100 + стр. 130 + стр. 140)',46)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (47,0,'170','Денежные средства учреждения (020100000) в том числе: ',47)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (48,0,'171','денежные средства учреждения на лицевых счетах в органе казначейства (020111000)',48)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (49,0,'172','денежные средства учреждения в органе казначейства в пути (020113000)',49)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (50,0,'173','денежные средства учреждения на счетах в кредитной организации (020121000)',50)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (51,0,'174','денежные средства учреждения в кредитной организации в пути (020123000)',51)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (52,0,'175','аккредитивы на счетах учреждения в кредитной организации (020126000)',52)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (53,0,'176','денежные средства учреждения в иностранной валюте на счетах в кредитной организации (020127000)',53)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (54,0,'177','касса (020134000)',54)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (55,0,'178','денежные документы (020135000)',55)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (56,0,'179','денежные средства учреждения, размещенные на депозиты в кредитной организации (020122000)',56)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (57,0,'210','Финансовые вложения (020400000) в том числе:',57)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (58,0,'211','ценные бумаги, кроме акций  (020420000)',58)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (59,0,'212','акции и иные формы участия в капитале (020430000)',59)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (60,0,'213','иные финансовые активы (020450000)',60)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (61,0,'230','Расчеты по доходам (020500000)',61)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (62,0,'260','Расчеты по выданным авансам (020600000)',62)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (63,0,'290','Расчеты по кредитам, займам (ссудам) (020700000) в том числе:',63)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (64,0,'291','по предоставленным кредитам, займам (ссудам) (020710000)',64)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (65,0,'292','в рамках целевых иностранных кредитов (заимствований) (020720000)',65)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (66,0,'293','с дебиторами по государственным (муниципальным) гарантиям (020730000)',66)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (67,0,'310','Расчеты с подотчетными лицами (020800000)',67)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (68,0,'320','Расчеты по ущербу имуществу (020900000)',68)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (69,0,'330','Прочие расчеты с дебиторами (021000000)',69)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (70,0,'331','из них:
расчеты по НДС по приобретенным материальным ценностям, работам, услугам (021001000)',70)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (71,0,'333','расчеты с финансовым органом по наличным денежным средствам (021003000)',71)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (72,0,'335','расчеты с прочими дебиторами (021005000)',72)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (73,0,'336','расчеты с учредителем (021006000)*',73)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (74,0,'337','показатель уменьшения балансовой стоимости ОЦИ *',74)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (75,0,'338','чистая стоимость ОЦИ (стр. 336 + стр. 337)',75)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (76,0,'370','Вложения в финансовые активы (021500000) в том числе:',76)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (77,0,'371','ценные бумаги, кроме акций (021520000)',77)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (78,0,'372','акции и иные формы участия в капитале (021530000)',78)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (79,0,'373','иные финансовые активы (021550000)',79)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (80,0,'400','Итого по разделу II (стр. 170 + стр. 210 +  стр. 230 + стр. 260 + стр. 290 + стр. 310 + стр. 320 + стр. 330 + стр. 370)',80)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (81,0,'410','БАЛАНС (стр. 150 + стр. 400)',81)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (82,0,'470','Расчеты с кредиторами по долговым обязательствам (030100000) в том числе: ',82)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (83,0,'471','по долговым обязательствам в рублях (030110000)',83)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (84,0,'472','по долговым обязательствам по целевым иностранным кредитам (заимствованиям) (030120000)',84)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (85,0,'473','по государственным (муниципальным) гарантиям (030130000)',85)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (86,0,'474','по долговым обязательствам в иностранной валюте (030140000)',86)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (87,0,'490','Расчеты по принятым обязательствам (030200000)',87)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (88,0,'510','Расчеты по платежам в бюджеты (030300000) из них:',88)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (89,0,'511','расчеты по налогу на доходы физических лиц (030301000)',89)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (90,0,'512','расчеты по страховым взносам на обязательное социальное страхование (030302000, 030306000)',90)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (91,0,'513','расчеты по налогу на прибыль организаций (030303000)',91)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (92,0,'514','расчеты по налогу на добавленную стоимость (030304000)',92)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (93,0,'515','расчеты по иным платежам в бюджет (030305000, 030312000, 030313000)',93)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (94,0,'516','расчеты по страховым взносам на медицинское и пенсионное страхование (030307000, 030308000, 030309000, 030310000, 030311000)',94)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (95,0,'530','Прочие расчеты с кредиторами (030400000) из них:',95)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (96,0,'531','расчеты по средствам, полученным во временное распоряжение (030401000)',96)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (97,0,'532','расчеты с депонентами (030402000)',97)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (98,0,'533','расчеты по удержаниям из выплат по оплате труда (030403000)',98)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (99,0,'534','внутриведомственные расчеты (030404000)',99)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (100,0,'536','расчеты с прочими кредиторами (030406000)',100)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (101,0,'600','Итого по разделу III (стр. 470 + стр. 490 + стр. 510 + стр. 530)',101)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (102,0,'620','Финансовый результат экономического субъекта (040100000) из них:',102)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (103,0,'623','финансовый результат прошлых отчетных периодов (040130000)',103)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (104,0,'623.1','финансовый результат по начисленной амортизации ОЦИ',104)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (105,0,'624','доходы будущих периодов (040140000)',105)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (106,0,'625','расходы будущих периодов (040150000)',106)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (107,0,'900','БАЛАНС (стр. 600 + стр. 620)',107)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (108,0,'010','Имущество, полученное в пользование, всего',108)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (109,0,'011','недвижимое',109)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (110,0,'012','непроизведенное',110)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (111,0,'015','движимое',111)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (112,0,'020','Материальные ценности, принятые на хранение, всего в том числе:',112)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (113,0,'030','Бланки строгой отчетности, всего в том числе:',113)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (114,0,'040','Задолженность неплатежеспособных дебиторов, всего в том числе:',114)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (115,0,'050','Материальные ценности, оплаченные по централизованному снабжению, всего',115)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (116,0,'051','в том числе: основные средства',116)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (117,0,'052','особо ценное движимое имущество',117)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (118,0,'054','в том числе: материальные запасы',118)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (119,0,'055','особо ценное движимое имущество',119)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (120,0,'060','Задолженность учащихся и студентов за невозвращенные материальные ценности',120)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (121,0,'070','Награды, призы, кубки и ценные подарки, сувениры, всего в том числе:',121)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (122,0,'071','в условной оценке',122)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (123,0,'072','по стоимости приобретения',123)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (124,0,'080','Путевки неоплаченные',124)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (125,0,'090','Запасные части к транспортным средствам, выданные взамен изношенных',125)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (126,0,'100','Обеспечение исполнения обязательств, всего в том числе:',126)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (127,0,'101','задаток',127)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (128,0,'102','залог',128)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (129,0,'103','банковская гарантия',129)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (130,0,'104','поручительство',130)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (131,0,'105','иное обеспечение',131)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (132,0,'110','Государственные и муниципальные гарантии, всего в том числе:',132)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (133,0,'111','государственные гарантии',133)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (134,0,'112','муниципальные гарантии',134)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (135,0,'120','Спецоборудование для выполнения научно-исследовательских работ по договорам с заказчиками, всего в том числе:',135)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (136,0,'130','Экспериментальные устройства',136)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (137,0,'140','Расчетные документы, ожидающие исполнения',137)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (138,0,'150','Расчетные документы, не оплаченные в срок из-за отсутствия средств на счете государственного (муниципального) учреждения',138)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (139,0,'160','Переплата пенсий и пособий вследствие неправильного применения законодательства о пенсиях и пособиях, счетных ошибок',139)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (140,0,'170','Поступления денежных средств на счета учреждения, всего в том числе:',140)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (141,0,'171','доходы',141)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (142,0,'172','расходы',142)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (143,0,'173','источники финансирования дефицита бюджета',143)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (144,0,'180','Выбытия денежных средств со счетов учреждения, всего в том числе:',144)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (145,0,'181','расходы',145)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (146,0,'182','источники финансирования дефицита бюджета',146)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (147,0,'190','Невыясненные поступления бюджета прошлых лет в том числе:',147)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (148,0,'200','Задолженность, не востребованная кредиторами, всего в том числе:',148)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (149,0,'210','Основные средства стоимостью до 3000 рублей включительно в эксплуатации, всего',149)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (150,0,'211','особо ценное движимое имущество',150)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (151,0,'212','иное движимое имущество',151)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (152,0,'220','Материальные ценности, полученные по централизованному снабжению, всего в том числе:',152)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (153,0,'221','основные средства',153)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (154,0,'222','особо ценное движимое имущество',154)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (155,0,'224','материальные запасы',155)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (156,0,'225','особо ценное движимое имущество',156)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (157,0,'230','Периодические издания для пользования, всего в том числе:',157)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (158,0,'240','Имущество, переданное в доверительное управление, всегов  том числе:',158)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (159,0,'241','основные средства из них:',159)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (160,0,'242','недвижимое имущество',160)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (161,0,'243','особо ценное движимое имущество',161)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (162,0,'244','нематериальные активы',162)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (163,0,'245','особо ценное движимое имущество',163)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (164,0,'246','материальные запасы',164)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (165,0,'247','особо ценное движимое имущество',165)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (166,0,'250','Имущество, переданное в возмездное пользование (аренду) в том числе:',166)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (167,0,'251','основные средства из них:',167)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (168,0,'252','недвижимое имущество',168)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (169,0,'253','особо ценное движимое имущество',169)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (170,0,'254','нематериальные активы',170)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (171,0,'255','особо ценное движимое имущество',171)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (172,0,'256','материальные запасы',172)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (173,0,'257','особо ценное движимое имущество',173)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (174,0,'260','Имущество, переданное в безвозмездное пользование в том числе:',174)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (175,0,'261','основные средства из них:',175)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (176,0,'262','недвижимое имущество',176)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (177,0,'263','особо ценное движимое имущество',177)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (178,0,'264','нематериальные активы',178)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (179,0,'265','особо ценное движимое имущество',179)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (180,0,'266','материальные запасы',180)
INSERT INTO [DV].[d_Line_Indicators]([ID],[RowType],[LineCode],[Name],[Code]) VALUES (181,0,'267','особо ценное движимое имущество',181)
GO

print N'Поднимаем генератор'
GO

SET IDENTITY_INSERT g.d_Line_Indicators ON
GO

INSERT INTO g.d_Line_Indicators (ID) 
VALUES(181)
GO

Delete from g.d_Line_Indicators
GO

SET IDENTITY_INSERT g.d_Line_Indicators OFF
GO

print N'Включение триггера'
GO

enable trigger t_d_Line_Indicators_aa on [DV].[d_Line_Indicators];
GO

