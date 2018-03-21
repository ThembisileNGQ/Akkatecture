using System;

namespace Akkatecture.Aggregates
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AggregateNameAttribute : Attribute
    {
        public string Name { get; }

        public AggregateNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}