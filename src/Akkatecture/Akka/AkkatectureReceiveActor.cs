using Akka.Actor;
using Akka.Event;

namespace Akkatecture.Akka
{
    public class AkkatectureReceiveActor : ReceiveActor, IAkkatectureActor
    {
        public ILoggingAdapter Logger { get; }
        
        protected AkkatectureReceiveActor()
        {
            Logger = Context.GetLogger();
        }
    }
}