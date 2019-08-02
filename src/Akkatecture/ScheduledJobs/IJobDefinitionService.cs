using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.ScheduledJobs
{
    public interface IJobDefinitionService : IVersionedTypeDefinitionService<JobVersionAttribute, JobDefinition>
    {
    }
}