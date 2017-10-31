SELECT ID, Code
  FROM [RIATest].[DV].[d_OKATO_OKATO] uch
  where (select COUNT(ID) FROM [RIATest].[DV].[d_OKATO_OKATO] u where u.Code = uch.Code ) > 1
GO