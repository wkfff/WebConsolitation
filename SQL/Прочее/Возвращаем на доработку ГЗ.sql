UPDATE [RIAYar].[DV].[f_F_ParameterDoc]
   SET [RefSost] = 5
     
 WHERE ID = ANY (select p.ID
       from [RIAYar].[DV].[f_F_ParameterDoc] p join 
            [RIAYar].[DV].[d_Org_Structure] s on (p.RefUchr = s.ID) join
            [RIAYar].[DV].[d_Org_PPO] ppo on (s.RefOrgPPO = ppo.ID) join
            [RIAYar].[DV].[d_Org_GRBS] grbs on (s.RefOrgGRBS = grbs.ID)
       where (p.RefSost = 7) and (p.RefPartDoc = 2) and (ppo.Code = '78401000000')
                        and (grbs.Code = '803')) 
GO


