using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Jobs
{
    public interface IJobDefinitionService : IVersionedTypeDefinitionService<JobVersionAttribute, JobDefinition>
    {
    }
}