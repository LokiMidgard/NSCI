using System;

namespace NSCI.UI
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T arg)
        {
            Argument = arg;
        }

        public T Argument { get; }
    }
}