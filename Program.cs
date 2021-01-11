using System;
using System.Collections.Concurrent;
using immutableSsd;

namespace deskpi
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            MainClass main = new MainClass();
            main.Run();
        }

        private MainClass()
        {
            gpioHandler = new GpioHandler();

            deskPi = DeskPiBuilder.Create(gpioHandler, events.Enqueue);
        }

        private void Run()
        {
            while (true)
            {
                var millis = gpioHandler.Millis;
                uint? next = deskPi.NextTick(millis);
                if (next.HasValue && next.Value == 0)
                {
                    deskPi = deskPi.Tick(millis);
                }
                if (events.TryDequeue(out object ev))
                {
                    deskPi = deskPi.ReceiveEvent(ev);
                }
            }
        }

        private ConcurrentQueue<object> events = new ConcurrentQueue<object>();
        private DeskPi deskPi;
        private GpioHandler gpioHandler;
    }
}
