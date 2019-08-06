namespace Akkatecture.Jobs
{
    public interface IJobManager
    {
        
    }

    public interface IJobManager<TJob, TIdentity> : IJobManager
        where TJob : IJob
        where TIdentity : IJobId
    {
        
    }


}