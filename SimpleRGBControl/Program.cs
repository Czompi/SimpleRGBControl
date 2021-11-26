using System.Text.Json;
using AuraServiceLib;
public static class SimpleRGBControl
{
    private static IAuraSyncDeviceCollection Devices { get; set; }
    private static bool stopProcess = false;
    public static void Main(string[] _args)
    {
        var args = new ArgumentHadler(_args);
        if (args.Length > 0) LogLine("Application is in unattended mode! All outputs will be saved in the log file.");
        while (!stopProcess)
        {
            LogLine($"Program started");
            IAuraSdk2 sdk = (IAuraSdk2)new AuraSdk();
            LogLine($"SDK casted");
            LogLine($"Take over control");
            sdk.SwitchMode();
            LogLine($"Control taken over");
            LogLine($"Get all devices");
            Devices = sdk.Enumerate(0);
            LogLine($"Got all devices");
            string color;
            if (args.Any())
            {
                color = args["color"];
            } else
            {
                Log("Please enter the desired color: ", showOnFile: false);
                color = Console.ReadLine();
                LogLine($"Please enter the desired color: {color}", showOnConsole: false);
            }
            if (color.Equals("q", StringComparison.OrdinalIgnoreCase)) stopProcess = true;
            else UpdateColor(color);
        }

    }

    private static void LogLine(string message, bool showOnConsole = true, bool showOnFile = true) => Log($"{message}{Environment.NewLine}", showOnConsole);

    private static void Log(string message, bool showOnConsole = true, bool showOnFile = true)
    {
        FileStream fs = File.Open($"bac-{DateTime.Now:yyyy'.'MM'.'dd}.log", FileMode.Append, FileAccess.Write, FileShare.Read);

        StreamWriter fw = new(fs);

        var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {new string('>', message.Count(x => x == '>') + 1)} {message}";
        if(showOnFile)fw.Write(msg);
        if (showOnConsole) Console.Write(msg);
        fw.Flush();

        // Cleanup
        fw.Close();
        fs.Close();
    }

    private static void UpdateColor(string color)
    {
        color = color.ToUpper();
        if (color.Length == 0)
        {
            LogLine("Invalid color!");
            return;
        }
        if (color.Length == 3) color = $"FF{color[0]}{color[0]}{color[1]}{color[1]}{color[2]}{color[2]}";
        else if (color.Length == 4) color = $"{color[0]}{color[0]}{color[1]}{color[1]}{color[2]}{color[2]}{color[3]}{color[3]}";
        else if (color.Length == 6) color = $"FF{color}";
        var fixedColorStr = $"{color[0..2]}{color[6..8]}{color[4..6]}{color[2..4]}";
        uint fixedColor;
        try
        {
            fixedColor = Convert.ToUInt32(fixedColorStr, 16);
        }
        catch (Exception)
        {
            LogLine("Invalid color!");
            return;
        }
        foreach (IAuraSyncDevice dev in Devices)
        {
            LogLine($"{dev.Name} found");
            foreach (IAuraRgbLight light in dev.Lights)
            {
                //Console.WriteLine($"{dev.Name} > AuraRgbLight(Name=\"{light.Name}\",Color=\"{light.Color}\",Red=\"{light.Red}\",Green=\"{light.Green}\",Blue=\"{light.Blue}\") found");
                light.Color = fixedColor;
            }
            LogLine($"{dev.Name} > Apply changes");
            dev.Apply();
            LogLine($"{dev.Name} > Changes applied");
        }
    }
}