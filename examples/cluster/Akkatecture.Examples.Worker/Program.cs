using System;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel;

namespace Akkatecture.Examples.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "worker.conf");
            var baseConfig = ConfigurationFactory.ParseString(File.ReadAllText(configPath));

            

            foreach (var worker in Enumerable.Range(1,2))
            {
                var config = ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = 600{worker}");
                config = config
                    .WithFallback(baseConfig)
                    .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());
                var clustername = config.GetString("akka.cluster.name");
                var actorSystem = ActorSystem.Create(clustername, config);
                StartUserAccountCluster(actorSystem);
            }

            Console.WriteLine("Akkatecture.Examples.Workers Running");

            while (true)
            {

            }
        }

        public static void StartUserAccountCluster(ActorSystem actorSystem)
        {
            var cluster = ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
                .StartAggregateCluster(actorSystem);
        }

        public static void StartUserAccountClusterProxy(ActorSystem actorSystem)
        {
            var clusterProxy = ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
                .StartAggregateClusterProxy(actorSystem,"worker");
        }
    }
}
