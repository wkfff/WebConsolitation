/* Start - #4352 - Удаляем тип документа(fx.FX.PartDoc) ID = 4 - shelpanov - 03.10.2012 */

print N'Отключение триггера'
GO

disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print N'Удаление документов с типом 4'
GO

delete from [DV].[f_F_ParameterDoc]
where RefPartDoc = 4;

 
print N'Включение триггера'
GO

enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];
 
GO

