using System;
using Akkatecture.Core;

namespace Akkatecture.Clustering.Core
{
    public class ShardIdentityExtractor
    {
        public static Tuple<string, object> IdentityExtrator(object message)
        {
            if(message is null)
                throw new ArgumentNullException();
            
            if (message is IIdentity command)
                return new Tuple<string, object>(command.Value, message);

            throw new ArgumentException(nameof(message));
        }
    }
    
}