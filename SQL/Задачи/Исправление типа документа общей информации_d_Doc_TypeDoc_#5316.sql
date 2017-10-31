/* Start - #5316 - Исправление типа документа общей информации - shelpanov - 15.01.2013 */

print N'Отключение триггера'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'Добавляем....'
GO

update [DV].[d_Doc_TypeDoc]
set Name = 'Отчет о результатах деятельности и об использовании имущества'
where ID = 11;

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #5316 - Исправление типа документа общей информации - shelpanov - 15.01.2013 */