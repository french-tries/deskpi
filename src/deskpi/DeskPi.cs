using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd;
using piCommon;

namespace deskpi
{
    public class PinValueChangeEvent
    {
        public ButtonId Button { get; }

        public PinValueChangeEvent(ButtonId Button)
        {
            this.Button = Button;
        }
    }

    // todo separate to be able to do concurrency?
    public class DeskPi : ITickable<DeskPi>
    {
        public DeskPi(ISsd stringWriter,
            IButtonAggregator buttonAggregator, DeskPiMode currentMode)
        {
            this.stringWriter = stringWriter;
            this.buttonAggregator = buttonAggregator;
            this.currentMode = currentMode;

            this.stringWriter = Write(this.stringWriter, currentMode.Text);

            Console.WriteLine("Start");
        }

        private DeskPi(DeskPi source,
            ISsd stringWriter = null,
            IButtonAggregator buttonAggregator = null,
            DeskPiMode currentMode = null)
        {
            this.stringWriter = stringWriter ?? source.stringWriter;
            this.buttonAggregator = buttonAggregator ?? source.buttonAggregator;
            this.currentMode = currentMode ?? source.currentMode;
        }

        public DeskPi ReceiveEvent(object ev)
        {
            switch (ev)
            {
                case PinValueChangeEvent pvce:
                    return ReceiveEvent(pvce);
                default:
                    Console.WriteLine($"Unrecognized event {ev}");
                    return this;
            }
        }

        public uint? NextTick(uint currentTime) => 
            PiUtils.Min(
                buttonAggregator.NextTick(currentTime), 
                stringWriter.NextTick(currentTime),
                currentMode.NextTick(currentTime));

        public DeskPi Tick(uint currentTime)
        {
            var buttonAggregatorN = buttonAggregator.Tick(currentTime);
            var currentModeN = currentMode.Tick(currentTime);

            if (buttonAggregator.KeyState != buttonAggregatorN.KeyState)
            {
                currentModeN = currentModeN.ReceiveKey(buttonAggregatorN.KeyState);
            }

            var stringWriterN = currentMode.Text.SequenceEqual(currentModeN.Text) ?
                stringWriter.Tick(currentTime) :
                Write(stringWriter, currentModeN.Text);

            if (buttonAggregator == buttonAggregatorN &&
                currentMode == currentModeN &&
                stringWriter == stringWriterN)
            {
                return this;
            }
            return new DeskPi(this, stringWriterN, buttonAggregatorN,
                currentModeN);
        }

        private DeskPi ReceiveEvent(PinValueChangeEvent pvce)
        {
            return new DeskPi(this, buttonAggregator: buttonAggregator.OnPinValueChange(pvce.Button));
        }

        private static ISsd Write(ISsd stringWriter, ImmutableList<(string, uint)> values)
        {
            if (values.Count == 1)
            {
                return stringWriter.Write(values[0].Item1);
            }
            return stringWriter.Write(values);
        }

        private readonly ISsd stringWriter;
        private readonly IButtonAggregator buttonAggregator;
        private readonly DeskPiMode currentMode;
    }
}
