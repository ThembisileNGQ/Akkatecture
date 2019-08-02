namespace Akkatecture.Jobs.Commands
{
    public class Cancel<TJob, TIdentity> : SchedulerCommand<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public Cancel(TIdentity id)
            : base(id)
        {
            
        }
    }
}