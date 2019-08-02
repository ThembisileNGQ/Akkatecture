using System;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobVersionAttribute : VersionedTypeAttribute
    {
        public JobVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}