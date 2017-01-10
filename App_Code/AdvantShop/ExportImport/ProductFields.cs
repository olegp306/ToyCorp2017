//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ProductFieldStatus
    {
        None,
        String,
        StringRequired,
        NotEmptyString,
        Float,
        Int
    }

    public enum ProductFields
    {
        [StringName("none")]
        [ResourceKey("ProductFields_NotSelected")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.None)]
        None,

        [StringName("sku")]
        [ResourceKey("ProductFields_Sku")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Sku,

        [StringName("name")]
        [ResourceKey("ProductFields_Name")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.StringRequired)]
        Name,

        [StringName("paramsynonym")]
        [ResourceKey("ProductFields_Synonym")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        ParamSynonym,

        [StringName("category")]
        [ResourceKey("ProductFields_Categories")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Category,

        [StringName("sorting")]
        [ResourceKey("ProductFields_Sorting")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Sorting,

        [StringName("enabled")]
        [ResourceKey("ProductFields_Enabled")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Enabled,

        [StringName("price")]
        [ResourceKey("ProductFields_Price")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        Price,

        [StringName("purchaseprice")]
        [ResourceKey("ProductFields_PurchasePrice")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        PurchasePrice,

        [StringName("amount")]
        [ResourceKey("ProductFields_Amount")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        Amount,

        [StringName("sku:size:color:price:purchaseprice:amount")]
        [ResourceKey("ProductFields_MultiOffer")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.NotEmptyString)]
        MultiOffer,

        [StringName("unit")]
        [ResourceKey("ProductFields_Unit")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Unit,

        [StringName("discount")]
        [ResourceKey("ProductFields_Discount")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        Discount,

        [StringName("shippingprice")]
        [ResourceKey("ProductFields_ShippingPrice")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        ShippingPrice,

        [StringName("weight")]
        [ResourceKey("ProductFields_Weight")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.Float)]
        Weight,

        [StringName("size")]
        [ResourceKey("ProductFields_Size")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Size,

        [StringName("briefdescription")]
        [ResourceKey("ProductFields_BriefDescription")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        BriefDescription,

        [StringName("description")]
        [ResourceKey("ProductFields_Description")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Description,

        [StringName("title")]
        [ResourceKey("ProductFields_SeoTitle")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Title,

        [StringName("metakeywords")]
        [ResourceKey("ProductFields_MetaKeywords")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        MetaKeywords,

        [StringName("metadescription")]
        [ResourceKey("ProductFields_MetaDescription")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        MetaDescription,

        [StringName("h1")]
        [ResourceKey("ProductFields_H1")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        H1,


        [StringName("photos")]
        [ResourceKey("ProductFields_Photos")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Photos,

        [StringName("videos")]
        [ResourceKey("ProductFields_Videos")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Videos,

        [StringName("markers")]
        [ResourceKey("ProductFields_Markers")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Markers,

        [StringName("properties")]
        [ResourceKey("ProductFields_Properties")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Properties,

        [StringName("producer")]
        [ResourceKey("ProductFields_Producer")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Producer,

        [StringName("preorder")]
        [ResourceKey("ProductFields_PreOrder")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        OrderByRequest,

        [StringName("salesnote")]
        [ResourceKey("ProductFields_SalesNotes")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        SalesNotes,

        [StringName("related sku")]
        [ResourceKey("ProductFields_RelatedSKU")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Related,

        [StringName("alternative sku")]
        [ResourceKey("ProductFields_AlternativeSKU")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Alternative,

        [StringName("custom options")]
        [ResourceKey("ProductFields_CustomOptions")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        CustomOption,

        [StringName("gtin")]
        [ResourceKey("ProductFields_Gtin")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Gtin,

        [StringName("googleproductcategory")]
        [ResourceKey("ProductFields_GoogleProductCategory")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        GoogleProductCategory,

        [StringName("adult")]
        [ResourceKey("ProductFields_Adult")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        Adult,

        [StringName("manufacturer_warranty")]
        [ResourceKey("ProductFields_ManufacturerWarranty")]
        [ProductFieldsStatusAttribute(ProductFieldStatus.String)]
        ManufacturerWarranty
    }
}