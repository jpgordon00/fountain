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
    /// Handles the creation and execution of prompts for an AI engine, usually given source code
    /// </summary>
    public interface PromptProcessor
    {
        string Process(string src, string extra="");
    }
}