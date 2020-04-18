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

        private static readonly Random random = new Random();

        public JonSnowService(IConfiguration configuration, IHttpService httpService)
        {
            this.configuration = configuration;
            this.httpService = httpService;
        }

        public async Task<string> GetResponse(string prompt)
        {
            var messageWithoutJonSnow = prompt.Replace("jon snow", string.Empty, StringComparison.InvariantCultureIgnoreCase).Replace("jon", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var messageWithJonSnow = prompt;

            var knowledgeBaseResponse1 = await this.httpService.Post(
                new Uri(this.configuration["KnowledgeBaseEndpoint"]),
                $"{{\"question\":\"{messageWithJonSnow}\",\"top\":5}}",
                "application/json",
                new Dictionary<string, string> { { "Authorization", $"EndpointKey {this.configuration["KnowledgeBaseEndpointKey"]}" } });

            var knowledgeBaseResponse2 = await this.httpService.Post(
                new Uri(this.configuration["KnowledgeBaseEndpoint"]),
                $"{{\"question\":\"{messageWithoutJonSnow}\",\"top\":5}}",
                "application/json",
                new Dictionary<string, string> { { "Authorization", $"EndpointKey {this.configuration["KnowledgeBaseEndpointKey"]}" } });

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

            var allQuotes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("Resources/jonsnowquotes.json"));
            return allQuotes[random.Next(0, allQuotes.Count)];
        }
    }
}
