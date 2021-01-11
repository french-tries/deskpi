using System;
using piCommon;

namespace deskpi
{
    [Flags]
    public enum ButtonId { Bottom = 1, Middle = 2, Top = 4 }
    public enum KeyId { None, A = 1, B = 2, C = 4, D = 3, E = 6, F = 5, G = 7 }

    public interface IButtonAggregator : ITickable<IButtonAggregator>
    {
        KeyId KeyState { get; }
        IButtonAggregator OnPinValueChange(ButtonId button);
    }
}