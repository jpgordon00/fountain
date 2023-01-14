using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// NOTE: This file needs to be replaced with an updated version that invokes a non-openAI affiliated endpoint.
/// </summary>
namespace fountain
{
    public class OpenAIAPI
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string apiKey = "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";
        private static readonly string model = "text-davinci-003";
        private static readonly float temperature = 0;

        internal static string _OUT = "";

        public static async Task<OpenAIResponse> Generate(string prompt)
        {
            //Console.WriteLine("Generate: " + prompt);
            string json = JsonConvert.SerializeObject(new OpenAIRequest
            {
                prompt = prompt,
                temperature = temperature,
                model = model,
                max_tokens = 50
            });
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.PostAsync(new Uri("https://api.openai.com/v1/completions"),
                new StringContent(json, Encoding.UTF8, "application/json"));
            string responseString = await response.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Remove("Authorization");
            OpenAIResponse resp;
            try {
                resp = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);
                resp.Error = false;
                Console.WriteLine(resp.Text);
                if (!resp.Completed) {
                    _OUT += resp.Text;
                    return await Generate(_OUT); // automatically generate until completion
                } else {
                    resp.choices[0].text = _OUT + resp.choices[0].text;
                    _OUT = "";
                }
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
        public string prompt { get; set; }
        public float temperature { get; set; }
        public string model { get; set; }
        public int max_tokens {get; set;}
    }

    public class OpenAIResponse
    {
        public OpenAIChoices[] choices;
        public bool Completed => choices[0].finish_reason == "stop";
        public string Text => choices[0].text;
        public string Reyes {get; set;}
        public bool Error {get; set;}
        public string ErrorMessage {get; set;}
    }

    public class OpenAIChoices
    {
        public string text {get; set;}
        public string finish_reason {get; set;}
    }
}