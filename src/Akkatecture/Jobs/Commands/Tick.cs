namespace Akkatecture.Jobs.Commands
{
    internal sealed class Tick <TJob,TIdentity> : SchedulerMessage<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public static Tick<TJob,TIdentity> Instance { get; } = new Tick<TJob,TIdentity>();
        private Tick() { }
    }
}