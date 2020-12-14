using System;
using immutableSsd;

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

            var topPin = new Pin(2, true);
            topLed = (b) => gpioHandler.Write(topPin, b);

            var middlePin = new Pin(16, true);
            middleLed = (b) => gpioHandler.Write(middlePin, b);

            var bottomPin = new Pin(20, true);
            bottomLed = (b) => gpioHandler.Write(bottomPin, b);
        }

        private void Run()
        {
            topLed(false);
            middleLed(false);
            bottomLed(false);

            //stringWriter = stringWriter.Write("Hello world please work...");
            stringWriter = stringWriter.Write("");

            while (true) { }
        }

        // TODO if raised before construction is completed, events are lost
        // TODO stop / restart program ?
        private void ReceiveInterrupt(object caller, uint currentTime)
        {
            if (stringWriter != null)
            {
                stringWriter = stringWriter.ReceiveInterrupt(caller, currentTime);
            }
        }

        private ISsdWriter<string> stringWriter;

        private Action<bool> topLed;
        private Action<bool> middleLed;
        private Action<bool> bottomLed;
    }
}
