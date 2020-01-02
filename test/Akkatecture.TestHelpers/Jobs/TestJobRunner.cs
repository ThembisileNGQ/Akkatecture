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
using Akka.Actor;
using Akka.Event;
using Akkatecture.Jobs;

namespace Akkatecture.TestHelpers.Jobs
{
    public class TestJobRunner : JobRunner<TestJob, TestJobId>,
        IRun<TestJob>
    {
        public IActorRef ProbeRef { get; }

        public TestJobRunner(
            IActorRef probeRef)
        {
            ProbeRef = probeRef;
        }

        public bool Run(TestJob job)
        {
            var time = Context.System.Settings.System.Scheduler.Now.DateTime;
            ProbeRef.Tell(new TestJobDone(job.Greeting, time));
            Context.GetLogger().Info("JobRunner has processed TestJob.");
            return true;
        }
    }

    public class TestJobDone
    {
        public string Greeting { get; }
        public DateTime At { get; }
        public TestJobDone(
            string greeting,
            DateTime at)
        {
            Greeting = greeting;
            At = at;
        }
    }
}