using System;

namespace AdvantShop.Customers
{
    public enum Role
    { 
        User = 0,
        Moderator = 50,
        Administrator = 100,
        Guest = 150
    }

    public enum RoleActionKey
    {
        DisplayCatalog,
        DisplayCatalogDictionaries,
        DisplayImportExport,
        DisplayMainPageBestsellers,
        DisplayMainPageNew,
        DisplayMainPageDiscount,
        DisplayComments,
        DisplayPriceRegulation,
        DisplayBrands,
        
        DisplayOrders,
        DisplayOrderStatuses,
        DisplayCustomers,
        DisplayCertificates,
        DisplaySubscription,

        DisplayStaticPages,
        DisplayStaticBlocks,
        DisplayMenus,
        DisplayNews,
        DisplayCarousel,

        AllowEditDiscounts,
        AllowEditCoupones,
        DislayVotes,
        DisplaySiteMap,
        DisplayExportFeed,
        DisplaySendMessages,

        DisplayCommonSettings,
        DisplayCountries,
        DisplayCurrencies,
        DisplayPayments,
        DisplayShippings,
        DisplayTaxes,
        DisplayMailFormats,
        DisplayLog,
        DisplayRedirect,

        DisplayDesignTransformer,
        DisplayDesignSettings,

        DisplayModules,

        DisplayAdminMainPageStatistics,

        DisplaySendPaymentLink
    }

    public class RoleAction
    {
        public RoleAction()
        {
            CustomerID = Guid.Empty;
            RoleActionID = 0;
            Name = "";
            Enabled = false;
            Category = "";
            SortOrder = 0;
        }

        public Guid CustomerID { get; set; }
        public int RoleActionID { get; set; }
        public string Name { get; set; }
        public RoleActionKey Key { get; set; }
        public bool Enabled { get; set; }
        public string Category { get; set; }
        public int SortOrder { get; set; }
    }
}