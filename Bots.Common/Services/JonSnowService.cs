namespace Bots.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Bots.Common.Models;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class JonSnowService : IJonSnowService
    {
        private readonly IHttpService httpService;

        private readonly IConfiguration configuration;

        private string[] responses;

        private int responsesSaved = 0;

        private static readonly Random random = new Random();

        private static readonly int responsesQueueCapacity = 5;

        private static readonly double logScoreFactor = 2;

        private static readonly double scoreThreshold = 0.95;

        private static readonly int kbResponseThreshold = 20;

        public JonSnowService(IConfiguration configuration, IHttpService httpService)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.responses = new string[responsesQueueCapacity];
        }

        public async Task<string> GetResponse(string prompt)
        {
            string response;
            var kbHitCount = 0;

            do
            {
                response = await this.GetResponse(prompt, kbHitCount >= kbResponseThreshold);
                kbHitCount++;
            }
            while (this.GetResponseScore(response) < scoreThreshold);
            
            if (responsesSaved == responsesQueueCapacity)
            {
                this.responses = this.responses.Skip(1).Concat(this.responses.Take(1)).ToArray();
                responsesSaved--;
            }

            this.responses[responsesSaved] = response;
            responsesSaved++;

            return response;
        }

        private double GetResponseScore(string response)
        {
            if (!this.responses.Contains(response))
            {
                return 1.0;
            }

            var score = 0.0;
            for (var i = 0; i < responsesQueueCapacity; i++)
            {
                if (this.responses[i] == response)
                {
                    score += 1 / Math.Pow(logScoreFactor, responsesSaved - i - 1);
                }
            }

            var maxPossibleScore = (Math.Pow(1 / logScoreFactor, responsesSaved) - 1) / ((1 / logScoreFactor) - 1);
            return 1.0 - (score / maxPossibleScore);
        }

        private async Task<string> GetResponse(string prompt, bool forceRandom)
        {
            if (!forceRandom)
            {
                var messageWithoutJonSnow = prompt.Replace("jon snow", string.Empty, StringComparison.InvariantCultureIgnoreCase).Replace("jon", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                var messageWithJonSnow = prompt;

                var knowledgeBaseTask1 = this.httpService.Post(
                    new Uri(this.configuration["KnowledgeBaseEndpoint"]),
                    $"{{\"question\":\"{messageWithJonSnow}\",\"top\":5}}",
                    "application/json",
                    new Dictionary<string, string> { { "Authorization", $"EndpointKey {this.configuration["KnowledgeBaseEndpointKey"]}" } });

                var knowledgeBaseTask2 = this.httpService.Post(
                    new Uri(this.configuration["KnowledgeBaseEndpoint"]),
                    $"{{\"question\":\"{messageWithoutJonSnow}\",\"top\":5}}",
                    "application/json",
                    new Dictionary<string, string> { { "Authorization", $"EndpointKey {this.configuration["KnowledgeBaseEndpointKey"]}" } });

                var apiTasks = new List<Task>
                {
                    knowledgeBaseTask1,
                    knowledgeBaseTask2
                };

                await Task.WhenAll(apiTasks);
                var knowledgeBaseResponse1 = knowledgeBaseTask1.Result;
                var knowledgeBaseResponse2 = knowledgeBaseTask2.Result;

                if (knowledgeBaseResponse1.IsSuccessStatusCode && knowledgeBaseResponse2.IsSuccessStatusCode)
                {
                    var responseContent1 = JsonConvert.DeserializeObject<KnowledgeBaseResponse>(knowledgeBaseResponse1.Content);
                    var responseContent2 = JsonConvert.DeserializeObject<KnowledgeBaseResponse>(knowledgeBaseResponse2.Content);
                    var allAnswers = new List<KnowledgeBaseAnswer>(responseContent1.Answers);
                    allAnswers.AddRange(responseContent2.Answers);
                    allAnswers.Sort(new KnowledgeBaseAnswerComparerByScore());

                    if (allAnswers.Any())
                    {
                        var highestScore = allAnswers.First().Score;
                        if (random.NextDouble() * 100.0 <= highestScore)
                        {
                            return allAnswers.First().Answer;
                        }
                    }
                }
            }

            var allQuotes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("Resources/jonsnowquotes.json"));
            return allQuotes[random.Next(0, allQuotes.Count)];
        }
    }
}
