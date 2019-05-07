using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Sockets;

namespace Appinsights.TrackAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var returnVal = false;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var result = socket.BeginConnect("microsoft.com", 443, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(3000, false);
                returnVal = socket.Connected;
                if(socket.Connected)
                {
                    socket.Disconnect(true);
                }
            }
                TelemetryConfiguration.Active.InstrumentationKey = "put instrumentation key here";
            
            for (int i = 0; i < 5; i++)
            {
                var telemetryClient = new TelemetryClient();
                
                var availabilityTelemetry = new AvailabilityTelemetry
                {
                    Timestamp = new DateTimeOffset(),
                    Name = "local url test",
                    Success = returnVal,
                    RunLocation = "location" + i, // which location the availability test was run
                    Id = Guid.NewGuid().ToString(), //run id
                    Duration = new TimeSpan(200000) // this is the test duration
                };
                telemetryClient.TrackAvailability(availabilityTelemetry);
            }
            Console.Read();
        }
    }
}
