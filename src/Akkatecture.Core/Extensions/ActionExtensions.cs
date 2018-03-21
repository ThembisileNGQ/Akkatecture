using System;

namespace Akkatecture.Extensions
{
    public static class FunctionalBindingExtensions
    {
        public static Action<T2> Bind<T1, T2>(this Action<T1, T2> action, T1 value)
        {
            return openArg => action(value, openArg);
        }
    }
    
}
