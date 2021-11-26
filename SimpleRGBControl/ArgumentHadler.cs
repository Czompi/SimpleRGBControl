internal class ArgumentHadler
{
    private string[] args;
    private IDictionary<string, string> arguments;

    public ArgumentHadler(string[] args)
    {
        this.args = args;
        if (args == null || args.Length == 0) return;
        arguments = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if(args[i].StartsWith("--"))
            {
                if (i + 1 == args.Length) arguments.Add(args[i], "");
                arguments.Add(args[i], args[i + 1]);
                i++;
            }
        }
    }
    public string this[string argName]=> arguments[argName];

    public int Length { get; internal set; }

    internal bool Any() => arguments?.Any() ?? false;
}