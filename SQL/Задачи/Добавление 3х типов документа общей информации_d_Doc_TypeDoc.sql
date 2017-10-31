/* Start - #4319 - Добавленеие 3х типов документа общей информации для нового интерфейса - shelpanov - 17.12.2012 */

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
           (12, 0, 'G', 'Правовой акт органа, осуществляющего проведение контрольного мероприятия')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (14, 0, 'H', 'Акт о результатах контрольного мероприятия')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (15, 0, 'J', 'Акт о повторно проведенном контрольном мероприятии и отчет о результатах')

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4319 - Добавленеие 3х типов документа общей информации для нового интерфейса - shelpanov - 17.12.2012 */