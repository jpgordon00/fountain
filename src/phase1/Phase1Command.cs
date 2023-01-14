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
    /// Represents "Phase 1" of the AI engine, which converts source code to psudo IAC (English representation of the Terraform code that is to be generated in Phase2).
    /// </summary>
    public class Phase1Command : CommandExt
    {
        /// <summary>
        /// Adds a single test option and a dummy handler.
        /// </summary>
        /// <returns></returns>
        public Phase1Command() : base("phase1")
        {
            // define options
            var srcOption = new Option<string>
            (
                name: "--src",
                description : "The raw text to feed the AI model. Use --congolomorate if not using raw text",
                getDefaultValue: () => null
            );

            // add options
            Add(srcOption);
            
            var cc = new ConglomerateCommand();
            cc.OnHandle += async (string src) => {
                Entrypoint.Tasks.Add(_Handle(src));
            };
            Add(cc);
            // set our dummy handler
            this.SetHandler(
                async (src) =>
                {
                    if (!Handled && src == null)
                    {
                        // assume conglomorate was not invoked
                        // TODO: remove without --srcdir option
                        cc.Invoke("--srcdir C:\\Users\\Jacob\\Desktop\\Waterfountain\\training_data\\python-lambda-helloworld");
                    } else if (src != null) {
                        //_Handle(src);
                    }
                },
            srcOption);
        }

        public async Task _Handle(string src)
        {
            if (Handled) return;
            Handled = true;
            Task<OpenAIResponse> resp = OpenAIAPI.Generate(new Phase1PromptProcessor().Process(src));
            Entrypoint.Tasks.Add(resp); // queue up tasks to ensure proper execution
            var respo = await resp;
            if (respo.Error) {
                Console.WriteLine("------------ Phase 1 Error --------------------");
                Console.WriteLine(respo.ErrorMessage);
            } else {
                Console.WriteLine("---------------------- Phase 1 ---------------------- ");
                Console.WriteLine(respo.Text.Trim());
            }
        }
        
    }
}