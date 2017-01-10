//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;

namespace AdvantShop.Modules.Interfaces
{
    /// <summary>
    /// IShoppingCartPopup - interface of module - popup on adding product to cart 
    /// </summary>
    public interface IShoppingCartPopup : IModule
    {
        /// <summary>
        /// Get cart popup json string
        /// </summary>
        /// <param name="offer">Offer of product that was added to cart</param>
        /// <param name="amount">Amount of added product</param>
        /// <param name="isSocialTemplate">Is social tempalte (obsolete)</param>
        /// <returns>Json string</returns>
        string GetShoppingCartPopupJson(Offer offer, float amount, bool isSocialTemplate);
    }
}
