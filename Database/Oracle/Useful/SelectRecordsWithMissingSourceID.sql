/* Запрос возвращает записи из таблицы фактов для которых нет источника в таблице DataSources */
select * from f_D_FO11Prj where SourceID in
(
  select MissingSourceID from
  (
    select distinct T.SourceID as MissingSourceID, S.ID as MissingDataSourceID from f_D_FO11Prj T left outer join DataSources S on (T.SourceID = S.ID)
  ) where MissingDataSourceID is null
)