using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;

namespace Akkatecture.Examples.Seed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory;
            var configPath = Path.Combine(path, "seed.conf"); 
            var config = ConfigurationFactory.ParseString(File.ReadAllText(configPath));
            var clustername = config.GetString("akka.cluster.name");

            var actorSystem = ActorSystem.Create(clustername, config);

            Console.WriteLine("Akkatecture.Examples.Seed Running");

            while (true)
            {
                
            }
        }
    }
}
