using System;
using System.Timers;
using immutableSsd;

namespace immutableSsd
{
    public class HackyInterruptHandler : IInterruptHandler, IDisposable
    {
        public HackyInterruptHandler(Func<uint> millis,
            Action<object, uint> mainReceiver)
        {
            this.millis = millis;
            this.mainReceiver = mainReceiver;
        }

        public void RequestInterrupt(object caller, uint delay)
        {
            this.caller = caller;

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            timer = new Timer(delay)
            {
                AutoReset = false
            };
            timer.Elapsed += Elapsed;
            timer.Start();
        }

        public void UnrequestInterrupt(object caller)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            this.caller = null;
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        private void Elapsed(object source, ElapsedEventArgs e)
        {
            mainReceiver(caller, millis());
        }

        private readonly Func<uint> millis;
        private readonly Action<object, uint> mainReceiver;

        private object caller;
        private Timer timer;
    }
}
