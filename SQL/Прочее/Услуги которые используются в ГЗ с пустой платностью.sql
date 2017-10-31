SELECT [ID]
  FROM [DV].[d_Services_VedPer] u
  where RefPl is NULL and EXISTS (select ID
									from [DV].F_F_GosZadanie
									where RefVedPch =u.ID
									)
GO


