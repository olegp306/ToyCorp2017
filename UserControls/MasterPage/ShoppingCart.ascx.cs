//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;

namespace UserControls.MasterPage
{
    public partial class ShoppingCartControl : UserControl
    {
        protected string Count = "";
        protected string TypeSite = "default";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            float itemsCount = ShoppingCartService.CurrentShoppingCart.TotalItems;
            Count = string.Format("{0} {1}", itemsCount == 0 ? "" : itemsCount.ToString(),
                                  Strings.Numerals(itemsCount, Resources.Resource.Client_UserControls_ShoppingCart_Empty,
                                                   Resources.Resource.Client_UserControls_ShoppingCart_1Product,
                                                   Resources.Resource.Client_UserControls_ShoppingCart_2Products,
                                                   Resources.Resource.Client_UserControls_ShoppingCart_5Products));

            var moduleShoppingCartPopup = AttachedModules.GetModules<IShoppingCartPopup>().FirstOrDefault();
            if (moduleShoppingCartPopup != null)
            {
                TypeSite = "productadded";
            }
        }
    }
}