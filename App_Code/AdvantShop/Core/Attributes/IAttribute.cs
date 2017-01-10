namespace AdvantShop.Core.Attributes
{
    public interface IAttribute<T>
    {
        T Value { get; }
    }
}