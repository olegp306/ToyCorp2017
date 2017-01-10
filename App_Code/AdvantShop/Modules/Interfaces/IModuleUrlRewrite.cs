namespace AdvantShop.Modules.Interfaces
{
    public interface IModuleUrlRewrite : IModule
    {
        bool RewritePath(string rawUrl, ref string newUrl);
    }
}