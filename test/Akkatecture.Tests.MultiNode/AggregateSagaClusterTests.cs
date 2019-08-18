using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.TestKit;
using Akka.Cluster.Tests.MultiNode;
using Akka.Configuration;
using Akka.Remote.TestKit;
using Akkatecture.Aggregates;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events;

namespace Akkatecture.Tests.MultiNode
{
    
    public class AggregateSagaClusterTestConfig : MultiNodeConfig
    {
        public RoleName Seed { get; }
        public RoleName Client { get; }
        public RoleName Worker { get; }

        public AggregateSagaClusterTestConfig()
        {
            Seed = Role("seed");
            Client = Role("client");
            Worker = Role("worker");
            
            CommonConfig = 
                ConfigurationFactory.ParseString($@"
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
    public class AggregateSagaClusterTests :  MultiNodeClusterSpec
    {
        public static Guid TestIdNamespace = Guid.Parse("9b235972-af6b-47eb-bcfe-09e8941fb2a7");
        private readonly AggregateSagaClusterTestConfig _config;
        private ImmutableList<Address> _seedNodes;
        private readonly int _numberOfShards;
        private readonly Lazy<IActorRef> _aggregateProxy;
        private readonly Lazy<IActorRef> _aggregateRegion;
        
        
        public AggregateSagaClusterTests()
            : this(new AggregateSagaClusterTestConfig())
        {
            
        }

        protected AggregateSagaClusterTests(AggregateSagaClusterTestConfig config)
            : base(config, typeof(AggregateSagaClusterTests))
        {
            _config = config;
            _numberOfShards = 1;
            var aggregateManagerName = typeof(TestAggregateManager).Name;
            _aggregateProxy = new Lazy<IActorRef>(() => ClusterSharding.Get(Sys).ShardRegionProxy(aggregateManagerName));
            _aggregateRegion = new Lazy<IActorRef>(() => ClusterSharding.Get(Sys).ShardRegion(aggregateManagerName));
        }
        
        [MultiNodeFact]
        public void AggregateSagaClusterFacts()
        {
            _seedNodes = ImmutableList.Create(GetAddress(_config.Seed));
            
            A_cluster_can_be_formed();
            Aggregate_managers_can_be_started();
            Client_proxies_can_be_started();
            Saga_managers_can_be_started();
            Aggregates_can_be_hydrated();
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
                
                region.Tell(new Identify(99));
                
                ExpectMsg<ActorIdentity>(x => x.MessageId.Equals(99));
                
                Sys.Log.Info("aggregate region address: {0}", region.Path);
                
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
        
        public void Saga_managers_can_be_started()
        {
            RunOn(() =>
            {
                
                var region = ClusterFactory<TestSagaManager, TestSaga, TestSagaId, TestSagaLocator>
                    .StartClusteredAggregateSaga(Sys, () => new TestSaga(_aggregateRegion.Value), "worker", _numberOfShards);
                
                region.Tell(new Identify(1));
                
                ExpectMsg<ActorIdentity>(x => x.MessageId.Equals(1));
                
                Sys.Log.Info("saga region address: {0}", region.Path);
                
            }, _config.Worker);
            
            EnterBarrier("aggregate-managers-started");
        }

        public void Aggregates_can_be_hydrated()
        {
            var senderTestId = TestId.NewDeterministic(TestIdNamespace, (DateTime.UtcNow.Day - 1).ToString());
            var senderTest = new Test(senderTestId);
            var receiver = TestAggregateId.NewDeterministic(TestIdNamespace, (DateTime.UtcNow.Day + 1).ToString());
            var sender = TestAggregateId.NewDeterministic(TestIdNamespace, DateTime.UtcNow.Day.ToString());
            
            RunOn(() =>
            {
                //set up event probe
                // set up sender
                var senderProbe = CreateTestProbe("sender-probe");
                var commandId = CommandId.New;
                _aggregateProxy.Value.Tell(new CreateTestCommand(sender, commandId), senderProbe);

                senderProbe.ExpectMsg<TestExecutionResult>(
                    x => x.Result.IsSuccess 
                         && x.SourceId.Value == commandId.Value ,TimeSpan.FromSeconds(10));
                
                
                var nextAggregateCommand = new AddTestCommand(sender, CommandId.New, senderTest);
                _aggregateProxy.Value.Tell(nextAggregateCommand, senderProbe);

                senderProbe.ExpectMsg<TestExecutionResult>(
                    x => x.Result.IsSuccess, TimeSpan.FromSeconds(10));
                
                // set up receiver
                var receiverProbe = CreateTestProbe("receiver-probe");
                var commandId2 = CommandId.New;
                _aggregateProxy.Value.Tell(new CreateTestCommand(receiver, commandId2), receiverProbe);

                receiverProbe.ExpectMsg<TestExecutionResult>(
                    x => x.Result.IsSuccess 
                         && x.SourceId.Value == commandId2.Value ,TimeSpan.FromSeconds(10));

            }, _config.Client);
            
            
            EnterBarrier("aggregates-hydrated");
            
            var eventProbe = CreateTestProbe("event-probe");
            
            RunOn(() =>
            {
                Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>));
                Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>));
                Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>));
            }, _config.Worker);
            
            RunOn(() =>
            {
                var senderProbe = CreateTestProbe("saga-starting-probe");
                
                var nextAggregateCommand = new GiveTestCommand(sender, CommandId.New, receiver, senderTest);
                _aggregateProxy.Value.Tell(nextAggregateCommand,senderProbe);
                    
                senderProbe.ExpectMsg<TestExecutionResult>(
                    x => x.Result.IsSuccess, TimeSpan.FromSeconds(10));
                    
            }, _config.Client);
            
            RunOn(() =>
            {
                eventProbe.
                    ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>>(
                        x => x.AggregateEvent.Sender.Equals(sender)
                             && x.AggregateEvent.Receiver.Equals(receiver)
                             && x.AggregateEvent.SentTest.Equals(senderTest), TimeSpan.FromSeconds(20));
            
                eventProbe.
                    ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>>(TimeSpan.FromSeconds(20));
            
                eventProbe.
                    ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>>(TimeSpan.FromSeconds(20)); 
                
            }, _config.Worker);
            
            
            EnterBarrier("saga-finished");
        }
    }
}