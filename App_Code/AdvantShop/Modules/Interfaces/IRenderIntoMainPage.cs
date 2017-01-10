
namespace AdvantShop.Modules.Interfaces
{
    public interface IRenderIntoMainPage : IModule
    {
        /// <summary>
        /// Get user control path for main page products (after carousel, before bestsellers)
        /// </summary>
        /// <returns></returns>
        string GetMainPageProductsUcPath();


        /// <summary>
        /// Render string on main page (after carousel, before bestsellers)
        /// </summary>
        /// <returns></returns>
        string RenderMainPageProducts();


        string RenderMainPageAfterCarousel();
    }
}