using LikwidatorBackend;
using System;
using System.Timers;

public class MockUSBConnection : USBConnection
{
    private Timer timer;

    public MockUSBConnection(string port, int baudRate) : base(port, baudRate)
    {
        // Set up a timer to simulate data reception every second
        timer = new Timer(100); // 1 second interval
        timer.Elapsed += GenerateMockData;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void GenerateMockData(Object source, ElapsedEventArgs e)
    {
        // Simulate telemetry data
        TelemetryData simulatedData = new TelemetryData
        {
            GyroX = GetRandomFloat(-180, 180),
            GyroY = GetRandomFloat(-180, 180),
            GyroZ = GetRandomFloat(-180, 180),
            Altitude = GetRandomFloat(0, 10000),
            VerVel = GetRandomFloat(-50, 50),
            Pitch = GetRandomFloat(-90, 90),
            Roll = GetRandomFloat(-90, 90),
            Heading = GetRandomFloat(0, 360)
        };

        // Invoke the OnDataProcessed event with the simulated data
        //OnDataProcessed?.Invoke(simulatedData);
    }

    private float GetRandomFloat(float min, float max)
    {
        Random random = new Random();
        return (float)(random.NextDouble() * (max - min) + min);
    }

    // Override Dispose if needed
    //public override void Dispose()
    //{
     //   timer.Stop();
     //   timer.Dispose();
     //   base.Dispose();
   // }
}
