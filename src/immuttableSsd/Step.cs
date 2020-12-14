using System;
namespace immutableSsd
{
    public class Step
    {
    }

    public class WriteStep : Step
    {
        public WriteStep(Pin pin, bool value)
        {
            Pin = pin;
            Value = value;
        }

        public Pin Pin { get; }
        public bool Value { get; }
    }

    public delegate void StepApplier(Step step);
}
