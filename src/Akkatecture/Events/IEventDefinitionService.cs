using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Events
{
    public interface IEventDefinitionService : IVersionedTypeDefinitionService<EventVersionAttribute, EventDefinition>
    {
    }
}
