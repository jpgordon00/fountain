using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fountain
{
    /// <summary>
    /// Entrypoint for our CLI.
    /// </summary>
    public class Entrypoint
    {
        protected static RootCommand ROOT_COMMAND = new RootCommand();
        protected const string ROOT_COMMAND_NAME = "fountain";
        protected const string VERSION = "alpha-1.0.0";

        public static string[] args;

        public static List<Task> Tasks = new List<Task>();

        public static async Task<int> Main(string[] _args)
        {
            args = _args;
            List<CommandExt> commands = new List<CommandExt>();
            commands.Add(new ConglomerateCommand());
            commands.Add(new TerraformCommand());
            commands.Add(new Phase1Command());
            foreach (var cmd in commands) ROOT_COMMAND.Add(cmd);

            // standard return response
            await ROOT_COMMAND.InvokeAsync(_args);
            await Task.WhenAll(Tasks);
            return 0;
        }
    }
}
