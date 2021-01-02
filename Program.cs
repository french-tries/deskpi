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
            var gpioHandler = new GpioHandler();

            deskPi = new DeskPi(gpioHandler, events.Enqueue);
        }

        private void Run()
        {
            while (true)
            {
                if (events.TryDequeue(out object ev))
                {
                    deskPi = deskPi.ReceiveEvent(ev);
                }
            }
        }

        private ConcurrentQueue<object> events = new ConcurrentQueue<object>();
        private DeskPi deskPi;
    }
}
