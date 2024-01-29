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
            return "Given the following source code for a microservice, please fully describe all the resources needed to deploy the service and any other resources that the code may be dependent on. When in doubt, describe it. When the platform is ambiguous, choose AWS. Do not generate IAC, instead describe ALL the resources and integrations with the code that are needed to deploy it.";
        }
    }
}