using System;
using System.Collections.Generic;
using System.Text;

namespace Akkatecture.Core.VersionedTypes
{
    public interface IVersionedTypeDefinitionService<TAttribute, TDefinition>
        where TAttribute : VersionedTypeAttribute
        where TDefinition : VersionedTypeDefinition
    {
        void Load(IReadOnlyCollection<Type> types);
        IEnumerable<TDefinition> GetDefinitions(string name);
        bool TryGetDefinition(string name, int version, out TDefinition definition);
        IEnumerable<TDefinition> GetAllDefinitions();
        TDefinition GetDefinition(string name, int version);
        TDefinition GetDefinition(Type type);
        IReadOnlyCollection<TDefinition> GetDefinitions(Type type);
        bool TryGetDefinition(Type type, out TDefinition definition);
        bool TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions);
        void Load(params Type[] types);
    }
}
