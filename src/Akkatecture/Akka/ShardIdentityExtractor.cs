using System;
using Akkatecture.Core;

namespace Akkatecture.Akka
{
    public class ShardIdentityExtractor
    {
        public static Tuple<string, object> IdentityExtrator(object message)
        {
            if (message is IIdentity command)
                return new Tuple<string, object>(command.Value, message);

            throw new ArgumentNullException(nameof(message));
        }
    }
    
}