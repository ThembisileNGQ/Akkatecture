using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Akkatecture.Extensions;
using Akkatecture.ValueObjects;

namespace Akkatecture.Core
{
    public abstract class Identity<T> : SingleValueObject<string>, IIdentity
        where T : Identity<T>
    {
        // ReSharper disable StaticMemberInGenericType
        private static readonly string Name;
        private static readonly Regex ValueValidation;
        // ReSharper enable StaticMemberInGenericType

        static Identity()
        {
            var nameReplace = new Regex("Id$");
            Name = nameReplace.Replace(typeof(T).Name, string.Empty).ToLowerInvariant();
            ValueValidation = new Regex(
                @"^[a-z0-9]+\-(?<guid>[a-f0-9]{8}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{12})$",
                RegexOptions.Compiled);
        }

        public static T New => With(Guid.NewGuid());

        public static T NewDeterministic(Guid namespaceId, string name)
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, name);
            return With(guid);
        }

        public static T NewDeterministic(Guid namespaceId, byte[] nameBytes)
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, nameBytes);
            return With(guid);
        }

        public static T NewComb()
        {
            var guid = GuidFactories.Comb.Create();
            return With(guid);
        }

        public static T With(string value)
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), value);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }

        public static T With(Guid guid)
        {
            var value = $"{Name}-{guid:D}";
            return With(value);
        }

        public static bool IsValid(string value)
        {
            return !Validate(value).Any();
        }

        public static IEnumerable<string> Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                yield return $"Identity of type '{typeof(T).PrettyPrint()}' is null or empty";
                yield break;
            }

            if (!string.Equals(value.Trim(), value, StringComparison.OrdinalIgnoreCase))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' contains leading and/or traling spaces";
            if (!value.StartsWith(Name))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not start with '{Name}'";
            if (!ValueValidation.IsMatch(value))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not follow the syntax '[NAME]-[GUID]' in lower case";
        }

        protected Identity(string value) : base(value)
        {
            var validationErrors = Validate(value).ToList();
            if (validationErrors.Any())
            {
                throw new ArgumentException($"Identity is invalid: {string.Join(", ", validationErrors)}");
            }

            _lazyGuid = new Lazy<Guid>(() => Guid.Parse(ValueValidation.Match(Value).Groups["guid"].Value));
        }

        private readonly Lazy<Guid> _lazyGuid;

        public Guid GetGuid() => _lazyGuid.Value;
    }
}
