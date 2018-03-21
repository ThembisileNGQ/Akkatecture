using System;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Events
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = true
    )]
    public class EventVersionAttribute : VersionedTypeAttribute
    {
        public EventVersionAttribute(string name, int version)
            : base(name, version)
        {
        }
    }
}