USE [toycorp]
GO
/****** Object:  StoredProcedure [Catalog].[sp_AddProduct]    Script Date: 10.01.2017 13:38:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Catalog].[sp_AddProduct]		
			@ArtNo nvarchar(50) = '',
			@Name nvarchar(255),			
			@Ratio float,
			@Discount float,
			@Weight float,
			@Size nvarchar(50),
			@BriefDescription nvarchar(max),
			@Description nvarchar(max),
			@Enabled tinyint,
			@Recomended bit,
			@New bit,
			@BestSeller bit,
			@OnSale bit,
			@BrandID int,
			@AllowPreOrder bit,
			@UrlPath nvarchar(150),
			@Unit nvarchar(50),
			@ShippingPrice float,
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
Declare @Id int
INSERT INTO [Catalog].[Product]
           ([ArtNo]
           ,[Name]                   
           ,[Ratio]
           ,[Discount]
           ,[Weight]
           ,[Size]
           ,[BriefDescription]
           ,[Description]
           ,[Enabled]
           ,[DateAdded]
           ,[DateModified]
           ,[Recomended]
           ,[New]
           ,[BestSeller]
           ,[OnSale]
           ,[BrandID]
           ,[AllowPreOrder]
		   ,[UrlPath]
		   ,[Unit]
		   ,[ShippingPrice]
		   ,[MinAmount]
		   ,[MaxAmount]
		   ,[Multiplicity]
		   ,[HasMultiOffer]
		   ,[SalesNote]
		   ,GoogleProductCategory
		   ,Gtin
		   ,Adult
		   ,[ManufacturerWarranty]
		   ,[AddManually]
		   ,[PopularityManually]
          )
     VALUES
           (@ArtNo,
			@Name,					
			@Ratio,
			@Discount,
			@Weight,
			@Size,
			@BriefDescription,
			@Description,
			@Enabled,
			GETDATE(),
			GETDATE(),
			@Recomended,
			@New,
			@BestSeller,
			@OnSale,
			@BrandID,
			@AllowPreOrder,
			@UrlPath,
			@Unit,
			@ShippingPrice,
			@MinAmount,
			@MaxAmount,
			@Multiplicity,
			@HasMultiOffer,
			@SalesNote,
			@GoogleProductCategory,
			@Gtin,
			@Adult,
			@ManufacturerWarranty,
			@AddManually,
			@PopularityManually
			);

	SET @ID = SCOPE_IDENTITY();
	if @ArtNo=''
	begin
		set @ArtNo = Convert(nvarchar(50), @ID)	

		WHILE (SELECT COUNT(*) FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo) > 0
		begin
				SET @ArtNo = @ArtNo + '_A'
		end

		UPDATE [Catalog].[Product] SET [ArtNo] = @ArtNo WHERE [ProductID] = @ID	
	end
	Select @ID
END

