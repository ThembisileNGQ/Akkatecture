using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.TestKit;
using Akka.Cluster.Tests.MultiNode;
using Akka.Configuration;
using Akka.Remote.TestKit;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;

namespace Akkatecture.Tests.MultiNode
{
    public class AggregateClusterTestConfig : MultiNodeConfig
    {
        public RoleName Seed { get; }
        public RoleName Client { get; }
        public RoleName Worker { get; }

        public AggregateClusterTestConfig()
        {
            Seed = Role("seed");
            Client = Role("client");
            Worker = Role("worker");
            
            CommonConfig = 
                ConfigurationFactory.ParseString(@"
                    akka.loglevel = INFO
                ")
                    .WithFallback(MultiNodeLoggingConfig.LoggingConfig)
                    .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig())
                    .WithFallback(MultiNodeClusterSpec.ClusterConfig());
            
            
            NodeConfig(new [] { Seed }, new [] {
                ConfigurationFactory.ParseString(@"
                    akka.cluster.roles=[""seed""]
                    akka.cluster.sharding.role = ""seed""
                    ")
            });
            
            NodeConfig(new [] { Client }, new [] {
                ConfigurationFactory.ParseString(@"
                    akka.cluster.roles=[""client""]
                    akka.cluster.sharding.role = ""client""
                    ")
            });
            
            NodeConfig(new [] { Worker }, new [] {
                ConfigurationFactory.ParseString(@"
                    akka.cluster.roles=[""worker""]
                    akka.cluster.sharding.role = ""worker""
                    ")
            });
            
        }
    }
    [SuppressMessage("ReSharper", "xUnit1013")]
    public class AggregateClusterTests : MultiNodeClusterSpec
    {
        private readonly AggregateClusterTestConfig _config;
        private ImmutableList<Address> _seedNodes;
        private readonly int _numberOfShards;
        private readonly Lazy<IActorRef> _aggregateManagerProxy;
        public AggregateClusterTests()
            : this(new AggregateClusterTestConfig())
        {
            
        }

        protected AggregateClusterTests(AggregateClusterTestConfig config)
            : base(config, typeof(AggregateClusterTests))
        {
            _config = config;
            _numberOfShards = 32;
            var aggregateManagerName = typeof(TestAggregateManager).Name;
            _aggregateManagerProxy = new Lazy<IActorRef>(() => ClusterSharding.Get(Sys).ShardRegionProxy(aggregateManagerName));
        }

        [MultiNodeFact]
        public void AggregateClusterTestFacts()
        {
            _seedNodes = ImmutableList.Create(GetAddress(_config.Seed));
            
            A_cluster_can_be_formed();
            Aggregate_managers_can_be_started();
            Client_proxies_can_be_started();
            Client_proxy_can_call_aggregate();
        }
        
        public void A_cluster_can_be_formed()
        {
            RunOn(() =>
            {
                Cluster.JoinSeedNodes(_seedNodes);
                AwaitMembersUp(1);
            },_config.Seed);
            
            EnterBarrier("seed-started");
            
            RunOn(() =>
            {
                Cluster.JoinSeedNodes(_seedNodes);
                
            }, _config.Worker, _config.Client);
            
            AwaitMembersUp(Roles.Count);
            
            EnterBarrier("cluster-formed");
        }

        public void Aggregate_managers_can_be_started()
        {
            RunOn(() =>
            {
                var region = ClusterFactory<TestAggregateManager, TestAggregate, TestAggregateId>
                    .StartClusteredAggregate(Sys,_numberOfShards);
                
                region.Tell(new Identify(1));
                
                ExpectMsg<ActorIdentity>(x => x.MessageId.Equals(1));
                
                Sys.Log.Info("region address: {0}", region.Path);
                
            }, _config.Worker);
            
            EnterBarrier("aggregate-managers-started");
        }
        
        public void Client_proxies_can_be_started()
        {
            RunOn(() =>
            {
                var proxy = ClusterFactory<TestAggregateManager, TestAggregate, TestAggregateId>
                    .StartAggregateClusterProxy(Sys, "worker", _numberOfShards);
                    
                proxy.Tell(new Identify(5));
                    
                ExpectMsg<ActorIdentity>(x => x.MessageId.Equals(5),TimeSpan.FromSeconds(10));
                Sys.Log.Info("proxy address: {0}", proxy.Path.ToString());

            }, _config.Client);
            
            EnterBarrier("client-proxies-started");
        }

        public void Client_proxy_can_call_aggregate()
        {
            
                RunOn(() =>
                {
                    var probe = CreateTestProbe();
                    var commandId = CommandId.New;
                    var aggregateId = TestAggregateId.New;
                    _aggregateManagerProxy.Value.Tell(new CreateTestCommand(aggregateId, commandId), probe);

                    probe.ExpectMsg<TestExecutionResult>(
                        x => x.Result.IsSuccess
                               && x.SourceId.Value == commandId.Value ,TimeSpan.FromSeconds(10));
                    
                }, _config.Client);
               
            
            
            EnterBarrier("finished");
        }
        
    } 
}