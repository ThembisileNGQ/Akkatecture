using System;

namespace Akkatecture.Sagas
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SagaNameAttribute : Attribute
    {
        public string Name { get; }

        public SagaNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
        }
        
    }
}