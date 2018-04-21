using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akkatecture.Clustering.Configuration;

namespace Akkatecture.Examples.Seed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get configuration file using Akkatecture's defaults as fallback
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "seed.conf"); 
            var config = ConfigurationFactory.ParseString(File.ReadAllText(configPath))
                .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());
            var clustername = config.GetString("akka.cluster.name");

            //Create actor system
            var actorSystem = ActorSystem.Create(clustername, config);

            Console.WriteLine("Akkatecture.Examples.Seed Running");
            
            var quit = false;

            while (!quit)
            {
                Console.Write("\rPress Q to Quit.         ");
                var key = Console.ReadLine();
                quit = key?.ToUpper() == "Q";
            }

            //Shut down the local actor system
            actorSystem.Terminate().Wait();
            Console.WriteLine("Akkatecture.Examples.Seed Exiting.");
        }
    }
    
    
}
