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

This sample shows how to print a log statement every 10 seconds that is scheduled by a persisted scheduled job. As configured by the configuration item `akkatecture.job-scheduler.tick-interval = 500ms`, the scheduler will poll itself for any new jobs to be triggered. If a new job is to be trigger, the scheduler will send the job command to the job runner via the parent (`JobManager`). The job manager receives this message and then processes it like any other akka actor would.