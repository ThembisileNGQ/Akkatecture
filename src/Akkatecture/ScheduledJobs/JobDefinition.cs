using System;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.ScheduledJobs
{
    public class JobDefinition : VersionedTypeDefinition
    {
        public JobDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}