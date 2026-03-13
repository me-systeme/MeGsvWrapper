using MeGsvWrapper;

using (var device = new GsvDevice(5))
{
    try
    {
        Console.WriteLine(Environment.Is64BitProcess ? "64-bit process" : "32-bit process");
        Console.WriteLine("=== GSV TEST START ===");
        Console.WriteLine("COM Port: " + device.ComNo);

        Console.WriteLine("\n[1] Open standard...");
        device.Open(32);
        Console.WriteLine("Device opened.");


        Console.WriteLine("\n[2] Read single value...");
        double value = device.ReadValueRetry();
        Console.WriteLine("Single value: " + value);

        Console.WriteLine("\n[3] Read multiple values...");
        double[] values = device.ReadMultiple(100);
        Console.WriteLine("Values read: " + values.Length);
        Console.WriteLine("Multiple values: " + string.Join(", ", values));


        Console.WriteLine("\n[4] Read norm...");
        Console.WriteLine("Norm: " + device.GetNorm());

        Console.WriteLine("\n[5] Read scale...");
        Console.WriteLine("Scale: " + device.GetScale());


        Console.WriteLine("\n[6] Read sampling frequency...");
        Console.WriteLine("Sampling frequency: " + device.GetSamplingFrequency());


        Console.WriteLine("\n[7] Read last error...");
        Console.WriteLine("Last error: " + device.GetLastError());

        Console.WriteLine("\n[8] Flush buffer...");
        device.FlushBuffer();
        Console.WriteLine("Buffer flushed.");

        Console.WriteLine("\n[9] Set zero...");
        device.SetZero();
        Console.WriteLine("Zero set.");


        Console.WriteLine("\n[10] Read value after zero...");
        double valueAfterZero = device.ReadValueRetry();
        Console.WriteLine("Value after zero: " + valueAfterZero);

        Console.WriteLine("\n=== GSV TEST END ===");
    }
    catch (Exception ex)
    {
        Console.WriteLine("TEST ERROR:");
        Console.WriteLine(ex.ToString());
    }
}