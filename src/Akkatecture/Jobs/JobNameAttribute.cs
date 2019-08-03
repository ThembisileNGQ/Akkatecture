using System;

namespace Akkatecture.Jobs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JobNameAttribute : Attribute
    {
        public string Name { get; }

        public JobNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}