using System;
namespace deskpi.src.deskpi
{
    public enum Button { Top, Middle, Bottom }

    public struct TimerEvent
    {
        public object Caller { get; }
    }
    public struct ButtonEvent
    {
        public Button Source { get; }
        public bool Rising { get; }
    }

    public class DeskPi
    {
        public DeskPi()
        {
        }

        public DeskPi ReceiveEvent(object ev)
        {
            switch (ev)
            {
                case TimerEvent te:
                    break;
                case ButtonEvent be:
                    break;
            }
            return this;
        }
    }
}
