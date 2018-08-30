using System;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Events
{
    public class EventDefinition : VersionedTypeDefinition
    {
        public EventDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}
