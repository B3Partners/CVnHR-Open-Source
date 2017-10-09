namespace QNH.Overheid.KernRegister.Business.Model.EntityComparers
{
    public interface IFunctionalEquals<in T>
    {
        bool FunctionalEquals(T other);
    }
}