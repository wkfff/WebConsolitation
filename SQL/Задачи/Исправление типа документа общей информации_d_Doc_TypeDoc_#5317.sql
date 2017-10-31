/* Start - #5317 - Исправление типа документа общей информации - shelpanov - 25.01.2013 */

print N'Отключение триггера'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'Добавляем....'
GO

update [DV].[d_Doc_TypeDoc]
set Name = 'Баланс главного распорядителя, распорядителя, получателя бюджетных средств (ф. 0503130)'
where ID = 19;

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #5317 - Исправление типа документа общей информации - shelpanov - 25.01.2013 */