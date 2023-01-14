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
    /// Represents a "test" command, AKA boilerplate for future commands.
    /// </summary>
    public class TestCommand : CommandExt
    {
        /// <summary>
        /// Adds a single test option and a dummy handler.
        /// </summary>
        /// <returns></returns>
        public TestCommand() : base("test")
        {
            // define options
            var testOption = new Option<string>
            (
                name: "--test",
                description: "Directory that the source code will be read from",
                getDefaultValue: () => null
            );

            // add options
            Add(testOption);
            
            // set our dummy handler
            this.SetHandler(
                (testOptionValue) =>
                {
                    Console.WriteLine(testOptionValue);
                },
            testOption);
        }
        
    }
}