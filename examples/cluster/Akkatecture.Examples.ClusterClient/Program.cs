using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;

namespace Akkatecture.Examples.ClusterClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "client.conf");
            var config = ConfigurationFactory.ParseString(File.ReadAllText(configPath))
                .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());

            var clustername = config.GetString("akka.cluster.name");
            var shardProxyRoleName = config.GetString("akka.cluster.singleton-proxy.role");

            var actorSystem = ActorSystem.Create(clustername, config);

            var aggregateProxy = StartUserAccountClusterProxy(actorSystem, shardProxyRoleName);

            Console.WriteLine("Press Enter To Create A Random User Account");
            Console.ReadLine();

            while (true)
            {
                var aggregateId = UserAccountId.New;
                var randomUserAccountName = Guid.NewGuid().ToString();
                var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, randomUserAccountName);
                
                aggregateProxy.Tell(createUserAccountCommand);

                Console.WriteLine($"CreateUsrAccountCommand: Id={createUserAccountCommand.AggregateId}; Name={createUserAccountCommand.Name} Sent.");

                Console.WriteLine("Press Enter To Create Another Random User Account");
                Console.ReadLine();
            }
            
        }


        public static IActorRef StartUserAccountClusterProxy(ActorSystem actorSystem, string proxyRoleName)
        {
            var clusterProxy = ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
                .StartAggregateClusterProxy(actorSystem, proxyRoleName);

            return clusterProxy;
        }
    }
}
