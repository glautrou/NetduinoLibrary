using System;
using Microsoft.SPOT;

namespace Netduino.Components.Sensor
{
    public interface IOutput
    {
        void SetData(byte[] data);
    }
}
