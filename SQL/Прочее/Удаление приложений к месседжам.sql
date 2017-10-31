
delete from [DV].MessageAttachment
where ID = ANY (
				select ID FROM [DV].MessageAttachment ma
				where not EXISTS (select *
				FROM [DV].Message m
				where m.RefMessageAttachment = ma.ID))


