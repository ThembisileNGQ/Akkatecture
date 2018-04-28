using Akka.Event;

namespace Akkatecture.Akka
{
    public interface IAkkatectureActor
    {
        ILoggingAdapter Logger { get; }
    }
}