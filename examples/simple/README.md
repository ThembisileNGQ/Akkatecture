# Akkatecture Simple Sample

The akkatecture simple sample is the most "hello world" sample of Akkatecture. what will follow is an explanation of the two projects that relate to this example.

# Akkatecture.Examples.Domain

This is the domain of the sample which is a basic model of user accounts, it supports two intents/commands, the creation of a user account and you can also change the user account's name. These commands if successful result in domain events being emitted that then are applied to the aggregates state, persisted to the event store, and are then published  through the actor system as domain events.

### Commands
* **CreateUserAccountCommand** - Command for creating a new user account.
* **UserAccountChangeNameCommand** - Command for changing a user account's name.
### Events
* **UserAccountCreatedEvent** - emitted when a new user account is created.
* **UserAccountNameChangedEvent** - Emitted when a user accounts name has been changed.

# Akkatecture.Examples.Application

This is the console application that creates the actor system and interfaces with it. First an aggregate manager is created, this aggregate manager is responsible for creating the aggregate actors beneath it, it is also responsible for supervising these actors and for routing messages to the appropriate aggregates. After this is done, a command for aggregate creation is made, and then the command is pushed through the aggregate manager, then a second command is done that pushed a change name command to the same aggregate that was previously created. Run the code to see how it works.

### Description

In this sample, we instantiate the actor system and the various domain entities required for it to work. The domain entity required to 
initialize this, is the `UserAccountAggregateManager`. We then interface with the domain by telling the manager to create user accounts 
by instantiating and telling it a `CreateUserAccountCommand`.