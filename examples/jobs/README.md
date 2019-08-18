# Akkatecture Jobs Sample

The akkatecture job sample is the most "hello world" way of doing a scheduled and persistent job.

# Akkatecture.Examples.Jobs

This is the model of the job same which models a basic repeat trigger using `ScheduleRepeatedly<,>`.

### Model
* **PrintJob** - Command that describes the job of printing to console.
* **PrintJobId** - The identifier for the print job.
* **PrintJobResponses** - Message types that are used to describe scheduler success or failure.
* **PrintJobRunner** - The actor which handles the job (`PrintJob`).
* **PrintJobScheduler** - The actor responsible for persisting and scheduling jobs to be triggered and sent to the job runner.

# Akkatecture.Examples.JobApplication

This is the console application that creates the actor system and interfaces with it. First a job manager is created, this job manager is responsible for supervising and creating the job scheduler and job runner actors beneath it, it is also responsible for routing messages between these two actors. Following that, a job is sent to be scheduled to the job scheduler (via the job manager). And then for every 10 seconds a log will be printed to the console showing the job being triggered.

### Description

In this sample, we instantiate the actor system and the various domain entities required for it to work. The domain entity required to 
initialize this, is the `UserAccountAggregateManager`. We then interface with the domain by telling the manager to create user accounts 
by instantiating and telling it a `CreateUserAccountCommand`.

> to run the application in jetbrains rider or visual studio code, run the `Akkatecture.Examples.Application` configuration in the IDE.