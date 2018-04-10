# Akkatecture
A CQRS+ES Toolkit for Akka.NET. Fully optimised around using Tell() and Akka's event stream pub sub mechanism. All written in .NET Core (netstandard 2.0).

### Features

* **CQRS+ES tool kit** Backed by Akka.NET's persistent actors, CQRS/ES is easy to build out.
* **Don'Ask, Tell:** For high levels of message throughput and processing Akkatecture [does not Ask, it tells](http://bartoszsypytkowski.com/dont-ask-tell-2/).
* **Highly configurable and extendable** Both via APIs and Akka's HOCON configuration.
* **Highly scalable** Actors with their thread safe and distributed nature gives us this plus point

  ### Examples

* **[Simple](https://github.com/Lutando/Akkatecture/tree/master/examples):** A simple console based example that shows the most simple example of how to create an aggregate and issue commands to it.
* **[Test Example](https://github.com/Lutando/Akkatecture/tree/master/test/Akkatecture.TestHelpers/Aggregates):** The test examples found in the Akkatecture.TestHelpers project is there to provide assistance when doing testing for Akkatecture. There is a simple aggregate with a simple aggregate saga, and these are used to do simple black box style testing on Akkatecture using Akka.NET's TestKit.

**Note:** This example is part of the Akkatecture simple example project, so [checkout](https://github.com/Lutando/Akkatecture/blob/master/examples/Akkatecture.Examples.UserAccount.Application/Program.cs#L13) the
code and give it a run.
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

### Assumptions About Akkatecture Users
You have a good enough grasp of domain driven design, cqrs, and event sourcing.
You have also used and played around with Akka.NET and are familiar with actor models in general. You also have a solid grasp of how event sourcing works with persisted actors in Akka.NET.

### Status of Akkatecture
Akkatecture is still in development. The goal of this projects first version is to provide you with the neccassary building blocks to build out your own CQRS/ES solution without having to think of the dirty primitives.

### Useful Resources for Akka and DDD, CQRS, and ES

What comes with territory of having a toolkit that presumes a bit of intermediary knowledge about domain driven design is that there are many different opinions and best practices when it comes to building out DDD based solutions. Here are a few articles and resources that can give you a good foundational grounding on the concepts used extensively in this project.

#### Domain-Driven Design
 - [Domain-Driven Design Reference](https://domainlanguage.com/ddd/reference/) by Eric Evans
#### General CQRS+ES
 - [CQRS Journey by Microsoft](https://msdn.microsoft.com/en-us/library/jj554200.aspx)
   published by Microsoft
 - [An In-Depth Look At CQRS](http://blog.sapiensworks.com/post/2015/09/01/In-Depth-CQRS/)
   by Mike Mogosanu
 - [CQRS, Task Based UIs, Event Sourcing agh!](http://codebetter.com/gregyoung/2010/02/16/cqrs-task-based-uis-event-sourcing-agh/)
   by Greg Young
 - [Busting some CQRS myths](https://lostechies.com/jimmybogard/2012/08/22/busting-some-cqrs-myths/)
   by Jimmy Bogard
 - [CQRS applied](https://lostechies.com/gabrielschenker/2015/04/12/cqrs-applied/)
   by Gabriel Schenker
#### Eventual consistency
 - [How To Ensure Idempotency In An Eventual Consistent DDD/CQRS Application](http://blog.sapiensworks.com/post/2015/08/26/How-To-Ensure-Idempotency)
   by Mike Mogosanu
#### Video Content
- [CQRS and Event Sourcing](https://www.youtube.com/watch?v=JHGkaShoyNs) by Eric Evans
- [An Introduction to CQRS and Event Sourcing Patterns](https://www.youtube.com/watch?v=9a1PqwFrMP0&t=2042s) by Mathew McLoughling
- [The Future of Distributed Programming in .NET](https://www.youtube.com/watch?v=ozelpjr9SXE&t=2140s) by Aaron Stannard


## Motivations
Doing domain driven design in a distributed scenario is quite tricky. And even more so when you add CQRS and event sourcing style mechanics to your business domain. Akka gives you powerful ways to co-ordinate and organise your business rules by using actors and message passing, which can be done by sending messages through location transparent addresses (or references). The major benefits of using Akka.NET is that we can isolate our domain models into actors where it makes sense.

Akkatecture gives you a set of semi-opinionated generic constructs that you can use to wire up your application so that you can focus on your main task, modelling and codifying your business domain.

Akka.NET gives us a wealth of good APIs out of the box that can be used to build entire systems out of. It also has a decent ecosystem and community for support. I also am of the opinion that commands translate well semantically in actor systems since you are telling the actor what you want, and the actor might or might not "respond" with a fact or a bag of facts relating to what that command produced in context of that aggregate.

### Personal Motivations

I find the lack of good domain driven design frameworks and libraries for akka.net quite sad. There are a few out there if you look hard enough but they fail in one or two aspects that I find really important. I really like the APIs that ReceiveActors and ReceivePersistentActors expose as opposed to their base variants. I find the APIs to be far more cleaner and geared towards a better functional programming paradigm, which can lead to code that is more readable, testable, and maintainable. Although nothing is perfect. Akkatecture tries to make your domain semi-declarative and at least highly readable and maintainable. 

# What it is not

This is not meant to be a framework for CQRS and ES. This toolkit is made such that you can enhance your Akka.NET based CQRS/ES solution, at anytime you can break out of the framework to do as you please within the Akka framework as per usual.

### Thanks

A huge thank you goes out to [EventFlow](https://github.com/eventflow/EventFlow) which is where Akkatecture draws most of its API surface inspiration from. A large amount of the CQRS/ES/DDD primitives come from that project. Furthermore, the [Akka.NET](https://github.com/akkadotnet/akka.net) project and the communities surrounding the ecosystem needs great praise, without Akka.NET this project would not be possible. And finally a thanks goes out to [Nact](https://nact.io/) for a good basis to form this projects documentation on.


## License

```
The MIT License (MIT)

Copyright (c) 2015-2018 Rasmus Mikkelsen
Copyright (c) 2015-2018 eBay Software Foundation
https://github.com/eventflow/EventFlow

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