# Akkatecture Cluster Sample

The Cluster Sample is made up of three projects, `Akkatecture.Examples.ClusterClient`, `Akkatecture.Examples.Worker`, and `Akkatecture.Examples.Seed`. Here is a brief description of each project.

## Akkatecture.Examples.ClusterClient

The cluster client project is the code that interfaces with the cluster. It can be seen as the way to invoke the domain from an external place. It works by opening a proxy to the actual worker(s). All requests done through the client are transported / serialized / deserialized through the actor system and the requests are routed to the correct cluster shard that runs on the worker.

## Akkatecture.Examples.Worker

The worker is where all the real computation is done. The worker is responsible for routing commands to the right aggregates and the aggregates then process the commands, which may or may not result in events being emitted from them, resulting in those aggregate events being persisted to their event journal. In typical scenarios the worker would be deployed in numerous nodes within a network on the cloud or in a kubernetes cluster.

## Akkatecture.Examples.Seed

The seed or seed node is only responsible for akka service discovery. Its only purpose is to exist at a well known location so that nodes within the actor system can effectively discover each other through the seed node. It is important that the seed does no work that can potentially disrupt its service level. In this sample the seeds address is 127.0.0.1:6000, the 10 workers are addressed from 127.0.0.1:6001-6011, and the console client is on 127.0.0.6100.

### Description

This sample uses the same domain model as the one found in the [simple](https://github.com/Lutando/Akkatecture/tree/dev/examples/simple/Akkatecture.Examples.Domain) sample. Since we are using the same domain model that is used in a simple non-clustered setting, you will now see how with Akkatecture, you can design your domain once and have it scale to 1 or many nodes with just a configuration change. Thanks to actors.

If you take a look at the hocon configurations for the [client](https://github.com/Lutando/Akkatecture/tree/dev/examples/cluster/Akkatecture.Examples.ClusterClient/client.conf), the [worker](https://github.com/Lutando/Akkatecture/tree/dev/examples/cluster/Akkatecture.Examples.Worker/worker.conf), and the [seed](https://github.com/Lutando/Akkatecture/tree/dev/examples/cluster/Akkatecture.Examples.Seed/seed.conf), you will see that they all point to the same well known address (127.0.0.1:6000). This address is the seed nodes address, it tells the actor systems to use that address to initiate cluster gossip, and thus to establish the clustered network of actor systems. Each of the projects use the default akkatecture hocon configuration as a fallback configuration because this configuration has akkatecture opinionated "sane" defaults.

The sample is as easy as running all three projects at the same time. To interact with the domain just press enter on the clients console window, resulting in random commands being created, which are then fed through the cluster proxy that will serialize, deserialize, and route the commands to the necessary aggregate. To exit the applications safely, Press `Q` and enter. 

The purpose of this example is to show how you can distribute your actorsystem using `Akka.Cluster`. Doing this does not require any changes to your domain code. Basically now your domain can be distributed across separate, networked nodes.