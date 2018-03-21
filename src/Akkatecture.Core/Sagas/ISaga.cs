namespace Akkatecture.Sagas
{
    public interface ISaga
    {
        
    }

    public interface ISaga<TLocator> : ISaga
        where TLocator : ISagaLocator
    {
    }
}