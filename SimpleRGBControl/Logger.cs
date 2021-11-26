internal class Logger
{
    internal static void LogLine(string message, bool showOnConsole = true, bool showOnFile = true) => Log($"{message}{Environment.NewLine}", showOnConsole);

    internal static void Log(string message, bool showOnConsole = true, bool showOnFile = true)
    {
        FileStream fs = File.Open($"bac-{DateTime.Now:yyyy'.'MM'.'dd}.log", FileMode.Append, FileAccess.Write, FileShare.Read);

        StreamWriter fw = new(fs);

        var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {new string('>', message.Count(x => x == '>') + 1)} {message}";
        if (showOnFile) fw.Write(msg);
        if (showOnConsole) Console.Write(msg);
        fw.Flush();

        // Cleanup
        fw.Close();
        fs.Close();
    }

}