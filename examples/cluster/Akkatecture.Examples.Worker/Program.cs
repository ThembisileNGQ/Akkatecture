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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Akkatecture.Clustering.Configuration;
using Akkatecture.Clustering.Core;
using Akkatecture.Examples.Domain.Model.UserAccount;

namespace Akkatecture.Examples.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get configuration file using Akkatecture's defaults as fallback
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "worker.conf");
            var baseConfig = ConfigurationFactory.ParseString(File.ReadAllText(configPath));
            
            //specified amount of workers running on their own thread
            var amountOfWorkers = 10;

            //Create several workers with each worker port will be 6001, 6002,...
            var actorSystems = new List<ActorSystem>();
            foreach (var worker in Enumerable.Range(1, amountOfWorkers+1))
            //foreach (var worker in Enumerable.Range(1, 1))
            {
                //Create worker with port 600X
                var config = ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = 600{worker}");
                config = config
                    .WithFallback(baseConfig)
                    .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());
                var clustername = config.GetString("akka.cluster.name");
                var actorSystem = ActorSystem.Create(clustername, config);
                actorSystems.Add(actorSystem);
                
                //Start the aggregate cluster, all requests being proxied to this cluster will be 
                //sent here to be processed
                StartUserAccountCluster(actorSystem);
            }

            Console.WriteLine("Akkatecture.Examples.Workers Running");

            var quit = false;
            
            while (!quit)
            {
                Console.Write("\rPress Q to Quit.         ");
                var key = Console.ReadLine();
                quit = key?.ToUpper() == "Q";
            }

            //Shut down all the local actor systems
            foreach (var actorsystem in actorSystems)
            {
                actorsystem.Terminate().Wait();
            }
            Console.WriteLine("Akkatecture.Examples.Workers Exiting.");
        }

        public static void StartUserAccountCluster(ActorSystem actorSystem)
        {
            ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
                .StartClusteredAggregate(actorSystem);
        }
        
    }
}
