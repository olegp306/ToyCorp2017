ALTER TABLE [Catalog].[Product]
ADD [RecomendedManual] int DEFAULT 0
 
Update [Catalog].[Product]
set [RecomendedManual]=0 from [Catalog].[Product]
