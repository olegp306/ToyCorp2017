USE [toycorp]
GO
/****** Object:  StoredProcedure [Catalog].[sp_UpdateProductById]    Script Date: 10.01.2017 13:44:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
		@ProductID int,	   
		@ArtNo nvarchar(50),
		@Name nvarchar(255),
		@Ratio float,		
		@Discount float,
		@Weight float,
		@Size nvarchar(50),
		@BriefDescription nvarchar(max),
		@Description nvarchar(max),
		@Enabled bit,
		@Recomended bit,
		@New bit,
		@BestSeller bit,
		@OnSale bit,
		@BrandID int,
		@AllowPreOrder bit,
		@UrlPath nvarchar(150),
		@Unit nvarchar(50),
		@ShippingPrice money,
		@MinAmount float,
		@MaxAmount float,
		@Multiplicity float,
		@HasMultiOffer bit,
		@SalesNote nvarchar(50),
		@GoogleProductCategory nvarchar(500),
		@Gtin nvarchar(50),
		@Adult bit,
		@ManufacturerWarranty bit,
		@AddManually bit,
		@PopularityManually int
AS
BEGIN
UPDATE [Catalog].[Product]
   SET [ArtNo] = @ArtNo
		,[Name] = @Name
		,[Ratio] = @Ratio
		,[Discount] = @Discount
		,[Weight] = @Weight
		,[Size] = @Size
		,[BriefDescription] = @BriefDescription
		,[Description] = @Description
		,[Enabled] = @Enabled
		,[Recomended] = @Recomended
		,[New] = @New
		,[BestSeller] = @BestSeller
		,[OnSale] = @OnSale
		,[DateModified] = GETDATE()
		,[BrandID] = @BrandID
		,[AllowPreOrder] = @AllowPreOrder
		,[UrlPath] = @UrlPath
		,[Unit] = @Unit
		,[ShippingPrice] = @ShippingPrice
		,[MinAmount] = @MinAmount
		,[MaxAmount] = @MaxAmount
		,[Multiplicity] = @Multiplicity
		,[HasMultiOffer] = @HasMultiOffer
		,[SalesNote] = @SalesNote
		,[GoogleProductCategory]=@GoogleProductCategory
		,[Gtin]=@Gtin
		,[Adult] = @Adult
		,[ManufacturerWarranty] = @ManufacturerWarranty
		,[PopularityManually]=@PopularityManually
 WHERE ProductID = @ProductID	 
END

