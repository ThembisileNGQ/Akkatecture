// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Examples.Domain.Model.UserAccount;
using Akkatecture.Examples.Domain.Model.UserAccount.Commands;

namespace Akkatecture.Examples.ClusterClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get configuration file using Akkatecture's defaults as fallback
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "client.conf");
            var config = ConfigurationFactory.ParseString(File.ReadAllText(configPath))
                .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());

            //Create actor system
            var clustername = config.GetString("akka.cluster.name");
            var shardProxyRoleName = config.GetString("akka.cluster.singleton-proxy.role");
            var actorSystem = ActorSystem.Create(clustername, config);

            //Instantiate an aggregate proxy reference to the worker node that hosts the 
            //aggregates. This is the reference to proxy client commands to
            var aggregateProxy = StartUserAccountClusterProxy(actorSystem, shardProxyRoleName);

            Console.WriteLine("Press Enter To Create A Random User Account, or Q to quit.");
            
            var key = Console.ReadLine();
            var quit = key?.ToUpper() == "Q";

            while (!quit)
            {
                //Generate random, new UserAccount
                var aggregateId = UserAccountId.New;
                var randomUserAccountName = Guid.NewGuid().ToString();
                var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, randomUserAccountName);
                
                //Send the command
                aggregateProxy.Tell(createUserAccountCommand);

                Console.WriteLine($"CreateUsrAccountCommand: Id={createUserAccountCommand.AggregateId}; Name={createUserAccountCommand.Name} Sent.");

                Console.WriteLine("Press Enter To Create Another Random User Account, or Q to quit.");
                key = Console.ReadLine();
                quit = key?.ToUpper() == "Q";
            }
            
            //Shut down the local actor system
            actorSystem.Terminate().Wait();
            Console.WriteLine("Akkatecture.Examples.ClusterClient Exiting.");         
        }

        public static IActorRef StartUserAccountClusterProxy(ActorSystem actorSystem, string proxyRoleName)
        {
            var clusterProxy = ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
                .StartAggregateClusterProxy(actorSystem, proxyRoleName);

            return clusterProxy;
        }
    }
}
