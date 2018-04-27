using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Subscribers
{
    public abstract class DomainEventSubscriber : ReceiveActor
    {
        public DomainEventSubscriberSettings Settings { get; }

        private readonly Type Type;

        

        protected DomainEventSubscriber()
        {
            var config = Context.System.Settings.Config;
            Settings = new DomainEventSubscriberSettings(config);
            Type = GetType();

            if (Settings.AutoSubscribe)
            {
                var subscriptionTypes =
                    Type
                        .GetDomainEventSubscriberSubscriptionTypes();

                foreach (var type in subscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
            }



            if (Settings.AutoReceive)
            {
                var subscriptionTypes =
                    Type
                        .GetDomainEventSubscriberSubscriptionTypes();

                var methods = Type
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


                var method = Type
                    .GetBaseType("ReceiveActor")
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(mi =>
                    {
                        if (mi.Name != "ReceiveAsync") return false;
                        var parameters = mi.GetParameters();
                        var o = parameters[0].ParameterType;
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