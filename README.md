
<a href="https://akkatecture.net/"><img src="https://raw.githubusercontent.com/Lutando/Akkatecture/master/logo.svg?sanitize=true" width="100%" height="200"></a>


| nuget | master | dev | quality | chat | who are we? |
|-------|:------:|:---:|:---:|:----:|-------------|
| [![Nuget Package](https://img.shields.io/nuget/v/Akkatecture.svg?style=flat)](https://www.nuget.org/packages/Akkatecture/) | [![Build status](https://dev.azure.com/lutando/Akkatecture/_apis/build/status/Akkatecture%20Master%20CI%20CD?branchName=master)](https://dev.azure.com/lutando/Akkatecture/_build/latest?definitionId=2) | [![Build status](https://dev.azure.com/lutando/Akkatecture/_apis/build/status/Akkatecture%20Development%20CI%20CD?branchName=dev)](https://dev.azure.com/lutando/Akkatecture/_build/latest?definitionId=1) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Lutando_Akkatecture&metric=alert_status)](https://sonarcloud.io/dashboard?id=Lutando_Akkatecture) | [![Come Chat](https://img.shields.io/discord/440517592545755137.svg)](https://akkatecture.net/community) | [![we are reactive](https://img.shields.io/badge/we_are-reactive-blue.svg?style=flat)](https://www.reactivemanifesto.org/) |

# Akkatecture

Akkatecture is a cqrs and event sourcing framework for dotnet core, build ontop of akka.net. Fully optimised around using akka's highly scalable message passing and event stream pub sub mechanisms. Akkatecture targets `netstandard2.0`.

Go ahead and take a look at [our documentation](http://akkatecture.net/docs/getting-started), go over [some concepts](http://akkatecture.net/docs/primitives), and read the [tips and tricks](http://akkatecture.net/docs/tips-and-tricks).


### Features

* **Distributed:** Each aggregate can operate concurrently in parallel inside of the actor system with isolated failure boundaries.
* **Message based:** Making it highly scalable by being reactive from message passing, Akkatecture [does not ask, it tells](http://bartoszsypytkowski.com/dont-ask-tell-2/).
* **Event sourced:** By design, aggregate roots derive their state by replaying persisted events.
* **Highly scalable:** Work proceeds interactively and concurrently, overlapping in time, and may be done across nodes.
* **Configurable:** Through akka.net's hocon configuration, you will be able to configure every aspect of your application.

### Examples

Akkatecture comes with a few prescribed examples on how one might use it:

* **[Simple](https://github.com/Lutando/Akkatecture/tree/master/examples/simple):** A simple console based example that shows the most simple example of how to create an aggregate and issue commands to it.

* **[Walkthrough](https://github.com/Akkatecture/Walkthrough):** Tutorial style sample based on the walkthrough in the Akkatecture documentation. The walkthrough proposes domain that should be modelled based on some business requirements. The walkthrough goes step by step covering all the primitives and features covered in Akkatecture to give you an understanding of the framework. The beginning of the walkthrough can be found [here](https://akkatecture.net/docs/walkthrough-introduction).


* **[Cluster](https://github.com/Lutando/Akkatecture/tree/master/examples/cluster):** A more involved sample that shows you how to do distributed aggregates using clustering. Read the [readme](https://github.com/Lutando/Akkatecture/tree/master/examples/cluster/README.md) for the sample for a good overview of the example.

* **[Web](https://github.com/Lutando/Akkatecture/tree/master/examples/web):** This sample shows how to integrate akka into an aspnet core project, specifically how to inject actor references when using akkatecture. Furthermore this project models a long running process that might be run behind a web application or something similar. Read the [readme](https://github.com/Lutando/Akkatecture/tree/master/examples/web/README.md) for more detailed information about the example.

* **[Tests](https://github.com/Lutando/Akkatecture/tree/master/test/Akkatecture.Tests):** The test examples found in the Akkatecture.Test project is there to provide assistance when doing testing for Akkatecture. There is a simple domain modelled within the [Akkatecture.TestHelpers](https://github.com/Lutando/Akkatecture/tree/master/test/Akkatecture.TestHelpers) project that includes a model of an aggregate with a simple aggregate saga, and these are used to do simple black box style testing on Akkatecture using akka.net's TestKit.


**Note:** This example is part of the Akkatecture simple example project, so checkout [the
code](https://github.com/Lutando/Akkatecture/blob/master/examples/simple/Akkatecture.Examples.Application/Program.cs#L13) and give it a run.
```csharp
//Create actor system
var system = ActorSystem.Create("useraccount-example");

//Create supervising aggregate manager for UserAccount aggregate root actors
var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

//Build create user account aggregate command with name "foo bar"
var aggregateId = UserAccountId.New;
var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, "foo bar");

//Send command, this is equivalent to command.publish() in other cqrs frameworks
aggregateManager.Tell(createUserAccountCommand);

//tell the aggregateManager to change the name of the aggregate root to "foo bar baz"
var changeNameCommand = new UserAccountChangeNameCommand(aggregateId, "foo bar baz");
aggregateManager.Tell(changeNameCommand);
```

### Assumptions About Akkatecture Developers

It would be ideal if you have some expirience in domain driven design, cqrs, and event sourcing.
It would also be beneficial for you to be familiar with actor systems, akka.net, and the extensibility points that akka gives you through hocon configuration. If you need to skill up on akka.net, check out petabridge's [akka-bootcamp](https://github.com/petabridge/akka-bootcamp). If you are already familiar with akka.net, go through the [walkthrough](https://akkatecture.net/docs/walkthrough-introduction) and you would have covered most of the concepts that this framework offers.

### Status of Akkatecture

Akkatecture is still in development. The goal of this projects first version is to provide you with the neccassary building blocks to build out your own cqrs and event sourced application without having to think of the implementation details of akka.net coupled with CQRS and event sourcing. Right now Akkatecture is focussed on developing the story for projection rebuilding. Projection rebuilding is a crucial feature that will lend Akkatecture to a version `1.0.0` release.

akkatecture is currently missing these crucial features:

- resumable projections / read models.
- scheduled jobs / persistent jobs.

### Contributing

**Code** - If you want to contribute to the framework, do so on the `dev` branch and submit a PR. 
**Documentation** - Akkatecture's documentation source is [here](https://github.com/Akkatecture/Documentation), if you have any suggestions or improvements that can be made to them.

All contributions big or small are greatly appreciated!

### Useful Resources

There are many different authoritative sources that prescribe best practices when building these kinds of systems that Akkatecture models. Here are a few articles and resources that can give you a good foundational grounding on the concepts used extensively in this project.

#### Domain-Driven Design

 - [Domain-Driven Design Reference](https://domainlanguage.com/ddd/reference/) by Eric Evans
#### CQRS & Event sourcing

 - [CQRS Journey by Microsoft](https://msdn.microsoft.com/en-us/library/jj554200.aspx)
   published by Microsoft
 - [An In-Depth Look At CQRS](https://blog.sapiensworks.com/post/2015/09/01/In-Depth-CQRS)
   by Mike Mogosanu
 - [CQRS, Task Based UIs, Event Sourcing agh!](http://codebetter.com/gregyoung/2010/02/16/cqrs-task-based-uis-event-sourcing-agh/)
   by Greg Young
 - [Busting some CQRS myths](https://lostechies.com/jimmybogard/2012/08/22/busting-some-cqrs-myths/)
   by Jimmy Bogard
 - [CQRS applied](https://lostechies.com/gabrielschenker/2015/04/12/cqrs-applied/)
   by Gabriel Schenker
#### Eventual consistency

 - [How To Ensure Idempotency In An Eventual Consistent DDD/CQRS Application](https://blog.sapiensworks.com/post/2015/08/26/How-To-Ensure-Idempotency)
   by Mike Mogosanu
#### Video Content

- [CQRS and Event Sourcing](https://www.youtube.com/watch?v=JHGkaShoyNs) by Eric Evans
- [An Introduction to CQRS and Event Sourcing Patterns](https://www.youtube.com/watch?v=9a1PqwFrMP0&t=2042s) by Mathew McLoughling
- [The Future of Distributed Programming in .NET](https://youtu.be/ozelpjr9SXE) by Aaron Stannard

## Motivations

Doing domain driven design in a distributed scenario is quite tricky. And even more so when you add cqrs and event sourcing style mechanics to your business domain. Akka.net gives you powerful ways to co-ordinate and organise your business rules by using actors and message passing, which can be done by sending messages through location transparent addresses (or references). The major benefits of using akka.net is that we can isolate our domain models into actors where it makes sense. There is a high impedance match when you impose the actor model onto domain driven design, because actors can only guarantee their own internal state's consistency just like aggregate roots which maintain their own consistency boundary. Modelling aggregates as actors feels frictionless.

Akkatecture gives you a set of opinionated generic constructs that you can use to wire up your application so that you can focus on your main task, modelling and codifying your business domain.

Akka.net gives us a wealth of good APIs out of the box that can be used to build entire systems out of. It also has a decent ecosystem and [community](https://gitter.im/akkadotnet/akka.net) for support. Akkatecture is also of the opinion that commands translate well semantically in actor systems since telling commands is a form of message passing that fits well into the actor model paradigm.

## Acknowledgements

- [Akka.NET](https://github.com/akkadotnet/akka.net) - The project which AKkatecture builds ontop of, without akka.net, Akkatecture wouldnt exist.
- [EventFlow](https://github.com/eventflow/EventFlow) - Akkatecture has adapted the api surface from event flow to work in the akka actor world.
- [Nact](https://nact.io/) - For giving us basis to write our [documentation](https://akkatecture.net). Powered by [gatsbyjs](https://www.gatsbyjs.org/).

## License


```
The MIT License (MIT)

Copyright (c) 2018 - 2019 Lutando Ngqakaza

https://github.com/Lutando/Akkatecture

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```