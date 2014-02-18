// Logging code lifted from HydroDesktop Application

using System;

namespace SDR.Common.logging
{
    interface ILogInitializer : IDisposable
    {
        string Destination { get; }
    }
}