using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Extensions;

namespace Akkatecture.Subscribers
{
    public abstract class DomainEventSubscriber : ReceiveActor
    {
        public DomainEventSubscriberSettings Settings { get; }


        protected DomainEventSubscriber()
        {
            Settings = new DomainEventSubscriberSettings(Context.System.Settings.Config);
            
            var type = GetType();

            if (Settings.AutoSubscribe)
            {
                var subscriptionTypes =
                    type
                        .GetDomainEventSubscriberSubscriptionTypes();

                foreach (var subscriptionType in subscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, subscriptionType);
                }
            }

            if (Settings.AutoReceive)
            {
                var subscriptionTypes =
                    type
                        .GetDomainEventSubscriberSubscriptionTypes();

                var methods = type
                    .GetTypeInfo()
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(mi =>
                    {
                        if (mi.Name != "Handle") return false;
                        var parameters = mi.GetParameters();
                        return
                            parameters.Length == 1;
                    })
                    .ToDictionary(
                        mi => mi.GetParameters()[0].ParameterType,
                        mi => mi);


                var method = type
                    .GetBaseType("ReceiveActor")
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(mi =>
                    {
                        if (mi.Name != "ReceiveAsync") return false;
                        var parameters = mi.GetParameters();
                        return
                            parameters.Length == 2
                            && parameters[0].ParameterType.Name.Contains("Func");
                    })
                    .First();

                foreach (var subscriptionType in subscriptionTypes)
                {
                    var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(Task));
                    var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                    var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                    actorReceiveMethod.Invoke(this, new []{subscriptionFunction,null});
                }

            }
            
        }
        
    }
}