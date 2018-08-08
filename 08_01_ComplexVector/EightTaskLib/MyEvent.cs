using System;

namespace EightTaskLib
{
    public sealed class MyEvent : EventArgs
    {
        public string Msg { get; }

        public MyEvent(string message)
        {
            Msg = message;
        }
    }
}
