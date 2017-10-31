/* Start - #5842 - Меняем статусы для документов - shelpanov - 18.04.2013 */

print N'Отключение триггера'
GO

disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print('Меняем состояния')
GO

update [DV].[f_F_ParameterDoc]
set  CloseDate = GETDATE(),
	 RefSost = 8
where (RefPartDoc = 2) and (RefSost = 7) and
      (RefYearForm = 2012) and (RefUchr IN (3524, 3858, 3661, 3697, 2908, 2873, 2760, 2781, 2908, 2873, 3102))

update [DV].[f_F_ParameterDoc]
set  CloseDate = GETDATE(),
	 RefSost = 8
where (RefPartDoc = 2) and (RefSost = 7) and
      (RefYearForm = 2013) and (RefUchr IN (2877, 5448, 2654, 2818, 2760, 2781, 2676, 2811, 3643, 3458, 3670, 3575, 3647, 3580, 3657, 4670, 3692, 4669, 3696, 4106, 3674, 3684, 3679, 3854, 3864, 3961, 
											3940, 3904, 3899, 3901, 3891, 4073, 3916, 3894, 3110, 4057, 3920, 3912, 3898, 3907, 4046, 3941, 3873, 4101, 4673, 4067, 4442, 3650, 3473, 3408, 3334, 3308,
											3880, 4424, 4066, 4091, 4371, 4682, 3460, 4663, 3910, 3939))


print N'Включение триггера'
GO

enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

/* End - #5842 - Меняем статусы для документов - shelpanov - 18.04.2013 */