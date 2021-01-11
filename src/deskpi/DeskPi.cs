using System;
using System.Collections.Generic;
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
            IButtonAggregator buttonAggregator, IDeskPiMode ocarinaSelector)
        {
            this.stringWriter = stringWriter;
            this.buttonAggregator = buttonAggregator;
            this.ocarinaSelector = ocarinaSelector;

            this.stringWriter = Write(this.stringWriter, ocarinaSelector.Text);

            Console.WriteLine("Start");
        }

        private DeskPi(DeskPi source,
            ISsd stringWriter = null,
            IButtonAggregator buttonAggregator = null,
            IDeskPiMode ocarinaSelector = null)
        {
            this.stringWriter = stringWriter ?? source.stringWriter;
            this.buttonAggregator = buttonAggregator ?? source.buttonAggregator;
            this.ocarinaSelector = ocarinaSelector ?? source.ocarinaSelector;
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
                ocarinaSelector.NextTick(currentTime));

        public DeskPi Tick(uint currentTime)
        {
            var buttonAggregatorN = buttonAggregator.Tick(currentTime);
            var ocarinaSelectorN = ocarinaSelector.Tick(currentTime);

            if (buttonAggregator.KeyState != buttonAggregatorN.KeyState)
            {
                ocarinaSelectorN = ocarinaSelectorN.ReceiveKey(buttonAggregatorN.KeyState);
            }

            var stringWriterN = ocarinaSelector.Text.SequenceEqual(ocarinaSelectorN.Text) ?
                stringWriter.Tick(currentTime) :
                Write(stringWriter, ocarinaSelectorN.Text);

            if (buttonAggregator == buttonAggregatorN &&
                ocarinaSelector == ocarinaSelectorN &&
                stringWriter == stringWriterN)
            {
                return this;
            }
            return new DeskPi(this, stringWriterN, buttonAggregatorN,
                ocarinaSelectorN);
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
        private readonly IDeskPiMode ocarinaSelector;
    }
}
