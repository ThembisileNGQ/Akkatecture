
<a href="https://akkatecture.github.io/"><img src="https://raw.githubusercontent.com/Lutando/Akkatecture/master/logo.svg?sanitize=true" width="100%" height="200"></a>

# Akkatecture

Akkatecture is a cqrs and event sourcing toolbox for akka.net. Fully optimised around using akka's tell() and event stream pub sub mechanism for event propegation. In .net core (targeting netstandard 2.0).

Go ahead and take a look at [our documentation](http://localhost:8000/docs/getting-started), go over [some concepts](http://localhost:8000/docs/primitives), and read the [tips and tricks](http://localhost:8000/docs/tips-and-tricks).

### Features

* **Distributed:** Backed by akka.net's persistent actors, cqrs and event sourcing is easy to build out with Akkatecture.
* **Message based:** For high levels of message throughput and processing Akkatecture [does not ask, it tells](http://bartoszsypytkowski.com/dont-ask-tell-2/).
* **Event sourced** By design, aggregate roots derive their state by replaying persisted events.
* **Highly scalable** Actors with their thread safe and distributed nature gives us this plus point.
* **Configurable** Through akka.net's hocon configuration.

### Examples

* **[Simple](https://github.com/Lutando/Akkatecture/tree/master/examples/simple):** A simple console based example that shows the most simple example of how to create an aggregate and issue commands to it.

* **[Cluster](https://github.com/Lutando/Akkatecture/tree/master/examples/cluster):** A more involved sample that shows you how to do distributed aggregates using clustering. Read the [readme](https://github.com/Lutando/Akkatecture/tree/master/examples/cluster/README.md) for the sample for a good overview of the example.

* **[Test Example](https://github.com/Lutando/Akkatecture/tree/master/test/Akkatecture.TestHelpers/Aggregates):** The test examples found in the Akkatecture.TestHelpers project is there to provide assistance when doing testing for Akkatecture. There is a simple domain modelled that includes an aggregate with a simple aggregate saga, and these are used to do simple black box style testing on Akkatecture using akka.net's TestKit.


**Note:** This example is part of the Akkatecture simple example project, so checkout [the
code](https://github.com/Lutando/Akkatecture/blob/master/examples/simple/Akkatecture.Examples.UserAccount.Application/Program.cs#L13) and give it a run.
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

You should have a comfortable grasp of domain driven design, cqrs, and event sourcing concepts.
It would also be benefitial for you to be familiar with actor systems, akka.net, and the extensibility points that akka gives you through hocon configuration.

### Status of Akkatecture

Akkatecture is still in development. The goal of this projects first version is to provide you with the neccassary building blocks to build out your own cqrs and event sourced application without having to think of the primitives. Akkatecture is still extensible by standard akka methods (hocon) so do feel free to apply those extension points where necassary.

### Useful Resources

There are many different opinions and best practices when it comes to building out ddd based solutions. Here are a few articles and resources that can give you a good foundational grounding on the concepts used extensively in this project.

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

Doing domain driven design in a distributed scenario is quite tricky. And even more so when you add cqrs and event sourcing style mechanics to your business domain. Akka gives you powerful ways to co-ordinate and organise your business rules by using actors and message passing, which can be done by sending messages through location transparent addresses (or references). The major benefits of using akka.net is that we can isolate our domain models into actors where it makes sense.

Akkatecture gives you a set of semi-opinionated generic constructs that you can use to wire up your application so that you can focus on your main task, modelling and codifying your business domain.

Akka.net gives us a wealth of good APIs out of the box that can be used to build entire systems out of. It also has a decent ecosystem and community for support. I also am of the opinion that commands translate well semantically in actor systems since you are telling the actor what you want, and the actor might or might not "respond" with a fact or a bag of facts relating to what that command produced in context of that aggregate.

## Acknowledgements

- [Akka.NET](https://github.com/akkadotnet/akka.net) - The project which AKkatecture builds ontop of, without akka.net, Akkatecture wouldnt exist.
- [EventFlow](https://github.com/eventflow/EventFlow) - Where Akkatecture draws most of its API surface inspiration from. A large amount of the CQRS/ES/DDD primitives come from that project, and have been adapted to work in the akka eco system.
- [Nact](https://nact.io/) - For giving us basis to write our [documentation](https://akkatecture.github.io).

## License


```
The MIT License (MIT)

Copyright (c) 2018 Lutando Ngqakaza

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