
-- SQL_4.1.13_Part1

-- Property FIX
UPDATE Catalog.Property set [UseInBrief]=0 where [UseInBrief] is null
GO--
ALTER TABLE [Catalog].[Property] ALTER COLUMN [UseInBrief] bit not null
-- Property FIX

GO--

UPDATE [Settings].[Settings] SET [Value] = 'RUB' where [Name] = 'GoogleBaseCurrency'
GO--

ALTER PROCEDURE [Catalog].[sp_ParseProductProperty]
		@nameProperty nvarchar(100),
		@propertyValue nvarchar(255),
		@productId int,
		@sort int
AS
BEGIN
	-- select or create property
	Declare @propertyId int
	if ((select count(PropertyID) from Catalog.[Property] where Name = @nameProperty)= 0)
		begin
			insert into Catalog.[Property] (Name,UseInFilter,Useindetails,SortOrder,Expanded,[Type],UseInBrief) values (@nameProperty,1,1,0,0,1,0)
			set @propertyId = (Select SCOPE_IDENTITY())
		end
	else
		set @propertyId = (select top(1) PropertyID from Catalog.[Property] where Name = @nameProperty)

		-- select or create value
	 Declare @propertyValueId int

	 Declare @useinfilter bit
	 set @useinfilter = (Select Top 1 UseInFilter from Catalog.[Property] Where PropertyID=@propertyId)
	 Declare @useindetails bit
	 set @useindetails = (Select Top 1 UseInDetails from Catalog.[Property] Where PropertyID=@propertyId)

	 if ((select count(PropertyValueID) from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)= 0)
	  begin
	   insert into Catalog.[PropertyValue] 
		  (PropertyId, Value, UseInFilter, UseInDetails, SortOrder) 
		values (@propertyId, @propertyValue, @useinfilter, @useindetails, 0)
	   set @propertyValueId = (Select SCOPE_IDENTITY())
	  end
	 else
	  set @propertyValueId = (select top(1) PropertyValueID from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)
	
	--create link between product and property value
	if ((select Count(*) from Catalog.ProductPropertyValue where ProductID=@productId and PropertyValueID=@propertyValueId)=0)
		insert into Catalog.ProductPropertyValue (ProductID,PropertyValueID,SortOrder) values (@productId,@propertyValueId,@sort)	
END
GO--

ALTER PROCEDURE [Catalog].[sp_GetChildCategoriesByParentID]
	@ParentCategoryID int,
	@HasProducts bit,
	@bigType  nvarchar(50),
	@smallType  nvarchar(50)
AS
BEGIN

if @hasProducts = 0
	SELECT 
		*,
		(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count],
		(SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,
		(SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture
	FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0 
	ORDER BY SortOrder, name
else
	SELECT 
		*,
		(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count] ,
		(SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,
		(SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture
	FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0 and Products_Count > 0
	ORDER BY SortOrder, name
END
GO--


GO--
if not exists(select * from sys.columns 
            where Name = N'Patronymic' and Object_ID = Object_ID(N'[Order].[OrderCustomer]'))
begin
    Alter table [Order].[OrderCustomer] add Patronymic nvarchar(1000)
end

GO--

if not exists(select * from sys.columns 
            where Name = N'Options' and Object_ID = Object_ID(N'[Order].[OrderByRequest]'))
begin
    Alter table [Order].[OrderByRequest] add Options nvarchar(Max)
end

GO--

if (select Count([Key]) from [CMS].[StaticBlock] where [Key]='CatalogBottom') = 0
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
	Values ('CatalogBottom', 'Нижняя часть каталога', '', GETDATE(), GETDATE(), 1)
GO--

update [Settings].[MailFormatType] set [Comment]= Replace([Comment], '#NUMBER# )', '#NUMBER#, #ORDERTABLE# )') where [MailFormatTypeID]=4

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.13' WHERE [settingKey] = 'db_version'

GO--

