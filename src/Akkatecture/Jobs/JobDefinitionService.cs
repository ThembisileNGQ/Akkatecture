using System;
using Akka.Event;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Jobs
{
    public class JobDefinitionService : VersionedTypeDefinitionService<IJob, JobVersionAttribute, JobDefinition>, IJobDefinitionService
    {
        public JobDefinitionService(ILoggingAdapter logger)
            : base(logger)
        {
        }

        protected override JobDefinition CreateDefinition(int version, Type type, string name)
        {
            return new JobDefinition(version, type, name);
        }
    }
}