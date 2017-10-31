/* Start - #4318 - Добавленеие нового типа документа общей информации для нового интерфейса - shelpanov - 14.12.2012 */

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
           (11, 0, 'D', 'Информация о результатах деятельности и об использовании имущества')

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4318 - Добавленеие нового типа документа общей информации для нового интерфейса - shelpanov - 14.12.2012 */