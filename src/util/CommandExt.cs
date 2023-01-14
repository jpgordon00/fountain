using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fountain
{
    public static class ActionExtensions
    {
        public static void ForceInvoke(this Action action)
        {
            action?.Invoke();
        }
    }

    public class CommandExt : Command
    {
        public Action? OnHandle;
        public bool Handled = false;

        public CommandExt(string name) : base(name)
        {
        }

        
    }
}