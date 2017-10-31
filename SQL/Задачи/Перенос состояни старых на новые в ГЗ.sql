/* Start - #3825 - Переносит старые состояния для ГЗ в новые - shelpanov - 12.10.2012 */

print N'Отключение триггера'
GO

disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print('Состояние: Создан')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 1)
WHERE [RefSost] = 2;
 
print('Состояние: На рассмотрении')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 2)
WHERE [RefSost] = 4;

print('Состояние: На доработке')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 3)
WHERE [RefSost] = 5;

print('Состояние: Завершен')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 4)
WHERE [RefSost] = 7;

print('Состояние: Завершен')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 5)
WHERE [RefSost] = 8;

print N'Включение триггера'
GO

enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

/* End - #3825 - Переносит старые состояния для ГЗ в новые - shelpanov - 12.10.2012 */