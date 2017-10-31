/* Start - #4570 - Добавленеие типов документа общей информации для нового интерфейса - shelpanov - 18.12.2012 */

print N'Отключение триггера'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'Добавляем....'
GO

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (16, 0, 'K', 'Баланс государственного (муниципального) учреждения (ф. 0503730)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (17, 0, 'M', 'Отчет о финансовых результатах деятельности (ф. 0503721)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (18, 0, 'N', 'Отчет об исполнении учреждением плана его финансово-хозяйственной деятельности (ф. 0503737)')

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (19, 0, 'Q', 'Баланс главного распорядителя, распорядителя, получателя бюджетных средств, главного администратора, администратора источников финансирования дефицита бюджета, главного администратора, администратора доходов бюджета (ф. 0503130)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (20, 0, 'R', 'Отчет о финансовых результатах деятельности (ф. 0503121)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (21, 0, 'T', 'Отчет об исполнении бюджета (ф. 0503127)')

INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (22, 0, 'U', 'Отчет об исполнении смет доходов и расходов по приносящей доход деятельности (ф. 0503137)')

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4570 - Добавленеие типов документа общей информации для нового интерфейса - shelpanov - 18.12.2012 */