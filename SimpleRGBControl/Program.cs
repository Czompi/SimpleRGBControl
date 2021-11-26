using System.Text.Json;
using AuraServiceLib;
public static class SimpleRGBControl
{
    private static IAuraSyncDeviceCollection Devices { get; set; }
    private static bool stopProcess = false;
    public static void Main(string[] _args)
    {
        var args = new ArgumentHadler(_args);
        if (args.Length > 0) Logger.LogLine("Application is in unattended mode! All outputs will be saved in the log file.");
        while (!stopProcess)
        {
            Logger.LogLine($"Program started");
            IAuraSdk2 sdk = (IAuraSdk2)new AuraSdk();
            Logger.LogLine($"SDK casted");
            Logger.LogLine($"Take over control");
            sdk.SwitchMode();
            Logger.LogLine($"Control taken over");
            Logger.LogLine($"Get all devices");
            Devices = sdk.Enumerate(0);
            Logger.LogLine($"Got all devices");
            string color;
            if (args.Any())
            {
                color = args["color"];
            } else
            {
                Logger.Log("Please enter the desired color: ", showOnFile: false);
                color = Console.ReadLine();
                Logger.LogLine($"Please enter the desired color: {color}", showOnConsole: false);
            }
            if (color.Equals("q", StringComparison.OrdinalIgnoreCase)) stopProcess = true;
            else UpdateColor(color);
        }

    }

    private static void UpdateColor(string color)
    {
        color = color.ToUpper();
        if (color.Length == 0)
        {
            Logger.LogLine("Invalid color!");
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
            Logger.LogLine("Invalid color!");
            return;
        }
        foreach (IAuraSyncDevice dev in Devices)
        {
            Logger.LogLine($"{dev.Name} found");
            foreach (IAuraRgbLight light in dev.Lights)
            {
                light.Color = fixedColor;
            }
            Logger.LogLine($"{dev.Name} > Apply changes");
            dev.Apply();
            Logger.LogLine($"{dev.Name} > Changes applied");
        }
    }
}