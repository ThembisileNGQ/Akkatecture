using Akka.Actor;

namespace Akkatecture.Jobs
{
    public class JobSchedulerFactory<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
    }
}