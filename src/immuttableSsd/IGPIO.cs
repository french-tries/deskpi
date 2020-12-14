using System;
namespace immutableSsd
{
    public interface IGPIO
    {
        void Write(Pin pin, bool active);
    }
}
