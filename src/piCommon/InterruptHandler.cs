using System;
using System.Threading;
using System.Threading.Tasks;

namespace piCommon
{
    public static class InterruptHandler
    {
        public static Func<object, uint, Action> RequestInterrupt(Action<object> oncomplete) =>
            (caller, delay) =>
            {
                var source = new CancellationTokenSource();
                var task = InterruptTask(caller, delay, oncomplete, source.Token);

                return () => { Console.WriteLine("cancelling"); source.Cancel(); };
            };

        private static async Task InterruptTask(object caller, uint delay,
            Action<object> mainReceiver, CancellationToken token)
        {
            await Task.Delay((int)delay, token);
            mainReceiver?.Invoke(caller);
        }
    }
}