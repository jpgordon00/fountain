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
    /// Handles the creation of prompts for "Phase 1" of our SCATTER engine
    /// </summary>
    public class Phase1PromptProcessor : PromptProcessor
    {
        /// <summary>
        /// </summary>
        /// <param name="src"></param>
        public string Process(string src, string extra="")
        {
            return $"Describe the dynamodb resoucres and GSI's needed\n{src}";
        }
    }
}