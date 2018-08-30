using System;
using System.Collections.Generic;
using System.Reflection;
using Akkatecture.Extensions;
using Akkatecture.ValueObjects;

namespace Akkatecture.Core.VersionedTypes
{
    public abstract class VersionedTypeDefinition : ValueObject
    {
        public int Version { get; }
        public Type Type { get; }
        public string Name { get; }

        protected VersionedTypeDefinition(
            int version,
            Type type,
            string name)
        {
            Version = version;
            Type = type;
            Name = name;
        }

        public override string ToString()
        {
            var assemblyName = Type.GetTypeInfo().Assembly.GetName();
            return $"{Name} v{Version} ({assemblyName.Name} - {Type.PrettyPrint()})";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;
            yield return Type;
            yield return Name;
        }
    }
}
