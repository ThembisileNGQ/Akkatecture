# Akkatecture
A CQRS+ES Toolkit for Akka.NET.

# Motivations
Doing domain driven design in a distributed scenario is quite tricky. And even more so when you add CQRS and event sourcing style mechanics to your business domain. Akka gives you powerful ways to co-ordinate and organise your business rules by using actors and message passing, which can be done by sending messages by location transparent addresses. The major benefits of using Akka.NET is that we can isolate our domain models into actors where it makes sense.

Akkatecture gives you a set of semi-opinionated generic constructs that you can use to wire up your application so that you can focus on your main task, modelling and codifying your business domain.

Akka.NET gives us a wealth of good APIs out of the box that can be used to build entire systems out of. It also has a decent ecosystem and community for support. I also am of the opinion that commands translate well semantically in actor systems since you are telling the actor what you want, and the actor might or might not "respond" with a fact or a bag of facts relating to what that command produced in context of that aggregate.

# What it is not
This is not meant to be a framework for CQRS and ES. This toolkit is made such that you can enhance your Akka.NET based CQRS/ES solution, at anytime you can break out of the framework to do as you please within the Akka framework.

# Assumptions about Akkatecture users
You have a good enough grasp of domain driven design, cqrs, and event sourcing.
You have also used and played around with Akka.NET and are familiar with actor models in general. You also have a solid grasp of how event sourcing works with persisted actors in Akka.NET.
