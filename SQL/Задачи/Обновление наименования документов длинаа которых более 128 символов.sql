/* Start - #4621 - ���������� ������������ ���������� ������ ������� ��������� 128 �������� - shelpanov - 08.11.2012 */

print N'���������� ��������'
GO

disable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];

print N'���������� ������������'
GO

UPDATE [DV].[f_Doc_Docum]
   SET [Name] = (select Name
                from [DV].[d_Doc_TypeDoc]
                where ID = RefTypeDoc)
 WHERE (LEN([Name])>128)
GO

print N'��������� ��������'
GO

enable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];

/* End - #4621 - ���������� ������������ ���������� ������ ������� ��������� 128 �������� - shelpanov - 08.11.2012 */

