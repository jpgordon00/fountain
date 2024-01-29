using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

/// <summary>
/// NOTE: This file needs to be replaced with an updated version that invokes a non-openAI affiliated endpoint.
/// </summary>
namespace fountain
{
    public class OpenAIAPI
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string apiKey = "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";
        private static readonly string model = "gpt-4-0125-preview";
        private static readonly float temperature = 0;

        internal static string _OUT = "";

        public static async Task<OpenAIResponse> Generate(string prompt, string prompt2 = null)
        {
            var messages = new List<OpenAIMessage>
            {
                new OpenAIMessage
                {
                    role = "system",
                    content = "You look at code and describe EVERYTHING someone would need to deploy it, including resource names (if the code specifies) and any dependencies. Given the following source code for a microservice, please fully describe all the resources needed to deploy the service and any other resources that the code may be dependent on. When in doubt, describe it. When the platform is ambiguous, choose AWS. Do not generate IAC, instead describe ALL the resources and integrations with the code that are needed to deploy it. The user will provide you with the source code in the next message. Do this in as little words as possible."
                },
                new OpenAIMessage
                {
                    role = "user",
                    content = prompt
                },
                prompt2 != null ? new OpenAIMessage
                {
                    role = "system",
                    content = prompt2
                } : null
            }.Where(message => message != null).ToArray();

            string json = JsonConvert.SerializeObject(new OpenAIRequest
            {
                messages = messages,
                temperature = temperature,
                model = model,
                max_tokens = 500
            });
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.PostAsync(new Uri("https://api.openai.com/v1/chat/completions"),
                new StringContent(json, Encoding.UTF8, "application/json"));
            string responseString = await response.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Remove("Authorization");
            OpenAIResponse resp;
            try {
                resp = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

                // if not completed by length, force it to continue
                if (resp.choices[0].finish_reason == "length") {
                    Console.WriteLine("Continuing req...");
                    //Console.WriteLine("Continuing request with response: " + JsonConvert.SerializeObject(resp));
                    return await Generate(prompt, resp.Text);
                }

                resp.Error = false;
                Console.WriteLine(JsonConvert.SerializeObject(resp));
                
            } catch (Exception ex) {
                string message = JsonConvert.DeserializeObject<OpenAIErrorResponse>(responseString).error.message;
                return new OpenAIResponse{Error = true, ErrorMessage = message};
            }
            return resp;
        }
    }

    public class OpenAIErrorResponse
    {
        public OpenAIError error {get; set;}
    }

    public class OpenAIError
    {
        public string message;
        public string type;
        public string param;
        public string code;
    }

    public class OpenAIRequest
    {
        public OpenAIMessage[] messages { get; set; }
        public float temperature { get; set; }
        public string model { get; set; }
        public int max_tokens {get; set;}
    }

    public class OpenAIResponse
    {
        public OpenAIChoices[] choices;
        public bool Completed => choices[0].finish_reason == "stop";
        public string Text => choices[0].message.content;
        public string Reyes {get; set;}
        public bool Error {get; set;}
        public string ErrorMessage {get; set;}
    }

    public class OpenAIChoices
    {
        public OpenAIMessage message {get; set;}
        public string finish_reason {get; set;}
    }

    public class OpenAIMessage
    {
        public string content { get; set; }
        public string role { get; set; }
    }
}