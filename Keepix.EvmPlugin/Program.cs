using Keepix.PluginSystem;
using System.Reflection;

namespace PluginProgram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string arg = args.Count() > 0 ? args[0] : "";

            // Example for debuging: (dotnet run)
            // arg = """{ "key": "exposed-functions" }""";

            Task task = KeepixPlugin.Run(arg, Assembly.GetExecutingAssembly().GetTypes());
            task.Wait();
        }
    }
}