# Akkatecture
A CQRS+ES Toolkit for Akka.NET.

# Motivations
Doing domain driven design in a distributed scenario is quite tricky. And even more so when you add CQRS and event sourcing style mechanics to your business domain logic it does cause quite a few uncomfortable realizations. Akka gives you powerful ways to co-ordinate and organise your business rules and it also gives you the power of distributed actors  which can be sent messages by location transparent addresses. 

Akkatecture gives you a set of semi-opinionated generic constructs that you can use to wire up your application so that you can focus on your main task, modelling and codifying your business domain.

# What it is not
This is not meant to be a framework for CQRS and ES. This toolkit is made such that you can enhance your Akka.NET based CQRS/ES solution, at anytime you can break out of the framework to do as you please within the Akka framework.

# Assumptions about Akkatecture users
You have a good enough grasp of domain driven design, cqrs, and event sourcing.
You have also used and played around with Akka.NET and are familiar with actor models in general.
