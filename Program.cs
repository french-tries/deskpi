using System;
using immutableSsd.src;

namespace immutableSsd
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

            var directWriter = new Max7219CommonAnodeWriter((obj) => gpioHandler.SpiWrite(obj));

            var converter = new SegmentsConverter();

            var selectorIntHandler = new HackyInterruptHandler(
                () => gpioHandler.Millis, ReceiveInterrupt);

            var selector = new ScrollingSelector<byte>(
                selectorIntHandler, 1000, 2000, 8);

            stringWriter = new StringSsdWriter(directWriter, converter.GetSegments, selector);

            stringWriter = stringWriter.Write("Hello world please work...");
        }

        private void Run()
        {
            while (true) { }
        }

        // TODO if raised before construction is completed, events are lost
        private void ReceiveInterrupt(object caller, uint currentTime)
        {
            if (stringWriter != null)
            {
                stringWriter = stringWriter.ReceiveInterrupt(caller, currentTime);
            }
        }

        private ISsdWriter<string> stringWriter;
    }
}
