print N'Отключение триггера'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
disable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];

print N'4->1'
GO

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (1, 0, 'F', 'Учредительные документы');

update [DV].[f_Doc_Docum]
set RefTypeDoc = 1
where RefTypeDoc = 4;

print N'5->4'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 5),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 5)
where ID = 4;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 4
where RefTypeDoc = 5;

print N'2->5'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 2),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 2)
where ID = 5;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 5
where RefTypeDoc = 2;

print N'3->2'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 3),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 3)
where ID = 2;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 2
where RefTypeDoc =3;

print N'6->3'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 6),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 6)
where ID = 3;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 3
where RefTypeDoc =6;

print N'7->6'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 7),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 7)
where ID = 6;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 6
where RefTypeDoc = 7;

print N'12->7'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 12),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 12)
where ID = 7;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 7
where RefTypeDoc = 12;

delete from [DV].[d_Doc_TypeDoc]
where ID = 12;

print N'10->13'
GO

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (13, 0, 'C', 'Государственное задание');


update [DV].[f_Doc_Docum]
set RefTypeDoc = 13
where RefTypeDoc = 10;

print N'8->10'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 8),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 8)
where ID = 10;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 10
where RefTypeDoc = 8;

print N'11->8'
GO

update [DV].[d_Doc_TypeDoc]
set 
  Name = (select Name from [DV].[d_Doc_TypeDoc] where ID = 11),
  Code = (select Code from [DV].[d_Doc_TypeDoc] where ID = 11)
where ID = 8;

update [DV].[f_Doc_Docum]
set RefTypeDoc = 8
where RefTypeDoc = 11;

delete from [DV].[d_Doc_TypeDoc]
where ID = 11;

print N'Включение триггера'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
enable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];
 
GO