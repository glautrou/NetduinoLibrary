using System;
using Microsoft.SPOT;

namespace Netduino.Components.Sensor
{
    public interface ISensor
    {
        IOutput Output { get; set; }
    }
}
