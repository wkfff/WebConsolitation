select *  FROM [RIAYar].[DV].[d_OKATO_OKATO] o
where ID > 8333 and (EXISTS (select *
                             FROM [RIAYar].[DV].[f_Org_Passport] pp
                             where pp.RefOKATO = o.ID))