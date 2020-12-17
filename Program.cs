using System;
using System.Threading;
using System.Threading.Tasks;
using piCommon;

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

            topLed = (b) => gpioHandler.Write(20, b);
            middleLed = (b) => gpioHandler.Write(16, b);
            bottomLed = (b) => gpioHandler.Write(17, b);

            var topButtonIntHandler = new HackyInterruptHandler(
                () => gpioHandler.Millis, (c, t) => { topButton = topButton.ReceiveInterrupt(c, t); });
            topButton = new ImmutableButton(topButtonIntHandler, () => gpioHandler.Read(4), (b) => topLed(!b));

            var middleButtonIntHandler = new HackyInterruptHandler(
                () => gpioHandler.Millis, (c, t) => { middleButton = middleButton.ReceiveInterrupt(c, t); });
            middleButton = new ImmutableButton(middleButtonIntHandler, () => gpioHandler.Read(3), (b) => middleLed(!b));

            var bottomButtonIntHandler = new HackyInterruptHandler(
               () => gpioHandler.Millis, (c, t) => { bottomButton = bottomButton.ReceiveInterrupt(c, t); });
            bottomButton = new ImmutableButton(bottomButtonIntHandler, () => gpioHandler.Read(2), (b) => bottomLed(!b));

            gpioHandler.RegisterInterruptCallback(4,
                () => { topButton = topButton.OnPinValueChange(); });
            gpioHandler.RegisterInterruptCallback(3,
                () => { middleButton = middleButton.OnPinValueChange(); });
            gpioHandler.RegisterInterruptCallback(2,
                () => { bottomButton = bottomButton.OnPinValueChange(); });
        }

        private void Run()
        {
            topLed(false);
            middleLed(false);
            bottomLed(false);

            stringWriter = stringWriter.Write("Hello world please work...");
           // stringWriter = stringWriter.Write("");

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

        private readonly Action<bool> topLed;
        private readonly Action<bool> middleLed;
        private readonly Action<bool> bottomLed;

        private ImmutableButton topButton;
        private ImmutableButton middleButton;
        private ImmutableButton bottomButton;
    }
}
