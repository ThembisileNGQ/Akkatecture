// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akkatecture.Examples.Jobs;
using Akkatecture.Jobs;
using Akkatecture.Jobs.Commands;
using static Akkatecture.Examples.Jobs.PrintJobSchedulerResponses;

namespace Akkatecture.Examples.JobApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Create our actor system and JobManager for the print job 
            var actorSystem = ActorSystem.Create("print-job-system", Configuration.Config);

            var jobManager = actorSystem.ActorOf(
                Props.Create(
                    () => new JobManager<PrintJobScheduler, PrintJobRunner, PrintJob, PrintJobId>(
                        (() => new PrintJobScheduler()),
                        () => new PrintJobRunner())));

            var jobId = PrintJobId.New;
            var job = new PrintJob("repeated job");
            var interval = TimeSpan.FromSeconds(10);
            var when = DateTime.UtcNow;
            
            //Create a message that will Ack with [Success] and will fail with [Failed]
            var scheduleMessage = new ScheduleRepeatedly<PrintJob, PrintJobId>(jobId, job, interval, when)
                .WithAck(Success.Instance)
                .WithNack(Failed.Instance);

            var scheduled = await jobManager.Ask<PrintJobResponse>(scheduleMessage);

            var result = scheduled
                .Match()
                .With<Success>(x => Console.WriteLine("Job was Scheduled"))
                .With<Failed>(x => Console.WriteLine("Job was NOT scheduled."))
                .WasHandled;

            Console.ReadLine();
        }
    }
}