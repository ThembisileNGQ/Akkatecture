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

namespace Akkatecture.Jobs.Commands
{
    
    public sealed class ScheduleRepeatedly<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TimeSpan Interval { get; }

        public ScheduleRepeatedly(
            TIdentity jobId,
            TJob job,
            TimeSpan interval,
            DateTime triggerDate,
            object ack = null,
            object nack = null)
            : base(jobId, job, triggerDate, ack, nack)
        {
            if(interval == default) throw new ArgumentException(nameof(interval));
            
            Interval = interval;
        }
        
        public override Schedule<TJob,TIdentity> WithNextTriggerDate(DateTime utcDate) => new ScheduleRepeatedly<TJob,TIdentity>(JobId, Job, Interval, TriggerDate + Interval);
        
        public override Schedule<TJob,TIdentity> WithAck(object ack)
        {
            return new ScheduleRepeatedly<TJob, TIdentity>(JobId, Job, Interval, TriggerDate, ack, Nack);
        }
        
        public override Schedule<TJob,TIdentity> WithNack(object nack)
        {
            return new ScheduleRepeatedly<TJob, TIdentity>(JobId, Job, Interval, TriggerDate, Ack, nack);
        }
    }
}