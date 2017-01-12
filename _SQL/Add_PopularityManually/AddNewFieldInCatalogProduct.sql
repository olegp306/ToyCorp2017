ALTER TABLE [Catalog].[Product]
ADD [PopularityManually] int DEFAULT 0
 
Update [Catalog].[Product]
set [PopularityManually]=0 from [Catalog].[Product]
