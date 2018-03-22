using Akka.Actor;
using Akkatecture.Core;

namespace Akkatecture.Extensions
{
    public static class ActorContextExtensions
    {
        public static IActorRef Child<TIdentity>(this IActorContext context, TIdentity identity)
            where TIdentity : IIdentity
        {
            return context.Child(identity.Value);
        }
    }
}
