using System;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandVersionAttribute : VersionedTypeAttribute
    {
        public CommandVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}